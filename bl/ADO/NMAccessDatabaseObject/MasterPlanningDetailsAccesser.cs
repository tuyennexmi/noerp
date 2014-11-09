// MasterPlanningDetailsAccesser.cs

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
    public class MasterPlanningDetailsAccesser
    {
        ISession session;

        public MasterPlanningDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMasterPlanningDetails(MasterPlanningDetails MasterPlanningDetailsX)
        {
            session.Merge(MasterPlanningDetailsX);
        }

        public void UpdateMasterPlanningDetails(MasterPlanningDetails MasterPlanningDetailsX)
        {
            session.Update(MasterPlanningDetailsX);
        }

        public void DeleteMasterPlanningDetails(MasterPlanningDetails MasterPlanningDetailsX)
        {
            session.Delete(MasterPlanningDetailsX);
        }

        public IList<MasterPlanningDetails> GetAllMasterPlanningDetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MasterPlanningDetails as m");
            IList<MasterPlanningDetails> list = query.List<MasterPlanningDetails>();
            if (evict)
            {
                foreach (MasterPlanningDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MasterPlanningDetails GetAllMasterPlanningDetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MasterPlanningDetails as c where c.Id = :x");
            query.SetString("x", id);
            MasterPlanningDetails s = (MasterPlanningDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public MasterPlanningDetails GetAllMasterPlanningDetailsByAccountIdAndIssueId(String AccountId, String IssueId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MasterPlanningDetails as c where c.AccountId = :x and c.IssueId = :y");
            query.SetString("x", AccountId);
            query.SetString("y", IssueId);
            MasterPlanningDetails s = (MasterPlanningDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MasterPlanningDetails> GetMasterPlanningDetailsByQuery(IQuery query, Boolean evict)
        {
            IList<MasterPlanningDetails> list = query.List<MasterPlanningDetails>();
            if (evict)
            {
                foreach (MasterPlanningDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
