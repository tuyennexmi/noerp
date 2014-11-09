// SalesOrders.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyê?n Quang Tuyê?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System.Collections.Generic;
using System.Text;
using System;
using Iesi.Collections;

namespace NEXMI
{
    public class SalesOrders
    {
        //what
        public virtual String OrderId { get; set; }
        public virtual String Reference { get; set; }
        public virtual String OrderTypeId { get; set; }
        public virtual String OrderGroup { get; set; }
        public virtual String Description { get; set; }
        public virtual String Transportation { get; set; }

        //who
        public virtual String CustomerId { get; set; }
        public virtual String SalesPersonId { get; set; }
        public virtual String CarrierId { get; set; }
        public virtual String ApproveBy { get; set; }

        //when
        public virtual DateTime OrderDate { get; set; }
        public virtual DateTime? ExpirationDate { get; set; }
        public virtual DateTime? DeliveryDate { get; set; }
        public virtual int RepairDate { get; set; }
        
        //how
        public virtual String Incoterm { get; set; }
        public virtual String ShippingPolicy { get; set; }
        public virtual String PaymentTerm { get; set; }
        
        public virtual String SalesTeam { get; set; }
        public virtual String CategoryId { get; set; }
        public virtual Boolean Paid { get; set; }
        public virtual Boolean Delivered { get; set; }
        public virtual String OrderStatus { get; set; }
        public virtual String CreateInvoice { get; set; }
        public virtual String PaymentMethod { get; set; }
        public virtual String InvoiceTypeId { get; set; }

        //how much
        public virtual double Amount { get; set; }
        public virtual double Tax { get; set; }
        public virtual double Discount { get; set; }
        public virtual double TotalAmount { get; set; }

        public virtual String Advances { get; set; }

        // when
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        //who make
        public virtual String CreatedBy { get; set; }
        public virtual String ModifiedBy { get; set; }

        // where
        public virtual String StockId { get; set; }
        
        //cho thue
        public virtual double Deposit { get; set; }
        public virtual double SetupFee { get; set; }
        public virtual DateTime? MaintainDate { get; set; }

        public virtual ICollection<SalesOrderDetails> OrderDetailsList { get; set; }

        public SalesOrders()
        {
            OrderDate = DateTime.Today;
        }
    }
}
