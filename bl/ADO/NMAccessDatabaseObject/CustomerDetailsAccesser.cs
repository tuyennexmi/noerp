// CustomerDetailsAccesser.cs

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
using NHibernate;
using NHibernate.Cfg;
using NEXMI;

namespace NEXMI
{
    public class CustomerDetailsAccesser
    {
        ISession session;

        public CustomerDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCustomerdetails(CustomerDetails customerdetailsX)
        {
            session.Merge(customerdetailsX);
        }

        public void UpdateCustomerdetails(CustomerDetails customerdetailsX)
        {
            session.Update(customerdetailsX);
        }

        public void DeleteCustomerdetails(CustomerDetails customerdetailsX)
        {
            session.Delete(customerdetailsX);
        }

        public IList<CustomerDetails> GetAllCustomerdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CustomerDetails as c");
            IList<CustomerDetails> list = query.List<CustomerDetails>();
            if (evict)
            {
                foreach (CustomerDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public CustomerDetails GetAllCustomerdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CustomerDetails as c where c.Id = :x");
            query.SetString("x", id);
            CustomerDetails s = (CustomerDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<CustomerDetails> GetCustomerdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<CustomerDetails> list = query.List<CustomerDetails>();
            if (evict)
            {
                foreach (CustomerDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
