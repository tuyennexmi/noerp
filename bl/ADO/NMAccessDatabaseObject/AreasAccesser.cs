// AreasAccesser.cs

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
    public class AreasAccesser
    {
        ISession session;

        public AreasAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertAreas(Areas areasX)
        {
            session.Merge(areasX);
        }

        public void UpdateAreas(Areas areasX)
        {
            session.Update(areasX);
        }

        public void DeleteAreas(Areas areasX)
        {
            session.Delete(areasX);
        }

        public IList<Areas> GetAllAreas(Boolean evict)
        {
            IQuery query = session.CreateQuery("select f from Areas as f order by f.Name");
            IList<Areas> list = query.List<Areas>();
            if (evict)
            {
                foreach (Areas s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Areas GetAllAreasByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Areas as c where c.Id = :x");
            query.SetString("x", id);
            Areas s = (Areas)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Areas> GetAreasByQuery(IQuery query, Boolean evict)
        {
            IList<Areas> list = query.List<Areas>();
            if (evict)
            {
                foreach (Areas s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Areas> GetAreasByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Areas> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Areas>();
            if (evict)
            {
                foreach (Areas s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
