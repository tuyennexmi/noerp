// Imports.cs

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
    public class Imports
    {
        public virtual String ImportId { get; set; }
        public virtual String ImportBatchId { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual String ImportTypeId { get; set; }
        public virtual String InvoiceId { get; set; }
        public virtual String SupplierId { get; set; }
        public virtual String CarrierId { get; set; }
        public virtual String StockId { get; set; }
        public virtual String ExportStockId { get; set; }
        public virtual String DescriptionInVietnamese { get; set; }
        public virtual String ImportStatus { get; set; }
        public virtual String Reference { get; set; }
        public virtual String SupplierReference { get; set; }
        public virtual String Transport { get; set; }
        public virtual String DeliveryMethod { get; set; }
        public virtual String BackOrderOf { get; set; }
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual double Amount { get; set; }
        public virtual String InvoiceTypeId { get; set; }

        public virtual ISet ImportDetailsList { get; set; }

        public Imports()
        {   
            
        }

        public virtual void CopyFromPO(PurchaseOrders source)
        {   
            this.ImportDate = DateTime.Today;
            this.ImportTypeId = NMConstant.ImportType.Direct;
            this.ImportStatus = NMConstant.IMStatuses.Draft;
            this.DeliveryMethod = NMConstant.DeliveryMethod.Once;
            this.InvoiceTypeId = source.InvoiceTypeId;

            this.SupplierId = source.SupplierId;
            this.Reference = source.Id;
            this.StockId = source.ImportStockId;
            
            this.DescriptionInVietnamese = source.Description;
            this.SupplierReference = source.Reference;

            this.CarrierId = source.CarrierId;
            this.Transport = source.Transportation;

            //this.ImportBatchId = "";
            //this.InvoiceId = source.Id;
            //this.ExportStockId = "";
        }
    }
}
