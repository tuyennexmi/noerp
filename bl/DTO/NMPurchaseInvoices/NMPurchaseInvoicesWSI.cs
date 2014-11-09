// NMPurchaseInvoicesWSI.cs

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
    public class NMPurchaseInvoicesWSI
    {
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }

        public PurchaseInvoices Invoice;
        public List<PurchaseInvoiceDetails> Details;
        public Customers Supplier;
        //public Statuses Status;
        public Customers CreatedBy;
        public List<Receipts> Receipts;
        public List<Payments> Payments;

        public String WsiError;
        public String Mode;
        public bool Commit;

        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;

        public NMPurchaseInvoicesWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";
            ActionBy = "";

            Invoice = new PurchaseInvoices();
            Details = null;
            this.Supplier = null;
            //this.Status = null;

            WsiError = "";
            Mode = "";
            this.Commit = true;

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(PurchaseInvoices older)
        {
            String result = "";
            if (this.Invoice.SupplierId != older.SupplierId && !string.IsNullOrEmpty(this.Invoice.SupplierId))
                result = "Nhà cung cấp: " + NMCommon.GetCustomerName(this.Invoice.SupplierId) + ";";
            if (this.Invoice.InvoiceDate != older.InvoiceDate)
                result += "Ngày mua: " + older.InvoiceDate.ToString("dd/MM/yyyy") + " => " + this.Invoice.InvoiceDate.ToString("dd/MM/yyyy") + ";";
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
            
            if (this.Invoice.Amount != older.Amount)
                result += "Tổng tiền: " + older.Amount + " => " + this.Invoice.Amount + ";";
            if (this.Invoice.Tax != older.Tax)
                result += "Thuế: " + older.Tax + " => " + this.Invoice.Tax + ";";
            if (this.Invoice.Discount != older.Discount)
                result += "Chiết khấu: " + older.Discount + " => " + this.Invoice.Discount + ";";
            if (this.Invoice.TotalAmount != older.TotalAmount)
                result += "Thành tiền: " + older.TotalAmount + " => " + this.Invoice.TotalAmount + ";";
            
            if (this.Invoice.InvoiceStatus != older.InvoiceStatus)
                result += "Trạng thái: " + NMCommon.GetStatusName(older.InvoiceStatus, "vi") + " => " + NMCommon.GetStatusName(this.Invoice.InvoiceStatus, "vi") + ";";
            
            if (this.Invoice.BuyerId != older.BuyerId)
                result += "Người mua: " + older.BuyerId + " => " + this.Invoice.BuyerId + ";";
            if (this.Invoice.SupplierReference != older.SupplierReference)
                result += "Tham chiếu NCC: " + older.SupplierReference + " => " + this.Invoice.SupplierReference + ";";

            return result;
        }
    }
}
