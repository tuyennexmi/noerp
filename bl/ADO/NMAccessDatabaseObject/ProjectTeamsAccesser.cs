// ProjectTeamsAccesser.cs

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
    public class ProjectTeamsAccesser
    {
        ISession session;

        public ProjectTeamsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProjectTeam(ProjectTeams ProjectTeamX)
        {
            session.Merge(ProjectTeamX);
        }

        public void UpdateProjectTeam(ProjectTeams ProjectTeamX)
        {
            session.Update(ProjectTeamX);
        }

        public void DeleteProjectTeam(ProjectTeams ProjectTeamX)
        {
            session.Delete(ProjectTeamX);
        }

        public IList<ProjectTeams> GetAllProjectTeams(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from ProjectTeams as b");
            IList<ProjectTeams> list = query.List<ProjectTeams>();
            if (evict)
            {
                foreach (ProjectTeams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProjectTeams GetAllProjectTeamsByID(String projectId, string userId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProjectTeams as c where c.ProjectId = :x and c.UserId = :y");
            query.SetString("x", projectId);
            query.SetString("y", userId);
            ProjectTeams s = (ProjectTeams)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProjectTeams> GetAllProjectTeamsByProjectID(string projectId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from ProjectTeams as b where b.ProjectId = :x");
            query.SetString("x", projectId);
            IList<ProjectTeams> list = query.List<ProjectTeams>();
            if (evict)
            {
                foreach (ProjectTeams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProjectTeams> GetProjectTeamsByQuery(IQuery query, Boolean evict)
        {
            IList<ProjectTeams> list = query.List<ProjectTeams>();
            if (evict)
            {
                foreach (ProjectTeams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProjectTeams> GetProjectTeamsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<ProjectTeams> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<ProjectTeams>();
            if (evict)
            {
                foreach (ProjectTeams s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
