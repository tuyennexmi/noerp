// ReceiptsAccesser.cs

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
    public class ReceiptsAccesser
    {
        ISession session;

        public ReceiptsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertReceipts(Receipts receiptsX)
        {
            session.Merge(receiptsX);
        }

        public void UpdateReceipts(Receipts receiptsX)
        {
            session.Update(receiptsX);
        }

        public void DeleteReceipts(Receipts receiptsX)
        {
            session.Delete(receiptsX);
        }

        public IList<Receipts> GetAllReceipts(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Receipts as r");
            IList<Receipts> list = query.List<Receipts>();
            if (evict)
            {
                foreach (Receipts s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Receipts GetAllReceiptsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Receipts as c where c.ReceiptId = :x");
            query.SetString("x", id);
            Receipts s = (Receipts)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Receipts> GetReceiptsByQuery(IQuery query, Boolean evict)
        {
            IList<Receipts> list = query.List<Receipts>();
            if (evict)
            {
                foreach (Receipts s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Receipts> GetReceiptsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Receipts> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Receipts>();
            if (evict)
            {
                foreach (Receipts s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
