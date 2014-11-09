// NMProjectTeamsBL.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMProjectTeamsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMProjectTeamsBL()
        {

        }

        public NMProjectTeamsWSI callSingleBL(NMProjectTeamsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMProjectTeamsWSI> callListBL(NMProjectTeamsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMProjectTeamsWSI>();
            }
        }

        public NMProjectTeamsWSI SelectObject(NMProjectTeamsWSI wsi)
        {
            ProjectTeamsAccesser Accesser = new ProjectTeamsAccesser(Session);
            ProjectTeams obj;
            obj = Accesser.GetAllProjectTeamsByID(wsi.ProjectTeam.ProjectId, wsi.ProjectTeam.UserId, true);
            if (obj != null)
            {
                wsi.ProjectTeam = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMProjectTeamsWSI SaveObject(NMProjectTeamsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProjectTeamsAccesser Accesser = new ProjectTeamsAccesser(Session);
                ProjectTeams obj = Accesser.GetAllProjectTeamsByID(wsi.ProjectTeam.ProjectId, wsi.ProjectTeam.UserId, true);
                if (obj == null)
                {
                    wsi.ProjectTeam.CreatedDate = DateTime.Now;
                    wsi.ProjectTeam.CreatedBy = wsi.ActionBy;
                    Accesser.InsertProjectTeam(obj);
                }
                tx.Commit();
                wsi.Id = obj.ProjectId;
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMProjectTeamsWSI DeleteObject(NMProjectTeamsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProjectTeamsAccesser Accesser = new ProjectTeamsAccesser(Session);
                ProjectTeams obj = Accesser.GetAllProjectTeamsByID(wsi.ProjectTeam.ProjectId, wsi.ProjectTeam.UserId, true);
                if (obj != null)
                {
                    Accesser.DeleteProjectTeam(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không thể xóa.";
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";   
                }
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public List<NMProjectTeamsWSI> SearchObject(NMProjectTeamsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.ProjectTeam != null)
            {
                if (!string.IsNullOrEmpty(wsi.ProjectTeam.ProjectId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProjectId = :ProjectId";
                    ListCriteria.Add("ProjectId", wsi.ProjectTeam.ProjectId);
                }
            }
            String strCmd = "SELECT O FROM ProjectTeams AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMProjectTeamsWSI> ListWSI = new List<NMProjectTeamsWSI>();
            ProjectTeamsAccesser Accesser = new ProjectTeamsAccesser(Session);
            IList<ProjectTeams> objs;
            objs = Accesser.GetProjectTeamsByQuery(query, false);
            foreach (ProjectTeams obj in objs)
            {
                wsi = new NMProjectTeamsWSI();
                wsi.ProjectTeam = obj;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
