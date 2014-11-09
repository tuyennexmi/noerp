// NMProjectsWSI.cs

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
    public class NMProjectsWSI
    {
        public string ActionBy { get; set; }
        public string Keyword { get; set; }

        public Projects Project;
        public Customers Customer;
        public Customers ManagedBy;
        public Statuses Status;
        public IList<Issues> Issues;
        public IList<Tasks> Tasks;
        public IList<Customers> Teams;
        public IList<Stages> Stages;

        public String Mode;
        public String WsiError;

        public int Limit { get; set; }
        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;

        public NMProjectsWSI()
        {
            Limit = 0;
            Mode = "";
            WsiError = "";
        }

        public String CompareTo(Projects older)
        {
            String result = "";
            if (this.Project.ProjectName != older.ProjectName)
                result = "Tên dự án: " + older.ProjectName + " => " + this.Project.ProjectName + ";";
            if (this.Project.CustomerId != older.CustomerId)
                result += "Khách hàng: " + NMCommon.GetCustomerName(older.CustomerId) + " => " + NMCommon.GetCustomerName(this.Project.CustomerId) + ";";
            if (this.Project.ManagedBy != older.ManagedBy)
                result += "Người quản lý: " + NMCommon.GetCustomerName(older.ManagedBy) + " => " + NMCommon.GetCustomerName(this.Project.ManagedBy) + ";";
            if (this.Project.StatusId != older.StatusId)
                result += "Trạng thái: " + NMCommon.GetStatusName(older.StatusId, "vi") + " => " + NMCommon.GetStatusName(this.Project.StatusId, "vi") + ";";
            return result;
        }
    }
}
