// NMAreasBL.cs

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

namespace NEXMI
{
    public class NMAreasBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMAreasBL()
        {

        }

        public NMAreasWSI callSingleBL(NMAreasWSI wsi)
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

        public List<NMAreasWSI> callListBL(NMAreasWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMAreasWSI>();
            }
        }

        public NMAreasWSI SelectObject(NMAreasWSI wsi)
        {
            AreasAccesser Accesser = new AreasAccesser(Session);
            Areas obj;
            Areas Area;
            obj = Accesser.GetAllAreasByID(wsi.Area.Id, true);
            if (obj != null)
            {
                wsi.Area = obj;
                wsi.Parent = Accesser.GetAllAreasByID(obj.ParentId, true);
                wsi.FullName = obj.Name;
                string parentId = obj.ParentId;
                while (!String.IsNullOrEmpty(parentId))
                {
                    Area = Accesser.GetAllAreasByID(parentId, true);
                    if (Area != null)
                    {
                        parentId = Area.ParentId;
                        wsi.FullName += ", " + Area.Name;
                        if (string.IsNullOrEmpty(Area.ParentId))
                            wsi.Country = Area;
                    }
                    else
                    {
                        parentId = null;
                    }
                }
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMAreasWSI SaveObject(NMAreasWSI wsi)
        {
            if (wsi.Area.Id == wsi.Area.ParentId)
            {
                wsi.WsiError = "Vui lòng chọn lại khu vực trực thuộc (không được trùng với khu vực đang tạo)!";
                return wsi;
            }

            ITransaction tx = Session.BeginTransaction();
            try
            {
                AreasAccesser Accesser = new AreasAccesser(Session);
                Areas obj = Accesser.GetAllAreasByID(wsi.Area.Id, true);
                if (obj != null)
                {
                    Accesser.UpdateAreas(wsi.Area);
                    wsi.Area.CreatedBy = obj.CreatedBy;
                    wsi.Area.CreatedDate = obj.CreatedDate;
                    wsi.Area.ModifiedBy = wsi.ActionBy;
                    wsi.Area.ModifiedDate = DateTime.Now;
                }
                else
                {
                    wsi.Area.CreatedBy = wsi.ActionBy;
                    wsi.Area.CreatedDate = DateTime.Now;
                    wsi.Area.ModifiedBy = wsi.ActionBy;
                    wsi.Area.ModifiedDate = DateTime.Now;
                    Accesser.InsertAreas(wsi.Area);
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMAreasWSI DeleteObject(NMAreasWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AreasAccesser Accesser = new AreasAccesser(Session);
                Areas obj = Accesser.GetAllAreasByID(wsi.Area.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteAreas(obj);
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
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMAreasWSI> SearchObject(NMAreasWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Area != null)
            {
                if (wsi.Area.ParentId != "")
                {
                    if (wsi.Area.ParentId == "root")
                    {
                        wsi.Area.ParentId = "";
                    }
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ParentId = :ParentId";
                    ListCriteria.Add("ParentId", wsi.Area.ParentId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Id LIKE :Keyword OR O.Name LIKE :Keyword)";
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Areas AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Id ASC";
            }
            String strCmd = "SELECT O FROM Areas AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMAreasWSI> ListWSI = new List<NMAreasWSI>();
            AreasAccesser Accesser = new AreasAccesser(Session);
            IList<Areas> objs;
            Areas Area;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetAreasByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetAreasByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Areas obj in objs)
            {
                wsi = new NMAreasWSI();
                wsi.Area = obj;
                wsi.Parent = Accesser.GetAllAreasByID(obj.ParentId, true);
                wsi.FullName = obj.Name;
                string parentId = obj.ParentId;
                while (!String.IsNullOrEmpty(parentId))
                {
                    Area = Accesser.GetAllAreasByID(parentId, true);
                    if (Area != null)
                    {
                        parentId = Area.ParentId;
                        wsi.FullName += ", " + Area.Name;
                    }
                    else
                    {
                        parentId = null;
                    }
                }
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
