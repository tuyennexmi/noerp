// NMStatusesBL.cs

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

namespace NEXMI
{
    public class NMStatusesBL
    {
        private ISession Session = SessionFactory.GetNewSession();
        public NMStatusesBL()
        {

        }

        public NMStatusesWSI callSingleBL(NMStatusesWSI wsi)
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

        public List<NMStatusesWSI> callListBL(NMStatusesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMStatusesWSI>();
            }
        }

        public NMStatusesWSI SelectObject(NMStatusesWSI wsi)
        {
            StatusesAccesser Acceser = new StatusesAccesser(Session);
            Statuses obj;
            obj = Acceser.GetAllStatusesByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Status = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMStatusesWSI SaveObject(NMStatusesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                StatusesAccesser Accesser = new StatusesAccesser(Session);
                Statuses obj = Accesser.GetAllStatusesByID(wsi.Status.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);
                    Accesser.UpdateStatuses(wsi.Status);
                }
                else
                {
                    wsi.Status.Id = AutomaticValueAccesser.AutoGenerateId("Statuses");
                    Accesser.InsertStatuses(wsi.Status);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMStatusesWSI DeleteObject(NMStatusesWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                StatusesAccesser Accesser = new StatusesAccesser(Session);
                Statuses obj = Accesser.GetAllStatusesByID(wsi.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteStatuses(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public List<NMStatusesWSI> SearchObject(NMStatusesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Status != null)
            {
                if (!string.IsNullOrEmpty(wsi.Status.ObjectName))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ObjectName = :ObjectName";
                    ListCriteria.Add("ObjectName", wsi.Status.ObjectName);
                }
            }
            String strCmd = "SELECT O FROM Statuses AS O" + strCriteria + " ORDER BY O.OrdinalNumber ASC";
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMStatusesWSI> ListWSI = new List<NMStatusesWSI>();
            StatusesAccesser Accesser = new StatusesAccesser(Session);
            IList<Statuses> objs;
            objs = Accesser.GetStatusesByQuery(query, false);
            foreach (Statuses obj in objs)
            {
                wsi = new NMStatusesWSI();
                wsi.Id = obj.Id;
                wsi.Status = obj;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
