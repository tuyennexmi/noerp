// ManufactureOrders.cs

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
using Iesi.Collections;
using System.Text;

namespace NEXMI
{
    public class ManufactureOrders
    {
        public virtual String Id { get; set; }
        public virtual String ProductId { get; set; }
        public virtual String CustomerId { get; set; }

        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }

        public virtual String Descriptions { get; set; }
        public virtual String Status { get; set; }
        public virtual String Priority { get; set; }

        public virtual String TypeId { get; set; }
        public virtual String SourceDocument { get; set; }
        public virtual String SaleReference { get; set; }

        public virtual double Quantity { get; set; }
        public virtual double LaborCosts { get; set; }
        public virtual double Consumption { get; set; }
        public virtual double OtherCosts { get; set; }
        public virtual double TotalAmount { get; set; }

        public virtual String ApprovalBy { get; set; }
        public virtual String ManagedBy { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual ICollection<MOMaterialDetails> MaterialDetails { get; set; }

        public ManufactureOrders()
        {
        }
    }
}