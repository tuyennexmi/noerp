﻿// Products.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class Products
    {
        public virtual String ProductId { get; set; }
        public virtual String ProductCode { get; set; }
        public virtual String BarCode { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual String CategoryId { get; set; }
        public virtual String ManufactureId { get; set; }
        public virtual String ProductNameInVietnamese { get; set; }
        public virtual String TypeId { get; set; }
        public virtual String GroupId { get; set; }
        public virtual String ProductUnit { get; set; }
        public virtual String ProductDescriptionInVietnamese { get; set; }
        public virtual double VATRate { get; set; }
        public virtual double ImportTaxRate { get; set; }
        public virtual Boolean? Discontinued { get; set; }
        public virtual Boolean? Highlight { get; set; }
        public virtual Boolean? IsNew { get; set; }
        public virtual Boolean? IsEmpty { get; set; }
        public virtual String ShortDescription { get; set; }
        public virtual String Description { get; set; }
        
        //dùng chỉ vị trí của nó
        public virtual String LocationId { get; set; }
        public virtual String MACAddress { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual String Warranty { get; set; }
        public virtual double CostPrice { get; set; }
        //public virtual double DefaultDiscount { get; set; }
        public virtual ISet ProductBOMs { get; set; }
        public virtual ISet ProductDetailsList { get; set; }

        public Products()
        {

        }
    }
}

