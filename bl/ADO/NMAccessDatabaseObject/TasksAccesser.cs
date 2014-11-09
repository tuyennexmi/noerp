// TasksAccesser.cs

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
    public class TasksAccesser
    {
        ISession session;

        public TasksAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertTask(Tasks TaskX)
        {
            session.Merge(TaskX);
        }

        public void UpdateTask(Tasks TaskX)
        {
            session.Update(TaskX);
        }

        public void DeleteTask(Tasks TaskX)
        {
            session.Delete(TaskX);
        }

        public IList<Tasks> GetAllTasks(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Tasks as b");
            IList<Tasks> list = query.List<Tasks>();
            if (evict)
            {
                foreach (Tasks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Tasks GetAllTasksByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Tasks as c where c.TaskId = :x");
            query.SetString("x", id);
            Tasks s = (Tasks)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Tasks> GetTasksByQuery(IQuery query, Boolean evict)
        {
            IList<Tasks> list = query.List<Tasks>();
            if (evict)
            {
                foreach (Tasks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Tasks> GetTasksByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Tasks> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Tasks>();
            if (evict)
            {
                foreach (Tasks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
