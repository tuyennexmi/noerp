// NMPermissionsWSI.cs

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
using System.Collections;
using System.Linq;
using System.Text;

namespace NEXMI
{
    public class NMPermissionsWSI
    {
        public String Id;
        public String FunctionId;
        public String UserGroupId;
        public String UserId;
        public String PSelect;
        public String PInsert;
        public String PUpdate;
        public String Reconcile;
        public String PDelete;
        public String Approval;
        public String ViewAll;
        public String Calculate;
        public String History;

        public String ViewPrice;
        public String Export;
        public String PPrint;
        public String Duplicate;        
        
        public String CreatedDate;
        public String CreatedBy;
        
        public String WsiError;
        public String IsWSIError;
        public String Mode;

        public NMPermissionsWSI()
        {
            Id = "";
            FunctionId = "";
            UserId = "";
            PSelect = "";
            PInsert = "";
            PUpdate = "";
            Reconcile = "";
            PDelete = "";
            Approval = "";
            ViewAll = "";
            Calculate = "";
            History = "";
            CreatedDate = "";
            CreatedBy = "";

            this.ViewPrice = "";
            this.Export = "";
            PPrint = "";
            Duplicate = "";

            WsiError = "";
            IsWSIError = "";
            Mode = "";
        }
    }
}
