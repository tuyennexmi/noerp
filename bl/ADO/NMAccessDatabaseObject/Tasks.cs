// Tasks.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyê?n Quang Tuyê?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System.Collections.Generic;
using System.Text;
using System;
using Iesi.Collections;

namespace NEXMI
{
    public class Tasks
    {
        public virtual String TaskId { get; set; }
        public virtual String TaskName { get; set; }
        public virtual String JobReference { get; set; }

        public virtual String ProjectId { get; set; }
        public virtual String StageId { get; set; }
        public virtual String Category { get; set; }
        public virtual String AssignedTo { get; set; }
        public virtual DateTime Deadline { get; set; }

        public virtual String Purpose { get; set; }
        public virtual String Criteria { get; set; }

        public virtual String Tags { get; set; }
        public virtual String Priority { get; set; }
        public virtual String ReportPeriod { get; set; }
        public virtual Int16 Sequence { get; set; }
        public virtual String Description { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual String StatusId { get; set; }

        public virtual String CustomerId { get; set; }
        public virtual String CheckedBy { get; set; }
        public virtual String Manager { get; set; }
        public virtual Boolean IsReportTimeRight { get; set; }
        //public virtual String FirstStage { get; set; }

        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String ModifiedBy { get; set; }

        public virtual ICollection<TaskDetails> TaskDetailList { get; set; }

        public Tasks()
        {

        }
    }
}
