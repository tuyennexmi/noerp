
// NMProductsWSI.cs

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
    public class NMProductsWSI
    {
        //Criterion for search
        public string Keyword { get; set; }
        public String ActionBy { get; set; }

        public Products Product { get; set; }
        public NMCategoriesWSI CategoryWSI { get; set; }
        public Customers Manufacture { get; set; }
        public PricesForSalesInvoice PriceForSales { get; set; }
        public PricesForSalesInvoice CostPrice { get; set; }
        public ProductUnits Unit { get; set; }
        public IList<Images> Images { get; set; }
        public IList<ProductBOMs> BoMs { get; set; }
        public Translations Translation;

        public Locations Location;
        public List<ProductPorts> ProductPorts;

        public string WsiError { get; set; }
        public string Mode { get; set; }
        public string StockId { get; set; }
        public string LanguageId { get; set; }
        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMProductsWSI()
        {
            this.Product = new Products();
            this.Manufacture = null;
            PriceForSales = null;
            
            this.StockId = "";
            Keyword = "";
            WsiError = "";
            Mode = "";

            this.LanguageId = NMCommon.GetLanguageDefault();
            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }

        public String CompareTo(Products older)
        {
            String result = "";

            if (this.Product.ProductNameInVietnamese != older.ProductNameInVietnamese)
                result = "Tên sản phẩm: " + older.ProductNameInVietnamese + " => " + this.Product.ProductNameInVietnamese + ";";
            if (this.Product.ProductCode != older.ProductCode)
                result += "Mã tham chiếu: " + older.ProductCode + " => " + this.Product.ProductCode + ";";
            if (this.Product.BarCode != older.BarCode)
                result += "Mã vạch: " + older.BarCode + " => " + this.Product.BarCode + ";";
            if (this.Product.CategoryId != older.CategoryId)
                result += "Chủng loại: " + older.CategoryId + " => " + this.Product.CategoryId + ";";
            if (this.Product.ManufactureId != older.ManufactureId)
                result += "Nhà sản xuất: " + older.ManufactureId + " => " + this.Product.ManufactureId + ";";
            if (this.Product.ProductUnit != older.ProductUnit)
                result += "Đơn vị tính: " + older.ProductUnit + " => " + this.Product.ProductUnit + ";";            
            if (this.Product.CostPrice != older.CostPrice)
                result += "Giá chi phí: " + older.CostPrice + " => " + this.Product.CostPrice + ";";
            if (this.Product.VATRate != older.VATRate)
                result += "Thuế VAT: " + older.VATRate + " => " + this.Product.VATRate + ";";
            if (this.Product.ImportTaxRate != older.ImportTaxRate)
                result += "Thuế nhập khẩu: " + older.ImportTaxRate + " => " + this.Product.ImportTaxRate + ";";
            if (this.Product.Discontinued != older.Discontinued)
                result += "Ngưng bán: " + older.Discontinued + " => " + this.Product.Discontinued + ";";
            if (this.Product.ShortDescription != older.ShortDescription)
                result += "Mô tả ngắn: " + older.ShortDescription + " => " + this.Product.ShortDescription + ";";
            if (this.Product.Description != older.Description)
                result += "Mô tả: " + older.Description + " => " + this.Product.Description + ";";
            if (this.Product.Warranty != older.Warranty)
                result += "Bảo hành: " + older.Warranty + " => " + this.Product.Warranty + ";";
            
            return result;
        }
    }
}
