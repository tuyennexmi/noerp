// AccountNumbers.cs

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
    public class AccountNumbers
    {
        public virtual String Id { get; set; }
        public virtual String ParentId { get; set; }
        public virtual String AccountTypeId { get; set; }
        public virtual String NameInVietnamese { get; set; }
        public virtual String NameInSecondLanguage { get; set; }
        public virtual String ForPayment { get; set; }
        public virtual String ForInvoice { get; set; }
        public virtual String ForReceipt { get; set; }
        public virtual String Descriptions { get; set; }
        public virtual Boolean IsDiscontinued { get; set; }
        public virtual Boolean? NoBalances { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }

        //public virtual ISet SubAccountNumbersList { get; set; }

        public AccountNumbers()
        {

        }
    }
}