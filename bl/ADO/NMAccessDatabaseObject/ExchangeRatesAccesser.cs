// ExchangeRatesAccesser.cs

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
    public class ExchangeRatesAccesser
    {
        ISession session;

        public ExchangeRatesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertExchangerate(ExchangeRates exchangerateX)
        {
            session.Merge(exchangerateX);
        }

        public void UpdateExchangerate(ExchangeRates exchangerateX)
        {
            session.Update(exchangerateX);
        }

        public void DeleteExchangerate(ExchangeRates exchangerateX)
        {
            session.Delete(exchangerateX);
        }

        public IList<ExchangeRates> GetAllExchangerate(Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from ExchangeRates as e");
            IList<ExchangeRates> list = query.List<ExchangeRates>();
            if (evict)
            {
                foreach (ExchangeRates s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ExchangeRates GetAllExchangerateByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ExchangeRates as c where c.CurrencyId = :x");
            query.SetString("x", id);
            ExchangeRates s = (ExchangeRates)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ExchangeRates GetAllExchangerateCloset(String currencyId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ExchangeRates as c where c.CurrencyId = :x and c.DateOfRate = (select MAX(DateOfRate) from ExchangeRates as c1 where c1.CurrencyId = :x)");
            query.SetString("x", currencyId);
            ExchangeRates s = (ExchangeRates)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ExchangeRates> GetExchangerateByQuery(IQuery query, Boolean evict)
        {
            IList<ExchangeRates> list = query.List<ExchangeRates>();
            if (evict)
            {
                foreach (ExchangeRates s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
