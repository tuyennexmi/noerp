// Customers.cs

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
    public class Customers
    {
        public virtual String CustomerId { get; set; }
        public virtual String CompanyNameInVietnamese { get; set; }
        public virtual String CompanyNameInSecondLanguage { get; set; }
        public virtual String Address { get; set; }
        public virtual String AreaId { get; set; }
        public virtual String Telephone { get; set; }
        public virtual String Website { get; set; }
        public virtual String FaxNumber { get; set; }
        public virtual String GroupId { get; set; }
        public virtual String TaxCode { get; set; }
        public virtual String MainBusiness { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual String Avatar { get; set; }
        public virtual String ManagedBy { get; set; }
        public virtual int? Gender { get; set; }
        public virtual String CustomerTypeId { get; set; }
        public virtual String EmailAddress { get; set; }
        public virtual String EmailPassword { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual double MaxDebitAmount { get; set; }
        public virtual Boolean Discontinued { get; set; }
        public virtual String Description { get; set; }
        public virtual String KindOfIndustryId { get; set; }
        public virtual String KindOfEnterpriseId { get; set; }
        public virtual String Cellphone { get; set; }
        public virtual String ParentId { get; set; }
        public virtual String JobPosition { get; set; }
        
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual String Code { get; set; }
        public virtual String Password { get; set; }
        public virtual String IdentifyId { get; set; }
        public virtual Boolean IsContact { get; set; }
        
        public virtual String DeparmentId { get; set; }
        public virtual Boolean IsInherit { get; set; }

        public virtual String MainUrl { get; set; }
        public virtual String StockId { get; set; }
        public virtual String ShiftId { get; set; }
        public virtual double MinSales { get; set; }
        
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual int BonusPoints { get; set; }
        
        public Customers()
        {
            this.MaxDebitAmount = 0;
            this.Discontinued = false;
        }

    }
}
