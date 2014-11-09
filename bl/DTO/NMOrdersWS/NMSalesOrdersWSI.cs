// NMSalesOrdersWSI.cs

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
    public class NMSalesOrdersWSI
    {
        NMFilter Filter = new NMFilter();
        //Criterion for search
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }
        //public String TypeName { get; set; }

        public SalesOrders Order;
        public List<SalesOrderDetails> Details;
        public IList<SalesInvoices> Invoices;
        public IList<Exports> Exports;
        public Customers Customer;
        public Customers SalesPerson;
        public Customers CreatedBy;
        public Statuses Status;
        public Parameters Incoterm;
        public Parameters ShippingPolicy;
        public Parameters CreateInvoice;
        //public List<NMReceipts> Receipts;

        public String WsiError;
        public String Mode;

        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;

        public NMSalesOrdersWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";

            Order = new SalesOrders();
            Details = null;
            Customer = null;
            SalesPerson = null;
            CreatedBy = null;
            Status = null;
            Incoterm = null;
            ShippingPolicy = null;
            CreateInvoice = null;

            WsiError = "";
            Mode = "";

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(SalesOrders older)
        {
            String result = "";

            if (this.Order.OrderDate != older.OrderDate)
                result = "Ngày đặt: " + older.OrderDate.ToString("dd/MM/yyyy") + " => " + this.Order.OrderDate.ToString("dd/MM/yyyy") + ";";
            if (this.Order.DeliveryDate != older.DeliveryDate)
                result += "Ngày giao: " + older.DeliveryDate.Value.ToString("dd/MM/yyyy") + " => " + this.Order.DeliveryDate.Value.ToString("dd/MM/yyyy") + ";";
            if (this.Order.ExpirationDate != older.ExpirationDate)
                result += "Ngày hết hạn: " + older.ExpirationDate.ToString() + " => " + this.Order.ExpirationDate.ToString() + ";";
            if (this.Order.Incoterm != older.Incoterm)
                result += "Điều khoản thương mại: " + NMCommon.GetParameterName(older.Incoterm) + " => " + NMCommon.GetParameterName(this.Order.Incoterm) + ";";
            if (this.Order.PaymentTerm != older.PaymentTerm)
                result += "Điều khoản thanh toán: " + NMCommon.GetParameterName(older.PaymentTerm) + " => " + NMCommon.GetParameterName(this.Order.PaymentTerm) + ";";
            if (this.Order.ShippingPolicy != older.ShippingPolicy)
                result += "Phương thức giao hàng: " + NMCommon.GetParameterName(older.ShippingPolicy) + " => " + NMCommon.GetParameterName(this.Order.ShippingPolicy) + ";";
            if (this.Order.CreateInvoice != older.CreateInvoice)
                result += "Xuất hóa đơn: " + NMCommon.GetParameterName(older.CreateInvoice) + " => " + NMCommon.GetParameterName(this.Order.CreateInvoice) + ";";
            if (this.Order.Reference != older.Reference)
                result += "Số tham chiếu: " + older.Reference + " => " + this.Order.Reference + ";";
            if (this.Order.Advances != older.Advances)
                result += "Tạm ứng: " + older.Advances + " => " + this.Order.Advances + ";";
            if (this.Order.Description != older.Description)
                result += "Ghi chú: " + older.Description + " => " + this.Order.Description + ";";
            if (this.Order.StockId != older.StockId)
                result += "Kho: " + older.StockId + " => " + this.Order.StockId + ";";
            if (this.Order.SalesPersonId != older.SalesPersonId)
                result += "Người bán: " + older.SalesPersonId + " => " + this.Order.SalesPersonId + ";";

            if (this.Order.Amount != older.Amount)
                result += "Tiền hàng: " + older.Amount + " => " + this.Order.Amount + ";";
            if (this.Order.Tax != older.Tax)
                result += "Thuế: " + older.Tax + " => " + this.Order.Tax + ";";
            if (this.Order.Discount != older.Discount)
                result += "Chiết khấu: " + older.Discount + " => " + this.Order.Discount + ";";
            if (this.Order.TotalAmount != older.TotalAmount)
                result += "Tổng tiền: " + older.TotalAmount + " => " + this.Order.TotalAmount + ";";
            
            if (this.Order.OrderStatus != older.OrderStatus)
                result += "Trạng thái: " + older.OrderStatus + " => " + this.Order.OrderStatus + ";";

            return result;
        }
    }
}
