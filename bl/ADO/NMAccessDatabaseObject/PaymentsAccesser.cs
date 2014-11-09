// PaymentsAccesser.cs

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
    public class PaymentsAccesser
    {
        ISession session;

        public PaymentsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertPayments(Payments paymentsX)
        {
            session.Merge(paymentsX);
        }

        public void UpdatePayments(Payments paymentsX)
        {
            session.Update(paymentsX);
        }

        public void DeletePayments(Payments paymentsX)
        {
            session.Delete(paymentsX);
        }

        public IList<Payments> GetAllPayments(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from Payments as p");
            IList<Payments> list = query.List<Payments>();
            if (evict)
            {
                foreach (Payments s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Payments GetAllPaymentsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Payments as c where c.PaymentId = :x");
            query.SetString("x", id);
            Payments s = (Payments)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Payments> GetPaymentsByQuery(IQuery query, Boolean evict)
        {
            IList<Payments> list = query.List<Payments>();
            if (evict)
            {
                foreach (Payments s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Payments> GetPaymentsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Payments> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Payments>();
            if (evict)
            {
                foreach (Payments s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
