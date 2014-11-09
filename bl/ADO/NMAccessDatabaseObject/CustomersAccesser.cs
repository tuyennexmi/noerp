// CustomersAccesser.cs

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
    public class CustomersAccesser
    {
        ISession session;

        public CustomersAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public CustomersAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCustomers(Customers customersX)
        {
            session.Merge(customersX);
        }

        public void UpdateCustomers(Customers customersX)
        {
            session.Update(customersX);
        }

        public void DeleteCustomers(Customers customersX)
        {
            session.Delete(customersX);
        }

        public bool CheckExist(string id)
        {
            IQuery query = session.CreateSQLQuery("select COUNT(c.CustomerId) from Customers as c where c.CustomerId = :x");
            query.SetString("x", id);
            int count = 0;
            try
            {
                count = (int)query.UniqueResult();
            }
            catch { }
            if (count != 0)
                return true;
            return false;
        }

        public IList<Customers> GetAllCustomers(Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Customers as c");
            IList<Customers> list = query.List<Customers>();
            if (evict)
            {
                foreach (Customers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Customers> GetAllCustomersByParentId(string parentId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Customers as c where c.ParentId = :x");
            query.SetString("x", parentId);
            IList<Customers> list = query.List<Customers>();
            if (evict)
            {
                foreach (Customers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Customers GetAllCustomersByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Customers as c where c.CustomerId = :x");
            query.SetString("x", id);
            Customers s = (Customers)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public int Counter(IQuery query)
        {
            int count = 0;
            try
            {
                count = (int)query.UniqueResult();
            }
            catch { }
            return count;
        }

        public IList<Customers> GetCustomersByQuery(IQuery query, Boolean evict)
        {
            IList<Customers> list = query.List<Customers>();
            if (evict)
            {
                foreach (Customers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Customers> GetCustomersByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);  
            IList<Customers> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Customers>();
            if (evict)
            {
                foreach (Customers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
