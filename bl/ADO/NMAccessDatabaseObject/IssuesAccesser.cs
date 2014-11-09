// IssuesAccesser.cs

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
    public class IssuesAccesser
    {
        ISession session;

        public IssuesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertIssue(Issues IssueX)
        {
            session.Merge(IssueX);
        }

        public void UpdateIssue(Issues IssueX)
        {
            session.Update(IssueX);
        }

        public void DeleteIssue(Issues IssueX)
        {
            session.Delete(IssueX);
        }

        public IList<Issues> GetAllIssues(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Issues as b");
            IList<Issues> list = query.List<Issues>();
            if (evict)
            {
                foreach (Issues s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Issues GetAllIssuesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Issues as c where c.IssueId = :x");
            query.SetString("x", id);
            Issues s = (Issues)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Issues> GetIssuesByQuery(IQuery query, Boolean evict)
        {
            IList<Issues> list = query.List<Issues>();
            if (evict)
            {
                foreach (Issues s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Issues> GetIssuesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Issues> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Issues>();
            if (evict)
            {
                foreach (Issues s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
