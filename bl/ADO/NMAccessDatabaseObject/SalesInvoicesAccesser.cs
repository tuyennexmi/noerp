// SalesInvoicesAccesser.cs

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
    public class SalesInvoicesAccesser
    {
        ISession session;

        public SalesInvoicesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertSalesinvoices(SalesInvoices salesinvoicesX)
        {
            session.Merge(salesinvoicesX);
        }

        public void UpdateSalesinvoices(SalesInvoices salesinvoicesX)
        {
            session.Update(salesinvoicesX);
        }

        public void DeleteSalesinvoices(SalesInvoices salesinvoicesX)
        {
            session.Delete(salesinvoicesX);
        }

        public IList<SalesInvoices> GetAllSalesinvoices(Boolean evict)
        {
            IQuery query = session.CreateQuery("select s from SalesInvoices as s");
            IList<SalesInvoices> list = query.List<SalesInvoices>();
            if (evict)
            {
                foreach (SalesInvoices s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public SalesInvoices GetAllSalesinvoicesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SalesInvoices as c where c.InvoiceId = :x");
            query.SetString("x", id);
            SalesInvoices s = (SalesInvoices)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<SalesInvoices> GetSalesinvoicesByQuery(IQuery query, Boolean evict)
        {
            IList<SalesInvoices> list = query.List<SalesInvoices>();
            if (evict)
            {
                foreach (SalesInvoices s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<SalesInvoices> GetSalesinvoicesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);  
            IList<SalesInvoices> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<SalesInvoices>();
            if (evict)
            {
                foreach (SalesInvoices s in list)
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
