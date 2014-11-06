// MessagesController.cs

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
using System.Text;
using System.Text.RegularExpressions;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class MessagesController : Controller
    {
        //
        // GET: /Messages/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RenderMessages(NMMessagesWSI msg, String sendTo)
        {            
            ViewData["Msg"] = msg;
            ViewData["SendTo"] = sendTo;

            return PartialView();
        }

        public ActionResult LogsTracking(string ownerId)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Message = new Messages();

            WSI.Mode = "SRC_OBJ";
            WSI.Message.Owner = ownerId;
                        
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult Logs(string ownerId, string sendTo, string sendSMSTo)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Message = new Messages();

            WSI.Mode = "SRC_OBJ";
            WSI.Message.Owner = ownerId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            
            ViewData["OwnerId"] = ownerId;
            ViewData["SendMSGTo"] = sendTo;
            ViewData["SendSMSTo"] = sendSMSTo;

            return PartialView();
        }

        public ActionResult Inbox(string ownerId)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Message = new Messages();

            WSI.Mode = "SRC_OBJ";
            //WSI.Owner = User.Identity.Name;
            WSI.Message.SendTo = User.Identity.Name;

            ViewData["WSIs"] = BL.callListBL(WSI);
            
            return PartialView();
        }

        public ActionResult Outbox(string userId)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Message = new Messages();

            WSI.Mode = "SRC_OBJ";
            //WSI.Owner = ownerId;
            WSI.ActionBy = User.Identity.Name;
            ViewData["WSIs"] = BL.callListBL(WSI);
            
            return PartialView();
        }

        public ActionResult WriteNote()
        {
            return PartialView();
        }

        public JsonResult LogANote(String title, String msgContent, String ownerId, String sendTo, String replyTo, string type)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Message = new Messages();

            WSI.Mode = "SAV_OBJ";           

            if (String.IsNullOrEmpty(title))
            {
                WSI.Message.MessageName = "";
            }
            else
            {
                WSI.Message.MessageName = title;
            }
            WSI.Message.Owner = ownerId;
            WSI.Message.Description = msgContent;
            if (String.IsNullOrEmpty(type))
            {
                WSI.Message.TypeId = NMConstant.MessageTypes.UserNote;
            }
            else
            {
                WSI.Message.TypeId = type;
            }
            // trang thai
            if (type == NMConstant.MessageTypes.SMS)
            {
                WSI.Message.StatusId = MessageStatues.Pending;
            }
            else if (type == NMConstant.MessageTypes.Message)
            {
                WSI.Message.StatusId = MessageStatues.Sent;
            }
            //người nhận
            if (!String.IsNullOrEmpty(sendTo))
            {
                WSI.Message.SendTo = sendTo;
            }
            if (!String.IsNullOrEmpty(replyTo))
            {
                WSI.Message.ReplyTo = replyTo;
            }
            WSI.ActionBy = User.Identity.Name;
            ViewData["WSI"] = BL.callSingleBL(WSI);

            return Json(WSI.WsiError);
        }

        public ActionResult ComposeEmail(String title, String msgContent, String ownerId)
        {
            return PartialView();
        }

        public ActionResult ReplyMessage(String ownerId, String sendTo, String replyTo)
        {
            ViewData["OwnerId"] = ownerId;
            ViewData["SendTo"] = sendTo;
            ViewData["ReplyTo"] = replyTo;
            return PartialView();
        }

        /// <summary>
        /// lich cong tac
        /// </summary>
        /// <returns></returns>
        public ActionResult Calendars()
        {
            return PartialView();
        }

        public JsonResult LoadEvents(string start, string end)
        {
            String error = "", Schedule = ""; ;
            NMCalendarsBL BL = new NMCalendarsBL();
            NMCalendarsWSI WSI = new NMCalendarsWSI();
            List<Calendars> WSIs = new List<Calendars>();

            try
            {
                WSI.Mode = "SRC_OBJ";
                //WSI.FromDate= start + " 00:00:00 AM";
                //WSI.ToDate = end + " 11:59:59 PM";
                
                WSIs = BL.callListBL(WSI);

                for (int i = 0; i < WSIs.Count; i++)
                {
                    Schedule += WSIs[i].Id + "^" + WSIs[i].StartDate + "^" + WSIs[i].EndDate + "^"
                        + WSIs[i].Title + "^" + WSIs[i].Contents + "|";
                }
            }
            catch
            {
                error = WSI.WsiError;
            }
            //return Json(WSIs.Select(i => new {id = i.Id, title = i.Title, start = i.StartDate, end = i.EndDate
            //}));            
            return Json( new { Schedule = Schedule, error = error });
        }

        /// <summary>
        /// bao cao hang ngay
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyReports()
        {
            return PartialView();
        }

        public ActionResult DailyReportsUC(string taskId)
        {
            NMDailyReportsBL bl = new NMDailyReportsBL();
            NMDailyReportsWSI wsi = new NMDailyReportsWSI();
            wsi.Mode = "SRC_OBJ";
            if (!string.IsNullOrEmpty(taskId))
                wsi.DailyReport.TaskId = taskId;

            ViewData["WSIs"] = bl.callListBL(wsi);
            return PartialView();
        }

        public ActionResult DailyReport(string id, string mode, string windowId, string taskId)
        {
            NMDailyReportsBL BL = new NMDailyReportsBL();
            NMDailyReportsWSI WSI = new NMDailyReportsWSI();
            
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI.DailyReport.Id = id;
                WSI = BL.callSingleBL(WSI);
            }

            //if (mode == "Copy")
            //{
            //    WSI.DailyReport.Id = "";                
            //}

            ViewData["WSI"] = WSI;
            ViewData["WindowId"] = windowId;
            ViewData["TaskId"] = taskId;

            if (mode == "detail")
                return PartialView("DailyReportDetail");
            
            return PartialView();
        }
        
        public JsonResult SaveOrUpdateDailyReport(string id, string title, string advantage, string content, string hard, string promote, string taskId)
        {
            NMDailyReportsBL BL = new NMDailyReportsBL();
            NMDailyReportsWSI WSI = new NMDailyReportsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.DailyReport.Id = id;
            WSI.DailyReport.Title = title;
            WSI.DailyReport.Contents = content;
            WSI.DailyReport.Advantages = advantage;
            WSI.DailyReport.Hards = hard;
            WSI.DailyReport.Promotes = promote;
            WSI.Filter.ActionBy = User.Identity.Name;
            if (!string.IsNullOrEmpty(taskId))
                WSI.DailyReport.TaskId = taskId;

            WSI = BL.callSingleBL(WSI);

            if (WSI.WsiError != "")
            {
                if (!string.IsNullOrEmpty(WSI.DailyReport.TaskId))
                {
                    NMTasksBL tkBL = new NMTasksBL();
                    NMTasksWSI tkWSI = new NMTasksWSI();
                    tkWSI.Mode = "SEL_OBJ";
                    tkWSI.Task.TaskId = WSI.DailyReport.TaskId;
                    tkWSI = tkBL.callSingleBL(tkWSI);

                    if (tkWSI.Task.EndDate.Value < DateTime.Now)
                    {
                        tkWSI.Mode = "SAV_OBJ";
                        tkWSI.Task.IsReportTimeRight = false;
                        tkWSI = tkBL.callSingleBL(tkWSI);
                    }
                }
            }

            return Json(new { id = WSI.DailyReport.Id, error = WSI.WsiError });
        }

        public ActionResult Feeds(string sendTo)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Mode = "SRC_OBJ";
            
            WSI.Message.SendTo = User.Identity.Name;
            WSI.IsRead = false.ToString();

            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }
    }
}
