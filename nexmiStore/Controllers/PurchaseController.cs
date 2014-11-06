// PurchaseController.cs

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
using NEXMI;

namespace nexmiStore.Controllers
{
    public class PurchaseController : Controller
    {
        //
        // GET: /Purchase/

        #region Order

        public ActionResult Quotations()
        {
            return PartialView();
        }

        public ActionResult PurchaseOrders(string typeId)
        {
            if (!String.IsNullOrEmpty(typeId))
            {
                ViewData["TypeId"] = typeId;
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.PQuotation;
                //ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Draft;
            }
            else
            {
                ViewData["TypeId"] = NEXMI.NMConstant.POType.PurchaseOrder;
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.PO;
                //ViewData["SOStatus"] = NEXMI.NMConstant.SOStatuses.Order;
            }

            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult PurchaseOrdersList(string pageNum, string status, string keyword, string typeId, string from, string to,
                                               string partnerId, string mode)
        {
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Order = new PurchaseOrders();
            WSI.Order.OrderTypeId = typeId;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.PO, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            //if (!string.IsNullOrEmpty(pageNum))
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
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            WSI.Order.OrderStatus = status;
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
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Order.SupplierId = partnerId;
            List<NMPurchaseOrdersWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            if (string.IsNullOrEmpty(mode))
            {
                mode = "";
            }
            ViewData["WSIs"] = WSIs;
            ViewData["POType"] = typeId;
            ViewData["Mode"] = mode;

            if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == false)
                return PartialView("POList4OneLineType");

            return PartialView();
        }


        public ActionResult POForm(string id, string mode, string type, string viewName)
        {
            string customerId = "", customerName = "", reference = "", paymentTerm = "", advances = "",
                amount = "0", totalAmount = "0", discount = "0", discountAmount = "", tax = "0", statusId = "";
            string transportation = "";
            //String orderDate = DateTime.Today.ToString("yyyy-MM-dd");
            string buyer = User.Identity.Name;
            string stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            string description = "", invoiceType = "";
            //DateTime deliveryDate ;
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            if (id != "")
            {
                NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
                WSI.Mode = "SEL_OBJ";                
                WSI.Order.Id = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    customerId = WSI.Supplier.CustomerId; customerName = WSI.Supplier.CompanyNameInVietnamese;
                    //orderDate = WSI.Order.OrderDate.ToString("yyyy-MM-dd"); 
                    reference = WSI.Order.Reference;
                    amount = WSI.Order.Amount.ToString("N3"); totalAmount = WSI.Order.TotalAmount.ToString("N3");
                    discount = WSI.Order.Discount.ToString("N3"); tax = WSI.Order.Tax.ToString("N3");
                    discountAmount = WSI.Order.Discount.ToString("N3");
                    statusId = WSI.Order.OrderStatus;
                    transportation = WSI.Order.Transportation;
                    description = WSI.Order.Description;
                    type = WSI.Order.OrderTypeId;
                    invoiceType = GlobalValues.GetACCType(WSI.Order.InvoiceTypeId).AccountNumber.NameInVietnamese;
                    
                    //try
                    //{
                    //     deliveryDate = WSI.Order.DeliveryDate.ToString();
                    //}
                    //catch { }
                    paymentTerm = WSI.Order.PaymentTerm;
                    buyer = WSI.Buyer.CompanyNameInVietnamese;
                    stockId = string.IsNullOrEmpty(WSI.Order.ImportStockId) ? "" : WSI.Order.ImportStockId;
                    ViewData["SendTo"] += ";" + WSI.Order.CreatedBy;   //gửi tin đến người tạo
                    foreach (PurchaseOrderDetails Item in WSI.Details)
                    {
                        if (mode == "Copy")
                        {
                            Item.Id = 0;
                        }
                    }

                    if (!string.IsNullOrEmpty( WSI.Order.SupplierId))
                    {
                        NMCustomersBL CBL = new NMCustomersBL();
                        NMCustomersWSI CWSI = new NMCustomersWSI();
                        CWSI.Mode = "SEL_OBJ";
                        CWSI.Customer = new Customers();
                        CWSI.Customer.CustomerId = customerId;
                        CWSI = CBL.callSingleBL(CWSI);
                        
                        ViewData["Customer"] = CWSI;
                    }
                    else
                    {
                        ViewData["Customer"] = "";
                    }
                }
            }
            if (mode == "Copy")
            {
                id = "";
            }
            ViewData["WSI"] = WSI;
            ViewData["Id"] = id;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["OrderDate"] = WSI.Order.OrderDate;//orderDate;
            ViewData["Reference"] = reference;
            ViewData["Transportation"] = transportation;
            ViewData["Amount"] = amount;
            ViewData["Discount"] = discount;
            ViewData["discountAmount"] = discountAmount;
            ViewData["Tax"] = tax;
            ViewData["TotalAmount"] = totalAmount;
            ViewData["PaymentTerm"] = paymentTerm;
            ViewData["Advances"] = advances;
            ViewData["Buyer"] = buyer;
            ViewData["DeliveryDate"] = WSI.Order.DeliveryDate;
            ViewData["PaymentDate"] = WSI.Order.PaymentDate;
            ViewData["StockId"] = stockId;
            ViewData["StockName"] = WSI.Stock != null ? WSI.Stock.Translation != null? WSI.Stock.Translation.Name: WSI.Stock.Name : "";
            ViewData["Description"] = description;
            ViewData["invoiceType"] = invoiceType;
            ViewData["TypeId"] = type;
            if (WSI.Details == null) WSI.Details = new List<PurchaseOrderDetails>();
            Session["Details"] = WSI.Details;
            ViewData["ViewType"] = "detail";

