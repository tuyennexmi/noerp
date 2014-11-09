// NMRequirementsBL.cs

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
    public class NMRequirementsBL
    {
        ISession Session = SessionFactory.GetNewSession();

        public NMRequirementsBL() 
        {
             
        }

        public NMRequirementsWSI callSingleBL(NMRequirementsWSI wsi)
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

        public List<NMRequirementsWSI> callListBL(NMRequirementsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMRequirementsWSI>();
            }
        }

        public NMRequirementsWSI SelectObject(NMRequirementsWSI wsi)
        {
            RequirementsAccesser Accesser = new RequirementsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            Requirements obj;
            obj = Accesser.GetAllRequirementsByID(wsi.Requirement.Id, false);
            if (obj != null)
            {
                wsi.Requirement = obj;                
                wsi.Details = obj.RequirementDetailsList.Cast<RequirementDetails>().ToList();
                Session.Evict(obj);
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.RequiredBy = CustomerAccesser.GetAllCustomersByID(obj.RequiredBy, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMRequirementsWSI SaveObject(NMRequirementsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                RequirementsAccesser Accesser = new RequirementsAccesser(Session);
                RequirementDetailsAccesser DetailAccesser = new RequirementDetailsAccesser(Session);
                List<RequirementDetails> OldDetails = null;
                Requirements obj = Accesser.GetAllRequirementsByID(wsi.Requirement.Id, false);
                RequirementDetails detail;
                wsi.Requirement.Amount = wsi.Details.Sum(i => i.Amount);
                if (obj != null)
                {
                    OldDetails = obj.RequirementDetailsList.Cast<RequirementDetails>().ToList();
                    Session.Evict(obj);
                    wsi.Requirement.CreatedDate = obj.CreatedDate;
                    wsi.Requirement.CreatedBy = obj.CreatedBy;
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Requirement.Id, "cập nhật thông tin", rs, wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    Accesser.UpdateRequirements(wsi.Requirement);
                }
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Requirement.Id = AutomaticValueAccesser.AutoGenerateId("Requirements");
                    wsi.Requirement.CreatedDate = DateTime.Now;
                    wsi.Requirement.CreatedBy = wsi.Filter.ActionBy;
                    Accesser.InsertRequirements(wsi.Requirement);
                    NMMessagesBL.SaveMessage(Session, wsi.Requirement.Id, "khởi tạo tài liệu", "", wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                foreach (RequirementDetails Item in wsi.Details)
                {
                    detail = DetailAccesser.GetAllRequirementdetailsByID(Item.Id.ToString(), true);
                    if (detail != null)
                    {
                        Item.RequirementId = wsi.Requirement.Id;
                        DetailAccesser.UpdateRequirementdetails(Item);
                    }
                    else
                    {
                        Item.RequirementId = wsi.Requirement.Id;
                        DetailAccesser.InsertRequirementdetails(Item);
                    }
                }
                if (OldDetails != null)
                {
                    bool flag = true;
                    foreach (RequirementDetails Old in OldDetails)
                    {
                        flag = true;
                        foreach (RequirementDetails New in wsi.Details)
                        {
                            if (Old.Id == New.Id)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeleteRequirementdetails(Old);
                        }
                    }
                }
                tx.Commit();
            }
            catch(Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục. \n" + ex.Message + ex.InnerException;
            }
            return wsi;
        }

        public NMRequirementsWSI DeleteObject(NMRequirementsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                RequirementsAccesser Accesser = new RequirementsAccesser(Session);
                Requirements obj = Accesser.GetAllRequirementsByID(wsi.Requirement.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteRequirements(obj);
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

        public List<NMRequirementsWSI> SearchObject(NMRequirementsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Requirement != null)
            {
                if (!string.IsNullOrEmpty( wsi.Requirement.OrderId ))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderId = :OrderId";
                    ListCriteria.Add("OrderId", wsi.Requirement.OrderId);
                }
                if (!string.IsNullOrEmpty(wsi.Requirement.RequirementTypeId ))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.RequirementTypeId = :RequirementTypeId";
                    ListCriteria.Add("RequirementTypeId", wsi.Requirement.RequirementTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Requirement.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Requirement.CustomerId);
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
                strCriteria += " O.CreatedDate >= :FromDate AND O.CreatedDate < :ToDate";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.Filter.FromDate));
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.Filter.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.Filter.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.Filter.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Filter.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.OrderId LIKE :Keyword OR O.Description LIKE :Keyword OR O.CustomerId LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Filter.ActionBy + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Requirements AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.RequireDate DESC";
            }
            String strCmd = "SELECT O FROM Requirements AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMRequirementsWSI> ListWSI = new List<NMRequirementsWSI>();
            RequirementsAccesser Accesser = new RequirementsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            IList<Requirements> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.Filter.PageSize != 0)
            {
                objs = Accesser.GetRequirementsByQuery(query, wsi.Filter.PageSize, wsi.Filter.PageNum, false);
            }
            else
            {
                objs = Accesser.GetRequirementsByQuery(query, false);
            }
            foreach (Requirements obj in objs)
            {
                wsi = new NMRequirementsWSI();
                wsi.Requirement = obj;
                wsi.Details = obj.RequirementDetailsList.Cast<RequirementDetails>().ToList();
                Session.Evict(obj);
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.RequiredBy = CustomerAccesser.GetAllCustomersByID(obj.RequiredBy, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                wsi.Filter.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }
    }
}
