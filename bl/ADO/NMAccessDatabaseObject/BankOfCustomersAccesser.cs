// BankOfCustomersAccesser.cs

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
    public class BankOfCustomersAccesser
    {
        ISession session;

        public BankOfCustomersAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertBankofcustomers(BankOfCustomers bankofcustomersX)
        {
            session.Merge(bankofcustomersX);
        }

        public void UpdateBankofcustomers(BankOfCustomers bankofcustomersX)
        {
            session.Update(bankofcustomersX);
        }

        public void DeleteBankofcustomers(BankOfCustomers bankofcustomersX)
        {
            session.Delete(bankofcustomersX);
        }

        public IList<BankOfCustomers> GetAllBankofcustomers(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from BankOfCustomers as b");
            IList<BankOfCustomers> list = query.List<BankOfCustomers>();
            if (evict)
            {
                foreach (BankOfCustomers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public BankOfCustomers GetAllBankofcustomersByID(String bankId, string customerId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from BankOfCustomers as c where c.BankId = :x and c.CustomerId = :y");
            query.SetString("x", bankId);
            query.SetString("y", customerId);
            BankOfCustomers s = (BankOfCustomers)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<BankOfCustomers> GetBankofcustomersByQuery(IQuery query, Boolean evict)
        {
            IList<BankOfCustomers> list = query.List<BankOfCustomers>();
            if (evict)
            {
                foreach (BankOfCustomers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<BankOfCustomers> GetBankofcustomersByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<BankOfCustomers> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<BankOfCustomers>();
            if (evict)
            {
                foreach (BankOfCustomers s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
