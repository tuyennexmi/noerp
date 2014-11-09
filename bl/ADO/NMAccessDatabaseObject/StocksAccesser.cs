// StocksAccesser.cs

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
    public class StocksAccesser
    {
        ISession session;

        public StocksAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertStocks(Stocks stocksX)
        {
            session.Merge(stocksX);
        }

        public void UpdateStocks(Stocks stocksX)
        {
            session.Update(stocksX);
        }

        public void DeleteStocks(Stocks stocksX)
        {
            session.Delete(stocksX);
        }

        public IList<Stocks> GetAllStocks(Boolean evict)
        {
            IQuery query = session.CreateQuery("select s from Stocks as s");
            IList<Stocks> list = query.List<Stocks>();
            if (evict)
            {
                foreach (Stocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Stocks GetAllStocksByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Stocks as c where c.Id = :x");
            query.SetString("x", id);
            Stocks s = (Stocks)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Stocks> GetStocksByQuery(IQuery query, Boolean evict)
        {
            IList<Stocks> list = query.List<Stocks>();
            if (evict)
            {
                foreach (Stocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Stocks> GetStocksByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Stocks> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Stocks>();
            if (evict)
            {
                foreach (Stocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
