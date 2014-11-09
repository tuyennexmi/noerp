// NMImportsWSI.cs

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
    public class NMImportsWSI
    {
        //Criterion for search
        public String FromDate;
        public String ToDate;
        public string Keyword;
        public string ActionBy { get; set; }

        public Imports Import;
        public List<ImportDetails> Details;
        public Customers Supplier;
        public NMStocksWSI Stock;
        public NMStocksWSI ExportStock;
        public Customers CreatedUser;
        //public Statuses Status;
        public Types ImportType;
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

        public NMImportsWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";

            Import = new Imports();
            Details = null;
            this.Supplier = new Customers();
            Stock = null;
            ExportStock = null;
            ImportType = null;
            Carrier = new Customers();

            WsiError = "";
            Mode = "";

            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(Imports older)
        {
            String result = "";

            if (this.Import.ImportDate != older.ImportDate)
                result = "Ngày nhập: " + older.ImportDate.ToString("dd/MM/yyyy") + " => " + this.Import.ImportDate.ToString("dd/MM/yyyy") + ";";
            if (this.Import.SupplierId != older.SupplierId)
                result += "Nhà cung cấp: " + older.SupplierId + " => " + this.Import.SupplierId + ";";
            if (this.Import.StockId != older.StockId)
                result += "Kho nhập: " + older.StockId + " => " + this.Import.StockId + ";";
            if (this.Import.ExportStockId != older.ExportStockId)
                result += "Kho xuất: " + older.ExportStockId + " => " + this.Import.ExportStockId + ";";
            if (this.Import.DescriptionInVietnamese != older.DescriptionInVietnamese)
                result += "Ghi chú: " + older.DescriptionInVietnamese + " => " + this.Import.DescriptionInVietnamese + ";";
            if (this.Import.ImportStatus != older.ImportStatus)
                result += "Trạng thái: " + older.ImportStatus + " => " + this.Import.ImportStatus + ";";
            if (this.Import.CarrierId != older.CarrierId)
                result += "Nhà vận chuyển: " + older.CarrierId + " => " + this.Import.CarrierId + ";";
            if (this.Import.Reference != older.Reference)
                result += "Số tham chiếu: " + older.Reference + " => " + this.Import.Reference + ";";
            if (this.Import.Transport != older.Transport)
                result += "Phương tiện vận chuyển: " + older.Transport + " => " + this.Import.Transport + ";";
            if (this.Import.DeliveryMethod != older.DeliveryMethod)
                result += "Phương thức vận chuyển: " + older.DeliveryMethod + " => " + this.Import.DeliveryMethod + ";";
            if (this.Import.SupplierReference != older.SupplierReference)
                result += "Tham chiếu của NCC: " + older.SupplierReference + " => " + this.Import.SupplierReference + ";";

            return result;
        }
    }
}
