// SalesOrdersAccesser.cs

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

namespace NEXMI
{
    public class SalesOrdersAccesser
    {
        ISession session;

        public SalesOrdersAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertOrders(SalesOrders ordersX)
        {
            session.Merge(ordersX);
        }

        public void UpdateOrders(SalesOrders ordersX)
        {
            session.Update(ordersX);
        }

        public void DeleteOrders(SalesOrders ordersX)
        {
            session.Delete(ordersX);
        }

        public IList<SalesOrders> GetAllOrders(Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from SalesOrders as o");
            IList<SalesOrders> list = query.List<SalesOrders>();
            if (evict)
            {
                foreach (SalesOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public SalesOrders GetAllOrdersByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesOrders as c where c.OrderId = :x");
            query.SetString("x", id);
            SalesOrders s = (SalesOrders)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<SalesOrders> GetOrdersByQuery(IQuery query, Boolean evict)
        {
            IList<SalesOrders> list = query.List<SalesOrders>();
            if (evict)
            {
                foreach (SalesOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<SalesOrders> GetOrdersByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum);
            IList<SalesOrders> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<SalesOrders>();
            if (evict)
            {
                foreach (SalesOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
