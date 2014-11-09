// NMManufactureOrdersWSI.cs

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

namespace NEXMI
{
    public class NMManufactureOrdersWSI
    {
        public NMFilter Filter;
        public string CategoryId;

        public ManufactureOrders ManufactureOrder;
        public Customers Customer;
        public Customers CreatedBy;
        public Customers ApprovalBy;
        public Customers ManagedBy;
        //public List<NEXMI.MasterManufactureOrderDetails> Details;

        public String WsiError;
        public String Mode;

        public NMManufactureOrdersWSI()
        {
            Filter = new NMFilter();
            this.ManufactureOrder = new ManufactureOrders();

            WsiError = "";
            Mode = "";
        }

        public String CompareTo(ManufactureOrders older)
        {
            String result = "";

            if (this.ManufactureOrder.ProductId != older.ProductId)
                result = "Sản phẩm: " + older.ProductId + " => " + this.ManufactureOrder.ProductId + ";";
            if (this.ManufactureOrder.Quantity != older.Quantity)
                result = "Số lượng: " + older.Quantity + " => " + this.ManufactureOrder.Quantity + ";";
            if (this.ManufactureOrder.Descriptions != older.Descriptions)
                result = "Ghi chú: " + older.Descriptions + " => " + this.ManufactureOrder.Descriptions + ";";
            if (this.ManufactureOrder.StartDate != older.StartDate)
                result = "Ngày bắt đầu: " + older.StartDate + " => " + this.ManufactureOrder.StartDate + ";";
            if (this.ManufactureOrder.EndDate != older.EndDate)
                result = "Ngày kết thúc: " + older.EndDate + " => " + this.ManufactureOrder.EndDate + ";";
            if (this.ManufactureOrder.Status != older.Status)
                result = "Trạng thái: " + older.Status + " => " + this.ManufactureOrder.Status + ";";

            return result;
        }
    }
}
