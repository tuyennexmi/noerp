// Exports.cs

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
    public class Exports
    {
        public virtual String ExportId { get; set; }
        public virtual String ExportBatchId { get; set; }
        public virtual DateTime ExportDate { get; set; }
        public virtual String ExportTypeId { get; set; }
        public virtual String InvoiceId { get; set; }
        public virtual String CustomerId { get; set; }
        public virtual String CarrierId { get; set; }
        public virtual String StockId { get; set; }
        public virtual String ImportStockId { get; set; }
        public virtual String DescriptionInVietnamese { get; set; }
        public virtual String ExportStatus { get; set; }
        public virtual String Reference { get; set; }
        public virtual String BackOrderOf { get; set; }
        public virtual String DeliveryMethod { get; set; }
        public virtual String Transport { get; set; }
        public virtual String InvoiceTypeId { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }
        
        public virtual ISet ExportdetailsList { get; set; }

        public Exports()
        {

        }

        public virtual void Copy(Exports source)
        {
            this.ExportBatchId = source.ExportBatchId;
            this.ExportDate = source.ExportDate;
            this.ExportTypeId = source.ExportTypeId;
            this.InvoiceId = source.InvoiceId;
            this.CustomerId = source.CustomerId;
            this.CarrierId = source.CarrierId;
            this.StockId = source.StockId;
            this.ImportStockId = source.ImportStockId;
            this.DescriptionInVietnamese = source.DescriptionInVietnamese;
            this.ExportStatus = source.ExportStatus;
            this.Reference = source.Reference;
            this.BackOrderOf = source.BackOrderOf;
            this.DeliveryMethod = source.DeliveryMethod;
            this.Transport = source.Transport;
            this.InvoiceTypeId = source.InvoiceTypeId;
        }

        public virtual void CopyFromSO(SalesOrders source)
        {
            // kiem tra lai tren BL cho truong hop ngay thang
            //if (NMCommon.GetSetting("USE_SODATE_FOR_EXPORT_DATE"))
            //    this.ExportDate = source.OrderDate;
            //else
            this.ExportDate = DateTime.Today;
            
            this.CustomerId = source.CustomerId;
            this.Reference = source.OrderId;

            this.StockId = source.StockId;

            this.CarrierId = source.CarrierId;
            this.Transport = source.Transportation;

            this.ExportTypeId = NMConstant.ExportType.ForCustomers;
            this.ExportStatus = NMConstant.EXStatuses.Draft;
            this.DeliveryMethod = source.ShippingPolicy;
            this.InvoiceTypeId = source.InvoiceTypeId;

            this.DescriptionInVietnamese = source.Description;

            //this.BackOrderOf = source.BackOrderOf;
            //this.InvoiceId = source.InvoiceId;
            //this.WSI.Export.ImportStockId = SOWSI.Order.StockId;
        }
    }
}
