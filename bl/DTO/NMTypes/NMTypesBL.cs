// NMTypesBL.cs

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
    public class NMTypesBL
    {
        private ISession Session = SessionFactory.GetNewSession();
        public NMTypesBL()
        {

        }

        public NMTypesWSI callSingleBL(NMTypesWSI wsi)
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

        public List<NMTypesWSI> callListBL(NMTypesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMTypesWSI>();
            }
        }

        public NMTypesWSI SelectObject(NMTypesWSI wsi)
        {
            TypesAccesser Acceser = new TypesAccesser(Session);
            Types obj;
            obj = Acceser.GetAllTypesByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.ObjectName = obj.ObjectName;
                try
                {
                    wsi.IsDefault = obj.IsDefault.ToString();
                }
                catch { }
                try
                {
                    wsi.Ordinal = obj.Ordinal.ToString();
                }
                catch { }
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

        public NMTypesWSI SaveObject(NMTypesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                TypesAccesser Accesser = new TypesAccesser(Session);
                Types obj = Accesser.GetAllTypesByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.Description = wsi.Description;
                    obj.ObjectName = wsi.ObjectName;
                    try
                    {
                        obj.IsDefault = Boolean.Parse(wsi.IsDefault);
                    }
                    catch { }
                    try
                    {
                        obj.Ordinal = int.Parse(wsi.Ordinal);
                    }
                    catch { }
                    Accesser.UpdateTypes(obj);
                }
                else
                {
                    obj = new Types();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId(wsi.ObjectName);
                    obj.Name = wsi.Name;
                    obj.Description = wsi.Description;
                    obj.ObjectName = wsi.ObjectName;
                    try
                    {
                        obj.IsDefault = Boolean.Parse(wsi.IsDefault);
                    }
                    catch { }
                    try
                    {
                        obj.Ordinal = int.Parse(wsi.Ordinal);
                    }
                    catch { }
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = wsi.CreatedBy;
                    Accesser.InsertTypes(obj);
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

        public NMTypesWSI DeleteObject(NMTypesWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                TypesAccesser Accesser = new TypesAccesser(Session);
                Types obj = Accesser.GetAllTypesByID(wsi.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteTypes(obj);
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

        public List<NMTypesWSI> SearchObject(NMTypesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Name != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Name LIKE :Name";
                ListCriteria.Add("Name", "%" + wsi.Name + "%");
            }
            if (wsi.ObjectName != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ObjectName = :ObjectName";
                ListCriteria.Add("ObjectName", wsi.ObjectName);
            }
            String strCmd = "SELECT O FROM Types AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMTypesWSI> ListWSI = new List<NMTypesWSI>();
            TypesAccesser Accesser = new TypesAccesser(Session);
            IList<Types> objs;
            objs = Accesser.GetTypesByQuery(query, false);
            foreach (Types obj in objs)
            {
                wsi = new NMTypesWSI();
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.ObjectName = obj.ObjectName;
                try
                {
                    wsi.IsDefault = obj.IsDefault.ToString();
                }
                catch { }
                try
                {
                    wsi.Ordinal = obj.Ordinal.ToString();
                }
                catch { }
                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
