// DailyReportsAccesser.cs

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
    public class DailyReportsAccesser
    {
        ISession session;

        public DailyReportsAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public DailyReportsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertDailyReports(DailyReports DailyReportsX)
        {
            session.Merge(DailyReportsX);
        }

        public void UpdateDailyReports(DailyReports DailyReportsX)
        {
            session.Update(DailyReportsX);
        }

        public void DeleteDailyReports(DailyReports DailyReportsX)
        {
            session.Delete(DailyReportsX);
        }

        public IList<DailyReports> GetAllDailyReports(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from DailyReports as r");
            IList<DailyReports> list = query.List<DailyReports>();
            if (evict)
            {
                foreach (DailyReports s in list)
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

        public DailyReports GetAllDailyReportsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from DailyReports as c where c.Id = :x");
            query.SetString("x", id);
            DailyReports s = (DailyReports)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<DailyReports> GetAllDailyReportsByTaskId(String TaskId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from DailyReports as c where c.TaskId = :x");
            query.SetString("x", TaskId);
            IList<DailyReports> s = query.List<DailyReports>();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<DailyReports> GetDailyReportsByQuery(IQuery query, Boolean evict)
        {
            IList<DailyReports> list = query.List<DailyReports>();
            if (evict)
            {
                foreach (DailyReports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<DailyReports> GetCustomersByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<DailyReports> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<DailyReports>();
            if (evict)
            {
                foreach (DailyReports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
