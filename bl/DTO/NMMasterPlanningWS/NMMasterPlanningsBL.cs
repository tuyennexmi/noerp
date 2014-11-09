// NMMasterPlanningsBL.cs

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
using Iesi.Collections;

namespace NEXMI
{
    public class NMMasterPlanningsBL
    {
        private readonly ISession Session;

        public NMMasterPlanningsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMMasterPlanningsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMMasterPlanningsWSI callSingleBL(NMMasterPlanningsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                case "CLO_MON":
                    //return CloseInventory(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMMasterPlanningsWSI> callListBL(NMMasterPlanningsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMMasterPlanningsWSI>();
            }
        }

        private NMMasterPlanningsWSI SaveObject(NMMasterPlanningsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {   
                MasterPlanningsAccesser PlanningAccesser = new MasterPlanningsAccesser(this.Session);                
                List<MasterPlanningDetails> OldDetails = null;

                MasterPlannings obj = PlanningAccesser.GetAllMasterPlanningsByID(wsi.Planning.Id, false);
                if (obj == null)
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Planning.Id = AutomaticValueAccesser.AutoGenerateId("MasterPlannings");
                    wsi.Planning.CreatedDate = DateTime.Now;                    
                    wsi.Planning.CreatedBy = wsi.Filter.ActionBy;
                    wsi.Planning.ModifiedDate = DateTime.Now;
                    wsi.Planning.ModifiedBy = wsi.Filter.ActionBy;
                    foreach (MasterPlanningDetails itm in wsi.Planning.Details)
                    {
                        itm.MasterId = wsi.Planning.Id;
                        itm.CreatedBy = wsi.Filter.ActionBy;
                        itm.CreatedDate = DateTime.Now;
                        itm.ModifiedBy = wsi.Filter.ActionBy;
                        itm.ModifiedDate = DateTime.Now;
                    }
                    PlanningAccesser.InsertMasterPlannings(wsi.Planning);
                    NMMessagesBL.SaveMessage(Session, wsi.Planning.Id, "khởi tạo tài liệu", "", wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    OldDetails = obj.Details.Cast<MasterPlanningDetails>().ToList();
                    Session.Evict(obj);
                    wsi.Planning.CreatedDate = obj.CreatedDate;
                    wsi.Planning.CreatedBy = obj.CreatedBy;
                    wsi.Planning.ModifiedDate = DateTime.Now;
                    wsi.Planning.ModifiedBy = wsi.Filter.ActionBy;
                    foreach (MasterPlanningDetails itm in wsi.Planning.Details)
                    {
                        itm.MasterId = wsi.Planning.Id;
                        itm.CreatedBy = itm.CreatedBy;
                        itm.CreatedDate = itm.CreatedDate;
                        itm.ModifiedBy = wsi.Filter.ActionBy;
                        itm.ModifiedDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Planning.Id, "cập nhật thông tin", rs, wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    PlanningAccesser.UpdateMasterPlannings(wsi.Planning);
                }

                MasterPlanningDetailsAccesser DetailAccesser = new MasterPlanningDetailsAccesser(this.Session);
                
                if (OldDetails != null)
                {
                    bool flag = true;
                    foreach (MasterPlanningDetails Old in OldDetails)
                    {
                        flag = true;
                        foreach (MasterPlanningDetails New in wsi.Planning.Details)
                        {
                            if (Old.Id == New.Id)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeleteMasterPlanningDetails(Old);
                        }
                    }
                }

                tx.Commit();
            }
            catch(Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }

            return wsi;
        }

        private NMMasterPlanningsWSI SelectObject(NMMasterPlanningsWSI wsi)
        {
            MasterPlanningsAccesser Accesser = new MasterPlanningsAccesser(Session);
            MasterPlannings obj;
            obj = Accesser.GetAllMasterPlanningsByID(wsi.Planning.Id, false);
            if (obj != null)
            {
                wsi.Planning = obj;
                //wsi.Details = 
                    obj.Details.Cast<MasterPlanningDetails>().ToList();
                wsi.WsiError = "";
                Session.Evict(obj);
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMMasterPlanningsWSI DeleteObject(NMMasterPlanningsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            MasterPlanningsAccesser Accesser = new MasterPlanningsAccesser(Session);
            MasterPlannings obj;
            obj = Accesser.GetAllMasterPlanningsByID(wsi.Planning.Id, true);
            if (obj != null)
            {
                Accesser.DeleteMasterPlannings(obj);
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
            return wsi;
        }

        private List<NMMasterPlanningsWSI> SearchObject(NMMasterPlanningsWSI wsi)
        {
            List<NMMasterPlanningsWSI> ListWSI = new List<NMMasterPlanningsWSI>();
            try
            {
                Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
                String strCriteria = "";
                if (wsi.Planning != null)
                {
                    if (!String.IsNullOrEmpty(wsi.Planning.Status))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.Status = :Status";
                        ListCriteria.Add("Status", wsi.Planning.Status);
                    }
                }
                if (!string.IsNullOrEmpty(wsi.Filter.FromDate))
                {
                    if (string.IsNullOrEmpty(wsi.Filter.ToDate))
                    {
                        wsi.Filter.ToDate = DateTime.Parse(wsi.Filter.FromDate).AddDays(1).ToString();
                    }
                    else
                    {
                        wsi.Filter.ToDate = DateTime.Parse(wsi.Filter.ToDate).AddDays(1).ToString();
                    }
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.BeginDate >= :FromDate AND O.BeginDate < :ToDate";
                    ListCriteria.Add("FromDate", DateTime.Parse(wsi.Filter.FromDate));
                    ListCriteria.Add("ToDate", DateTime.Parse(wsi.Filter.ToDate));
                }

                if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
                {
                    strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
                }
                else
                {
                    strCriteria += " ORDER BY O.BeginDate";
                }
                
                String strCmd = "SELECT O FROM MasterPlannings AS O" + strCriteria;
                IQuery query = Session.CreateQuery(strCmd);
                foreach (var Item in ListCriteria)
                {
                    query.SetParameter(Item.Key, Item.Value);
                }
                MasterPlanningsAccesser Accesser = new MasterPlanningsAccesser(Session);
                IList<MasterPlannings> objs;
                objs = Accesser.GetMasterPlanningsByQuery(query, false);
                foreach (MasterPlannings obj in objs)
                {
                    wsi = new NMMasterPlanningsWSI();
                    wsi.Planning = obj;
                    ListWSI.Add(wsi);
                }
            }
            catch { }
            return ListWSI;
        }
    }
    
}