            if (type == NEXMI.NMConstant.POType.Quotation)
            {
                ViewData["Tilte"] = "Yêu cầu báo giá";
                if (statusId == "")
                    statusId = NEXMI.NMConstant.POStatuses.Draft;
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.PQuotation;
            }
            else
            {
                ViewData["Tilte"] = NEXMI.NMCommon.GetInterface("SO_TITLE", Session["Lang"].ToString());
                if (statusId == "")
                    statusId = NEXMI.NMConstant.POStatuses.Order;
                ViewData["FunctionId"] = NEXMI.NMConstant.Functions.PO;
            }
            ViewData["StatusId"] = statusId;
            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER_4_PO);
            ViewData["SalerInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_BUYER_BY_USER);

            if (!string.IsNullOrEmpty(viewName))
                return PartialView(viewName);
            return PartialView();
        }

        public JsonResult SaveOrUpdatePO(string id, string orderDate, string deliveryDate, string supplierId, string reference,
            string description, string type, string status, string trans, string stock, string buyer, string paydate, string invoiceTypes)
        {
            string error = "";
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            WSI.Order.Id = id;

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
            catch { }
            WSI.Order.SupplierId = supplierId;
            WSI.Order.Reference = reference; 
            WSI.Order.Description = description; 

            WSI.Order.OrderTypeId = type;   // NMConstant.SOType.SalesOrder;
            WSI.Order.OrderStatus = status; // NMConstant.SOStatuses.Order;
            WSI.Order.Transportation = trans;
            WSI.Order.ImportStockId = stock;
            WSI.Order.BuyerId = buyer;
            WSI.Order.PaymentDate = DateTime.Parse(paydate);
            //WSI.Order.InvoiceDate = DateTime.Parse(invoiceDate);
            WSI.Order.InvoiceTypeId = invoiceTypes;

            WSI.Details = (List<PurchaseOrderDetails>)Session["Details"];
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            return Json(new { result = WSI.Order.Id, error = error });
        }

