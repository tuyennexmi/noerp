// ExportsAccesser.cs

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
    public class ExportsAccesser
    {
        ISession session;

        public ExportsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertExports(Exports exportsX)
        {
            session.Merge(exportsX);
        }

        public void UpdateExports(Exports exportsX)
        {
            session.Update(exportsX);
        }

        public void DeleteExports(Exports exportsX)
        {
            session.Delete(exportsX);
        }

        public IList<Exports> GetAllExports(Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from Exports as e");
            IList<Exports> list = query.List<Exports>();
            if (evict)
            {
                foreach (Exports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Exports GetAllExportsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Exports as c where c.ExportId = :x");
            query.SetString("x", id);
            Exports s = (Exports)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Exports> GetExportsByQuery(IQuery query, Boolean evict)
        {
            IList<Exports> list = query.List<Exports>();
            if (evict)
            {
                foreach (Exports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Exports> GetExportsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Exports> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Exports>();
            if (evict)
            {
                foreach (Exports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
