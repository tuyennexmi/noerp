// MonthlyGeneralJournals.cs

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
    public class MonthlyGeneralJournals
    {
        public virtual int JournalId { get; set; }
        public virtual String CloseMonth { get; set; }
        public virtual String IssueId { get; set; }
        
        public virtual String SIID { get; set; }
        public virtual String EXID { get; set; }
        public virtual String RPTID { get; set; }
        
        public virtual String PIID { get; set; }
        public virtual String IMID { get; set; }
        public virtual String PADID { get; set; }

        public virtual DateTime IssueDate { get; set; }
        public virtual String ActionBy { get; set; }
        public virtual String StockId { get; set; }
        public virtual String PartnerId { get; set; }
        public virtual String Descriptions { get; set; }
        public virtual String AccountId { get; set; }
        public virtual double DebitAmount { get; set; }
        public virtual double CreditAmount { get; set; }
        public virtual String CurrencyId { get; set; }
        public virtual double ExchangeRate { get; set; }
        public virtual String BankId { get; set; }
        
        public virtual bool IsBegin { get; set; }
        public virtual int Status { get; set; }

        public virtual String ProductId { get; set; }
        public virtual double ImportQuantity { get; set; }
        public virtual double ExportQuantity { get; set; }
        public virtual String UnitId { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public MonthlyGeneralJournals()
        {
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }
    }
}