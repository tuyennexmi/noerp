// CalendarsAccesser.cs

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
    public class CalendarsAccesser
    {
        ISession session;

        public CalendarsAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public CalendarsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCalendars(Calendars CalendarsX)
        {
            session.Merge(CalendarsX);
        }

        public void UpdateCalendars(Calendars CalendarsX)
        {
            session.Update(CalendarsX);
        }

        public void DeleteCalendars(Calendars CalendarsX)
        {
            session.Delete(CalendarsX);
        }

        public IList<Calendars> GetAllCalendars(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Calendars as r");
            IList<Calendars> list = query.List<Calendars>();
            if (evict)
            {
                foreach (Calendars s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Calendars GetAllCalendarsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Calendars as c where c.Id = :x");
            query.SetString("x", id);
            Calendars s = (Calendars)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Calendars> GetCalendarsByQuery(IQuery query, Boolean evict)
        {
            IList<Calendars> list = query.List<Calendars>();
            if (evict)
            {
                foreach (Calendars s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
