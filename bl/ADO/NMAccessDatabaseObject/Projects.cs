// Projects.cs

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
    public class Projects
    {
        public virtual String ProjectId { get; set; }
        public virtual String ProjectName { get; set; }
        public virtual String CustomerId { get; set; }
        public virtual String StatusId { get; set; }
        public virtual DateTime? PlannedTime { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual String Escalation { get; set; }
        public virtual Boolean TimeSheets { get; set; }
        public virtual String ManagedBy { get; set; }
        public virtual Boolean Task { get; set; }
        public virtual Boolean Issue { get; set; }
        public virtual String Team { get; set; }
        public virtual String Stage { get; set; }

        public virtual double SalesForecast { get; set; }
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        //public virtual ISet Stages { get; set; }
        //public virtual ISet Issues { get; set; }
        //public virtual ISet ProjectTeams { get; set; }

        public Projects()
        {

        }
    }
}
