// MonthlyInventoryControl.cs

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
    public class MonthlyInventoryControl
    {
        public virtual int Id { get; set; }
        public virtual String ProductId { get; set; }
        public virtual String StockId { get; set; }
        public virtual double BeginQuantity { get; set; }
        double _ImportQuantity;
        public virtual double ImportQuantity {
            get { return _ImportQuantity; }
            set { _ImportQuantity = value; CalculateEndQuantity(); } 
        }
        double _ExportQuantity;
        public virtual double ExportQuantity {
            get { return _ExportQuantity; }
            set { _ExportQuantity = value; CalculateEndQuantity(); }
        }
        double _SalesQuantity;
        public virtual double SalesQuantity {
            get { return _SalesQuantity; }
            set { _SalesQuantity = value; CalculateEndQuantity(); }
        }
        double _OutQuantity;
        public virtual double OutQuantity {
            get { return _OutQuantity; }
            set { _OutQuantity = value; CalculateEndQuantity(); }
        }
        public virtual double EndQuantity { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }

        protected void CalculateEndQuantity()
        {
            this.EndQuantity = this.BeginQuantity + this.ImportQuantity - this.OutQuantity - this.SalesQuantity - this.ExportQuantity;
        }

        public MonthlyInventoryControl()
        {

        }
    }
}
