// NMIssuesBL.cs

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
    public class NMIssuesBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMIssuesBL()
        {

        }

        public NMIssuesWSI callSingleBL(NMIssuesWSI wsi)
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

        public List<NMIssuesWSI> callListBL(NMIssuesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMIssuesWSI>();
            }
        }

        public NMIssuesWSI SelectObject(NMIssuesWSI wsi)
        {
            IssuesAccesser Acceser = new IssuesAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            TasksAccesser TaskAccesser = new TasksAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            Issues obj;
            obj = Acceser.GetAllIssuesByID(wsi.Issue.IssueId, true);
            if (obj != null)
            {
                wsi.Issue = obj;
                wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, true);
                wsi.AssignedUser = CustomerAccesser.GetAllCustomersByID(obj.UserId, true);
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.ContactId, true);
                wsi.Stage = StageAccesser.GetAllStagesByID(obj.StageId, true);
                wsi.Task = TaskAccesser.GetAllTasksByID(obj.TaskId, true);
                wsi.Priority = ParameterAccesser.GetAllParametersByID(obj.Priority, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMIssuesWSI SaveObject(NMIssuesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                IssuesAccesser Accesser = new IssuesAccesser(Session);
                Issues obj = Accesser.GetAllIssuesByID(wsi.Issue.IssueId, true);
                if (obj != null)
                {
                    wsi.Issue.CreatedBy = obj.CreatedBy;
                    wsi.Issue.CreatedDate = obj.CreatedDate;
                    Accesser.UpdateIssue(wsi.Issue);
                }
                else
                {
                    wsi.Issue.IssueId = AutomaticValueAccesser.AutoGenerateId("Issues");
                    wsi.Issue.CreatedDate = DateTime.Now;
                    wsi.Issue.CreatedBy = wsi.ActionBy;
                    Accesser.InsertIssue(wsi.Issue);
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\n" + ex.Message;
            }
            return wsi;
        }

        public NMIssuesWSI DeleteObject(NMIssuesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                IssuesAccesser Accesser = new IssuesAccesser(Session);
                Issues obj = Accesser.GetAllIssuesByID(wsi.Issue.IssueId, true);
                if (obj != null)
                {
                    Accesser.DeleteIssue(obj);
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

        public List<NMIssuesWSI> SearchObject(NMIssuesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Issue != null)
            {
                if (!string.IsNullOrEmpty(wsi.Issue.StageId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StageId = :StageId";
                    ListCriteria.Add("StageId", wsi.Issue.StageId);
                }
                if (!string.IsNullOrEmpty(wsi.Issue.ProjectId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProjectId = :ProjectId";
                    ListCriteria.Add("ProjectId", wsi.Issue.ProjectId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.IssueId LIKE :Keyword OR O.IssueName LIKE :Keyword OR O.ProjectId in (SELECT P.ProjectId FROM Projects AS P WHERE P.ProjectId LIKE :Keyword OR P.ProjectName LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.IssueId) FROM NMIssues AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.IssueId DESC";
            }
            String strCmd = "SELECT O FROM Issues AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMIssuesWSI> ListWSI = new List<NMIssuesWSI>();
            IssuesAccesser Accesser = new IssuesAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            TasksAccesser TaskAccesser = new TasksAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            IList<Issues> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetIssuesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetIssuesByQuery(query, false);
            }
            foreach (Issues obj in objs)
            {
                wsi = new NMIssuesWSI();
                wsi.Issue = obj;
                wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, true);
                wsi.AssignedUser = CustomerAccesser.GetAllCustomersByID(obj.UserId, true);
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.ContactId, true);
                wsi.Stage = StageAccesser.GetAllStagesByID(obj.StageId, true);
                wsi.Task = TaskAccesser.GetAllTasksByID(obj.TaskId, true);
                wsi.Priority = ParameterAccesser.GetAllParametersByID(obj.Priority, true);
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