        public JsonResult CheckAPDebit(string customerId, string maxDebitAmount)
        {
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "331";
            WSI.MGJ.PartnerId = customerId;

            List<NEXMI.NMMonthlyGeneralJournalsWSI> MGJs = BL.callListBL(WSI);
            string status = "";
            if (MGJs.Count > 0)
            {
                double begin = 0, buyinmonth = 0, paid = 0, debit = 0;
                //double totalBegin = 0, totalBuy = 0, totalPaid = 0, totalDebit = 0;

                begin = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == true)).Sum(i => i.MGJ.CreditAmount);
                buyinmonth = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.CreditAmount);
                paid = MGJs.Where(c => (c.MGJ.PartnerId == customerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.DebitAmount);
                debit = begin + buyinmonth  - paid;
                //totalBegin += begin;
                //totalBuy += buyinmonth;
                //totalPaid += paid;
                //totalDebit = debit;
                if (debit > double.Parse(maxDebitAmount))
                    status = "Y";
                else
                    status = "N";
            }

            return Json(status);
        }

        public ActionResult ConfirmPO(string id, string status, string typeId)
        {
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order.Id = id;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Order.OrderStatus = status;
                if (WSI.Order.OrderStatus == NEXMI.NMConstant.POStatuses.Order)
                    WSI.Order.OrderDate = DateTime.Now;
                if(!string.IsNullOrEmpty(typeId))
                    WSI.Order.OrderTypeId = typeId;
                WSI.ActionBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
            }

            return Json(WSI.WsiError);
        }

        public JsonResult SaveImportFromPO(string orderId)
        {
            string error = "", id = "";
            NMImportsBL ImportBL = new NMImportsBL();
            NMImportsWSI ImportWSI = new NMImportsWSI();
            ImportWSI.Mode = "SRC_OBJ";
            ImportWSI.Import.Reference = orderId;
            List<NMImportsWSI> WSIs = ImportBL.callListBL(ImportWSI);
            if (WSIs.Count <= 0)
            {
                NMPurchaseOrdersBL poBL = new NMPurchaseOrdersBL();
                ImportWSI = poBL.CreateImport(orderId);
                
                ImportWSI.Mode = "SAV_OBJ";
                ImportWSI.ActionBy = User.Identity.Name;
                ImportWSI = ImportBL.callSingleBL(ImportWSI);
                error = ImportWSI.WsiError;
                id = ImportWSI.Import.ImportId;
            }
            else
            {
                id = WSIs[0].Import.ImportId;
            }
            return Json(new { error = error, id = id });
        }

        public JsonResult DeletePO(string id)
        {
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Order = new NEXMI.PurchaseOrders();
            WSI.Order.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult POLineForm(string productId)
        {
            List<PurchaseOrderDetails> Details = new List<PurchaseOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<PurchaseOrderDetails>)Session["Details"];
            }
            foreach (PurchaseOrderDetails Item in Details)
            {
                if (Item.ProductId== productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }
            return PartialView();
        }

        public ActionResult AddPODetail(int id, string productId, string unitId, string quantity, string price, string discount, 
                                        string VATRate, string description)
        {
            String strError = "";
            List<PurchaseOrderDetails> Details = new List<PurchaseOrderDetails>();
            double detailAmount = 0;
            try
            {
                if (Session["Details"] != null)
                {
                    Details = (List<PurchaseOrderDetails>)Session["Details"];
                }
                
                PurchaseOrderDetails Detail;
                if (id > 0)
                    Detail = Details.Find(d => d.Id == id);
                else
                    Detail = Details.Find(d => d.ProductId == productId);
                
                if (Detail == null)
                {
                    Detail = new PurchaseOrderDetails();
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
                }
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            
            return PartialView("PODetails");
        }

        public JsonResult RemovePODetail(string productId)
        {
            List<PurchaseOrderDetails> Details = new List<PurchaseOrderDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<PurchaseOrderDetails>)Session["Details"];
            }
            foreach (PurchaseOrderDetails Item in Details)
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

        public ActionResult PQuotationPDFContent(string id)
        {
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new PurchaseOrders();
            WSI.Order.Id = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            return PartialView();
        }

        #endregion

        #region InvoiceBatchs

        public ActionResult PurchaseInvoiceBatchs()
        {
            return PartialView();
        }

        //public ActionResult PurchaseInvoiceBatchForm(string id)
        //{
        //    string batchId = "", status = "", description = "", batchDate = DateTime.Today.ToString("dd/MM/yyyy");
        //    NEXMI.NMPurchaseInvoiceBatchsBL BL = new NEXMI.NMPurchaseInvoiceBatchsBL();
        //    NEXMI.NMPurchaseInvoiceBatchsWSI WSI = new NEXMI.NMPurchaseInvoiceBatchsWSI();
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

        //public JsonResult SaveOrUpdatePurchaseInvoiceBatch(string id, string batchDate, string description, string status)
        //{
        //    NMPurchaseInvoiceBatchsWSI WSI = new NMPurchaseInvoiceBatchsWSI();
        //    NMPurchaseInvoiceBatchsBL BL = new NMPurchaseInvoiceBatchsBL();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.InvoiceBatchId = id;
        //    WSI.InvoiceBatchDate = NMCommon.convertDate(batchDate).ToString();
        //    WSI.BatchStatus = status;
        //    WSI.Description = description;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}

        public ActionResult PurchaseInvoiceBatchDetail(string batchId)
        {
            NEXMI.NMPurchaseInvoicesWSI WSI = new NEXMI.NMPurchaseInvoicesWSI();
            NEXMI.NMPurchaseInvoicesBL BL = new NEXMI.NMPurchaseInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Invoice = new PurchaseInvoices();
            WSI.Invoice.InvoiceBatchId = batchId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["BatchId"] = batchId;
            return PartialView();
        }

        //public JsonResult DeletePurchaseInvoiceBatch(string batchId)
        //{
        //    NMPurchaseInvoiceBatchsBL BL = new NMPurchaseInvoiceBatchsBL();
        //    NMPurchaseInvoiceBatchsWSI WSI = new NMPurchaseInvoiceBatchsWSI();
        //    WSI.Mode = "DEL_OBJ";
        //    WSI.InvoiceBatchId = batchId;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}
        #endregion

        #region Invoices
        public ActionResult PurchaseInvoices()
        {
            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult PurchaseInvoiceList(string pageNum, string sortDataField, string sortOrder, string keyword,
            string status, string orderId, string partnerId, string from, string to)
        {   
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            WSI.Mode = "SRC_OBJ";
            
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.PI, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }

            //if (mode != "Print")
            //{
                int page = 1;
                try
                {
                    page = int.Parse(pageNum);
                }
                catch { }
                WSI.PageNum = page - 1;
                WSI.PageSize = NMCommon.PageSize();
            //}
            
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
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

            WSI.Keyword = keyword;
            WSI.Invoice = new PurchaseInvoices();
            WSI.Invoice.InvoiceStatus = status;
            if (!string.IsNullOrEmpty(orderId))
                WSI.Invoice.Reference = orderId;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Invoice.SupplierId = partnerId;

            List<NMPurchaseInvoicesWSI> WSIs = BL.callListBL(WSI);
            
            ViewData["WSIs"] = WSIs;

            return PartialView();
        }

        public ActionResult PurchaseInvoiceDetail(string invoiceId)
        {
            ArrayList objs = new ArrayList();
            ArrayList obj;
            double amount = 0, totalAmount = 0;
            double discount = 0, totalDiscount = 0;
            double VATRate = 0, totaVATRate = 0;
            double shipCost = 0, otherCost = 0;

            
            NEXMI.NMPurchaseInvoicesWSI WSI = new NEXMI.NMPurchaseInvoicesWSI();
            NEXMI.NMPurchaseInvoicesBL BL = new NEXMI.NMPurchaseInvoicesBL();
            NEXMI.NMProductsBL ProductBL = new NEXMI.NMProductsBL();
            NEXMI.NMProductsWSI ProductWSI;
            NEXMI.NMProductUnitsBL PUBL = new NEXMI.NMProductUnitsBL();
            NEXMI.NMProductUnitsWSI PUWSI;
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new NEXMI.PurchaseInvoices();
            WSI.Invoice.InvoiceId = invoiceId;
            WSI = BL.callSingleBL(WSI);
            ViewData["InvoiceId"] = WSI.Invoice.InvoiceId;
            ViewData["strDate"] = WSI.Invoice.InvoiceDate.ToString("dd");
            ViewData["strMonth"] = WSI.Invoice.InvoiceDate.ToString("MM");
            ViewData["strYear"] = WSI.Invoice.InvoiceDate.ToString("yyyy");
            ViewData["invoiceStatus"] = WSI.Invoice.InvoiceStatus;
            ViewData["description"] = WSI.Invoice.DescriptionInVietnamese;
            foreach (PurchaseInvoiceDetails Item in WSI.Details)
            {
                ProductWSI = new NEXMI.NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = Item.ProductId;
                ProductWSI = ProductBL.callSingleBL(ProductWSI);
                PUWSI = new NEXMI.NMProductUnitsWSI();
                PUWSI.Mode = "SEL_OBJ";
                PUWSI.Id = ProductWSI.Product.ProductUnit;
                PUWSI = PUBL.callSingleBL(PUWSI);
                amount = Item.Quantity * Item.Price;
                discount = Item.Discount * amount / 100;
                VATRate = Item.Tax * amount / 100;
                totalDiscount += discount;
                totaVATRate += VATRate;
                totalAmount += amount;
                obj = new ArrayList();
                obj.Add(Item.ProductId);
                obj.Add((ProductWSI.Translation == null) ? "" : ProductWSI.Translation.Name);
                obj.Add(Item.Quantity.ToString("N3"));
                obj.Add(Item.Price.ToString("N3"));
                obj.Add(Item.Discount.ToString("N3"));
                obj.Add(Item.Tax.ToString("N3"));
                obj.Add(amount.ToString("N3"));
                obj.Add(PUWSI.Name);
                obj.Add(Item.OrdinalNumber);
                objs.Add(obj);
            }
            ViewData["objs"] = objs;
            ViewData["invoiceAmount"] = totalAmount + shipCost + otherCost + totaVATRate - totalDiscount;
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public ActionResult PurchaseInvoiceForm(String invoiceId, string orderId, string mode, string viewName)
        {
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();

            if (String.IsNullOrEmpty(invoiceId))
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    List<NMPurchaseInvoicesWSI> PIs = new List<NMPurchaseInvoicesWSI>();
                    WSI.Mode = "SRC_OBJ";
                    WSI.Invoice = new PurchaseInvoices();
                    WSI.Invoice.Reference = orderId;
                    PIs = BL.callListBL(WSI);
                    if (PIs.Count > 0)
                    {
                        if (PIs.Count == 1)
                        {
                            WSI.Mode = "SEL_OBJ";
                            WSI.Invoice.InvoiceId = PIs[0].Invoice.InvoiceId;
                            WSI = BL.callSingleBL(WSI);
                            viewName = "PurchaseInvoice";
                        }
                        else
                        {
                            ViewData["orderId"] = orderId;
                            viewName = "PurchaseInvoices";
                        }

                    }
                    else
                    {
                        NMPurchaseOrdersBL poBL = new NMPurchaseOrdersBL();
                        WSI = poBL.CreatePurchaseInvoice(orderId);

                        WSI.Mode = "SAV_OBJ";
                        WSI.ActionBy = User.Identity.Name;
                        
                        WSI = BL.callSingleBL(WSI);

                        //lấy thông tin khách hàng
                        if (WSI.Supplier == null)
                        {
                            NMCustomersWSI cusi = new NMCustomersWSI();
                            NMCustomersBL cubl = new NMCustomersBL();
                            cusi.Mode = "SEL_OBJ";
                            cusi.Customer.CustomerId = WSI.Invoice.SupplierId;
                            cusi = cubl.callSingleBL(cusi);
                            WSI.Supplier = cusi.Customer;
                        }
                        viewName = "PurchaseInvoice";
                    }
                }
                else
                {
                    viewName = "PurchaseInvoiceForm";
                }
            }
            else
            {
                WSI.Mode = "SEL_OBJ";
                WSI.Invoice = new PurchaseInvoices();
                WSI.Invoice.InvoiceId = invoiceId;
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    if (mode == "Copy")
                    {
                        WSI.Invoice.InvoiceId = "";
                        WSI.Invoice.InvoiceDate = DateTime.Now;
                    }
                    foreach (PurchaseInvoiceDetails Item in WSI.Details)
                    {
                        if (mode == "Copy") Item.OrdinalNumber = 0;
                    }
                }
                else
                {
                    WSI.Invoice.InvoiceDate = DateTime.Now;
                    WSI.Invoice.InvoiceStatus = NMConstant.PIStatuses.Draft;
                    WSI.Invoice.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                    WSI.Invoice.BuyerId = User.Identity.Name;
                    WSI.Details = new List<PurchaseInvoiceDetails>();
                }
            }
            Session["Details"] = WSI.Details;
            ViewData["WSI"] = WSI;
            ViewData["Mode"] = mode;
            ViewData["ViewType"] = "detail";

            ViewData["PriceInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_BUYER_BY_USER);

            return PartialView(viewName);
        }

        public ActionResult PurchaseInvoiceDetailForm(string productId)
        {
            List<PurchaseInvoiceDetails> Details = new List<PurchaseInvoiceDetails>();
            if (Session["Details"] != null)
                Details = (List<PurchaseInvoiceDetails>)Session["Details"];
            foreach (PurchaseInvoiceDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }
            return PartialView();
        }

        public ActionResult AddPurchaseInvoiceDetail(int ordinalNumber, string productId, string unitId, string quantity, string price, string discount, 
                                                    string VATRate, string description)
        {
            String strError = "";
            List<PurchaseInvoiceDetails> Details = new List<PurchaseInvoiceDetails>();
            double detailAmount = 0;
            try
            {
                if (Session["Details"] != null)
                {
                    Details = (List<PurchaseInvoiceDetails>)Session["Details"];
                }
                bool flag = true;
                PurchaseInvoiceDetails Detail;
                if (ordinalNumber > 0)
                    Detail = Details.Find(d => d.OrdinalNumber == ordinalNumber);
                else
                    Detail = Details.Find(d => d.ProductId == productId);
                
                if (Detail == null)
                {
                    Detail = new PurchaseInvoiceDetails();
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
                }
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            
            return PartialView("PurchaseInvoiceDetails");
        }

        public JsonResult RemovePurchaseInvoiceDetail(string productId)
        {
            List<PurchaseInvoiceDetails> Details = new List<PurchaseInvoiceDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<PurchaseInvoiceDetails>)Session["Details"];
            }
            foreach (PurchaseInvoiceDetails Item in Details)
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

        public JsonResult SaveOrUpdatePurchaseInvoice(string id, string invoiceDate, string customerId, string shipCost,
                    string otherCost, string description, string reference, string buyerId, string supplierReference, string stockId)
        {
            string result = "", error = "";
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            WSI.Invoice = new PurchaseInvoices();
            WSI.Invoice.InvoiceId = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Invoice.InvoiceDate = DateTime.Parse(invoiceDate);
            //WSI.Invoice.InvoiceTypeId = NMConstant.PurchaseInvoiceType.Purchase;
            WSI.Invoice.SupplierId = customerId;
            WSI.Invoice.InvoiceStatus = NMConstant.PIStatuses.Draft;
            WSI.Invoice.DescriptionInVietnamese = description;
            WSI.Invoice.Reference = reference;
            WSI.Invoice.BuyerId = buyerId;
            WSI.Invoice.CurrencyId = "VND";
            WSI.Invoice.ExchangeRate = 1;
            WSI.Invoice.SupplierReference = supplierReference;
            //try
            //{
            //    WSI.Invoice.ShipCost = double.Parse(shipCost);
            //}
            //catch { }
            //try
            //{
            //    WSI.Invoice.OtherCost = double.Parse(otherCost);
            //}
            //catch { }
            WSI.Invoice.StockId = stockId;
            WSI.ActionBy = User.Identity.Name;
            WSI.Details = (List<PurchaseInvoiceDetails>)Session["Details"];
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

        public ActionResult ConfirmInvoice( string id)
        {
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Invoice.InvoiceStatus = NMConstant.PIStatuses.Debit;                
                WSI.ActionBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
            }
            
            return Json(WSI.WsiError);
        }
        //public JsonResult ApprovalPurchaseInvoice(String invoiceId)
        //{
        //    string result = "";
        //    NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
        //    NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
        //    WSI.Mode = "SEL_OBJ";
        //    WSI.Id = invoiceId;
        //    WSI = BL.callSingleBL(WSI);
        //    if (WSI.WsiError == "")
        //    {
        //        WSI.Mode = "SAV_OBJ";
        //        WSI.InvoiceStatus = "True";
        //        WSI.CreatedBy = User.Identity.Name;
        //        WSI.ModifiedBy = User.Identity.Name;
        //        WSI = BL.callSingleBL(WSI);
        //        result = WSI.WsiError;
        //    }
        //    else
        //    {
        //        result = WSI.WsiError;
        //    }
        //    return Json(result);
        //}

        public JsonResult DeletePurchaseInvoice(String id)
        {
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Invoice = new NEXMI.PurchaseInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        //public JsonResult SavePayment(String paymentId, String paymentDate, String supplierId, String invoiceId, String amount, String paymentAmount, String description)
        //{
        //    NMPaymentsWSI WSI = new NMPaymentsWSI();
        //    NMPaymentsBL BL = new NMPaymentsBL();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.PaymentId = paymentId;
        //    WSI.PaymentDate = NMCommon.convertDate(paymentDate).ToString();
        //    WSI.SupplierId = supplierId;
        //    WSI.InvoiceId = invoiceId;
        //    WSI.Amount = amount;
        //    WSI.PaymentAmount = paymentAmount;
        //    WSI.DescriptionInVietnamese = description;
        //    WSI.PaymentTypeId = NMConstant.PaymentType.Purchase;
        //    WSI.PaymentStatus = "True";
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}

        public ActionResult PaymentForm(String invoiceId)
        {
            ViewData["InvoiceId"] = invoiceId;
            return PartialView();
        }
        #endregion

        public ActionResult AccountPayable()
        {
            return View();
        }

        public ActionResult AccountPayable(string monthYear, string fromDate, string toDate)
        {
            ViewData["MonthYear"] = monthYear;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;

            ArrayList cols = new ArrayList();
            cols.Add("SupplierId");
            cols.Add("SupplierName");
            cols.Add("BeginAmount");
            cols.Add("PurchaseAmount");
            cols.Add("PaidAmount");
            cols.Add("EndAMount");
            cols.Add("MaxDebitAmount");
            Dictionary<String, object> criterias = new Dictionary<String, object>();

            NEXMI.NMDataTableBL BL = new NEXMI.NMDataTableBL();
            NEXMI.NMDataTableWSI WSI = new NEXMI.NMDataTableWSI();
            if (string.IsNullOrEmpty(monthYear) && string.IsNullOrEmpty(fromDate))
            {
                WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, AP.BeginAmount, AP.PurchaseAmount, AP.PaidAmount, AP.EndAmount, C.MaxDebitAmount FROM Customers C, MonthlyAccountPayable AP WHERE C.CustomerId = AP.SupplierId";
            }
            else
            {
                if (string.IsNullOrEmpty(fromDate))
                {
                    WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, AP.BeginAmount, AP.PurchaseAmount, AP.PaidAmount, AP.EndAmount, C.MaxDebitAmount FROM Customers C, CloseMonthlyAccountPayableDetails AP WHERE C.CustomerId = AP.SupplierId AND CloseMonth = :MonthYear";
                    criterias.Add("MonthYear", monthYear);
                }
                else
                {
                    DateTime startDate = NEXMI.NMCommon.convertDate(fromDate);
                    DateTime endDate = NEXMI.NMCommon.convertDate(toDate);
                    WSI.QueryString = "SELECT C.CustomerId, C.CompanyNameInVietnamese, "
                        + "(SELECT AP.EndAmount FROM CloseMonthAccountPayableDetails AP WHERE AP.CloseMonth = :PreviousMonth AND AP.SupplierId = C.CustomerId) AS BeginAmount, "
                        + "(SELECT ISNULL(SUM(ID.Quantity * ID.Price + (ID.VATRate * (ID.Quantity * ID.Price) / 100) - ID.Discount * (ID.Quantity * ID.Price) / 100 + I.ShipCost + I.OtherCost), 0) FROM PurchaseInvoices I, PurchaseInvoiceDetails ID WHERE I.InvoiceId = ID.InvoiceId AND I.InvoiceDate BETWEEN :FromDate AND :ToDate AND I.SupplierId = C.CustomerId) AS PurchaseAmount, "
                        + "(SELECT ISNULL(SUM(R.PaymentAmount), 0) FROM Payments R WHERE R.PaymentDate BETWEEN :FromDate AND :ToDate AND R.SupplierId = C.CustomerId) AS PaidAmount, "
                        + "0 AS EndAmount, C.MaxDebitAmount "
                        + "FROM Customers C WHERE C.GroupId = :GroupId";
                    criterias.Add("PreviousMonth", startDate.AddMonths(-1).ToString("MM/yyyy"));
                    criterias.Add("FromDate", startDate);
                    criterias.Add("ToDate", endDate);
                    criterias.Add("GroupId", NEXMI.NMConstant.CustomerGroups.Supplier);
                }
            }
            WSI.Columns = cols;
            WSI.Criterias = criterias;
            WSI = BL.GetData(WSI);
            ViewData["dt"] = WSI.Data;
            return PartialView();
        }

        //public JsonResult SaveMonthlyAccountPayable(string supplierId, string beginAmount, string purchaseAmount, string paidAmount, string endAmount)
        //{
        //    NMMonthlyAccountPayableBL BL = new NMMonthlyAccountPayableBL();
        //    NMMonthlyAccountPayableWSI WSI = new NMMonthlyAccountPayableWSI();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.SupplierId = supplierId;
        //    WSI.BeginAmount = beginAmount;
        //    WSI.PurchaseAmount = purchaseAmount;
        //    WSI.PaidAmount = paidAmount;
        //    WSI.EndAmount = endAmount;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);
        //    return Json(WSI.WsiError);
        //}

        public ActionResult Requirements()
        {
            return PartialView();
        }

        public ActionResult RequirementList(string viewType, string pageNum, string keyword)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            NMRequirementsBL BL = new NMRequirementsBL();
            NMRequirementsWSI WSI = new NMRequirementsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Filter.PageNum = page - 1;
            WSI.Filter.PageSize = NMCommon.PageSize();
            WSI.Filter.Keyword = keyword;
            List<NEXMI.NMRequirementsWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;
            ViewData["Page"] = page;
            if (WSIs.Count > 0)
                ViewData["TotalRows"] = WSIs[0].Filter.TotalRows;
            else
                ViewData["TotalRows"] = "";
            return PartialView();
        }

        public ActionResult RequirementForm(string id, string mode)
        {
            NMRequirementsBL BL = new NMRequirementsBL();
            NMRequirementsWSI WSI = new NMRequirementsWSI();
            WSI.Requirement = new Requirements();
            WSI.Requirement.Status = NMConstant.RequirementStatuses.Draft;
            WSI.Requirement.RequireDate = DateTime.Today;

            if (!string.IsNullOrEmpty(id))
            {                
                WSI.Mode = "SEL_OBJ";                
                WSI.Requirement.Id = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.Details == null) WSI.Details = new List<RequirementDetails>();                
            }
                        
            if (mode == "Copy")
            {
                WSI.Requirement.Id = "";
                WSI.Requirement.Status = NMConstant.RequirementStatuses.Draft;
                WSI.Requirement.RequireDate = DateTime.Today;
            }
            
            ViewData["WSI"] = WSI;
            Session["Details"] = WSI.Details;

            if (mode == "detail")
                return PartialView("RequirementDetail");

            return PartialView();
        }

        public ActionResult RequirementDetailForm(string productId)
        {
            List<RequirementDetails> Details = new List<RequirementDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<RequirementDetails>)Session["Details"];
            }
            foreach (RequirementDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }
            return PartialView();
        }

        public JsonResult AddRQDetail(string productId, string quantity, string price, string description)
        {
            String strError = "";
            List<RequirementDetails> Details = new List<RequirementDetails>();
            double detailAmount = 0;
            try
            {
                if (Session["Details"] != null)
                {
                    Details = (List<RequirementDetails>)Session["Details"];
                }
                bool flag = true;
                foreach (RequirementDetails Item in Details)
                {
                    if (Item.ProductId == productId)
                    {
                        flag = false;
                        Item.Quantity = double.Parse(quantity);
                        Item.Price = double.Parse(price);                  
                        detailAmount = Item.Amount;
                        Item.Description = description;
                        break;
                    }
                }
                if (flag)
                {
                    RequirementDetails Detail = new RequirementDetails();
                    Detail.ProductId = productId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.Price = double.Parse(price);
                    detailAmount = Detail.Amount;
                    Detail.Description = description;
                    
                    Details.Add(Detail);
                }
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            return Json(new
            {
                strError = strError,
                detailAmount = detailAmount.ToString("N3"),
                totalAmount = Details.Sum(i => i.Amount).ToString("N3")
            });
        }

        public JsonResult SaveOrUpdateRequirement(string id, string type, string date, string customer, string order,
            string requireBy, string responseDay, string status, string description)
        {
            NMRequirementsBL BL = new NMRequirementsBL();
            NMRequirementsWSI WSI = new NMRequirementsWSI();
            WSI.Requirement.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Requirement.RequirementTypeId = type;
            WSI.Requirement.RequireDate = DateTime.Parse(date);
            WSI.Requirement.Status = status;
            if(!String.IsNullOrEmpty(customer))
                WSI.Requirement.CustomerId = customer;
            if(!string.IsNullOrEmpty(order))
                WSI.Requirement.OrderId = order;
            WSI.Requirement.RequiredBy = requireBy;
            WSI.Requirement.ResponseDays = responseDay;
            WSI.Requirement.Description = description;

            WSI.Filter.ActionBy = User.Identity.Name;

            WSI.Details = (List<RequirementDetails>)Session["Details"];

            WSI = BL.callSingleBL(WSI);

            string result = "", error = "";
            if (WSI.WsiError == "")
            {
                result = WSI.Requirement.Id;
                Session["Details"] = null;
            }
            else
            {
                error = WSI.WsiError;
            }
            
            return Json(new { result = result, error = error });
            
        }


        public JsonResult SetStatus(string id, string status)
        {
            string result = "", error = "";
            if (!string.IsNullOrEmpty(id))
            {
                NMRequirementsBL BL = new NMRequirementsBL();
                NMRequirementsWSI WSI = new NMRequirementsWSI();

                WSI.Mode = "SEL_OBJ";
                WSI.Requirement.Id = id;
                WSI = BL.callSingleBL(WSI);

                WSI.Mode = "SAV_OBJ";                
                WSI.Requirement.Status = status;

                WSI.Filter.ActionBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
                error = WSI.WsiError;
                result = WSI.Requirement.Id;
            }
            
            return Json(new { result = result, error = error });
        }
    }
}
