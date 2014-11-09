// AutomaticValues.cs

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
    public class AutomaticValues
    {
        public virtual String ObjectName { get; set; }
        public virtual String PrefixOfDefaultValueForId { get; set; }
        public virtual Int32 LengthOfDefaultValueForId { get; set; }
        public virtual String LastValueOfColumnId { get; set; }
        public virtual String DefaultValueForSelection { get; set; }
        
        public virtual Boolean? FullYear { get; set; }
        public virtual String Sign { get; set; }
        public virtual String ResetChar { get; set; }
        public virtual DateTime? ResetDate { get; set; }
        public virtual String DateFormat { get; set; }
        
        //dao trat tu giua so thu tu va tien to
        public virtual Boolean? Invert { get; set; }

        public AutomaticValues()
        {

        }
    }
}
