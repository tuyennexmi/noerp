// NMProjectStagesBL.cs

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
    public class NMProjectStagesBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMProjectStagesBL()
        {

        }

        public NMProjectStagesWSI callSingleBL(NMProjectStagesWSI wsi)
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

        public List<NMProjectStagesWSI> callListBL(NMProjectStagesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMProjectStagesWSI>();
            }
        }

        public NMProjectStagesWSI SelectObject(NMProjectStagesWSI wsi)
        {
            ProjectStagesAccesser Accesser = new ProjectStagesAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            IEnumerable<Tasks> Tasks;
            NMTasksWSI TaskWSI;
            IEnumerable<Issues> Issues;
            NMIssuesWSI IssueWSI;
            ProjectStages obj = Accesser.GetAllProjectStagesByID(wsi.ProjectStage.StageId, false);
            if (obj != null)
            {
                wsi.ProjectStage = obj;
                //wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, false);
                //wsi.TaskWSIs = new List<NMTasksWSI>();
                //Tasks = obj.Tasks.Cast<Tasks>();
                //foreach (Tasks Item in Tasks)
                //{
                //    TaskWSI = new NMTasksWSI();
                //    TaskWSI.Task = Item;
                //    TaskWSI.Details = Item.TaskDetailList.Cast<TaskDetails>().ToList();
                //    wsi.TaskWSIs.Add(TaskWSI);
                //}
                //wsi.IssueWSIs = new List<NMIssuesWSI>();
                //Issues = obj.Issues.Cast<Issues>();
                //foreach (Issues Item in Issues)
                //{
                //    IssueWSI = new NMIssuesWSI();
                //    IssueWSI.Issue = Item;
                //    wsi.IssueWSIs.Add(IssueWSI);
                //}
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMProjectStagesWSI SaveObject(NMProjectStagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProjectStagesAccesser Accesser = new ProjectStagesAccesser(Session);
                ProjectStages obj = Accesser.GetAllProjectStagesByID(wsi.ProjectStage.StageId, true);
                if (obj != null)
                {
                    Accesser.UpdateProjectStage(wsi.ProjectStage);
                }
                else
                {
                    wsi.ProjectStage.StageId = AutomaticValueAccesser.AutoGenerateId("ProjectStages");
                    wsi.ProjectStage.CreatedDate = DateTime.Now;
                    wsi.ProjectStage.CreatedBy = wsi.ActionBy;
                    Accesser.InsertProjectStage(wsi.ProjectStage);
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

        public NMProjectStagesWSI DeleteObject(NMProjectStagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProjectStagesAccesser Accesser = new ProjectStagesAccesser(Session);
                ProjectStages obj = Accesser.GetAllProjectStagesByID(wsi.ProjectStage.StageId, true);
                if (obj != null)
                {
                    Accesser.DeleteProjectStage(obj);
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

        public List<NMProjectStagesWSI> SearchObject(NMProjectStagesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.ProjectStage != null)
            {
                if (!string.IsNullOrEmpty(wsi.ProjectStage.ProjectId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProjectId = :ProjectId";
                    ListCriteria.Add("ProjectId", wsi.ProjectStage.ProjectId);
                }
                //if (!string.IsNullOrEmpty(wsi.ProjectStage.TypeId))
                //{
                //    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                //    strCriteria += " O.TypeId = :TypeId";
                //    ListCriteria.Add("TypeId", wsi.ProjectStage.TypeId);
                //}
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
                strCriteria += " (O.StageId LIKE :Keyword OR O.StageName LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.StageId) FROM ProjectStages AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.StageId DESC";
            }
            String strCmd = "SELECT O FROM ProjectStages AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMProjectStagesWSI> ListWSI = new List<NMProjectStagesWSI>();
            ProjectStagesAccesser Accesser = new ProjectStagesAccesser(Session);
            ProjectsAccesser ProjectAccesser = new ProjectsAccesser(Session);
            IList<ProjectStages> objs;
            IEnumerable<Tasks> Tasks;
            NMTasksWSI TaskWSI;
            IEnumerable<Issues> Issues;
            NMIssuesWSI IssueWSI;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetProjectStagesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetProjectStagesByQuery(query, false);
            }
            foreach (ProjectStages obj in objs)
            {
                wsi = new NMProjectStagesWSI();
                wsi.ProjectStage = obj;
                //wsi.Project = ProjectAccesser.GetAllProjectsByID(obj.ProjectId, false);
                //wsi.TaskWSIs = new List<NMTasksWSI>();
                //Tasks = obj.Tasks.Cast<Tasks>();
                //foreach (Tasks Item in Tasks)
                //{
                //    TaskWSI = new NMTasksWSI();
                //    TaskWSI.Task = Item;
                //    TaskWSI.Details = Item.TaskDetailList.Cast<TaskDetails>().ToList();
                //    wsi.TaskWSIs.Add(TaskWSI);
                //}
                //wsi.IssueWSIs = new List<NMIssuesWSI>();
                //Issues = obj.Issues.Cast<Issues>();
                //foreach (Issues Item in Issues)
                //{
                //    IssueWSI = new NMIssuesWSI();
                //    IssueWSI.Issue = Item;
                //    wsi.IssueWSIs.Add(IssueWSI);
                //}
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
