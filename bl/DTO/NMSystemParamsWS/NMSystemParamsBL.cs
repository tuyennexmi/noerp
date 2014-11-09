// NMSystemParamsBL.cs

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
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMSystemParamsBL
    {

        private ISession Session = SessionFactory.GetNewSession();
        public NMSystemParamsBL()
        {

        }

        public NMSystemParamsWSI callSingleBL(NMSystemParamsWSI wsi)
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

        public List<SystemParams> callListBL(NMSystemParamsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<SystemParams>();
            }
        }

        public NMSystemParamsWSI SelectObject(NMSystemParamsWSI wsi)
        {
            SystemParamsAccesser Acceser = new SystemParamsAccesser(Session);
            SystemParams obj;
            obj = Acceser.GetAllSystemParamsByID(wsi.SystemParam.Id, true);
            if (obj != null)
            {
                wsi.SystemParam = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMSystemParamsWSI SaveObject(NMSystemParamsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                SystemParamsAccesser Accesser = new SystemParamsAccesser(Session);
                SystemParams obj = Accesser.GetAllSystemParamsByID(wsi.SystemParam.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);
                    wsi.SystemParam.CreatedDate = obj.CreatedDate;
                    wsi.SystemParam.CreatedBy = obj.CreatedBy;
                    Accesser.UpdateSystemParams(wsi.SystemParam);
                }
                else
                {
                    wsi.SystemParam.Id = AutomaticValueAccesser.AutoGenerateId("SystemParams");
                    wsi.SystemParam.CreatedDate = DateTime.Now;
                    Accesser.InsertSystemParams(wsi.SystemParam);
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

        public NMSystemParamsWSI DeleteObject(NMSystemParamsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                SystemParamsAccesser Accesser = new SystemParamsAccesser(Session);
                SystemParams obj = Accesser.GetAllSystemParamsByID(wsi.SystemParam.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteSystemParams(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public List<SystemParams> SearchObject(NMSystemParamsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.SystemParam != null)
            {
                if (!string.IsNullOrEmpty(wsi.SystemParam.Name))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Name = :Name";
                    ListCriteria.Add("Name", wsi.SystemParam.Name);
                }
                if (!string.IsNullOrEmpty(wsi.SystemParam.ObjectName))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ObjectName = :ObjectName";
                    ListCriteria.Add("ObjectName", wsi.SystemParam.ObjectName);
                }
                if (!string.IsNullOrEmpty(wsi.SystemParam.Type))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Type = :Type";
                    ListCriteria.Add("Type", wsi.SystemParam.Type);
                }
                //if (wsi.SystemParam.ActionDate != DateTime.MinValue)
                //{
                //    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                //    strCriteria += " O.ActionDate = :ActionDate";
                //    ListCriteria.Add("ActionDate", wsi.SystemParam.ActionDate);
                //}
            }
            String strCmd = "SELECT O FROM SystemParams AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            //List<NMSystemParamsWSI> ListWSI = new List<NMSystemParamsWSI>();
            SystemParamsAccesser Accesser = new SystemParamsAccesser(Session);
            IList<SystemParams> objs;
            objs = Accesser.GetSystemParamsByQuery(query, false);
            //foreach (SystemParams obj in objs)
            //{
            //    wsi = new NMSystemParamsWSI();                    
            //    wsi.Status = obj;
            //    ListWSI.Add(wsi);
            //}
            return objs.ToList<SystemParams>();
        }

    }
}
