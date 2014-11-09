// NMStagesBL.cs

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
    public class NMStagesBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMStagesBL()
        {

        }

        public NMStagesWSI callSingleBL(NMStagesWSI wsi)
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

        public List<NMStagesWSI> callListBL(NMStagesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMStagesWSI>();
            }
        }

        public NMStagesWSI SelectObject(NMStagesWSI wsi)
        {
            StagesAccesser Accesser = new StagesAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            Stages obj = Accesser.GetAllStagesByID(wsi.Stage.StageId, false);
            if (obj != null)
            {
                wsi.Stage = obj;
                wsi.Status = StatusAccesser.GetAllStatusesByID(obj.RelatedStatus, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMStagesWSI SaveObject(NMStagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                StagesAccesser Accesser = new StagesAccesser(Session);
                Stages obj = Accesser.GetAllStagesByID(wsi.Stage.StageId, true);
                if (obj != null)
                {
                    Accesser.UpdateStage(wsi.Stage);
                }
                else
                {
                    wsi.Stage.StageId = AutomaticValueAccesser.AutoGenerateId("Stages");
                    wsi.Stage.CreatedDate = DateTime.Now;
                    wsi.Stage.CreatedBy = wsi.ActionBy;
                    Accesser.InsertStage(wsi.Stage);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMStagesWSI DeleteObject(NMStagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                StagesAccesser Accesser = new StagesAccesser(Session);
                Stages obj = Accesser.GetAllStagesByID(wsi.Stage.StageId, true);
                if (obj != null)
                {
                    Accesser.DeleteStage(obj);
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

        public List<NMStagesWSI> SearchObject(NMStagesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Stage != null)
            {
                if (!string.IsNullOrEmpty(wsi.Stage.TypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TypeId = :TypeId";
                    ListCriteria.Add("TypeId", wsi.Stage.TypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Stage.RelatedStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.RelatedStatus = :RelatedStatus";
                    ListCriteria.Add("RelatedStatus", wsi.Stage.RelatedStatus);
                }
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.StageId LIKE :Keyword OR O.StageName LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.StageId) FROM Stages AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.StageId ASC";
            }
            String strCmd = "SELECT O FROM Stages AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMStagesWSI> ListWSI = new List<NMStagesWSI>();
            StagesAccesser Accesser = new StagesAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            IList<Stages> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetStagesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetStagesByQuery(query, false);
            }
            foreach (Stages obj in objs)
            {
                wsi = new NMStagesWSI();
                wsi.Stage = obj;
                wsi.Status = StatusAccesser.GetAllStatusesByID(obj.RelatedStatus, true);
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
