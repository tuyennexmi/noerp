
// NMFunctionsBL.cs

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
    public class NMFunctionsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMFunctionsBL()
        {

        }

        public NMFunctionsWSI callSingleBL(NMFunctionsWSI wsi)
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

        public List<NMFunctionsWSI> callListBL(NMFunctionsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMFunctionsWSI>();
            }
        }

        public NMFunctionsWSI SelectObject(NMFunctionsWSI wsi)
        {
            FunctionsAccesser Acceser = new FunctionsAccesser(Session);
            Functions obj;
            obj = Acceser.GetAllFunctionsByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.ModuleId = obj.ModuleId;
                wsi.Description = obj.Description;
                wsi.Action = obj.Action;
                wsi.OrdinalNumber = obj.OrdinalNumber.ToString();
                wsi.Active = obj.Active.ToString();
                wsi.Icon = obj.Icon;
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

        public NMFunctionsWSI SaveObject(NMFunctionsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                FunctionsAccesser Accesser = new FunctionsAccesser(Session);
                Functions obj = Accesser.GetAllFunctionsByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.ModuleId = wsi.ModuleId;
                    obj.Description = wsi.Description;
                    obj.Action = wsi.Action;
                    Accesser.UpdateFunctions(obj);
                }
                else
                {
                    obj = new Functions();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId("Functions");
                    obj.Name = wsi.Name;
                    obj.ModuleId = wsi.ModuleId;
                    obj.Description = wsi.Description;
                    obj.Action = wsi.Action;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = wsi.CreatedBy;
                    Accesser.InsertFunctions(obj);
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục\n" + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMFunctionsWSI DeleteObject(NMFunctionsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                FunctionsAccesser Accesser = new FunctionsAccesser(Session);
                Functions obj = Accesser.GetAllFunctionsByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteFunctions(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không thể xóa.\n" + ex.Message + "\n" + ex.InnerException;
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";   
                }
            }
            catch(Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục\n" + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMFunctionsWSI> SearchObject(NMFunctionsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.Active))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Active = :Active";
                ListCriteria.Add("Active", bool.Parse(wsi.Active));
            }
            if (!string.IsNullOrEmpty(wsi.ModuleId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ModuleId = :ModuleId";
                ListCriteria.Add("ModuleId", wsi.ModuleId);
            }
            if (!string.IsNullOrEmpty(wsi.ParentId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ParentId = :ParentId";
                ListCriteria.Add("ParentId", wsi.ParentId);
            }
            String strCmd = "SELECT O FROM Functions AS O" + strCriteria + " ORDER BY O.OrdinalNumber";
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMFunctionsWSI> ListWSI = new List<NMFunctionsWSI>();
            FunctionsAccesser Acceser = new FunctionsAccesser(Session);
            IList<Functions> objs;
            objs = Acceser.GetFunctionsByQuery(query, false);
            foreach (Functions obj in objs)
            {
                wsi = new NMFunctionsWSI();
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.NameInEnglish = obj.NameInEnglish;
                wsi.NameInKorea = obj.NameInKorea;
                wsi.ModuleId = obj.ModuleId;
                wsi.Description = obj.Description;
                wsi.Action = obj.Action;
                wsi.FormName = obj.FormName;
                wsi.OrdinalNumber = obj.OrdinalNumber.ToString();
                wsi.Active = obj.Active.ToString();
                wsi.Icon = obj.Icon;
                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
