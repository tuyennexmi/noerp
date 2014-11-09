
// NMCustomersWSI.cs

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
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMCustomersWSI
    {
        //Criterion for search
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }
        public string TypeName { get; set; }

        public Customers Customer { get; set; }
        public Customers ManagedBy { get; set; }
        public NMAreasWSI AreaWSI { get; set; }
        public List<CustomerDetails> Details { get; set; }
        public IList<Customers> ContactPersons { get; set; }
        public List<NMSalesInvoicesWSI> SalesInvoicesWSI { get; set; }
        public List<NMPurchaseInvoicesWSI> PurchaseInvoicesWSI { get; set; }
        public NMStocksWSI DefaultStock { get; set; }

        public string WsiError { get; set; }
        public string Mode { get; set; }

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMCustomersWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";
            ActionBy = "";
            TypeName = "";

            Customer = new Customers();
            ManagedBy = null;
            Details = null;
            ContactPersons = null;
            SalesInvoicesWSI = null;
            PurchaseInvoicesWSI = null;

            WsiError = "";
            Mode = "";

            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(Customers older)
        {
            String result = "";
            if (this.Customer.CompanyNameInVietnamese != older.CompanyNameInVietnamese)
                result = "Tên: " + older.CompanyNameInVietnamese + " => " + this.Customer.CompanyNameInVietnamese + ";";
            if (this.Customer.TaxCode != older.TaxCode)
                result += "MST: " + older.TaxCode + " => " + this.Customer.TaxCode + ";";
            if (this.Customer.Address != older.Address)
                result += "Địa chỉ: " + older.Address + " => " + this.Customer.Address + ";";
            if (this.Customer.Website != older.Website)
                result += "Website: " + older.Website + " => " + this.Customer.Website + ";";
            if (this.Customer.Telephone != older.Telephone)
                result += "Điện thoại: " + older.Telephone + " => " + this.Customer.Telephone + ";";
            if (this.Customer.Cellphone != older.Cellphone)
                result += "Số di động: " + older.Cellphone + " => " + this.Customer.Cellphone + ";";
            if (this.Customer.FaxNumber != older.FaxNumber)
                result += "Fax: " + older.FaxNumber + " => " + this.Customer.FaxNumber + ";";
            if (this.Customer.EmailAddress != older.EmailAddress)
                result += "Email: " + older.EmailAddress + " => " + this.Customer.EmailAddress + ";";
            if (this.Customer.Description != older.Description)
                result += "Ghi chú: " + older.Description + " => " + this.Customer.Description + ";";

            return result;
        }
    }
}
