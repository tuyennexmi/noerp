
// NMFunctionsWSI.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEXMI
{
    public class NMFunctionsWSI
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameInEnglish { get; set; }
        public string NameInKorea { get; set; }
        public string ModuleId { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string FormName { get; set; }
        public string ParentId { get; set; }
        public string OrdinalNumber { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Active { get; set; }
        public string Icon { get; set; }
        public string WsiError { get; set; }
        public string Mode { get; set; }

        public NMFunctionsWSI()
        {
            Id = "";
            Name = "";
            NameInEnglish = "";
            ModuleId = "";
            Description = "";
            Action = "";
            ParentId = "";
            OrdinalNumber = "";
            Active = "";
            Icon = "";
            WsiError = "";
            Mode = "";
        }
    }
}
