// Messages.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguy�?n Quang Tuy�?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class Messages
    {
        public virtual String MessageId { get; set; }
        public virtual String MessageName { get; set; }
        public virtual String TypeId { get; set; }
        public virtual String Owner { get; set; } 
        public virtual String ReplyTo { get; set; } //message ID
        public virtual String SendTo { get; set; }  //list user Id
        public virtual String Description { get; set; }
        public virtual Boolean IsRead { get; set; }
        public virtual Boolean AttachFile { get; set; }
        public virtual Boolean Publish { get; set; }
        public virtual String StatusId { get; set; }
        public virtual String CategoryId { get; set; }
        public virtual String GroupId { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String CreatedBy { get; set; }

        public Messages()
        {
            this.IsRead = false;
            this.Publish = false;
            this.AttachFile = false;
        }
    }
}
