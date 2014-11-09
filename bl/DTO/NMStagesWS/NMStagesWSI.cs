// NMStagesWSI.cs

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
    public class NMStagesWSI
    {
        public string ActionBy { get; set; }
        public string Keyword { get; set; }

        public Stages Stage;
        public Statuses Status;

        public String Mode;
        public String WsiError;

        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;

        public NMStagesWSI()
        {
            ActionBy = "";
            Keyword = "";

            Stage = null;

            Mode = "";
            WsiError = "";
        }
    }
}
