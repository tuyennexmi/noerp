// ImportsAccesser.cs

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
    public class ImportsAccesser
    {
        ISession session;

        public ImportsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertImports(Imports importsX)
        {
            session.Merge(importsX);
        }

        public void UpdateImports(Imports importsX)
        {
            session.Update(importsX);
        }

        public void DeleteImports(Imports importsX)
        {
            session.Delete(importsX);
        }

        public IList<Imports> GetAllImports(Boolean evict)
        {
            IQuery query = session.CreateQuery("select i from Imports as i");
            IList<Imports> list = query.List<Imports>();
            if (evict)
            {
                foreach (Imports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Imports GetAllImportsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Imports as c where c.ImportId = :x");
            query.SetString("x", id);
            Imports s = (Imports)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Imports> GetImportsByQuery(IQuery query, Boolean evict)
        {
            IList<Imports> list = query.List<Imports>();
            if (evict)
            {
                foreach (Imports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Imports> GetImportsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Imports> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Imports>();
            if (evict)
            {
                foreach (Imports s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
