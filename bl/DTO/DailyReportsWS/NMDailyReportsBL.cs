// NMDailyReportsBL.cs

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
    public class NMDailyReportsBL
    {

        private ISession Session;
        public NMDailyReportsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }


        public NMDailyReportsWSI callSingleBL(NMDailyReportsWSI wsi)
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

        public List<NMDailyReportsWSI> callListBL(NMDailyReportsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMDailyReportsWSI>();
            }
        }

        public NMDailyReportsWSI SelectObject(NMDailyReportsWSI wsi)
        {
            DailyReportsAccesser Acceser = new DailyReportsAccesser(Session);
            DailyReports obj;
            obj = Acceser.GetAllDailyReportsByID(wsi.DailyReport.Id, true);
            if (obj != null)
            {
                wsi.DailyReport = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMDailyReportsWSI SaveObject(NMDailyReportsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                DailyReportsAccesser Accesser = new DailyReportsAccesser(Session);
                DailyReports obj = Accesser.GetAllDailyReportsByID(wsi.DailyReport.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);
                    wsi.DailyReport.CreatedDate = obj.CreatedDate;
                    wsi.DailyReport.CreatedBy = obj.CreatedBy;
                    wsi.DailyReport.ModifiedDate = DateTime.Now;
                    wsi.DailyReport.ModifiedBy = wsi.Filter.ActionBy;
                    Accesser.UpdateDailyReports(wsi.DailyReport);
                }
                else
                {
                    wsi.DailyReport.Id = AutomaticValueAccesser.AutoGenerateId("DailyReports");
                    wsi.DailyReport.CreatedDate = DateTime.Now;
                    wsi.DailyReport.CreatedBy = wsi.Filter.ActionBy;
                    wsi.DailyReport.ModifiedDate = DateTime.Now;
                    wsi.DailyReport.ModifiedBy = wsi.Filter.ActionBy;

                    Accesser.InsertDailyReports(wsi.DailyReport);
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

        private NMDailyReportsWSI DeleteObject(NMDailyReportsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                DailyReportsAccesser Accesser = new DailyReportsAccesser(Session);
                DailyReports obj = Accesser.GetAllDailyReportsByID(wsi.DailyReport.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteDailyReports(obj);
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

        private List<NMDailyReportsWSI> SearchObject(NMDailyReportsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.DailyReport != null)
            {
                if (!string.IsNullOrEmpty(wsi.DailyReport.Title))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Title LIKE :Title";
                    ListCriteria.Add("Title", wsi.DailyReport.Title);
                }
                if (!string.IsNullOrEmpty(wsi.DailyReport.Contents))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Contents LIKE :Contents";
                    ListCriteria.Add("Contents", wsi.DailyReport.Contents);
                }
                if (!string.IsNullOrEmpty(wsi.DailyReport.Advantages))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Advantages LIKE :Advantages";
                    ListCriteria.Add("Advantages", wsi.DailyReport.Advantages);
                }
                if (!string.IsNullOrEmpty(wsi.DailyReport.Hards))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Hards LIKE :Hards";
                    ListCriteria.Add("Hards", wsi.DailyReport.Hards);
                }
                if (!string.IsNullOrEmpty(wsi.DailyReport.Promotes))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Promotes LIKE :Promotes";
                    ListCriteria.Add("Promotes", wsi.DailyReport.Promotes);
                }

                if (!string.IsNullOrEmpty(wsi.DailyReport.CreatedBy))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CreatedBy = :CreatedBy";
                    ListCriteria.Add("CreatedBy", wsi.DailyReport.CreatedBy);
                }
                
                if (!string.IsNullOrEmpty(wsi.DailyReport.TaskId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TaskId = :TaskId";
                    ListCriteria.Add("TaskId", wsi.DailyReport.TaskId);
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

            String strCmd = "SELECT O FROM DailyReports AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            string strCmdCounter = "SELECT COUNT(O.Id) FROM DailyReports AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);

            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            
            List<NMDailyReportsWSI> ListWSI = new List<NMDailyReportsWSI>();
            DailyReportsAccesser Accesser = new DailyReportsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            IList<DailyReports> objs;
            int totalRows = Accesser.Counter(queryCounter);
            if (wsi.Filter.PageSize != 0)
            {
                objs = Accesser.GetCustomersByQuery(query, wsi.Filter.PageSize, wsi.Filter.PageNum, false);
            }
            else
            {
                objs = Accesser.GetDailyReportsByQuery(query, false);
            }
            foreach (DailyReports obj in objs)
            {
                wsi = new NMDailyReportsWSI();
                wsi.DailyReport = obj;
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                ListWSI.Add(wsi);
            }
            if(ListWSI.Count > 0)
                ListWSI[0].Filter.TotalRows = totalRows; 
            return ListWSI;
        }

    }
}