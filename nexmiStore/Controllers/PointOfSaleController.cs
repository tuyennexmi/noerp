// PointOfSaleController.cs

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
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.IO;

namespace nexmiStore.Controllers
{
    public class PointOfSaleController : Controller
    {
        //
        // GET: /PointOfSale/

        public ActionResult Sales()
        {
            Session["InvoiceDetails"] = null;
            ViewData["UserName"] = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.CompanyNameInVietnamese;
            return View();
        }

        public ActionResult divCategories(string parentId)
        {
            if (parentId == "")
            {
                parentId = null;
            }
            NMCategoriesBL BL = new NMCategoriesBL();
            NMCategoriesWSI WSI = new NMCategoriesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Category = new Categories();
            WSI.Category.ObjectName = "Products";
            WSI.Category.ParentId = parentId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult divProducts(string categoryId, string keyword)
        {
            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Product = new Products();
            WSI.Product.CategoryId = categoryId;
            WSI.Keyword = keyword;
            WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            WSI.LanguageId = Session["Lang"].ToString();
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public JsonResult AddToList(string productId, double price, double quantity, int tax, string discount, string mode)
        {
            int disc = 0;
            try
            {
                disc = int.Parse(discount);
            }
            catch { }
            List<SalesInvoiceDetails> OrderDetails = new List<SalesInvoiceDetails>();
            if (Session["InvoiceDetails"] != null)
            {
                OrderDetails = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
            }
            double amount = price * quantity;
            double discAmount = amount * disc / 100;
            double taxAmount = tax * (amount - discAmount) / 100;
            double newAmount = amount - discAmount + taxAmount, oldAmount = 0, oldTaxAmount = 0, oldDiscountAmount = 0;
            SalesInvoiceDetails Item = OrderDetails.Select(i => i).Where(i => i.ProductId == productId).FirstOrDefault();
            if (Item == null)
            {
                Item = new SalesInvoiceDetails();
                Item.ProductId = productId;
                Item.Price = price;
                Item.Quantity = quantity;
                Item.Tax = tax;
                //Item.TaxAmount = taxAmount;
                Item.Discount = disc;
                //Item.DiscountAmount = discAmount;
                //Item.Amount = amount;
                //Item.TotalAmount = newAmount;
                Item.CreatedBy = User.Identity.Name;
                Item.CreatedDate = DateTime.Now;
                OrderDetails.Add(Item);
            }
            else
            {
                if (mode == "+")
                {
                    Item.Price = price;
                    Item.Quantity += quantity;
                    Item.Tax = tax;
                    //Item.TaxAmount += taxAmount;
                    Item.Discount = disc;
                    //Item.DiscountAmount += discAmount;
                    //Item.Amount += amount;
                    //Item.TotalAmount += newAmount;
                }
                else
                {
                    oldAmount = Item.TotalAmount;
                    oldTaxAmount = Item.TaxAmount;
                    oldDiscountAmount = Item.DiscountAmount;
                    Item.Price = price;
                    Item.Quantity = quantity;
                    Item.Tax = tax;
                    //Item.TaxAmount = taxAmount;
                    Item.Discount = disc;
                    //Item.DiscountAmount = discAmount;
                    //Item.Amount = amount;
                    //Item.TotalAmount = newAmount;
                }
            }
            Session["InvoiceDetails"] = OrderDetails;
            return Json(new { Amount = (Item.Amount - Item.DiscountAmount).ToString("N3"), TotalAmount = Item.TotalAmount.ToString("N3"), newAmount = newAmount - oldAmount, taxAmount = taxAmount - oldTaxAmount, Quantity = Item.Quantity.ToString("N3"), DiscountAmount = discAmount - oldDiscountAmount });
        }

        public JsonResult RemoveFromList(string productId)
        {
            double amount = 0, taxAmount = 0, discountAmount = 0;
            if (Session["InvoiceDetails"] != null)
            {
                List<SalesInvoiceDetails> OrderDetails = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
                SalesInvoiceDetails Item = OrderDetails.Select(i => i).Where(i => i.ProductId == productId).FirstOrDefault();
                if (Item != null)
                {
                    amount = Item.Amount;
                    taxAmount = Item.TaxAmount;
                    discountAmount = Item.DiscountAmount;
                    OrderDetails.Remove(Item);
                }
                Session["InvoiceDetails"] = OrderDetails;
            }
            return Json(new { Amount = amount, TaxAmount = taxAmount, DiscountAmount = discountAmount });
        }

        public ActionResult Cash(string windowId, string totalAmount)
        {
            ViewData["WindowId"] = windowId;
            ViewData["TotalAmount"] = totalAmount;
            return PartialView();
        }

        public JsonResult SaveInvoice(string customerId, string cash, string voucherTotal)
        {
            string error = "", id = "";
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Invoice = new SalesInvoices();
            if (!string.IsNullOrEmpty(customerId))
            {
                if (NEXMI.NMCommon.GetSetting("FELIX_CONN") == true)
                {
                    NMCustomersBL cBL = new NMCustomersBL();
                    NMCustomersWSI cWSI = new NMCustomersWSI();
                    cWSI.Mode = "SRC_OBJ";
                    cWSI.Customer.Code = customerId;
                    List<NMCustomersWSI> lsCus = cBL.callListBL(cWSI);
                    if (lsCus.Count != 0)
                    {
                        WSI.Customer = lsCus[0].Customer;
                        WSI.Invoice.CustomerId = lsCus[0].Customer.CustomerId;
                    }
                    else
                    {
                        var lstResult = (IDictionary<string, object>)Session["FelixMemberInfo"];
                        cWSI.Mode = "SAV_OBJ";
                        cWSI.Customer.Code = customerId;
                        cWSI.Customer.GroupId = NMConstant.CustomerGroups.Customer;
                        cWSI.Customer.CompanyNameInVietnamese = lstResult["consumerName"].ToString();
                        cWSI.Customer.EmailAddress = lstResult["consumerEmail"].ToString();
                        cWSI.Customer.Telephone = lstResult["consumerPhone"].ToString();
                        //cWSI.Customer.Gender = lstResult["consumerGender"].ToString();
                        cWSI.ActionBy = User.Identity.Name;

                        cWSI = cBL.callSingleBL(cWSI);
                        WSI.Invoice.CustomerId = cWSI.Customer.CustomerId;
                        WSI.Customer = cWSI.Customer;
                    }
                }
                else
                {
                    WSI.Invoice.CustomerId = customerId;
                }
            }
            WSI.Invoice.InvoiceDate = DateTime.Now;
            WSI.Details = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
            
            WSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Paid;
            WSI.Invoice.SalesPersonId = User.Identity.Name;
            WSI.Invoice.StockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            WSI.ActionBy = User.Identity.Name;
            WSI.Invoice.CurrencyId = "VND";
            WSI.Invoice.ExchangeRate = 1;

            if (Session["VoucherInfo"] != null)
                WSI.Invoice.VoucherCode = Session["VoucherCode"].ToString();

            WSI = BL.callSingleBL(WSI);
            try
            {
                id = WSI.Invoice.InvoiceId;
            }
            catch { }
            error = WSI.WsiError;
            
            #region Luu phieu thu

            if (error == "")
            {
                NMReceiptsBL rBL = new NMReceiptsBL();
                NMReceiptsWSI rWSI = new NMReceiptsWSI();
                rWSI.Mode = "SAV_OBJ";
                rWSI.Receipt = new Receipts();
                rWSI.Receipt.ReceiptId = "";
                rWSI.Receipt.ReceiptDate = DateTime.Now;
                rWSI.Receipt.CustomerId = WSI.Invoice.CustomerId;
                if (!string.IsNullOrEmpty(id))
                    rWSI.Receipt.InvoiceId = id;
                rWSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;

                rWSI.Receipt.CurrencyId = "VND";
                rWSI.Receipt.ExchangeRate = 1;
                rWSI.Receipt.Amount = WSI.Invoice.TotalAmount; //WSI.Invoice.DetailsList.Sum(i => i.TotalAmount);
                rWSI.Receipt.DescriptionInVietnamese = "Thanh toán tiền mua hàng.";
                rWSI.Receipt.ReceiptTypeId = "131";
                rWSI.Receipt.PaymentMethodId = NMConstant.PaymentMethod.Cash;
                
                rWSI.ActionBy = User.Identity.Name;
                rWSI = rBL.callSingleBL(rWSI);

                error = rWSI.WsiError;

                // luu phieu xuat
                NMExportsWSI eWSI = new NMExportsWSI();
                NMExportsBL eBL = new NMExportsBL();
                eWSI.Mode = "SAV_OBJ";
                eWSI.Export = new Exports();
                eWSI.Export.ExportDate = WSI.Invoice.InvoiceDate;
                eWSI.Export.ExportTypeId = NMConstant.ExportType.ForCustomers;
                if (!string.IsNullOrEmpty(customerId))
                    eWSI.Export.CustomerId = customerId;
                eWSI.Export.ExportStatus = NMConstant.EXStatuses.Delivered;
                if (!string.IsNullOrEmpty(WSI.Invoice.InvoiceId))
                    eWSI.Export.InvoiceId = WSI.Invoice.InvoiceId;
                eWSI.Export.DescriptionInVietnamese = "";
                eWSI.Export.StockId = WSI.Invoice.StockId;
                eWSI.ActionBy = User.Identity.Name;

                var details = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
                eWSI.Details = new List<ExportDetails>();
                ExportDetails Detail;
                for (int i = 0; i < details.Count; i++)
                {
                    Detail = new ExportDetails();
                    try
                    {
                        Detail.OrdinalNumber = int.Parse(details[i].OrdinalNumber.ToString());
                    }
                    catch { }
                    Detail.ProductId = details[i].ProductId.ToString();
                    Detail.Quantity = double.Parse(details[i].Quantity.ToString());
                    eWSI.Details.Add(Detail);
                }
                eWSI = eBL.callSingleBL(eWSI);
                //if (eWSI.WsiError == "")
                //{
                //    eWSI.Export.ExportStatus = NMConstant.EXStatuses.Delivered;
                //    eWSI = eBL.callSingleBL(eWSI);
                //}

                if (error == "")
                {
                    Session["InvoiceDetails"] = null;
                    Session["FelixMemberInfo"] = null;
                    Session["VoucherCode"] = null;
                    Session["VoucherInfo"] = null;
                }
            }
            #endregion

            #region Luu thong tin hoa don Felix server 
            
            if (NEXMI.NMCommon.GetSetting("FELIX_CONN") == true)
            {
                if (this.FelixSendInvoiceInfo(this.ConvertToJson(WSI)) == false)
                    error += "\nKhông thể gửi thông tin hóa đơn sang đối tác!";

            }

            #endregion

            return Json(new { id = id, error = error });
        }

        public ActionResult PrintReceipt(string id)
        {
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            ViewData["Id"] = WSI.Invoice.InvoiceId;
            ViewData["InvoiceDate"] = WSI.Invoice.InvoiceDate.ToString("dd/MM/yyyy hh:mm");
            ViewData["SalesPerson"] = WSI.SalesPerson != null? WSI.SalesPerson.CompanyNameInVietnamese : "";
            ViewData["CustomerName"] = (WSI.Customer != null) ? WSI.Customer.CompanyNameInVietnamese : "";
            ViewData["Amount"] = WSI.Invoice.Amount.ToString("N3");
            ViewData["TotalAmount"] = WSI.Invoice.TotalAmount.ToString("N3");
            ViewData["TaxAmount"] = WSI.Invoice.Tax.ToString("N3");
            ViewData["DiscountAmount"] = WSI.Invoice.Discount.ToString("N3");
            double cash = WSI.Receipts[0].ReceiptAmount;
            double change = WSI.Receipts[0].ReceiptAmount - WSI.Receipts[0].Amount;
            ViewData["Cash"] = cash.ToString("N3");
            ViewData["change"] = change.ToString("N3");
            ViewData["Details"] = WSI.Details;
            return PartialView();
        }

        public ActionResult Map(string categoryId)
        {
            List<NMCategoriesWSI> WSIs = new List<NMCategoriesWSI>();
            NMCategoriesBL BL = new NMCategoriesBL();
            NMCategoriesWSI WSI;
            while (!String.IsNullOrEmpty(categoryId))
            {
                WSI = new NMCategoriesWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Category = new Categories();
                WSI.Category.Id = categoryId;
                WSI = BL.callSingleBL(WSI);
                WSIs.Insert(0, WSI);
                categoryId = WSI.Category.ParentId;
            }
            ViewData["WSIs"] = WSIs;
            return PartialView();
        }

        public ActionResult ImportForm(string windowId)
        {
            ViewData["ImportDate"] = DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveImport(string importDate, string supplierId, string importType, string description, string stockId, string invoiceId, string carrierId, string transport)
        {
            string result = "", error = "";
            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Mode = "SAV_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportDate = DateTime.Parse(importDate);
            WSI.Import.ImportTypeId = importType;
            if (!string.IsNullOrEmpty(invoiceId))
                WSI.Import.InvoiceId = invoiceId;
            if (!string.IsNullOrEmpty(supplierId))
                WSI.Import.SupplierId = supplierId;
            WSI.Import.StockId = stockId;
            WSI.Import.ImportStatus = NMConstant.IMStatuses.Imported;
            WSI.Import.DescriptionInVietnamese = description;
            if (!String.IsNullOrEmpty(carrierId)) WSI.Import.CarrierId = carrierId;
            WSI.Import.Transport = transport;
            WSI.ActionBy = User.Identity.Name;
            var details = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
            WSI.Details = new List<ImportDetails>();
            ImportDetails Detail;
            for (int i = 0; i < details.Count; i++)
            {
                Detail = new ImportDetails();
                try
                {
                    Detail.OrdinalNumber = int.Parse(details[i].OrdinalNumber.ToString());
                }
                catch { }
                Detail.ProductId = details[i].ProductId.ToString();
                try
                {
                    Detail.RequiredQuantity = double.Parse(details[i].Quantity.ToString());
                    Detail.GoodQuantity = Detail.RequiredQuantity;
                }
                catch { }
                try
                {
                    Detail.BadQuantity = 0;
                }
                catch { }
                WSI.Details.Add(Detail);
            }
            WSI = BL.callSingleBL(WSI);
            result = WSI.Import.ImportId;
            error = WSI.WsiError;
            if (error == "")
                Session["InvoiceDetails"] = null;
            return Json(new { result = result, error = error });
        }

        public ActionResult ExportForm(string windowId)
        {
            ViewData["ExportDate"] = DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveExport(string exportDate, string customerId, string exportType, string description, string stockId, string invoiceId, string carrierId, string transport)
        {
            string result = "", error = "";
            NMExportsWSI WSI = new NMExportsWSI();
            NMExportsBL BL = new NMExportsBL();
            WSI.Mode = "SAV_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportDate = NMCommon.convertDate(exportDate);
            WSI.Export.ExportTypeId = exportType;
            if (!string.IsNullOrEmpty(customerId))
                WSI.Export.CustomerId = customerId;
            WSI.Export.ExportStatus = NMConstant.EXStatuses.Draft;
            if (!string.IsNullOrEmpty(invoiceId))
                WSI.Export.InvoiceId = invoiceId;
            WSI.Export.DescriptionInVietnamese = description;
            WSI.Export.StockId = stockId;
            WSI.ActionBy = User.Identity.Name;

            var details = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
            WSI.Details = new List<ExportDetails>();
            ExportDetails Detail;
            for (int i = 0; i < details.Count; i++)
            {
                Detail = new ExportDetails();
                try
                {
                    Detail.OrdinalNumber = int.Parse(details[i].OrdinalNumber.ToString());
                }
                catch { }
                Detail.ProductId = details[i].ProductId.ToString();
                Detail.Quantity = double.Parse(details[i].Quantity.ToString());
                WSI.Details.Add(Detail);
            }
            WSI = BL.callSingleBL(WSI);
            result = WSI.Export.ExportId;
            error = WSI.WsiError;
            if (error == "")
                Session["InvoiceDetails"] = null;
            return Json(new { result = result, error = error });
        }

        public ActionResult OrderForm(string windowId)
        {
            ViewData["OrderDate"] = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrder(string orderDate, string deliveryDate, string customerId, string reference, string description,
            string salePerson, string incoterm, string shippingPolicy, string createInvoice, string saleAt)
        {
            string result = "", error = "";
            NMSalesOrdersBL BL = new NMSalesOrdersBL();
            NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();
            WSI.Mode = "SAV_OBJ";
            //WSI.TypeName = "SalesOrders";
            //WSI.Order = new SalesOrders();
            WSI.Order.OrderDate = DateTime.Parse(orderDate);
            try
            {
                WSI.Order.DeliveryDate = DateTime.Parse(deliveryDate);
            }
            catch { }
            WSI.Order.CustomerId = customerId;
            WSI.Order.Reference = reference; 
            WSI.Order.Description = description; 
            WSI.Order.SalesPersonId = salePerson; 
            WSI.Order.Incoterm = incoterm;
            WSI.Order.ShippingPolicy = shippingPolicy; 
            WSI.Order.CreateInvoice = createInvoice;
            WSI.Order.OrderTypeId = NMConstant.SOType.SalesOrder;
            WSI.Order.OrderStatus = NMConstant.SOStatuses.Order;
            if (String.IsNullOrEmpty(saleAt))
                WSI.Order.StockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;   //UserInfoCache.User.Customer.StockId;
            else
                WSI.Order.StockId = saleAt;
            WSI.Details = new List<SalesOrderDetails>();
            SalesOrderDetails Detail;
            var details = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
            for (int i = 0; i < details.Count; i++)
            {
                Detail = new SalesOrderDetails();
                try
                {
                    Detail.Id = int.Parse(details[i].OrdinalNumber.ToString());
                }
                catch { }
                Detail.ProductId = details[i].ProductId.ToString();
                Detail.Quantity = details[i].Quantity;
                Detail.Price = details[i].Price;
                Detail.Discount = details[i].Discount;
                //Detail.DiscountAmount = details[i].DiscountAmount;
                Detail.Tax = details[i].Tax;
                //Detail.TaxAmount = details[i].TaxAmount;
                //Detail.Amount = details[i].Amount;
                //Detail.TotalAmount = details[i].TotalAmount;
                WSI.Details.Add(Detail);
                
            }
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            if (error == "")
            {
                result = WSI.Order.OrderId;
                Session["InvoiceDetails"] = null;
            }
            return Json(new { result = result, error = error });
        }

        public JsonResult CheckExistCustomer(string id)
        {
            bool rs = NMCommon.CheckExistCustomer(id);
            string data = "";
            if (rs == false)
            {
                if (NEXMI.NMCommon.GetSetting("FELIX_CONN") == true)
                {
                    var lstResult = GetInfo("http://felixvn.com/posGetMemberInfoServet", "json={'memberCode': '" + id + "','outletCode': 'OUT0001'}");
                    string status = lstResult["status"].ToString();
                    if (status.CompareTo("success") != 0)
                    {
                        data = "Mã khách hàng này không tồn tại!";
                    }
                    else
                    {
                        Session["FelixMemberInfo"] = lstResult;
                    }
                }
                else
                {
                    data = "Mã khách hàng này không tồn tại!";
                }
            }
            return Json(data);
        }

        #region Felix

        public IDictionary<string, object> GetInfo(string strLinkWeb, string strParam)
        {
            string result;
            IDictionary<string, object> lstResult;
            JavaScriptSerializer json_serializer;

            WebRequest request = (HttpWebRequest)WebRequest.Create(strLinkWeb);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.           
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string strJson = strParam;

                streamWriter.Write(strJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            // Get the response.
            WebResponse response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                // Clean up the streams.
                response.Close();
            }
            json_serializer = new JavaScriptSerializer();
            lstResult = (IDictionary<string, object>)json_serializer.DeserializeObject(result);
            return lstResult;
        }

        public JsonResult FelixMemberInfoCheck(string memberCode)
        {
            var lstResult = GetInfo("http://felixvn.com/posGetMemberInfoServet", "json={'memberCode': '" + memberCode + "','outletCode': 'OUT0001'}");
            
            return Json(lstResult["status"]);
        }

        public ActionResult FelixMemberInfoDialog(string memberCode)
        {
            var lstResult = GetInfo("http://felixvn.com/posGetMemberInfoServet", "json={'memberCode': '" + memberCode + "','outletCode': 'OUT0001'}");
            ViewData["Result"] = lstResult;

            return PartialView();
        }

        public JsonResult FelixVoucherInfoCheck(string voucherCode)
        {
            var lstResult = GetInfo("http://felixvn.com/posGetVoucherInfoServet", "json={'voucherCode': '" + voucherCode + "','outletCode': 'OUT0001'}");

            string value = "0";
            List<SalesInvoiceDetails> OrderDetails = new List<SalesInvoiceDetails>();
                if (Session["InvoiceDetails"] != null)
                {
                    OrderDetails = (List<SalesInvoiceDetails>)Session["InvoiceDetails"];
                }
            if (lstResult["status"].ToString() == "success")
            {
                value = lstResult["value"].ToString();
                Session["VoucherInfo"] = lstResult;
                Session["VoucherCode"] = voucherCode;

                // tính lại giá trị thanh toán
                double itemDiscount = double.Parse(value) / OrderDetails.Count;
                foreach(var Item in OrderDetails)
                {
                    Item.Discount = itemDiscount / Item.Quantity / Item.Price * 100;                    
                }
            }

            return Json(new { status = lstResult["status"], value = value, vat = OrderDetails.Sum(i=>i.TaxAmount), pay = OrderDetails.Sum(i=>i.TotalAmount) });
        }

        public ActionResult FelixVoucherInfoDialog(string voucherCode)
        {
            var lstResult = GetInfo("http://felixvn.com/posGetVoucherInfoServet", "json={'voucherCode': '"+ voucherCode + "','outletCode': 'OUT0001'}");
            ViewData["Result"] = lstResult;

            return PartialView();
        }
        
        public string ConvertToJson(NMSalesInvoicesWSI SaleInvoices)
        {
            string strParam = "json={";
            strParam += "'receiptCode':'3rq3982nksaf9238', ";
            strParam += "'tableNumber':'TABLE', ";
            strParam += "'cashierCode':'CASHIER', ";
            strParam += "'cashierName':'', ";
            strParam += "'invoiceCode':'3rq3982nksaf9238', ";
            strParam += "'checkInTime:'01-12-2013 12:34:56', ";
            strParam += "'checkOutTime':'01-12-2013 12:34:56', ";
            strParam += "'totalOfBill':'100000', ";
            strParam += "'serviceCharge':'10000', ";
            strParam += "'vat':'10000', ";
            strParam += "'refund':'50000', ";
            strParam += "'consumerCode':'0', ";
            strParam += "'memberCode':'VTC9hMGv6j', ";
            strParam += "'cashOfProducts':'100000', ";
            strParam += "'cash':'150000', ";
            strParam += "'discountMoney':'10000', ";
            strParam += "'discountRank':'0', ";
            strParam += "'valueRank':'0', ";
            strParam += "'discount':'10', ";
            strParam += "'totalOfDiscount':'20000', ";
            strParam += "'warranties':[{'productCode':'', 'code':'', 'expired':'', 'wokingTime':'', 'warrantyAddress':''}], ";
            strParam += "'payments':[{'code':'001', 'name':'VND','value':'150000'}], ";
            strParam += "'vouchers':[''], ";
            strParam += "'products':[{'code':'1', 'name':'PRODUCT', 'quantity':'10', 'price':'10000', 'total':'100000'}], ";
            strParam += "'description':'1 - PRODUCT - 10 - 10000 - 100000 \n 1 - PRODUCT - 10 - 10000 - 100000', ";
            strParam += "'outletCode':'OUT0001'}";


            //string description = "";
            //if (SaleInvoices.Details.Count > 0)
            //{
            //    int i = 0;
            //    foreach (var item in SaleInvoices.Details)
            //        description += i + " - " + item.ProductId + " - " + item.Quantity + " - " + item.Price + " - " + item.Amount + "\n";
            //}
            //string strParam = "json={";
            //strParam += "'receiptCode':'"+ SaleInvoices.Invoice.InvoiceId + "',";
            //strParam += "'tableNumber':'',";
            //strParam += "'cashierName':'" + ((NMCustomersWSI)Session["UserInfo"]).Customer.CompanyNameInVietnamese + "',";
            //strParam += "'cashierCode':'" + SaleInvoices.Invoice.SalesPersonId + "',";
            //strParam += "'invoiceCode':'" + SaleInvoices.Invoice.InvoiceId + "',";
            //strParam += "'checkInTime':'" + SaleInvoices.Invoice.InvoiceDate + "',";
            //strParam += "'checkOutTime':'',";
            //strParam += "'totalOfBill':'" + SaleInvoices.Invoice.TotalAmount + "',";
            //strParam += "'serviceCharge':'',";
            //strParam += "'vat':'',";
            //strParam += "'refund':'',";
            //strParam += "'outletCode':'OUT0001',";
            //strParam += "'consumerCode':'0',";
            //strParam += "'memberCode':'" + SaleInvoices.Customer.Code +  "',";
            //strParam += "'vouchers':['" + SaleInvoices.Invoice.VoucherCode + "'],";
            //strParam += "'cashOfProducts':'',";
            //strParam += "'products':'',";
            //strParam += "'payments':'',";
            //strParam += "'warranties':'',";
            //strParam += "'cash':'',";
            //strParam += "'discountMoney':'',";
            //strParam += "'discountRank':'',";
            //strParam += "'valueRank':'',";
            //strParam += "'discount':'',";
            //strParam += "'totalOfDiscount':'',";
            //strParam += "'description':'" + description + "'";
            //strParam += "}";

            return strParam;
        }

        public bool FelixSendInvoiceInfo(string json )
        {
            var lstResult = GetInfo("http://felixvn.com/posSendInvoiceInfoServet", json);
            string status = lstResult["status"].ToString();

            if (status == "success")
                return true;
            else
                return false;
        }

        #endregion

    }
}
