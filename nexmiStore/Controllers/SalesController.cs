// SalesController.cs

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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using NEXMI;
using System.Web.Routing;

namespace nexmiStore.Controllers
{
    public class SalesController : Controller
    {
        //
        // GET: /Sales/

        #region Customers
      
        public ActionResult Customers(string groupId)
        {
            ViewData["ViewType"] = "kanban";
            if (!String.IsNullOrEmpty(groupId))
            {
                ViewData["GroupId"] = groupId;
            }
            else
            {
                ViewData["GroupId"] = NMConstant.CustomerGroups.Customer;
            }
            if (groupId == NEXMI.NMConstant.CustomerGroups.Customer)
            {
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.Customer;
            }
            else if (groupId == NEXMI.NMConstant.CustomerGroups.Manufacturer)
            {
                ViewData["FunctionId"]= NEXMI.NMConstant.Functions.Manufacturers;
            }
            else if (groupId == NEXMI.NMConstant.CustomerGroups.Supplier)
            {
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.Suppilers;
            }
            else if (groupId == NEXMI.NMConstant.CustomerGroups.User)
            {
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.Users;
            }
            else
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.Customer;
            
            return PartialView();
        }

        public ActionResult CustomerDetail(String id, String groupId, string mode)
        {
            NMCustomersBL BL = new NMCustomersBL();
            NMCustomersWSI WSI = new NMCustomersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = id;
            WSI = BL.callSingleBL(WSI);

            ViewData["Customer"] = WSI;

            ViewData["CustomerId"] = WSI.Customer.CustomerId;            
            ViewData["CustomerCode"] = WSI.Customer.Code;
            ViewData["CompanyNameInVietnamese"] = WSI.Customer.CompanyNameInVietnamese;
            try
            {
                ViewData["DateOfBirth"] = WSI.Customer.DateOfBirth.Value.ToString("dd-MM-yyyy");
            }
            catch { }
            ViewData["TaxCode"] = WSI.Customer.TaxCode;
            ViewData["Address"] = WSI.Customer.Address;
            ViewData["Cellphone"] = WSI.Customer.Cellphone;
            ViewData["Area"] = WSI.AreaWSI.FullName;
            ViewData["Telephone"] = WSI.Customer.Telephone;
            ViewData["FaxNumber"] = WSI.Customer.FaxNumber;
            ViewData["EmailAddress"] = WSI.Customer.EmailAddress;
            ViewData["Website"] = WSI.Customer.Website;
            
            ViewData["Description"] = WSI.Customer.Description;
            ViewData["Latitude"] = WSI.Customer.Latitude;
            ViewData["Longitude"] = WSI.Customer.Longitude;
            ViewData["BonusPoints"] = WSI.Customer.BonusPoints;
            ViewData["SendTo"] = WSI.Customer.CreatedBy;    // dùng để gửi tin nhắn đến người tạo
            if (WSI.ManagedBy != null)
            {
                if(WSI.ManagedBy.CustomerId != WSI.Customer.CreatedBy)  //người tạo khác người quản lý
                    ViewData["SendTo"] += ";" + WSI.ManagedBy.CustomerId;   //gửi tin đến người quản lý
                ViewData["ManagedBy"] = WSI.ManagedBy.CompanyNameInVietnamese;
            }
            ViewData["DueDate"] = WSI.Customer.DueDate;            
            ViewData["MaxDebitAmount"] = WSI.Customer.MaxDebitAmount.ToString("N3");
            
            if (!string.IsNullOrEmpty(WSI.Customer.Avatar))
            {
                ViewData["avatar"] = Url.Content("~/Content/avatars/") + WSI.Customer.Avatar;
            }
            else
            {
                if (WSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                {
                    ViewData["avatar"] = Url.Content("~/Content/avatars/company.png");
                }
                else
                {
                    ViewData["avatar"] = Url.Content("~/Content/avatars/personal.png");
                }
            }
            if (WSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
            {
                ViewData["DOBName"] = NEXMI.NMCommon.GetInterface("ESTABLISH_DATE", Session["Lang"].ToString());
            }
            else
            {
                ViewData["DOBName"] = NMCommon.GetInterface("BIRTHDAY", Session["Lang"].ToString());
            }
            ViewData["ContactPersons"] = WSI.ContactPersons;
            ViewData["SalesInvoicesWSI"] = WSI.SalesInvoicesWSI;
            ViewData["KOI"] = GlobalValues.GetType(WSI.Customer.KindOfIndustryId);
            ViewData["KOE"] = GlobalValues.GetType(WSI.Customer.KindOfEnterpriseId);
            ViewData["GroupId"] = groupId;
            ViewData["ViewType"] = "detail";

            if(!string.IsNullOrEmpty(mode))
                return PartialView("CustomerPopup");

            return PartialView();
        }

        public ActionResult CustomerList(string pageNum, string keyword, string area, string groupId, string functionId, string viewType)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }

            NMCustomersWSI CustomerWSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            CustomerWSI.Mode = "SRC_OBJ";
            CustomerWSI.Customer = new Customers();
            
            if (String.IsNullOrEmpty(groupId) | groupId == "undefined")
            {   
                groupId = NEXMI.NMConstant.CustomerGroups.Customer;
            }
            
            CustomerWSI.Customer.GroupId = groupId;
            ViewData["GroupId"] = groupId;           

            if (GetPermissions.GetViewAll((List < NMPermissionsWSI >)Session["Permissions"], functionId) == false)
            {
                CustomerWSI.Customer.ManagedBy = User.Identity.Name;
            }
            CustomerWSI.PageNum = page - 1;
            CustomerWSI.PageSize = NMCommon.PageSize();
            //keyword = NMCryptography.base64Decode(keyword);
            if (!String.IsNullOrEmpty(keyword))
            {
                CustomerWSI.Keyword = keyword;
            }

            //tìm theo khu vực
            if (!string.IsNullOrEmpty(area))
            {
                CustomerWSI.Customer.AreaId = area;
                ViewData["cbbAreas"] = area;
                //ViewData["cbbAreaName"] = CustomerWSI.AreaWSI.FullName;
            }
            
            List<NEXMI.NMCustomersWSI> Customers = BL.callListBL(CustomerWSI);

            ViewData["Customers"] = Customers;

            if (String.IsNullOrEmpty(functionId))
            {
                if (groupId == NMConstant.CustomerGroups.Customer)
                    functionId = NMConstant.Functions.Customer;
                else if (groupId == NMConstant.CustomerGroups.Manufacturer)
                    functionId = NMConstant.Functions.Manufacturers;
                else if (groupId == NMConstant.CustomerGroups.Supplier)
                    functionId = NMConstant.Functions.Suppilers;
                else if (groupId == NMConstant.CustomerGroups.User)
                    functionId = NMConstant.Functions.Users;
            }
            ViewData["FunctionId"] = functionId;
            
            ViewData["Page"] = page;
            if (Customers.Count > 0)
                ViewData["TotalRows"] = Customers[0].TotalRows;
            else
                ViewData["TotalRows"] = "";

            string view = "";
            if (viewType == "kanban")
            {
                view = "CustomerTags";
                ViewData["ViewType"] = "kanban";
            }
            else
            {
                view = "CustomerList";
                ViewData["ViewType"] = "list";
            }
            
            Session["Keyword"] = keyword;

            return PartialView(view);
        }

