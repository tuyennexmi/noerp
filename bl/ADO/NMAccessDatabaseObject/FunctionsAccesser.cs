// FunctionsAccesser.cs

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
    public class FunctionsAccesser
    {
        ISession session;

        public FunctionsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertFunctions(Functions functionsX)
        {
            session.Merge(functionsX);
        }

        public void UpdateFunctions(Functions functionsX)
        {
            session.Update(functionsX);
        }

        public void DeleteFunctions(Functions functionsX)
        {
            session.Delete(functionsX);
        }

        public IList<Functions> GetAllFunctions(Boolean evict)
        {
            IQuery query = session.CreateQuery("select f from Functions as f ORDER BY O.OrdinalNumber");
            IList<Functions> list = query.List<Functions>();
            if (evict)
            {
                foreach (Functions s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Functions GetAllFunctionsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Functions as c where c.Id = :x");
            query.SetString("x", id);
            Functions s = (Functions)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Functions> GetFunctionsByQuery(IQuery query, Boolean evict)
        {
            IList<Functions> list = query.List<Functions>();
            if (evict)
            {
                foreach (Functions s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
