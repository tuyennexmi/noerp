// Requirements.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguy�?n Quang Tuy�?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class Requirements
    {
        public virtual String Id { get; set; }
        public virtual String OrderId { get; set; }
        public virtual String RequirementTypeId { get; set; }
        public virtual DateTime RequireDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual String Description { get; set; }
        
        public virtual String Status { get; set; }
        public virtual String CustomerId { get; set; }
        public virtual String ResponseDays { get; set; }

        public virtual double Amount { get; set; }
        public virtual String RequiredBy { get; set; }
        public virtual String ApprovalBy { get; set; }

        public virtual ISet RequirementDetailsList { get; set; }

        public Requirements()
        {

        }
    }
}
