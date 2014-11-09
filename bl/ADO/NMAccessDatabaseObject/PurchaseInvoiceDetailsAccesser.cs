// PurchaseInvoiceDetailsAccesser.cs

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
    public class PurchaseInvoiceDetailsAccesser
    {
        ISession session;

        public PurchaseInvoiceDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertPurchaseinvoicedetails(PurchaseInvoiceDetails purchaseinvoicedetailsX)
        {
            session.Merge(purchaseinvoicedetailsX);
        }

        public void UpdatePurchaseinvoicedetails(PurchaseInvoiceDetails purchaseinvoicedetailsX)
        {
            session.Update(purchaseinvoicedetailsX);
        }

        public void DeletePurchaseinvoicedetails(PurchaseInvoiceDetails purchaseinvoicedetailsX)
        {
            session.Delete(purchaseinvoicedetailsX);
        }

        public IList<PurchaseInvoiceDetails> GetAllPurchaseinvoicedetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from PurchaseInvoiceDetails as p");
            IList<PurchaseInvoiceDetails> list = query.List<PurchaseInvoiceDetails>();
            if (evict)
            {
                foreach (PurchaseInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PurchaseInvoiceDetails GetAllPurchaseinvoicedetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PurchaseInvoiceDetails as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            PurchaseInvoiceDetails s = (PurchaseInvoiceDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PurchaseInvoiceDetails> GetAllPurchaseinvoicedetailsByInvoiceId(String invoiceId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PurchaseInvoiceDetails as c where c.InvoiceId = :x");
            query.SetString("x", invoiceId);
            IList<PurchaseInvoiceDetails> list = query.List<PurchaseInvoiceDetails>();
            if (evict)
            {
                foreach (PurchaseInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<PurchaseInvoiceDetails> GetPurchaseinvoicedetailsByQuery(IQuery query, Boolean evict)
        {
            IList<PurchaseInvoiceDetails> list = query.List<PurchaseInvoiceDetails>();
            if (evict)
            {
                foreach (PurchaseInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