        public ActionResult CustomerForm(String id, String groupId, string parentElement, string windowId)
        {
            if (string.IsNullOrEmpty(groupId))
                groupId = NMConstant.CustomerGroups.Customer;

            Session["ContactPersons"] = null;
            if (!string.IsNullOrEmpty(id))
            {
                NMCustomersBL BL = new NMCustomersBL();
                NMCustomersWSI WSI = new NMCustomersWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Customer = new Customers();
                WSI.Customer.CustomerId = id;
                WSI = BL.callSingleBL(WSI);

                if (WSI.Customer != null)
                {
                    ViewData["id"] = WSI.Customer.CustomerId;
                    ViewData["name"] = WSI.Customer.CompanyNameInVietnamese;
                    ViewData["code"] = WSI.Customer.Code;
                    try
                    {
                        ViewData["DateOfBirth"] = WSI.Customer.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        ViewData["DateOfBirth"] = "";
                    }
                    if (WSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                    {
                        ViewData["strChecked"] = "checked";
                    }
                    ViewData["UserId"] = WSI.Customer.ManagedBy;
                    ViewData["taxCode"] = WSI.Customer.TaxCode;
                    ViewData["address"] = WSI.Customer.Address;
                    ViewData["telephone"] = WSI.Customer.Telephone;
                    ViewData["cellphone"] = WSI.Customer.Cellphone;
                    ViewData["faxNumber"] = WSI.Customer.FaxNumber;
                    ViewData["email"] = WSI.Customer.EmailAddress;
                    ViewData["website"] = WSI.Customer.Website;
                    ViewData["description"] = WSI.Customer.Description;
                    ViewData["KOI"] = WSI.Customer.KindOfIndustryId;
                    ViewData["KOE"] = WSI.Customer.KindOfEnterpriseId;
                    ViewData["BonusPoints"] = WSI.Customer.BonusPoints;
                    groupId = WSI.Customer.GroupId;
                    ViewData["maxDebitAmount"] = WSI.Customer.MaxDebitAmount.ToString("N3");

                    if (!string.IsNullOrEmpty(WSI.Customer.Avatar))
                        ViewData["avatar"] = Url.Content("~/Content/avatars/") + WSI.Customer.Avatar;
                    try
                    {
                        ViewData["dueDate"] = WSI.Customer.DueDate.Value.ToString("yyyy-MM-dd");
                    }
                    catch { }
                    try
                    {
                        ViewData["AreaId"] = WSI.AreaWSI.Area.Id;
                        ViewData["AreaName"] = WSI.AreaWSI.Area.Name;
                    }
                    catch { }

                    if (groupId == "G004")
                        ViewData["slStocks"] = WSI.Customer.StockId;
                    Session["ContactPersons"] = WSI.ContactPersons;
                }
            }
            else
            {
                if (NMCommon.GetSetting("DEFAULT_CUSTOMER_TYPE") == true)
                    ViewData["strChecked"] = "checked";
            }
            ViewData["GroupId"] = groupId;
            ViewData["ViewType"] = "detail";
            ViewData["ParentElement"] = parentElement;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateCustomer(String id, String name, string typeId, String taxCode, String address, String telephone, string cellphone, String faxNumber, 
            string groupId, String email, String description, string maxDebitAmount, string areaId, string fileName, string imageUrl, string managedBy, string dueDate,
            string website, string code, string dateDOB, string KOI, string KOE, string bonusPoints)
        {
            String result = "", error = "";
            NMCustomersWSI WSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = id;
            
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }
            
            if (WSI.Customer == null) WSI.Customer = new NEXMI.Customers();
            WSI.WsiError = "";
            WSI.Mode = "SAV_OBJ";
            WSI.Customer.CompanyNameInVietnamese = name;
            WSI.Customer.Code = code;
            try
            {
                WSI.Customer.DateOfBirth = DateTime.Parse(dateDOB);
            }
            catch { }
            if (typeId == "PERSONAL")
            {
                WSI.Customer.CustomerTypeId = NMConstant.CustomerTypes.Individual;
            }
            else
            {
                WSI.Customer.CustomerTypeId = NMConstant.CustomerTypes.Enterprise;
            }
            WSI.Customer.GroupId = groupId;
            if (WSI.Customer.GroupId == NMConstant.CustomerGroups.Customer)
                WSI.TypeName = "Customers";
            else if (WSI.Customer.GroupId == NMConstant.CustomerGroups.Manufacturer)
                WSI.TypeName = "Manufacturers";
            else if (WSI.Customer.GroupId == NMConstant.CustomerGroups.Supplier)
                WSI.TypeName = "Suppliers";
            else
                WSI.TypeName = "Users";
            if (!string.IsNullOrEmpty(KOI))
                WSI.Customer.KindOfIndustryId = KOI;
            else
                WSI.Customer.KindOfIndustryId = null;
            if (!string.IsNullOrEmpty(KOE))
                WSI.Customer.KindOfEnterpriseId = KOE;
            else
                WSI.Customer.KindOfEnterpriseId = null;
            WSI.Customer.TaxCode = taxCode;
            WSI.Customer.Address = address;
            WSI.Customer.Telephone = telephone;
            WSI.Customer.Cellphone = cellphone;
            WSI.Customer.FaxNumber = faxNumber;
            WSI.Customer.EmailAddress = email;
            WSI.Customer.Description = description;
            WSI.Customer.Website = website;
            try
            {
                WSI.Customer.MaxDebitAmount = double.Parse(maxDebitAmount);
            }
            catch { }
            if (!string.IsNullOrEmpty(areaId))
            {
                WSI.Customer.AreaId = areaId;
            }
            if (fileName != "")
            {
                UserControlController UC = new UserControlController();
                WSI.Customer.Avatar = UC.Upload(imageUrl, Server.MapPath(@"~/Content/avatars/"), fileName);
            }
            if(!string.IsNullOrEmpty(managedBy))
            {
                WSI.Customer.ManagedBy = managedBy;
            }
            try
            {
                WSI.Customer.DueDate = DateTime.Parse(dueDate);
            }
            catch { }
            WSI.ContactPersons = (IList<Customers>)Session["ContactPersons"];
            try
            {
                WSI.Customer.BonusPoints = int.Parse(bonusPoints);
            }
            catch { }
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            result = WSI.Customer.CustomerId;

            error = WSI.WsiError;
            return Json(new { result = result, error = error });
        }

        public JsonResult DeleteCustomer(String id)
        {
            NMCustomersWSI WSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult CustomerProjects(string id)
        {

            return PartialView();
        }

        #endregion

        #region Quotations
      
        public ActionResult Quotations()
        {
            ViewData["ViewType"] = "list";
            return PartialView();
        }
        
        public ActionResult PrintQuotation(string id, string mode, string lang)
        {   
            NMProductsBL ProductBL = new NMProductsBL();
            NMProductsWSI ProductWSI;
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            WSI.Order.OrderId = id;
            WSI = BL.callSingleBL(WSI);

            if (WSI.WsiError == "")
            {   
                ViewData["SOWSI"] = WSI;
                
                if (String.IsNullOrEmpty(lang))
                    lang = "vi";
                ViewData["Lang"] = lang;
                if (WSI.Order.OrderTypeId == NEXMI.NMConstant.SOType.Quotation)
                {
                    ViewData["Title"] = NMCommon.GetInterface("QUOTATION_TITLE", lang);
                }
                else
                {
                    ViewData["Title"] = NMCommon.GetInterface("SO_TITLE", lang);
                }

                if (WSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                {
                    NMCustomersBL CBL = new NMCustomersBL();
                    NMCustomersWSI CWSI = new NMCustomersWSI();
                    CWSI.Mode = "SEL_OBJ";
                    CWSI.Customer = new Customers();
                    CWSI.Customer.CustomerId = WSI.Customer.CustomerId;
                    CWSI = CBL.callSingleBL(CWSI);
                    if (CWSI.ContactPersons.Count > 0)
                    {
                        ViewData["CustomerAntt"] = CWSI;
                    }
                }

                List<NMProductsWSI> ProductList = new List<NMProductsWSI>();
                foreach (SalesOrderDetails Item in WSI.Details)
                {
                    ProductWSI = new NMProductsWSI();
                    ProductWSI.Mode = "SEL_OBJ";
                    ProductWSI.Product = new Products();
                    ProductWSI.Product.ProductId = Item.ProductId;
                    ProductWSI = ProductBL.callSingleBL(ProductWSI);
                    ProductList.Add(ProductWSI);
                }

                ViewData["ProductList"] = ProductList;
            }
            
            ViewData["ViewType"] = "detail";
            string formName = "PrintQuotation";

            switch (mode)
            {
                case "QUOTE":
                    int selectForm = NMCommon.GetSettingValue("PRINT_QUOTATION_FORM");

                    if (selectForm == 1)
                        formName = "PrintQuotation_1";
                    break;
                case "CONTRACT":
                    formName ="PrintContract";
                    break;
                case"APPDIX":
                    formName = "PrintAppendix";
                    break;
                case "PAYREQ":
                    formName = "PrintPaymentRequest";
                    break;
                case "CNTO":    //  check and take over
                    formName = "PrintCheckAndTakeOver";
                    break;
            }

            return PartialView(formName);
        }

        #endregion

        #region SalesOrders

        public ActionResult SalesOrders(String SOType)
        {
            ViewData["ViewType"] = "list";
            if (!String.IsNullOrEmpty(SOType)){
                ViewData["SOType"] = SOType;
                ViewData["SOFunc"] = NEXMI.NMConstant.Functions.Quotation;
                ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Draft;
            }
            else
            {
                ViewData["SOType"] = NEXMI.NMConstant.SOType.SalesOrder;
                ViewData["SOFunc"] = NEXMI.NMConstant.Functions.SalesOrder;
                ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Order;
            }

            return PartialView();
        }

        public ActionResult SalesOrderList(string pageNum, string status, string keyword, string typeId, 
                                           string partnerId, string from, string to, string mode)
        {
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "SRC_OBJ";
            
            WSI.Order.OrderTypeId = typeId;
            WSI.Order.OrderGroup = NMConstant.SOrderGroups.Sale;
            if (typeId == NMConstant.SOType.Quotation)
            {
                if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Quotation, "ViewAll") == false) WSI.ActionBy = User.Identity.Name;
            }
            else
                if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.SalesOrder, "ViewAll") == false) WSI.ActionBy = User.Identity.Name;
            //if(!string.IsNullOrEmpty(pageNum))
            //    WSI.PageNum = int.Parse(pageNum);
            if (mode != "Print")
            {
                int page = 1;
                try
                {
                    page = int.Parse(pageNum);
                }
                catch { }
                WSI.PageNum = page - 1;
                WSI.PageSize = NMCommon.PageSize();
            }
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            //else
            //{
            //    double days = NMCommon.GetSettingValue(NMConstant.Settings.DF_LOAD_LIST_DAYS);
            //    WSI.FromDate = DateTime.Today.AddDays(-days).ToString();
            //}
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;
            //else
            //    WSI.ToDate = DateTime.Today.ToString();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Order.OrderStatus = status;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Order.CustomerId = partnerId;

