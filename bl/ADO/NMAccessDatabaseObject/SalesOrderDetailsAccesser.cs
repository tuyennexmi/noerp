// SalesOrderDetailsAccesser.cs

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
using NHibernate;

namespace NEXMI
{
    public class SalesOrderDetailsAccesser
    {
        ISession session;

        public SalesOrderDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertOrderdetails(SalesOrderDetails orderdetailsX)
        {
            session.Merge(orderdetailsX);
        }

        public void UpdateOrderdetails(SalesOrderDetails orderdetailsX)
        {
            session.Update(orderdetailsX);
        }

        public void DeleteOrderdetails(SalesOrderDetails orderdetailsX)
        {
            session.Delete(orderdetailsX);
        }

        public IList<SalesOrderDetails> GetAllOrderdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from SalesOrderDetails as o");
            IList<SalesOrderDetails> list = query.List<SalesOrderDetails>();
            if (evict)
            {
                foreach (SalesOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public SalesOrderDetails GetAllOrderdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesOrderDetails as c where c.Id = :x");
            query.SetString("x", id);
            SalesOrderDetails s = (SalesOrderDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<SalesOrderDetails> GetAllOrderdetailsBySOID(String soId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesOrderDetails as c where c.OrderId = :x");
            query.SetString("x", soId);
            IList<SalesOrderDetails> list = query.List<SalesOrderDetails>();
            if (evict)
            {
                foreach (SalesOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<SalesOrderDetails> GetOrderdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<SalesOrderDetails> list = query.List<SalesOrderDetails>();
            if (evict)
            {
                foreach (SalesOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
