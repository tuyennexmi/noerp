// StatusesAccesser.cs

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
    public class StatusesAccesser
    {
        ISession session;

        public StatusesAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public StatusesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertStatuses(Statuses StatusesX)
        {
            session.Merge(StatusesX);
        }

        public void UpdateStatuses(Statuses StatusesX)
        {
            session.Update(StatusesX);
        }

        public void DeleteStatuses(Statuses StatusesX)
        {
            session.Delete(StatusesX);
        }

        public IList<Statuses> GetAllStatuses(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Statuses as r");
            IList<Statuses> list = query.List<Statuses>();
            if (evict)
            {
                foreach (Statuses s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Statuses GetAllStatusesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Statuses as c where c.Id = :x");
            query.SetString("x", id);
            Statuses s = (Statuses)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Statuses> GetStatusesByQuery(IQuery query, Boolean evict)
        {
            IList<Statuses> list = query.List<Statuses>();
            if (evict)
            {
                foreach (Statuses s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
