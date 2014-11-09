// Receipts.cs

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
    public class Receipts
    {
        public virtual String ReceiptId { get; set; }
        public virtual String ReceiptBatchId { get; set; }
        public virtual DateTime ReceiptDate { get; set; }
        public virtual String ReceiptTypeId { get; set; }
        public virtual String CustomerId { get; set; }
        public virtual String StockId { get; set; }
        public virtual String InvoiceId { get; set; }
        public virtual String PaymentMethodId { get; set; }
        public virtual String CurrencyId { get; set; }

        public virtual String Reference { get; set; }

        double _amount;
        public virtual double Amount
        {
            get { return _amount; }
            set { _amount = value; this.Calculation(); }
        }

        double _exchangeRate;
        public virtual double ExchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; this.Calculation(); }
        }

        public virtual double ReceiptAmount { get; set; }
        public virtual String DescriptionInVietnamese { get; set; }
        public virtual String ReceiptStatus { get; set; }
        public virtual String ReceivedBankAccount { get; set; }
        public virtual String ApprovedBy { get; set; }
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        void Calculation()
        {
            this.ReceiptAmount = this.Amount * this.ExchangeRate;
        }

        public Receipts()
        {

        }
    }
}
