// SalesOrderDetails.cs

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
    public class SalesOrderDetails
    {
        public virtual int Id { get; set; }
        public virtual String OrderId { get; set; }
        public virtual String ProductId { get; set; }
        public virtual String UnitId { get; set; }
        double _Quantity;
        public virtual double Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; CalculateAmount(); }
        }
        double _Price;
        public virtual double Price
        {
            get { return _Price; }
            set { _Price = value; CalculateAmount(); }
        }
        double _Tax;
        public virtual double Tax
        {
            get { return _Tax; }
            set { _Tax = value; CalculateAmount(); }
        }
        public virtual double TaxAmount { get; set; }
        double _Discount;
        public virtual double Discount
        {
            get { return _Discount; }
            set { _Discount = value; CalculateAmount(); }
        }
        public virtual double DiscountAmount { get; set; }
        public virtual double Amount { get; set; }
        public virtual double TotalAmount { get; set; }
        public virtual String Description { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        protected void CalculateAmount()
        {
            this.Amount = this.Quantity * this.Price;
            this.DiscountAmount = this.Amount * this.Discount / 100;
            this.TaxAmount = (this.Amount - this.DiscountAmount) * this.Tax / 100;
            this.TotalAmount = this.Amount - this.DiscountAmount + this.TaxAmount;
        }

        public SalesOrderDetails()
        {

        }
    }
}
