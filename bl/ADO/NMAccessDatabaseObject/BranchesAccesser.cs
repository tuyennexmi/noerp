// BranchesAccesser.cs

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
    public class BranchesAccesser
    {
        ISession session;

        public BranchesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertBranches(Branches branchesX)
        {
            session.Merge(branchesX);
        }

        public void UpdateBranches(Branches branchesX)
        {
            session.Update(branchesX);
        }

        public void DeleteBranches(Branches branchesX)
        {
            session.Delete(branchesX);
        }

        public IList<Branches> GetAllBranches(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Branches as b");
            IList<Branches> list = query.List<Branches>();
            if (evict)
            {
                foreach (Branches s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Branches GetAllBranchesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Branches as c where c.Id = :x");
            query.SetString("x", id);
            Branches s = (Branches)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Branches> GetBranchesByQuery(IQuery query, Boolean evict)
        {
            IList<Branches> list = query.List<Branches>();
            if (evict)
            {
                foreach (Branches s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
