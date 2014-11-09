// SalesInvoices.cs

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
    public class SalesInvoices
    {
        public virtual String InvoiceId { get; set; }
        public virtual String InvoiceBatchId { get; set; }
        public virtual DateTime InvoiceDate { get; set; }
        public virtual String InvoiceTypeId { get; set; }
        public virtual String CustomerId { get; set; }
        public virtual String CurrencyId { get; set; }
        public virtual double ExchangeRate { get; set; }
        public virtual String DescriptionInVietnamese { get; set; }
        public virtual String InvoiceStatus { get; set; }
        //public virtual double ShipCost { get; set; }
        //public virtual double OtherCost { get; set; }
        public virtual String SalesPersonId { get; set; }
        public virtual String Reference { get; set; }
        public virtual String VoucherCode { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual String StockId { get; set; }
        
        public virtual double Amount { get; set; }
        public virtual double Tax { get; set; }
        public virtual double Discount { get; set; }
        public virtual double TotalAmount { get; set; }

        public virtual String SourceDocument { get; set; }
        public virtual String PaymentMethod { get; set; }
        public virtual String BankAccount { get; set; }
        public virtual String ApprovalBy { get; set; }

        public virtual ICollection<SalesInvoiceDetails> DetailsList { get; set; }
        public virtual ICollection<Exports> ExportList { get; set; }
        public virtual ICollection<Receipts> ReceiptList { get; set; }
        public virtual ICollection<Payments> PaymentList { get; set; }

        
        public SalesInvoices()
        {

        }

        public virtual void CopyFromSO(SalesOrders source)
        {   
            this.InvoiceDate = DateTime.Today;
                        
            this.CustomerId = source.CustomerId;

            this.CurrencyId = "VND";
            this.ExchangeRate = 1;
            
            this.DescriptionInVietnamese = source.Description;
            this.InvoiceStatus = NMConstant.SIStatuses.Draft;
            this.PaymentMethod = source.PaymentMethod;
            this.InvoiceTypeId = source.InvoiceTypeId;

            this.SalesPersonId = source.SalesPersonId;
            this.Reference = source.OrderId;
            this.StockId = source.StockId;
            this.SourceDocument = source.Reference + "; " + source.Transportation;
            
            this.ApprovalBy = source.ApproveBy;

            //this.VoucherCode = source
            //this.BankAccount = source
            //this.InvoiceBatchId = source
        }

        public virtual void CopyFromExport(Exports source)
        {
            this.InvoiceDate = DateTime.Today;

            this.CustomerId = source.CustomerId;

            this.CurrencyId = "VND";
            this.ExchangeRate = 1;

            this.DescriptionInVietnamese = source.DescriptionInVietnamese;
            this.InvoiceStatus = NMConstant.SIStatuses.Draft;
            //this.PaymentMethod = source.PaymentMethod;
            this.InvoiceTypeId = source.InvoiceTypeId;

            //this.SalesPersonId = source.SalesPersonId;
            this.Reference = source.ExportId;
            this.StockId = source.StockId;
            this.SourceDocument = source.Reference + "; " + source.Transport;

            //this.ApprovalBy = source.ApproveBy;
        }
    }
}