            List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;
            ViewData["SOType"] = typeId;
            
            if (string.IsNullOrEmpty(mode))
            {
                mode = "";
            }
            ViewData["Mode"] = mode;

            if(NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == false)
                return PartialView("SOList4OneLineType");
            return PartialView();
        }

        public ActionResult SalesOrderForm(string id, string mode, string type, string viewName)
        {
            if (mode == null) mode = "";
            string status = "";
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            if (id != "")
            {
                WSI.Order.OrderId = id;
                WSI = BL.callSingleBL(WSI);                
                if (WSI.WsiError == "")
                {
                    type = WSI.Order.OrderTypeId;
                    status = WSI.Order.OrderStatus;
                    if (mode == "Copy")
                    {
                        WSI.Order.OrderId = "";
                        WSI.Order.OrderDate = DateTime.Now;
                        status = "";
                    }
                    foreach (SalesOrderDetails Item in WSI.Details)
                    {
                        if (mode == "Copy") Item.Id = 0;
                    }
                    Session["SalesOrder"] = WSI;
                }
            }
            else
            {
                WSI.Order.OrderDate = DateTime.Now;
                WSI.Order.DeliveryDate = DateTime.Now.AddDays(7);
                WSI.Order.OrderTypeId = type;
                WSI.Order.OrderGroup = NMConstant.SOrderGroups.Sale;
                WSI.Order.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                WSI.Order.SalesPersonId = User.Identity.Name;
                WSI.Details = new List<SalesOrderDetails>();
                
            }
            if (type == NEXMI.NMConstant.SOType.Quotation)
            {
                ViewData["Tilte"] = NEXMI.NMCommon.GetInterface("QUOTATION_TITLE", Session["Lang"].ToString());
                ViewData["FunctionId"] = NMConstant.Functions.Quotation;
                if (status == "") WSI.Order.OrderStatus = NMConstant.SOStatuses.Draft;
            }
            else
            {
                ViewData["Tilte"] = NEXMI.NMCommon.GetInterface("SO_TITLE", Session["Lang"].ToString());
                ViewData["FunctionId"] = NMConstant.Functions.SalesOrder;
                if (status == "") WSI.Order.OrderStatus = NMConstant.SOStatuses.Order;
            }
            Session["Details"] = WSI.Details;
            ViewData["WSI"] = WSI;
            ViewData["Mode"] = mode;
            ViewData["ViewType"] = "detail";

            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER);
            ViewData["SalerInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_SALER_BY_USER);

            return PartialView(viewName);
        }

