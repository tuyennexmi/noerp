// SystemParamsAccesser.cs

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
    public class SystemParamsAccesser
    {
        ISession session;

        public SystemParamsAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public SystemParamsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertSystemParams(SystemParams SystemParamsX)
        {
            session.Merge(SystemParamsX);
        }

        public void UpdateSystemParams(SystemParams SystemParamsX)
        {
            session.Update(SystemParamsX);
        }

        public void DeleteSystemParams(SystemParams SystemParamsX)
        {
            session.Delete(SystemParamsX);
        }

        public IList<SystemParams> GetAllSystemParams(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from SystemParams as r");
            IList<SystemParams> list = query.List<SystemParams>();
            if (evict)
            {
                foreach (SystemParams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public SystemParams GetAllSystemParamsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from SystemParams as c where c.Id = :x");
            query.SetString("x", id);
            SystemParams s = (SystemParams)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<SystemParams> GetSystemParamsByQuery(IQuery query, Boolean evict)
        {
            IList<SystemParams> list = query.List<SystemParams>();
            if (evict)
            {
                foreach (SystemParams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
