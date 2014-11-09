// PurchaseOrdersAccesser.cs

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
    public class PurchaseOrdersAccesser
    {
        ISession session;

        public PurchaseOrdersAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertOrders(PurchaseOrders ordersX)
        {
            session.Merge(ordersX);
        }

        public void UpdateOrders(PurchaseOrders ordersX)
        {
            session.Update(ordersX);
        }

        public void DeleteOrders(PurchaseOrders ordersX)
        {
            session.Delete(ordersX);
        }

        public IList<PurchaseOrders> GetAllOrders(Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from PurchaseOrders as o");
            IList<PurchaseOrders> list = query.List<PurchaseOrders>();
            if (evict)
            {
                foreach (PurchaseOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PurchaseOrders GetAllOrdersByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PurchaseOrders as c where c.Id = :x");
            query.SetString("x", id);
            PurchaseOrders s = (PurchaseOrders)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PurchaseOrders> GetOrdersByQuery(IQuery query, Boolean evict)
        {
            IList<PurchaseOrders> list = query.List<PurchaseOrders>();
            if (evict)
            {
                foreach (PurchaseOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<PurchaseOrders> GetOrdersByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<PurchaseOrders> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<PurchaseOrders>();
            if (evict)
            {
                foreach (PurchaseOrders s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
