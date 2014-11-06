// AccountingController.cs

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
    public class AccountingController : Controller
    {
        //
        // GET: /Accounting/

        #region Receipts

        public ActionResult Receipts(string type)
        {
            ViewData["ACCType"] = type;
            return PartialView();
        }

        public ActionResult ReceiptGrid(string type, string pageNum, string status, string keyword, string from, string to)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            NEXMI.NMReceiptsBL BL = new NEXMI.NMReceiptsBL();
            NEXMI.NMReceiptsWSI WSI = new NEXMI.NMReceiptsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Receipt = new Receipts();
            WSI.Keyword = keyword;
            WSI.Receipt.ReceiptStatus = status;
            if(!string.IsNullOrEmpty(type))
                WSI.Receipt.ReceiptTypeId = type;
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;

            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Receipts, "ViewAll") == false)
                WSI.ActionBy = User.Identity.Name;
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public JsonResult SaveOrUpdateReceipt(String receiptId, string receiptDate, String customerId, string invoiceId, String receiptAmount, 
            String description, string receiptTypeId, string status)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            NMReceiptsWSI WSI = new NMReceiptsWSI();
            WSI.Receipt = new Receipts();
            WSI.Receipt.ReceiptId = receiptId;

            if (!string.IsNullOrEmpty(receiptId))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Receipt.ReceiptDate = NMCommon.convertDate(receiptDate);
            WSI.Receipt.CustomerId = customerId;
            WSI.Receipt.InvoiceId = invoiceId;
            
            WSI.Receipt.Amount = double.Parse(receiptAmount);
            WSI.Receipt.CurrencyId = "VND";
            WSI.Receipt.ExchangeRate = 1;
            WSI.Receipt.ReceiptAmount = WSI.Receipt.Amount / WSI.Receipt.ExchangeRate;
            
            WSI.Receipt.DescriptionInVietnamese = description;
            WSI.Receipt.ReceiptTypeId = receiptTypeId;
            WSI.Receipt.ReceiptStatus = status;
            WSI.ActionBy = User.Identity.Name;
            WSI.Receipt.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;

            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult DeleteReceipt(string id)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            NMReceiptsWSI WSI = new NMReceiptsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Receipt.ReceiptId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Payments

        public ActionResult Payments(string type)
        {
            ViewData["ACCType"] = type;
            return PartialView();
        }

        public ActionResult PaymentGrid(string type, string pageNum, string status, string keyword, string from, string to)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            NEXMI.NMPaymentsBL BL = new NEXMI.NMPaymentsBL();
            NEXMI.NMPaymentsWSI WSI = new NEXMI.NMPaymentsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Payment = new Payments();
            WSI.Keyword = keyword;
            WSI.Payment.PaymentStatus = status;
            if (!string.IsNullOrEmpty(type))
                WSI.Payment.PaymentTypeId = type;
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;

            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Payments) == false)
            {
                WSI.Payment.CreatedBy = User.Identity.Name;
            }
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        private NMAccountNumbersWSI GetTypeByID(String id)
        {
            NMAccountNumbersBL BL = new NMAccountNumbersBL();
            NMAccountNumbersWSI WSI = new NMAccountNumbersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.AccountNumber.Id = id;
            WSI = BL.callSingleBL(WSI);
            return WSI;
        }

        public ActionResult PaymentForm(string id, string invoiceId, string typeId, string viewName, string windowId)
        {
            DateTime paymentDate = DateTime.Now;
            string paymentAmount = "", customerId = "", customerName = "", status = NMConstant.PaymentStatuses.Draft;
            string bankAcc = "";
            if (!string.IsNullOrEmpty(id))
            {
                NMPaymentsBL BL = new NMPaymentsBL();
                NMPaymentsWSI WSI = new NMPaymentsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Payment = new Payments();
                WSI.Payment.PaymentId = id;
                WSI = BL.callSingleBL(WSI);
                ViewData["WSI"] = WSI;
                if (WSI.Customer != null)
                {
                    customerId = WSI.Customer.CustomerId;
                    customerName = WSI.Customer.CompanyNameInVietnamese;
                }
                try
                {
                    paymentDate = WSI.Payment.PaymentDate;
                }
                catch { }
                invoiceId = WSI.Payment.InvoiceId;
                typeId = WSI.Payment.PaymentTypeId;
                status = WSI.Payment.PaymentStatus;
                //amount = WSI.Payment.Amount.ToString("N3");
                paymentAmount = WSI.Payment.PaymentAmount.ToString("N3");
                ViewData["Description"] = WSI.Payment.DescriptionInVietnamese;
                //status = (WSI.Payment.PaymentStatus != null && WSI.Payment.PaymentStatus.Value) ? "checked" : "";
                if (WSI.PaymentMethod != null)
                {
                    ViewData["PaymentMethodId"] = WSI.PaymentMethod.Id;
                    ViewData["PaymentMethodName"] = WSI.PaymentMethod.Name;
                }
                //ViewData["TypeId"] = WSI.Payment.PaymentTypeId;
                ViewData["TypeName"] = this.GetTypeByID(WSI.Payment.PaymentTypeId).AccountNumber.NameInVietnamese;// GlobalValues.GetType(WSI.Payment.PaymentTypeId);
                bankAcc = WSI.Payment.PaymentBankAccount;
                ViewData["CreatedBy"] = WSI.Payment.CreatedBy;
            }
            else
            {
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    NMPurchaseInvoicesBL piBL = new NMPurchaseInvoicesBL();
                    NMPurchaseInvoicesWSI piWSI = new NMPurchaseInvoicesWSI();
                    piWSI.Mode = "SEL_OBJ";                    
                    piWSI.Invoice.InvoiceId = invoiceId;
                    piWSI = piBL.callSingleBL(piWSI);
                    if (piWSI.Supplier != null)
                    {
                        customerId = piWSI.Supplier.CustomerId;
                        customerName = piWSI.Supplier.CompanyNameInVietnamese;
                    }
                    ViewData["TotalAmount"] = piWSI.Invoice.TotalAmount.ToString("N3");
                    ViewData["Paid"] = (piWSI.Payments.Sum(i => i.PaymentAmount) - piWSI.Receipts.Sum(i => i.ReceiptAmount)).ToString("N3");
                    typeId = "331";//piWSI.Invoice.InvoiceTypeId;

                    ViewData["disabled"] = "disabled";
                }
            }
            ViewData["PaymentId"] = id;
            ViewData["InvoiceId"] = invoiceId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["PaymentAmount"] = paymentAmount;
            ViewData["bankAccount"] = bankAcc;
            ViewData["TypeId"] = typeId;
            ViewData["Status"] = status;
            
            if (!string.IsNullOrEmpty(viewName))
                ViewData["PaymentDate"] = paymentDate.ToString("dd/MM/yyyy");
            else
                ViewData["PaymentDate"] = paymentDate.ToString("yyyy-MM-dd");
            ViewData["WindowId"] = windowId;
            return PartialView(viewName);
        }

        public JsonResult SaveOrUpdatePayment(string txtPaymentId, string cbbCustomers, string txtInvoiceId, string txtPaymentDate,
            string txtTotalAmount, string txtDescription, string slPaymentTypes, string slPaymentMethods, string txtPaid, 
            string txtPayAmount, string status, string bank, string receiveBank)
        {
            NMPaymentsBL BL = new NMPaymentsBL();
            NMPaymentsWSI WSI = new NMPaymentsWSI();
            
            WSI.Payment = new Payments();
            WSI.Payment.PaymentId = txtPaymentId;

            if (!string.IsNullOrEmpty(txtPaymentId))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Payment.PaymentDate = DateTime.Parse(txtPaymentDate);
            WSI.Payment.SupplierId = cbbCustomers;
            if(!string.IsNullOrEmpty(txtInvoiceId))
                WSI.Payment.InvoiceId = txtInvoiceId;
            double totalAmount = 0;
            double payAmount = 0;
            try
            {
                totalAmount = double.Parse(txtTotalAmount);
            }
            catch { }
            try
            {
                payAmount = double.Parse(txtPayAmount);
            }
            catch
            {
                return Json(new { id = "", error = "Chưa nhập số tiền thanh toán!" });
            }
            WSI.Payment.Amount = payAmount;
            WSI.Payment.PaymentAmount = payAmount;
            WSI.Payment.DescriptionInVietnamese = txtDescription;
            if (!String.IsNullOrEmpty(slPaymentTypes)) WSI.Payment.PaymentTypeId = slPaymentTypes;
            if (WSI.Payment.PaymentTypeId.Substring(0,3) == "112")
                WSI.Payment.ReceiveBank = receiveBank;

            if (!String.IsNullOrEmpty(slPaymentMethods)) WSI.Payment.PaymentMethodId = slPaymentMethods;
            if (!String.IsNullOrEmpty(bank)) WSI.Payment.PaymentBankAccount = bank;
            WSI.Payment.PaymentStatus = status;
            WSI.Payment.CurrencyId = "VND";
            WSI.Payment.ExchangeRate = 1;
            WSI.ActionBy = User.Identity.Name;
            WSI.Payment.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;

            WSI = BL.callSingleBL(WSI);
            return Json(new { id = WSI.Payment.PaymentId, error = WSI.WsiError });
        }

        //public ActionResult PaymentForm(string id, string invoiceId)
        //{
        //    String paymentDate = DateTime.Today.ToString("dd/MM/yyyy"), amount = "", paymentAmount = "", status = "", description = "",
        //        supplierId = "", supplierName = "", paymentTypeId = "";
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        NEXMI.NMPaymentsBL BL = new NEXMI.NMPaymentsBL();
        //        NEXMI.NMPaymentsWSI WSI = new NEXMI.NMPaymentsWSI();
        //        WSI.Mode = "SEL_OBJ";
        //        WSI.Payment = new Payments();
        //        WSI.Payment.PaymentId = id;
        //        WSI = BL.callSingleBL(WSI);
        //        if (WSI.WsiError == "")
        //        {
        //            id = WSI.Payment.PaymentId;
        //            if (WSI.Customer != null)
        //            {
        //                supplierId = WSI.Customer.CustomerId;
        //                supplierName = WSI.Customer.CompanyNameInVietnamese;
        //            }
        //            try
        //            {
        //                paymentDate = WSI.Payment.PaymentDate.ToString("dd/MM/yyyy");
        //            }
        //            catch { }
        //            paymentTypeId = WSI.Payment.PaymentTypeId;
        //            amount = WSI.Payment.Amount.ToString("N3");
        //            paymentAmount = WSI.Payment.PaymentAmount.ToString("N3");
        //            status = WSI.Payment.PaymentStatus.ToString();
        //            description = WSI.Payment.DescriptionInVietnamese;
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(invoiceId))
        //        {
        //            NEXMI.NMPurchaseInvoicesBL PIBL = new NEXMI.NMPurchaseInvoicesBL();
        //            NEXMI.NMPurchaseInvoicesWSI PIWSI = new NEXMI.NMPurchaseInvoicesWSI();
        //            PIWSI.Mode = "SEL_OBJ";
        //            PIWSI.Invoice = new PurchaseInvoices();
        //            PIWSI.Invoice.InvoiceId = invoiceId;
        //            PIWSI = PIBL.callSingleBL(PIWSI);
        //            supplierId = PIWSI.Customer.CustomerId;
        //            supplierName = PIWSI.Customer.CompanyNameInVietnamese;
        //        }
        //    }
        //    ViewData["id"] = id;
        //    ViewData["SupplierId"] = supplierId;
        //    ViewData["SupplierName"] = supplierName;
        //    ViewData["paymentDate"] = paymentDate;
        //    ViewData["slPaymentTypes"] = paymentTypeId;
        //    ViewData["amount"] = amount;
        //    ViewData["paymentAmount"] = paymentAmount;
        //    ViewData["status"] = status;
        //    ViewData["description"] = description;
        //    return PartialView();
        //}

        //public JsonResult SaveOrUpdatePayment(String paymentId, string paymentDate, String supplierId, string invoiceId, String paymentAmount,
        //    String description, string paymentTypeId, string status)
        //{
        //    NMPaymentsBL BL = new NMPaymentsBL();
        //    NMPaymentsWSI WSI = new NMPaymentsWSI();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.Payment = new Payments();
        //    WSI.Payment.PaymentId = paymentId;
        //    WSI.Payment.PaymentDate = NMCommon.convertDate(paymentDate);
        //    WSI.Payment.SupplierId = supplierId;
        //    WSI.Payment.InvoiceId = invoiceId;
        //    WSI.Payment.Amount = double.Parse(paymentAmount);
        //    WSI.Payment.PaymentAmount =  double.Parse(paymentAmount);
        //    WSI.Payment.DescriptionInVietnamese = description;
        //    WSI.Payment.PaymentTypeId = paymentTypeId;
        //    WSI.Payment.PaymentStatus =  bool.Parse(status);
        //    WSI.ActionBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}

        public JsonResult DeletePayment(string id)
        {
            NMPaymentsBL BL = new NMPaymentsBL();
            NMPaymentsWSI WSI = new NMPaymentsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Payment = new Payments();
            WSI.Payment.PaymentId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult ApprovePayment(string id, string statusId)
        {
            NMPaymentsBL BL = new NMPaymentsBL();
            NMPaymentsWSI WSI = new NMPaymentsWSI();
            WSI.Mode = "APP_OBJ";
            WSI.Payment = new Payments();
            WSI.Payment.PaymentId = id;
            WSI.Payment.PaymentStatus = statusId;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            
            return Json(WSI.WsiError);
        }
        #endregion

        #region CashBalances

        public ActionResult CashBalances()
        {
            return PartialView();
        }

        public ActionResult CashBalancesUC(String payMethod, string bankAcc, string partnerId, string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            if(payMethod == NMConstant.PaymentMethod.Cash)
                WSI.MGJ.AccountId = "1111";
            else
                WSI.MGJ.AccountId = "1121";
            if(!String.IsNullOrEmpty(bankAcc))
                WSI.MGJ.BankId = bankAcc;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.MGJ.PartnerId = partnerId;

            if (!string.IsNullOrEmpty(from))
                WSI.Filter.FromDate = from;
            if (!string.IsNullOrEmpty(to))
                WSI.Filter.ToDate = to;

            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["fromDate"] = from;
            ViewData["toDate"] = to;

            return PartialView();
        }
        #endregion

        #region CloseMonth
        public ActionResult CloseMonth()
        {
            return View();
        }

        public ActionResult CloseMonth(string monthYear, string closeType)
        {
            ViewData["MonthYear"] = monthYear;
            ViewData["CloseType"] = closeType;
            return PartialView();
        }
        #endregion

        //#region ProductInStocks
        //public ActionResult ProductInStocks()
        //{
        //    return View();
        //}

        //public ActionResult ProductInStocks(String stockId, string monthYear, string fromDate, string toDate)
        //{
        //    ViewData["StockId"] = stockId;
        //    ViewData["MonthYear"] = monthYear;
        //    ViewData["FromDate"] = fromDate;
        //    ViewData["ToDate"] = toDate;
        //    return PartialView();
        //}

        //public ActionResult ProductInStockForm(String productId, String stockId)
        //{
        //    ViewData["ProductId"] = productId;
        //    ViewData["StockId"] = stockId;
        //    return PartialView();
        //}

        //public ActionResult UpdateStock(string stockId)
        //{
        //    ViewData["StockId"] = stockId;
        //    return View();
        //}

        //public ActionResult UpdateStock(string stockId)
        //{
        //    ViewData["StockId"] = stockId;
        //    return PartialView();
        //}

        //public JsonResult UpdateProductInStock(string stockId, string productId, string quantity, string badQuantity)
        //{
        //    NMProductInStocksBL BL = new NMProductInStocksBL();
        //    NMProductInStocksWSI WSI = new NMProductInStocksWSI();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.ProductId = productId;
        //    WSI.StockId = stockId;
        //    WSI.GoodProductInStock = quantity;
        //    WSI.BadProductInStock = badQuantity;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);

        //    return Json(WSI.WsiError);
        //}

        //public JsonResult SaveMonthlyInventoryControl(string stockId, string productId, string beginQuantity, string importQuantity, string exportQuantity, string endQuantity)
        //{
        //    NMMonthlyInventoryControlBL BL = new NMMonthlyInventoryControlBL();
        //    NMMonthlyInventoryControlWSI WSI = new NMMonthlyInventoryControlWSI();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.ProductId = productId;
        //    WSI.StockId = stockId;
        //    WSI.BeginQuantity = beginQuantity;
        //    WSI.ImportQuantity = importQuantity;
        //    WSI.ExportQuantity = exportQuantity;
        //    WSI.EndQuantity = endQuantity;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);

        //    return Json(WSI.WsiError);
        //}
        //#endregion

        #region Khoan phai thu

        public ActionResult AccountReceivable()
        {
            return PartialView();
        }

        public ActionResult AccountReceivableUC(string area, string mode, string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";

            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }

            if (!string.IsNullOrEmpty(from))
                WSI.Filter.FromDate = from;
            else
                WSI.Filter.FromDate = new DateTime(year, month, 01).ToString();

            if (!string.IsNullOrEmpty(to))
                WSI.Filter.ToDate = to;
            else
                WSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            ViewData["WSIs"] = BL.callListBL(WSI);

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            if (!string.IsNullOrEmpty(area))
            {
                CWSI.Customer.AreaId = area;
            }
            //CWSI.Customer.GroupId = NMConstant.CustomerGroups.Customer;

            ViewData["Customers"] = CBL.callListBL(CWSI);
            if (string.IsNullOrEmpty(mode))
            {
                mode = "";
            }
            ViewData["Mode"] = mode;
            ViewData["dtFrom"] = from;
            ViewData["dtTo"] = to;

            return PartialView();
        }

        public ActionResult ARDetails(string customerId, string from, string to, string mode)
        {
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
            ViewData["WSIs"] = mgjList;

            //lấy thong tin khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();

            CWSI.Customer.CustomerId = customerId;
            CWSI.Mode = "SEL_OBJ";
            CWSI = CBL.callSingleBL(CWSI);

            ViewData["Customer"] = CWSI;
            ViewData["dtFrom"] = from;
            ViewData["dtTo"] = to;


            if (mode == "detail")
            {
                NMSalesInvoicesBL iBL = new NMSalesInvoicesBL();
                NMSalesInvoicesWSI iWSI = new NMSalesInvoicesWSI();
                List<NMSalesInvoicesWSI> iList = new List<NMSalesInvoicesWSI>();                

                foreach (NMMonthlyGeneralJournalsWSI itm in mgjList)
                {
                    if (itm.MGJ.AccountId == "131" & itm.MGJ.IsBegin == false & (itm.MGJ.SIID != null))
                    {
                        iWSI= new NMSalesInvoicesWSI();
                        iWSI.Mode = "SEL_OBJ";
                        iWSI.Invoice.InvoiceId = itm.MGJ.SIID;
                        iWSI = iBL.callSingleBL(iWSI);
                        iList.Add(iWSI);
                    }
                }
                ViewData["Invoices"] = iList;
                return PartialView("ARCompare");
            }
            return PartialView();
        }

        public ActionResult PaymentRequire(string customerId, string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";
            if (!string.IsNullOrEmpty(customerId))
                WSI.MGJ.PartnerId = customerId;

            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;

            ViewData["MGJs"] = BL.callListBL(WSI);

            //lấy thong tin khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SEL_OBJ";
            CWSI.Customer.CustomerId  = customerId;
            ViewData["Customers"] = CBL.callSingleBL(CWSI);
            ViewData["slUsers"] = ((NMCustomersWSI)Session["UserInfo"]).Customer.CompanyNameInVietnamese;
            return PartialView();
        }

        

        #endregion

        #region Khoan phai tra
        // công nợ phải trả
        public ActionResult AccountPayable()
        {            
            return PartialView();
        }

        public ActionResult AccountPayableUC(string area, string mode, string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "331";
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }
            if (!string.IsNullOrEmpty(from))
                WSI.Filter.FromDate = from;
            else
                WSI.Filter.FromDate = new DateTime(year, month, 01).ToString();

            if (!string.IsNullOrEmpty(to))
                WSI.Filter.ToDate = to;
            else
                WSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            ViewData["WSIs"] = BL.callListBL(WSI);

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            if (!string.IsNullOrEmpty(area))
            {
                CWSI.Customer.AreaId = area;
            }
            //CWSI.Customer.GroupId = NMConstant.CustomerGroups.Supplier;
            ViewData["Supplier"] = CBL.callListBL(CWSI);
            if (string.IsNullOrEmpty(mode))
            {
                mode = "";
            }
            ViewData["Mode"] = mode;
            ViewData["dtFrom"] = from;
            ViewData["dtTo"] = to;

            return PartialView();
        }

        public ActionResult APDetails(string supplierId, string from, string to, string mode)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "331";
            if (!string.IsNullOrEmpty(supplierId))
                WSI.MGJ.PartnerId = supplierId;

            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;

            ViewData["WSIs"] = BL.callListBL(WSI);

            //lấy thong tin khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();

            CWSI.Customer.CustomerId = supplierId;
            CWSI.Mode = "SEL_OBJ";
            CWSI = CBL.callSingleBL(CWSI);

            ViewData["Supplier"] = CWSI;
            ViewData["dtFrom"] = from;
            ViewData["dtTo"] = to;
            ViewData["Mode"] = mode;

            return PartialView();
        }

        #endregion

        #region IS-P-LS KQHDKD

        public ActionResult IncomeStament()
        {
            return PartialView();
        }

        public ActionResult IncomeStamentUC(string from, string to)
        {
            ViewData["From"] = from;
            ViewData["To"] = to;

            return PartialView();
        }

        #endregion

        #region Beginning Amount

        public ActionResult BeginAmount()
        {         
            return PartialView();
        }

        public ActionResult CBBeginAmount()
        {
            NEXMI.NMAccountNumbersBL aBL = new NEXMI.NMAccountNumbersBL();
            NEXMI.NMAccountNumbersWSI aWsi = new NEXMI.NMAccountNumbersWSI();
            aWsi.Mode = "SRC_OBJ";
            aWsi.AccountNumber.ParentId = "111";
            aWsi.AccountNumber.IsDiscontinued = false;
            
            ViewData["Cash"] = aBL.callListBL(aWsi);

            aWsi.Mode = "SRC_OBJ";
            aWsi.AccountNumber.ParentId = "112";
            aWsi.AccountNumber.IsDiscontinued = false;
            
            ViewData["BankAcc"] = aBL.callListBL(aWsi);

            NEXMI.NMBanksBL BL = new NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NMBanksWSI();
            WSI.Mode = "SRC_OBJ";            
            ViewData["Banks"] = BL.callListBL(WSI);

            Session["MGJ"] = null;

            return PartialView();
        }

        public JsonResult AddToList(string account, string amount, string bank)
        {
            List<MonthlyGeneralJournals> List = new List<MonthlyGeneralJournals>();
            if (Session["MGJ"] != null)
                List = (List<MonthlyGeneralJournals>)Session["MGJ"];

            if (!string.IsNullOrEmpty(amount))
            {
                if (string.IsNullOrEmpty(bank))
                {
                    try
                    {
                        List.Where(i => i.AccountId == account).FirstOrDefault().DebitAmount = double.Parse(amount);
                    }
                    catch
                    {
                        MonthlyGeneralJournals mgj = new MonthlyGeneralJournals();
                        mgj.AccountId = account;
                        mgj.DebitAmount = double.Parse(amount);

                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = "COMPANY";
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";

                        List.Add(mgj);
                    }
                }
                else
                {
                    try
                    {
                        account = account.Replace(bank, "");
                        List.Where(i => i.AccountId == account & i.BankId == bank).FirstOrDefault().DebitAmount = double.Parse(amount);
                    }
                    catch
                    {
                        MonthlyGeneralJournals mgj = new MonthlyGeneralJournals();
                        mgj.AccountId = account;
                        mgj.BankId = bank;
                        mgj.DebitAmount = double.Parse(amount);

                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = "COMPANY";
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";

                        List.Add(mgj);
                    }
                }
                Session["MGJ"] = List;
            }

            return Json("");
        }

        public JsonResult SaveBeginCash(string period)
        {
            DateTime month;
            try
            {
                month = DateTime.Parse(period);
            }
            catch
            {
                return Json("Bạn chưa nhập đúng kỳ kế toán!");
            }
            string result = "";
            if (Session["MGJ"] != null)
            {
                List<MonthlyGeneralJournals> List = (List<MonthlyGeneralJournals>)Session["MGJ"];
                NMMonthlyGeneralJournalsBL bl = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI wsi = new NMMonthlyGeneralJournalsWSI();
                List<NMMonthlyGeneralJournalsWSI> mgjList;
                foreach (MonthlyGeneralJournals itm in List)
                {
                    wsi = new NMMonthlyGeneralJournalsWSI();
                    // tim co da luu chua
                    wsi.Mode = "SRC_OBJ";
                    wsi.MGJ.IssueId = itm.IssueId;
                    wsi.MGJ.PartnerId = itm.PartnerId;
                    wsi.MGJ.AccountId = itm.AccountId;
                    if (!string.IsNullOrEmpty(itm.BankId))
                        wsi.MGJ.BankId = itm.BankId;
                    mgjList = bl.callListBL(wsi);

                    if (mgjList.Count > 0)
                    {
                        itm.JournalId = mgjList[0].MGJ.JournalId;
                    }

                    itm.IssueDate = month;                                                           
                    wsi.MGJ = itm;                   

                    wsi.Filter.ActionBy = User.Identity.Name;
                    wsi.Mode = "SAV_OBJ";
                    wsi = bl.callSingleBL(wsi);
                }

                result = wsi.WsiError;

                Session.Remove("MGJ");
            }
            else
                result = "Chưa có dữ liệu!";

            return Json(result);
        }

        public ActionResult BeginARAmount()
        {
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Customer;

            ViewData["Customers"] = CBL.callListBL(CWSI);
            Session["MGJ"] = null;

            return PartialView();
        }

        public JsonResult AddARToList(string partnerId, string amount)
        {
            List<MonthlyGeneralJournals> List = new List<MonthlyGeneralJournals>();
            if (Session["MGJ"] != null)
                List = (List<MonthlyGeneralJournals>)Session["MGJ"];

            if (!string.IsNullOrEmpty(amount))
            {
                try
                {
                    List.Where(i => i.PartnerId == partnerId).FirstOrDefault().DebitAmount = double.Parse(amount);
                }
                catch
                {
                    MonthlyGeneralJournals mgj = new MonthlyGeneralJournals();
                    mgj.AccountId = "131";
                    mgj.PartnerId = partnerId;
                    mgj.DebitAmount = double.Parse(amount);
                    
                    mgj.IssueId = "SDDK";
                    mgj.PartnerId = partnerId;
                    mgj.CurrencyId = "VND";
                    mgj.ExchangeRate = 1;
                    mgj.IsBegin = true;
                    mgj.Descriptions = "Số dư đầu kỳ.";
                    mgj.CreditAmount = 0;
                                        
                    List.Add(mgj);
                }

                Session["MGJ"] = List;
            }

            return Json("");
        }

        public ActionResult BeginAPAmount()
        {
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Supplier;

            ViewData["Customers"] = CBL.callListBL(CWSI);

            Session["MGJ"] = null;

            return PartialView();
        }

        public JsonResult AddAPToList(string partnerId, string amount)
        {
            List<MonthlyGeneralJournals> List = new List<MonthlyGeneralJournals>();
            if (Session["MGJ"] != null)
                List = (List<MonthlyGeneralJournals>)Session["MGJ"];

            if (!string.IsNullOrEmpty(amount))
            {
                try
                {
                    List.Where(i => i.PartnerId == partnerId).FirstOrDefault().CreditAmount = double.Parse(amount);
                }
                catch
                {
                    MonthlyGeneralJournals mgj = new MonthlyGeneralJournals();
                    mgj.AccountId = "331";
                    mgj.PartnerId = partnerId;
                    mgj.DebitAmount = 0;
                    mgj.CreditAmount = double.Parse(amount);

                    mgj.IssueId = "SDDK";
                    mgj.PartnerId = partnerId;
                    mgj.CurrencyId = "VND";
                    mgj.ExchangeRate = 1;
                    mgj.IsBegin = true;
                    mgj.Descriptions = "Số dư đầu kỳ.";
                    

                    List.Add(mgj);
                }

                Session["MGJ"] = List;
            }

            return Json("");
        }

        #endregion

        public ActionResult GeneralJournals(string IssueId)
        {   
            if (!string.IsNullOrEmpty(IssueId))
            {
                NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.MGJ.IssueId = IssueId;
                ViewData["WSIs"] = BL.callListBL(WSI);
            }
            else
            {
                ViewData["WSIs"] = new List<NMMonthlyGeneralJournalsWSI>();
            }

            return PartialView();
        }

        public ActionResult InventoryJournals(string IssueId)
        {   
            if (!string.IsNullOrEmpty(IssueId))
            {
                NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.MGJ.IssueId = IssueId;
                ViewData["WSIs"] = BL.callListBL(WSI);
            }
            else
            {
                ViewData["WSIs"] = new List<NMMonthlyGeneralJournalsWSI>();
            }

            return PartialView();
        }


        #region Khoa so

        public ActionResult CloseMonths()
        {
            
            return PartialView();
        }

        public ActionResult CloseMonthGrid()
        {
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "SRC_OBJ";

            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult CloseMonthForm()
        {
            //lấy tất cả record NKC
            //NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            //NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            //WSI.Mode = "SRC_OBJ";
            int month = DateTime.Today.Month - 1;
            int year = DateTime.Today.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }
            
            //WSI.Filter.FromDate = new DateTime(year, month, 01).ToString();
            //WSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            //ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["FromDate"] = new DateTime(year, month, 01).ToString("yyyy-MM-dd");
            ViewData["ToDate"] = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");

            return PartialView();
        }

        public ActionResult CloseMonthCalculate(string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            int fromMonth = DateTime.Parse(from).Month;
            int toMonth = DateTime.Parse(to).Month;
            if (fromMonth != toMonth)
            {
            }

            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;

            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["FromDate"] = from;
            ViewData["ToDate"] = to;

            return PartialView();
        }

        public JsonResult SaveOrUpdateCloseMonth(string from, string to, string descriptions)
        {
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "SAV_OBJ";
            
            int year = DateTime.Parse(from).Year;
            int month = DateTime.Parse(from).Month;
            
            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;
            WSI.CloseMonth.CloseMonth = month.ToString() + "/" + year.ToString();
            WSI.CloseMonth.Descriptions = descriptions;
            WSI.Filter.ActionBy = User.Identity.Name;

            WSI = BL.callSingleBL(WSI);

            return Json(new { id = WSI.CloseMonth.CloseMonth, error = WSI.WsiError });
        }

        public ActionResult CloseMonthDetails(string closeMonth)
        {
            //lấy tất cả record NKC
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.CloseMonth.CloseMonth = closeMonth;

            ViewData["WSI"] = BL.callSingleBL(WSI);

            return PartialView();
        }

        public ActionResult CloseMonthBalanced(string closeMonth, string account)
        {
            //lấy tất cả record NKC
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "GET_DTL";
            WSI.CloseMonth.CloseMonth = closeMonth;
            WSI.AccountId = account;

            ViewData["WSI"] = BL.callSingleBL(WSI);

            return PartialView();
        }

        public ActionResult CloseMonthAR(string closeMonth)
        {
            //lấy tất cả record NKC
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "GET_DTL";
            WSI.CloseMonth.CloseMonth = closeMonth;
            WSI.AccountId = "131";

            ViewData["WSI"] = BL.callSingleBL(WSI);

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            
            ViewData["Customers"] = CBL.callListBL(CWSI);

            return PartialView();
        }

        public ActionResult CloseMonthAP(string closeMonth)
        {
            //lấy tất cả record NKC
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "GET_DTL";
            WSI.CloseMonth.CloseMonth = closeMonth;
            WSI.AccountId = "331";

            ViewData["WSI"] = BL.callSingleBL(WSI);

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";

            ViewData["Supplier"] = CBL.callListBL(CWSI);

            return PartialView();
        }

        public ActionResult CloseMonthIC(string closeMonth)
        {
            //lấy tất cả record NKC
            NMCloseMonthsBL BL = new NMCloseMonthsBL();
            NMCloseMonthsWSI WSI = new NMCloseMonthsWSI();
            WSI.Mode = "GET_IC";
            WSI.CloseMonth.CloseMonth = closeMonth;
            //WSI.AccountId = "331";

            ViewData["WSIs"] = BL.callSingleBL(WSI);

            return PartialView();
        }

        #endregion

    }
}
