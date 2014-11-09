// NMManufactureOrdersBL.cs

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
using Iesi.Collections;

namespace NEXMI
{
    public class NMManufactureOrdersBL
    {
        private readonly ISession Session;

        public NMManufactureOrdersBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMManufactureOrdersBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMManufactureOrdersWSI callSingleBL(NMManufactureOrdersWSI wsi)
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

        public List<NMManufactureOrdersWSI> callListBL(NMManufactureOrdersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMManufactureOrdersWSI>();
            }
        }

        private NMManufactureOrdersWSI SaveObject(NMManufactureOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ManufactureOrders obj = new ManufactureOrders();
                ManufactureOrdersAccesser ManufactureOrderAccesser = new ManufactureOrdersAccesser(this.Session);
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                List<MOMaterialDetails> OldMDetails = null;

                obj = ManufactureOrderAccesser.GetAllManufactureOrdersByID(wsi.ManufactureOrder.Id, false);
                if (obj == null)
                {
                    wsi.ManufactureOrder.Id = AutomaticValueAccesser.AutoGenerateId("ManufactureOrders");
                    wsi.ManufactureOrder.CreatedDate = DateTime.Now;
                    wsi.ManufactureOrder.CreatedBy = wsi.Filter.ActionBy;
                    wsi.ManufactureOrder.ModifiedDate = DateTime.Now;
                    wsi.ManufactureOrder.ModifiedBy = wsi.Filter.ActionBy;
                    if (wsi.ManufactureOrder.MaterialDetails != null)
                    foreach (MOMaterialDetails itm in wsi.ManufactureOrder.MaterialDetails)
                    {
                        itm.MOId = wsi.ManufactureOrder.Id;
                        itm.CreatedBy = wsi.Filter.ActionBy;
                        itm.CreatedDate = DateTime.Now;
                        itm.ModifiedBy = wsi.Filter.ActionBy;
                        itm.ModifiedDate = DateTime.Now;
                    }
                    ManufactureOrderAccesser.InsertManufactureOrders(wsi.ManufactureOrder);
                    NMMessagesBL.SaveMessage(Session, wsi.ManufactureOrder.Id, "khởi tạo tài liệu", "", wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    OldMDetails = obj.MaterialDetails.Cast<MOMaterialDetails>().ToList();
                    Session.Evict(obj);
                    wsi.ManufactureOrder.CreatedDate = obj.CreatedDate;
                    wsi.ManufactureOrder.CreatedBy = obj.CreatedBy;
                    wsi.ManufactureOrder.ModifiedDate = DateTime.Now;
                    wsi.ManufactureOrder.ModifiedBy = wsi.Filter.ActionBy;
                    if(wsi.ManufactureOrder.MaterialDetails != null)
                        foreach (MOMaterialDetails itm in wsi.ManufactureOrder.MaterialDetails)
                        {
                            itm.MOId = wsi.ManufactureOrder.Id;
                            itm.CreatedBy = wsi.Filter.ActionBy;
                            itm.CreatedDate = DateTime.Now;
                            itm.ModifiedBy = wsi.Filter.ActionBy;
                            itm.ModifiedDate = DateTime.Now;
                        }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.ManufactureOrder.Id, "cập nhật thông tin", rs, wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    ManufactureOrderAccesser.UpdateManufactureOrders(wsi.ManufactureOrder);
                }

                MOMaterialDetailsAccesser DetailAccesser = new MOMaterialDetailsAccesser(this.Session);
                MOMaterialDetails  detail;
                //foreach (MasterManufactureOrderDetails Item in wsi.ManufactureOrder.Details)
                //{
                //    Item.MasterId = wsi.ManufactureOrder.Id;
                //    detail = DetailAccesser.GetAllMasterManufactureOrderDetailsByID(Item.Id.ToString(), true);
                //    if (detail != null)
                //    {
                //        Item.CreatedBy = detail.CreatedBy;
                //        Item.CreatedDate = detail.CreatedDate;
                //        DetailAccesser.UpdateMasterManufactureOrderDetails(Item);
                //    }
                //    else
                //    {
                //        Item.CreatedBy = wsi.Filter.ActionBy;
                //        Item.CreatedDate = DateTime.Now;
                //        DetailAccesser.InsertMasterManufactureOrderDetails(Item);
                //    }
                //}
                if (OldMDetails != null)
                {
                    bool flag = true;
                    foreach (MOMaterialDetails Old in OldMDetails)
                    {
                        flag = true;
                        foreach (MOMaterialDetails New in wsi.ManufactureOrder.MaterialDetails)
                        {
                            if (Old.OrdinalNumber == New.OrdinalNumber)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeleteMOMaterialDetails(Old);
                        }
                    }
                }

                tx.Commit();
            }
            catch(Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }

            return wsi;
        }

