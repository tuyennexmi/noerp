// NMBanksWSI.cs

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
    public class NMBanksWSI
    {
        //Criterion for search
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public String Keyword { get; set; }
        public String ActionBy { get; set; }

        public Banks Bank { get; set; }
        public Areas Area { get; set; }

        public String WsiError { get; set; }
        public String Mode { get; set; }

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public String SortField { get; set; }
        public String SortOrder { get; set; }

        public NMBanksWSI()
        {
            FromDate = "";
            ToDate = "";
            Keyword = "";

            Bank = new Banks();
            Area = null;

            WsiError = "";
            Mode = "";

            Limit = 0;
            TotalRows = 0;
            PageSize = 0;
            PageNum = 0;
            SortField = "";
            SortOrder = "";
        }
    }
}
