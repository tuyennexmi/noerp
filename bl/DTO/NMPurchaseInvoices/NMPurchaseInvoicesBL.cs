// NMPurchaseInvoicesBL.cs

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
using System.Text;
using NHibernate;
using System.Data;

namespace NEXMI
{
    public class NMPurchaseInvoicesBL
    {
        private readonly ISession Session;

        public NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();

        public NMPurchaseInvoicesBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMPurchaseInvoicesBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMPurchaseInvoicesWSI callSingleBL(NMPurchaseInvoicesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMPurchaseInvoicesWSI> callListBL(NMPurchaseInvoicesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMPurchaseInvoicesWSI>();
            }
        }

        private NMPurchaseInvoicesWSI SelectObject(NMPurchaseInvoicesWSI wsi)
        {
            try
            {
                PurchaseInvoicesAccesser Acceser = new PurchaseInvoicesAccesser(Session);
                PurchaseInvoices obj;
                obj = Acceser.GetAllPurchaseinvoicesByID(wsi.Invoice.InvoiceId, false);
                if (obj != null)
                {
                    CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                    PurchaseInvoiceDetailsAccesser detailAccesser = new PurchaseInvoiceDetailsAccesser(Session);

                    wsi.Invoice = obj;
                    wsi.Details = detailAccesser.GetAllPurchaseinvoicedetailsByInvoiceId(obj.InvoiceId, true).ToList();  //.DetailsList.Cast<PurchaseInvoiceDetails>().ToList();
                    
                    wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                    wsi.Payments = obj.PaymentList.Cast<Payments>().ToList();
                    wsi.Receipts = obj.ReceiptList.Cast<Receipts>().ToList();
                    Session.Evict(obj);
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMPurchaseInvoicesWSI SaveObject(NMPurchaseInvoicesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PurchaseInvoicesAccesser Accesser = new PurchaseInvoicesAccesser(Session);
                PurchaseInvoiceDetailsAccesser DetailAccesser = new PurchaseInvoiceDetailsAccesser(Session);
                List<PurchaseInvoiceDetails> DetailOlds = null;
                PurchaseInvoices obj = Accesser.GetAllPurchaseinvoicesByID(wsi.Invoice.InvoiceId, false);
                wsi.Invoice.Discount = wsi.Details.Sum(i => i.DiscountAmount);
                wsi.Invoice.Tax = wsi.Details.Sum(i => i.TaxAmount);
                wsi.Invoice.Amount = wsi.Details.Sum(i => i.Amount);
                wsi.Invoice.TotalAmount = wsi.Details.Sum(i => i.TotalAmount);  // +wsi.Invoice.ShipCost + wsi.Invoice.OtherCost;
                PurchaseInvoiceDetails detail;
                if (obj != null)
                {
                    DetailOlds = obj.DetailsList.Cast<PurchaseInvoiceDetails>().ToList();
                    Session.Evict(obj);

                    wsi.Invoice.CreatedDate = obj.CreatedDate;
                    wsi.Invoice.CreatedBy = obj.CreatedBy;
                    wsi.Invoice.ModifiedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedDate = DateTime.Now;

                    if (wsi.Invoice.InvoiceStatus != obj.InvoiceStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_PI_DATE_BY_ACTION"))
                            wsi.Invoice.InvoiceDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Invoice.InvoiceId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    Accesser.UpdatePurchaseinvoices(wsi.Invoice);
                }
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Invoice.InvoiceId = AutomaticValueAccesser.AutoGenerateId("PurchaseInvoices");
                    wsi.Invoice.InvoiceStatus = NMConstant.PIStatuses.Draft;

                    wsi.Invoice.CreatedDate = DateTime.Now;
                    wsi.Invoice.CreatedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedDate = DateTime.Now;
                    
                    Accesser.InsertPurchaseinvoices(wsi.Invoice);
                    NMMessagesBL.SaveMessage(Session, wsi.Invoice.InvoiceId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                foreach (PurchaseInvoiceDetails Item in wsi.Details)
                {
                    Item.InvoiceId = wsi.Invoice.InvoiceId;
                    detail = DetailAccesser.GetAllPurchaseinvoicedetailsByID(Item.OrdinalNumber.ToString(), true);
                    if (detail != null)
                    {
                        Item.CreatedBy = detail.CreatedBy;
                        Item.CreatedDate = detail.CreatedDate;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.UpdatePurchaseinvoicedetails(Item);
                    }
                    else
                    {
                        Item.CreatedBy = wsi.ActionBy;
                        Item.CreatedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.InsertPurchaseinvoicedetails(Item);
                    }
                }
                if (DetailOlds != null)
                {
                    bool flag = true;
                    foreach (PurchaseInvoiceDetails Old in DetailOlds)
                    {
                        flag = true;
                        foreach (PurchaseInvoiceDetails New in wsi.Details)
                        {
                            if (Old.OrdinalNumber == New.OrdinalNumber)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeletePurchaseinvoicedetails(Old);
                        }
                    }
                }
                
                if (wsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Debit || wsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Paid)
                {
                    if (NMCommon.GetSetting(NMConstant.Settings.IMPORT_ON_PI))
                    {
                        ImportFromInvoice(wsi);
                    }
                    
                    if (NMCommon.GetSetting(NMConstant.Settings.PAYMENT_ON_PINVOICE))
                    {
                        PaymentFromInvoice(wsi);
                    }
                }
                
                if (wsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Debit)
                {
                    // ghi nhận đã trả tiền nợ
                    this.WriteDebt(wsi);
                    //cập nhật trạng thái PO liên quan
                    if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                    {
                        NMCommon.UpdateObjectStatus(Session, "PurchaseOrders", wsi.Invoice.Reference, NMConstant.POStatuses.Invoice, null, wsi.ActionBy, "Trạng thái: => Đã ghi sổ. Số hóa đơn: " + wsi.Invoice.InvoiceId + " \n");
                    }
                }

                if (wsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Paid)
                {
                    // ghi nhận đã trả tiền nợ
                    this.WriteDebt(wsi);
                    //cập nhật trạng thái PO liên quan
                    if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                    {
                        NMCommon.UpdateObjectStatus(Session, "PurchaseOrders", wsi.Invoice.Reference, NMConstant.POStatuses.Invoice, null, wsi.ActionBy, "Trạng thái: => Đã thanh toán. Số hóa đơn: " + wsi.Invoice.InvoiceId + " \n");
                    }
                }

                //
                if(wsi.Commit)
                    tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMPurchaseInvoicesWSI DeleteObject(NMPurchaseInvoicesWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                PurchaseInvoicesAccesser Accesser = new PurchaseInvoicesAccesser(Session);
                PurchaseInvoices obj = Accesser.GetAllPurchaseinvoicesByID(wsi.Invoice.InvoiceId, true);
                if (obj != null)
                {
                    Accesser.DeletePurchaseinvoices(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không được xóa hóa đơn này.";
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private List<NMPurchaseInvoicesWSI> SearchObject(NMPurchaseInvoicesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Invoice != null)
            {
                if (!string.IsNullOrEmpty(wsi.Invoice.InvoiceBatchId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceBatchId = :InvoiceBatchId";
                    ListCriteria.Add("InvoiceBatchId", wsi.Invoice.InvoiceBatchId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.SupplierId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.SupplierId = :SupplierId";
                    ListCriteria.Add("SupplierId", wsi.Invoice.SupplierId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.InvoiceTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceTypeId = :InvoiceTypeId";
                    ListCriteria.Add("InvoiceTypeId", wsi.Invoice.InvoiceTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.InvoiceStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceStatus = :InvoiceStatus";
                    ListCriteria.Add("InvoiceStatus", wsi.Invoice.InvoiceStatus);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Invoice.Reference);
                }

                if (!string.IsNullOrEmpty(wsi.Invoice.DescriptionInVietnamese))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.DescriptionInVietnamese LIKE :DescriptionInVietnamese";
                    ListCriteria.Add("DescriptionInVietnamese", wsi.Invoice.DescriptionInVietnamese);
                }
            }
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {   
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.InvoiceDate >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.InvoiceDate <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.DescriptionInVietnamese LIKE :Keyword OR O.Reference LIKE :Keyword OR O.InvoiceId LIKE :Keyword OR O.InvoiceBatchId LIKE :Keyword OR O.SupplierId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.InvoiceId) FROM PurchaseInvoices AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.InvoiceDate DESC, O.InvoiceId DESC";
            }
            string strCmd = "SELECT O FROM PurchaseInvoices AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMPurchaseInvoicesWSI> ListWSI = new List<NMPurchaseInvoicesWSI>();
            PurchaseInvoicesAccesser Accesser = new PurchaseInvoicesAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            PaymentsAccesser PayAccesser = new PaymentsAccesser(this.Session);
            IList<PurchaseInvoices> objs;
            int totalRows = Accesser.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetPurchaseinvoicesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetPurchaseinvoicesByQuery(query, false);
            }
            foreach (PurchaseInvoices obj in objs)
            {
                wsi = new NMPurchaseInvoicesWSI();
                wsi.Invoice = obj;
                //wsi.Details = obj.PurchaseInvoiceDetailsList.Cast<PurchaseInvoiceDetails>().ToList();
                wsi.Payments = obj.PaymentList.Cast<Payments>().ToList();
                wsi.Receipts = obj.ReceiptList.Cast<Receipts>().ToList();
                wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                wsi.TotalRows = totalRows;
                wsi.WsiError = "";
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }

        //private void PriceCalculation(NMPurchaseInvoicesWSI wsi)
        //{
        //    PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
        //    ProductInStocksAccesser PISAccesser = new ProductInStocksAccesser(Session);
        //    PricesForSalesInvoice PRS = new PricesForSalesInvoice();
        //    Double oldPrice = 0, newPrice = 0;
        //    //lấy giá xuất kho
        //    foreach (PurchaseInvoiceDetails Item in wsi.Details)
        //    {
        //        try
        //        {
        //            oldPrice = PriceAccesser.GetAllPricesforExportCloset(Item.ProductId, wsi.Invoice.StockId, true).Price;
        //            if (oldPrice != Item.Price) {
        //                double quantity = PISAccesser.GetAllProductinstocksByStockIdAndProductId(wsi.Invoice.StockId, Item.ProductId, true).GoodProductInStock;
        //                newPrice = (quantity * oldPrice + Item.Quantity * Item.Price) / (quantity + Item.Quantity);
        //                PRS.Price = newPrice;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            PRS.Price = Item.Price;
        //        }

        //        PRS.IsCost = true;                
        //        PRS.ProductId = Item.ProductId;
        //        PRS.DateOfPrice = DateTime.Today;
        //        PRS.StockId = wsi.Invoice.StockId;
        //        PRS.CurrencyId = "VND";
        //        PRS.PriceDiscontinued = false;
        //        PRS.CreatedBy = wsi.ActionBy;
        //        PRS.CreatedDate = DateTime.Now;
        //    }
        //}

        private void ImportFromInvoice(NMPurchaseInvoicesWSI wsi)
        {
            //kiểm tra đã xuất kho cho đơn hàng hay chưa
            NMImportsBL importBL = new NMImportsBL();
            NMImportsWSI importWSI = new NMImportsWSI();
            importWSI.Import = new Imports();
            importWSI.Mode = "SRC_OBJ";
            importWSI.Import.Reference = wsi.Invoice.Reference;
            List<NMImportsWSI> impList = importBL.callListBL(importWSI);
            if (impList.Count <= 0)
            {
                ImportsAccesser Accesser = new ImportsAccesser(Session);
                ImportDetailsAccesser DetailAccesser = new ImportDetailsAccesser(Session);
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                Imports Import = new Imports();
                Import.ImportId = AutomaticValueAccesser.AutoGenerateId("Imports");
                Import.ImportDate = DateTime.Today;
                Import.ImportTypeId = NMConstant.ImportType.Direct;
                Import.InvoiceTypeId = wsi.Invoice.InvoiceTypeId;
                if (!string.IsNullOrEmpty(wsi.Invoice.SupplierId))
                    Import.SupplierId = wsi.Invoice.SupplierId;
                Import.ImportStatus = NMConstant.IMStatuses.Imported;
                Import.InvoiceId = wsi.Invoice.InvoiceId;
                Import.StockId = wsi.Invoice.StockId;
                Import.Reference = wsi.Invoice.Reference;
                
                Import.CreatedDate = DateTime.Now;
                Import.CreatedBy = wsi.ActionBy;
                Import.ModifiedDate = DateTime.Now;
                Import.ModifiedBy = wsi.ActionBy;

                Import.Amount = wsi.Details.Sum(i => i.Amount);

                Accesser.InsertImports(Import);
                ImportDetails Detail;
                foreach (PurchaseInvoiceDetails Item in wsi.Details)
                {
                    Detail = new ImportDetails();
                    Detail.ProductId = Item.ProductId;
                    Detail.RequiredQuantity = Item.Quantity;
                    Detail.GoodQuantity = Item.Quantity;
                    Detail.StockId = Import.StockId;
                    Detail.ImportId = Import.ImportId;
                    
                    Detail.CreatedDate = DateTime.Now;
                    Detail.CreatedBy = wsi.ActionBy;
                    Detail.ModifiedDate = DateTime.Now;
                    Detail.ModifiedBy = wsi.ActionBy;

                    DetailAccesser.InsertImportdetails(Detail);
                    if (Import.ImportStatus == NMConstant.IMStatuses.Imported) NMCommon.UpdateStock(Session, null, Detail);
                }
                NMMessagesBL.SaveMessage(Session, Import.ImportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                //cập nhật trạng thái PO liên quan
                if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                {
                    NMCommon.UpdateObjectStatus(Session, "PurchaseOrders", wsi.Invoice.Reference, NMConstant.POStatuses.Invoice, null, wsi.ActionBy, "Trạng thái: => Đã nhập kho.\n");
                }
            }
        }

        private void PaymentFromInvoice(NMPurchaseInvoicesWSI wsi)
        {
            PaymentsAccesser PaymentsAccesser = new PaymentsAccesser(Session);
            AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
            Payments Payment = new Payments();
            Payment.PaymentId = AutomaticValueAccesser.AutoGenerateId("Payments");

            Payment.InvoiceId = wsi.Invoice.InvoiceId;
            Payment.PaymentDate = wsi.Invoice.InvoiceDate;
            Payment.Amount = wsi.Invoice.TotalAmount;
            Payment.CurrencyId = "VND";
            Payment.ExchangeRate = 1;
            Payment.PaymentAmount = Payment.Amount / Payment.ExchangeRate;
            Payment.PaymentTypeId = NMConstant.PaymentType.Purchase;
            Payment.PaymentStatus = NMConstant.PaymentStatuses.Draft;


            Payment.CreatedDate = DateTime.Now;
            Payment.CreatedBy = wsi.ActionBy;
            Payment.ModifiedDate = DateTime.Now;
            Payment.ModifiedBy = wsi.ActionBy;

            PaymentsAccesser.InsertPayments(Payment);
            NMMessagesBL.SaveMessage(Session, Payment.PaymentId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
        }

        private void WriteDebt(NMPurchaseInvoicesWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsByPurchaseInvoice(wsi.Invoice.InvoiceId);

            //MGJ.IssueId = wsi.Invoice.InvoiceId;
            MGJ.PIID = wsi.Invoice.InvoiceId;
            MGJ.IssueDate = wsi.Invoice.InvoiceDate;
            MGJ.PartnerId = wsi.Invoice.SupplierId;
            MGJ.ActionBy = wsi.Invoice.BuyerId;
            MGJ.StockId = wsi.Invoice.StockId;

            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;

            //ghi nợ             // tiền hàng
            
            //ghi có    => tiền hàng + thue                        
            MGJ.AccountId = "331";
            MGJ.DebitAmount = 0;
            MGJ.CreditAmount = wsi.Invoice.Amount + wsi.Invoice.Tax - wsi.Invoice.Discount;
            MGJ.Descriptions = "Phải trả cho người bán";
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);            

            // có thuế VAT
            if (wsi.Invoice.Tax > 0)
            {
                // ghi nợ => thuế VAT được nhận lại
                MGJ = new MonthlyGeneralJournals();
                MGJ.PIID = wsi.Invoice.InvoiceId;
                MGJ.IssueDate = wsi.Invoice.InvoiceDate;
                MGJ.PartnerId = wsi.Invoice.SupplierId;
                MGJ.ActionBy = wsi.Invoice.BuyerId;
                MGJ.StockId = wsi.Invoice.StockId;

                MGJ.CreatedBy = wsi.ActionBy;
                MGJ.ModifiedBy = wsi.ActionBy;

                MGJ.CurrencyId = "VND";
                MGJ.ExchangeRate = 1;
                MGJ.IsBegin = false;
                MGJ.AccountId = "1331";
                MGJ.DebitAmount = wsi.Invoice.Tax;
                MGJ.CreditAmount = 0;
                MGJ.Descriptions = "Thuế VAT được khấu trừ của HH/DV";
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            return;
        }

        private void WriteCash(NMPurchaseInvoicesWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            MGJ.IssueId = wsi.Invoice.InvoiceId;
            MGJ.IssueDate = wsi.Invoice.InvoiceDate;
            MGJ.PartnerId = wsi.Invoice.SupplierId;
            
            MGJ.CreatedDate = DateTime.Now;
            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedDate = DateTime.Now;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            //ghi nợ
            MGJ.AccountId = wsi.Invoice.InvoiceTypeId;  // "1561";
            MGJ.DebitAmount = wsi.Invoice.Amount;
            MGJ.CreditAmount = 0;
            MGJ.Descriptions = "nguyên giá mua hàng hóa";
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            //ghi có
            //MGJ.AccountId = "1111";
            //MGJ.DebitAmount = 0;
            //MGJ.CreditAmount = wsi.Invoice.Amount;
            //MGJ.Descriptions = "trả tiền mua hàng hóa";
            //mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            //ghi nợ => thue VAT
            if (wsi.Invoice.Tax > 0)
            {
                MGJ.AccountId = "1331";
                MGJ.DebitAmount = wsi.Invoice.Tax;
                MGJ.CreditAmount = 0;
                MGJ.Descriptions = "thuế VAT mua hàng hóa";
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
                
                //ghi có
                //MGJ.AccountId = "1111";
                //MGJ.DebitAmount = 0;
                //MGJ.CreditAmount = wsi.Invoice.Tax;
                //MGJ.Descriptions = "trả tiền thuế VAT được khấu trừ của hàng hóa mua vào";
                //mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            return;
        }

        //public void FromPurchaseOrder(NMPurchaseOrdersWSI POWSI)
        //{
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.Invoice.InvoiceId = "";

        //    if (NMCommon.GetSetting("USE_PODATE_FOR_PI_DATE"))
        //        WSI.Invoice.InvoiceDate = DateTime.Today;
        //    else
        //        WSI.Invoice.InvoiceDate = POWSI.Order.OrderDate;

        //    WSI.Invoice.SupplierId = POWSI.Order.SupplierId;
        //    WSI.Invoice.BuyerId = POWSI.Order.BuyerId;
        //    WSI.Invoice.Reference = POWSI.Order.Id;
        //    WSI.Invoice.SupplierReference = POWSI.Order.Reference;

        //    WSI.Invoice.StockId = POWSI.Order.ImportStockId;
            
        //    WSI.Invoice.CurrencyId = "VND";
        //    WSI.Invoice.ExchangeRate = 1;
        //    WSI.Invoice.DescriptionInVietnamese = POWSI.Order.Description;

        //    WSI.Invoice.InvoiceStatus = NMConstant.PIStatuses.Draft;
        //    WSI.Invoice.InvoiceTypeId = POWSI.Order.InvoiceTypeId;

        //    PurchaseInvoiceDetails detail;
        //    WSI.Details = new List<PurchaseInvoiceDetails>();
        //    foreach (PurchaseOrderDetails Item in POWSI.Details)
        //    {
        //        detail = new PurchaseInvoiceDetails();
        //        detail.CopyFromPODetail(Item);

        //        WSI.Details.Add(detail);
        //    }

        //    WSI.Invoice.Amount = POWSI.Order.Amount;
        //    WSI.Invoice.Discount = POWSI.Order.Discount;
        //    WSI.Invoice.Tax = POWSI.Order.Tax;
        //    WSI.Invoice.TotalAmount = POWSI.Order.TotalAmount;

        //    return;
        //}

        //public string FromPurchaseOrder(string poId)
        //{
        //    NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
        //    NMPurchaseOrdersWSI PO = new NMPurchaseOrdersWSI();
        //    PO.Mode = "SEL_OBJ";
        //    PO.Order.Id = poId;
        //    PO = POBL.callSingleBL(PO);

        //    if (PO.WsiError == string.Empty)
        //    {
        //        this.FromPurchaseOrder(PO);
        //    }

        //    return PO.WsiError;
        //}


        #region Receipt

        public NMPaymentsWSI CreatePayment()
        {
            NMPaymentsWSI payWsi = new NMPaymentsWSI();

            payWsi.Payment.SupplierId = this.WSI.Invoice.SupplierId;
            payWsi.Payment.StockId = this.WSI.Invoice.StockId;
            payWsi.Payment.InvoiceId = this.WSI.Invoice.InvoiceId;
            payWsi.Payment.PaymentDate = this.WSI.Invoice.InvoiceDate;
            payWsi.Payment.Amount = this.WSI.Invoice.TotalAmount;
            //payWsi.Payment.PaymentMethodId = this.WSI.Invoice

            payWsi.Payment.CurrencyId = "VND";
            payWsi.Payment.ExchangeRate = 1;
            payWsi.Payment.PaymentAmount = payWsi.Payment.Amount / payWsi.Payment.ExchangeRate;
            payWsi.Payment.PaymentTypeId = NMConstant.PaymentType.Purchase;
            payWsi.Payment.PaymentStatus = NMConstant.PaymentStatuses.Draft;

            return payWsi;
        }

        public NMPaymentsWSI CreatePayment(string piId)
        {
            NMPaymentsWSI payWsi = new NMPaymentsWSI();

            NMPurchaseInvoicesBL piBL = new NMPurchaseInvoicesBL();
            NMPurchaseInvoicesWSI piWSI = new NMPurchaseInvoicesWSI();
            piWSI.Mode = "SEL_OBJ";
            piWSI.Invoice.InvoiceId = piId;
            piWSI = piBL.callSingleBL(piWSI);

            if (piWSI.WsiError == string.Empty)
            {
                payWsi = this.CreatePayment();
                payWsi.WsiError = piWSI.WsiError;
            }
            else
            {
                payWsi.WsiError = piWSI.WsiError;
            }

            return payWsi;
        }

        #endregion

    }
}
