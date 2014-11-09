// NMPaymentsWSI.cs

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
    public class NMPaymentsWSI
    {
        //Criterion for search
        public String FromDate;
        public String ToDate;
        public String ActionBy;
        public string Keyword;

        public Payments Payment;
        public Customers Customer;
        public Parameters PaymentMethod;
        public AccountNumbers PaymentType;
        public Banks ReceiveBank;
        public Banks PaymentBank;

        public string WsiError;
        public string Mode;

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMPaymentsWSI()
        {
            FromDate = "";
            ToDate = "";
            ActionBy = "";

            Payment = new Payments();
            Customer = null;
            PaymentMethod = null;
            this.PaymentType = null;

            WsiError = "";
            Mode = "";

            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public string CompareTo(Payments older)
        {
            String result = "";
            if (this.Payment.PaymentDate != older.PaymentDate)
                result = "Ngày: " + older.PaymentDate.ToString("dd/MM/yyyy") + " => " + this.Payment.PaymentDate.ToString("dd/MM/yyyy") + ";";
            if (this.Payment.Amount != older.Amount)
                result += "Tổng tiền: " + older.Amount.ToString() + " => " + this.Payment.Amount.ToString() + ";";
            if (this.Payment.DescriptionInVietnamese != older.DescriptionInVietnamese)
                result += "Diễn giải: " + older.DescriptionInVietnamese + " => " + this.Payment.DescriptionInVietnamese + ";";
            if (this.Payment.PaymentStatus != older.PaymentStatus)
                result += "Trạng thái: " + older.PaymentStatus + " => " + this.Payment.PaymentStatus + ";";
            if (this.Payment.ApprovedBy != older.ApprovedBy)
                result += "Người duyệt: " + older.ApprovedBy + " => " + this.Payment.ApprovedBy + ";";
            if (this.Payment.PaymentMethodId != older.PaymentMethodId)
                result += "Phương thức thanh toán: " + older.PaymentMethodId + " => " + this.Payment.PaymentMethodId + ";";
            if (this.Payment.PaymentBankAccount != older.PaymentBankAccount)
                result += "Tài khoản ngân hàng: " + older.PaymentBankAccount + " => " + this.Payment.PaymentBankAccount + ";";
            if (this.Payment.PaymentTypeId != older.PaymentTypeId)
                result += "Loại phiếu: " + older.PaymentTypeId + " => " + this.Payment.PaymentTypeId + ";";
            if (this.Payment.SupplierId != older.SupplierId)
                result += "Nhà cung cấp: " + older.SupplierId + " => " + this.Payment.SupplierId + ";";

            return result;
        }
    }
}
