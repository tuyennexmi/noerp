// NMReceiptsBL.cs

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
    public class NMReceiptsBL
    {
        private readonly ISession Session;

        public NMReceiptsWSI WSI = new NMReceiptsWSI();

        public NMReceiptsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMReceiptsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMReceiptsWSI callSingleBL(NMReceiptsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                case "APP_OBJ":
                    return ApprovalObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMReceiptsWSI> callListBL(NMReceiptsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMReceiptsWSI>();
            }
        }

        public NMReceiptsWSI SelectObject(NMReceiptsWSI wsi)
        {
            ReceiptsAccesser Accesser = new ReceiptsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            BanksAccesser BankAcc = new BanksAccesser(Session);
            Receipts obj;
            obj = Accesser.GetAllReceiptsByID(wsi.Receipt.ReceiptId, true);
            if (obj != null)
            {
                wsi.Receipt = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.PaymentMethod = ParameterAccesser.GetAllParametersByID(obj.PaymentMethodId, true);
                wsi.ReceiveBank = BankAcc.GetAllBanksByID(obj.ReceivedBankAccount, true);
                
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMReceiptsWSI SaveObject(NMReceiptsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {   
                ReceiptsAccesser ReceiptsAccesser = new ReceiptsAccesser(Session);
                Receipts obj = ReceiptsAccesser.GetAllReceiptsByID(wsi.Receipt.ReceiptId, true);
                if (obj != null)
                {
                    Session.Evict(obj);

                    wsi.Receipt.CreatedBy = obj.CreatedBy;
                    wsi.Receipt.CreatedDate = obj.CreatedDate;
                    wsi.Receipt.ModifiedBy = wsi.ActionBy;                    
                    wsi.Receipt.ModifiedDate = DateTime.Now;
                    
                    ReceiptsAccesser.UpdateReceipts(wsi.Receipt);
                    //ghi log
                    String rs = wsi.CompareTo(obj);
                    if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.Receipt.ReceiptId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Receipt.ReceiptId = AutomaticValueAccesser.AutoGenerateId("Receipts");

                    wsi.Receipt.CreatedDate = DateTime.Now;
                    wsi.Receipt.CreatedBy = wsi.ActionBy;
                    wsi.Receipt.ModifiedBy = wsi.ActionBy;
                    wsi.Receipt.ModifiedDate = DateTime.Now;
                    
                    ReceiptsAccesser.InsertReceipts(wsi.Receipt);
                    NMMessagesBL.SaveMessage(Session, wsi.Receipt.ReceiptId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                //if (wsi.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Done && !string.IsNullOrEmpty(wsi.Receipt.InvoiceId))
                //{
                //    //UpdateInvoiceStatus(Session, wsi.Receipt.InvoiceId, wsi.ActionBy);
                //}
                if (wsi.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Done)
                {
                    this.WriteCash(wsi);
                    //if (!string.IsNullOrEmpty(obj.InvoiceId))
                    //    NMCommon.UpdateInvoiceStatus(Session, obj.InvoiceId, wsi.ActionBy);
                }
                tx.Commit();

                if (wsi.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Done)
                {
                    NMCommon.CreatePaymentFromReceipt(wsi);
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMReceiptsWSI DeleteObject(NMReceiptsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                ReceiptsAccesser Accesser = new ReceiptsAccesser(Session);
                Receipts obj = Accesser.GetAllReceiptsByID(wsi.Receipt.ReceiptId, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteReceipts(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMReceiptsWSI> SearchObject(NMReceiptsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Receipt != null)
            {
                if (!string.IsNullOrEmpty(wsi.Receipt.InvoiceId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceId = :InvoiceId";
                    ListCriteria.Add("InvoiceId", wsi.Receipt.InvoiceId);
                }
                if (!string.IsNullOrEmpty(wsi.Receipt.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Receipt.CustomerId);
                }
                if (!string.IsNullOrEmpty(wsi.Receipt.ReceiptStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ReceiptStatus = :ReceiptStatus";
                    ListCriteria.Add("ReceiptStatus", wsi.Receipt.ReceiptStatus);
                }
                if (!string.IsNullOrEmpty(wsi.Receipt.ReceiptTypeId))
                {
                    NMAccountNumbersBL BL = new NMAccountNumbersBL();
                    NMAccountNumbersWSI accWsi = new NMAccountNumbersWSI();
                    accWsi.Mode = "SRC_OBJ";
                    accWsi.AccountNumber.ParentId = wsi.Receipt.ReceiptTypeId;
                    List<NMAccountNumbersWSI> al = BL.callListBL(accWsi);
                    if (al.Count == 0)
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.ReceiptTypeId = :ReceiptTypeId";
                        ListCriteria.Add("ReceiptTypeId", wsi.Receipt.ReceiptTypeId);
                    }
                    else
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.ReceiptTypeId in (select a.Id from AccountNumbers a where a.ParentId = :ReceiptTypeId)";
                        ListCriteria.Add("ReceiptTypeId", wsi.Receipt.ReceiptTypeId);                        
                    }
                }
                if (!string.IsNullOrEmpty(wsi.Receipt.CreatedBy))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CreatedBy = :CreatedBy";
                    ListCriteria.Add("CreatedBy", wsi.Receipt.CreatedBy);
                }

                if (!string.IsNullOrEmpty(wsi.Receipt.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Receipt.Reference);
                }
            }
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {   
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ReceiptDate >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ReceiptDate <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.InvoiceId LIKE :Keyword OR O.CustomerId LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.ReceiptId) FROM Receipts AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.ReceiptDate DESC, O.ReceiptId DESC";
            }
            String strCmd = "SELECT O FROM Receipts AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMReceiptsWSI> ListWSI = new List<NMReceiptsWSI>();
            ReceiptsAccesser Accesser = new ReceiptsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            IList<Receipts> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetReceiptsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetReceiptsByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            objs = Accesser.GetReceiptsByQuery(query, false);
            foreach (Receipts obj in objs)
            {
                wsi = new NMReceiptsWSI();
                wsi.Receipt = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.PaymentMethod = ParameterAccesser.GetAllParametersByID(obj.PaymentMethodId, true);
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }

        public NMReceiptsWSI ApprovalObject(NMReceiptsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ReceiptsAccesser ReceiptsAccesser = new ReceiptsAccesser(Session);
                Receipts obj = ReceiptsAccesser.GetAllReceiptsByID(wsi.Receipt.ReceiptId, true);
                if (obj != null)
                {
                    obj.ReceiptStatus = wsi.Receipt.ReceiptStatus;
                    obj.ModifiedBy = wsi.ActionBy;
                    obj.ModifiedDate = DateTime.Now;
                    if (wsi.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Approved)
                        obj.ApprovedBy = wsi.ActionBy;
                    ReceiptsAccesser.UpdateReceipts(obj);

                    if (obj.ReceiptStatus == NMConstant.ReceiptStatuses.Done)
                    {
                        wsi.Receipt = obj;
                        this.WriteCash(wsi);
                    }

                    tx.Commit();

                    // log 
                    NMCommon.SaveMessage(obj.ReceiptId,
                                        "thay đổi trạng thái",
                                        "Trạng thái: " + NMCommon.GetStatusName(obj.ReceiptStatus, "vi"),
                                        wsi.ActionBy);

                    if (!string.IsNullOrEmpty(wsi.Receipt.InvoiceId))
                    {
                        // kiểm tra hóa đơn đã ghi sổ nợ chưa?
                        NMSalesInvoicesBL BL = new NMSalesInvoicesBL(this.Session);
                        NMSalesInvoicesWSI iWSI = new NMSalesInvoicesWSI();
                        iWSI.Mode = "SEL_OBJ";
                        iWSI.Invoice.InvoiceId = wsi.Receipt.InvoiceId;
                        iWSI = BL.callSingleBL(iWSI);

                        // nếu chua ghi no, ghi nhận no de giam giá trị tồn kho
                        if (iWSI.Invoice.InvoiceStatus == NMConstant.SIStatuses.Draft)
                        {
                            // cap nhat trang thai cho PInvoice, POrder
                            iWSI.Mode = "SAV_OBJ";
                            iWSI.Invoice.InvoiceStatus = NMConstant.SIStatuses.Paid;
                            iWSI.ActionBy = wsi.ActionBy;
                            iWSI = BL.callSingleBL(iWSI);
                        }
                        else if (iWSI.Invoice.InvoiceStatus == NMConstant.SIStatuses.Debit)
                        {
                            NMCommon.UpdateObjectStatus(this.Session, "SalesInvoices", iWSI.Invoice.InvoiceId, NMConstant.SIStatuses.Paid, null, wsi.ActionBy, "Đã thanh toán. Số phiếu thu:" + wsi.Receipt.ReceiptId);
                        }
                    }

                    if (wsi.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Done)
                    {
                        NMCommon.CreatePaymentFromReceipt(wsi);
                    }
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private void WriteCash(NMReceiptsWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsByReceipt(wsi.Receipt.ReceiptId);

            //MGJ.IssueId = wsi.Receipt.ReceiptId;
            MGJ.RPTID = wsi.Receipt.ReceiptId;
            MGJ.IssueDate = wsi.Receipt.ReceiptDate;
            MGJ.PartnerId = wsi.Receipt.CustomerId;
            MGJ.SIID = wsi.Receipt.InvoiceId;
            MGJ.ActionBy = wsi.ActionBy;
            MGJ.StockId = wsi.Receipt.StockId;

            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            //ghi nợ => tăng tiền 
            if (wsi.Receipt.PaymentMethodId == NMConstant.PaymentMethod.Cash)
            {
                MGJ.AccountId = "1111";                
            }
            else
            {
                MGJ.AccountId = "1121";
                MGJ.BankId = wsi.Receipt.ReceivedBankAccount;
            }
            MGJ.DebitAmount = wsi.Receipt.ReceiptAmount;
            MGJ.CreditAmount = 0;
            MGJ.Descriptions = wsi.Receipt.DescriptionInVietnamese;
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            //ghi có
            MGJ = new MonthlyGeneralJournals();
            MGJ.RPTID = wsi.Receipt.ReceiptId;
            MGJ.IssueDate = wsi.Receipt.ReceiptDate;
            MGJ.PartnerId = wsi.Receipt.CustomerId;
            MGJ.SIID = wsi.Receipt.InvoiceId;
            MGJ.ActionBy = wsi.ActionBy;
            MGJ.StockId = wsi.Receipt.StockId;

            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            // nếu là thu tiền hóa đơn bán hàng
            if (!string.IsNullOrEmpty(wsi.Receipt.InvoiceId))
            {
                // nếu đã ghi => ghi giảm nợ
                MGJ.AccountId = "131";
                MGJ.DebitAmount = 0;
                MGJ.CreditAmount = wsi.Receipt.ReceiptAmount;
                MGJ.Descriptions = wsi.Receipt.DescriptionInVietnamese;
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }
            else    // thu khác
            {
                MGJ.AccountId = wsi.Receipt.ReceiptTypeId;
                MGJ.DebitAmount = 0;
                MGJ.CreditAmount = wsi.Receipt.ReceiptAmount;
                MGJ.Descriptions = wsi.Receipt.DescriptionInVietnamese;
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            return;
        }

        //public void FromSalesInvoice(NMSalesInvoicesWSI siWsi)
        //{
        //    this.WSI.Receipt.CustomerId = siWsi.Invoice.CustomerId;
        //    this.WSI.Receipt.StockId = siWsi.Invoice.StockId;
        //    this.WSI.Receipt.InvoiceId = siWsi.Invoice.InvoiceId;
        //    this.WSI.Receipt.ReceiptDate = siWsi.Invoice.InvoiceDate;
        //    this.WSI.Receipt.Amount = siWsi.Invoice.DetailsList.Sum(i => i.TotalAmount);
        //    this.WSI.Receipt.CurrencyId = "VND";
        //    this.WSI.Receipt.ExchangeRate = 1;
        //    this.WSI.Receipt.ReceiptAmount = this.WSI.Receipt.Amount / this.WSI.Receipt.ExchangeRate;
        //    this.WSI.Receipt.ReceiptTypeId = NMConstant.ReceiptType.Sales;
        //    this.WSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Draft;
        //    this.WSI.Receipt.PaymentMethodId = siWsi.Invoice.PaymentMethod;
        //}

        //public string FromSalesInvoice(string siId)
        //{
        //    string error ="";
        //    if (!string.IsNullOrEmpty(siId))
        //    {
        //        NMSalesInvoicesBL siBL = new NMSalesInvoicesBL();
        //        NMSalesInvoicesWSI siWSI = new NMSalesInvoicesWSI();
        //        siWSI.Mode = "SEL_OBJ";
        //        siWSI.Invoice = new SalesInvoices();
        //        siWSI.Invoice.InvoiceId = siId;
        //        siWSI = siBL.callSingleBL(siWSI);

        //        if (siWSI.WsiError == string.Empty)
        //        {
        //            this.FromSalesInvoice(siWSI);
        //            error = siWSI.WsiError;
        //        }
        //    }
        //    else
        //    {
        //        error = "Dữ liệu không tồn tại, vui lòng kiểm tra lại!";
        //    }
        //    return error;
        //}
    }
}
