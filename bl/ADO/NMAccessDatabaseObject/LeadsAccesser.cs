// LeadsAccesser.cs

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
    public class LeadsAccesser
    {
        ISession session;

        public LeadsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertLeads(Leads x)
        {
            session.Merge(x);
        }

        public void UpdateLeads(Leads x)
        {
            session.Update(x);
        }

        public void DeleteLeads(Leads x)
        {
            session.Delete(x);
        }

        public IList<Leads> GetAllLeads(Boolean evict)
        {
            IQuery query = session.CreateQuery("select o from Leads as o");
            IList<Leads> list = query.List<Leads>();
            if (evict)
            {
                foreach (Leads s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Leads GetAllLeadsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Leads as c where c.Id = :x");
            query.SetString("x", id);
            Leads s = (Leads)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Leads> GetLeadsByQuery(IQuery query, Boolean evict)
        {
            IList<Leads> list = query.List<Leads>();
            if (evict)
            {
                foreach (Leads s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Leads> GetLeadsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Leads> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Leads>();
            if (evict)
            {
                foreach (Leads s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
