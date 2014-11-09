// Permissions.cs

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
    public class Permissions
    {
        public virtual int Id { get; set; }
        public virtual String FunctionId { get; set; }
        public virtual String UserGroupId { get; set; }
        public virtual String UserId { get; set; }
        public virtual String PSelect { get; set; }
        public virtual String PInsert { get; set; }
        public virtual String PUpdate { get; set; }
        public virtual String Reconcile { get; set; }
        public virtual String PDelete { get; set; }
        public virtual String Approval { get; set; }
        public virtual String ViewAll { get; set; }
        public virtual String Calculate { get; set; }
        public virtual String History { get; set; }

        public virtual String ViewPrice { get; set; }
        public virtual String Export { get; set; }
        public virtual String PPrint { get; set; }
        public virtual String Duplicate { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public Permissions()
        {

        }
    }
}
