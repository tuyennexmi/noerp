// MasterPlanningsAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class MasterPlanningsAccesser
    {
        ISession session;

        public MasterPlanningsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMasterPlannings(MasterPlannings MasterPlanningsX)
        {
            session.Merge(MasterPlanningsX);
        }

        public void UpdateMasterPlannings(MasterPlannings MasterPlanningsX)
        {
            session.Update(MasterPlanningsX);
        }

        public void DeleteMasterPlannings(MasterPlannings MasterPlanningsX)
        {
            session.Delete(MasterPlanningsX);
        }

        public IList<MasterPlannings> GetAllMasterPlannings(Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MasterPlannings as m");
            IList<MasterPlannings> list = query.List<MasterPlannings>();
            if (evict)
            {
                foreach (MasterPlannings s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MasterPlannings GetAllMasterPlanningsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MasterPlannings as c where c.Id = :x");
            query.SetString("x", id);
            MasterPlannings s = (MasterPlannings)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public MasterPlannings GetAllMasterPlanningsByAccountIdAndIssueId(String AccountId, String IssueId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MasterPlannings as c where c.AccountId = :x and c.IssueId = :y");
            query.SetString("x", AccountId);
            query.SetString("y", IssueId);
            MasterPlannings s = (MasterPlannings)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MasterPlannings> GetMasterPlanningsByQuery(IQuery query, Boolean evict)
        {
            IList<MasterPlannings> list = query.List<MasterPlannings>();
            if (evict)
            {
                foreach (MasterPlannings s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
