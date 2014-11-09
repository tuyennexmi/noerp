// NMReceiptsWSI.cs

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
    public class NMReceiptsWSI
    {
        //Criterion for search
        public String FromDate;
        public String ToDate;
        public String ActionBy;
        public string Keyword;

        public Customers Customer;
        public Receipts Receipt;
        public Parameters PaymentMethod;
        public AccountNumbers PaymentType;
        public Banks ReceiveBank;
        
        public string WsiError;
        public string Mode;

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMReceiptsWSI()
        {
            FromDate = "";
            ToDate = "";
            ActionBy = "";

            Customer = null;
            Receipt = new Receipts();
            PaymentMethod = null;
            PaymentType = null;

            WsiError = "";
            Mode = "";

            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(Receipts older)
        {
            String result = "";

            if (this.Receipt.ReceiptDate != older.ReceiptDate)
                result = "Ngày: " + older.ReceiptDate.ToString("dd/MM/yyyy") + " => " + this.Receipt.ReceiptDate.ToString("dd/MM/yyyy") + ";";
            if (this.Receipt.Amount != older.Amount)
                result += "Tổng tiền: " + older.Amount.ToString() + " => " + this.Receipt.Amount.ToString() + ";";
            if (this.Receipt.DescriptionInVietnamese != older.DescriptionInVietnamese)
                result += "Diễn giải: " + older.DescriptionInVietnamese + " => " + this.Receipt.DescriptionInVietnamese + ";";
            if (this.Receipt.ReceiptStatus != older.ReceiptStatus)
                result += "Trạng thái: " + older.ReceiptStatus + " => " + this.Receipt.ReceiptStatus + ";";
            if (this.Receipt.ApprovedBy != older.ApprovedBy)
                result += "Người duyệt: " + older.ApprovedBy + " => " + this.Receipt.ApprovedBy + ";";
            if (this.Receipt.PaymentMethodId != older.PaymentMethodId)
                result += "Phương thức thanh toán: " + older.PaymentMethodId + " => " + this.Receipt.PaymentMethodId + ";";
            if (this.Receipt.ReceivedBankAccount != older.ReceivedBankAccount)
                result += "Tài khoản ngân hàng: " + older.ReceivedBankAccount + " => " + this.Receipt.ReceivedBankAccount + ";";
            if (this.Receipt.ReceiptTypeId != older.ReceiptTypeId)
                result += "Loại phiếu: " + older.ReceiptTypeId + " => " + this.Receipt.ReceiptTypeId + ";";
            if (this.Receipt.CustomerId != older.CustomerId)
                result += "Khách hàng: " + older.CustomerId + " => " + this.Receipt.CustomerId + ";";

            return result;
        }
    }
}
