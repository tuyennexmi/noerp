// NMExportsWSI.cs

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

namespace NEXMI
{
    public class NMExportsWSI
    {
        //Criterion for search
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public string Keyword { get; set; }
        public string ActionBy { get; set; }

        public Exports Export;
        public List<ExportDetails> Details;
        public Customers Customer;
        public NMStocksWSI Stock;
        public NMStocksWSI ImportStock;
        public Customers CreatedBy;
        public Statuses Status;
        public Types ExportType;
        public Customers Carrier;
        public List<ProductSNs> ProductSNs;
        public List<NMProductsWSI> ProductWSIs;
        
        public String WsiError;
        public String Mode;

        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;


        public NMExportsWSI()
        {
            this.Export = new Exports();

            FromDate = "";
            ToDate = "";
            Keyword = "";
            ActionBy = "";

            WsiError = "";
            Mode = "";

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(Exports older)
        {
            String result = "";

            if (this.Export.ExportDate != older.ExportDate)
                result = "Ngày xuất: " + older.ExportDate.ToString("dd/MM/yyyy") + " => " + this.Export.ExportDate.ToString("dd/MM/yyyy") + ";";
            if (this.Export.InvoiceId != older.InvoiceId)
                result += "Thuộc hóa đơn: " + older.InvoiceId + " => " + this.Export.InvoiceId + ";";
            if (this.Export.CustomerId != older.CustomerId)
                result += "Khách hàng: " + older.CustomerId + " => " + this.Export.CustomerId + ";";
            if (this.Export.StockId != older.StockId)
                result += "Kho xuất: " + older.StockId + " => " + this.Export.StockId + ";";
            if (this.Export.ImportStockId != older.ImportStockId)
                result += "Kho nhập: " + older.ImportStockId + " => " + this.Export.ImportStockId + ";";
            if (this.Export.DescriptionInVietnamese != older.DescriptionInVietnamese)
                result += "Ghi chú: " + older.DescriptionInVietnamese + " => " + this.Export.DescriptionInVietnamese + ";";
            if (this.Export.Reference != older.Reference)
                result += "Số tham chiếu: " + older.Reference + " => " + this.Export.Reference + ";";
            if (this.Export.ExportStatus != older.ExportStatus)
                result += "Trạng thái: " + older.ExportStatus + " => " + this.Export.ExportStatus + ";";
            if (this.Export.CarrierId != older.CarrierId)
                result += "Nhà vận chuyển: " + older.CarrierId + " => " + this.Export.CarrierId + ";";
            if (this.Export.Transport != older.Transport)
                result += "Phương tiện vận chuyển: " + older.Transport + " => " + this.Export.Transport + ";";
            if (this.Export.DeliveryMethod != older.DeliveryMethod)
                result += "Phương thức giao nhận: " + older.DeliveryMethod + " => " + this.Export.DeliveryMethod + ";";
            

            return result;
        }
    }
}
