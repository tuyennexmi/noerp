// NMParametersBL.cs

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
    public class NMParametersBL
    {
        private ISession Session = SessionFactory.GetNewSession();
        public NMParametersBL()
        {

        }

        public NMParametersWSI callSingleBL(NMParametersWSI wsi)
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

        public List<NMParametersWSI> callListBL(NMParametersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMParametersWSI>();
            }
        }

        public NMParametersWSI SelectObject(NMParametersWSI wsi)
        {
            ParametersAccesser Acceser = new ParametersAccesser(Session);
            Parameters obj;
            obj = Acceser.GetAllParametersByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.ObjectName = obj.ObjectName;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMParametersWSI SaveObject(NMParametersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ParametersAccesser Accesser = new ParametersAccesser(Session);
                Parameters obj = Accesser.GetAllParametersByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.Description = wsi.Description;
                    obj.ObjectName = wsi.ObjectName;
                    Accesser.UpdateParameters(obj);
                }
                else
                {
                    obj = new Parameters();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId(wsi.ObjectName);
                    obj.Name = wsi.Name;
                    obj.Description = wsi.Description;
                    obj.ObjectName = wsi.ObjectName;
                    Accesser.InsertParameters(obj);
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

        public NMParametersWSI DeleteObject(NMParametersWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                ParametersAccesser Accesser = new ParametersAccesser(Session);
                Parameters obj = Accesser.GetAllParametersByID(wsi.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteParameters(obj);
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

        public List<NMParametersWSI> SearchObject(NMParametersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.ObjectName))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ObjectName = :ObjectName";
                ListCriteria.Add("ObjectName", wsi.ObjectName);
            }
            String strCmd = "SELECT O FROM Parameters AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMParametersWSI> ListWSI = new List<NMParametersWSI>();
            ParametersAccesser Accesser = new ParametersAccesser(Session);
            IList<Parameters> objs;
            objs = Accesser.GetParametersByQuery(query, false);
            foreach (Parameters obj in objs)
            {
                wsi = new NMParametersWSI();
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.ObjectName = obj.ObjectName;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