        public ActionResult SalesOrderDetailForm(string productId)
        {
            List<SalesOrderDetails> Details = new List<SalesOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<SalesOrderDetails>)Session["Details"];
            }
            foreach (SalesOrderDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }

            ViewData["PriceInput"] = NMCommon.GetSetting(NMConstant.Settings.PRICE_BY_USER);

            return PartialView();
        }

        public ActionResult AllProductsForm()
        {
            return PartialView();
        }

        public ActionResult EditSODetailDescriptions(string productId, string windowId)
        {
            List<SalesOrderDetails> Details = new List<SalesOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<SalesOrderDetails>)Session["Details"];
            }
            SalesOrderDetails detail = Details.Find(i => i.ProductId == productId);
            if ( detail == null)
            {
                NMProductsBL ProductBL = new NMProductsBL();
                NMProductsWSI ProductWSI = new NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = productId;

                ViewData["Description"] = ProductBL.callSingleBL(ProductWSI).Translation.Description;
            }
            else
            {
                ViewData["Description"] = detail.Description;
            }

            ViewData["WindowId"] = windowId;

            return PartialView();
        }

        public JsonResult CheckDebit(string customerId, string maxDebitAmount)
        {
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";
            WSI.MGJ.PartnerId = customerId;
            
            List<NEXMI.NMMonthlyGeneralJournalsWSI> MGJs = BL.callListBL(WSI);
            string status = "";
            if (MGJs.Count > 0)
            {                
                double begin = 0, buyinmonth = 0, paid = 0, debit = 0;
                double totalBegin = 0, totalBuy = 0, totalPaid = 0, totalDebit = 0;                

                begin = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == true)).Sum(i => i.MGJ.DebitAmount);
                buyinmonth = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.DebitAmount);
                paid = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.CreditAmount);
                debit = buyinmonth - paid;
                totalBegin += begin;
                totalBuy += buyinmonth;
                totalPaid += paid;
                totalDebit += debit;
                if (debit > double.Parse(maxDebitAmount))
                    status = "Y";
                else
                    status = "N";
            }

            return Json(status);
        }

        public JsonResult ChangeStatus(string id, string status, string typeId)
        {
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            //WSI = (NEXMI.NMSalesOrdersWSI)Session["SalesOrder"];
            
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Order.OrderId = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    WSI.Mode = "CONFIRM";
                    //WSI.Order.OrderId = id;
                    WSI.Order.OrderStatus = status;
                    WSI.Order.OrderTypeId = typeId;
                    WSI.Order.ApproveBy = User.Identity.Name;
                    WSI.ActionBy = User.Identity.Name;
                    WSI = BL.callSingleBL(WSI);
                }
            }
            else
            {
                WSI.WsiError = "Bạn chưa nhập đối tượng!";
            }

            return Json(WSI.WsiError);
        }

        public JsonResult ApprovalQuotation(string id)
        {
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI = (NEXMI.NMSalesOrdersWSI)Session["SalesOrder"];

            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Order.OrderId = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    WSI.Mode = "APP_OBJ";
                    WSI.Order.OrderStatus = NEXMI.NMConstant.SOStatuses.Approval;
                    WSI.Order.ApproveBy = User.Identity.Name;
                    WSI.ActionBy = User.Identity.Name;
                    WSI = BL.callSingleBL(WSI);
                }
            }
            else
            {
                WSI.WsiError = "Bạn chưa nhập đối tượng!";
            }
            
            return Json(WSI.WsiError);
        }
        
        public ActionResult PrintOrder(string id)
        {
            ArrayList objs = new ArrayList();
            ArrayList obj;
            string customerId = "", customerName = "", orderDate = DateTime.Now.ToString("dd-MM-yyyy"), reference = "",
                paymentTerm = "", advances = "", amount = "0", totalAmount = "0", discount = "0", tax = "0", customerAddress = "";
            NMProductsBL ProductBL = new NMProductsBL();
            NMProductsWSI ProductWSI;
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            WSI.Order.OrderId = id;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                customerId = WSI.Customer.CustomerId; customerName = WSI.Customer.CompanyNameInVietnamese; customerAddress = WSI.Customer.Address;
                orderDate = WSI.Order.OrderDate.ToString("dd-MM-yyyy"); reference = WSI.Order.Reference;
                amount = WSI.Order.Amount.ToString("N3"); 
                totalAmount = WSI.Order.TotalAmount.ToString("N3");
                discount = WSI.Order.Discount.ToString("N3"); 
                tax = WSI.Order.Tax.ToString("N3");
                ViewData["slUsers"] = WSI.SalesPerson.CompanyNameInVietnamese;
                ViewData["slIncoterms"] = WSI.Incoterm.Name;
                ViewData["slShippingPolicy"] = WSI.ShippingPolicy.Name;
                ViewData["slCreateInvoice"] = WSI.CreateInvoice.Name;
                ViewData["OrderStatus"] = WSI.Order.OrderStatus;
                ViewData["Description"] = WSI.Order.Description;
                paymentTerm = WSI.Order.PaymentTerm; advances = WSI.Order.Advances;
                foreach (SalesOrderDetails Item in WSI.Details)
                {
                    ProductWSI = new NMProductsWSI();
                    ProductWSI.Mode = "SEL_OBJ";
                    ProductWSI.Product = new Products();
                    ProductWSI.Product.ProductId = Item.ProductId;
                    ProductWSI = ProductBL.callSingleBL(ProductWSI);
                    obj = new ArrayList();
                    obj.Add(WSI.Details.IndexOf(Item));
                    obj.Add(Item.Id); obj.Add(ProductWSI.Product.ProductId);
                    obj.Add((ProductWSI.Translation == null) ? "" : ProductWSI.Translation.Name);
                    obj.Add(Item.Quantity.ToString("N3")); obj.Add(Item.Price.ToString("N3"));
                    obj.Add(Item.Discount); obj.Add(Item.Tax); obj.Add(Item.Amount.ToString("N3"));
                    objs.Add(obj);
                }
            }
            ViewData["Id"] = id;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["CustomerAddress"] = customerAddress;
            ViewData["OrderDate"] = orderDate;
            ViewData["Reference"] = reference;
            ViewData["Amount"] = amount;
            ViewData["Discount"] = discount;
            ViewData["Tax"] = tax;
            ViewData["TotalAmount"] = totalAmount;
            ViewData["PaymentTerm"] = paymentTerm;
            ViewData["Advances"] = advances;
            ViewData["objs"] = objs;
            Session["QuotationDetails"] = objs;
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult AddSalesOrderDetail(string id, string productId, string  unitId, string description, string quantity,
                                                string price, string discount, string VATRate)
        {
            String strError = "";
            List<SalesOrderDetails> Details = new List<SalesOrderDetails>();
            double detailAmount = 0;
            try
            {
                if (Session["Details"] != null)
                {
                    Details = (List<SalesOrderDetails>)Session["Details"];
                }
                
                SalesOrderDetails Detail;
                int intId = int.Parse(id);
                if (intId > 0)
                    Detail = Details.Find(d => d.Id == intId);
                else
                    Detail = Details.Find(d => d.ProductId == productId);
                if (Detail == null)
                {
                    Detail = new SalesOrderDetails();
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.Price = double.Parse(price);
                    Detail.Discount = double.Parse(discount);
                    Detail.Tax = double.Parse(VATRate);
                    Detail.Description = NMCryptography.base64Decode(description);
                    detailAmount = Detail.Amount;
                    Details.Add(Detail);
                }
                else
                {
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.Price = double.Parse(price);
                    Detail.Discount = double.Parse(discount);
                    Detail.Tax = double.Parse(VATRate);
                    Detail.Description = NMCryptography.base64Decode(description);
                    detailAmount = Detail.Amount;
                }
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }

            return PartialView("SalesOrderDetails");
        }

        public JsonResult RemoveSalesOrderDetail(string productId)
        {
            List<SalesOrderDetails> Details = new List<SalesOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<SalesOrderDetails>)Session["Details"];
            }
            foreach (SalesOrderDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    Details.Remove(Item);
                    break;
                }
            }
            return Json(new {
                amount = Details.Sum(i => i.Amount).ToString("N3"),
                taxAmount = Details.Sum(i => i.TaxAmount).ToString("N3"),
                discountAmount = Details.Sum(i => i.DiscountAmount).ToString("N3"),
                totalAmount = Details.Sum(i => i.TotalAmount).ToString("N3")
            });
        }

        public JsonResult SaveOrUpdateSalesOrder(string id, string orderDate, string deliveryDate, string customerId, string reference, string description,
            string salePerson, string incoterm, string shippingPolicy, string createInvoice, string paymentTerm, string advances,
            string repair, string type, string status, string saleAt, string paymentMethod, string transportation, string invoiceType, string group,
            string deposit, string maintainDate, string setupFee)
        {
            string result = "", error = "";
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Order.OrderId = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Order.OrderDate = DateTime.Parse(orderDate);
            try
            {
                WSI.Order.DeliveryDate = DateTime.Parse(deliveryDate);
            }
            catch 
            {
                WSI.Order.DeliveryDate = WSI.Order.OrderDate;
            }
            try
            {
                WSI.Order.RepairDate = int.Parse(repair);
            }
            catch { } 
            WSI.Order.CustomerId = customerId;
            WSI.Order.Reference = reference; WSI.Order.Description = NMCryptography.base64Decode(description); WSI.Order.SalesPersonId = salePerson; WSI.Order.Incoterm = incoterm;
            WSI.Order.ShippingPolicy = shippingPolicy; WSI.Order.CreateInvoice = createInvoice; WSI.Order.PaymentTerm = NMCryptography.base64Decode(paymentTerm);
            WSI.Order.Advances = advances;
            WSI.Order.PaymentMethod = paymentMethod;
            WSI.Order.Transportation = transportation;
            //WSI.Order.Amount = double.Parse(amount); WSI.Order.TotalAmount = double.Parse(totalAmount);
            //WSI.Order.Tax = double.Parse(tax); WSI.Order.Discount = double.Parse(discount);

            WSI.Order.OrderTypeId = type;   // NMConstant.SOType.SalesOrder;
            WSI.Order.OrderGroup = group;   // bán hoặc cho thuê
            WSI.Order.OrderStatus = status; // NMConstant.SOStatuses.Order;
            WSI.Order.InvoiceTypeId = invoiceType;

            //cho thue
            try
            {
                WSI.Order.Deposit = double.Parse(deposit);
                WSI.Order.MaintainDate = DateTime.Parse(maintainDate);
                WSI.Order.SetupFee = double.Parse(setupFee);
            }
            catch { }
            if (String.IsNullOrEmpty(saleAt))
            {
                WSI.Order.StockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;   //UserInfoCache.User.Customer.StockId;
            }
            else
            {
                WSI.Order.StockId = saleAt;
            }
            List<SalesOrderDetails> Details = new List<SalesOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<SalesOrderDetails>)Session["Details"];
            }
            WSI.Details = Details;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                result = WSI.Order.OrderId;
                try
                {
                    double adv = double.Parse(advances);
                    if (adv != 0)
                    {
                        NMReceiptsBL rcpBL = new NMReceiptsBL();
                        NMReceiptsWSI rcpWSI = new NMReceiptsWSI();
                        rcpWSI.Mode = "SAV_OBJ";
                        rcpWSI.Receipt = new Receipts();
                        rcpWSI.Receipt.ReceiptId = "";
                        rcpWSI.Receipt.ReceiptDate = NMCommon.convertDate(DateTime.Now.ToString());
                        rcpWSI.Receipt.CustomerId = customerId;
                        rcpWSI.Receipt.InvoiceId = WSI.Order.OrderId;
                        rcpWSI.Receipt.Amount = adv;
                        rcpWSI.Receipt.CurrencyId = "VND";
                        rcpWSI.Receipt.ExchangeRate = 1;
                        rcpWSI.Receipt.ReceiptAmount = adv / rcpWSI.Receipt.ExchangeRate;
                        rcpWSI.Receipt.DescriptionInVietnamese = "Thu tiền tạm ứng của khách";
                        rcpWSI.Receipt.ReceiptTypeId = NEXMI.NMConstant.ReceiptType.Deposit;
                        rcpWSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;
                        rcpWSI.ActionBy = User.Identity.Name;
                        
                        rcpWSI = rcpBL.callSingleBL(rcpWSI);
                    }
                }
                catch
                {
                }                
            }
            error = WSI.WsiError;
            return Json(new { result = result, error = error, group = group });
        }

        public JsonResult DeleteSalesOrder(string id)
        {
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            WSI.Order.OrderId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult CancelSOForm(string id, string windowId)
        {
            ViewData["Id"] = id;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult CancelSO(string orderId, string message)
        {
            string error = NMCommon.UpdateObjectStatus("SalesOrders", orderId, NMConstant.SOStatuses.Canceled, null, User.Identity.Name, NMCommon.GetInterface("STATUS", Session["Lang"].ToString()) + ": Đã hủy\n" + message);
            return Json(error);
        }

        public JsonResult CreateExportFromSO(string orderId)
        {
            string error = "", id = "";
            NMExportsBL ExportBL = new NMExportsBL();
            NMExportsWSI ExportWSI = new NMExportsWSI();
            ExportWSI.Mode = "SRC_OBJ";
            ExportWSI.Export = new Exports();
            ExportWSI.Export.Reference = orderId;
            List<NMExportsWSI> WSIs = ExportBL.callListBL(ExportWSI);
            if (WSIs.Count <= 0)
            {
                NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                SOWSI.Mode = "SEL_OBJ";
                SOWSI.Order = new SalesOrders();
                SOWSI.Order.OrderId = orderId;
                SOWSI = SOBL.callSingleBL(SOWSI);
                error = SOWSI.WsiError;
                if (error == "")
                {
                    ExportWSI = new NMExportsWSI();
                    ExportWSI.Mode = "SAV_OBJ";
                    ExportWSI.Export = new Exports();
                    ExportWSI.Export.CustomerId = SOWSI.Customer.CustomerId;
                    ExportWSI.Export.Reference = SOWSI.Order.OrderId;

                    if (NMCommon.GetSetting("USE_SODATE_FOR_IMPORT_DATE"))
                        ExportWSI.Export.ExportDate = SOWSI.Order.OrderDate;
                    else
                        ExportWSI.Export.ExportDate = DateTime.Today;

                    ExportWSI.Export.StockId = SOWSI.Order.StockId;
                    ExportWSI.Export.ExportStatus = NMConstant.EXStatuses.Draft;
                    ExportWSI.Export.ExportTypeId = NMConstant.ExportType.ForCustomers;
                    ExportWSI.Export.DeliveryMethod = SOWSI.Order.ShippingPolicy;
                    List<ExportDetails> Details = new List<ExportDetails>();
                    ExportDetails Detail;
                    foreach (SalesOrderDetails Item in SOWSI.Details)
                    {
                        Detail = new ExportDetails();
                        Detail.CopyFromSODetail(Item);
                        Details.Add(Detail);
                    }
                    ExportWSI.Details = Details;
                    ExportWSI.ActionBy = User.Identity.Name;
                    ExportWSI = ExportBL.callSingleBL(ExportWSI);
                    error = ExportWSI.WsiError;
                    id = ExportWSI.Export.ExportId;
                }
            }
            else
            {
                id = WSIs[0].Export.ExportId;
            }
            return Json(new { error = error, id = id });
        }
        #endregion

        #region InvoiceBatchs
        //public ActionResult SalesInvoiceBatchs()
        //{
        //    return View();
        //}

        public ActionResult SalesInvoiceBatchs()
        {
            return PartialView();
        }

        //public ActionResult SalesInvoiceBatchForm(string id)
        //{
        //    string batchId = "", status = "", description = "", batchDate = DateTime.Today.ToString("dd-MM-yyyy");
        //    NEXMI.NMSalesInvoiceBatchsBL BL = new NEXMI.NMSalesInvoiceBatchsBL();
        //    NEXMI.NMSalesInvoiceBatchsWSI WSI = new NEXMI.NMSalesInvoiceBatchsWSI();
        //    WSI.Mode = "SEL_OBJ";
        //    WSI.InvoiceBatchId = id;
        //    WSI = BL.callSingleBL(WSI);
        //    if (WSI.WsiError == "")
        //    {
        //        batchId = WSI.InvoiceBatchId;
        //        batchDate = WSI.InvoiceBatchDate;
        //        status = WSI.BatchStatus;
        //        description = WSI.Description;
        //    }
        //    ViewData["batchId"] = batchId;
        //    ViewData["batchDate"] = batchDate;
        //    ViewData["status"] = status;
        //    ViewData["description"] = description;
        //    return PartialView();
        //}

        //public JsonResult SaveOrUpdateSalesInvoiceBatch(string id, string batchDate, string description, string status)
        //{
        //    String result = "", error = "";
        //    NMSalesInvoiceBatchsWSI WSI = new NMSalesInvoiceBatchsWSI();
        //    NMSalesInvoiceBatchsBL BL = new NMSalesInvoiceBatchsBL();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.InvoiceBatchId = id;
        //    WSI.InvoiceBatchDate = NMCommon.convertDate(batchDate).ToString();
        //    WSI.BatchStatus = status;
        //    WSI.Description = description;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);
        //    result = WSI.InvoiceBatchId;
        //    error = WSI.WsiError;
        //    return Json(new { result = result, error = error });
        //}

        public ActionResult SalesInvoiceBatchDetail(string batchId)
        {
            NEXMI.NMSalesInvoicesWSI WSI = new NEXMI.NMSalesInvoicesWSI();
            NEXMI.NMSalesInvoicesBL BL = new NEXMI.NMSalesInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceBatchId = batchId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["BatchId"] = batchId;
            return PartialView();
        }

        //public JsonResult DeleteSalesInvoiceBatch(string batchId)
        //{
        //    NMSalesInvoiceBatchsBL BL = new NMSalesInvoiceBatchsBL();
        //    NMSalesInvoiceBatchsWSI WSI = new NMSalesInvoiceBatchsWSI();
        //    WSI.Mode = "DEL_OBJ";
        //    WSI.InvoiceBatchId = batchId;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}
        #endregion

        #region Invoices

        public ActionResult ChooseInvoiceOrder(string orderId, string windowId)
        {
            ViewData["OrderId"] = orderId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult CreateInvoiceFromOrder(string orderId)
        {
            string error = "", invoiceId = "";
            NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
            NMSalesInvoicesWSI SIWSI = SOBL.CreateSaleInvoice(orderId);

            if (SIWSI.WsiError == string.Empty)
            {
                SIWSI.Mode = "SAV_OBJ";
                SIWSI.ActionBy = User.Identity.Name;

                NMSalesInvoicesBL SIBL = new NMSalesInvoicesBL();
                SIWSI = SIBL.callSingleBL(SIWSI);
                error = SIWSI.WsiError; invoiceId = SIWSI.Invoice.InvoiceId;
            }

            //NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
            //SOWSI.Mode = "SEL_OBJ";
            //SOWSI.Order.OrderId = orderId;
            //SOWSI = SOBL.callSingleBL(SOWSI);
            
            //if (SOWSI.WsiError == "")
            //{
            //    NMSalesInvoicesBL SIBL = new NMSalesInvoicesBL();
            //    SIBL.FromSalesOrder(SOWSI);

            //    NMSalesInvoicesWSI SIWSI = SIBL.WSI;
            //    SIWSI.Mode = "SAV_OBJ";
            //    SIWSI.ActionBy = User.Identity.Name;                

            //    SIWSI = SIBL.callSingleBL(SIWSI);
            //    error = SIWSI.WsiError; invoiceId = SIWSI.Invoice.InvoiceId;
            //}
            return Json(new { error = error, invoiceId = invoiceId });
        }

        public ActionResult SalesInvoices(string status, string orderId)
        {
            if (status == null) status = "";
            ViewData["Status"] = status;
            if (orderId == null) orderId = "";
            ViewData["OrderId"] = orderId;
            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult SalesInvoiceList(string pageNum, string pageSize, string sortDataField, string sortOrder, string keyword, 
            string status, string partnerId, string orderId, string from, string to)
        {
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Invoice = new SalesInvoices();
            if (!string.IsNullOrEmpty(orderId))
                WSI.Invoice.Reference = orderId;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Invoice.CustomerId = partnerId;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.SI, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            //else
            //{
            //    double days = NMCommon.GetSettingValue(NMConstant.Settings.DF_LOAD_LIST_DAYS);
            //    WSI.FromDate = DateTime.Today.AddDays(-days).ToString();
            //}
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;
            //else
            //    WSI.ToDate = DateTime.Today.ToString();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Invoice.InvoiceStatus = status;

            List<NMSalesInvoicesWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;

            return PartialView();
        }

        public ActionResult SalesInvoiceForm(string invoiceId, string orderId, string mode)
        {
            string customerId = "", customerName = "", customerAddress = "";
            List<SalesInvoiceDetails> Details = new List<SalesInvoiceDetails>();
            
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Invoice.InvoiceDate = DateTime.Today;
            WSI.Invoice.InvoiceStatus =NEXMI.NMConstant.SIStatuses.Draft;

            if (!string.IsNullOrEmpty(invoiceId))
            {   
                WSI.Mode = "SEL_OBJ";
                WSI.Invoice.InvoiceId = invoiceId;
                WSI = BL.callSingleBL(WSI);
                customerId = WSI.Customer.CustomerId; customerName = WSI.Customer.CompanyNameInVietnamese;
                customerAddress = WSI.Customer.Address;
                
                foreach (SalesInvoiceDetails Item in WSI.Details)
                {
                    if (mode == "Copy")
                        Item.OrdinalNumber = 0;
                    Details.Add(Item);
                }
            }
            else
            {
                invoiceId = "";
                if (!string.IsNullOrEmpty(orderId))
                {
                    NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                    //NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                    //SOWSI.Mode = "SEL_OBJ";
                    //SOWSI.Order.OrderId = orderId;
                    //SOWSI = SOBL.callSingleBL(SOWSI);

                    WSI = SOBL.CreateSaleInvoice(orderId);
                    //WSI = BL.WSI;
                    customerName = WSI.Customer.CompanyNameInVietnamese;

                    Details = WSI.Details;
                }
            }
            if (mode == "Copy")
            {
                invoiceId = "";
                WSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Draft;
                WSI.Invoice.InvoiceDate = DateTime.Today;
            }
            ViewData["Id"] = invoiceId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["CustomerAddress"] = customerAddress;            
            ViewData["WSI"] = WSI;
            Session["Details"] = Details;

            return PartialView();
        }

        public JsonResult SaveOrUpdateSalesInvoice(string id, string invoiceDate, string customerId, string shipCost,
            string otherCost, string description, string reference, string salePerson, string sourceDocument, string stockId)
        {
            string result = "", error = "";
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Invoice.InvoiceDate = DateTime.Parse(invoiceDate);            
            WSI.Invoice.CustomerId = customerId;
            WSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Draft;
            WSI.Invoice.DescriptionInVietnamese = description;
            WSI.Invoice.Reference = reference;
            WSI.Invoice.SalesPersonId = salePerson;
            WSI.Invoice.StockId = stockId;
            WSI.ActionBy = User.Identity.Name;
            WSI.Invoice.CurrencyId = "VND";
            WSI.Invoice.ExchangeRate = 1;
            WSI.Details = (List<SalesInvoiceDetails>)Session["Details"];
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                result = WSI.Invoice.InvoiceId;
                Session["Details"] = null;
            }
            else
            {
                error = WSI.WsiError;
            }
            return Json(new { result = result, error = error });
        }

        public ActionResult SalesInvoiceDetailForm(string productId)
        {
            List<SalesInvoiceDetails> Details = new List<SalesInvoiceDetails>();
            if (Session["Details"] != null)
                Details = (List<SalesInvoiceDetails>)Session["Details"];
            foreach (SalesInvoiceDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }

            ViewData["PriceInput"] = NMCommon.GetSetting(NMConstant.Settings.PRICE_BY_USER);
            return PartialView();
        }

        public ActionResult AddSalesInvoiceDetail(int ordinalNumber, string productId, string unitId, string quantity, string price, 
                                                string discount, string VATRate, string description)
        {
            String strError = "";
            List<SalesInvoiceDetails> Details = new List<SalesInvoiceDetails>();
            double detailAmount = 0;
            try
            {
                if (Session["Details"] != null)
                {
                    Details = (List<SalesInvoiceDetails>)Session["Details"];
                }
                bool flag = true;
                SalesInvoiceDetails Detail;
                if (ordinalNumber > 0)
                    Detail = Details.Find(d => d.OrdinalNumber == ordinalNumber);
                else
                    Detail = Details.Find(d => d.ProductId == productId);
                
                if (Detail == null)
                {
                    Detail = new SalesInvoiceDetails();
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.Price = double.Parse(price);
                    Detail.Discount = double.Parse(discount);
                    Detail.Tax = double.Parse(VATRate);
                    Detail.Description = description;
                    detailAmount = Detail.Amount;
                    Details.Add(Detail);
                }
                else
                {
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.Price = double.Parse(price);
                    Detail.Discount = double.Parse(discount);
                    Detail.Tax = double.Parse(VATRate);
                    Detail.Description = description;
                    detailAmount = Detail.Amount;
                }
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            //return Json(new
            //{
            //    strError = strError,
            //    detailAmount = detailAmount.ToString("N3"),
            //    amount = Details.Sum(i => i.Amount).ToString("N3"),
            //    taxAmount = Details.Sum(i => i.TaxAmount).ToString("N3"),
            //    discountAmount = Details.Sum(i => i.DiscountAmount).ToString("N3"),
            //    totalAmount = Details.Sum(i => i.TotalAmount).ToString("N3")
            //});

            return PartialView("SalesInvoiceDetails");
        }

        public JsonResult RemoveSalesInvoiceDetail(string productId)
        {
            List<SalesInvoiceDetails> Details = new List<SalesInvoiceDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<SalesInvoiceDetails>)Session["Details"];
            }
            foreach (SalesInvoiceDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    Details.Remove(Item);
                    break;
                }
            }
            return Json(new
            {
                amount = Details.Sum(i => i.Amount).ToString("N3"),
                taxAmount = Details.Sum(i => i.TaxAmount).ToString("N3"),
                discountAmount = Details.Sum(i => i.DiscountAmount).ToString("N3"),
                totalAmount = Details.Sum(i => i.TotalAmount).ToString("N3")
            });
        }

        public ActionResult SalesInvoice(string invoiceId)
        {
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = invoiceId;
            WSI = BL.callSingleBL(WSI);
            ViewData["WSI"] = WSI;
            ViewData["CustomerId"] = (WSI.Customer == null) ? "" : WSI.Customer.CustomerId;
            ViewData["CustomerName"] = (WSI.Customer == null) ? "" : WSI.Customer.CompanyNameInVietnamese;
            ViewData["CustomerAddress"] = (WSI.Customer == null) ? "" : WSI.Customer.Address;
            ViewData["Reference"] = WSI.Invoice.Reference;
            ViewData["InvoiceDate"] = WSI.Invoice.InvoiceDate.ToString("dd-MM-yyyy"); ViewData["Description"] = WSI.Invoice.DescriptionInVietnamese;
            ViewData["InvoiceStatus"] = WSI.Invoice.InvoiceStatus;
            ViewData["Reference"] = WSI.Invoice.Reference;
            //ViewData["ShipCost"] = WSI.Invoice.ShipCost.ToString("N3");
            //ViewData["OtherCost"] = WSI.Invoice.OtherCost.ToString("N3");
            ViewData["slUsers"] = WSI.CreatedBy.CompanyNameInVietnamese;
            // for log and messages
            ViewData["SendTo"] = WSI.Invoice.SalesPersonId;    // dùng để gửi tin nhắn đến người quản lý
            if (WSI.Invoice.SalesPersonId != WSI.Invoice.CreatedBy)  //người tạo khác người quản lý
                ViewData["SendTo"] += ";" + WSI.Invoice.CreatedBy;   //gửi tin đến người tạo

            ViewData["Id"] = invoiceId;
            ViewData["Amount"] = WSI.Invoice.Amount.ToString("N3");
            ViewData["Discount"] = WSI.Invoice.Discount.ToString("N3");
            ViewData["Tax"] = WSI.Invoice.Tax.ToString("N3");
            ViewData["TotalAmount"] = WSI.Invoice.TotalAmount.ToString("N3");
            ViewData["PaidAmount"] = (WSI.Receipts.Sum(i => i.Amount) - WSI.Payments.Sum(i => i.PaymentAmount)).ToString("N3");
            ViewData["BalanceAmount"] = (WSI.Invoice.TotalAmount - WSI.Receipts.Sum(i => i.Amount) + WSI.Payments.Sum(i => i.PaymentAmount)).ToString("N3");
            ViewData["Details"] = WSI.Details;
            ViewData["Receipts"] = WSI.Receipts;
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public JsonResult DeleteSalesInvoice(String id)
        {
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Invoice = new NEXMI.SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult SaveReceipt(String receiptId, string receiptDate, String customerId, string invoiceId, String receiptAmount, String description)
        {
            NMReceiptsWSI WSI = new NMReceiptsWSI();
            NMReceiptsBL BL = new NMReceiptsBL();
            WSI.Mode = "SAV_OBJ";
            WSI.Receipt = new Receipts();
            WSI.Receipt.ReceiptId = receiptId;
            WSI.Receipt.ReceiptDate = NMCommon.convertDate(receiptDate);
            WSI.Receipt.CustomerId = customerId;
            WSI.Receipt.InvoiceId = invoiceId;
            WSI.Receipt.Amount = double.Parse(receiptAmount);
            WSI.Receipt.CurrencyId = "VND";
            WSI.Receipt.ExchangeRate = 1;
            WSI.Receipt.ReceiptAmount = WSI.Receipt.Amount / WSI.Receipt.ExchangeRate;
            WSI.Receipt.DescriptionInVietnamese = description;
            WSI.Receipt.ReceiptTypeId = NMConstant.ReceiptType.Sales;
            WSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult PrintInvoice(string id)
        {
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);

            ViewData["WSI"] = WSI;
            ViewData["CustomerAntt"] = "";
            ViewData["CustomerAnttTitle"] = "";
            ViewData["CustomerArea"] = "";

            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SEL_OBJ";
            CWSI.Customer = new Customers();
            CWSI.Customer.CustomerId = WSI.Invoice.CustomerId;
            CWSI = CBL.callSingleBL(CWSI);
            if (WSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
            {
                if (CWSI.ContactPersons.Count > 0)
                {
                    ViewData["CustomerAntt"] = CWSI.ContactPersons[0].CompanyNameInVietnamese;
                    ViewData["CustomerAnttTitle"] = CWSI.ContactPersons[0].JobPosition;
                }
            }
            ViewData["CustomerArea"] = CWSI.AreaWSI.FullName;

            return PartialView();
        }

        public JsonResult ConfirmInvoice(string id, string reference)
        {
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Debit;
                if( NMCommon.GetSetting("UPDATE_SI_DATE_BY_ACTION"))
                    WSI.Invoice.InvoiceDate = DateTime.Now;
                WSI.ActionBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
            }
            //string error = "";
            //error = NMCommon.UpdateObjectStatus("SalesInvoices", id, NMConstant.SIStatuses.Open, "", User.Identity.Name, "<%= NEXMI.NMCommon.GetInterface("STATUS", Session["Lang"].ToString()) %>: Đã xác nhận");
            //if (error == "")
            //    error = NMCommon.UpdateObjectStatus("SalesOrders", reference, NMConstant.SOStatuses.Invoice, "", User.Identity.Name, "<%= NEXMI.NMCommon.GetInterface("STATUS", Session["Lang"].ToString()) %>: <%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", Session["Lang"].ToString()) %>");
            //if (error != "")
            //    NMCommon.UpdateObjectStatus("SalesInvoices", id, NMConstant.SIStatuses.Draft, "", User.Identity.Name, null);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Kho

        public ActionResult Inventory()
        {
            Session.Remove("InventoryDetails");
            Session.Remove("Flag");
            //NMProductInStocksBL BL = new NMProductInStocksBL();
            //NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            //WSI.Mode = "SRC_OBJ";
            //ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult InventoryForm(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult AddToInventoryList(string productId, double quantity)
        {
            string error = "";

            try
            {
                List<SalesInvoiceDetails> InventoryDetails = new List<SalesInvoiceDetails>();
                if (Session["InventoryDetails"] != null)
                {
                    InventoryDetails = (List<SalesInvoiceDetails>)Session["InventoryDetails"];
                }
                SalesInvoiceDetails Item = InventoryDetails.Select(i => i).Where(i => i.ProductId == productId).FirstOrDefault();
                if (Item == null)
                {
                    NMProductsBL BL = new NMProductsBL();
                    NMProductsWSI WSI = new NMProductsWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.Product = new Products();
                    WSI.Product.ProductId = productId;
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError == "")
                    {
                        Item = new SalesInvoiceDetails();
                        InventoryDetails.Add(Item);
                        Item.ProductId = productId;
                        Item.Price = WSI.PriceForSales.Price;
                        Item.Discount = WSI.PriceForSales.Discount;
                        Item.Tax = WSI.Product.VATRate;
                    }
                    else
                    {
                        error = WSI.WsiError;
                        return Json(error);
                    }
                }
                if (quantity > 0)
                {
                    Item.Quantity = quantity;
                    Item.Amount = Item.Price * Item.Quantity;
                    Item.DiscountAmount = Item.Amount * Item.Discount / 100;
                    Item.TaxAmount = (Item.Amount - Item.DiscountAmount) * Item.Tax / 100;
                    Item.TotalAmount = Item.Amount - Item.DiscountAmount + Item.TaxAmount;
                    Session["InventoryDetails"] = InventoryDetails;
                    Session["Flag"] = "OK";
                }
                else
                {
                    InventoryDetails.Remove(Item);
                }
            }
            catch
            {
                error = "Không thực hiện được!\nVui lòng liên hệ người quản trị để khắc phục!";
            }
            return Json(error);
        }

        public JsonResult SaveOrderAfterInventory(string invoiceDate, string reference, string description)
        {
            
            //if(Session["InventoryDetails"] == null)
            //    return Json(new { error = "Lỗi chưa nhập dữ liệu!", id = "" });
            //string error = "", id = "";
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceDate = DateTime.Parse(invoiceDate);
            WSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Paid;
            WSI.Invoice.InvoiceTypeId = NMConstant.SalesInvoiceType.Sales;
            WSI.Invoice.SalesPersonId = User.Identity.Name;
            WSI.Invoice.Reference = reference;
            WSI.Invoice.DescriptionInVietnamese = description;
            WSI.ActionBy = User.Identity.Name;
            WSI.Invoice.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            WSI.Details = (List<SalesInvoiceDetails>)Session["InventoryDetails"];
            
            WSI = BL.callSingleBL(WSI);
            return Json(new { error = WSI.WsiError, id = WSI.Invoice.InvoiceId });
        }

        #endregion

        //#region PricesForSalesInvoice
        //public ActionResult PricesForSalesInvoice(String productCategoryId)
        //{
        //    ViewData["ProductCategoryId"] = productCategoryId;
        //    return PartialView();
        //}

        //public ActionResult PricesForSalesInvoiceForm(String productId)
        //{
        //    ViewData["ProductId"] = productId;
        //    return PartialView();
        //}

        //public JsonResult SaveOrUpdatePricesForSalesInvoice(String productId, String price)
        //{
        //    NMPricesForSalesInvoiceWSI WSI = new NMPricesForSalesInvoiceWSI();
        //    NMPricesForSalesInvoiceBL BL = new NMPricesForSalesInvoiceBL();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.ProductId = productId;
        //    WSI.DateOfPrice = DateTime.Today.ToString();
        //    WSI.Price = price;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}

        //public ActionResult PricesForSalesInvoiceHistory(String productId)
        //{
        //    ViewData["ProductId"] = productId;
        //    return PartialView();
        //}
        //#endregion

                //public ActionResult AccountReceivable(string monthYear, string fromDate, string toDate)
        //{
        //    ViewData["MonthYear"] = monthYear;
        //    ViewData["FromDate"] = fromDate;
        //    ViewData["ToDate"] = toDate;

        //    ArrayList cols = new ArrayList();
        //    cols.Add("CustomerId");
        //    cols.Add("CustomerName");
        //    cols.Add("BeginAmount");
        //    cols.Add("SalesAmount");
        //    cols.Add("ReceivedAmount");
        //    cols.Add("EndAmount");
        //    cols.Add("MaxDebitAmount");
        //    Dictionary<String, object> criterias = new Dictionary<String, object>();

        //    NEXMI.NMDataTableBL BL = new NEXMI.NMDataTableBL();
        //    NEXMI.NMDataTableWSI WSI = new NEXMI.NMDataTableWSI();
        //    if (string.IsNullOrEmpty(monthYear) && string.IsNullOrEmpty(fromDate))
        //    {
        //        WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, AR.BeginAmount, AR.SalesAmount, AR.ReceivedAmount, AR.EndAmount, C.MaxDebitAmount FROM Customers C, MonthlyAccountReceivable AR WHERE C.CustomerId = AR.CustomerId";
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(fromDate))
        //        {
        //            WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, AR.BeginAmount, AR.SalesAmount, AR.ReceivedAmount, AR.EndAmount, C.MaxDebitAmount FROM Customers C, CloseMonthAccountReceivableDetails AR WHERE C.CustomerId = AR.CustomerId AND CloseMonth = :MonthYear";
        //            criterias.Add("MonthYear", monthYear);
        //        }
        //        else
        //        {
        //            DateTime startDate = NEXMI.NMCommon.convertDate(fromDate);
        //            DateTime endDate = NEXMI.NMCommon.convertDate(toDate);
        //            WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, "
        //                + "(SELECT AR.EndAmount FROM CloseMonthAccountReceivableDetails AR WHERE AR.CloseMonth = :PreviousMonth AND AR.CustomerId = C.CustomerId) AS BeginAmount, "
        //                + "(SELECT ISNULL(SUM(ID.Quantity * ID.Price + (ID.VATRate * (ID.Quantity * ID.Price) / 100) - ID.Discount * (ID.Quantity * ID.Price) / 100 + I.ShipCost + I.OtherCost), 0) FROM SalesInvoices I, SalesInvoiceDetails ID WHERE I.InvoiceId = ID.InvoiceId AND I.InvoiceDate BETWEEN :FromDate AND :ToDate AND I.CustomerId = C.CustomerId) AS SalesAmount, "
        //                + "(SELECT ISNULL(SUM(R.ReceiptAmount), 0) FROM Receipts R WHERE R.ReceiptDate BETWEEN :FromDate AND :ToDate AND R.CustomerId = C.CustomerId) AS ReceiptAmount, "
        //                + "0 AS EndAmount, C.MaxDebitAmount "
        //                + "FROM Customers C WHERE C.GroupId = :GroupId";
        //            criterias.Add("PreviousMonth", startDate.AddMonths(-1).ToString("MM/yyyy"));
        //            criterias.Add("FromDate", startDate);
        //            criterias.Add("ToDate", endDate);
        //            criterias.Add("GroupId", NEXMI.NMConstant.CustomerGroups.Customer);
        //        }
        //    }
        //    WSI.Columns = cols;
        //    WSI.Criterias = criterias;
        //    WSI = BL.GetData(WSI);
        //    ViewData["dt"] = WSI.Data;
        //    return PartialView();
        //}

        #region Cho thuê

        public ActionResult LeaseOrders(String SOType)
        {
            ViewData["ViewType"] = "list";
            if (!String.IsNullOrEmpty(SOType))
            {
                ViewData["SOType"] = SOType;
                ViewData["SOFunc"] = NEXMI.NMConstant.Functions.Quotation;
                ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Draft;
            }
            else
            {
                ViewData["SOType"] = NEXMI.NMConstant.SOType.SalesOrder;
                ViewData["SOFunc"] = NEXMI.NMConstant.Functions.LeaseOrder;
                ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Order;
            }

            return PartialView();
        }

        public ActionResult LeaseOrderList(string pageNum, string status, string keyword, string typeId,
                                           string partnerId, string from, string to, string mode)
        {
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "SRC_OBJ";
            
            WSI.Order.OrderGroup = NMConstant.SOrderGroups.Lease;
            if (typeId == NMConstant.SOType.Quotation)
            {
                if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Quotation) == false) WSI.ActionBy = User.Identity.Name;
            }
            else
                if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.LeaseOrder) == false) WSI.ActionBy = User.Identity.Name;
            //if(!string.IsNullOrEmpty(pageNum))
            //    WSI.PageNum = int.Parse(pageNum);
            if (mode != "Print")
            {
                int page = 1;
                try
                {
                    page = int.Parse(pageNum);
                }
                catch { }
                WSI.PageNum = page - 1;
                WSI.PageSize = NMCommon.PageSize();
            }
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            else
            {
                //double days = NMCommon.GetSettingValue(NMConstant.Settings.DF_LOAD_LIST_DAYS);
                WSI.FromDate = DateTime.Today.AddDays(-3).ToString(); //DateTime.Today.AddDays(-days).ToString();
            }
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;
            else
            {
                int daysinMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                if (DateTime.Today.Day <= daysinMonth - 3)
                    WSI.ToDate = DateTime.Today.AddDays(3).ToString();
                else
                    WSI.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, daysinMonth).ToString();
            }
            WSI.SortField = "Reference";
            WSI.SortOrder = "ASC";
            WSI.Keyword = keyword;
            WSI.Order.OrderStatus = status;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Order.CustomerId = partnerId;

            List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;
            ViewData["SOType"] = typeId;

            if (string.IsNullOrEmpty(mode))
            {
                mode = "";
            }
            ViewData["Mode"] = mode;

            //if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == false)
            //    return PartialView("SOList4OneLineType");

            return PartialView();
        }

        public ActionResult LeaseOrderForm(string id, string mode, string type, string viewName)
        {
            if (mode == null) mode = "";
            string status = "";
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new NEXMI.SalesOrders();
            if (id != "")
            {
                WSI.Order.OrderId = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    type = WSI.Order.OrderTypeId;
                    status = WSI.Order.OrderStatus;
                    if (mode == "Copy")
                    {
                        WSI.Order.OrderId = "";
                        WSI.Order.OrderDate = DateTime.Now;
                        status = "";
                    }
                    foreach (SalesOrderDetails Item in WSI.Details)
                    {
                        if (mode == "Copy") Item.Id = 0;
                    }
                    Session["SalesOrder"] = WSI;
                }
            }
            else
            {
                WSI.Order.OrderDate = DateTime.Now;
                WSI.Order.DeliveryDate = DateTime.Now;
                WSI.Order.MaintainDate = DateTime.Now;
                WSI.Order.OrderTypeId = type;
                WSI.Order.OrderGroup = NMConstant.SOrderGroups.Lease;
                WSI.Order.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                WSI.Order.SalesPersonId = User.Identity.Name;
                WSI.Details = new List<SalesOrderDetails>();
            }
            if (type == NEXMI.NMConstant.SOType.Quotation)
            {
                ViewData["Tilte"] = NEXMI.NMCommon.GetInterface("QUOTATION_TITLE", Session["Lang"].ToString()) ;
                ViewData["FunctionId"] = NMConstant.Functions.Quotation;
                if (status == "") WSI.Order.OrderStatus = NMConstant.LOStatuses.Draft;
            }
            else
            {
                ViewData["Tilte"] = NEXMI.NMCommon.GetInterface("RENT_ORDER", Session["Lang"].ToString());
                ViewData["FunctionId"] = NMConstant.Functions.LeaseOrder;
                if (status == "") WSI.Order.OrderStatus = NMConstant.LOStatuses.Order;
            }
            Session["Details"] = WSI.Details;
            ViewData["WSI"] = WSI;
            ViewData["Mode"] = mode;
            ViewData["ViewType"] = "detail";

            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER);
            ViewData["SalerInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_SALER_BY_USER);

            return PartialView(viewName);
        }

        public ActionResult ChangeMaintainDateDialog(string orderId, string windowId)
        {
            ViewData["OrderId"] = orderId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveMaintainDateChange(string orderId, string maintainDate)
        {
            return Json(NMCommon.UpdateLeaseOrderMaintainDate(orderId, maintainDate, User.Identity.Name).ToString());
        }

        public JsonResult CreateExportSparePartsfromSO(string orderId)
        {
            string error = "", id = "";
            NMExportsBL ExportBL = new NMExportsBL();
            NMExportsWSI ExportWSI = new NMExportsWSI();
            ExportWSI.Mode = "SRC_OBJ";
            ExportWSI.Export = new Exports();
            ExportWSI.Export.Reference = orderId;
            List<NMExportsWSI> WSIs = ExportBL.callListBL(ExportWSI);
            if (WSIs.Count <= 0)
            {
                NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                SOWSI.Mode = "SEL_OBJ";
                SOWSI.Order = new SalesOrders();
                SOWSI.Order.OrderId = orderId;
                SOWSI = SOBL.callSingleBL(SOWSI);
                error = SOWSI.WsiError;
                if (error == "")
                {
                    ExportWSI = new NMExportsWSI();
                    ExportWSI.Mode = "SAV_OBJ";
                    ExportWSI.Export = new Exports();
                    ExportWSI.Export.CustomerId = SOWSI.Customer.CustomerId;
                    ExportWSI.Export.Reference = SOWSI.Order.OrderId;
                    ExportWSI.Export.ExportDate = DateTime.Today;
                    ExportWSI.Export.StockId = SOWSI.Order.StockId; //((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId
                    ExportWSI.Export.ExportStatus = NMConstant.EXStatuses.Draft;
                    ExportWSI.Export.ExportTypeId = NMConstant.ExportType.ForCustomers;
                    ExportWSI.Export.DeliveryMethod = SOWSI.Order.ShippingPolicy;
                    List<ExportDetails> Details = new List<ExportDetails>();
                    ExportDetails Detail;
                    foreach (SalesOrderDetails Item in SOWSI.Details)
                    {
                        Detail = new ExportDetails();
                        Detail.ProductId = Item.ProductId;
                        Detail.Quantity = Item.Quantity;
                        Details.Add(Detail);
                    }
                    ExportWSI.Details = Details;
                    ExportWSI.ActionBy = User.Identity.Name;
                    ExportWSI = ExportBL.callSingleBL(ExportWSI);
                    error = ExportWSI.WsiError;
                    id = ExportWSI.Export.ExportId;
                }
            }
            else
            {
                id = WSIs[0].Export.ExportId;
            }
            return Json(new { error = error, id = id });
        }

        #endregion

        public ActionResult OrdersHictory(string pageNum, string status, string keyword, string typeId,
                                           string partnerId, string from, string to, string mode)
        {
            //NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            //NMSalesOrdersBL BL = new NMSalesOrdersBL();
            //WSI.Mode = "SRC_OBJ";
            
            //if (typeId == NMConstant.SOType.Quotation)
            //{
            //    if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Quotation) == false) WSI.ActionBy = User.Identity.Name;
            //}
            //else
            //    if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.LeaseOrder) == false) WSI.ActionBy = User.Identity.Name;
            
            //if (!String.IsNullOrEmpty(partnerId))
            //    WSI.Order.CustomerId = partnerId;

            //List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            //ViewData["WSIs"] = WSIs;
            //ViewData["SOType"] = typeId;

            //if (string.IsNullOrEmpty(mode))
            //{
            //    mode = "";
            //}
            //ViewData["Mode"] = mode;

            return PartialView();
        }

    }
}
