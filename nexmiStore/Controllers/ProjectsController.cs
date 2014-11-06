// ProjectsController.cs

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
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace NEXMI.Controllers
{
    public class ProjectsController : Controller
    {
        //
        // GET: /Projects/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Projects()
        {
            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult ProjectKanban(string pageNum, string categoryId, string keyword)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }

            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Project, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            WSI.Keyword = NMCryptography.base64Decode(keyword);
            List<NEXMI.NMProjectsWSI> WSIs = BL.callListBL(WSI);

            ViewData["WSIs"] = WSIs;
            ViewData["Page"] = page;
            if (WSIs.Count > 0)
                ViewData["TotalRows"] = WSIs[0].TotalRows;
            else
                ViewData["TotalRows"] = "";
            ViewData["ViewType"] = "kanban";
            ViewData["Keyword"] = keyword;

            return PartialView();
        }

        public ActionResult ProjectDetail(String id)
        {
            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Project = new Projects();
            WSI.Project.ProjectId = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            ViewData["WindowId"] = NMCommon.RandomString(8, true);
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public ActionResult ProjectForm(string id, string comboBoxId, string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NMProjectsBL BL = new NMProjectsBL();
                NMProjectsWSI WSI = new NMProjectsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Project = new Projects();
                WSI.Project.ProjectId = id;
                //WSI = BL.callSingleBL(WSI);
                ViewData["WSI"] = BL.callSingleBL(WSI);
            }
            if (comboBoxId == null) comboBoxId = "";
            ViewData["ComboBoxId"] = comboBoxId;
            if (String.IsNullOrEmpty(windowId))
            {
                ViewData["WindowMode"] = "";
                windowId = NMCommon.RandomString(8, true);
            }
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateProject(string id, string name, string customerId, string managedBy, string task,
            string issue, string team, string stage, string startDate, string endDate, string parentId, string description,
            string statusId, string salesForecast)
        {
            string result = "";
            NMProjectsWSI WSI = new NMProjectsWSI();
            NMProjectsBL BL = new NMProjectsBL();
            WSI.Mode = "SAV_OBJ";
            WSI.Project = new Projects();
            WSI.Project.ProjectId = id;
            WSI.Project.ProjectName = name;
            if (!string.IsNullOrEmpty(customerId))
                WSI.Project.CustomerId = customerId;
            WSI.Project.ManagedBy = managedBy;
            WSI.Project.Task = bool.Parse(task);
            WSI.Project.StatusId = statusId;
            WSI.Project.Issue = bool.Parse(issue);
            try {
                WSI.Project.SalesForecast = float.Parse(salesForecast);
            } catch { }
            try { 
                WSI.Project.StartDate = DateTime.Parse(startDate); 
            }catch { }
            try { 
                WSI.Project.EndDate = DateTime.Parse(endDate); 
            }catch { }
            WSI.Project.Stage = stage;
            WSI.Project.Team = team;
            //List<Stages> PS = new List<NEXMI.Stages>();
            //List<Customers> PT = new List<Customers>();
            //string[] arrStage = Regex.Split(stage, "_NM_"), arrMember = Regex.Split(team, "_NM_");
            //if (arrStage.Length > 0)
            //{
            //    Stages st;
            //    for (int i = 0; i < arrStage.Length - 1; i++)
            //    {
            //        st = new Stages();
            //        st.StageId = arrStage[i];
            //        PS.Add(st);
            //    }
            //}
            //if (arrMember.Length > 0)
            //{
            //    Customers st;
            //    for (int i = 0; i < arrMember.Length - 1; i++)
            //    {
            //        st = new Customers();
            //        st.CustomerId = arrStage[i];
            //        PT.Add(st);
            //    }
            //}
            //WSI.Teams = PT;
            //WSI.Stages = PS;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            result = WSI.WsiError;
            return Json(new { id = WSI.Project.ProjectId, error = result });
        }

        public JsonResult UpdateProject(string id, string statusId)
        {
            string error = "";
            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Project = new NEXMI.Projects();
            WSI.Project.ProjectId = id;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Project.StatusId = statusId;
                WSI.ActionBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
                error = WSI.WsiError;
            }
            else
                error = WSI.WsiError;
            return Json(error);
        }

        public JsonResult DeleteProject(String id)
        {
            NMProjectsWSI WSI = new NMProjectsWSI();
            NMProjectsBL BL = new NMProjectsBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Project = new NEXMI.Projects();
            WSI.Project.ProjectId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult ProjectList(string customer, string pageNum, string pageSize, string sortDataField, string sortOrder, string keyword)
        {
            NMCustomersWSI cWSI = new NMCustomersWSI();
            NMCustomersBL cBL = new NMCustomersBL();
            cWSI.Mode = "SRC_OBJ";
            cWSI.Customer = new Customers();
            ViewData["Customers"] = cBL.callListBL(cWSI);

            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Project = new Projects();
            if (!string.IsNullOrEmpty(customer))
                WSI.Project.CustomerId = customer;

            WSI.Keyword = keyword;
            WSI.PageNum = 0;
            try
            {
                WSI.PageNum = int.Parse(pageNum);
            }
            catch { }
            WSI.PageSize = NMCommon.PageSize();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Project, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            List<NMProjectsWSI> WSIs = BL.callListBL(WSI);

            ViewData["WSIs"] = WSIs;

            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult TaskList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult Tasks(string projectId)
        {
            List<Stages> Stages;
            if (!string.IsNullOrEmpty(projectId))
            {
                NMProjectsBL BL = new NMProjectsBL();
                NMProjectsWSI WSI = new NMProjectsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Project = new Projects();
                WSI.Project.ProjectId = projectId;
                Stages = BL.callSingleBL(WSI).Stages.ToList();
            }
            else
            {
                if (projectId == null) projectId = "";
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SRC_OBJ";
                Stages = BL.callListBL(WSI).Select(i => i.Stage).ToList();
            }
            ViewData["Stages"] = Stages;
            ViewData["ProjectId"] = projectId;
            return PartialView();
        }

        public ActionResult TaskDetail(string id, string mode, string page)
        {
            NMTasksBL BL = new NMTasksBL();
            NMTasksWSI WSI = new NMTasksWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Task = new Tasks();
            WSI.Task.TaskId = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);

            ViewData["Page"] = page;
            if (mode == "Print")
                return PartialView("TaskCard");

            return PartialView();
        }

        public ActionResult TaskForm(string id, string stageId, string projectId, string comboBoxId, string windowId)
        {
            Session["TaskDetails"] = null;
            if (stageId == null) stageId = "";
            ViewData["ProjectId"] = "";
            if (!string.IsNullOrEmpty(id))
            {
                NMTasksBL BL = new NMTasksBL();
                NMTasksWSI WSI = new NMTasksWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Task = new Tasks();
                WSI.Task.TaskId = id;
                
                WSI = BL.callSingleBL(WSI);
                ViewData["WSI"] = WSI;
                Session["TaskDetails"] = WSI.Details;
                if (WSI.Project != null)
                {
                    ViewData["ProjectId"] = WSI.Project.ProjectId;
                    ViewData["ProjectName"] = WSI.Project.ProjectName;
                    ViewData["ProjectStages"] = WSI.Project.Stage;
                }
            }
            else if (!string.IsNullOrEmpty(projectId))
            {
                NMProjectsBL ProjectBL = new NMProjectsBL();
                NMProjectsWSI ProjectWSI = new NMProjectsWSI();
                ProjectWSI.Mode = "SEL_OBJ";
                ProjectWSI.Project = new NEXMI.Projects();
                ProjectWSI.Project.ProjectId = projectId;
                ProjectWSI = ProjectBL.callSingleBL(ProjectWSI);
                if (ProjectWSI.Project != null)
                {
                    ViewData["ProjectId"] = ProjectWSI.Project.ProjectId;
                    ViewData["ProjectName"] = ProjectWSI.Project.ProjectName;
                    ViewData["ProjectStages"] = ProjectWSI.Project.Stage;
                }
            }
            if (String.IsNullOrEmpty(windowId))
            {
                ViewData["WindowMode"] = "";
                windowId = NMCommon.RandomString(8, true);
            }
            
            ViewData["StageId"] = stageId;
            if (comboBoxId == null) comboBoxId = "";
            ViewData["ComboBoxId"] = comboBoxId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateTask(string id, string name, string stageId, string projectId, string deadLine, string jobref,
                        string assignedTo, string priority, string sequence, string tags, string criteria, string purpose,
                        string description, string statusId, string start, string end, string checkedBy, string manager,
                        string category, string reportsPeriod)
        {
            string result = "";
            NMTasksWSI WSI = new NMTasksWSI();
            NMTasksBL BL = new NMTasksBL();
            WSI.Mode = "SAV_OBJ";
            WSI.Task = new Tasks();
            WSI.Task.TaskId = id;
            WSI.Task.TaskName = name;
            WSI.Task.StageId = stageId;

            if(!string.IsNullOrEmpty(projectId))
                WSI.Task.ProjectId = projectId;

            WSI.Task.AssignedTo = assignedTo;
            WSI.Task.StatusId = statusId;
            WSI.Task.Purpose = purpose;
            WSI.Task.Criteria = criteria;
            WSI.Task.CheckedBy = checkedBy;
            WSI.Task.Manager = manager;

            if (!string.IsNullOrEmpty(jobref))
                WSI.Task.JobReference = jobref;

            WSI.Task.Category = category;
            WSI.Task.IsReportTimeRight = true;
            try
            {
                WSI.Task.Deadline = DateTime.Parse(deadLine);
            }
            catch { }
            try
            {
                WSI.Task.StartDate = DateTime.Parse(start);
            }
            catch { }
            try
            {
                WSI.Task.EndDate = DateTime.Parse(end);
            }
            catch { }
            WSI.Task.Tags = tags;
            WSI.Task.Priority = priority;
            WSI.Task.ReportPeriod = reportsPeriod;
            try
            {
                WSI.Task.Sequence = Convert.ToInt16(sequence);
            }
            catch { }
            WSI.Task.Description = description;
            WSI.ActionBy = User.Identity.Name;
            WSI.Details = (List<TaskDetails>)Session["TaskDetails"];
            WSI = BL.callSingleBL(WSI);
            result = WSI.WsiError;
            if (WSI.WsiError == "")
            {
                Session["TaskDetails"] = null;
            }

            return Json(new { id = WSI.Task.TaskId, error = result });
        }

        public JsonResult DeleteTask(string id)
        {
            NMTasksBL BL = new NMTasksBL();
            NMTasksWSI WSI = new NMTasksWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Task = new Tasks();
            WSI.Task.TaskId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult SetStageForTask(string taskId, string stageStatusId, string statusId)
        {
            string error = "";
            if (!string.IsNullOrEmpty(statusId))
            {
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.Stage = new Stages();
                WSI.Stage.RelatedStatus = stageStatusId;
                List<NMStagesWSI> WSIs = BL.callListBL(WSI);
                if (WSIs.Count > 0)
                {
                    NMTasksBL TaskBL = new NMTasksBL();
                    NMTasksWSI TaskWSI = new NMTasksWSI();
                    TaskWSI.Mode = "SEL_OBJ";
                    TaskWSI.Task = new Tasks();
                    TaskWSI.Task.TaskId = taskId;
                    TaskWSI = TaskBL.callSingleBL(TaskWSI);
                    if (TaskWSI.WsiError == "")
                    {
                        TaskWSI.Mode = "SAV_OBJ";
                        //TaskWSI.Task.FirstStage = TaskWSI.Task.StageId;
                        TaskWSI.Task.StageId = WSIs[0].Stage.StageId;
                        TaskWSI.Task.StatusId = statusId;
                        TaskWSI = TaskBL.callSingleBL(TaskWSI);
                        error = TaskWSI.WsiError;
                    }
                    else
                    {
                        error = TaskWSI.WsiError;
                    }
                }
                else
                {
                    error = "None";
                }
            }
            else
            {
                NMTasksBL TaskBL = new NMTasksBL();
                NMTasksWSI TaskWSI = new NMTasksWSI();
                TaskWSI.Mode = "SEL_OBJ";
                TaskWSI.Task = new Tasks();
                TaskWSI.Task.TaskId = taskId;
                TaskWSI = TaskBL.callSingleBL(TaskWSI);
                if (TaskWSI.WsiError == "")
                {
                    TaskWSI.Mode = "SAV_OBJ";
                    //TaskWSI.Task.StageId = TaskWSI.Task.FirstStage;
                    TaskWSI.Task.StatusId = NMConstant.TaskStatuses.InProgress;
                    TaskWSI = TaskBL.callSingleBL(TaskWSI);
                    error = TaskWSI.WsiError;
                }
                else
                {
                    error = TaskWSI.WsiError;
                }
            }
            return Json(error);
        }

        public ActionResult WorkForm(string id, string windowId)
        {
            List<TaskDetails> Details = new List<TaskDetails>();
            if (Session["TaskDetails"] != null)
                Details = (List<TaskDetails>)Session["TaskDetails"];
            int No = 0;
            try
            {
                No = int.Parse(id);
            }
            catch{}
            int index = Details.FindIndex(i => i.No == No);
            string name = "", timeSpent = "0", startDate = DateTime.Now.ToString("MM/dd/yyyyTHH:mm"), userId = "";
            if (index != -1)
            {
                TaskDetails Item = Details[index];
                name = Item.Name;
                timeSpent = Item.TimeSpent.ToString();
                startDate = Item.StartDate.ToString("MM/dd/yyyyTHH:mm");
                userId = Item.UserId;
            }
            ViewData["Id"] = id;
            ViewData["Name"] = name;
            ViewData["TimeSpent"] = timeSpent;
            ViewData["StartDate"] = startDate;
            ViewData["UserId"] = userId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult AddWorkToSession(string no, string name, string timeSpent, string startDate, string userId)
        {
            try
            {
                List<TaskDetails> Details = new List<TaskDetails>();
                if (Session["TaskDetails"] != null)
                    Details = (List<TaskDetails>)Session["TaskDetails"];
                TaskDetails Item = new TaskDetails();
                Item.No = int.Parse(no);
                Item.Name = name;
                try
                {
                    Item.TimeSpent = float.Parse(timeSpent);
                }
                catch {
                    Item.TimeSpent = 0;
                }
                Item.StartDate = DateTime.Parse(startDate);
                Item.EndDate = Item.StartDate.AddHours(Item.TimeSpent);
                Item.UserId = userId;
                int index = Details.FindIndex(i => i.No == Item.No);
                if (index == -1)
                {
                    Item.No = Details.Count + 1;
                    Details.Add(Item);
                }
                else
                {
                    Item.No = index + 1;
                    Details[index] = Item;
                }
                Session["TaskDetails"] = Details;
                return Json(new { No = Item.No, error = "" });
            }
            catch (Exception ex){
                return Json(new { No = 0, error = "Lỗi!\n" + ex.Message });
            }
        }

        public ActionResult TaskItemLine()
        {
            return PartialView();
        }

        public JsonResult RemoveWorkFromSession(string id)
        {
            try
            {
                List<TaskDetails> Details = new List<TaskDetails>();
                if (Session["TaskDetails"] != null)
                    Details = (List<TaskDetails>)Session["TaskDetails"];
                
                int index = Details.FindIndex(i => i.No == int.Parse(id));
                if (index != -1)
                    Details.RemoveAt(index);
                int count = 1;
                foreach (TaskDetails Item in Details)
                    Item.No = count++;

                Session["TaskDetails"] = Details;
                return Json("");
            }
            catch (Exception ex)
            {
                return Json("Lỗi!\n" + ex.Message);
            }
        }

        public ActionResult TasksOfStage(string stageId, string projectId)
        {
            NMTasksBL BL = new NMTasksBL();
            NMTasksWSI WSI = new NMTasksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Task = new Tasks();
            WSI.Task.StageId = stageId;
            WSI.Task.ProjectId = projectId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult Issues(string projectId)
        {
            List<Stages> Stages;
            if (!string.IsNullOrEmpty(projectId))
            {
                NMProjectsBL BL = new NMProjectsBL();
                NMProjectsWSI WSI = new NMProjectsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Project = new Projects();
                WSI.Project.ProjectId = projectId;
                Stages = BL.callSingleBL(WSI).Stages.ToList();
            }
            else
            {
                if (projectId == null) projectId = "";
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SRC_OBJ";
                Stages = BL.callListBL(WSI).Select(i => i.Stage).ToList();
            }
            ViewData["Stages"] = Stages;
            ViewData["ProjectId"] = projectId;
            return PartialView();
        }

        public ActionResult IssuesOfStage(string stageId, string projectId)
        {
            NMIssuesBL BL = new NMIssuesBL();
            NMIssuesWSI WSI = new NMIssuesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Issue = new Issues();
            WSI.Issue.StageId = stageId;
            WSI.Issue.ProjectId = projectId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult IssueForm(string id, string stageId, string projectId, string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NMIssuesBL BL = new NMIssuesBL();
                NMIssuesWSI WSI = new NMIssuesWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Issue = new Issues();
                WSI.Issue.IssueId = id;
                ViewData["WSI"] = BL.callSingleBL(WSI);
                if (WSI.Project != null)
                {
                    ViewData["ProjectId"] = WSI.Project.ProjectId;
                    ViewData["ProjectName"] = WSI.Project.ProjectName;
                    ViewData["ProjectStages"] = WSI.Project.Stage;
                }
            }
            if (String.IsNullOrEmpty(windowId))
            {
                ViewData["WindowMode"] = "";
                windowId = NMCommon.RandomString(8, true);
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                NMProjectsBL ProjectBL = new NMProjectsBL();
                NMProjectsWSI ProjectWSI = new NMProjectsWSI();
                ProjectWSI.Mode = "SEL_OBJ";
                ProjectWSI.Project = new NEXMI.Projects();
                ProjectWSI.Project.ProjectId = projectId;
                ProjectWSI = ProjectBL.callSingleBL(ProjectWSI);
                if (ProjectWSI.Project != null)
                {
                    ViewData["ProjectId"] = ProjectWSI.Project.ProjectId;
                    ViewData["ProjectName"] = ProjectWSI.Project.ProjectName;
                    ViewData["ProjectStages"] = ProjectWSI.Project.Stage;
                }
            }
            ViewData["StageId"] = stageId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult IssueDetail(string id)
        {
            NMIssuesBL BL = new NMIssuesBL();
            NMIssuesWSI WSI = new NMIssuesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Issue = new Issues();
            WSI.Issue.IssueId = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            return PartialView();
        }

        public JsonResult SaveOrUpdateIssue(string id, string name, string stageId, string tags, string assignedTo, string priority,
            string projectId, string taskId, string customerId, string email, string description, string statusId)
        {
            NMIssuesBL BL = new NMIssuesBL();
            NMIssuesWSI WSI = new NMIssuesWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Issue = new Issues();
            WSI.Issue.IssueId = id;
            WSI.Issue.IssueContent = name;
            WSI.Issue.StageId = stageId;
            WSI.Issue.Tags = tags;
            WSI.Issue.UserId = assignedTo;
            WSI.Issue.Priority = priority;
            if (!string.IsNullOrEmpty(projectId))
                WSI.Issue.ProjectId = projectId;
            if (!string.IsNullOrEmpty(taskId))
                WSI.Issue.TaskId = taskId;
            if (!string.IsNullOrEmpty(customerId))
                WSI.Issue.ContactId = customerId;
            WSI.Issue.Email = email;
            WSI.Issue.Description = description;
            WSI.Issue.StatusId = statusId;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);

            return Json(new { error = WSI.WsiError });
        }

        public JsonResult SetStageForIssue(string issueId, string stageStatusId, string statusId)
        {
            string error = "";
            if (!string.IsNullOrEmpty(statusId))
            {
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.Stage = new Stages();
                WSI.Stage.RelatedStatus = stageStatusId;
                List<NMStagesWSI> WSIs = BL.callListBL(WSI);
                if (WSIs.Count > 0)
                {
                    NMIssuesBL IssueBL = new NMIssuesBL();
                    NMIssuesWSI IssueWSI = new NMIssuesWSI();
                    IssueWSI.Mode = "SEL_OBJ";
                    IssueWSI.Issue = new Issues();
                    IssueWSI.Issue.IssueId = issueId;
                    IssueWSI = IssueBL.callSingleBL(IssueWSI);
                    if (IssueWSI.WsiError == "")
                    {
                        IssueWSI.Mode = "SAV_OBJ";
                        IssueWSI.Issue.FirstStage = IssueWSI.Issue.StageId;
                        IssueWSI.Issue.StageId = WSIs[0].Stage.StageId;
                        IssueWSI.Issue.StatusId = statusId;
                        IssueWSI = IssueBL.callSingleBL(IssueWSI);
                        error = IssueWSI.WsiError;
                    }
                    else
                    {
                        error = IssueWSI.WsiError;
                    }
                }
                else
                {
                    error = "None";
                }
            }
            else
            {
                NMIssuesBL IssueBL = new NMIssuesBL();
                NMIssuesWSI IssueWSI = new NMIssuesWSI();
                IssueWSI.Mode = "SEL_OBJ";
                IssueWSI.Issue = new Issues();
                IssueWSI.Issue.IssueId = issueId;
                IssueWSI = IssueBL.callSingleBL(IssueWSI);
                if (IssueWSI.WsiError == "")
                {
                    IssueWSI.Mode = "SAV_OBJ";
                    IssueWSI.Issue.StageId = IssueWSI.Issue.FirstStage;
                    IssueWSI.Issue.StatusId = NMConstant.IssueStatuses.InProgress;
                    IssueWSI = IssueBL.callSingleBL(IssueWSI);
                    error = IssueWSI.WsiError;
                }
                else
                {
                    error = IssueWSI.WsiError;
                }
            }
            return Json(error);
        }

        public JsonResult DeleteIssue(string id)
        {
            NMIssuesBL BL = new NMIssuesBL();
            NMIssuesWSI WSI = new NMIssuesWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Issue = new Issues();
            WSI.Issue.IssueId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult StageList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult StageForm(string id, string parentId, string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Stage = new Stages();
                WSI.Stage.StageId = id;
                ViewData["WSI"] = BL.callSingleBL(WSI);
            }
            if (parentId == null)
                parentId = "";
            ViewData["ParentId"] = parentId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateStage(string id, string name, string relatedStatus, string sequence, string description, string isCommon, string folded)
        {
            NMStagesBL BL = new NMStagesBL();
            NMStagesWSI WSI = new NMStagesWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Stage = new Stages();
            WSI.Stage.StageId = id;
            WSI.Stage.StageName = name;
            try { WSI.Stage.Sequence = int.Parse(sequence); }
            catch { }
            WSI.Stage.Description = description;
            WSI.Stage.IsCommon = bool.Parse(isCommon);
            WSI.Stage.Folded = bool.Parse(folded);
            WSI.Stage.TypeId = NMConstant.ProjectStages;
            if (!string.IsNullOrEmpty(relatedStatus))
                WSI.Stage.RelatedStatus = relatedStatus;
            WSI = BL.callSingleBL(WSI);
            return Json(new { id = WSI.Stage.StageId, name = WSI.Stage.StageName, error = WSI.WsiError });
        }

        public JsonResult DeleteStage(string id)
        {
            NMStagesBL BL = new NMStagesBL();
            NMStagesWSI WSI = new NMStagesWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Stage = new Stages();
            WSI.Stage.StageId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult Jobs()
        {   
            return PartialView();
        }

        public ActionResult JobList(string pageNum)
        {
            NMJobsBL BL = new NMJobsBL();
            NMJobsWSI WSI = new NMJobsWSI();
            WSI.Mode = "SRC_OBJ";
            int page = 0;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            WSI.Filter.PageNum = page - 1;
            WSI.Filter.PageSize = NMCommon.PageSize();

            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult JobForm(string id, string mode , string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NMJobsBL BL = new NMJobsBL();
                NMJobsWSI WSI = new NMJobsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Job.Id = id;
                ViewData["WSI"] = BL.callSingleBL(WSI);
            }

            ViewData["WindowId"] = windowId;

            if(mode == "Detail")
                return PartialView("JobDetail");
            
            return PartialView();
        }

        public JsonResult SaveOrUpdateJob(string id, string name, string timeSpent, string purpose, string criteria,
                                          string workSummary, string tags, string workGuids, string status)
        {
            NMJobsBL BL = new NMJobsBL();
            NMJobsWSI WSI = new NMJobsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Job.Id = id;
            WSI.Job.Name = name;
            float tSpent = 0;
            float.TryParse(timeSpent, out tSpent);
            WSI.Job.TimeSpent = tSpent;
            WSI.Job.Purpose = purpose;
            WSI.Job.Criteria = criteria;
            WSI.Job.WorkSummary = workSummary;
            WSI.Job.WorkGuids = workGuids;
            WSI.Job.Status = status;

            WSI.Filter.ActionBy = User.Identity.Name;

            WSI = BL.callSingleBL(WSI);

            return Json(WSI.WsiError);
        }

        public JsonResult DeleteJob(string id)
        {
            NMJobsBL BL = new NMJobsBL();
            NMJobsWSI WSI = new NMJobsWSI();
            
            WSI.Job.Id = id;
            WSI.Mode = "DEL_OBJ";
            
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult TaskCards()
        {   
            return PartialView();
        }

        public ActionResult TaskCardList(string pageNum, string keyword, string from, string to, string user, string status)
        {
            NMTasksBL BL = new NMTasksBL();
            NMTasksWSI WSI = new NMTasksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Task = new Tasks();

            if (!String.IsNullOrEmpty(user))
                WSI.Task.AssignedTo = user;
            
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.TaskCard) == false)
                WSI.Task.CreatedBy = User.Identity.Name;
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Task.StatusId = status;
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;

            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }
    }
}
