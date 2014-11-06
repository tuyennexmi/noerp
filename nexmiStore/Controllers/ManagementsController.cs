// ManagementsController.cs

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
    public class ManagementsController : Controller
    {
        //
        // GET: /Managements/

        public ActionResult Index()
        {
            return View();
        }

        #region Users
        public ActionResult Users()
        {
            NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
            NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult UserForm(String id, string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
                NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Customer = new Customers();
                WSI.Customer.CustomerId = id;
                WSI = BL.callSingleBL(WSI);
                ViewData["id"] = WSI.Customer.CustomerId;
                ViewData["code"] = WSI.Customer.Code;
                ViewData["password"] = NMCryptography.ECBDecrypt(WSI.Customer.Password);
                ViewData["name"] = WSI.Customer.CompanyNameInVietnamese;
                ViewData["phoneNumber"] = WSI.Customer.Cellphone;
                ViewData["Email"] = WSI.Customer.EmailAddress;
                ViewData["Gender"] = WSI.Customer.Gender;
                try
                {
                    ViewData["EmailPassword"] = NMCryptography.ECBDecrypt(WSI.Customer.EmailPassword);
                }
                catch { }
                if (WSI.Customer.Discontinued == false)
                {
                    ViewData["strStatus"] = "checked";
                }
                ViewData["slStocks"] = WSI.Customer.StockId;
            }
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateUser(String id, String name, String code, String password, String phoneNumber, string email, string emailPassword, string gender, String status, string stockId)
        {
            NMCustomersWSI WSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            WSI.Customer = new Customers();

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI.Customer.CustomerId = id;                
                WSI = BL.callSingleBL(WSI);
            }
            WSI.ActionBy = User.Identity.Name;
            if (WSI.Customer == null) WSI.Customer = new NEXMI.Customers();
            WSI.WsiError = "";
            WSI.Mode = "SAV_OBJ";
            WSI.Customer.CompanyNameInVietnamese = name;
            WSI.Customer.Code = code;
            WSI.Customer.Password = NMCryptography.ECBEncrypt(password);
            WSI.Customer.Cellphone = phoneNumber;
            WSI.Customer.EmailAddress = email;
            try
            {
                WSI.Customer.Gender = int.Parse(gender);
            }
            catch {
                WSI.Customer.Gender = null;
            }
            WSI.Customer.EmailPassword = NMCryptography.ECBEncrypt(emailPassword);
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            WSI.Customer.CustomerTypeId = NMConstant.CustomerTypes.Individual;
            try
            {
                WSI.Customer.Discontinued = !bool.Parse(status);
            }
            catch { WSI.Customer.Discontinued = true; }
            if (!String.IsNullOrEmpty(stockId))
            {
                WSI.Customer.StockId = stockId;
            }
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult DeleteUser(String id)
        {
            NMCustomersWSI WSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Permissions
        public ActionResult Permissions(String userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                NEXMI.NMCustomersBL UserBL = new NMCustomersBL();
                NEXMI.NMCustomersWSI UserWSI = new NEXMI.NMCustomersWSI();
                UserWSI.Mode = "SEL_OBJ";
                UserWSI.Customer = new Customers();
                UserWSI.Customer.CustomerId = userId;
                UserWSI = UserBL.callSingleBL(UserWSI);
                ViewData["UserId"] = UserWSI.Customer.CustomerId;
                if (UserWSI.Customer.Discontinued == false)
                {
                    ViewData["checkStatus"] = "checked";
                }
                NEXMI.NMModulesBL ModuleBL = new NEXMI.NMModulesBL();
                NEXMI.NMModulesWSI ModuleWSI = new NEXMI.NMModulesWSI();
                ModuleWSI.Mode = "SRC_OBJ";
                ModuleWSI.Active = "true";
                ViewData["ModuleWSIs"] = ModuleBL.callListBL(ModuleWSI);
            }
            ViewData["slUsers"] = userId;
            return PartialView();
        }

        public JsonResult ChangePermissions(string id, String userId, String functionId, String pSelect, String pInsert, String pUpdate, String pDelete,
            String approval, String viewAll, String calculate, String history, String pViewPrice, String pPrint, String pExport, String pDuplicate, string reconcile)
        {
            NMPermissionsBL BL = new NMPermissionsBL();
            NMPermissionsWSI WSI = new NMPermissionsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Id = id;
            WSI.UserId = userId;
            WSI.FunctionId = functionId;
            WSI.PSelect = pSelect;
            WSI.PInsert = pInsert;
            WSI.PUpdate = pUpdate;
            WSI.Reconcile = reconcile;
            WSI.PDelete = pDelete;

            WSI.ViewPrice = pViewPrice;
            WSI.PPrint = pPrint;
            WSI.Export = pExport;
            WSI.Duplicate = pDuplicate;

            WSI.Approval = approval;
            WSI.ViewAll = viewAll;
            WSI.Calculate = calculate;
            WSI.History = history;
            WSI.CreatedBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        public ActionResult SystemLogs()
        {
            return PartialView();
        }

        public ActionResult Interface()
        {
            ViewData["Logo"] = NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "logo");
            ViewData["Background"] = NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "background");
            ViewData["Footer1BG"] = NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer1bg").Substring(1);
            ViewData["Footer1"] = NMCryptography.base64Decode(NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer1"));
            ViewData["Footer2BG"] = NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer2bg").Substring(1);
            ViewData["Footer2"] = NMCryptography.base64Decode(NMCommon.GetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer2"));
            return PartialView();
        }

        public JsonResult SaveInterface(string logo, string background, string siteIcon, string footer1BG, string footer1, 
            string footer2BG, string footer2)
        {
            string error = "";
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "logo", logo);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "background", background);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "siteicon", siteIcon);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer1bg", footer1BG);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer1", footer1);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer2bg", footer2BG);
            error += NMCommon.SetXmlValue(Server.MapPath(@"~/parameters.xml"), "footer2", footer2);
            return Json(error);
        }
    }
}
