// ImportDetailsAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguy�?n Quang Tuy�?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class ImportDetailsAccesser
    {
        ISession session;

        public ImportDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertImportdetails(ImportDetails importdetailsX)
        {
            session.Merge(importdetailsX);
        }

        public void UpdateImportdetails(ImportDetails importdetailsX)
        {
            session.Update(importdetailsX);
        }

        public void DeleteImportdetails(ImportDetails importdetailsX)
        {
            session.Delete(importdetailsX);
        }

        public IList<ImportDetails> GetAllImportdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select i from ImportDetails as i");
            IList<ImportDetails> list = query.List<ImportDetails>();
            if (evict)
            {
                foreach (ImportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ImportDetails> GetAllImportdetailsByImportId(String importId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select i from ImportDetails as i where i.ImportId = :y");
            query.SetString("y", importId);
            IList<ImportDetails> list = query.List<ImportDetails>();
            if (evict)
            {
                foreach (ImportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ImportDetails GetAllImportdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ImportDetails as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            ImportDetails s = (ImportDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ImportDetails GetAllImportdetailsByProductIdAndImportId(String productId, String importId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ImportDetails as c where c.ProductId = :x and c.ImportId = :y");
            query.SetString("x", productId);
            query.SetString("y", importId);
            ImportDetails s = (ImportDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ImportDetails> GetImportdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<ImportDetails> list = query.List<ImportDetails>();
            if (evict)
            {
                foreach (ImportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}