// AccountController.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *



using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using nexmiStore.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data.SqlTypes;
using NEXMI;

namespace nexmiStore.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            //if (Request.IsAuthenticated)
            //    return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(String userCode, String password, string lang, string returnUrl)
        {
            string error = "";
            if (userCode != "" && password != "")
            {
                Session["Lang"] = lang;

                NMCustomersWSI UserWSI = new NMCustomersWSI();
                NMCustomersBL UserBL = new NMCustomersBL();
                UserWSI.Mode = "SRC_OBJ";
                UserWSI.Customer = new Customers();

                if (userCode.Split('@').Count() > 1)
                    UserWSI.Customer.EmailAddress = userCode;
                else
                    UserWSI.Customer.Code = userCode;

                UserWSI.Customer.Password = NMCryptography.ECBEncrypt(password);
                List<NMCustomersWSI> UserWSIs = UserBL.callListBL(UserWSI);
                if (UserWSIs.Count > 0)
                {
                    UserWSI = UserWSIs[0];                    
                    if (UserWSI.Customer.Discontinued == false)
                    {
                        NMPermissionsBL PermissionBL = new NMPermissionsBL();
                        NMPermissionsWSI PermissionWSI = new NMPermissionsWSI();
                        PermissionWSI.Mode = "SRC_OBJ";
                        PermissionWSI.UserId = UserWSI.Customer.CustomerId;
                        //GetPermissions.Permissions = PermissionBL.callListBL(PermissionWSI);
                        Session["Permissions"] = PermissionBL.callListBL(PermissionWSI);
                        Session["UserInfo"] = UserWSI;
                        FormsService.SignIn(UserWSI.Customer.CustomerId, true);
                        SessionChecker.UpdateUsers(UserWSI.Customer.CustomerId);
                        //NMTimeKeepingBL TKBL = new NMTimeKeepingBL();
                        //NMTimeKeepingWSI TKWSI = new NMTimeKeepingWSI();
                        //TKWSI.Mode = "SAV_OBJ";
                        //TKWSI.UserId = UserWSI.Id;
                        //TKWSI.StartTime = DateTime.Now;
                        //TKWSI.EndTime = (DateTime)SqlDateTime.MinValue;
                        //TKWSI = TKBL.callBussinessLogic(TKWSI);

                        //if (SessionChecker.CheckExist(UserWSI.Customer.CustomerId) == false)
                        //{
                            
                        //}
                        //else
                        //{
                        //    error = "";
                        //    //error = "Tài khoản <b>" + UserWSI.Customer.CompanyNameInVietnamese + "</b> đang được sử dụng. Thử lại sau ít phút.";
                        //}
                    }
                    else
                    {
                        error = "Tài khoản <b>" + userCode + "</b> đã bị khóa.";
                    }
                }
                else
                {
                    error = "Thông tin đăng nhập không đúng!";
                }
            }
            else
            {
                error = "Thông tin đăng nhập không đúng!";
            }
            if (error == "")
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(Url.Content("~/Home/Index") + NMCryptography.base64Decode(returnUrl));
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["userCode"] = userCode;
                ViewData["strError"] = error;
                return View();
            }
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff(string returnUrl)
        {
            //Session["Permissions"] = null;
            //Session["UserInfo"] = null;            
            SessionChecker.RemoveUser(User.Identity.Name);
            FormsService.SignOut();
            Session.Abandon();
            return RedirectToAction("LogOn", "Account", new { returnUrl = returnUrl });
        }

        public ActionResult UserInfo()
        {
            NEXMI.NMCustomersWSI WSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];
            ViewData["id"] = WSI.Customer.CustomerId;
            ViewData["code"] = WSI.Customer.Code;
            ViewData["password"] = WSI.Customer.Password;
            ViewData["name"] = WSI.Customer.CompanyNameInVietnamese;
            ViewData["phoneNumber"] = WSI.Customer.Cellphone;
            if (WSI.Customer.Discontinued == false)
            {
                ViewData["strStatus"] = "checked";
            }
            ViewData["StockId"] = WSI.Customer.StockId;
            return View();
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        public ActionResult ChangePassword()
        {
            ViewData["Message"] = "";
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string OldPass, string NewPass, string ConfirmNewPass)
        {            
            //NMCustomersWSI WSI = new NMCustomersWSI();
            //WSI.Mode = "SEL_OBJ";
            //WSI.Customer = new Customers();
            //WSI.Customer.CustomerId = User.Identity.Name;
            //WSI = BL.callSingleBL(WSI);
            NMCustomersWSI WSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];  // UserInfoCache.User;
            if (WSI.Customer.Password == OldPass)
            {
                NMCustomersBL BL = new NMCustomersBL();
                WSI.Mode = "SAV_OBJ";
                WSI.Customer.Password = ConfirmNewPass;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    ViewData["Message"] = "Mật khẩu đã được thay đổi.";
                }
                else
                {
                    ViewData["Message"] = WSI.WsiError;
                }
            }
            else
            {
                ViewData["Message"] = "Mật khẩu cũ không đúng.";
            }
            return View();
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public JsonResult RequestAfterTimerTick()
        {
            SessionChecker.UpdateUsers(User.Identity.Name);
            return Json("");
        }

        //public bool Get(string functionId, string action)
        //{
        //    List<NMPermissionsWSI> Permissions = (List<NMPermissionsWSI>)Session["Permissions"];
        //    bool isOK = false;
        //    var Permission = Permissions.Select(i => i).Where(i => i.FunctionId == functionId).FirstOrDefault();
        //    if (Permission != null)
        //    {
        //        if (action == "")
        //        {
        //            if (Permission.PSelect == "Y" || Permission.PInsert == "Y" || Permission.PUpdate == "Y" || Permission.PDelete == "Y"
        //                || Permission.Approve == "Y" || Permission.ViewAll == "Y" || Permission.Calculate == "Y" || Permission.History == "Y")
        //            {
        //                isOK = true;
        //            }
        //        }
        //        else
        //        {
        //            switch (action)
        //            {
        //                case "Select": if (Permission.PSelect == "Y") isOK = true; break;
        //                case "Insert": if (Permission.PInsert == "Y") isOK = true; break;
        //                case "Update": if (Permission.PUpdate == "Y") isOK = true; break;
        //                case "Delete": if (Permission.PDelete == "Y") isOK = true; break;
        //                case "Approve": if (Permission.Approve == "Y") isOK = true; break;
        //                case "ViewAll": if (Permission.ViewAll == "Y") isOK = true; break;
        //                case "Calculate": if (Permission.Calculate == "Y") isOK = true; break;
        //                case "History": if (Permission.History == "Y") isOK = true; break;

        //                case "ViewPrice": if (Permission.ViewPrice == "Y") isOK = true; break;
        //                case "Export": if (Permission.Export == "Y") isOK = true; break;
        //                case "PPrint": if (Permission.PPrint == "Y") isOK = true; break;
        //                case "Duplicate": if (Permission.Duplicate == "Y") isOK = true; break;
        //            }
        //        }
        //    }
        //    return isOK;
        //}
    }
}
