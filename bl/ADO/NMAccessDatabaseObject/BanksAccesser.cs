// BanksAccesser.cs

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
    public class BanksAccesser
    {
        ISession session;

        public BanksAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertBanks(Banks banksX)
        {
            session.Merge(banksX);
        }

        public void UpdateBanks(Banks banksX)
        {
            session.Update(banksX);
        }

        public void DeleteBanks(Banks banksX)
        {
            session.Delete(banksX);
        }

        public IList<Banks> GetAllBanks(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Banks as b");
            IList<Banks> list = query.List<Banks>();
            if (evict)
            {
                foreach (Banks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Banks GetAllBanksByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Banks as c where c.Id = :x");
            query.SetString("x", id);
            Banks s = (Banks)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Banks> GetBanksByQuery(IQuery query, Boolean evict)
        {
            IList<Banks> list = query.List<Banks>();
            if (evict)
            {
                foreach (Banks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Banks> GetBanksByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Banks> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Banks>();
            if (evict)
            {
                foreach (Banks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