        private NMManufactureOrdersWSI SelectObject(NMManufactureOrdersWSI wsi)
        {
            ManufactureOrdersAccesser Accesser = new ManufactureOrdersAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ManufactureOrders obj;
            obj = Accesser.GetAllManufactureOrdersByID(wsi.ManufactureOrder.Id, false);
            if (obj != null)
            {
                wsi.ManufactureOrder = obj;
                obj.MaterialDetails.Cast<MOMaterialDetails>().ToList();
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);                
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                wsi.ApprovalBy = CustomerAccesser.GetAllCustomersByID(obj.ApprovalBy, true);
                wsi.ManagedBy = CustomerAccesser.GetAllCustomersByID(obj.ManagedBy, true);
                wsi.WsiError = "";
                Session.Evict(obj);
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMManufactureOrdersWSI DeleteObject(NMManufactureOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            ManufactureOrdersAccesser Accesser = new ManufactureOrdersAccesser(Session);
            ManufactureOrders obj;
            obj = Accesser.GetAllManufactureOrdersByID(wsi.ManufactureOrder.Id, true);
            if (obj != null)
            {
                Accesser.DeleteManufactureOrders(obj);
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
            return wsi;
        }

        private List<NMManufactureOrdersWSI> SearchObject(NMManufactureOrdersWSI wsi)
        {
            List<NMManufactureOrdersWSI> ListWSI = new List<NMManufactureOrdersWSI>();
            try
            {
                Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
                String strCriteria = "";
                if (wsi.ManufactureOrder != null)
                {
                    if (!String.IsNullOrEmpty(wsi.ManufactureOrder.Status))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.Status = :Status";
                        ListCriteria.Add("Status", wsi.ManufactureOrder.Status);
                    }                  

                }
                if (!string.IsNullOrEmpty(wsi.Filter.FromDate))
                {
                    if (string.IsNullOrEmpty(wsi.Filter.ToDate))
                    {
                        wsi.Filter.ToDate = DateTime.Parse(wsi.Filter.FromDate).AddDays(1).ToString();
                    }
                    else
                    {
                        wsi.Filter.ToDate = DateTime.Parse(wsi.Filter.ToDate).AddDays(1).ToString();
                    }
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StartDate >= :FromDate AND O.StartDate < :ToDate";
                    ListCriteria.Add("FromDate", DateTime.Parse(wsi.Filter.FromDate));
                    ListCriteria.Add("ToDate", DateTime.Parse(wsi.Filter.ToDate));
                }

                if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
                {
                    strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
                }
                else
                {
                    strCriteria += " ORDER BY O.StartDate";
                }
                
                String strCmd = "SELECT O FROM ManufactureOrders AS O" + strCriteria;
                IQuery query = Session.CreateQuery(strCmd);
                foreach (var Item in ListCriteria)
                {
                    query.SetParameter(Item.Key, Item.Value);
                }
                ManufactureOrdersAccesser Accesser = new ManufactureOrdersAccesser(Session);
                CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                IList<ManufactureOrders> objs;
                objs = Accesser.GetManufactureOrdersByQuery(query, false);
                foreach (ManufactureOrders obj in objs)
                {
                    wsi = new NMManufactureOrdersWSI();
                    wsi.ManufactureOrder = obj;
                    wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                    wsi.ManagedBy = CustomerAccesser.GetAllCustomersByID(obj.ManagedBy, true);
                    wsi.ApprovalBy = CustomerAccesser.GetAllCustomersByID(obj.ApprovalBy, true);
                    ListWSI.Add(wsi);
                }
            }
            catch { }
            return ListWSI;
        }
    }
    
}
