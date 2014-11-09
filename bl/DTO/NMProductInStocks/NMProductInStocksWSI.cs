// NMProductInStocksWSI.cs

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
    public class NMProductInStocksWSI
    {
        public NMFilter Filter;
        //Criterion for search
        public string Keyword { get; set; }
        public String ActionBy { get; set; }
        public String CategoryId { get; set; }

        public ProductInStocks PIS;

        public NMProductsWSI ProductWSI;
        public NMStocksWSI StockWSI;
        
        public string WsiError { get; set; }
        public string Mode { get; set; }

        public string LanguageId { get; set; }
        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMProductInStocksWSI()
        {
            Filter = new NMFilter();

            Keyword = "";
            ActionBy = "";
            CategoryId = "";

            PIS = new ProductInStocks();
            ProductWSI = null;
            StockWSI = null;

            WsiError = "";
            Mode = "";

            LanguageId = NMCommon.GetLanguageDefault();
            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }
    }
}
