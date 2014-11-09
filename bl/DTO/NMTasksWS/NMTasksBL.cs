// NMTasksBL.cs

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
    public class NMTasksBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMTasksBL()
        {

        }

        public NMTasksWSI callSingleBL(NMTasksWSI wsi)
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

        public List<NMTasksWSI> callListBL(NMTasksWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMTasksWSI>();
            }
        }

        public NMTasksWSI SelectObject(NMTasksWSI wsi)
        {
            TasksAccesser Accesser = new TasksAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            DailyReportsAccesser DRAcc = new DailyReportsAccesser(Session);

            Tasks obj;
            obj = Accesser.GetAllTasksByID(wsi.Task.TaskId, false);
            if (obj != null)
            {
                wsi.Task = obj;
                wsi.Details = obj.TaskDetailList.Cast<TaskDetails>().ToList();
                wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, true);
                wsi.AssignedUser = CustomerAccesser.GetAllCustomersByID(obj.AssignedTo, true);
                wsi.CheckedBy = CustomerAccesser.GetAllCustomersByID(obj.CheckedBy, true);
                wsi.Manager = CustomerAccesser.GetAllCustomersByID(obj.Manager, true);
                wsi.Stage = StageAccesser.GetAllStagesByID(obj.StageId, true);
                wsi.Priority = ParameterAccesser.GetAllParametersByID(obj.Priority, true);
                wsi.ReportPeriod = ParameterAccesser.GetAllParametersByID(obj.ReportPeriod, true);
                wsi.Reports = DRAcc.GetAllDailyReportsByTaskId(wsi.Task.TaskId, true).ToList();

                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMTasksWSI SaveObject(NMTasksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                TasksAccesser Accesser = new TasksAccesser(Session);
                TaskDetailsAccesser DetailAccesser = new TaskDetailsAccesser(Session);
                Tasks obj = Accesser.GetAllTasksByID(wsi.Task.TaskId, false);
                List<TaskDetails> OldDetails = null;
                TaskDetails Detail;
                string rs = "";
                if (obj != null)
                {
                    try
                    {
                        OldDetails = obj.TaskDetailList.Cast<TaskDetails>().ToList();
                    }
                    catch { }
                    Session.Evict(obj);
                    wsi.Task.CreatedDate = obj.CreatedDate;
                    wsi.Task.CreatedBy = obj.CreatedBy;
                    wsi.Task.ModifiedBy = wsi.ActionBy;
                    wsi.Task.ModifiedDate = DateTime.Now;

                    Accesser.UpdateTask(wsi.Task);
                    if (rs != "")
                        NMCommon.SaveMessage(Session, wsi.Task.TaskId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    wsi.Task.TaskId = AutomaticValueAccesser.AutoGenerateId("Tasks");
                    wsi.Task.CreatedDate = DateTime.Now;
                    wsi.Task.CreatedBy = wsi.ActionBy;
                    wsi.Task.ModifiedBy = wsi.ActionBy;
                    wsi.Task.ModifiedDate = DateTime.Now;

                    Accesser.InsertTask(wsi.Task);
                    NMCommon.SaveMessage(Session, wsi.Task.TaskId, "khởi tạo đối tượng", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                if (wsi.Details != null)
                {
                    foreach (TaskDetails Item in wsi.Details)
                    {
                        Detail = DetailAccesser.GetAllTaskDetailsByID(Item.OrdinalNumber.ToString(), true);
                        if (Detail != null)
                        {
                            Item.TaskId = wsi.Task.TaskId;
                            Item.CreatedBy = Detail.CreatedBy;
                            Item.CreatedDate = Detail.CreatedDate;
                            Item.ModifiedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;
                            DetailAccesser.UpdateTaskDetail(Item);
                        }
                        else
                        {
                            Item.TaskId = wsi.Task.TaskId;
                            Item.CreatedBy = wsi.ActionBy;
                            Item.CreatedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;
                            DetailAccesser.InsertTaskDetail(Item);
                        }
                    }
                    if (OldDetails != null)
                    {
                        bool flag = true;
                        foreach (TaskDetails Old in OldDetails)
                        {
                            flag = true;
                            foreach (TaskDetails New in wsi.Details)
                            {
                                if (Old.OrdinalNumber == New.OrdinalNumber)
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                DetailAccesser.DeleteTaskDetail(Old);
                            }
                        }
                    }
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

        public NMTasksWSI DeleteObject(NMTasksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                TasksAccesser Accesser = new TasksAccesser(Session);
                Tasks obj = Accesser.GetAllTasksByID(wsi.Task.TaskId, true);
                if (obj != null)
                {
                    Accesser.DeleteTask(obj);
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

        public List<NMTasksWSI> SearchObject(NMTasksWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Task != null)
            {
                if (!string.IsNullOrEmpty(wsi.Task.AssignedTo))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.AssignedTo = :AssignedTo";
                    ListCriteria.Add("AssignedTo", wsi.Task.AssignedTo);
                }
                if (!string.IsNullOrEmpty(wsi.Task.StatusId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StatusId = :StatusId";
                    ListCriteria.Add("StatusId", wsi.Task.StatusId);
                }
                if (!string.IsNullOrEmpty(wsi.Task.StageId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StageId = :StageId";
                    ListCriteria.Add("StageId", wsi.Task.StageId);
                }
                if (!string.IsNullOrEmpty(wsi.Task.ProjectId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProjectId = :ProjectId";
                    ListCriteria.Add("ProjectId", wsi.Task.ProjectId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            
            // thời hạn t/hien trong một khoảng from -> to
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Deadline >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Deadline <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.TaskId LIKE :Keyword OR O.TaskName LIKE :Keyword OR O.ProjectId in (SELECT P.ProjectId FROM Projects AS P WHERE P.ProjectId LIKE :Keyword OR P.ProjectName LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.TaskId) FROM Tasks AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
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
            List<NMTasksWSI> ListWSI = new List<NMTasksWSI>();
            TasksAccesser Accesser = new TasksAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            DailyReportsAccesser DRAcc = new DailyReportsAccesser(Session);

            IList<Tasks> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetTasksByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetTasksByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Tasks obj in objs)
            {
                wsi = new NMTasksWSI();
                wsi.Task = obj;
                wsi.Details = obj.TaskDetailList.Cast<TaskDetails>().ToList();
                wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, true);
                wsi.AssignedUser = CustomerAccesser.GetAllCustomersByID(obj.AssignedTo, true);
                wsi.CheckedBy = CustomerAccesser.GetAllCustomersByID(obj.CheckedBy, true);
                wsi.Manager = CustomerAccesser.GetAllCustomersByID(obj.Manager, true);
                wsi.Stage = StageAccesser.GetAllStagesByID(obj.StageId, true);
                wsi.Priority = ParameterAccesser.GetAllParametersByID(obj.Priority, true);
                wsi.Reports = DRAcc.GetAllDailyReportsByTaskId(wsi.Task.TaskId, true).ToList();

                ListWSI.Add(wsi);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;
            return ListWSI;
        }       
    }
}
