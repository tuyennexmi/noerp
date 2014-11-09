﻿// NMCloseMonthsWSI.cs

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
    public class NMCloseMonthsWSI
    {
        public NMFilter Filter;
        public String AccountId;

        public CloseMonths CloseMonth;
        public List<MonthlyGeneralJournals> Details;

        public List<ProductInStocks> PIS;

        public String WsiError;
        public String Mode;

        public NMCloseMonthsWSI()
        {
            this.Filter = new NMFilter();
            this.CloseMonth = new CloseMonths();

            WsiError = ""; ;
            Mode = "";
        }

        public string CompareTo(NMCloseMonthsWSI older)
        {
            String result = "";

            if (this.CloseMonth.Descriptions != older.CloseMonth.Descriptions)
                result = "Ghi chú: " + older.CloseMonth.Descriptions + " => " + this.CloseMonth.Descriptions + ";";
            
            return result;
        }
    }
}
