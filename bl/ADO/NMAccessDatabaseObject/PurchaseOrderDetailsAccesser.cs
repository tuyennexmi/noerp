// PurchaseOrderDetailsAccesser.cs

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
    public class PurchaseOrderDetailsAccesser
    {
        ISession session;

        public PurchaseOrderDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertOrderdetails(PurchaseOrderDetails orderdetailsX)
        {
            session.Merge(orderdetailsX);
        }

        public void UpdateOrderdetails(PurchaseOrderDetails orderdetailsX)
        {
            session.Update(orderdetailsX);
        }

        public void DeleteOrderdetails(PurchaseOrderDetails orderdetailsX)
        {
            session.Delete(orderdetailsX);
        }

        public IList<PurchaseOrderDetails> GetAllOrderdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from PurchaseOrderDetails as o");
            IList<PurchaseOrderDetails> list = query.List<PurchaseOrderDetails>();
            if (evict)
            {
                foreach (PurchaseOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PurchaseOrderDetails GetAllOrderdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PurchaseOrderDetails as c where c.Id = :x");
            query.SetString("x", id);
            PurchaseOrderDetails s = (PurchaseOrderDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PurchaseOrderDetails> GetAllOrderdetailsByPOID(String poId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from PurchaseOrderDetails as o where o.OrderId = :x");
            query.SetString("x", poId);
            IList<PurchaseOrderDetails> list = query.List<PurchaseOrderDetails>();
            if (evict)
            {
                foreach (PurchaseOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<PurchaseOrderDetails> GetOrderdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<PurchaseOrderDetails> list = query.List<PurchaseOrderDetails>();
            if (evict)
            {
                foreach (PurchaseOrderDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
