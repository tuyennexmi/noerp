// MessagesAccesser.cs

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
    public class MessagesAccesser
    {
        ISession session;

        public MessagesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMessage(Messages MessageX)
        {
            session.Merge(MessageX);
        }

        public void UpdateMessage(Messages MessageX)
        {
            session.Update(MessageX);
        }

        public void DeleteMessage(Messages MessageX)
        {
            session.Delete(MessageX);
        }

        public IList<Messages> GetAllMessages(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Messages as b");
            IList<Messages> list = query.List<Messages>();
            if (evict)
            {
                foreach (Messages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Messages GetAllMessagesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Messages as c where c.MessageId = :x");
            query.SetString("x", id);
            Messages s = (Messages)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Messages> GetMessagesByQuery(IQuery query, Boolean evict)
        {
            IList<Messages> list = query.List<Messages>();
            if (evict)
            {
                foreach (Messages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Messages> GetMessagesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Messages> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Messages>();
            if (evict)
            {
                foreach (Messages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
