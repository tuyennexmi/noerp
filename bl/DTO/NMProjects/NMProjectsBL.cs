// NMProjectsBL.cs

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
    public class NMProjectsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMProjectsBL()
        {

        }

        public NMProjectsWSI callSingleBL(NMProjectsWSI wsi)
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

        public List<NMProjectsWSI> callListBL(NMProjectsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMProjectsWSI>();
            }
        }

        public NMProjectsWSI SelectObject(NMProjectsWSI wsi)
        {
            ProjectsAccesser Acceser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            TasksAccesser TaskAccesser = new TasksAccesser(Session);
            IssuesAccesser IssueAccesser = new IssuesAccesser(Session);
            Projects obj = Acceser.GetAllProjectsByID(wsi.Project.ProjectId, true);
            if (obj != null)
            {
                wsi.Project = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.ManagedBy = CustomerAccesser.GetAllCustomersByID(obj.ManagedBy, true);
                wsi.Status = StatusAccesser.GetAllStatusesByID(obj.StatusId, true);
                wsi.Tasks = TaskAccesser.GetTasksByQuery(Session.CreateQuery("select O from Tasks O where O.ProjectId = '" + obj.ProjectId + "'"), true);
                wsi.Issues = IssueAccesser.GetIssuesByQuery(Session.CreateQuery("select O from Issues O where O.ProjectId = '" + obj.ProjectId + "'"), true);
                //wsi.Teams = CustomerAccesser.GetCustomersByQuery(Session.CreateQuery("select C from Customers C, ProjectTeams PT where C.CustomerId = PT.UserId and PT.ProjectId = '" + obj.ProjectId + "'"), true);
                //wsi.Stages = StageAccesser.GetStagesByQuery(Session.CreateQuery("select S from Stages S, ProjectStages PS where S.StageId = PS.StageId and PS.ProjectId = '" + obj.ProjectId + "' order by PS.Sequence"), true);
                wsi.Teams = CustomerAccesser.GetCustomersByQuery(Session.CreateQuery("select O from Customers O where CHARINDEX(O.CustomerId, '" + obj.Team + "') > 0"), true);
                wsi.Stages = StageAccesser.GetStagesByQuery(Session.CreateQuery("select O from Stages O where CHARINDEX(O.StageId, '" + obj.Stage + "') > 0"), true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMProjectsWSI SaveObject(NMProjectsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProjectsAccesser Accesser = new ProjectsAccesser(Session);
                ProjectStagesAccesser PSAccesser = new ProjectStagesAccesser(Session);
                ProjectTeamsAccesser PTAccesser = new ProjectTeamsAccesser(Session);
                //IList<ProjectStages> OldStages = null;
                //IList<ProjectTeams> OldTeams = null;
                Projects obj = Accesser.GetAllProjectsByID(wsi.Project.ProjectId, true);
                if (obj != null)
                {
                    //OldStages = PSAccesser.GetAllProjectStagesByProjectID(obj.ProjectId, true);
                    //OldTeams = PTAccesser.GetAllProjectTeamsByProjectID(obj.ProjectId, true);
                    //Session.Evict(obj);
                    wsi.Project.CreatedDate = obj.CreatedDate;
                    wsi.Project.CreatedBy = obj.CreatedBy;
                    wsi.Project.ModifiedDate = DateTime.Now;
                    wsi.Project.ModifiedBy = wsi.ActionBy;
                    String rs = wsi.CompareTo(obj);
                    Accesser.UpdateProject(wsi.Project);
                    if (rs != "")
                        NMCommon.SaveMessage(Session, wsi.Project.ProjectId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    wsi.Project.ProjectId = AutomaticValueAccesser.AutoGenerateId("Projects");
                    wsi.Project.CreatedDate = DateTime.Now;
                    wsi.Project.CreatedBy = wsi.ActionBy;
                    wsi.Project.ModifiedDate = DateTime.Now;
                    wsi.Project.ModifiedBy = wsi.ActionBy;
                    Accesser.InsertProject(wsi.Project);
                    NMCommon.SaveMessage(Session, wsi.Project.ProjectId, "khởi tạo đối tượng", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                //ProjectStages PS, PSTemp;
                //foreach (Stages Item in wsi.Stages)
                //{
                //    PSTemp = PSAccesser.GetAllProjectStagesByID(wsi.Project.ProjectId, Item.StageId, true);
                //    if (PSTemp != null)
                //    {
                //        PS = new ProjectStages();
                //        PS.ProjectId = wsi.Project.ProjectId;
                //        PS.StageId = Item.StageId;
                //        PS.Sequence = Item.Sequence;
                //        PSAccesser.UpdateProjectStage(PS);
                //    }
                //    else
                //    {
                //        PS = new ProjectStages();
                //        PS.ProjectId = wsi.Project.ProjectId;
                //        PS.StageId = Item.StageId;
                //        PS.Sequence = Item.Sequence;
                //        PSAccesser.InsertProjectStage(PS);
                //    }
                //}
                //if (OldStages != null)
                //{
                //    bool flag = true;
                //    foreach (ProjectStages Old in OldStages)
                //    {
                //        flag = true;
                //        foreach (Stages New in wsi.Stages)
                //        {
                //            if (Old.StageId == New.StageId)
                //            {
                //                flag = false;
                //            }
                //        }
                //        if (flag)
                //        {
                //            PSAccesser.DeleteProjectStage(Old);
                //        }
                //    }
                //}
                //ProjectTeams PT;
                //foreach (Customers Item in wsi.Teams)
                //{
                //    PT = PTAccesser.GetAllProjectTeamsByID(wsi.Project.ProjectId, Item.CustomerId, true);
                //    if (PT == null)
                //    {
                //        PT = new ProjectTeams();
                //        PT.ProjectId = wsi.Project.ProjectId;
                //        PT.UserId = Item.CustomerId;
                //        PT.Status = Item.Status;
                //        PTAccesser.InsertProjectTeam(PT);
                //    }
                //}
                //if (OldTeams != null)
                //{
                //    bool flag = true;
                //    foreach (ProjectTeams Old in OldTeams)
                //    {
                //        flag = true;
                //        foreach (Customers New in wsi.Teams)
                //        {
                //            if (Old.UserId == New.CustomerId)
                //            {
                //                flag = false;
                //            }
                //        }
                //        if (flag)
                //        {
                //            PTAccesser.DeleteProjectTeam(Old);
                //        }
                //    }
                //}
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMProjectsWSI DeleteObject(NMProjectsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProjectsAccesser Accesser = new ProjectsAccesser(Session);
                Projects obj = Accesser.GetAllProjectsByID(wsi.Project.ProjectId, true);
                if (obj != null)
                {
                    Accesser.DeleteProject(obj);
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
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMProjectsWSI> SearchObject(NMProjectsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Project != null)
            {
                if (!String.IsNullOrEmpty(wsi.Project.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Project.CustomerId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ProjectId LIKE :Keyword OR O.ProjectName LIKE :Keyword";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.ProjectId) FROM Projects AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.ProjectId DESC";
            }
            string strCmd = "SELECT O FROM Projects AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMProjectsWSI> ListWSI = new List<NMProjectsWSI>();
            ProjectsAccesser Accesser = new ProjectsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            StagesAccesser StageAccesser = new StagesAccesser(Session);
            TasksAccesser TaskAccesser = new TasksAccesser(Session);
            IssuesAccesser IssueAccesser = new IssuesAccesser(Session);
            IList<Projects> objs = Accesser.GetProjectsByQuery(query, false);
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetProjectsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetProjectsByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Projects obj in objs)
            {
                wsi = new NMProjectsWSI();
                wsi.Project = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.ManagedBy = CustomerAccesser.GetAllCustomersByID(obj.ManagedBy, true);
                wsi.Status = StatusAccesser.GetAllStatusesByID(obj.StatusId, true);
                wsi.Tasks = TaskAccesser.GetTasksByQuery(Session.CreateQuery("select O from Tasks O where O.ProjectId = '" + obj.ProjectId + "'"), true);
                wsi.Issues = IssueAccesser.GetIssuesByQuery(Session.CreateQuery("select O from Issues O where O.ProjectId = '" + obj.ProjectId + "'"), true);
                //wsi.Teams = CustomerAccesser.GetCustomersByQuery(Session.CreateQuery("select C from Customers C, ProjectTeams PT where C.CustomerId = PT.UserId and PT.ProjectId = '" + obj.ProjectId + "'"), true);
                //wsi.Stages = StageAccesser.GetStagesByQuery(Session.CreateQuery("select S from Stages S, ProjectStages PS where S.StageId = PS.StageId and PS.ProjectId = '" + obj.ProjectId + "' order by PS.Sequence"), true);
                wsi.Teams = CustomerAccesser.GetCustomersByQuery(Session.CreateQuery("select O from Customers O where CHARINDEX(O.CustomerId, '" + obj.Team + "') > 0"), true);
                wsi.Stages = StageAccesser.GetStagesByQuery(Session.CreateQuery("select O from Stages O where CHARINDEX(O.StageId, '" + obj.Stage + "') > 0"), true);
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
