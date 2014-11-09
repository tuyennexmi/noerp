// NMCalendarsBL.cs

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
    public class NMCalendarsBL
    {

        private ISession Session;
        public NMCalendarsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }


        public NMCalendarsWSI callSingleBL(NMCalendarsWSI wsi)
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

        public List<Calendars> callListBL(NMCalendarsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<Calendars>();
            }
        }

        public NMCalendarsWSI SelectObject(NMCalendarsWSI wsi)
        {
            CalendarsAccesser Acceser = new CalendarsAccesser(Session);
            Calendars obj;
            obj = Acceser.GetAllCalendarsByID(wsi.Calendar.Id, true);
            if (obj != null)
            {
                wsi.Calendar = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMCalendarsWSI SaveObject(NMCalendarsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                CalendarsAccesser Accesser = new CalendarsAccesser(Session);
                Calendars obj = Accesser.GetAllCalendarsByID(wsi.Calendar.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);
                    
                    wsi.Calendar.CreatedDate = obj.CreatedDate;
                    wsi.Calendar.CreatedBy = obj.CreatedBy;
                    //wsi.Calendar.ModifiedBy = wsi.Filter.ActionBy;
                    //wsi.Calendar.ModifiedDate = DateTime.Now;

                    Accesser.UpdateCalendars(wsi.Calendar);
                }
                else
                {
                    wsi.Calendar.Id = AutomaticValueAccesser.AutoGenerateId("Calendars");

                    wsi.Calendar.CreatedDate = DateTime.Now;
                    wsi.Calendar.CreatedBy = wsi.Filter.ActionBy;
                    //wsi.Location.ModifiedBy = wsi.Filter.ActionBy;
                    //wsi.Location.ModifiedDate = DateTime.Now;

                    Accesser.InsertCalendars(wsi.Calendar);
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

        private NMCalendarsWSI DeleteObject(NMCalendarsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                CalendarsAccesser Accesser = new CalendarsAccesser(Session);
                Calendars obj = Accesser.GetAllCalendarsByID(wsi.Calendar.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteCalendars(obj);
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

        private List<Calendars> SearchObject(NMCalendarsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Calendar != null)
            {
                if (!string.IsNullOrEmpty(wsi.Calendar.Title))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Title = :Title";
                    ListCriteria.Add("Title", wsi.Calendar.Title);
                }
                if (!string.IsNullOrEmpty(wsi.Calendar.IsPublish.ToString()))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IsPublish = :IsPublish";
                    ListCriteria.Add("IsPublish", wsi.Calendar.IsPublish);
                }
                if (!string.IsNullOrEmpty(wsi.Calendar.TypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TypeId = :TypeId";
                    ListCriteria.Add("TypeId", wsi.Calendar.TypeId);
                }
                
            }
            String strCmd = "SELECT O FROM Calendars AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            //List<NMCalendarsWSI> ListWSI = new List<NMCalendarsWSI>();
            CalendarsAccesser Accesser = new CalendarsAccesser(Session);
            IList<Calendars> objs;
            objs = Accesser.GetCalendarsByQuery(query, false);
            //foreach (Calendars obj in objs)
            //{
            //    wsi = new NMCalendarsWSI();                    
            //    wsi.Status = obj;
            //    ListWSI.Add(wsi);
            //}
            return objs.ToList<Calendars>();
        }

    }
}
