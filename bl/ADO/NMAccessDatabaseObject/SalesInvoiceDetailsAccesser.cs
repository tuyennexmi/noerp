// SalesInvoiceDetailsAccesser.cs

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
    public class SalesInvoiceDetailsAccesser
    {
        ISession session;

        public SalesInvoiceDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertSalesinvoicedetails(SalesInvoiceDetails salesinvoicedetailsX)
        {
            session.Merge(salesinvoicedetailsX);
        }

        public void UpdateSalesinvoicedetails(SalesInvoiceDetails salesinvoicedetailsX)
        {
            session.Update(salesinvoicedetailsX);
        }

        public void DeleteSalesinvoicedetails(SalesInvoiceDetails salesinvoicedetailsX)
        {
            session.Delete(salesinvoicedetailsX);
        }

        public IList<SalesInvoiceDetails> GetAllSalesinvoicedetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select s from SalesInvoiceDetails as s");
            IList<SalesInvoiceDetails> list = query.List<SalesInvoiceDetails>();
            if (evict)
            {
                foreach (SalesInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public SalesInvoiceDetails GetAllSalesinvoicedetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesInvoiceDetails as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            SalesInvoiceDetails s = (SalesInvoiceDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<SalesInvoiceDetails> GetAllSalesinvoicedetailsByInvoiceId(String invoiceId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesInvoiceDetails as c where c.InvoiceId = :x");
            query.SetString("x", invoiceId);
            IList<SalesInvoiceDetails> list = query.List<SalesInvoiceDetails>();
            if (evict)
            {
                foreach (SalesInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<SalesInvoiceDetails> GetSalesinvoicedetailsByQuery(IQuery query, Boolean evict)
        {
            IList<SalesInvoiceDetails> list = query.List<SalesInvoiceDetails>();
            if (evict)
            {
                foreach (SalesInvoiceDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
