// NMSalesInvoicesBL.cs

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
    public class NMSalesInvoicesBL
    {
        private readonly ISession Session;
        public NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();

        public NMSalesInvoicesBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMSalesInvoicesBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMSalesInvoicesWSI callSingleBL(NMSalesInvoicesWSI wsi)
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

        public List<NMSalesInvoicesWSI> callListBL(NMSalesInvoicesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMSalesInvoicesWSI>();
            }
        }

        public NMSalesInvoicesWSI SelectObject(NMSalesInvoicesWSI wsi)
        {
            try
            {
                SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
                SalesInvoices obj;
                obj = Accesser.GetAllSalesinvoicesByID(wsi.Invoice.InvoiceId, false);
                if (obj != null)
                {
                    SalesInvoiceDetailsAccesser detailAccesser = new SalesInvoiceDetailsAccesser(Session);
                    CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                    StatusesAccesser StatusAccesser = new StatusesAccesser(Session);

                    wsi.Invoice = obj;
                    wsi.Details = detailAccesser.GetAllSalesinvoicedetailsByInvoiceId(obj.InvoiceId, true).ToList();    // .DetailsList.Cast<SalesInvoiceDetails>().ToList();

                    wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                    wsi.SalesPerson = CustomerAccesser.GetAllCustomersByID(obj.SalesPersonId, true);
                    wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                    wsi.ApprovalBy = CustomerAccesser.GetAllCustomersByID(obj.ApprovalBy, true);
                    //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.InvoiceStatus, true);
                    wsi.Receipts = obj.ReceiptList.Cast<Receipts>().ToList();
                    wsi.Payments = obj.PaymentList.Cast<Payments>().ToList();
                    wsi.WsiError = "";
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

        public NMSalesInvoicesWSI SaveObject(NMSalesInvoicesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
                SalesInvoiceDetailsAccesser DetailAccesser = new SalesInvoiceDetailsAccesser(Session);
                List<SalesInvoiceDetails> OldDetails = null;
                SalesInvoices obj = Accesser.GetAllSalesinvoicesByID(wsi.Invoice.InvoiceId, false);
                wsi.Invoice.Discount = wsi.Details.Sum(i => i.DiscountAmount);
                wsi.Invoice.Tax = wsi.Details.Sum(i => i.TaxAmount);
                wsi.Invoice.Amount = wsi.Details.Sum(i => i.Amount);
                wsi.Invoice.TotalAmount = wsi.Details.Sum(i => i.TotalAmount);  // +wsi.Invoice.ShipCost + wsi.Invoice.OtherCost;
                SalesInvoiceDetails detail;
                #region Update master
                if (obj != null)
                {
                    OldDetails = obj.DetailsList.Cast<SalesInvoiceDetails>().ToList();
                    Session.Evict(obj);
                    wsi.Invoice.CreatedDate = obj.CreatedDate;
                    wsi.Invoice.CreatedBy = obj.CreatedBy;
                    wsi.Invoice.ModifiedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedDate = DateTime.Now;
                    if (wsi.Invoice.InvoiceStatus != obj.InvoiceStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_SI_DATE_BY_ACTION"))
                            wsi.Invoice.InvoiceDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Invoice.InvoiceId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    Accesser.UpdateSalesinvoices(wsi.Invoice);
                }
                #endregion
                
                #region Tao moi master
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Invoice.InvoiceId = AutomaticValueAccesser.AutoGenerateId("SalesInvoices");
                    
                    wsi.Invoice.CreatedDate = DateTime.Now;
                    wsi.Invoice.CreatedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedBy = wsi.ActionBy;
                    wsi.Invoice.ModifiedDate = DateTime.Now;
                    
                    Accesser.InsertSalesinvoices(wsi.Invoice);

                    NMMessagesBL.SaveMessage(Session, wsi.Invoice.InvoiceId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                #endregion

                #region Update detail
                foreach (SalesInvoiceDetails Item in wsi.Details)
                {
                    Item.InvoiceId = wsi.Invoice.InvoiceId;
                    detail = DetailAccesser.GetAllSalesinvoicedetailsByID(Item.OrdinalNumber.ToString(), true);
                    if (detail != null)
                    {
                        Item.CreatedBy = detail.CreatedBy;
                        Item.CreatedDate = detail.CreatedDate;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.UpdateSalesinvoicedetails(Item);
                    }
                    else
                    {
                        Item.CreatedBy = wsi.ActionBy;
                        Item.CreatedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.InsertSalesinvoicedetails(Item);
                    }
                }
                if (OldDetails != null)
                {
                    bool flag = true;
                    foreach (SalesInvoiceDetails Old in OldDetails)
                    {
                        flag = true;
                        foreach (SalesInvoiceDetails New in wsi.Details)
                        {
                            if (Old.OrdinalNumber == New.OrdinalNumber)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeleteSalesinvoicedetails(Old);
                        }
                    }
                }
                #endregion

                if (wsi.Invoice.InvoiceStatus == NMConstant.SIStatuses.Debit || wsi.Invoice.InvoiceStatus == NMConstant.SIStatuses.Paid)
                {
                    if (NMCommon.GetSetting(NMConstant.Settings.EXPORT_ON_INVOICE))
                    {
                        ExportFromInvoice(wsi);
                    }
                    //if (NMCommon.GetSetting(NMConstant.Settings.RECEIPT_ON_INVOICE))
                    //ReceiptFromInvoice(wsi);
                }
                if(wsi.Invoice.InvoiceStatus == NMConstant.SIStatuses.Debit)
                {
                    //ghi nhận nợ doanh thu 
                    this.WriteDebt(wsi);
                    //cập nhật trạng thái SO liên quan
                    if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                    {
                        SalesOrdersAccesser salesOrdersAccesser = new SalesOrdersAccesser(Session);
                        SalesOrders SO = salesOrdersAccesser.GetAllOrdersByID(wsi.Invoice.Reference, true);
                        if (SO != null)
                        {
                            if (SO.OrderGroup == NMConstant.SOrderGroups.Lease)
                            {
                                if (SO.OrderStatus != NMConstant.LOStatuses.Maintain)
                                    NMCommon.UpdateObjectStatus(Session, "SalesOrders", wsi.Invoice.Reference, NMConstant.LOStatuses.Maintain, null, wsi.ActionBy, "Trạng thái: => Đã ghi sổ. Số hóa đơn: " + wsi.Invoice.InvoiceId + "\n");
                            }
                            else
                            {
                                NMCommon.UpdateObjectStatus(Session, "SalesOrders", wsi.Invoice.Reference, NMConstant.SOStatuses.Invoice, null, wsi.ActionBy, "Trạng thái: => Đã ghi sổ. Số hóa đơn: " + wsi.Invoice.InvoiceId + "\n");
                            }
                        }
                    }
                }
                else if (wsi.Invoice.InvoiceStatus == NMConstant.SIStatuses.Paid)
                {
                    //ghi nhận nợ doanh thu 
                    this.WriteDebt(wsi);
                    //cập nhật trạng thái SO liên quan
                    if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                    {
                        SalesOrdersAccesser salesOrdersAccesser = new SalesOrdersAccesser(Session);
                        SalesOrders SO = salesOrdersAccesser.GetAllOrdersByID(wsi.Invoice.Reference, true);

                        if (SO.OrderGroup == NMConstant.SOrderGroups.Lease)
                        {
                            if (SO.OrderStatus != NMConstant.LOStatuses.Maintain)
                                NMCommon.UpdateObjectStatus(Session, "SalesOrders", wsi.Invoice.Reference, NMConstant.LOStatuses.Maintain, null, wsi.ActionBy, "Trạng thái: => Đã thanh toán. Số hóa đơn: " + wsi.Invoice.InvoiceId + "\n");
                        }
                        else
                        {
                            NMCommon.UpdateObjectStatus(Session, "SalesOrders", wsi.Invoice.Reference, NMConstant.SOStatuses.Invoice, null, wsi.ActionBy, "Trạng thái: => Đã thanh toán. Số hóa đơn: " + wsi.Invoice.InvoiceId + "\n");
                        }
                        
                    }
                }

                // thuc hien
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

        public NMSalesInvoicesWSI DeleteObject(NMSalesInvoicesWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
                SalesInvoices obj = Accesser.GetAllSalesinvoicesByID(wsi.Invoice.InvoiceId, true);
                if (obj != null)
                {
                    Accesser.DeleteSalesinvoices(obj);
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

        public List<NMSalesInvoicesWSI> SearchObject(NMSalesInvoicesWSI wsi)
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
                if (!string.IsNullOrEmpty(wsi.Invoice.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Invoice.CustomerId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.InvoiceTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceTypeId = :InvoiceTypeId";
                    ListCriteria.Add("InvoiceTypeId", wsi.Invoice.InvoiceTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.StockId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StockId = :StockId";
                    ListCriteria.Add("StockId", wsi.Invoice.StockId);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Invoice.Reference);
                }
                if (!string.IsNullOrEmpty(wsi.Invoice.InvoiceStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceStatus = :InvoiceStatus";
                    ListCriteria.Add("InvoiceStatus", wsi.Invoice.InvoiceStatus);
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
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Reference LIKE :Keyword OR O.DescriptionInVietnamese LIKE :Keyword OR O.InvoiceId LIKE :Keyword OR O.Reference LIKE :Keyword OR O.InvoiceBatchId LIKE :Keyword OR O.CustomerId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.InvoiceId) FROM SalesInvoices AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.InvoiceDate DESC, O.InvoiceId DESC";
            }
            string strCmd = "SELECT O FROM SalesInvoices AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMSalesInvoicesWSI> ListWSI = new List<NMSalesInvoicesWSI>();
            SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            IList<SalesInvoices> objs;
            int totalRows = Accesser.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetSalesinvoicesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetSalesinvoicesByQuery(query, false);
            }
            
            bool IsGetDetails = wsi.GetDetails;

            foreach (SalesInvoices obj in objs)
            {
                wsi = new NMSalesInvoicesWSI();
                wsi.Invoice = obj;
                if (IsGetDetails)
                {
                    wsi.Details = obj.DetailsList.Cast<SalesInvoiceDetails>().ToList();                    
                    wsi.SalesPerson = CustomerAccesser.GetAllCustomersByID(obj.SalesPersonId, true);
                    wsi.ApprovalBy = CustomerAccesser.GetAllCustomersByID(obj.ApprovalBy, true);
                }
                wsi.Receipts = obj.ReceiptList.Cast<Receipts>().ToList();
                wsi.Payments = obj.PaymentList.Cast<Payments>().ToList();
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.InvoiceStatus, true);
                
                wsi.TotalRows = totalRows;
                wsi.WsiError = "";
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }

        public void ExportFromInvoice(NMSalesInvoicesWSI wsi)
        {
            //kiểm tra đã xuất kho cho đơn hàng hay chưa

            if (!String.IsNullOrEmpty(wsi.Invoice.Reference))
            {
                NMExportsBL ExportBL = new NMExportsBL();
                NMExportsWSI ExportWSI = new NMExportsWSI();
                ExportWSI.Export = new Exports();
                ExportWSI.Mode = "SRC_OBJ";
                ExportWSI.Export.Reference = wsi.Invoice.Reference;
                List<NMExportsWSI> expList = ExportBL.callListBL(ExportWSI);

                if (expList.Count >= 0)
                {
                    return;
                }
            }

            ExportsAccesser Accesser = new ExportsAccesser(Session);
            ExportDetailsAccesser DetailAccesser = new ExportDetailsAccesser(Session);
            AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
            PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
            Exports Export = new Exports();
            Export.ExportId = AutomaticValueAccesser.AutoGenerateId("Exports");
            Export.ExportDate = DateTime.Now;
            Export.ExportTypeId = NMConstant.ExportType.ForCustomers;
            if (!string.IsNullOrEmpty(wsi.Invoice.CustomerId))
                Export.CustomerId = wsi.Invoice.CustomerId;
            Export.ExportStatus = NMConstant.EXStatuses.Draft;
            Export.InvoiceId = wsi.Invoice.InvoiceId;
            Export.Reference = wsi.Invoice.Reference;
            Export.StockId = wsi.Invoice.StockId;

            Export.CreatedDate = DateTime.Now;
            Export.CreatedBy = wsi.ActionBy;
            Export.ModifiedDate = DateTime.Now;
            Export.ModifiedBy = wsi.ActionBy;

            //thiếu phương thức giao hàng
            //Export.DeliveryMethod =??
            Accesser.InsertExports(Export);
            ExportDetails Detail;
            foreach (SalesInvoiceDetails Item in wsi.Details)
            {
                Detail = new ExportDetails();
                Detail.ProductId = Item.ProductId;
                Detail.RequiredQuantity = Item.Quantity;
                Detail.Quantity = Item.Quantity;
                //lấy giá xuất kho
                //Detail.Price = PriceAccesser.GetAllPricesforExportCloset(Detail.ProductId, wsi.Invoice.StockId, true).Price;

                Detail.StockId = Export.StockId;
                Detail.ExportId = Export.ExportId;

                Detail.CreatedDate = DateTime.Now;
                Detail.CreatedBy = wsi.ActionBy;
                Detail.ModifiedDate = DateTime.Now;
                Detail.ModifiedBy = wsi.ActionBy;

                DetailAccesser.InsertExportdetails(Detail);
                if (Export.ExportStatus == NMConstant.EXStatuses.Delivered) NMCommon.UpdateStock(Session, null, Detail, NMConstant.ExportType.ForCustomers);
            }
            NMMessagesBL.SaveMessage(Session, Export.ExportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

        }

        public void ReceiptFromInvoice(NMSalesInvoicesWSI wsi)
        {
            ReceiptsAccesser ReceiptsAccesser = new ReceiptsAccesser(Session);
            AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
            Receipts Receipt = new Receipts();
            Receipt.ReceiptId = AutomaticValueAccesser.AutoGenerateId("Receipts");

            Receipt.CustomerId = wsi.Invoice.CustomerId;
            Receipt.StockId = wsi.Invoice.StockId;
            Receipt.InvoiceId = wsi.Invoice.InvoiceId;
            Receipt.ReceiptDate = wsi.Invoice.InvoiceDate;
            Receipt.Amount = wsi.Invoice.DetailsList.Sum(i=>i.TotalAmount);
            Receipt.CurrencyId = "VND";
            Receipt.ExchangeRate = 1;
            Receipt.ReceiptAmount = Receipt.Amount / Receipt.ExchangeRate;
            Receipt.ReceiptTypeId = NMConstant.ReceiptType.Sales;
            Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;
            Receipt.PaymentMethodId = NMConstant.PaymentMethod.Cash;
            
            Receipt.CreatedDate = DateTime.Now;
            Receipt.CreatedBy = wsi.ActionBy;
            Receipt.ModifiedDate = DateTime.Now;
            Receipt.ModifiedBy = wsi.ActionBy;

            ReceiptsAccesser.InsertReceipts(Receipt);
            NMMessagesBL.SaveMessage(Session, Receipt.ReceiptId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
        }

        //public void FromSalesOrder(NMSalesOrdersWSI SOWSI)
        //{
        //    this.WSI.Invoice.CopyFromSO(SOWSI.Order);

        //    if (NMCommon.GetSetting("USE_SODATE_FOR_SI_DATE"))
        //        WSI.Invoice.InvoiceDate = SOWSI.Order.OrderDate;

        //    WSI.Details = new List<SalesInvoiceDetails>();
        //    SalesInvoiceDetails SIDetail;
        //    foreach (SalesOrderDetails Item in SOWSI.Details)
        //    {
        //        SIDetail = new SalesInvoiceDetails();
        //        SIDetail.CopyFromSODetail(Item);

        //        WSI.Details.Add(SIDetail);
        //    }

        //    WSI.Invoice.Amount = WSI.Details.Sum(i => i.Amount);
        //    WSI.Invoice.Discount = WSI.Details.Sum(i => i.DiscountAmount);
        //    WSI.Invoice.Tax = WSI.Details.Sum(i => i.TaxAmount);
        //    WSI.Invoice.TotalAmount = WSI.Details.Sum(i => i.TotalAmount);
        //}

        //public string FromSalesOrder(string soId)
        //{
        //    NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
        //    NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
        //    SOWSI.Mode = "SEL_OBJ";
        //    SOWSI.Order.OrderId = soId;
        //    SOWSI = SOBL.callSingleBL(SOWSI);

        //    if (SOWSI.WsiError == string.Empty)
        //    {
        //        this.FromSalesOrder(SOWSI);
        //    }

        //    return SOWSI.WsiError;
        //}

        // Hoa don tu Phieu xuat kho
        
        private void WriteDebt(NMSalesInvoicesWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsBySalesInvoice(wsi.Invoice.InvoiceId);

            if (wsi.Invoice.DetailsList == null)
                wsi.Invoice.DetailsList = wsi.Details;
            //MGJ.IssueId = wsi.Invoice.InvoiceId;
            MGJ.SIID = wsi.Invoice.InvoiceId;
            MGJ.IssueDate = wsi.Invoice.InvoiceDate;
            MGJ.PartnerId = wsi.Invoice.CustomerId;
            MGJ.ActionBy = wsi.Invoice.SalesPersonId;
            MGJ.StockId = wsi.Invoice.StockId;

            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            //ghi nợ    => phai thu
            double amout = wsi.Invoice.TotalAmount;
            double tax = wsi.Invoice.Tax;
            MGJ.AccountId = "131";
            MGJ.DebitAmount = amout;    //hàng-giảm giá+thuế
            MGJ.CreditAmount = 0;
            MGJ.Descriptions = "Khoản phải thu khách hàng";
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            //ghi có => doanh thu, thuế, doanh thu ghi tùy theo loại sp
            // thue VAT
            if (tax > 0)
            {
                MGJ = new MonthlyGeneralJournals();
                MGJ.SIID = wsi.Invoice.InvoiceId;
                MGJ.IssueDate = wsi.Invoice.InvoiceDate;
                MGJ.PartnerId = wsi.Invoice.CustomerId;
                MGJ.ActionBy = wsi.Invoice.SalesPersonId;
                MGJ.StockId = wsi.Invoice.StockId;

                MGJ.CreatedBy = wsi.ActionBy;
                MGJ.ModifiedBy = wsi.ActionBy;

                MGJ.CurrencyId = "VND";
                MGJ.ExchangeRate = 1;
                MGJ.IsBegin = false;
                //ghi có => thu VAT phai nop
                MGJ.AccountId = "33311";    //"511";
                MGJ.DebitAmount = 0;
                MGJ.CreditAmount = tax;
                MGJ.Descriptions = "Thuế VAT đầu ra";
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            // ghi nhận giá vốn hàng bán và Tồn kho hàng hóa
            
            Products pro = new Products();
            ProductsAccesser proAcc = new ProductsAccesser(this.Session);
            
            foreach (SalesInvoiceDetails itm in wsi.Details)
            {
                MGJ = new MonthlyGeneralJournals();
                MGJ.SIID = wsi.Invoice.InvoiceId;
                MGJ.IssueDate = wsi.Invoice.InvoiceDate;
                MGJ.PartnerId = wsi.Invoice.CustomerId;
                MGJ.ActionBy = wsi.Invoice.SalesPersonId;
                MGJ.StockId = wsi.Invoice.StockId;

                MGJ.CreatedBy = wsi.ActionBy;
                MGJ.ModifiedBy = wsi.ActionBy;

                MGJ.CurrencyId = "VND";
                MGJ.ExchangeRate = 1;
                MGJ.IsBegin = false;
                pro = proAcc.GetAllProductsByID(itm.ProductId, true);
                switch (pro.TypeId)
                {
                    case "155":                        
                        MGJ.AccountId = "5112";
                        MGJ.Descriptions = "Doanh thu bán sản phẩm";
                        break;
                    case "1561":
                        MGJ.AccountId = "5111";
                        MGJ.Descriptions = "Doanh thu bán hàng hóa";
                        break;
                    case "158":
                        MGJ.AccountId = "5113";
                        MGJ.Descriptions = "Doanh thu cung cấp dịch vụ";
                        break;
                    default:
                        MGJ.AccountId = "7113";
                        MGJ.Descriptions = "Doanh thu khác";
                        break;
                }
                MGJ.ProductId = itm.ProductId;
                MGJ.UnitId = itm.UnitId;    //pro.ProductUnit;
                MGJ.DebitAmount = 0;
                MGJ.CreditAmount = itm.Amount - itm.DiscountAmount;     //doanh thu sau chiết khấu
                MGJ.ExportQuantity = itm.Quantity * (1 - itm.Discount / 100);
                MGJ.ImportQuantity = 0;
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);

                if (itm.Discount != 0)
                {
                    MGJ = new MonthlyGeneralJournals();
                    MGJ.SIID = wsi.Invoice.InvoiceId;
                    MGJ.IssueDate = wsi.Invoice.InvoiceDate;
                    MGJ.PartnerId = wsi.Invoice.CustomerId;
                    MGJ.ActionBy = wsi.Invoice.SalesPersonId;
                    MGJ.StockId = wsi.Invoice.StockId;

                    MGJ.CreatedBy = wsi.ActionBy;
                    MGJ.ModifiedBy = wsi.ActionBy;

                    MGJ.CurrencyId = "VND";
                    MGJ.ExchangeRate = 1;
                    MGJ.IsBegin = false;
                    switch (pro.TypeId)
                    {
                        case "155":
                            MGJ.AccountId = "5212";
                            MGJ.Descriptions = "Chiết khấu sản phẩm";
                            break;
                        case "1561":
                            MGJ.AccountId = "5211";
                            MGJ.Descriptions = "Chiết khấu hàng hóa";
                            break;
                        case "158":
                            MGJ.AccountId = "5213";
                            MGJ.Descriptions = "Chiết khấu cung cấp dịch vụ";
                            break;
                    }
                    MGJ.ProductId = itm.ProductId;
                    MGJ.UnitId = itm.UnitId;    // pro.ProductUnit;
                    MGJ.DebitAmount = 0;
                    MGJ.CreditAmount = itm.DiscountAmount;  //chiết khấu
                    MGJ.ExportQuantity = itm.Quantity * itm.Discount / 100;
                    MGJ.ImportQuantity = 0;
                    mgjAccesser.InsertMonthlygeneraljournals(MGJ);
                }
                
            }

            return;
        }

        private void WriteCash(NMSalesInvoicesWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            MGJ.IssueId = wsi.Invoice.InvoiceId;
            MGJ.IssueDate = wsi.Invoice.InvoiceDate;
            MGJ.PartnerId = wsi.Invoice.CustomerId;
            
            MGJ.CreatedDate = DateTime.Now;
            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedDate = DateTime.Now;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            //ghi nợ
            MGJ.AccountId = "111";
            MGJ.DebitAmount = wsi.Invoice.DetailsList.Sum(i=>i.TotalAmount);
            MGJ.CreditAmount = 0;
            MGJ.Descriptions = "thu tiền bán hàng";
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            //ghi có
            MGJ.AccountId = "511";
            MGJ.DebitAmount = 0;
            MGJ.CreditAmount = wsi.Invoice.DetailsList.Sum(i => i.TotalAmount);
            MGJ.Descriptions = "doanh thu bán hàng";
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            return;
        }


        #region Receipt

        public NMReceiptsWSI CreateReceipt()
        {
            NMReceiptsWSI rptWsi = new NMReceiptsWSI();

            rptWsi.Receipt.CustomerId = this.WSI.Invoice.CustomerId;
            rptWsi.Receipt.StockId = this.WSI.Invoice.StockId;
            rptWsi.Receipt.InvoiceId = this.WSI.Invoice.InvoiceId;
            rptWsi.Receipt.ReceiptDate = this.WSI.Invoice.InvoiceDate;
            rptWsi.Receipt.PaymentMethodId = this.WSI.Invoice.PaymentMethod;
            rptWsi.Receipt.Amount = this.WSI.Invoice.TotalAmount;

            rptWsi.Receipt.CurrencyId = "VND";
            rptWsi.Receipt.ExchangeRate = 1;
            rptWsi.Receipt.ReceiptAmount = rptWsi.Receipt.Amount / rptWsi.Receipt.ExchangeRate;
            rptWsi.Receipt.ReceiptTypeId = NMConstant.ReceiptType.Sales;
            rptWsi.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Draft;

            return rptWsi;
        }

        public NMReceiptsWSI CreateReceipt(string siId)
        {
            NMReceiptsWSI rptWsi = new NMReceiptsWSI();

            NMSalesInvoicesBL siBL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI siWSI = new NMSalesInvoicesWSI();
            siWSI.Mode = "SEL_OBJ";
            siWSI.Invoice.InvoiceId = siId;
            siWSI = siBL.callSingleBL(siWSI);

            if (siWSI.WsiError == string.Empty)
            {
                rptWsi = this.CreateReceipt();
                rptWsi.WsiError = siWSI.WsiError;
            }

            else
            {
                rptWsi.WsiError = siWSI.WsiError;
            }

            return rptWsi;
        }

        #endregion
    }
}
