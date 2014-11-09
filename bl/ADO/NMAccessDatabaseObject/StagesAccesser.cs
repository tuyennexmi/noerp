// StagesAccesser.cs

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
    public class StagesAccesser
    {
        ISession session;

        public StagesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertStage(Stages StageX)
        {
            session.Merge(StageX);
        }

        public void UpdateStage(Stages StageX)
        {
            session.Update(StageX);
        }

        public void DeleteStage(Stages StageX)
        {
            session.Delete(StageX);
        }

        public IList<Stages> GetAllStages(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Stages as b");
            IList<Stages> list = query.List<Stages>();
            if (evict)
            {
                foreach (Stages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Stages GetAllStagesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Stages as c where c.StageId = :x");
            query.SetString("x", id);
            Stages s = (Stages)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Stages> GetStagesByQuery(IQuery query, Boolean evict)
        {
            IList<Stages> list = query.List<Stages>();
            if (evict)
            {
                foreach (Stages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Stages> GetStagesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Stages> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Stages>();
            if (evict)
            {
                foreach (Stages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
