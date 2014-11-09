
// NMFilter.cs

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

namespace NEXMI
{
    public class NMFilter
    {
        public string Keyword { get; set; }

        //for permission viewall true => ""; permission viewall false => "UserId"; 
        public string ActionBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string Type { get; set; }
        public string Statuses { get; set; }
        public string Categories { get; set; }

        public string Owner { get; set; }

        public string LanguageId { get; set; }

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int TotalPages { get; set; }

        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public string GroupBy { get; set; }

        public NMFilter()
        {
            this.Keyword = "";
            this.ActionBy = "";
            this.FromDate = "";
            this.ToDate = "";
            this.Type = "";
            this.Statuses = "";
            this.Categories = "";
            this.Owner = "";
            this.Limit = 0;
            this.TotalRows = 0;
            this.PageNum = 0;
            this.PageSize = 0;
            this.TotalPages = 0;
            this.SortField = "";
            this.SortOrder = "";
        }
    }
}
