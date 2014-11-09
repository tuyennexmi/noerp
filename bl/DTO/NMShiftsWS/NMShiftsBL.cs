// NMShiftsBL.cs

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
    public class NMShiftsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMShiftsBL()
        {

        }

        public NMShiftsWSI callSingleBL(NMShiftsWSI wsi)
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

        public List<NMShiftsWSI> callListBL(NMShiftsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMShiftsWSI>();
            }
        }

        public NMShiftsWSI SelectObject(NMShiftsWSI wsi)
        {
            ShiftsAccesser Acceser = new ShiftsAccesser(Session);
            Shifts obj;
            obj = Acceser.GetAllShiftsByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.Start = obj.Start.ToString();
                wsi.Finish = obj.Finish.ToString();
                wsi.WsiError = "";
            }
            else
            {
                wsi.Id = "";
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMShiftsWSI SaveObject(NMShiftsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ShiftsAccesser Accesser = new ShiftsAccesser(Session);
                Shifts obj = Accesser.GetAllShiftsByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.Start = DateTime.Parse(wsi.Start);
                    obj.Finish = DateTime.Parse(wsi.Finish);
                    obj.Description = wsi.Description;
                    Accesser.UpdateShifts(obj);
                }
                else
                {
                    obj = new Shifts();
                    obj.Name = wsi.Name;
                    obj.Description = wsi.Description;
                    obj.Start = DateTime.Parse(wsi.Start);
                    obj.Finish = DateTime.Parse(wsi.Finish);
                    Accesser.InsertShifts(obj);
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

        public NMShiftsWSI DeleteObject(NMShiftsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ShiftsAccesser Accesser = new ShiftsAccesser(Session);
                Shifts obj = Accesser.GetAllShiftsByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteShifts(obj);
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

        public List<NMShiftsWSI> SearchObject(NMShiftsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Name != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Name LIKE :Name";
                ListCriteria.Add("Name", "%" + wsi.Name + "%");
            }
            
            if (wsi.Keyword != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Id LIKE :Keyword OR O.Name LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            String strCmd = "SELECT O FROM Shifts AS O" + strCriteria + " ORDER BY O.Name";
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMShiftsWSI> ListWSI = new List<NMShiftsWSI>();
            ShiftsAccesser Accesser = new ShiftsAccesser(Session);
            IList<Shifts> objs;
            objs = Accesser.GetShiftsByQuery(query, false);
            foreach (Shifts obj in objs)
            {
                wsi = new NMShiftsWSI();
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.Description = obj.Description;
                wsi.Start = obj.Start.ToString();
                wsi.Finish = obj.Finish.ToString();
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
