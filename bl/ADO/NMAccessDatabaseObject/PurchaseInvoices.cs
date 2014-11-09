// PurchaseInvoices.cs

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
    public class PurchaseInvoices
    {
        public virtual String InvoiceId { get; set; }
        public virtual String InvoiceBatchId { get; set; }
        public virtual DateTime InvoiceDate { get; set; }
        public virtual String InvoiceTypeId { get; set; }
        public virtual String SupplierId { get; set; }
        public virtual String CurrencyId { get; set; }
        public virtual double ExchangeRate { get; set; }
        public virtual String DescriptionInVietnamese { get; set; }
        public virtual String InvoiceStatus { get; set; }
        public virtual double ShipCost { get; set; }
        public virtual double OtherCost { get; set; }
        
        double _tax = 0;
        public virtual double Tax
        { 
            get { return _tax; }
            set { _tax = value; CalculateAmount(); }
        }
        double _discount = 0;
        public virtual double Discount 
        {
            get { return _discount; }
            set
            {
                _discount = value; this.CalculateAmount();
            }
        }
        double _amount = 0;
        public virtual double Amount 
        {
            get { return _amount; }
            set
            {
                _amount = value; this.CalculateAmount();
            }
        }
        public virtual double TotalAmount { get; set; }
        public virtual String Reference { get; set; }
        public virtual String SupplierReference { get; set; }

        public virtual String StockId { get; set; }
        public virtual String BuyerId { get; set; }
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual ICollection<PurchaseInvoiceDetails> DetailsList { get; set; }

        public virtual ICollection<Receipts> ReceiptList { get; set; }
        public virtual ICollection<Payments> PaymentList { get; set; }

        protected void CalculateAmount()
        {
            this.TotalAmount = this.Amount - this.Discount + this.Tax;
        }

        public PurchaseInvoices()
        {

        }

        public virtual void CopyFromPO(PurchaseOrders source)
        {
            this.InvoiceDate = DateTime.Today;
            this.InvoiceStatus = NMConstant.SIStatuses.Draft;

            this.SupplierId = source.SupplierId;
            this.Reference = source.Id;
            this.BuyerId = source.BuyerId;
            this.SupplierReference = source.Reference + "; " + source.Transportation;

            this.StockId = source.ImportStockId;

            this.DescriptionInVietnamese = source.Description;

            this.CurrencyId = "VND";
            this.ExchangeRate = 1;

            this.InvoiceTypeId = source.InvoiceTypeId;

            //this.ApprovalBy = source.ApproveBy;
        }
    }
}
