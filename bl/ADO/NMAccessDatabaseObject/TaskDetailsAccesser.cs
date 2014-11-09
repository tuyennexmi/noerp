// TaskDetailsAccesser.cs

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
    public class TaskDetailsAccesser
    {
        ISession session;

        public TaskDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertTaskDetail(TaskDetails TaskDetailX)
        {
            session.Merge(TaskDetailX);
        }

        public void UpdateTaskDetail(TaskDetails TaskDetailX)
        {
            session.Update(TaskDetailX);
        }

        public void DeleteTaskDetail(TaskDetails TaskDetailX)
        {
            session.Delete(TaskDetailX);
        }

        public IList<TaskDetails> GetAllTaskDetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from TaskDetails as b");
            IList<TaskDetails> list = query.List<TaskDetails>();
            if (evict)
            {
                foreach (TaskDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public TaskDetails GetAllTaskDetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from TaskDetails as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            TaskDetails s = (TaskDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<TaskDetails> GetTaskDetailsByQuery(IQuery query, Boolean evict)
        {
            IList<TaskDetails> list = query.List<TaskDetails>();
            if (evict)
            {
                foreach (TaskDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<TaskDetails> GetTaskDetailsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<TaskDetails> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<TaskDetails>();
            if (evict)
            {
                foreach (TaskDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
