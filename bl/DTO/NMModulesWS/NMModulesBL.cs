
// NMModulesBL.cs

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
    public class NMModulesBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMModulesBL()
        {
            
        }

        public NMModulesWSI callSingleBL(NMModulesWSI wsi)
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

        public List<NMModulesWSI> callListBL(NMModulesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMModulesWSI>();
            }
        }

        public NMModulesWSI SelectObject(NMModulesWSI wsi)
        {
            ModulesAccesser Accesser = new ModulesAccesser(Session);
            FunctionsAccesser FunctionAccesser = new FunctionsAccesser(Session);
            Modules obj;
            obj = Accesser.GetAllModulesByID(wsi.Id, false);
            if (obj != null)
            {
                wsi.Module = obj;
                try
                {
                    wsi.Active = obj.Active.ToString();
                }
                catch { }
                wsi.DefaultFunction = FunctionAccesser.GetAllFunctionsByID(obj.DefaultFunctionId, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMModulesWSI SaveObject(NMModulesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ModulesAccesser Accesser = new ModulesAccesser(Session);
                Modules obj = Accesser.GetAllModulesByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.UpdateModules(obj);
                }
                else
                {
                    obj = new Modules();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId("Modules");
                    Accesser.InsertModules(obj);
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

        public NMModulesWSI DeleteObject(NMModulesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ModulesAccesser Accesser = new ModulesAccesser(Session);
                Modules obj = Accesser.GetAllModulesByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteModules(obj);
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
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMModulesWSI> SearchObject(NMModulesWSI wsi)
        {
            if (wsi.Module == null)
            {
                wsi.Module = new Modules();
            }
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.Active))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Active = :Active";
                ListCriteria.Add("Active", bool.Parse(wsi.Active));
            }
            String strCmd = "SELECT O FROM Modules AS O" + strCriteria + " ORDER BY Ordinal";
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMModulesWSI> ListWSI = new List<NMModulesWSI>();
            ModulesAccesser Acceser = new ModulesAccesser(Session);
            FunctionsAccesser FunctionAccesser = new FunctionsAccesser(Session);
            IList<Modules> objs;
            objs = Acceser.GetModulesByQuery(query, false);
            foreach (Modules obj in objs)
            {
                wsi = new NMModulesWSI();
                wsi.Id = obj.Id;
                wsi.Module = obj;
                try
                {
                    wsi.Active = obj.Active.ToString();
                }
                catch { }
                wsi.DefaultFunction = FunctionAccesser.GetAllFunctionsByID(obj.DefaultFunctionId, true);
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
