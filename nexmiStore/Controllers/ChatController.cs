// ChatController.cs

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
using System.Collections;
using System.Web.Security;
using NEXMI;

namespace nexmiStore.Controllers
{    
    public class ChatController : Controller
    {
        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChatUserList()
        {
            NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
            NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult OpenChatWindows(string ToUserId)
        {
            //UserService = new MRPUsersService();
            ViewData["ToUserId"] = ToUserId;
            ViewData["ToUserName"] = "";

            NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
            NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = ToUserId;
            WSI = BL.callSingleBL(WSI);

            //MRPUsers u = UserService.GetAllMrpusersByID(ToUserId, true);
            if (WSI.WsiError != "")
            {
                ViewData["ToUserName"] = WSI.Customer.CompanyNameInVietnamese;
            }
            return PartialView();
        }

        public static List<UserMessage> MessagesList = new List<UserMessage>();
        //public static List<UserMessage> ChattingList = new List<UserMessage>();
        // luu tin nhan của người gửi
        public JsonResult SaveMessage(string ToUser, string Message)
        {
            try
            {
                String UserID = User.Identity.Name;

                UserMessage userMessage = new UserMessage(UserID, ToUser, Message);

                MessagesList.Add(userMessage);

                return Json(null);
            }
            catch
            {
                return Json(null);
            }
        }

        public JsonResult GetMessage()//string fromUser)
        {
            SessionChecker.UpdateUsers(User.Identity.Name);
            if (MessagesList.Count > 0)
            {
                string Sender = "";
                string Msg = "";
                string Result = "";
                try
                {
                    String UserID = User.Identity.Name;
                    //int from = int.Parse(fromUser);

                    List<int> pos = new List<int>();

                    for (int i = 0; i < MessagesList.Count; i++)
                    {
                        // lay dung tin được gửi cho mình
                        if (MessagesList[i].Receiver == UserID)// && MessagesList[i].Sender == from)
                        {
                            Msg += MessagesList[i].Sender + "<:>";
                            Msg += MessagesList[i].Message + "<;>";
                            pos.Add(i);
                        }
                    }
                    //xóa những tin đã lấy
                    if (pos.Count > 0)
                    {
                        Sender = MessagesList[pos[0]].Sender.ToString();
                        for (int j = pos.Count - 1; j >= 0; j--)
                        {
                            MessagesList.RemoveAt(pos[j]);
                        }
                        //Result = "<strong>" + Sender + "</strong>: " + Msg;
                        Result = Msg;
                    }

                    return Json(Result);
                }
                catch
                {
                    return Json("");
                }
            }
            return Json("");            
        }

        //public JsonResult CheckMyMessage()
        //{
        //    // chỉ nên kiểm tra có message hay ko mà thôi
        //    if (MessagesList.Count > 0)
        //    {
        //        try
        //        {
        //            string Sender = "";
        //            int iSender = -1;
        //            int Username = int.Parse(User.Identity.Name);

        //            foreach (UserMessage umg in MessagesList.Where(u => u.Receiver == Username))
        //            {
        //                Sender = umg.Sender.ToString();
        //                break;
        //            }
        //            //if (iSender > 0)
        //            //{
        //            //    if (ChattingList.Count == 0)
        //            //    {
        //            //        UserMessage user = new UserMessage(iSender, Username, "");
        //            //        ChattingList.Add(user);
        //            //        Sender = user.Sender.ToString();
        //            //    }
        //            //    else
        //            //    {
        //            //        bool saved = false;
        //            //        for (int i = 0; i < ChattingList.Count; i++)
        //            //        {
        //            //            if ((ChattingList[i].Receiver == Username && ChattingList[i].Sender == iSender)
        //            //                || (ChattingList[i].Receiver == iSender && ChattingList[i].Sender == Username))
        //            //            {
        //            //                saved = true;
        //            //                Sender = "";
        //            //                break;
        //            //            }                            
                                
        //            //        }
        //            //        if(saved == false)
        //            //        {
        //            //            UserMessage user = new UserMessage(iSender, Username, "");
        //            //            ChattingList.Add(user);
        //            //            Sender = user.Sender.ToString();                                
        //            //        }
        //            //    }

        //            //}

        //            return Json(Sender);
        //        }
        //        catch
        //        {
        //            return Json("");
        //        }
        //    }
        //    return Json("");
        //}


        //public JsonResult StopChat(string chatUser)
        //{
        //    if (ChattingList.Count > 0)
        //    {
        //        int ichatUser = int.Parse(chatUser);
        //        int Username = int.Parse(User.Identity.Name);

        //        try
        //        {
        //            UserMessage user = ChattingList.Single(x => (x.Receiver == Username && x.Sender == ichatUser) ||
        //                (x.Receiver == ichatUser && x.Sender == Username));
        //            ChattingList.Remove(user);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    return Json(null);
        //}

        public JsonResult RefreshList()
        {
            NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
            NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            List<NMCustomersWSI> UserList = BL.callListBL(WSI);

            //UserService = new MRPUsersService();
            ArrayList UserCodeList = new ArrayList();
            //IList<MRPUsers> UserList = UserService.GetAllMrpusers(true);
            foreach (NMCustomersWSI Item in UserList)
            {
                if (Item.Customer.CustomerId != User.Identity.Name)
                {
                    if (SessionChecker.CheckExist(Item.Customer.CustomerId))
                    {
                        UserCodeList.Add(Item.Customer.CustomerId);
                    }
                }
            }
            String[] myArr = (String[])UserCodeList.ToArray(typeof(string));
            return Json(myArr);
        }
    }
    
}
