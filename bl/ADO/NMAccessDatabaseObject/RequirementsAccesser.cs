// RequirementsAccesser.cs

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
    public class RequirementsAccesser
    {
        ISession session;

        public RequirementsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertRequirements(Requirements requirementsX)
        {
            session.Merge(requirementsX);
        }

        public void UpdateRequirements(Requirements requirementsX)
        {
            session.Update(requirementsX);
        }

        public void DeleteRequirements(Requirements requirementsX)
        {
            session.Delete(requirementsX);
        }

        public IList<Requirements> GetAllRequirements(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Requirements as r");
            IList<Requirements> list = query.List<Requirements>();
            if (evict)
            {
                foreach (Requirements s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Requirements GetAllRequirementsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Requirements as c where c.Id = :x");
            query.SetString("x", id);
            Requirements s = (Requirements)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Requirements> GetRequirementsByQuery(IQuery query, Boolean evict)
        {
            IList<Requirements> list = query.List<Requirements>();
            if (evict)
            {
                foreach (Requirements s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Requirements> GetRequirementsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Requirements> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Requirements>();
            if (evict)
            {
                foreach (Requirements s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
