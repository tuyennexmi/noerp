// NMPermissionsBL.cs

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
using NHibernate.Cfg;
using Iesi.Collections;

namespace NEXMI
{
    public class NMPermissionsBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();

        public NMPermissionsBL() 
        {

        }

        public NMPermissionsWSI callSingleBL(NMPermissionsWSI wsi)
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

        public List<NMPermissionsWSI> callListBL(NMPermissionsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMPermissionsWSI>();
            }
        }

        public NMPermissionsWSI SelectObject(NMPermissionsWSI wsi)
        {
            PermissionsAccesser Acceser = new PermissionsAccesser(Session);
            Permissions obj;
            obj = Acceser.GetAllPermissionsByUserIdAndFunctionId(wsi.UserId, wsi.FunctionId, true);
            if (obj != null)
            {
                wsi.Id = obj.Id.ToString();
                wsi.FunctionId = obj.FunctionId;
                wsi.UserGroupId = obj.UserGroupId;
                wsi.UserId = obj.UserId;
                wsi.PSelect = obj.PSelect;
                wsi.PInsert = obj.PInsert;
                wsi.PUpdate = obj.PUpdate;
                wsi.Reconcile = obj.Reconcile;
                wsi.PDelete = obj.PDelete;
                wsi.Approval = obj.Approval;
                wsi.ViewAll = obj.ViewAll;
                wsi.Calculate = obj.Calculate;
                wsi.History = obj.History;
                
                wsi.ViewPrice = obj.ViewPrice;
                wsi.Export = obj.Export;
                wsi.PPrint = obj.PPrint;
                wsi.Duplicate = obj.Duplicate;

                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMPermissionsWSI SaveObject(NMPermissionsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {   
                PermissionsAccesser Accesser = new PermissionsAccesser(Session);
                Permissions obj;
                if(!string.IsNullOrEmpty(wsi.UserId))
                    obj = Accesser.GetAllPermissionsByUserIdAndFunctionId(wsi.UserId, wsi.FunctionId, true);
                else
                    obj = Accesser.GetAllPermissionsByGroupUserIdAndFunctionId(wsi.UserGroupId, wsi.FunctionId, true);

                if (obj != null)
                {
                    obj.FunctionId = wsi.FunctionId;
                    obj.UserGroupId = wsi.UserGroupId;
                    obj.UserId = wsi.UserId;
                    obj.PSelect = wsi.PSelect;
                    obj.PInsert = wsi.PInsert;
                    obj.PUpdate = wsi.PUpdate;
                    obj.Reconcile = wsi.Reconcile;
                    obj.PDelete = wsi.PDelete;
                    obj.Approval = wsi.Approval;
                    obj.ViewAll = wsi.ViewAll;
                    obj.Calculate = wsi.Calculate;
                    obj.History = wsi.History;

                    obj.ViewPrice = wsi.ViewPrice;
                    obj.Export = wsi.Export;
                    obj.PPrint = wsi.PPrint;
                    obj.Duplicate = wsi.Duplicate;
                    
                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifiedBy = wsi.CreatedBy;

                    Accesser.UpdatePermissions(obj);
                }
                else
                {
                    obj = new Permissions();
                    obj.FunctionId = wsi.FunctionId;
                    obj.UserGroupId = wsi.UserGroupId;
                    obj.UserId = wsi.UserId;
                    obj.PSelect = wsi.PSelect;
                    obj.PInsert = wsi.PInsert;
                    obj.PUpdate = wsi.PUpdate;
                    obj.Reconcile = wsi.Reconcile;
                    obj.PDelete = wsi.PDelete;
                    obj.Approval = wsi.Approval;
                    obj.ViewAll = wsi.ViewAll;
                    obj.Calculate = wsi.Calculate;
                    obj.History = wsi.History;

                    obj.ViewPrice = wsi.ViewPrice;
                    obj.Export = wsi.Export;
                    obj.PPrint = wsi.PPrint;
                    obj.Duplicate = wsi.Duplicate;

                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = wsi.CreatedBy;
                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifiedBy = wsi.CreatedBy;

                    Accesser.InsertPermissions(obj);
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

        public NMPermissionsWSI DeleteObject(NMPermissionsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PermissionsAccesser Accesser = new PermissionsAccesser(Session);
                Permissions obj = Accesser.GetAllPermissionsByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeletePermissions(obj);
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

        public List<NMPermissionsWSI> SearchObject(NMPermissionsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";            
            if (!string.IsNullOrEmpty(wsi.UserId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.UserId = :UserId";
                ListCriteria.Add("UserId", wsi.UserId);
            }
            if (!string.IsNullOrEmpty(wsi.UserGroupId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.UserGroupId = :UserGroupId";
                ListCriteria.Add("UserGroupId", wsi.UserGroupId);
            }
            if (!string.IsNullOrEmpty(wsi.FunctionId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.FunctionId = :FunctionId";
                ListCriteria.Add("FunctionId", wsi.FunctionId);
            }
            String strCmd = "SELECT O FROM Permissions AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMPermissionsWSI> ListWSI = new List<NMPermissionsWSI>();
            PermissionsAccesser Acceser = new PermissionsAccesser(Session);
            IList<Permissions> objs;
            objs = Acceser.GetPermissionsByQuery(query, false);
            foreach (Permissions obj in objs)
            {
                wsi = new NMPermissionsWSI();
                wsi.Id = obj.Id.ToString();
                wsi.FunctionId = obj.FunctionId;
                wsi.UserGroupId = obj.UserGroupId;
                wsi.UserId = obj.UserId;
                wsi.PSelect = obj.PSelect;
                wsi.PInsert = obj.PInsert;
                wsi.PUpdate = obj.PUpdate;
                wsi.Reconcile = obj.Reconcile;
                wsi.PDelete = obj.PDelete;
                wsi.Approval = obj.Approval;
                wsi.ViewAll = obj.ViewAll;
                wsi.Calculate = obj.Calculate;
                wsi.History = obj.History;
                
                wsi.ViewPrice = obj.ViewPrice;
                wsi.Export = obj.Export;
                wsi.PPrint = obj.PPrint;
                wsi.Duplicate = obj.Duplicate;

                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
