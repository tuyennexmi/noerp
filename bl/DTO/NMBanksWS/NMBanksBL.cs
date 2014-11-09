// NMBanksBL.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMBanksBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMBanksBL()
        {
            
        }

        public NMBanksWSI callSingleBL(NMBanksWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMBanksWSI> callListBL(NMBanksWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMBanksWSI>();
            }
        }

        private NMBanksWSI SelectObject(NMBanksWSI wsi)
        {
            BanksAccesser Accesser = new BanksAccesser(Session);
            AreasAccesser AreaAccesser = new AreasAccesser(Session);
            Banks obj;
            obj = Accesser.GetAllBanksByID(wsi.Bank.Id, true);
            if (obj != null)
            {
                wsi.Bank = obj;
                wsi.Area = AreaAccesser.GetAllAreasByID(obj.AreaId, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMBanksWSI SaveObject(NMBanksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                BanksAccesser Accesser = new BanksAccesser(Session);
                Banks obj = Accesser.GetAllBanksByID(wsi.Bank.Id, true);
                if (obj != null)
                {
                    Session.Evict(obj);
                    wsi.Bank.CreatedDate = obj.CreatedDate;
                    wsi.Bank.CreatedBy = obj.CreatedBy;
                    Accesser.UpdateBanks(wsi.Bank);
                }
                else
                {
                    wsi.Bank.Id = AutomaticValueAccesser.AutoGenerateId("Banks");
                    wsi.Bank.CreatedDate = DateTime.Now;
                    wsi.Bank.CreatedBy = wsi.ActionBy;
                    Accesser.InsertBanks(wsi.Bank);
                }
                wsi.WsiError = "";
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        private NMBanksWSI DeleteObject(NMBanksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                BanksAccesser Accesser = new BanksAccesser(Session);
                Banks obj = Accesser.GetAllBanksByID(wsi.Bank.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteBanks(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không thể xóa.";
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";   
                }
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        private List<NMBanksWSI> SearchObject(NMBanksWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {
                if (string.IsNullOrEmpty(wsi.ToDate))
                {
                    wsi.ToDate = DateTime.Parse(wsi.FromDate).AddDays(1).ToString();
                }
                else
                {
                    wsi.ToDate = DateTime.Parse(wsi.ToDate).AddDays(1).ToString();
                }
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedDate >= :FromDate AND O.CreatedDate < :ToDate";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Id LIKE :Keyword OR O.Name LIKE :Keyword OR O.Address LIKE :Keyword)";
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Banks AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Id ASC";
            }
            string strCmd = "SELECT O FROM Banks AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMBanksWSI> ListWSI = new List<NMBanksWSI>();
            BanksAccesser Accesser = new BanksAccesser(Session);
            AreasAccesser AreaAccesser = new AreasAccesser(Session);
            IList<Banks> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetBanksByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetBanksByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Banks obj in objs)
            {
                wsi = new NMBanksWSI();
                wsi.Bank = obj;
                wsi.Area = AreaAccesser.GetAllAreasByID(obj.AreaId, true);
                wsi.WsiError = "";
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }
    }
}
