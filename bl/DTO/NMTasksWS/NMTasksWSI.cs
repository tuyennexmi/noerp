// NMTasksWSI.cs

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
    public class NMTasksWSI
    {
        public string ActionBy { get; set; }
        public string Keyword { get; set; }

        public Tasks Task;
        public List<TaskDetails> Details;
        public Projects Project;
        public Customers AssignedUser;
        public Stages Stage;
        public Parameters Priority;
        public Parameters ReportPeriod;

        public Customers CheckedBy;
        public Customers Manager;

        public List<DailyReports> Reports;

        public String Mode;
        public String WsiError;

        public int Limit;
        public int TotalRows;
        public int PageSize;
        public int PageNum;
        public string SortField;
        public string SortOrder;
        public String FromDate { get; set; }
        public String ToDate { get; set; }

        public NMTasksWSI()
        {
            ActionBy = "";
            Keyword = "";

            Task = null; 
            Details = null;

            Limit = 0;
            Mode = "";
            WsiError = "";
        }

        public String CompareTo(Tasks older)
        {
            String result = "";
            if (this.Task.TaskName != older.TaskName)
                result = "Tên nhiệm vụ: " + older.TaskName + " => " + this.Task.TaskName + ";";
            if (this.Task.Purpose != older.Purpose)
                result += "Mục đích: " + older.Purpose + " => " + this.Task.Purpose + ";";
            if (this.Task.Criteria != older.Criteria)
                result += "Tiêu chí: " + older.Criteria + " => " + this.Task.Criteria + ";";
            if (this.Task.Purpose != older.Purpose)
                result += "Người nhận: " + older.AssignedTo + " => " + this.Task.AssignedTo + ";";
            if (this.Task.CheckedBy != older.CheckedBy)
                result += "Người kiểm: " + older.CheckedBy + " => " + this.Task.CheckedBy + ";";
            if (this.Task.StartDate != older.StartDate)
                result += "Ngày bắt đầu: " + older.StartDate + " => " + this.Task.StartDate + ";";
            if (this.Task.EndDate != older.EndDate)
                result += "Ngày kết thúc: " + older.EndDate + " => " + this.Task.EndDate + ";";
            if (this.Task.Priority != older.Priority)
                result += "Độ ưu tiên: " + older.Priority + " => " + this.Task.Priority + ";";

            return result;
        }
    }
}
