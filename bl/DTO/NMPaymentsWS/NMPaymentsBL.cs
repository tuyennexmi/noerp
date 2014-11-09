// NMPaymentsBL.cs

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
    public class NMPaymentsBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();

        public NMPaymentsWSI WSI = new NMPaymentsWSI();

        public NMPaymentsBL()
        {

        }

        public NMPaymentsWSI callSingleBL(NMPaymentsWSI wsi)
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

        public List<NMPaymentsWSI> callListBL(NMPaymentsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMPaymentsWSI>();
            }
        }

        public NMPaymentsWSI SelectObject(NMPaymentsWSI wsi)
        {
            PaymentsAccesser Acceser = new PaymentsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            BanksAccesser BankAcc = new BanksAccesser(Session);

            Payments obj;
            obj = Acceser.GetAllPaymentsByID(wsi.Payment.PaymentId, true);
            if (obj != null)
            {
                wsi.Payment = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                wsi.PaymentMethod = ParameterAccesser.GetAllParametersByID(obj.PaymentMethodId, true);
                wsi.ReceiveBank = BankAcc.GetAllBanksByID(obj.ReceiveBank, true);
                wsi.PaymentBank = BankAcc.GetAllBanksByID(obj.PaymentBankAccount, true);
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMPaymentsWSI SaveObject(NMPaymentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {   
                PaymentsAccesser Accesser = new PaymentsAccesser(Session);
                Payments obj = Accesser.GetAllPaymentsByID(wsi.Payment.PaymentId, true);
                if (obj != null)
                {
                    Session.Evict(obj);
                    
                    wsi.Payment.CreatedBy = obj.CreatedBy;
                    wsi.Payment.CreatedDate = obj.CreatedDate;
                    wsi.Payment.ModifiedBy = wsi.ActionBy;
                    wsi.Payment.ModifiedDate = DateTime.Now;

                    Accesser.UpdatePayments(wsi.Payment);
                    //ghi log
                    String rs = wsi.CompareTo(obj);
                    if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.Payment.PaymentId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);                    
                }
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Payment.PaymentId = AutomaticValueAccesser.AutoGenerateId("Payments");
                    wsi.Payment.CreatedDate = DateTime.Now;
                    wsi.Payment.CreatedBy = wsi.ActionBy;
                    wsi.Payment.ModifiedBy = wsi.ActionBy;
                    wsi.Payment.ModifiedDate = DateTime.Now;
                    Accesser.InsertPayments(wsi.Payment);
                    NMMessagesBL.SaveMessage(Session, wsi.Payment.PaymentId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }

                if (wsi.Payment.PaymentStatus == NMConstant.PaymentStatuses.Done)
                {
                    this.WriteMGJ(wsi);
                }

                tx.Commit();

                if (wsi.Payment.PaymentStatus == NMConstant.PaymentStatuses.Done)
                {
                    NMCommon.CreateReceiptFromPayment(wsi);
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMPaymentsWSI DeleteObject(NMPaymentsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                PaymentsAccesser Accesser = new PaymentsAccesser(Session);
                Payments obj = Accesser.GetAllPaymentsByID(wsi.Payment.PaymentId, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeletePayments(obj);
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

        public List<NMPaymentsWSI> SearchObject(NMPaymentsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Payment != null)
            {
                if (!string.IsNullOrEmpty(wsi.Payment.InvoiceId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceId = :InvoiceId";
                    ListCriteria.Add("InvoiceId", wsi.Payment.InvoiceId);
                }
                if (!string.IsNullOrEmpty(wsi.Payment.SupplierId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.SupplierId = :SupplierId";
                    ListCriteria.Add("SupplierId", wsi.Payment.SupplierId);
                }
                if (!string.IsNullOrEmpty(wsi.Payment.PaymentStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.PaymentStatus = :PaymentStatus";
                    ListCriteria.Add("PaymentStatus", wsi.Payment.PaymentStatus);
                }
                if (!string.IsNullOrEmpty(wsi.Payment.PaymentTypeId))
                {

                    NMAccountNumbersBL BL = new NMAccountNumbersBL();
                    NMAccountNumbersWSI accWsi = new NMAccountNumbersWSI();
                    accWsi.Mode = "SRC_OBJ";
                    accWsi.AccountNumber.ParentId = wsi.Payment.PaymentTypeId;
                    List<NMAccountNumbersWSI> al = BL.callListBL(accWsi);
                    if (al.Count > 0)
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.PaymentTypeId in (select a.Id from AccountNumbers a where a.ParentId = :PaymentTypeId)";
                        ListCriteria.Add("PaymentTypeId", wsi.Payment.PaymentTypeId);
                    }
                    else
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.PaymentTypeId = :PaymentTypeId";
                        ListCriteria.Add("PaymentTypeId", wsi.Payment.PaymentTypeId);
                    }

                }
                if (!string.IsNullOrEmpty(wsi.Payment.CreatedBy))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CreatedBy = :CreatedBy";
                    ListCriteria.Add("CreatedBy", wsi.Payment.CreatedBy);
                }

                if (!string.IsNullOrEmpty(wsi.Payment.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Payment.Reference);
                }
            }

            if (!string.IsNullOrEmpty(wsi.FromDate))
            {   
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.PaymentDate >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.PaymentDate <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }

            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.InvoiceId LIKE :Keyword OR O.CustomerId LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.PaymentId) FROM Payments AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.PaymentDate DESC, O.PaymentId DESC";
            }
            String strCmd = "SELECT O FROM Payments AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMPaymentsWSI> ListWSI = new List<NMPaymentsWSI>();
            PaymentsAccesser Accesser = new PaymentsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            IList<Payments> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetPaymentsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetPaymentsByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList(); //query.SetFirstResult(0).SetMaxResults(wsi.Limit);
            }
            objs = Accesser.GetPaymentsByQuery(query, false);
            foreach (Payments obj in objs)
            {
                wsi = new NMPaymentsWSI();
                wsi.Payment = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                wsi.PaymentMethod = ParameterAccesser.GetAllParametersByID(obj.PaymentMethodId, true);
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }

        public NMPaymentsWSI ApprovalObject(NMPaymentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PaymentsAccesser PaymentAccesser = new PaymentsAccesser(Session);
                Payments obj = PaymentAccesser.GetAllPaymentsByID(wsi.Payment.PaymentId, true);
                if (obj != null)
                {
                    obj.ModifiedBy = wsi.ActionBy;
                    obj.ModifiedDate = DateTime.Now;
                    obj.PaymentStatus = wsi.Payment.PaymentStatus;
                    if (wsi.Payment.PaymentStatus == NMConstant.PaymentStatuses.Approved)
                        obj.ApprovedBy = wsi.ActionBy;
                    PaymentAccesser.UpdatePayments(obj);

                    NMCommon.SaveMessage(Session, obj.PaymentId, "thay đổi trạng thái", "Trạng thái: " + NMCommon.GetStatusName(obj.PaymentStatus, "vi"), wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    
                    if (obj.PaymentStatus == NMConstant.PaymentStatuses.Done)
                    {
                        wsi.Payment = obj;
                        this.WriteMGJ(wsi);
                    }
                    tx.Commit();

                    if (wsi.Payment.PaymentStatus == NMConstant.PaymentStatuses.Done)
                    {
                        NMCommon.CreateReceiptFromPayment(wsi);
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

        private void WriteMGJ(NMPaymentsWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsByPayment(wsi.Payment.PaymentId);

            //MGJ.IssueId = wsi.Payment.PaymentId;
            MGJ.PADID = wsi.Payment.PaymentId;
            MGJ.IssueDate = wsi.Payment.PaymentDate;
            MGJ.PartnerId = wsi.Payment.SupplierId;
            MGJ.PIID = wsi.Payment.InvoiceId;
            MGJ.ActionBy = wsi.ActionBy;
            MGJ.StockId = wsi.Payment.StockId;
            
            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;

            //ghi nợ
            // nếu là thanh toán hóa đơn
            if (!string.IsNullOrEmpty(wsi.Payment.InvoiceId))
            {   
                //kiểm tra hóa đơn mới hay đã ghi sổ nợ
                NMPurchaseInvoicesBL invBl = new NMPurchaseInvoicesBL(this.Session);
                NMPurchaseInvoicesWSI invWsi = new NMPurchaseInvoicesWSI();
                invWsi.Mode = "SEL_OBJ";
                invWsi.Invoice.InvoiceId = wsi.Payment.InvoiceId;
                invWsi = invBl.callSingleBL(invWsi);

                // nếu đã ghi nợ => ghi giảm nợ
                if (invWsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Debit)
                {
                    NMCommon.UpdateObjectStatus(this.Session, "PurchaseInvoices", invWsi.Invoice.InvoiceId, NMConstant.PIStatuses.Paid, null, wsi.ActionBy, "Đã thanh toán. Số phiếu thu:" + wsi.Payment.PaymentId);
                }
                else if (invWsi.Invoice.InvoiceStatus == NMConstant.PIStatuses.Draft)
                {
                    // cap nhat trang thai cho PInvoice, POrder
                    invWsi.Mode = "SAV_OBJ";
                    invWsi.Commit = false;
                    invWsi.Invoice.InvoiceStatus = NMConstant.PIStatuses.Paid;
                    invWsi.ActionBy = wsi.ActionBy;
                    invWsi = invBl.callSingleBL(invWsi);
                }

                //ghi giam no
                MGJ.AccountId = "331";
                MGJ.DebitAmount = wsi.Payment.PaymentAmount;
                MGJ.CreditAmount = 0;
                MGJ.Descriptions = wsi.Payment.DescriptionInVietnamese;
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }
            else    // thanh toan khac
            {
                MGJ.AccountId = wsi.Payment.PaymentTypeId;

                //  nop tien vao tai khoan
                if (wsi.Payment.PaymentTypeId.Contains("112"))
                    MGJ.BankId = wsi.Payment.ReceiveBank;
                MGJ.DebitAmount = wsi.Payment.PaymentAmount;
                MGJ.CreditAmount = 0;
                MGJ.Descriptions = wsi.Payment.DescriptionInVietnamese;
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            //giảm tiền => ghi có            
            MGJ = new MonthlyGeneralJournals();
            MGJ.PADID = wsi.Payment.PaymentId;
            MGJ.IssueDate = wsi.Payment.PaymentDate;
            MGJ.PartnerId = wsi.Payment.SupplierId;
            MGJ.PIID = wsi.Payment.InvoiceId;
            MGJ.ActionBy = wsi.ActionBy;
            MGJ.StockId = wsi.Payment.StockId;

            MGJ.CreatedBy = wsi.ActionBy;
            MGJ.ModifiedBy = wsi.ActionBy;

            MGJ.CurrencyId = "VND";
            MGJ.ExchangeRate = 1;
            MGJ.IsBegin = false;
            MGJ.PADID = wsi.Payment.PaymentId;
            MGJ.IssueDate = wsi.Payment.PaymentDate;
            if (wsi.Payment.PaymentMethodId == NMConstant.PaymentMethod.Cash)
            {
                MGJ.AccountId = "1111";
            }
            else
            {
                MGJ.AccountId = "1121";
                MGJ.BankId = wsi.Payment.PaymentBankAccount;                
            }
            MGJ.DebitAmount = 0;
            MGJ.CreditAmount = wsi.Payment.PaymentAmount;
            MGJ.Descriptions = wsi.Payment.DescriptionInVietnamese;
            mgjAccesser.InsertMonthlygeneraljournals(MGJ);

            return;
        }


    }
}
