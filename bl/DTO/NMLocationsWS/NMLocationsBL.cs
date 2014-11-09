// NMLocationsBL.cs

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
    public class NMLocationsBL
    {

        private ISession Session;
        public NMLocationsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMLocationsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMLocationsWSI callSingleBL(NMLocationsWSI wsi)
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

        public List<NMLocationsWSI> callListBL(NMLocationsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMLocationsWSI>();
            }
        }

        public NMLocationsWSI SelectObject(NMLocationsWSI wsi)
        {
            LocationsAccesser Acceser = new LocationsAccesser(Session);
            Locations obj;
            obj = Acceser.GetAllLocationsByID(wsi.Location.Id, true);
            if (obj != null)
            {
                wsi.Location = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMLocationsWSI SaveObject(NMLocationsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                LocationsAccesser Accesser = new LocationsAccesser(Session);
                Locations obj = Accesser.GetAllLocationsByID(wsi.Location.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);

                    wsi.Location.CreatedDate = obj.CreatedDate;
                    wsi.Location.CreatedBy = obj.CreatedBy;
                    wsi.Location.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Location.ModifiedDate = DateTime.Now;

                    Accesser.UpdateLocations(wsi.Location);
                }
                else
                {
                    wsi.Location.Id = AutomaticValueAccesser.AutoGenerateId("Locations");
                    
                    wsi.Location.CreatedDate = DateTime.Now;
                    wsi.Location.CreatedBy = wsi.Filter.ActionBy;
                    wsi.Location.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Location.ModifiedDate = DateTime.Now;

                    Accesser.InsertLocations(wsi.Location);
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

        private NMLocationsWSI DeleteObject(NMLocationsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                LocationsAccesser Accesser = new LocationsAccesser(Session);
                Locations obj = Accesser.GetAllLocationsByID(wsi.Location.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteLocations(obj);
                        tx.Commit();
                    }
                    catch(Exception ex)
                    {
                        wsi.WsiError = "Không được xóa.\n" + ex.Message + "\n" + ex.InnerException;
                        tx.Rollback();
                    }
                }
            }
            catch(Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục\n" + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private List<NMLocationsWSI> SearchObject(NMLocationsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Location != null)
            {
                if (!string.IsNullOrEmpty(wsi.Location.Name))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Name = :Name";
                    ListCriteria.Add("Name", wsi.Location.Name);
                }
                if (wsi.Location.IsAlarm != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IsAlarm = :IsAlarm";
                    ListCriteria.Add("IsAlarm", wsi.Location.IsAlarm.Value);
                }
                if (!string.IsNullOrEmpty(wsi.Location.Description))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Description LIKE %:Description%";
                    ListCriteria.Add("Description", wsi.Location.Description);
                }

            }
            String strCmd = "SELECT O FROM Locations AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMLocationsWSI> ListWSI = new List<NMLocationsWSI>();
            LocationsAccesser Accesser = new LocationsAccesser(Session);
            IList<Locations> objs;
            objs = Accesser.GetLocationsByQuery(query, false);
            foreach (Locations obj in objs)
            {
                wsi = new NMLocationsWSI();
                wsi.Location = obj;
                ListWSI.Add(wsi);
            }
            return ListWSI; // objs.ToList<Locations>();
        }

    }
}
