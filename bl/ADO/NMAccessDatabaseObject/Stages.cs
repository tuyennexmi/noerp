// Stages.cs

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
    public class Stages
    {
        public virtual String StageId { get; set; }
        public virtual String StageName { get; set; }
        public virtual String RelatedStatus { get; set; }
        public virtual int Sequence { get; set; }
        public virtual Boolean IsCommon { get; set; }
        public virtual Boolean IsHide { get; set; }
        public virtual Boolean Folded { get; set; }
        public virtual String TypeId { get; set; }
        public virtual String Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }
        //public virtual ISet Tasks { get; set; }
        //public virtual ISet Issues { get; set; }
        public virtual double Probability { get; set; }
        public virtual Boolean Change { get; set; }
        public virtual Boolean DefaultNewSale { get; set; }

        public Stages()
        {

        }
    }
}
