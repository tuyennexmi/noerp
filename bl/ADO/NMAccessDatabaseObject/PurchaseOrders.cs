// PurchaseOrders.cs

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
    public class PurchaseOrders
    {
        public virtual String Id { get; set; }
        public virtual String SupplierId { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual String OrderStatus { get; set; }
        
        public virtual String OrderTypeId { get; set; }
        public virtual String Reference { get; set; }
        public virtual String Transportation { get; set; }
        
        public virtual String PaymentTerm { get; set; }
        public virtual Boolean Paid { get; set; }
        public virtual DateTime PaymentDate { get; set; }

        public virtual DateTime DeliveryDate { get; set; }
        public virtual Boolean Delivered { get; set; }

        public virtual String InvoiceControl { get; set; }
        public virtual Boolean InvoiceReceive { get; set; }
        public virtual DateTime InvoiceDate { get; set; }

        public virtual double Amount { get; set; }
        public virtual double Tax { get; set; }
        public virtual double Discount { get; set; }
        public virtual double TotalAmount { get; set; }
        
        public virtual String Description { get; set; }
        public virtual String CarrierId { get; set; }
        
        public virtual String CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual String ApproveBy { get; set; }
        public virtual String ImportStockId { get; set; }
        public virtual String BuyerId { get; set; }
        public virtual String InvoiceTypeId { get; set; }
        public virtual ICollection<PurchaseOrderDetails> OrderDetailsList { get; set; }

        public PurchaseOrders()
        {
            this.OrderDate = DateTime.Today;
            this.InvoiceDate = DateTime.Today.AddDays(7);
            this.DeliveryDate = DateTime.Today.AddDays(7);
            this.PaymentDate = this.DeliveryDate.AddDays(30);            
        }



        //public override string ToString()
        //{
        //    String s = "";
        //    if (id != null)
        //        s += "id : " + id.ToString() + "\n";
        //    else
        //        s += "id : null\n";
        //    if (customerId != null)
        //        s += "customerId : " + customerId.ToString() + "\n";
        //    else
        //        s += "customerId : null\n";
        //    if (createdDate != null)
        //        s += "createdDate : " + createdDate.ToString() + "\n";
        //    else
        //        s += "createdDate : null\n";
        //    if (createdBy != null)
        //        s += "createdBy : " + createdBy.ToString() + "\n";
        //    else
        //        s += "createdBy : null\n";
        //    if (modifiedDate != null)
        //        s += "modifiedDate : " + modifiedDate.ToString() + "\n";
        //    else
        //        s += "modifiedDate : null\n";
        //    if (modifiedBy != null)
        //        s += "modifiedBy : " + modifiedBy.ToString() + "\n";
        //    else
        //        s += "modifiedBy : null\n";
        //    if (orderDate != null)
        //        s += "orderDate : " + orderDate.ToString() + "\n";
        //    else
        //        s += "orderDate : null\n";
        //    if (deliveryDate != null)
        //        s += "deliveryDate : " + deliveryDate.ToString() + "\n";
        //    else
        //        s += "deliveryDate : null\n";
        //    if (ordertypeId != null)
        //        s += "ordertypeId : " + ordertypeId.ToString() + "\n";
        //    else
        //        s += "ordertypeId : null\n";
        //    return s;
        //}
    }
}
