// DocumentCheckingAccesser.cs

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
using NEXMI;

namespace NEXMI
{
    public class DocumentCheckingAccesser
    {
        ISession session;

        public DocumentCheckingAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertDocumentChecking(DocumentChecking documentCheckingX)
        {
            session.Merge(documentCheckingX);
        }

        public void UpdateDocumentChecking(DocumentChecking documentCheckingX)
        {
            session.Update(documentCheckingX);
        }

        public void DeleteDocumentChecking(DocumentChecking documentCheckingX)
        {
            session.Delete(documentCheckingX);
        }

        public IList<DocumentChecking> GetAllDocumentChecking(Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from DocumentChecking as e");
            IList<DocumentChecking> list = query.List<DocumentChecking>();
            if (evict)
            {
                foreach (DocumentChecking s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public DocumentChecking GetAllDocumentCheckingByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from DocumentChecking as c where c.Id = :x");
            query.SetString("x", id);
            DocumentChecking s = (DocumentChecking)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<DocumentChecking> GetAllDocumentCheckingByDocument(String documentId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from DocumentChecking as e where e.DocumentId = :x");
            query.SetString("x", documentId);
            IList<DocumentChecking> list = query.List<DocumentChecking>();
            if (evict)
            {
                foreach (DocumentChecking s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public DocumentChecking GetAllDocumentCheckingCloset(String currencyId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from DocumentChecking as c where c.Id = :x and c.Time = (select MAX(Time) from DocumentChecking as c1 where c1.Id = :x)");
            query.SetString("x", currencyId);
            DocumentChecking s = (DocumentChecking)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<DocumentChecking> GetDocumentCheckingByQuery(IQuery query, Boolean evict)
        {
            IList<DocumentChecking> list = query.List<DocumentChecking>();
            if (evict)
            {
                foreach (DocumentChecking s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
