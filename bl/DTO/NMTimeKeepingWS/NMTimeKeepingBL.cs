// NMTimeKeepingBL.cs

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
    public class NMTimeKeepingBL
    {
        private ISession Session = SessionFactory.GetNewSession();
        public NMTimeKeepingBL()
        {

        }
        public NMTimeKeepingWSI callBussinessLogic(NMTimeKeepingWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    wsi = SaveObject(wsi);
                    break;
                case "SEL_OBJ":
                    wsi = SelectObject(wsi);
                    break;
                case "SEARCH_BY_DATE":
                    wsi = SearchByDate(wsi);
                    break;
                case "GET_LASTEST":
                    wsi = GetLastest(wsi);
                    break;
                case "GET_LASTEST_END":
                    wsi = GetLastestEnd(wsi);
                    break;
            }
            return wsi;
        }

        public NMTimeKeepingWSI SaveObject(NMTimeKeepingWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                TimeKeepingAccesser Accesser = new TimeKeepingAccesser(Session);
                TimeKeeping obj = Accesser.GetTimeKeepingByUserIDAndEndTime(wsi.UserId, true);
                if (obj == null)
                {
                    obj = new TimeKeeping();
                    obj.UserId = wsi.UserId;
                    obj.StartTime = wsi.StartTime;
                    obj.EndTime = wsi.EndTime;
                    Accesser.InsertTimeKeeping(obj);
                }
                else
                {
                    obj.EndTime = wsi.EndTime;
                    Accesser.UpdateTimeKeeping(obj);
                }
                try
                {
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    wsi.WsiError = "Không thực hiện được. Liên hệ người quản trị để khắc phục.";
                }
            }
            catch
            {
                wsi.WsiError = "Có lỗi xảy ra trong quá trình xử lí. Liên hệ người quản trị để khắc phục.";
            }
            
            return wsi;
        }

        public NMTimeKeepingWSI SelectObject(NMTimeKeepingWSI wsi)
        {
            TimeKeepingAccesser Accesser = new TimeKeepingAccesser(Session);
            try
            {
                TimeKeeping obj = Accesser.GetTimeKeepingByUserIDAndEndTime(wsi.UserId, true);
                if (obj != null)
                {
                    wsi.Id = obj.Id.ToString();
                    wsi.UserId = obj.UserId.ToString();
                    wsi.StartTime = obj.StartTime;
                    wsi.EndTime = obj.EndTime;
                }
            }
            catch { }
            return wsi;
        }

        public NMTimeKeepingWSI GetLastest(NMTimeKeepingWSI wsi)
        {
            TimeKeepingAccesser Accesser = new TimeKeepingAccesser(Session);
            try
            {
                TimeKeeping obj = Accesser.GetTimeKeepingLastest(wsi.UserId, true);
                if (obj != null)
                {
                    wsi.Id = obj.Id.ToString();
                    wsi.UserId = obj.UserId.ToString();
                    wsi.StartTime = obj.StartTime;
                    wsi.EndTime = obj.EndTime;
                }
            }
            catch { }
            return wsi;
        }

        public NMTimeKeepingWSI GetLastestEnd(NMTimeKeepingWSI wsi)
        {
            TimeKeepingAccesser Accesser = new TimeKeepingAccesser(Session);
            try
            {
                TimeKeeping obj = Accesser.GetTimeKeepingLastestEnd(wsi.UserId, true);
                if (obj != null)
                {
                    wsi.Id = obj.Id.ToString();
                    wsi.UserId = obj.UserId.ToString();
                    wsi.StartTime = obj.StartTime;
                    wsi.EndTime = obj.EndTime;
                }
            }
            catch { }
            return wsi;
        }

        public NMTimeKeepingWSI SearchByDate(NMTimeKeepingWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                TimeKeepingAccesser Accesser = new TimeKeepingAccesser(Session);
                IList<TimeKeeping> TimeKeepings = Accesser.GetAllTimeKeepingByDate(wsi.FromDate, true);
                foreach (TimeKeeping Item in TimeKeepings)
                {
                    wsi.IdList.Add(Item.Id);
                    wsi.UserIdList.Add(Item.UserId);
                    wsi.StartTimeList.Add(Item.StartTime);
                    wsi.EndTimeList.Add(Item.EndTime);
                }

            }
            catch { }
            return wsi;
        }
    }
}
