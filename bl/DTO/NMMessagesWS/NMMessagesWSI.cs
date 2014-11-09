// NMMessagesWSI.cs

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
    public class NMMessagesWSI
    {
        public string ActionBy { get; set; }
        public string Keyword { get; set; }
        public string IsRead { get; set; }

        public Messages Message;
        public Customers CreatedBy;

        public String Mode;
        public String WsiError;

        public int Limit { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public NMMessagesWSI()
        {
            CreatedBy = null;
            this.Message = new Messages();

            Mode = "";
            WsiError = "";

            this.Limit = 0;
            this.TotalRows = 0;
            this.PageSize = 0;
            this.PageNum = 0;
            this.SortField = "";
            this.SortOrder = "";
        }
    }

    public static class MessageStatues
    {
        public static string Cancel = "MSG00";
        public static string Pending = "MSG01";
        public static string Sent = "MSG02";
        public static string Read = "MSG03";
    }
}
