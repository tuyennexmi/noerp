// PurchaseInvoicesAccesser.cs

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
    public class PurchaseInvoicesAccesser
    {
        ISession session;

        public PurchaseInvoicesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertPurchaseinvoices(PurchaseInvoices purchaseinvoicesX)
        {
            session.Merge(purchaseinvoicesX);
        }

        public void UpdatePurchaseinvoices(PurchaseInvoices purchaseinvoicesX)
        {
            session.Update(purchaseinvoicesX);
        }

        public void DeletePurchaseinvoices(PurchaseInvoices purchaseinvoicesX)
        {
            session.Delete(purchaseinvoicesX);
        }

        public IList<PurchaseInvoices> GetAllPurchaseinvoices(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from PurchaseInvoices as p");
            IList<PurchaseInvoices> list = query.List<PurchaseInvoices>();
            if (evict)
            {
                foreach (PurchaseInvoices s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PurchaseInvoices GetAllPurchaseinvoicesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PurchaseInvoices as c where c.InvoiceId = :x");
            query.SetString("x", id);
            PurchaseInvoices s = (PurchaseInvoices)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PurchaseInvoices> GetPurchaseinvoicesByQuery(IQuery query, Boolean evict)
        {
            IList<PurchaseInvoices> list = query.List<PurchaseInvoices>();
            if (evict)
            {
                foreach (PurchaseInvoices s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<PurchaseInvoices> GetPurchaseinvoicesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<PurchaseInvoices> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<PurchaseInvoices>();
            if (evict)
            {
                foreach (PurchaseInvoices s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
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
    }
}
