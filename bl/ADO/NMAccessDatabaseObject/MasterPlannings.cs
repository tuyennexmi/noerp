// MasterPlannings.cs

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
    public class MasterPlannings
    {
        public virtual String Id { get; set; }

        public virtual DateTime BeginDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual String Title { get; set; }
        public virtual String Descriptions { get; set; }
        public virtual String Status { get; set; }

        public virtual double SalesAmount { get; set; }
        public virtual double PurchaseAmount { get; set; }
        public virtual double InventoryAmount { get; set; }
        public virtual double CostAmount { get; set; }
        public virtual double ProfitAmount { get; set; }
        public virtual double MoneyAmount { get; set; }        
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual ICollection<MasterPlanningDetails> Details { get; set; }

        public MasterPlannings()
        {
        }
    }
}