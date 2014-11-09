// ProjectStagesAccesser.cs

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
    public class ProjectStagesAccesser
    {
        ISession session;

        public ProjectStagesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProjectStage(ProjectStages ProjectStageX)
        {
            session.Merge(ProjectStageX);
        }

        public void UpdateProjectStage(ProjectStages ProjectStageX)
        {
            session.Update(ProjectStageX);
        }

        public void DeleteProjectStage(ProjectStages ProjectStageX)
        {
            session.Delete(ProjectStageX);
        }

        public IList<ProjectStages> GetAllProjectStages(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from ProjectStages as b");
            IList<ProjectStages> list = query.List<ProjectStages>();
            if (evict)
            {
                foreach (ProjectStages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProjectStages GetAllProjectStagesByID(String stageId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProjectStages as c where c.StageId = :y");
            query.SetString("y", stageId);
            ProjectStages s = (ProjectStages)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ProjectStages GetAllProjectStagesByID(String projectId, String stageId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProjectStages as c where c.ProjectId = :x and c.StageId = :y");
            query.SetString("x", projectId);
            query.SetString("y", stageId);
            ProjectStages s = (ProjectStages)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProjectStages> GetAllProjectStagesByProjectID(string projectId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from ProjectStages as b where b.ProjectId = :x");
            query.SetString("x", projectId);
            IList<ProjectStages> list = query.List<ProjectStages>();
            if (evict)
            {
                foreach (ProjectStages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProjectStages> GetProjectStagesByQuery(IQuery query, Boolean evict)
        {
            IList<ProjectStages> list = query.List<ProjectStages>();
            if (evict)
            {
                foreach (ProjectStages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProjectStages> GetProjectStagesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<ProjectStages> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<ProjectStages>();
            if (evict)
            {
                foreach (ProjectStages s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
