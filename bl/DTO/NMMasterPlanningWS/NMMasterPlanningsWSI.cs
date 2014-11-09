// NMMasterPlanningsWSI.cs

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
    public class NMMasterPlanningsWSI
    {
        public NMFilter Filter;
        public string CategoryId;

        public NEXMI.MasterPlannings  Planning;
        //public List<NEXMI.MasterPlanningDetails> Details;

        public String WsiError;
        public String Mode;

        public NMMasterPlanningsWSI()
        {
            Filter = new NMFilter();
            this.Planning = new MasterPlannings();

            WsiError = "";
            Mode = "";
        }

        public String CompareTo(MasterPlannings older)
        {
            String result = "";

            if (this.Planning.Title != older.Title)
                result = "Tiêu đề: " + older.Title + " => " + this.Planning.Title + ";";
            if (this.Planning.Descriptions != older.Descriptions)
                result = "Ghi chú: " + older.Descriptions + " => " + this.Planning.Descriptions + ";";
            if (this.Planning.BeginDate != older.BeginDate)
                result = "Ngày bắt đầu: " + older.BeginDate + " => " + this.Planning.BeginDate + ";";
            if (this.Planning.EndDate != older.EndDate)
                result = "Ngày kết thúc: " + older.EndDate + " => " + this.Planning.EndDate + ";";
            if (this.Planning.Status != older.Status)
                result = "Trạng thái: " + older.Status + " => " + this.Planning.Status + ";";

            return result;
        }
    }
}
