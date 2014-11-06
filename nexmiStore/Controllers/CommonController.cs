// CommonController.cs

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
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using nexmiStore.Models;
using System.Text;
using System.Data;
using System.Reflection;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Loading()
        {
            return PartialView();
        }

        public ActionResult Processing()
        {
            return PartialView();
        }

        public JsonResult GetCustomerForAutocomplete(string keyword, string cgroupId, string limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMCustomersWSI> WSIs = new List<NMCustomersWSI>();
            NMCustomersWSI WSI;
            WSI = new NMCustomersWSI();
            NMCustomersBL BL = new NMCustomersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = cgroupId;
            try
            {
                WSI.Limit = int.Parse(limit);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);

            if (mode == "select")
            {
                WSI = new NMCustomersWSI();
                WSI.Customer = new Customers();
                WSI.Customer.CustomerId = "Search";
                WSI.Customer.CompanyNameInVietnamese = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMCustomersWSI();
                WSI.Customer = new Customers();
                WSI.Customer.CustomerId = "CreateOrEdit";
                WSI.Customer.CompanyNameInVietnamese = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            return Json(WSIs.Select(item => item.Customer), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanksForAutocomplete(string keyword, string limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMBanksWSI> WSIs = new List<NMBanksWSI>();
            NMBanksWSI WSI;
            WSI = new NMBanksWSI();
            NMBanksBL BL = new NMBanksBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            try
            {
                WSI.Limit = int.Parse(limit);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);
            if (mode == "select")
            {
                WSI = new NMBanksWSI();
                WSI.Bank = new Banks();
                WSI.Bank.Id = "Search";
                WSI.Bank.Code = "-";
                WSI.Bank.Name = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMBanksWSI();
                WSI.Bank = new Banks();
                WSI.Bank.Id = "Create";
                WSI.Bank.Code = "-";
                WSI.Bank.Name = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            var data = WSIs.Select(i => new { Id = i.Bank.Id, Name = (i.Bank.Name + " - " + i.Bank.Code).Replace(" - -", "") });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankOfCustomersForAutocomplete(string keyword, string limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMBankOfCustomersWSI> WSIs = new List<NMBankOfCustomersWSI>();
            NMBankOfCustomersWSI WSI;
            if (!string.IsNullOrEmpty(keyword))
            {
                WSI = new NMBankOfCustomersWSI();
                NMBankOfCustomersBL BL = new NMBankOfCustomersBL();
                WSI.Mode = "SRC_OBJ";
                WSI.Keyword = keyword;
                try
                {
                    WSI.Limit = int.Parse(limit);
                }
                catch {
                    WSI.Limit = 0;
                }
                WSIs = BL.callListBL(WSI);
            }
            if (mode == "select")
            {
                if (!string.IsNullOrEmpty(keyword) && WSIs.Count == 0)
                {
                    WSI = new NMBankOfCustomersWSI();
                    WSI.Bank = new Banks();
                    WSI.Bank.Id = "Create";
                    WSI.Bank.Name = "Create " + "'" + keyword + "'";
                    WSIs.Add(WSI);
                }
                WSI = new NMBankOfCustomersWSI();
                WSI.Bank = new Banks();
                WSI.Bank.Id = "Search";
                WSI.Bank.Code = "-";
                WSI.Bank.Name = NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMBankOfCustomersWSI();
                WSI.Bank = new Banks();
                WSI.Bank.Id = "CreateOrEdit";
                WSI.Bank.Code = "-";
                WSI.Bank.Name = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            var data = WSIs.Select(i => new { Id = i.Bank.Id, Name = (i.BankOfCustomer.VNDAccountNo + " - -" + i.Bank.Code).Replace(" - -", "") });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadBanksOfCustomer(string customerId)
        {
            NMBankOfCustomersWSI WSI = new NMBankOfCustomersWSI();
            NMBankOfCustomersBL BL = new NMBankOfCustomersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.BankOfCustomer = new BankOfCustomers();
            WSI.BankOfCustomer.CustomerId = customerId;
            List<NMBankOfCustomersWSI> WSIs = BL.callListBL(WSI);
            WSI = new NMBankOfCustomersWSI();
            WSI.BankOfCustomer = new BankOfCustomers();
            WSI.BankOfCustomer.Id = 0;
            WSI.BankOfCustomer.BankId = "--";
            WSI.BankOfCustomer.VNDAccountNo = "Create new...";
            WSIs.Add(WSI);
            var data = WSIs.Select(i => new { Id = i.BankOfCustomer.Id, Name = (i.BankOfCustomer.VNDAccountNo + " - " + i.BankOfCustomer.BankId).Replace(" - --", "") });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomers(int pageNum, int pageSize, string sortDataField, string sortOrder, string groupId, string txtKeyword, string areaId)
        {
            NEXMI.NMCustomersWSI CustomerWSI = new NEXMI.NMCustomersWSI();
            NEXMI.NMCustomersBL CustomerBL = new NEXMI.NMCustomersBL();
            CustomerWSI.Mode = "SRC_OBJ";
            CustomerWSI.Customer = new Customers();
            CustomerWSI.Customer.GroupId = groupId;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Customer, "ViewAll") == false)
            {
                CustomerWSI.ActionBy = User.Identity.Name;
            }
            CustomerWSI.Customer.AreaId = areaId;
            CustomerWSI.PageNum = pageNum;
            CustomerWSI.PageSize = pageSize;
            CustomerWSI.SortField = sortDataField;
            CustomerWSI.SortOrder = sortOrder;
            CustomerWSI.Keyword = txtKeyword;
            //if (NMCommon.GetSetting(NMConstant.Settings.SHOW_COMPANY_ONLY))
            //    CustomerWSI.Customer.CustomerTypeId = NMConstant.CustomerTypes.Enterprise;
            List<NEXMI.NMCustomersWSI> CustomerWSIs = CustomerBL.callListBL(CustomerWSI);
            int totalRows = 0;
            try
            {
                totalRows = CustomerWSIs[0].TotalRows;
            }
            catch { }
            return Json(new { Rows = CustomerWSIs.Select(item => item.Customer), TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesOrdersForAutocomplete(string keyword, int limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMSalesOrdersWSI> WSIs = new List<NMSalesOrdersWSI>();
            NMSalesOrdersWSI WSI;
            if (!string.IsNullOrEmpty(keyword))
            {
                WSI = new NMSalesOrdersWSI();
                NMSalesOrdersBL BL = new NMSalesOrdersBL();
                WSI.Mode = "SRC_OBJ";
                WSI.Keyword = keyword;
                //try
                //{
                //    WSI.Limit = int.Parse(limit);
                //}
                //catch
                //{
                //    WSI.Limit = 0;
                //}
                WSIs = BL.callListBL(WSI);
            }
            if (mode == "select")
            {
                WSI = new NMSalesOrdersWSI();
                WSI.Order = new SalesOrders();
                WSI.Order.OrderId = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
            }
            return Json(WSIs.Select(item => item.Order), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesOrders(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string typeId)
        {
            string langId = Session["Lang"].ToString();

            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Order = new SalesOrders();
            WSI.Order.OrderTypeId = typeId;
            if (typeId == NMConstant.SOType.Quotation)
            {
                if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Quotation, "ViewAll") == false) WSI.ActionBy = User.Identity.Name;
            }
            else
                if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SalesOrder, "ViewAll") == false) WSI.ActionBy = User.Identity.Name;
            
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                OrderId = i.Order.OrderId,
                OrderDate = i.Order.OrderDate.ToString("dd/MM/yyyy"),
                DeliveryDate = i.Order.DeliveryDate.Value.ToString("dd/MM/yyyy"),
                CustomerId = i.Customer.CustomerId,
                CustomerName = i.Customer.CompanyNameInVietnamese,
                Details = i.Details.Count,    //Details.Count,
                //Note = i.Order. Description,
                Reference = i.Order.Reference,
                Trans = i.Order.Transportation,
                OrderStatus = i.Order.OrderStatus,
                CreatedBy = i.CreatedBy.CompanyNameInVietnamese,
                Status = NMCommon.GetStatusName(i.Order.OrderStatus, langId),//.Status.Name,       //i.Status.Name,
                TotalAmount = (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SalesOrder)) ? i.Order.TotalAmount.ToString("N3") : "0"
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetSalesInvoices(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string status, string orderId)
        //{
        //    NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
        //    NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
        //    WSI.Mode = "SRC_OBJ";
        //    WSI.Invoice = new SalesInvoices();
        //    WSI.Invoice.Reference = orderId;
        //    if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SI, "ViewAll") == false)
        //    {
        //        WSI.ActionBy = User.Identity.Name;
        //    }
        //    WSI.PageNum = pageNum;
        //    WSI.PageSize = pageSize;
        //    WSI.SortField = sortDataField;
        //    WSI.SortOrder = sortOrder;
        //    WSI.Keyword = keyword;
        //    WSI.Invoice.InvoiceStatus = status;
        //    List<NMSalesInvoicesWSI> WSIs = BL.callListBL(WSI);
        //    int totalRows = 0;
        //    try
        //    {
        //        totalRows = WSIs[0].TotalRows;
        //    }
        //    catch { }
        //    var data = WSIs.Select(i => new
        //    {
        //        InvoiceId = i.Invoice.InvoiceId,
        //        InvoiceBatchId = i.Invoice.InvoiceBatchId,
        //        InvoiceDate = i.Invoice.InvoiceDate.ToString("dd/MM/yyyy"),
        //        CustomerId = (i.Customer == null) ? "" : i.Customer.CustomerId,
        //        CustomerName = (i.Customer == null) ? "" : i.Customer.CompanyNameInVietnamese,
        //        //Details = i.Invoice.SalesInvoiceDetailsList.Count,
        //        Note = i.Invoice.DescriptionInVietnamese,
        //        Reference = i.Invoice.Reference,
        //        InvoiceStatus = i.Invoice.InvoiceStatus,
        //        StatusName = NMCommon.GetStatusName(i.Invoice.InvoiceStatus),
        //        CreatedBy = i.CreatedBy.CompanyNameInVietnamese,
        //        TotalAmount = i.Invoice.DetailsList.Sum(o => o.TotalAmount).ToString("N3"),
        //        PaidAmount = (i.Receipts.Sum(x => x.ReceiptAmount) - i.Payments.Sum(x => x.PaymentAmount)).ToString("N3"),
        //        RemainingAmount = (i.Invoice.DetailsList.Sum(o => o.TotalAmount) - (i.Receipts.Sum(x => x.Amount) - i.Payments.Sum(x => x.PaymentAmount))).ToString("N3")
        //    });
        //    return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetExports(int pageNum, int pageSize, string sortDataField, string sortOrder, string status, string keyword, string referenceId, string typeId, string stockId)
        {
            NMExportsWSI WSI = new NMExportsWSI();
            NMExportsBL BL = new NMExportsBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Export, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Export = new Exports();
            WSI.Export.Reference = referenceId;
            WSI.Export.ExportTypeId = typeId;
            WSI.Export.StockId = stockId;
            WSI.Export.ExportStatus = status;
            List<NMExportsWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { ExportId = i.Export.ExportId, 
                ExportDate = i.Export.ExportDate.ToString("dd/MM/yyyy"), 
                CustomerName = (i.Customer == null) ? "" : i.Customer.CompanyNameInVietnamese, 
                Details = i.Details.Count, 
                Note = i.Export.DescriptionInVietnamese, 
                Status = i.Status.Name, 
                CreatedBy = i.CreatedBy.CompanyNameInVietnamese, 
                StockName = (i.Stock.Translation == null) ? "" : i.Stock.Translation.Name 
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetSalesInvoiceBatchs(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        //{
        //    NMSalesInvoiceBatchsWSI WSI = new NMSalesInvoiceBatchsWSI();
        //    NMSalesInvoiceBatchsBL BL = new NMSalesInvoiceBatchsBL();
        //    WSI.Mode = "SRC_OBJ";
        //    if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SIB, "ViewAll") == false)
        //    {
        //        WSI.CreatedBy = User.Identity.Name;
        //    }
        //    WSI.PageNum = pageNum;
        //    WSI.PageSize = pageSize;
        //    WSI.SortField = sortDataField;
        //    WSI.SortOrder = sortOrder;
        //    WSI.Keyword = keyword;
        //    List<NMSalesInvoiceBatchsWSI> WSIs = BL.callListBL(WSI);
        //    int totalRows = 0;
        //    try
        //    {
        //        totalRows = WSIs[0].TotalRows;
        //    }
        //    catch { }
        //    return Json(new { Rows = WSIs, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetPurchaseOrdersForAutocomplete(string keyword, int limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMPurchaseOrdersWSI> WSIs = new List<NMPurchaseOrdersWSI>();
            NMPurchaseOrdersWSI WSI;
            if (!string.IsNullOrEmpty(keyword))
            {
                WSI = new NMPurchaseOrdersWSI();
                NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
                WSI.Mode = "SRC_OBJ";
                WSI.Keyword = keyword;
                //try
                //{
                //    WSI.Limit = int.Parse(limit);
                //}
                //catch
                //{
                //    WSI.Limit = 0;
                //}
                WSIs = BL.callListBL(WSI);
            }
            if (mode == "select")
            {
                WSI = new NMPurchaseOrdersWSI();
                WSI.Order = new PurchaseOrders();
                WSI.Order.Id = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
            }
            return Json(WSIs.Select(item => item.Order), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseOrders(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string typeId)
        {
            string langId = Session["Lang"].ToString();

            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Order = new PurchaseOrders();
            WSI.Order.OrderTypeId = typeId;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.PO, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            List<NMPurchaseOrdersWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                OrderId = i.Order.Id,
                OrderDate = i.Order.OrderDate.ToString("dd/MM/yyyy"),
                DeliveryDate = i.Order.DeliveryDate.ToString("dd/MM/yyyy"),
                Reference = i.Order.Reference,
                Transportation = i.Order.Transportation,
                CustomerId = (i.Supplier == null) ? "" : i.Supplier.CustomerId,
                CustomerName = (i.Supplier == null) ? "" : i.Supplier.CompanyNameInVietnamese,
                //Details = i.Details.Count,
                Note = i.Order.Description,
                OrderStatus = i.Order.OrderStatus,
                CreatedBy = i.CreatedUser.CompanyNameInVietnamese,
                Status = NMCommon.GetStatusName(i.Order.OrderStatus, langId),//.Status.Name,
                TotalAmount = i.Order.TotalAmount.ToString("N3")
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseInvoices(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string status, string orderId)
        {
            string langId = Session["Lang"].ToString();

            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.PI, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Invoice = new PurchaseInvoices();
            WSI.Invoice.InvoiceStatus = status;
            if (!string.IsNullOrEmpty(orderId))
                WSI.Invoice.Reference = orderId;
            List<NMPurchaseInvoicesWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                InvoiceId = i.Invoice.InvoiceId,
                InvoiceBatchId = i.Invoice.InvoiceBatchId,
                CustomerId = i.Supplier.CustomerId,
                CustomerName = i.Supplier.CompanyNameInVietnamese,
                Details = i.Details.Count,
                Note = i.Invoice.DescriptionInVietnamese,
                InvoiceStatus = i.Invoice.InvoiceStatus,
                Status = NMCommon.GetStatusName(i.Invoice.InvoiceStatus, langId),//.Status.Name,
                CreatedBy = i.Invoice.CreatedBy
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetImports(int pageNum, int pageSize, string sortDataField, string sortOrder, string status, string keyword, string typeId, string stockId)
        {
            string langId = Session["Lang"].ToString();

            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportStatus = status;
            WSI.Import.ImportTypeId = typeId;
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.TransferProduct) == false)
                WSI.Import.StockId = stockId;
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Import) == false)
                WSI.ActionBy = User.Identity.Name;
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            List<NMImportsWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { 
                ImportId = i.Import.ImportId, 
                ImportDate = i.Import.ImportDate.ToString("dd/MM/yyyy"), 
                CustomerName = (i.Supplier == null) ? "" : i.Supplier.CompanyNameInVietnamese, 
                Details = i.Details.Count, 
                DescriptionInVietnamese = i.Import.DescriptionInVietnamese,
                Status = NMCommon.GetStatusName(i.Import.ImportStatus, langId),//.Status.Name, // == null) ? "" : i.Status.Name, 
                CreatedBy = i.CreatedUser.CompanyNameInVietnamese, 
                Reference = i.Import.Reference, 
                StockName = (i.Stock.Translation == null) ? "" : i.Stock.Translation.Name });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetPurchaseInvoiceBatchs(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        //{
        //    NMPurchaseInvoiceBatchsWSI WSI = new NMPurchaseInvoiceBatchsWSI();
        //    NMPurchaseInvoiceBatchsBL BL = new NMPurchaseInvoiceBatchsBL();
        //    WSI.Mode = "SRC_OBJ";
        //    if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.PIB, "ViewAll") == false)
        //    {
        //        WSI.CreatedBy = User.Identity.Name;
        //    }
        //    WSI.PageNum = pageNum;
        //    WSI.PageSize = pageSize;
        //    WSI.SortField = sortDataField;
        //    WSI.SortOrder = sortOrder;
        //    WSI.Keyword = keyword;
        //    List<NMPurchaseInvoiceBatchsWSI> WSIs = BL.callListBL(WSI);
        //    int totalRows = 0;
        //    try
        //    {
        //        totalRows = WSIs[0].TotalRows;
        //    }
        //    catch { }
        //    return Json(new { Rows = WSIs, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetProductForAutocomplete(string keyword, string limit, string mode)
        {
            string langId = Session["Lang"].ToString();

            List<NMProductsWSI> WSIs = new List<NMProductsWSI>();
            NMProductsWSI WSI;
            WSI = new NMProductsWSI();
            NMProductsBL BL = new NMProductsBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            try
            {
                WSI.Limit = int.Parse(limit);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSI.LanguageId = Session["Lang"].ToString();
            WSIs = BL.callListBL(WSI);

            if (mode == "select")
            {
                WSI = new NMProductsWSI();
                WSI.Product = new Products();
                WSI.Product.ProductId = "Search";
                WSI.Translation = new Translations();
                WSI.Translation.Name = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMProductsWSI();
                WSI.Product = new Products();
                WSI.Product.ProductId = "CreateOrEdit";
                WSI.Translation = new Translations();
                WSI.Translation.Name = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            var data = WSIs.Select(i => new { ProductId = i.Product.ProductId, ProductNameInVietnamese = (i.Translation == null) ? "" : i.Translation.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(string categoryId, int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        {
            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Product = new Products();
            WSI.Keyword = keyword;
            WSI.Product.CategoryId = categoryId;
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Products, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.LanguageId = Session["Lang"].ToString();
            List<NMProductsWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                ProductId = i.Product.ProductId,
                ProductCode = i.Product.ProductCode,
                Name = (i.Translation == null || String.IsNullOrEmpty(i.Translation.Name)) ? "[Không tên]" : i.Translation.Name,
                Price = (i.PriceForSales == null) ? 0 : i.PriceForSales.Price,
                Discontinued = (i.Product.Discontinued != null && i.Product.Discontinued.Value) ? true : false,
                VATRate = i.Product.VATRate.ToString("N3")
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPCForAutocomplete(string keyword, string limit, string mode, string objectName)
        {
            string langId = Session["Lang"].ToString();

            List<NMCategoriesWSI> WSIs = new List<NMCategoriesWSI>();
            NMCategoriesWSI WSI;
            WSI = new NMCategoriesWSI();
            NMCategoriesBL BL = new NMCategoriesBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Category = new Categories();
            WSI.Category.ObjectName = objectName;
            WSI.Keyword = keyword;
            WSI.Category.ParentId = "";
            try
            {
                WSI.Limit = int.Parse(limit);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSI.LanguageId = Session["Lang"].ToString();
            WSIs = BL.callListBL(WSI);
            if (mode == "select")
            {
                WSI = new NMCategoriesWSI();
                WSI.Category = new Categories();
                WSI.Category.Id = "Search";
                WSI.Category.Name = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSI.FullName = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMCategoriesWSI();
                WSI.Category = new Categories();
                WSI.Category.Id = "CreateOrEdit";
                WSI.Category.Name = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSI.FullName = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            var data = WSIs.Select(i => new { Id = i.Category.Id, Name = i.Category.Name, FullName = i.FullName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPCs(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string objectName,string parentId)
        {
            NMCategoriesWSI WSI = new NMCategoriesWSI();
            NMCategoriesBL BL = new NMCategoriesBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Category = new Categories();
            WSI.Category.ObjectName = objectName;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.ProductCategories, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Category.ParentId = parentId;
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.LanguageId = Session["Lang"].ToString();
            List<NMCategoriesWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new {   Id = i.Category.Id, 
                                                Code = i.Category.CustomerCode,
                                                Name = i.Category.Name, 
                                                Note = i.Translation.Description,
                                                FullName = i.FullName });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocuments(int pageNum, int pageSize, string sortDataField, string sortOrder, 
            string keyword, string typeId, string categoryId, string groupId, string statusId, string owner)
        {
            NMDocumentsBL BL = new NMDocumentsBL();
            NMDocumentsWSI WSI = new NMDocumentsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Document = new Documents();
            WSI.Document.TypeId = typeId;
            WSI.Document.CategoryId = categoryId;
            WSI.Document.GroupId = groupId;
            WSI.Document.StatusId = statusId;
            WSI.Document.Owner = owner;
            //if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Introduces, "ViewAll") == false)
            //{
            //    WSI.ActionBy = User.Identity.Name;
            //}
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            List<NMDocumentsWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { 
                    DocumentId = i.Document.DocumentId, 
                    Name = (i.Translation == null) ? "" : i.Translation.Name, 
                    Short = (i.Translation == null) ? "" : i.Translation.ShortDescription });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStocks(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        {
            NMStocksWSI WSI = new NMStocksWSI();
            NMStocksBL BL = new NMStocksBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Stock, "ViewAll") == false)
            {
                WSI.CreatedBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.LanguageId = Session["Lang"].ToString();
            List<NMStocksWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { Id = i.Id, Name = (i.Translation == null) ? "" : i.Translation.Name, Address = (i.Translation == null) ? "" : i.Translation.Address, TypeName = i.TypeName });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductInStocksNoPaging(string keyword, string stockId, string categoryId)
        {
            if (!String.IsNullOrEmpty(categoryId))
            {
                if (String.IsNullOrEmpty(stockId))
                {
                    stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId; //UserInfoCache.User.Customer.StockId;
                }

                NMProductInStocksBL BL = new NMProductInStocksBL();
                NMProductInStocksWSI WSI = new NMProductInStocksWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.Keyword = keyword;
                WSI.PIS = new ProductInStocks();
                WSI.PIS.StockId = stockId;
                WSI.CategoryId = categoryId;
                List<NMProductInStocksWSI> WSIs = BL.callListBL(WSI);
                int x = 1;
                List<SalesInvoiceDetails> InventoryDetails = new List<SalesInvoiceDetails>();
                if (Session["InventoryDetails"] != null)
                {
                    InventoryDetails = (List<SalesInvoiceDetails>)Session["InventoryDetails"];
                }
                var data = WSIs.Select(i => new
                {
                    STT = x++,
                    StockName = (i.StockWSI != null && i.StockWSI.Translation != null) ? "" : i.StockWSI.Translation.Name,
                    ProductId = i.ProductWSI.Product.ProductId,
                    ProductNameInVietnamese = (i.ProductWSI.Translation == null) ? "" : i.ProductWSI.Translation.Name + " | " + i.ProductWSI.Product.ProductCode,
                    GoodQuantity = (i.PIS.BeginQuantity + i.PIS.ImportQuantity - i.PIS.ExportQuantity - i.PIS.BadProductInStock).ToString("N3"),
                    BadQuantity = i.PIS.BadProductInStock.ToString("N3"),
                    CurrentQuantity = (InventoryDetails.Select(y => y).Where(y => y.ProductId == i.ProductWSI.Product.ProductId).FirstOrDefault() == null) ? "" : ((i.PIS.BeginQuantity+i.PIS.ImportQuantity-i.PIS.ExportQuantity-i.PIS.BadProductInStock) - InventoryDetails.Select(y => y).Where(y => y.ProductId == i.ProductWSI.Product.ProductId).FirstOrDefault().Quantity).ToString("N3"),
                    ExportQuantity = (InventoryDetails.Select(y => y).Where(y => y.ProductId == i.ProductWSI.Product.ProductId).FirstOrDefault() == null) ? "" : InventoryDetails.Select(y => y).Where(y => y.ProductId == i.ProductWSI.Product.ProductId).FirstOrDefault().Quantity.ToString("N3")
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPriceForSales(int pageNum, int pageSize, string sortDataField, string sortOrder, string productId, string date)
        {
            NMPricesForSalesInvoiceBL BL = new NMPricesForSalesInvoiceBL();
            NMPricesForSalesInvoiceWSI WSI = new NMPricesForSalesInvoiceWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.PriceForSale = new PricesForSalesInvoice();
            WSI.PriceForSale.ProductId = productId;
            try
            {
                WSI.PriceForSale.DateOfPrice = DateTime.Parse(date);
            }
            catch { }
            List<NMPricesForSalesInvoiceWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                DateOfPrice = i.PriceForSale.DateOfPrice.ToString("dd/MM/yyyy"),
                Price = String.Format("{0:0,0}", i.PriceForSale.Price),
                CreatedBy = NMCommon.GetCustomerName(i.PriceForSale.CreatedBy)
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductInfo(string productId)
        {
            string vatRate = "", unitName = "", price = "0.00", discount = "0.00", unitId = "";
            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Product = new Products();
            WSI.Product.ProductId = productId;
            WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            WSI.LanguageId = Session["Lang"].ToString();
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                vatRate = WSI.Product.VATRate.ToString("N3");
                unitName = (WSI.Unit == null) ? "" : WSI.Unit.Name;
                discount = (WSI.PriceForSales == null) ? "0.00" : WSI.PriceForSales.Discount.ToString("N3");
                price = (WSI.PriceForSales == null) ? "0.00" : WSI.PriceForSales.Price.ToString("N3");
                unitId = WSI.Product.ProductUnit;
            }
            return Json(new { VATRate = vatRate, Discount = discount, UnitName = unitName, UnitId = unitId, Price = price });
        }

        public JsonResult GetUsers()
        {
            string data = "";
            NMCustomersBL BL = new NMCustomersBL();
            NMCustomersWSI WSI = new NMCustomersWSI();
            WSI.Mode = "SRC_OBJ";
            List<NMCustomersWSI> WSIs = BL.callListBL(WSI);
            foreach (NMCustomersWSI Item in WSIs)
            {
                if (data != "")
                {
                    data += "<tr>";
                }
                data += Item.Customer.CustomerId + "<td>" + Item.Customer.Code;
            }
            String[] arrData = Regex.Split(data, "<tr>");
            return Json(arrData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPriceForSalesInvoice(String productId)
        {
            NMPricesForSalesInvoiceWSI WSI = new NMPricesForSalesInvoiceWSI();
            NMPricesForSalesInvoiceBL BL = new NMPricesForSalesInvoiceBL();
            WSI.Mode = "SEL_CUR";
            WSI.PriceForSale = new PricesForSalesInvoice();
            WSI.PriceForSale.ProductId = productId;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.PriceForSale.Price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreaForAutocomplete(string keyword, string limit, string mode)
        {
            List<NMAreasWSI> WSIs = new List<NMAreasWSI>();
            NMAreasWSI WSI;
            NMAreasBL BL = new NMAreasBL();
            WSI = new NMAreasWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            try
            {
                WSI.Limit = int.Parse(limit);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);
            
            string langId = Session["Lang"].ToString();

            if (mode == "select")
            {
                WSI = new NMAreasWSI();
                WSI.Area = new Areas();
                WSI.Area.Id = "Search";
                WSI.Area.Name = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSI.FullName = "Search";
                WSIs.Add(WSI);
            }
            var data = WSIs.Select(i => new { Id = i.Area.Id, Name = i.Area.Name, FullName = i.FullName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreas(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        {
            NMAreasBL BL = new NMAreasBL();
            NMAreasWSI WSI = new NMAreasWSI();
            WSI.Mode = "SRC_OBJ";
            //if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Areas, "ViewAll") == false)
            //{
            //    WSI.ActionBy = User.Identity.Name;
            //}
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            List<NMAreasWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { Id = i.Area.Id, Name = i.Area.Name, FullName = i.FullName, ZipCode = i.Area.ZipCode });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShippingPolicy()
        {
            NEXMI.NMParametersBL BL = new NEXMI.NMParametersBL();
            NEXMI.NMParametersWSI WSI = new NEXMI.NMParametersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ObjectName = "ShippingPolicy";
            List<NEXMI.NMParametersWSI> WSIs = BL.callListBL(WSI);
            WSI = new NEXMI.NMParametersWSI();
            WSI.Id = ""; WSI.Name = "";
            WSIs.Insert(0, WSI);
            return Json(WSI);
        }

        public JsonResult SetStatus(string objectName, string id, string status, string typeId, string description)
        {
            return Json(NMCommon.UpdateObjectStatus(objectName, id, status, typeId, User.Identity.Name, description));
        }

        #region Upload
        public String UploadFile(HttpPostedFileBase fileData)
        {
            FileInfo fileInfo = new FileInfo(fileData.FileName);
            string filePathOriginal = Server.MapPath("~/uploads/images/");
            string filePathThumbnail = Server.MapPath("~/uploads/_thumbs/");
            string fileName = fileInfo.Name.Split('.')[0];
            string fileNameTemp = fileName;
            string fileExtension = fileInfo.Extension;
            int i = 1;
            while (System.IO.File.Exists(filePathOriginal + fileNameTemp + fileExtension))
            {
                fileNameTemp = fileName + i;
                i++;
            }
            Image originalImage = new Bitmap(fileData.InputStream);
            Image.GetThumbnailImageAbort callBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image thumbnailImage;
            //Get original Image Dimensions
            int originalHeight = originalImage.Height;
            int originalWidth = originalImage.Width;
            thumbnailImage = originalImage.GetThumbnailImage(originalImage.Width, originalImage.Height, callBack, IntPtr.Zero);
            if (originalWidth > 250 && originalWidth > originalHeight)
            {
                thumbnailImage = originalImage.GetThumbnailImage(250, (250 * originalImage.Height) / originalImage.Width, callBack, IntPtr.Zero);
            }
            if (originalHeight > 250 && originalHeight > originalWidth)
            {
                thumbnailImage = originalImage.GetThumbnailImage((250 * originalImage.Width) / originalImage.Height, 250, callBack, IntPtr.Zero);
            }
            originalImage.Save(filePathOriginal + fileNameTemp + fileExtension);
            thumbnailImage.Save(filePathThumbnail + fileNameTemp + fileExtension);
            return fileNameTemp + fileExtension;
        }

        public JsonResult DeleteFile(string id, String fileName, string type, string owner)
        {
            string result = "";

            NMImagesBL BL = new NMImagesBL();
            NMImagesWSI WSI = new NMImagesWSI();
            WSI.Mode = "DEL_OBJ";

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Id = id;
                WSI = BL.callSingleBL(WSI);
            }
            else
            {
                WSI.Name = fileName;
                WSI.Owner = owner;
                WSI = BL.callSingleBL(WSI);
            }
            
            result = WSI.WsiError;

            //try
            //{
            //    if (WSI.WsiError == "")
            //    {
            //        string filePathOriginal = "", filePathThumbnail = "";
            //        switch (type)
            //        {
            //            case "Images":
            //                filePathOriginal = Server.MapPath("~/uploads/images/" + fileName);
            //                System.IO.File.Delete(filePathOriginal);
            //                filePathThumbnail = Server.MapPath("~/uploads/_thumbs/" + fileName);
            //                System.IO.File.Delete(filePathThumbnail);
            //                break;
            //            case "Videos":
            //                filePathOriginal = Server.MapPath("~/uploads/videos/" + fileName);
            //                System.IO.File.Delete(filePathOriginal);
            //                filePathThumbnail = Server.MapPath("~/uploads/_thumbs/" + fileName);
            //                System.IO.File.Delete(filePathThumbnail);
            //                break;
            //            default:
            //                filePathOriginal = Server.MapPath("~/uploads/files/" + fileName);
            //                System.IO.File.Delete(filePathOriginal);
            //                break;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi:"
            //        + ex.Message;
            //}

            return Json(result);
        }
        
        public bool ThumbnailCallback()
        {
            return false;
        }
        #endregion

        public JsonResult CheckPermission(string functionId, string strAction)
        {
            bool isOK = GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], functionId, strAction);
            return Json(isOK, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuantityInStock(string productId, string stockId)
        {
            NMProductInStocksBL BL = new NMProductInStocksBL();
            NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.PIS = new ProductInStocks();
            WSI.PIS.ProductId = productId;
            WSI.PIS.StockId = stockId;
            WSI = BL.callSingleBL(WSI);
            string Good = (WSI.PIS.BeginQuantity + WSI.PIS.ImportQuantity - WSI.PIS.ExportQuantity - WSI.PIS.BadProductInStock).ToString("N3");
            return Json(new { Good = Good, Bad = WSI.PIS.BadProductInStock.ToString("N3") });
        }

        public JsonResult ChangePassword(string oldPass, string newPass)
        {
            string result = "";
            NMCustomersBL BL = new NMCustomersBL();
            NMCustomersWSI WSI = new NMCustomersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = User.Identity.Name;
            //NMCustomersWSI WSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];    // UserInfoCache.User;
            WSI = BL.callSingleBL(WSI);
            if (WSI.Customer.Password == NMCryptography.ECBEncrypt(oldPass))
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Customer.Password = NMCryptography.ECBEncrypt(newPass);
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    result = "Mật khẩu đã được thay đổi.";
                }
                else
                {
                    result = WSI.WsiError;
                }
            }
            else
            {
                result = "Mật khẩu cũ không đúng.";
            }
            return Json(result);
        }

        //public FileContentResult jqxGridExport(string filename, string format, string content)
        //{
        //    string a = Request.Params["aaa"];
        //    return File(new System.Text.UnicodeEncoding().GetBytes(content), "text/csv", filename + DateTime.Today.ToString() + ".csv");
        //}

        public void DataTable2Excel(string fileName, DataTable dtb)
        {
            Response.Charset = "";
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.ContentEncoding = Encoding.Unicode;
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            try
            {
                StringBuilder sb = new StringBuilder();
                //Tạo dòng tiêu để cho bảng tính
                //for (int count = 0; count < dtb.Columns.Count; count++)
                //{
                //    if (dtb.Columns[count].ColumnName != null)
                //        sb.Append(dtb.Columns[count].Caption);
                //    if (count < dtb.Columns.Count - 1)
                //    {
                //        sb.Append("\t");
                //    }
                //}
                //Response.Write(sb.ToString() + "\n");
                //Response.Flush();
                //Duyệt từng bản ghi 
                int soDem = 0;
                while (dtb.Rows.Count >= soDem + 1)
                {
                    sb = new StringBuilder();

                    for (int col = 0; col < dtb.Columns.Count - 1; col++)
                    {
                        if (dtb.Rows[soDem][col] != null)
                            sb.Append(dtb.Rows[soDem][col].ToString());
                        sb.Append("\t");
                    }
                    if (dtb.Rows[soDem][dtb.Columns.Count - 1] != null)
                        sb.Append(dtb.Rows[soDem][dtb.Columns.Count - 1].ToString());

                    Response.Write(sb.ToString() + "\n");
                    Response.Flush();
                    soDem = soDem + 1;
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            dtb.Dispose();
            Response.End();
        }

        //public ActionResult Products2Excel()
        //{
        //    ArrayList cols = new ArrayList();
        //    cols.Add("ProductId");
        //    cols.Add("ProductCode");
        //    cols.Add("ProductNameInVietnamese");
        //    cols.Add("Price");
        //    cols.Add("Discontinued");
        //    cols.Add("VATRate");
        //    cols.Add("CustomerName");
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
        //}

        public ActionResult Orders2Excel(string keyword, string type, string mode,string status, string from, string to)
        {
            string langId = Session["Lang"].ToString();

            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Order.OrderGroup = NMConstant.SOrderGroups.Sale;
            WSI.Order.OrderTypeId = type;
            WSI.Keyword = keyword;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SalesOrder, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Order.OrderStatus = status;
            WSI.FromDate = from;
            WSI.ToDate = to;
            List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            
            //if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == true)
            if (mode == "TH")
            {
                string[] row = new string[13];
                row[0] = "#";
                row[1] = NEXMI.NMCommon.GetInterface("ORDER_NO", langId);
                row[2] = "Ngày";
                row[3] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
                row[4] = "Số sản phẩm";
                row[5] = "Ghi chú";
                row[6] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId);
                row[7] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId);
                row[8] = "Thuế VAT";
                row[9] = "Tổng sau thuế";
                row[10] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[11] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[12] = NEXMI.NMCommon.GetInterface("STATUS", langId);
                items.Add(row);
                int count = 0;
                foreach (NMSalesOrdersWSI Item in WSIs)
                {
                    row = new string[13];
                    row[0] = (++count).ToString();
                    row[1] = Item.Order.OrderId;
                    row[2] = Item.Order.OrderDate.ToString("dd/MM/yyyy");
                    row[3] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                    row[4] = Item.Order.OrderDetailsList.Count.ToString();
                    row[5] = Item.Order.Description;
                    if (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SalesOrder))
                    {
                        row[6] = Item.Order.Amount.ToString("N3");
                        row[7] = Item.Order.Discount.ToString("N3");
                        row[8] = Item.Order.Tax.ToString("N3");
                        row[9] = Item.Order.TotalAmount.ToString("N3");
                    }
                    else
                    {
                        row[6] = ""; row[7] = ""; row[8] = ""; row[9] = "";
                    }
                    row[10] = Item.CreatedBy.CompanyNameInVietnamese;
                    row[11] = NMCommon.GetName(Item.Order.StockId, langId);
                    row[12] = NMCommon.GetStatusName(Item.Order.OrderStatus, langId);
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[15];
                row[0] = "#";
                row[1] = "ID";
                row[2] = "Ngày";
                row[3] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = "Mã số";
                row[6] = "PTVC";
                row[7] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[8] = "Tên Sản phẩm";
                row[9] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[10] = NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId);
                row[11] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId);
                row[12] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId);
                row[13] = "Thuế";
                row[14] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) + " sau thuế";

                items.Add(row);
                int count = 1;
                foreach (NMSalesOrdersWSI Item in WSIs)
                {
                    Item.Details = Item.Order.OrderDetailsList.ToList();
                    foreach (SalesOrderDetails detail in Item.Details)
                    {
                        row = new string[15];
                        row[0] = count++.ToString();
                        row[1] = Item.Order.OrderId;
                        row[2] = Item.Order.OrderDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Order.StockId, langId);
                        row[5] = Item.Order.Reference;
                        row[6] = Item.Order.Transportation;
                        row[7] = NMCommon.GetProductCode(detail.ProductId);
                        row[8] = NMCommon.GetName(detail.ProductId, langId);
                        row[9] = detail.Quantity.ToString();
                        if (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SalesOrder))
                        {
                            row[10] = (detail.Price).ToString("N3");
                            row[11] = detail.Amount.ToString("N3");
                            row[12] = detail.DiscountAmount.ToString("N3");
                            row[13] = detail.TaxAmount.ToString("N3");
                            row[14] = detail.TotalAmount.ToString("N3");
                        }
                        else
                        {
                            row[10] = ""; row[11] = ""; row[12] = ""; row[13] = ""; row[14] = "";
                        }
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("DHB-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult Quotations2Excel(string keyword)
        {
            string langId = Session["Lang"].ToString();

            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Order.OrderGroup = NMConstant.SOrderGroups.Sale;
            WSI.Order.OrderTypeId = NMConstant.SOType.Quotation;
            WSI.Keyword = keyword;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SalesOrder, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            List<NMSalesOrdersWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            string[] row = new string[8];
            row[0] = NEXMI.NMCommon.GetInterface("ORDER_NO", langId);
            row[1] = "Ngày";
            row[2] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
            row[3] = "Số sản phẩm";
            row[4] = "Ghi chú";
            row[5] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId);
            row[6] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
            row[7] = NEXMI.NMCommon.GetInterface("STATUS", langId);
            items.Add(row);
            foreach (NMSalesOrdersWSI Item in WSIs)
            {
                row = new string[8];
                row[0] = Item.Order.OrderId;
                row[1] = Item.Order.OrderDate.ToString("dd/MM/yyyy");
                row[2] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                row[3] = Item.Details.Count.ToString();
                row[4] = Item.Order.Description;
                row[5] = Item.Order.OrderDetailsList.Sum(i=>i.TotalAmount).ToString("N3");
                row[6] = Item.CreatedBy.CompanyNameInVietnamese;
                row[7] = Item.Status.Name;
                items.Add(row);
            }
            DataTable2Excel("BG-" + DateTime.Today.ToString("dd-MM-yyyy") + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult Products2Excel(string keyword, string categoryId)
        {
            string langId = Session["Lang"].ToString();

            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Product = new Products();
            WSI.Keyword = keyword;
            WSI.Product.CategoryId = categoryId;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Products, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.LanguageId = Session["Lang"].ToString();
            List<NMProductsWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            string[] row = new string[6];
            row[0] = "Mã sản phẩm";
            row[1] = "Tên sản phẩm";
            row[2] = "Giá";
            row[3] = NEXMI.NMCommon.GetInterface("STATUS", langId);
            row[4] = "Thuế (%)";
            row[5] = "Nhà sản xuất";
            items.Add(row);
            foreach (NMProductsWSI Item in WSIs)
            {
                row = new string[6];
                row[0] = Item.Product.ProductCode;
                row[1] = (Item.Translation == null) ? "" : Item.Translation.Name;
                //row[2] = (Item.PriceForSales == null) ? "0.00" : Item.PriceForSales.Price.ToString("N3");
                row[3] = (Item.Product.Discontinued == true) ? "Ngưng bán" : "";
                row[4] = Item.Product.VATRate.ToString("N3");
                row[5] = (Item.Manufacture == null) ? "" : Item.Manufacture.CompanyNameInVietnamese;
                items.Add(row);
            }
            DataTable2Excel("SP-" + DateTime.Today.ToString("dd-MM-yyyy") + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult SaleInvoices2Excel(string keyword, string status, string mode, string from, string to)
        {
            string langId = Session["Lang"].ToString();

            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.SI, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Keyword = keyword;
            WSI.Invoice.InvoiceStatus = status;
            WSI.GetDetails = true;
            WSI.FromDate = from;
            WSI.ToDate = to;
            List<NMSalesInvoicesWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            if (mode == "TH")
            {
                string[] row = new string[8];
                row[0] = NEXMI.NMCommon.GetInterface("INVOICE_NO", langId);
                row[1] = NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) ;
                row[2] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
                row[3] = "Số sản phẩm";
                row[4] = "Ghi chú";
                row[5] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[6] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId);
                row[7] = NEXMI.NMCommon.GetInterface("PAID", langId);
                items.Add(row);
                foreach (NMSalesInvoicesWSI Item in WSIs)
                {
                    row = new string[8];
                    row[0] = Item.Invoice.InvoiceId;
                    row[1] = Item.Invoice.InvoiceDate.ToString("dd/MM/yyyy");
                    row[2] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                    row[3] = Item.Details.Count.ToString();
                    row[4] = Item.Invoice.DescriptionInVietnamese;
                    row[5] = (Item.CreatedBy == null) ? "" : Item.CreatedBy.CompanyNameInVietnamese;
                    row[6] = Item.Invoice.TotalAmount.ToString("N3");
                    row[7] = (Item.Receipts.Sum(x => x.Amount) - Item.Payments.Sum(x => x.PaymentAmount)).ToString("N3");
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[15];
                row[0] = "#";
                row[1] = NEXMI.NMCommon.GetInterface("INVOICE_NO", langId);
                row[2] = NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId);
                row[3] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId) ;
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = "Mã số/PTVC";
                row[6] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[7] = "Tên Sản phẩm";
                row[8] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[9] = NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) ;
                row[10] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) ;
                row[11] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId) ;
                row[12] = "Thuế";
                row[13] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) + " sau thuế";
                row[14] = "Ghi chú";
                items.Add(row);
                int count = 1;
                foreach (NMSalesInvoicesWSI Item in WSIs)
                {
                    Item.Details = Item.Invoice.DetailsList.ToList();
                    foreach (SalesInvoiceDetails detail in Item.Details)
                    {
                        row = new string[15];
                        row[0] = count++.ToString();
                        row[1] = Item.Invoice.InvoiceId;
                        row[2] = Item.Invoice.InvoiceDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Invoice.StockId, langId);
                        row[5] = Item.Invoice.Reference;
                        row[6] = NMCommon.GetProductCode(detail.ProductId);
                        row[7] = NMCommon.GetName(detail.ProductId, langId);
                        row[8] = detail.Quantity.ToString("N3");
                        row[9] = detail.Price.ToString("N3");
                        row[10] = detail.Amount.ToString("N3");
                        row[11] = detail.DiscountAmount.ToString("N3");
                        row[12] = detail.TaxAmount.ToString("N3");
                        row[13] = detail.TotalAmount.ToString("N3");
                        row[14] = Item.Invoice.DescriptionInVietnamese;
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("HDBH-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult AR2Excel(string customerId, string from, string to)
        {
            string langId = Session["Lang"].ToString();

            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            List<NMMonthlyGeneralJournalsWSI> mgjList;

            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";
            if (!string.IsNullOrEmpty(customerId))
                WSI.MGJ.PartnerId = customerId;

            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;
            mgjList = BL.callListBL(WSI);

            //lấy thong tin khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();

            CWSI.Customer.CustomerId = customerId;
            CWSI.Mode = "SEL_OBJ";
            CWSI = CBL.callSingleBL(CWSI);

            NMSalesInvoicesBL iBL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI iWSI = new NMSalesInvoicesWSI();
            List<NMSalesInvoicesWSI> iList = new List<NMSalesInvoicesWSI>();

            foreach (NMMonthlyGeneralJournalsWSI itm in mgjList)
            {
                if (itm.MGJ.AccountId == "131" & itm.MGJ.IsBegin == false & (itm.MGJ.SIID != null))
                {
                    iWSI = new NMSalesInvoicesWSI();
                    iWSI.Mode = "SEL_OBJ";
                    iWSI.Invoice.InvoiceId = itm.MGJ.SIID;
                    iWSI = iBL.callSingleBL(iWSI);
                    iList.Add(iWSI);
                }
            }

            List<string[]> items = new List<string[]>();
            string[] row = new string[9];
            row[0] = "ĐỐI CHIẾU CÔNG NỢ KHÁCH HÀNG";
            items.Add(row);
            row = new string[9];
            row[0] = "Từ ngày " + from + " đến ngày " + to;
            items.Add(row);
            row[0] = "";
            items.Add(row);
            row = new string[9];
            row[0] = "Ngày";
            row[1] = NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId);
            row[2] = "Mã số / PTVC";
            row[3] = "ĐVT";
            row[4] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
            row[5] = NEXMI.NMCommon.GetInterface("PROMOTION", langId);
            row[6] = NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) ;
            row[7] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) ;
            row[8] = NEXMI.NMCommon.GetInterface("PAID", langId) ;
            items.Add(row);
            double totalCredit = 0, totalDebit = 0, totalAmount = 0, promote = 0;
            string productsName = "";
            foreach (NEXMI.NMMonthlyGeneralJournalsWSI mgj in mgjList)
            {
                if (mgj.MGJ.IsBegin)
                    continue;
                if (mgj.MGJ.SIID != null & mgj.MGJ.RPTID == null)
                {
                    NEXMI.NMSalesInvoicesWSI Item = iList.Where(i => i.Invoice.InvoiceId == mgj.MGJ.SIID).FirstOrDefault();
                    productsName = "";
                    foreach (NEXMI.SalesInvoiceDetails dt in Item.Details)
                        productsName += NEXMI.NMCommon.GetName(dt.ProductId, langId);
                    totalDebit += Item.Invoice.TotalAmount;
                    totalAmount += Item.Details[0].Quantity;
                    promote += Item.Details[0].Quantity * Item.Details[0].Discount / 100;

                    row = new string[9];
                    row[0] = Item.Invoice.InvoiceDate.ToString("dd-MM-yyyy");
                    row[1] = productsName;
                    row[2] = Item.Invoice.SourceDocument;
                    row[3] = "";
                    row[4] = Item.Details[0].Quantity.ToString("N3");
                    row[5] = Item.Details[0].DiscountAmount.ToString("N3");
                    row[6] = Item.Details[0].Price.ToString("N3");
                    row[7] = Item.Details[0].TotalAmount.ToString("N3");
                    row[8] = "";
                    items.Add(row);
                }
                else
                {
                    totalCredit += mgj.MGJ.CreditAmount;
                    row = new string[9];
                    row[0] = mgj.MGJ.IssueDate.ToString("dd-MM-yyyy");
                    row[1] = "";
                    row[2] = "";
                    row[3] = "";
                    row[4] = "";
                    row[5] = "";
                    row[6] = "";
                    row[7] = "";
                    row[8] = mgj.MGJ.CreditAmount.ToString("N3");
                    items.Add(row);
                }
            }

            row = new string[9];
            row[0] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId);
            row[7] = totalDebit.ToString("N3");
            items.Add(row);
            row = new string[9];
            row[0] = "Nợ đầu kỳ";
            double begin = mgjList.Where(i => i.MGJ.IsBegin == true).Sum(i => i.MGJ.DebitAmount);
            row[7] = begin.ToString("N3");                
            items.Add(row);

            row = new string[9];
            row[0] = "Nợ cuối kỳ";
            row[7] = (begin + totalDebit - totalCredit).ToString("N3");            
            items.Add(row);
            row = new string[9];
            row[0] = "Bằng chữ: " + NMCommon.ReadNum(begin+totalDebit-totalCredit) + ".";
            items.Add(row);

            row = new string[9];
            row[0] = "Ngày " + DateTime.Today.Day.ToString() + " tháng " + DateTime.Today.Month.ToString() + " năm " + DateTime.Today.Year.ToString() + ".";
            items.Add(row);
            row = new string[9];
            row[0] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId) + " xác nhận";
            row[5] = "Đại diện công ty";
            items.Add(row);

            DataTable2Excel("DCCN" + DateTime.Today.ToString("dd-MM-yyyy") + ".xls", ListToDataTable(items));
            
            return null;
        }

        public ActionResult Imports2Excel(string keyword, string status, string mode, string from, string to)
        {
            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI = new NMImportsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportStatus = status;
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Import, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Keyword = keyword;
            WSI.FromDate = from;
            WSI.ToDate = to;

            List<NMImportsWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();

            string langId = Session["Lang"].ToString();

            if (mode == "TH")
            {   
                string[] row = new string[7];
                row[0] = NMCommon.GetInterface("BILL_NO", langId);
                row[1] = NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId);
                row[2] = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
                row[3] = "Số sản phẩm";
                row[4] = "Ghi chú";
                row[5] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[6] = NEXMI.NMCommon.GetInterface("STATUS", langId);
                items.Add(row);
                foreach (NMImportsWSI Item in WSIs)
                {
                    row = new string[7];
                    row[0] = Item.Import.ImportId;
                    row[1] = Item.Import.ImportDate.ToString("dd/MM/yyyy");
                    row[2] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                    row[3] = Item.Details.Count.ToString();
                    row[4] = Item.Import.DescriptionInVietnamese;
                    row[5] = (Item.CreatedUser == null) ? "" : Item.CreatedUser.CompanyNameInVietnamese;
                    row[6] = NMCommon.GetStatusName(Item.Import.ImportStatus, langId);//.Status.Name;  // == null) ? "" : Item.Status.Name;
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[11];
                row[0] = "#";
                row[1] = "ID";
                row[2] = "Ngày";
                row[3] = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = NEXMI.NMCommon.GetInterface("SO_TITLE", langId);
                row[6] = "PTVC";
                row[7] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[8] = "Tên Sản phẩm";
                row[9] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[10] = NEXMI.NMCommon.GetInterface("STATUS", langId);
                
                items.Add(row);
                int count = 1;
                foreach (NMImportsWSI Item in WSIs)
                {
                    foreach (ImportDetails detail in Item.Details)
                    {
                        row = new string[11];
                        row[0] = count++.ToString();
                        row[1] = Item.Import.ImportId;
                        row[2] = Item.Import.ImportDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Import.StockId, langId);
                        row[5] = Item.Import.Reference;
                        row[6] = Item.Import.Transport;
                        row[7] = NMCommon.GetProductCode(detail.ProductId);
                        row[8] = NMCommon.GetName(detail.ProductId, langId);
                        row[9] = detail.GoodQuantity.ToString();
                        row[10] = NMCommon.GetStatusName(Item.Import.ImportStatus, langId);
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("PNK-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult Exports2Excel(string keyword, string status, string mode, string from, string to)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List < NMPermissionsWSI >)Session["Permissions"], NMConstant.Functions.Export, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Keyword = keyword;
            WSI.Export = new Exports();
            WSI.Export.ExportStatus = status;
            WSI.FromDate = from;
            WSI.ToDate = to;
            List<NMExportsWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            
            string langId = Session["Lang"].ToString();

            if (mode == "TH")
            {
                string[] row = new string[7];
                row[0] = NEXMI.NMCommon.GetInterface("BILL_NO", langId);
                row[1] = NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId);
                row[2] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId) ;
                row[3] = "Số sản phẩm";
                row[4] = "Ghi chú";
                row[5] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[6] = NEXMI.NMCommon.GetInterface("STATUS", langId);
                items.Add(row);
                foreach (NMExportsWSI Item in WSIs)
                {
                    row = new string[7];
                    row[0] = Item.Export.ExportId;
                    row[1] = Item.Export.ExportDate.ToString("dd/MM/yyyy");
                    row[2] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                    row[3] = Item.Details.Count.ToString();
                    row[4] = Item.Export.DescriptionInVietnamese;
                    row[5] = (Item.CreatedBy == null) ? "" : Item.CreatedBy.CompanyNameInVietnamese;
                    row[6] = (Item.Status == null) ? "" : Item.Status.Name;
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[11];
                row[0] = "#";
                row[1] = "ID";
                row[2] = "Ngày";
                row[3] = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = NEXMI.NMCommon.GetInterface("SO_TITLE", langId);
                row[6] = "PTVC";
                row[7] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[8] = "Tên Sản phẩm";
                row[9] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[10] = NEXMI.NMCommon.GetInterface("STATUS", langId);

                items.Add(row);
                int count = 1;
                foreach (NMExportsWSI Item in WSIs)
                {
                    foreach (ExportDetails detail in Item.Details)
                    {
                        row = new string[11];
                        row[0] = count++.ToString();
                        row[1] = Item.Export.ExportId;
                        row[2] = Item.Export.ExportDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Export.StockId, langId);
                        row[5] = Item.Export.Reference;
                        row[6] = Item.Export.Transport;
                        row[7] = NMCommon.GetProductCode(detail.ProductId);
                        row[8] = NMCommon.GetName(detail.ProductId, langId);
                        row[9] = detail.Quantity.ToString();
                        row[10] = NMCommon.GetStatusName(Item.Export.ExportStatus, langId);
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("PXK-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        static DataTable ListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        public ActionResult POrders2Excel(string keyword, string type, string mode,string status, string from, string to)
        {
            string langId = Session["Lang"].ToString();

            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            WSI.Mode = "SRC_OBJ";            
            WSI.Order.OrderTypeId = type;
            WSI.Keyword = keyword;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.SalesOrder, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Order.OrderStatus = status;
            WSI.FromDate = from;
            WSI.ToDate = to;
            List<NMPurchaseOrdersWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            //if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == true)
            if (mode == "TH")
            {
                string[] row = new string[13];
                row[0] = "#";
                row[1] = NEXMI.NMCommon.GetInterface("ORDER_NO", langId);
                row[2] = NEXMI.NMCommon.GetInterface("ORDER_DATE", langId);
                row[3] = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
                row[4] = "Số sản phẩm";
                row[5] = "Ghi chú";
                row[6] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId);
                row[7] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId);
                row[8] = "Thuế";
                row[9] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) + " sau thuế";
                row[10] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[11] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[12] = NEXMI.NMCommon.GetInterface("STATUS", langId);
                items.Add(row);
                int count = 1;
                foreach (NMPurchaseOrdersWSI Item in WSIs)
                {
                    row = new string[13];
                    row[0] = count++.ToString();
                    row[1] = Item.Order.Id;
                    row[2] = Item.Order.OrderDate.ToString("dd/MM/yyyy");
                    row[3] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                    row[4] = Item.Order.OrderDetailsList.Count.ToString();
                    row[5] = Item.Order.Description;
                    if (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SalesOrder))
                    {
                        row[6] = Item.Order.Amount.ToString("N3");
                        row[7] = Item.Order.Discount.ToString("N3");
                        row[8] = Item.Order.Tax.ToString("N3");
                        row[9] = Item.Order.TotalAmount.ToString("N3");
                    }
                    else
                    {
                        row[6] = ""; row[7] = ""; row[8] = ""; row[9] = "";
                    }
                    row[10] = Item.CreatedUser.CompanyNameInVietnamese;
                    row[11] = NMCommon.GetName(Item.Order.ImportStockId, langId);
                    row[12] = NMCommon.GetStatusName(Item.Order.OrderStatus, langId);
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[15];
                row[0] = "#";
                row[1] = "ID";
                row[2] = "Ngày";
                row[3] = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = "Mã số";
                row[6] = "PTVC";
                row[7] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[8] = "Tên Sản phẩm";
                row[9] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[10] = NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId);
                row[11] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId);
                row[12] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId);
                row[13] = "Thuế";
                row[14] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) + "sau thuế";

                items.Add(row);
                int count = 1;
                foreach (NMPurchaseOrdersWSI Item in WSIs)
                {
                    Item.Details = Item.Order.OrderDetailsList.ToList();
                    foreach (PurchaseOrderDetails detail in Item.Details)
                    {
                        row = new string[15];
                        row[0] = count++.ToString();
                        row[1] = Item.Order.Id;
                        row[2] = Item.Order.OrderDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Order.ImportStockId, langId);
                        row[5] = Item.Order.Reference;
                        row[6] = Item.Order.Transportation;

                        row[7] = NMCommon.GetProductCode(detail.ProductId);
                        row[8] = NMCommon.GetName(detail.ProductId, langId);
                        row[9] = detail.Quantity.ToString();

                        if (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.PO))
                        {
                            row[10] = detail.Price.ToString("N3");
                            row[11] = detail.Amount.ToString("N3");
                            row[12] = detail.DiscountAmount.ToString("N3");
                            row[13] = detail.TaxAmount.ToString("N3");
                            row[14] = detail.TotalAmount.ToString("N3");
                        }
                        else
                        {
                            row[10] = ""; row[11] = ""; row[12] = ""; row[13] = ""; row[14] = "";
                        }
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("DHM-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        public ActionResult PInvoices2Excel(string keyword, string status, string mode, string from, string to)
        {
            string langId = Session["Lang"].ToString();

            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.PI) == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.Keyword = keyword;
            WSI.Invoice.InvoiceStatus = status;
            WSI.FromDate = from;
            WSI.ToDate = to;

            List<NMPurchaseInvoicesWSI> WSIs = BL.callListBL(WSI);
            List<string[]> items = new List<string[]>();
            if (mode == "TH")
            {
                string[] row = new string[8];
                row[0] = NEXMI.NMCommon.GetInterface("INVOICE_NO", langId);
                row[1] = NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) ;
                row[2] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
                row[3] = "Số sản phẩm";
                row[4] = "Ghi chú";
                row[5] = NEXMI.NMCommon.GetInterface("CREATED_BY", langId);
                row[6] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) ;
                row[7] = NEXMI.NMCommon.GetInterface("PAID", langId);
                items.Add(row);
                foreach (NMPurchaseInvoicesWSI Item in WSIs)
                {
                    Item.Details = Item.Invoice.DetailsList.ToList();
                    row = new string[8];
                    row[0] = Item.Invoice.InvoiceId;
                    row[1] = Item.Invoice.InvoiceDate.ToString("dd/MM/yyyy");
                    row[2] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                    row[3] = Item.Details.Count.ToString();
                    row[4] = Item.Invoice.DescriptionInVietnamese;
                    row[5] = (Item.CreatedBy == null) ? "" : Item.CreatedBy.CompanyNameInVietnamese;
                    row[6] = Item.Details.Sum(i => i.TotalAmount).ToString("N3");
                    row[7] = (Item.Receipts.Sum(x => x.Amount) - Item.Payments.Sum(x => x.PaymentAmount)).ToString("N3");
                    items.Add(row);
                }
            }
            else
            {
                string[] row = new string[15];
                row[0] = "#";
                row[1] = NEXMI.NMCommon.GetInterface("INVOICE_NO", langId);
                row[2] = NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId);
                row[3] = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
                row[4] = NEXMI.NMCommon.GetInterface("STORE", langId);
                row[5] = "Mã số/PTVC";
                row[6] = NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId);
                row[7] = "Tên Sản phẩm";
                row[8] = NEXMI.NMCommon.GetInterface("QUANTITY", langId);
                row[9] = NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId);
                row[10] = NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId);
                row[11] = NEXMI.NMCommon.GetInterface("DISCOUNT", langId);
                row[12] = "Thuế";
                row[13] = NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) + " sau thuế";
                row[14] = "Ghi chú";
                items.Add(row);
                int count = 1;
                foreach (NMPurchaseInvoicesWSI Item in WSIs)
                {
                    Item.Details = Item.Invoice.DetailsList.ToList();
                    foreach (PurchaseInvoiceDetails detail in Item.Details)
                    {
                        row = new string[15];
                        row[0] = count++.ToString();
                        row[1] = Item.Invoice.InvoiceId;
                        row[2] = Item.Invoice.InvoiceDate.ToString("dd/MM/yyyy");
                        row[3] = (Item.Supplier == null) ? "" : Item.Supplier.CompanyNameInVietnamese;
                        row[4] = NMCommon.GetName(Item.Invoice.StockId, langId);
                        row[5] = Item.Invoice.Reference;
                        row[6] = NMCommon.GetProductCode(detail.ProductId);
                        row[7] = NMCommon.GetName(detail.ProductId, langId);
                        row[8] = detail.Quantity.ToString("N3");
                        row[9] = detail.Price.ToString("N3");
                        row[10] = detail.Amount.ToString("N3");
                        row[11] = detail.DiscountAmount.ToString("N3");
                        row[12] = detail.TaxAmount.ToString("N3");
                        row[13] = detail.TotalAmount.ToString("N3");
                        row[14] = Item.Invoice.DescriptionInVietnamese;
                        items.Add(row);
                    }
                }
            }
            DataTable2Excel("HDMH-" + DateTime.Today.ToString("dd-MM-yyyy") + "-" + mode + ".xls", ListToDataTable(items));
            return null;
        }

        public JsonResult CheckAuthentication()
        {
            bool value = false;
            if (Request.IsAuthenticated && Session["UserInfo"] != null)
                value = true;
            return Json(value);
        }

        public JsonResult GetLanguages()
        {
            NMTypesBL BL = new NMTypesBL();
            NMTypesWSI WSI = new NMTypesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ObjectName = "Languages";
            List<NMTypesWSI> WSIs = BL.callListBL(WSI);
            string data = "";
            foreach (NMTypesWSI Item in WSIs)
            {
                if (data != "")
                    data += ";";
                data += Item.Id;
            }
            return Json(data);
        }

        public JsonResult GetMessages(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword, string typeId)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Message = new Messages();
            WSI.Message.TypeId = typeId;
            WSI.Keyword = keyword;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Message, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            List<NMMessagesWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new { MessageId = i.Message.MessageId, Content = i.Message.Description, CreatedBy = (i.CreatedBy == null) ? "Anonymous" : i.CreatedBy.CompanyNameInVietnamese, CreatedDate = i.Message.CreatedDate.ToString("dd/MM/yyyy") });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }
                
        public JsonResult GetInvoiceInfo(string id)
        {
            string error = "", totalAmount = "", balance = "", customerId = "", customerName = "";
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            if (error == "")
            {
                if (WSI.Customer != null)
                {
                    customerId = WSI.Customer.CustomerId;
                    customerName = WSI.Customer.CompanyNameInVietnamese;
                }
                totalAmount = WSI.Invoice.TotalAmount.ToString("N3");
                balance = (WSI.Invoice.TotalAmount - WSI.Receipts.Sum(i => i.ReceiptAmount) + WSI.Payments.Sum(i => i.PaymentAmount)).ToString("N3");
            }
            return Json(new { Error = error, TotalAmount = totalAmount, Balance = balance, CustomerId = customerId, CustomerName = customerName });
        }

        public JsonResult GetTaskForAutocomplete(string keyword, string mode, string projectId)
        {
            NMTasksBL BL = new NMTasksBL();
            List<NMTasksWSI> WSIs = new List<NMTasksWSI>();
            NMTasksWSI WSI;
            //if (!string.IsNullOrEmpty(keyword))
            //{
            WSI = new NMTasksWSI();
            WSI.Mode = "SRC_OBJ";
            switch (mode)
            {
                case "select":

                    break;
                case "select1":
                    if (string.IsNullOrEmpty(projectId)) projectId = "hasValue";
                    break;
            }
            WSI.Task = new Tasks();
            WSI.Task.ProjectId = projectId;
            WSI.Keyword = keyword;
            try
            {
                WSI.Limit = int.Parse(Request.Params["limit"]);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);

            string langId = Session["Lang"].ToString();

            switch (mode)
            {
                case "select":
                    WSI = new NMTasksWSI();
                    WSI.Task = new Tasks();
                    WSI.Task.TaskId = "Search";
                    WSI.Task.TaskName = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                    WSIs.Add(WSI);
                    WSI = new NMTasksWSI();
                    WSI.Task = new Tasks();
                    WSI.Task.TaskId = "CreateOrEdit";
                    WSI.Task.TaskName = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                    WSIs.Add(WSI);
                    break;
                case "select1":
                    WSI = new NMTasksWSI();
                    WSI.Task = new Tasks();
                    WSI.Task.TaskId = "CreateOrEdit";
                    WSI.Task.TaskName = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                    WSIs.Add(WSI);
                    break;
            }
            var data = WSIs.Select(i => i.Task);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTasks(int pageNum, int pageSize, string sortDataField, string sortOrder, string txtKeyword)
        {
            NMTasksBL BL = new NMTasksBL();
            NMTasksWSI WSI = new NMTasksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Task = new Tasks();
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Customer, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            //WSI.Keyword = txtKeyword;
            List<NMTasksWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            return Json(new { Rows = WSIs.Select(item => item.Task), TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStages(int pageNum, int pageSize, string sortDataField, string sortOrder, string txtKeyword)
        {
            NMProjectStagesBL BL = new NMProjectStagesBL();
            NMProjectStagesWSI WSI = new NMProjectStagesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ProjectStage = new ProjectStages();
            //if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Project, "ViewAll") == false)
            //{
            //    WSI.ActionBy = User.Identity.Name;
            //}
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            //WSI.Keyword = txtKeyword;
            List<NMProjectStagesWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            return Json(new { Rows = WSIs.Select(item => item.ProjectStage), TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjects(int pageNum, int pageSize, string sortDataField, string sortOrder, string keyword)
        {
            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Project = new Projects();
            WSI.Keyword = keyword;
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Project, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            List<NMProjectsWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                ProjectId = i.Project.ProjectId,
                ProjectName = i.Project.ProjectName,
                CustomerName = (i.Customer != null) ? i.Customer.CompanyNameInVietnamese : "",
                PlannedTime = "",
                TotalTime = "",
                TimeSpent = "",
                StatusName = (i.Status != null) ? i.Status.Name : ""
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetProjectForAutocomplete(string keyword, string mode)
        {
            NMProjectsBL BL = new NMProjectsBL();
            List<NMProjectsWSI> WSIs = new List<NMProjectsWSI>();
            NMProjectsWSI WSI;
            //if (!string.IsNullOrEmpty(keyword))
            //{
            WSI = new NMProjectsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            try
            {
                WSI.Limit = int.Parse(Request.Params["limit"]);
            }
            catch
            {
                WSI.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);

            string langId = Session["Lang"].ToString();

            if (mode == "select")
            {
                //if (!string.IsNullOrEmpty(keyword) && WSIs.Count == 0)
                //{
                //    WSI = new NMProductsWSI();
                //    WSI.ProductId = "Create";
                //    WSI.ProductNameInVietnamese = "Create " + "'" + keyword + "'";
                //    WSIs.Add(WSI);
                //}
                WSI = new NMProjectsWSI();
                WSI.Project = new Projects();
                WSI.Project.ProjectId = "Search";
                WSI.Project.ProjectName = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                WSI = new NMProjectsWSI();
                WSI.Project = new Projects();
                WSI.Project.ProjectId = "CreateOrEdit";
                WSI.Project.ProjectName = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            return Json(WSIs.Select(i => new
            {
                ProjectId = i.Project.ProjectId,
                ProjectName = i.Project.ProjectName,
                CustomerId = (i.Customer != null) ? i.Customer.CustomerId : "",
                CustomerName = (i.Customer != null) ? i.Customer.CompanyNameInVietnamese : ""
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClearSession(String sessionName)
        {
            Session[sessionName] = null;
            return Json("");
        }

        
    }
}
