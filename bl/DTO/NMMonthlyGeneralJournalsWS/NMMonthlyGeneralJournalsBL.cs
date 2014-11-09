// NMMonthlyGeneralJournalsBL.cs

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
    public class NMMonthlyGeneralJournalsBL
    {
        private readonly ISession Session;

        public NMMonthlyGeneralJournalsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMMonthlyGeneralJournalsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMMonthlyGeneralJournalsWSI callSingleBL(NMMonthlyGeneralJournalsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                //case "DEL_OBJ":
                //    return DeleteObject(wsi);
                case "CLO_MON":
                    //return CloseInventory(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMMonthlyGeneralJournalsWSI> callListBL(NMMonthlyGeneralJournalsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMMonthlyGeneralJournalsWSI>();
            }
        }

        private NMMonthlyGeneralJournalsWSI SaveObject(NMMonthlyGeneralJournalsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                MonthlyGeneralJournals obj = new MonthlyGeneralJournals();
                MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);
                
                obj = mgjAccesser.GetAllmonthlygeneraljournalsByID(wsi.MGJ.JournalId.ToString(), true);
                if (obj == null)
                {
                    wsi.MGJ.CreatedDate = DateTime.Now;
                    wsi.MGJ.CreatedBy = wsi.Filter.ActionBy;
                    wsi.MGJ.ModifiedDate = DateTime.Now;
                    wsi.MGJ.ModifiedBy = wsi.Filter.ActionBy;
                    mgjAccesser.InsertMonthlygeneraljournals(wsi.MGJ);
                }
                else
                {   
                    wsi.MGJ.CreatedDate = obj.CreatedDate;
                    wsi.MGJ.CreatedBy = obj.CreatedBy;
                    wsi.MGJ.ModifiedDate = DateTime.Now;
                    wsi.MGJ.ModifiedBy = wsi.Filter.ActionBy;
                    mgjAccesser.Updatemonthlygeneraljournals(wsi.MGJ);
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

        private NMMonthlyGeneralJournalsWSI SelectObject(NMMonthlyGeneralJournalsWSI wsi)
        {
            MonthlyGeneralJournalsAccesser Accesser = new MonthlyGeneralJournalsAccesser(Session);
            MonthlyGeneralJournals obj;
            obj = Accesser.GetAllmonthlygeneraljournalsByID(wsi.MGJ.JournalId.ToString(), true);
            if (obj != null)
            {
                wsi.MGJ = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private List<NMMonthlyGeneralJournalsWSI> SearchObject(NMMonthlyGeneralJournalsWSI wsi)
        {
            List<NMMonthlyGeneralJournalsWSI> ListWSI = new List<NMMonthlyGeneralJournalsWSI>();
            try
            {
                Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
                String strCriteria = "";
                if (wsi.MGJ != null)
                {
                    if (!String.IsNullOrEmpty(wsi.MGJ.AccountId))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.AccountId = :AccountId";
                        ListCriteria.Add("AccountId", wsi.MGJ.AccountId);
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.IssueId))
                    {
                        AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);

                        if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("SalesInvoices", true).PrefixOfDefaultValueForId))// "SI"))
                        {   
                            wsi.MGJ.SIID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.SIID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.SIID);
                        }
                        else if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("PurchaseInvoices", true).PrefixOfDefaultValueForId))
                        {   
                            wsi.MGJ.PIID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.PIID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.PIID);
                        }
                        else if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("Payments", true).PrefixOfDefaultValueForId))
                        {
                            wsi.MGJ.PADID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.PADID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.PADID);
                        }
                        else if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("Receipts", true).PrefixOfDefaultValueForId))
                        {
                            wsi.MGJ.RPTID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.RPTID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.RPTID);
                        }
                        else if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("Exports", true).PrefixOfDefaultValueForId))
                        {   
                            wsi.MGJ.EXID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.EXID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.EXID);
                        }
                        else if (wsi.MGJ.IssueId.Contains(AutomaticValueAccesser.GetAutomaticValuesByObjectName("Imports", true).PrefixOfDefaultValueForId))//"I"))
                        {
                            wsi.MGJ.IMID = wsi.MGJ.IssueId;
                            wsi.MGJ.IssueId = "";
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.IMID = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.IMID);
                        }
                        else
                        {
                            strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                            strCriteria += " O.IssueId = :IssueId";
                            ListCriteria.Add("IssueId", wsi.MGJ.IssueId);
                        }
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.PartnerId))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.PartnerId = :PartnerId";
                        ListCriteria.Add("PartnerId", wsi.MGJ.PartnerId);
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.BankId))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.BankId = :BankId";
                        ListCriteria.Add("BankId", wsi.MGJ.BankId);
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.ProductId))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.ProductId = :ProductId";
                        ListCriteria.Add("ProductId", wsi.MGJ.ProductId);
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.ActionBy))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.ActionBy = :ActionBy";
                        ListCriteria.Add("ActionBy", wsi.MGJ.ActionBy);
                    }
                    if (!String.IsNullOrEmpty(wsi.MGJ.StockId))
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " O.StockId = :StockId";
                        ListCriteria.Add("StockId", wsi.MGJ.StockId);
                    }
                }
                if (!string.IsNullOrEmpty(wsi.Filter.FromDate))
                {   
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IssueDate >= :FromDate";
                    ListCriteria.Add("FromDate", DateTime.Parse(wsi.Filter.FromDate));                    
                }
                if (!string.IsNullOrEmpty(wsi.Filter.ToDate))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IssueDate < :ToDate";
                    ListCriteria.Add("ToDate", DateTime.Parse(wsi.Filter.ToDate).AddDays(1));
                }
                
                if (wsi.Filter.SortField != null && wsi.Filter.SortOrder != "")
                {
                    strCriteria += " ORDER BY O." + wsi.Filter.SortField + " " + wsi.Filter.SortOrder;
                }
                else
                {
                    strCriteria += " ORDER BY O.IssueDate, O.CreatedDate";
                }
                if (!string.IsNullOrEmpty(wsi.Filter.GroupBy))
                {
                    strCriteria += " GROUP BY O." + wsi.Filter.GroupBy;
                }

                String strCmd = "SELECT O FROM MonthlyGeneralJournals AS O" + strCriteria;
                IQuery query = Session.CreateQuery(strCmd);
                foreach (var Item in ListCriteria)
                {
                    query.SetParameter(Item.Key, Item.Value);
                }
                MonthlyGeneralJournalsAccesser Accesser = new MonthlyGeneralJournalsAccesser(Session);
                IList<MonthlyGeneralJournals> objs;
                objs = Accesser.GetmonthlygeneraljournalsByQuery(query, false);
                foreach (MonthlyGeneralJournals obj in objs)
                {
                    wsi = new NMMonthlyGeneralJournalsWSI();
                    wsi.MGJ = obj;
                    ListWSI.Add(wsi);
                }

                if (objs.Count > 0)
                    ListWSI[0].Filter.TotalRows = objs.Count;
            }
            catch { }
            return ListWSI;
        }
    }
}
