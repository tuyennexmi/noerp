// NMTaskDetailsBL.cs

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
    public class NMTaskDetailsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMTaskDetailsBL()
        {

        }

        public NMTaskDetailsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMTaskDetailsWSI callSingleBL(NMTaskDetailsWSI wsi)
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

        public List<NMTaskDetailsWSI> callListBL(NMTaskDetailsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMTaskDetailsWSI>();
            }
        }

        public NMTaskDetailsWSI SelectObject(NMTaskDetailsWSI wsi)
        {
            TaskDetailsAccesser Accesser = new  TaskDetailsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            
            TaskDetails obj;
            obj = Accesser.GetAllTaskDetailsByID(wsi.TaskDetail.OrdinalNumber.ToString(), false);
            if (obj != null)
            {
                wsi.TaskDetail = obj;
                wsi.User = CustomerAccesser.GetAllCustomersByID(obj.UserId, true);
                wsi.CheckedBy = CustomerAccesser.GetAllCustomersByID(obj.CheckedBy, true);
                wsi.Manager = CustomerAccesser.GetAllCustomersByID(obj.Manager, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMTaskDetailsWSI SaveObject(NMTaskDetailsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                TaskDetailsAccesser Accesser = new TaskDetailsAccesser(Session);
                TaskDetailsAccesser DetailAccesser = new TaskDetailsAccesser(Session);
                TaskDetails obj = Accesser.GetAllTaskDetailsByID(wsi.TaskDetail.OrdinalNumber.ToString(), false);
                
                if (obj != null)
                {   
                    Session.Evict(obj);
                    wsi.TaskDetail.CreatedDate = obj.CreatedDate;
                    wsi.TaskDetail.CreatedBy = obj.CreatedBy;
                    wsi.TaskDetail.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.TaskDetail.ModifiedDate = DateTime.Now;

                    Accesser.UpdateTaskDetail(wsi.TaskDetail);
                }
                else
                {   
                    wsi.TaskDetail.CreatedDate = DateTime.Now;
                    wsi.TaskDetail.CreatedBy = wsi.Filter.ActionBy;
                    wsi.TaskDetail.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.TaskDetail.ModifiedDate = DateTime.Now;

                    Accesser.InsertTaskDetail(wsi.TaskDetail);
                }
                
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMTaskDetailsWSI DeleteObject(NMTaskDetailsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                TaskDetailsAccesser Accesser = new TaskDetailsAccesser(Session);
                TaskDetails obj = Accesser.GetAllTaskDetailsByID(wsi.TaskDetail.OrdinalNumber.ToString(), true);
                if (obj != null)
                {
                    Accesser.DeleteTaskDetail(obj);
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
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMTaskDetailsWSI> SearchObject(NMTaskDetailsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.TaskDetail != null)
            {
                if (!string.IsNullOrEmpty(wsi.TaskDetail.TaskId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TaskId = :TaskId";
                    ListCriteria.Add("TaskId", wsi.TaskDetail.TaskId);
                }
                if (!string.IsNullOrEmpty(wsi.TaskDetail.UserId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.UserId = :UserId";
                    ListCriteria.Add("UserId", wsi.TaskDetail.UserId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.Filter.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.Filter.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Filter.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.TaskId LIKE :Keyword OR O.TaskName LIKE :Keyword OR O.ProjectId in (SELECT P.ProjectId FROM Projects AS P WHERE P.ProjectId LIKE :Keyword OR P.ProjectName LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Filter.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.TaskId) FROM Tasks AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.TaskId DESC";
            }
            String strCmd = "SELECT O FROM Tasks AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMTaskDetailsWSI> ListWSI = new List<NMTaskDetailsWSI>();
            TaskDetailsAccesser Accesser = new TaskDetailsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            
            IList<TaskDetails> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.Filter.PageSize != 0)
            {
                objs = Accesser.GetTaskDetailsByQuery(query, wsi.Filter.PageSize, wsi.Filter.PageNum, false);
            }
            else
            {
                objs = Accesser.GetTaskDetailsByQuery(query, false);
            }
            if (wsi.Filter.Limit != 0)
            {
                objs = objs.Take(wsi.Filter.Limit).ToList();
            }
            foreach (TaskDetails obj in objs)
            {
                wsi = new NMTaskDetailsWSI();
                wsi.TaskDetail = obj;
                
                wsi.User = CustomerAccesser.GetAllCustomersByID(obj.UserId, true);
                
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
