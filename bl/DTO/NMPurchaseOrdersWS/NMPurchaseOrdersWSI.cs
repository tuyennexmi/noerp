// NMPurchaseOrdersWSI.cs

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
using System.Collections;
using System.Linq;
using System.Text;

namespace NEXMI
{
    public class NMPurchaseOrdersWSI
    {
        //Criterion for search
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }
        
        public PurchaseOrders Order;
        public List<PurchaseOrderDetails> Details;
        public IList<PurchaseInvoices> Invoices;
        public IList<Imports> Imports;
        public Customers Supplier;
        public Customers CreatedUser;
        public Statuses Status;
        public Parameters InvoiceControl;
        public NMStocksWSI Stock;

        public Customers Buyer;
        public Customers ApprovalBy;

        public String WsiError;
        public String Mode;

        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;


        public NMPurchaseOrdersWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";

            Order = new PurchaseOrders();
            Details = null;
            Supplier = null;
            CreatedUser = null;
            Status = null;
            InvoiceControl = null;
            Stock = null;
            Buyer = null;
            ApprovalBy = null;

            WsiError = "";
            Mode = "";

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(PurchaseOrders older)
        {
            String result = "";

            if (this.Order.OrderDate != older.OrderDate)
                result = "Ngày đặt: " + older.OrderDate.ToString("dd/MM/yyyy") + " => " + this.Order.OrderDate.ToString("dd/MM/yyyy") + ";";
            try
            {
                if (this.Order.DeliveryDate != older.DeliveryDate)
                    result += "Ngày nhận dự kiến: " + older.DeliveryDate.ToString("dd/MM/yyyy") + " => " + this.Order.DeliveryDate.ToString("dd/MM/yyyy") + ";";
            }
            catch { }
            if (this.Order.PaymentTerm != older.PaymentTerm)
                result += "Điều khoản thanh toán: " + older.PaymentTerm + " => " + this.Order.PaymentTerm + ";";
            if (this.Order.PaymentDate != older.PaymentDate)
                result += "Ngày thanh toán: " + older.PaymentDate + " => " + this.Order.PaymentDate + ";";

            if (this.Order.Reference != older.Reference)
                result += "Số tham chiếu: " + older.Reference + " => " + this.Order.Reference + ";";
            if (this.Order.Description != older.Description)
                result += "Ghi chú: " + older.Description + " => " + this.Order.Description + ";";
            
            if (this.Order.Amount != older.Amount)
                result += "Tiền hàng: " + older.Amount + " => " + this.Order.Amount + ";";
            if (this.Order.Tax != older.Tax)
                result += "Thuế: " + older.Tax + " => " + this.Order.Tax + ";";
            if (this.Order.Discount != older.Discount)
                result += "Chiết khấu: " + older.Discount + " => " + this.Order.Discount + ";";
            if (this.Order.TotalAmount != older.TotalAmount)
                result += "Thành tiền: " + older.TotalAmount + " => " + this.Order.TotalAmount + ";";
            if (this.Order.OrderStatus != older.OrderStatus)
                result += "Trạng thái: " + NMCommon.GetStatusName(older.OrderStatus, "vi") + " => " + NMCommon.GetStatusName(this.Order.OrderStatus, "vi") + ";";
            
            if (this.Order.Transportation != older.Transportation)
                result += "PTVC: " + older.Transportation + " => " + this.Order.Transportation + ";";
            
            if (this.Order.SupplierId != older.SupplierId)
                result += "Nhà cung cấp: " + older.SupplierId + " => " + this.Order.SupplierId + ";";
            if (this.Order.ImportStockId != older.ImportStockId)
                result += "Kho nhập: " + older.ImportStockId + " => " + this.Order.ImportStockId + ";";

            if (this.Order.CarrierId != older.CarrierId)
                result += "Nhà vận chuyển: " + older.CarrierId + " => " + this.Order.CarrierId + ";";
            if (this.Order.ApproveBy != older.ApproveBy)
                result += "Người duyệt: " + older.ApproveBy + " => " + this.Order.ApproveBy + ";";
            if (this.Order.BuyerId != older.BuyerId)
                result += "Người mua: " + older.BuyerId + " => " + this.Order.BuyerId + ";";

            return result;
        }
    }
}
