// UtilitiesController.cs

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
using NEXMI;

namespace nexmiStore.Controllers
{
    public class UtilitiesController : Controller
    {
        //
        // GET: /Utilities/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult SendEmail()
        {
            return PartialView();
        }

        public ActionResult ImagesList(String ownerId, string type)
        {
            NMImagesBL ImageBL = new NMImagesBL();
            NMImagesWSI ImageWSI = new NMImagesWSI();
            ImageWSI.Mode = "SRC_OBJ";
            if (!String.IsNullOrEmpty(ownerId))
            {
                ImageWSI.Owner = ownerId;
            }

            if (String.IsNullOrEmpty(type))
            {   
                type = NMConstant.FileTypes.Images;
            }
            ImageWSI.TypeId = type;
            List<NEXMI.NMImagesWSI> ImageWSIs = ImageBL.callListBL(ImageWSI);

            ViewData["ImageWSIs"] = ImageWSIs;

            ViewData["Type"] = type;
            
            return PartialView();
        }

        public ActionResult DocumentsList(String ownerId, string type)
        {
            return PartialView();
        }

        public ActionResult SystemParamList(string viewName)
        {
            NMSystemParamsBL BL = new NMSystemParamsBL();
            NMSystemParamsWSI WSI = new NMSystemParamsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.SystemParam.ObjectName = NMConstant.ObjectNames.Marketing;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView(viewName);
        }

        public ActionResult SystemParamForm(string id, string viewName)
        {
            //string name = "", subject = "", content = "", email = "", actionDate = "";
            NMSystemParamsWSI ParamWSI = new NMSystemParamsWSI();
            if (!string.IsNullOrEmpty(id))
            {
                NMSystemParamsBL ParamBL = new NMSystemParamsBL();
                ParamWSI.Mode = "SEL_OBJ";
                ParamWSI.SystemParam.Id = id;
                ParamWSI = ParamBL.callSingleBL(ParamWSI);
                //name = ParamWSI.SystemParam.Name;
                //subject = ParamWSI.SystemParam.Subject;
                //content = ParamWSI.SystemParam.Contents;
                //email = ParamWSI.SystemParam.Email;
                //actionDate = ParamWSI.SystemParam.ActionDate.ToString();
            }
            //ViewData["Id"] = id;
            //ViewData["Name"] = name;
            //ViewData["Subject"] = subject;
            //ViewData["Content"] = content;
            //ViewData["Email"] = email;
            //ViewData["ActionDate"] = actionDate;
            ViewData["WSI"] = ParamWSI;
            return PartialView(viewName);
        }

        public JsonResult SaveSystemParam(string id, string name, string subject, string actionDate, string autoSend,
            string group, string customerType, string gender, string content, string objectName, string type)
        {
            NMSystemParamsBL BL = new NMSystemParamsBL();
            NMSystemParamsWSI WSI = new NMSystemParamsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.SystemParam.Id = id;
            WSI.SystemParam.Name = name;
            WSI.SystemParam.Subject = subject;
            WSI.SystemParam.ActionDate = DateTime.Parse(actionDate);
            WSI.SystemParam.Value = int.Parse(autoSend);
            WSI.SystemParam.CustomerGroup = group;
            WSI.SystemParam.CustomerType = customerType;
            WSI.SystemParam.Gender = gender;
            WSI.SystemParam.Contents = content;
            WSI.SystemParam.ObjectName = objectName;
            WSI.SystemParam.Type = type;
            WSI.SystemParam.CreatedBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult DeleteSystemParam()
        {
            return Json("");
        }

        public ActionResult SearchBox(String objectName)
        {
            ViewData["ObjectName"] = objectName;
            return PartialView();
        }

        public ActionResult Search(String objectName, String keyWord, String fromDate, String toDate)
        {
            if(String.IsNullOrEmpty(keyWord))
                return PartialView("Error!");

            switch (objectName)
            {
                case "Customer":
                    // gọi hàm xữ lý tìm customer

                    break;
                case "Product":
                    break;
            }

            return PartialView();
        }
    }
}
