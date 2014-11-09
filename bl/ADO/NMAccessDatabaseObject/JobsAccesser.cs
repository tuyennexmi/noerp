// JobsAccesser.cs

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
    public class JobsAccesser
    {
        ISession session;

        public JobsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertJob(Jobs JobX)
        {
            session.Merge(JobX);
        }

        public void UpdateJob(Jobs JobX)
        {
            session.Update(JobX);
        }

        public void DeleteJob(Jobs JobX)
        {
            session.Delete(JobX);
        }

        public IList<Jobs> GetAllJobs(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Jobs as b");
            IList<Jobs> list = query.List<Jobs>();
            if (evict)
            {
                foreach (Jobs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Jobs GetAllJobsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Jobs as c where c.Id = :x");
            query.SetString("x", id);
            Jobs s = (Jobs)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Jobs> GetJobsByQuery(IQuery query, Boolean evict)
        {
            IList<Jobs> list = query.List<Jobs>();
            if (evict)
            {
                foreach (Jobs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Jobs> GetJobsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Jobs> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Jobs>();
            if (evict)
            {
                foreach (Jobs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
