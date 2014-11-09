// NMRequirementsWSI.cs

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
    public class NMRequirementsWSI
    {
        public Requirements Requirement;
        public Customers Customer;
        public Customers CreatedBy;
        public Customers ApprovalBy;
        public Customers RequiredBy;

        public List<RequirementDetails> Details;
        public NMFilter Filter;
        public string WsiError { get; set; }
        public string Mode { get; set; }

        public NMRequirementsWSI()
        {
            this.Requirement = new Requirements();
            Filter = new NMFilter();
            WsiError = "";
            Mode = "";
        }


        public string CompareTo(Requirements older)
        {
            string result = "";

            if (this.Requirement.Amount != older.Amount)
                result += "Tổng giá trị: " + older.Amount.ToString("N") + " => " + this.Requirement.Amount.ToString("N") + ";";

            if (this.Requirement.ApprovalBy != older.ApprovalBy)
                result += "Người duyệt: " + older.ApprovalBy + " => " + this.Requirement.ApprovalBy + ";";
            
            if (this.Requirement.CustomerId != older.CustomerId)
                result += "Khách hàng: " + older.CustomerId + " => " + this.Requirement.CustomerId + ";";
            
            if (this.Requirement.RequiredBy != older.RequiredBy)
                result += "Người đề xuất: " + older.RequiredBy + " => " + this.Requirement.RequiredBy + ";";
            
            if (this.Requirement.RequirementTypeId != older.RequirementTypeId)
                result += "Lý do: " + NMCommon.GetTypeName(older.RequirementTypeId) + " => " + NMCommon.GetTypeName(this.Requirement.RequirementTypeId) + ";";
            
            if (this.Requirement.Status != older.Status)
                result += "Trạng thái: " + NMCommon.GetStatusName(older.Status, "vi") + " => " + NMCommon.GetStatusName(this.Requirement.Status, "vi") + ";";

            if (this.Requirement.ResponseDays != older.ResponseDays)
                result += "Thời gian đáp ứng: " + older.ResponseDays + " => " + this.Requirement.ResponseDays + ";";

            if (this.Requirement.OrderId != older.OrderId)
                result += "Đơn hàng/Hợp đồng: " + older.OrderId + " => " + this.Requirement.OrderId + ";";

            if (this.Requirement.RequireDate != older.RequireDate)
                result += "Ngày đề xuất: " + older.RequireDate.ToString("dd-MM-yyyy") + " => " + this.Requirement.RequireDate.ToString("dd-MM-yyyy") + ";";

            if (this.Requirement.Description != older.Description)
                result += "Diễn giải: " + older.Description + " => " + this.Requirement.Description + ";";

            return result;
        }
    }
}
