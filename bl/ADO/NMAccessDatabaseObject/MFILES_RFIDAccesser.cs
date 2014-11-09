// MFILES_RFIDAccesser.cs

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
    public class MFILES_RFIDAccesser
    {
        ISession session;

        public MFILES_RFIDAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertDocument(MFILES_RFID DocumentX)
        {
            session.Merge(DocumentX);
        }

        public void UpdateDocument(MFILES_RFID DocumentX)
        {
            session.Update(DocumentX);
        }

        public void DeleteDocument(MFILES_RFID DocumentX)
        {
            session.Delete(DocumentX);
        }

        public IList<MFILES_RFID> GetAllMFILES_RFID(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from MFILES_RFID as b");
            IList<MFILES_RFID> list = query.List<MFILES_RFID>();
            if (evict)
            {
                foreach (MFILES_RFID s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MFILES_RFID GetAllMFILES_RFIDByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MFILES_RFID as c where c.Object_ID = :x");
            query.SetString("x", id);
            MFILES_RFID s = (MFILES_RFID)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public MFILES_RFID GetAllMFILES_RFIDByDocumentId(String documentId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MFILES_RFID as c where c.Document_ID = :x");
            query.SetString("x", documentId);
            MFILES_RFID s = (MFILES_RFID)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public MFILES_RFID GetAllMFILES_RFIDByRFID(String rfid, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MFILES_RFID as c where c.RFID_NO = :x");
            query.SetString("x", rfid);
            MFILES_RFID s = (MFILES_RFID)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MFILES_RFID> GetMFILES_RFIDByQuery(IQuery query, Boolean evict)
        {
            IList<MFILES_RFID> list = query.List<MFILES_RFID>();
            if (evict)
            {
                foreach (MFILES_RFID s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<MFILES_RFID> GetMFILES_RFIDByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<MFILES_RFID> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<MFILES_RFID>();
            if (evict)
            {
                foreach (MFILES_RFID s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}

