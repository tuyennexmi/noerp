// NMSalesInvoicesWSI.cs

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
using System.Collections;

namespace NEXMI
{
    public class NMSalesInvoicesWSI
    {
        //Criterion for search
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }

        public Customers Customer;
        public SalesInvoices Invoice;
        public List<SalesInvoiceDetails> Details;
        public List<Receipts> Receipts;
        public List<Payments> Payments;
        public Customers CreatedBy;
        public Customers SalesPerson;
        //public Statuses Status;
        public Customers ApprovalBy;

        public String WsiError;
        public String Mode;
        public bool Commit;

        public bool GetDetails;
        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;

        public NMSalesInvoicesWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";

            Customer = null;
            Invoice = new SalesInvoices();
            Details = null;
            Receipts = null;
            Payments = null;
            SalesPerson = null;
            CreatedBy = null;
            //Status = null;

            WsiError = "";
            Mode = "";
            this.GetDetails = false;
            this.Commit = true;

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(SalesInvoices older)
        {
            String result = "";
            if (this.Invoice.CustomerId != older.CustomerId && !string.IsNullOrEmpty(this.Invoice.CustomerId))
                result = "Khách hàng: " + NMCommon.GetCustomerName(this.Invoice.CustomerId) + ";";
            if (this.Invoice.InvoiceDate != older.InvoiceDate)
                result += "Ngày đặt: " + older.InvoiceDate.ToString("dd/MM/yyyy") + " => " + this.Invoice.InvoiceDate.ToString("dd/MM/yyyy") + ";";
            if (this.Invoice.ExchangeRate != older.ExchangeRate)
                result += "Tỷ giá: " + older.ExchangeRate.ToString() + " => " + this.Invoice.ExchangeRate.ToString() + ";";
            //if (this.Invoice.ShipCost != older.ShipCost)
            //    result += "Phí vận chuyển: " + older.ShipCost.ToString() + " => " + this.Invoice.ShipCost.ToString() + ";";
            //if (this.Invoice.OtherCost != older.OtherCost)
            //    result += "Phí khác: " + older.OtherCost + " => " + this.Invoice.OtherCost + ";";            
            
            if (this.Invoice.Reference != older.Reference)
                result += "Số tham chiếu: " + older.Reference + " => " + this.Invoice.Reference + ";";            
            if (this.Invoice.DescriptionInVietnamese != older.DescriptionInVietnamese)
                result += "Ghi chú: " + older.DescriptionInVietnamese + " => " + this.Invoice.DescriptionInVietnamese + ";";

            if (this.Invoice.SalesPersonId != older.SalesPersonId)
                result += "Người bán: " + older.SalesPersonId + " => " + this.Invoice.SalesPersonId + ";";
            if (this.Invoice.PaymentMethod != older.PaymentMethod)
                result += "Phương thức thanh toán: " + older.PaymentMethod + " => " + this.Invoice.PaymentMethod + ";";
            if (this.Invoice.BankAccount != older.BankAccount)
                result += "Ngân hàng: " + older.BankAccount + " => " + this.Invoice.BankAccount + ";";
            if (this.Invoice.ApprovalBy != older.ApprovalBy)
                result += "Người duyệt: " + older.ApprovalBy + " => " + this.Invoice.ApprovalBy + ";";
            if (this.Invoice.StockId != older.StockId)
                result += "Kho: " + older.StockId + " => " + this.Invoice.StockId + ";";

            //if (this.Invoice.DetailsList.Sum(i=>i.TaxAmount) != older.DetailsList.Sum(i=>i.TaxAmount))
            //    result += "Thuế: " + older.DetailsList.Sum(i=>i.TaxAmount) + " => " + this.Invoice.DetailsList.Sum(i=>i.TaxAmount) + ";";
            //if (this.Invoice.DetailsList.Sum(i=>i.DiscountAmount) != older.DetailsList.Sum(i=>i.DiscountAmount))
            //    result += "Chiết khấu: " + older.DetailsList.Sum(i=>i.DiscountAmount) + " => " + this.Invoice.DetailsList.Sum(i=>i.DiscountAmount) + ";";
            //if (this.Invoice.DetailsList.Sum(i=>i.TotalAmount) != older.DetailsList.Sum(i=>i.TotalAmount))
            //    result += "Thành tiền: " + older.DetailsList.Sum(i=>i.TotalAmount) + " => " + this.Invoice.DetailsList.Sum(i=>i.TotalAmount) + ";";

            if (this.Invoice.Amount != older.Amount)
                result += "Tổng chưa thuế: " + older.Amount + " => " + this.Invoice.Amount + ";";
            if (this.Invoice.Tax != older.Tax)
                result += "Thuế: " + older.Tax + " => " + this.Invoice.Tax + ";";
            if (this.Invoice.Discount != older.Discount)
                result += "Chiết khấu: " + older.Discount + " => " + this.Invoice.Discount + ";";
            if (this.Invoice.TotalAmount != older.TotalAmount)
                result += "Thành tiền sau thuế: " + older.TotalAmount + " => " + this.Invoice.TotalAmount + ";";

            if (this.Invoice.InvoiceStatus != older.InvoiceStatus)
                result += "Trạng thái: " + NMCommon.GetStatusName(older.InvoiceStatus, "vi") + " => " + NMCommon.GetStatusName(this.Invoice.InvoiceStatus, "vi") + ";";

            return result;
        }
    }
}
