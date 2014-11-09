// DocumentsAccesser.cs

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
    public class DocumentsAccesser
    {
        ISession session;

        public DocumentsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertDocument(Documents DocumentX)
        {
            session.Merge(DocumentX);
        }

        public void UpdateDocument(Documents DocumentX)
        {
            session.Update(DocumentX);
        }

        public void DeleteDocument(Documents DocumentX)
        {
            session.Delete(DocumentX);
        }

        public IList<Documents> GetAllDocuments(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Documents as b");
            IList<Documents> list = query.List<Documents>();
            if (evict)
            {
                foreach (Documents s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Documents GetAllDocumentsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Documents as c where c.DocumentId = :x");
            query.SetString("x", id);
            Documents s = (Documents)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public Documents GetAllDocumentsByRFID(String rfid, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Documents as c where c.PredefinedRFID = :x");
            query.SetString("x", rfid);
            Documents s = (Documents)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Documents> GetDocumentsByQuery(IQuery query, Boolean evict)
        {
            IList<Documents> list = query.List<Documents>();
            if (evict)
            {
                foreach (Documents s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Documents> GetDocumentsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Documents> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Documents>();
            if (evict)
            {
                foreach (Documents s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}

