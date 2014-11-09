// RequirementDetails.cs

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
    public class RequirementDetails
    {
        public virtual int Id { get; set; }
        public virtual String RequirementId { get; set; }
        public virtual String Description { get; set; }
        public virtual String ProductId { get; set; }
        double _quantity;
        public virtual double Quantity 
        {
            get { return _quantity; }
            set { _quantity = value; this.Calculation(); }
        }
        double _price;
        public virtual double Price 
        {
            get { return _price; }
            set { _price = value; this.Calculation(); }
        }

        public virtual double Amount
        {
            get;
            set;
        }

        void Calculation()
        {
            this.Amount = this.Quantity * this.Price;
        }

        public RequirementDetails()
        {

        }
    }
}
