// NMJobsBL.cs

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
    public class NMJobsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMJobsBL()
        {

        }

        public NMJobsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMJobsWSI callSingleBL(NMJobsWSI wsi)
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

        public List<NMJobsWSI> callListBL(NMJobsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMJobsWSI>();
            }
        }

        public NMJobsWSI SelectObject(NMJobsWSI wsi)
        {
            JobsAccesser Accesser = new  JobsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            
            Jobs obj;
            obj = Accesser.GetAllJobsByID(wsi.Job.Id, false);
            if (obj != null)
            {
                wsi.Job = obj;
                
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMJobsWSI SaveObject(NMJobsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                JobsAccesser Accesser = new JobsAccesser(Session);
                JobsAccesser DetailAccesser = new JobsAccesser(Session);
                Jobs obj = Accesser.GetAllJobsByID(wsi.Job.Id, false);
                
                if (obj != null)
                {   
                    Session.Evict(obj);
                    wsi.Job.CreatedDate = obj.CreatedDate;
                    wsi.Job.CreatedBy = obj.CreatedBy;
                    wsi.Job.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Job.ModifiedDate = DateTime.Now;

                    Accesser.UpdateJob(wsi.Job);

                    String rs = wsi.CompareTo(obj);
                    if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.Job.Id, "cập nhật thông tin", rs, wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    wsi.Job.Id = AutomaticValueAccesser.AutoGenerateId("Jobs");
                    wsi.Job.CreatedDate = DateTime.Now;
                    wsi.Job.CreatedBy = wsi.Filter.ActionBy;
                    wsi.Job.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Job.ModifiedDate = DateTime.Now;

                    Accesser.InsertJob(wsi.Job);

                    NMMessagesBL.SaveMessage(Session, wsi.Job.Id, "khởi tạo tài liệu", "", wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
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

        public NMJobsWSI DeleteObject(NMJobsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                JobsAccesser Accesser = new JobsAccesser(Session);
                Jobs obj = Accesser.GetAllJobsByID(wsi.Job.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteJob(obj);
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

        public List<NMJobsWSI> SearchObject(NMJobsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Job != null)
            {
                if (!string.IsNullOrEmpty(wsi.Job.Name))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Name LIKE :Name";
                    ListCriteria.Add("Name", wsi.Job.Name);
                }
                if (!string.IsNullOrEmpty(wsi.Job.Purpose))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Purpose LIKE :Purpose";
                    ListCriteria.Add("Purpose", wsi.Job.Purpose);
                }
            }
            if (!string.IsNullOrEmpty(wsi.Filter.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.Filter.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Filter.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Id LIKE :Keyword OR O.Name LIKE :Keyword OR O.Tags LIKE :Keyword OR O.WorkSummary LIKE :Keyword OR O.Purpose LIKE :Keyword OR O.Criteria LIKE :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Filter.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Jobs AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Id DESC";
            }
            String strCmd = "SELECT O FROM Jobs AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMJobsWSI> ListWSI = new List<NMJobsWSI>();
            JobsAccesser Accesser = new JobsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            
            IList<Jobs> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.Filter.PageSize != 0)
            {
                objs = Accesser.GetJobsByQuery(query, wsi.Filter.PageSize, wsi.Filter.PageNum, false);
            }
            else
            {
                objs = Accesser.GetJobsByQuery(query, false);
            }
            if (wsi.Filter.Limit != 0)
            {
                objs = objs.Take(wsi.Filter.Limit).ToList();
            }
            foreach (Jobs obj in objs)
            {
                wsi = new NMJobsWSI();
                wsi.Job = obj;
                
                ListWSI.Add(wsi);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].Filter.TotalRows = totalRows;

            return ListWSI;
        }       
    }
}
