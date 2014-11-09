// TimeKeepingAccesser.cs

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

namespace NEXMI
{
    public class TimeKeepingAccesser
    {
        ISession session;

        public TimeKeepingAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertTimeKeeping(TimeKeeping TimeKeepingX)
        {
            session.Merge(TimeKeepingX);
        }

        public void UpdateTimeKeeping(TimeKeeping TimeKeepingX)
        {
            session.Update(TimeKeepingX);
        }

        public void DeleteTimeKeeping(TimeKeeping TimeKeepingX)
        {
            session.Delete(TimeKeepingX);
        }

        public IList<TimeKeeping> GetAllTimeKeeping(Boolean evict)
        {
            IQuery query = session.CreateQuery("Select u from TimeKeeping as u");
            IList<TimeKeeping> list = query.List<TimeKeeping>();
            if (evict)
            {
                foreach (TimeKeeping s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public TimeKeeping GetAllTimeKeepingByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from TimeKeeping as c where c.Id = :x");
            query.SetString("x", id);
            TimeKeeping s = (TimeKeeping)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public TimeKeeping GetTimeKeepingByUserIDAndEndTime(String userid, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from TimeKeeping as c where c.UserId = :x and c.EndTime = :y");
            query.SetString("x", userid);
            query.SetDateTime("y", (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
            TimeKeeping s = (TimeKeeping)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public TimeKeeping GetTimeKeepingLastest(String userid, Boolean evict)
        {
            IQuery query = session.CreateQuery("SELECT TK FROM TimeKeeping AS TK WHERE TK.UserId = :x AND TK.StartTime = (SELECT MAX(TK.StartTime) FROM TimeKeeping AS TK WHERE TK.UserId = :x)");
            query.SetString("x", userid);
            TimeKeeping s = (TimeKeeping)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public TimeKeeping GetTimeKeepingLastestEnd(String userid, Boolean evict)
        {
            IQuery query = session.CreateQuery("SELECT TK FROM TimeKeeping AS TK WHERE TK.UserId = :x AND TK.EndTime = (SELECT MAX(TK.EndTime) FROM TimeKeeping AS TK WHERE TK.UserId = :x)");
            query.SetString("x", userid);
            TimeKeeping s = (TimeKeeping)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<TimeKeeping> GetAllTimeKeepingByDate(DateTime Date, Boolean evict)
        {
            IQuery query = session.CreateQuery("Select u from TimeKeeping as u where u.StartTime = :Date");
            IList<TimeKeeping> list = query.List<TimeKeeping>();
            if (evict)
            {
                foreach (TimeKeeping s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<TimeKeeping> GetUsersByQuery(IQuery query, Boolean evict)
        {
            IList<TimeKeeping> list = query.List<TimeKeeping>();
            if (evict)
            {
                foreach (TimeKeeping s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
