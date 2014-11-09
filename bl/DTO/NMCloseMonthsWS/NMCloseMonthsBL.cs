// NMCloseMonthsBL.cs

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
using System.Linq.Expressions;
namespace NEXMI
{
    public class NMCloseMonthsBL
    {
        private readonly ISession Session;
        public NMCloseMonthsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMCloseMonthsBL(ISession session)
        {
            this.Session = session;
        }

        public NMCloseMonthsWSI callSingleBL(NMCloseMonthsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "GET_DTL":
                    return GetDetails(wsi);
                case "GET_IC":
                    return GetICDetails(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMCloseMonthsWSI> callListBL(NMCloseMonthsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMCloseMonthsWSI>();
            }
        }

        public NMCloseMonthsWSI SelectObject(NMCloseMonthsWSI wsi)
        {
            CloseMonthsAccesser Acceser = new CloseMonthsAccesser(Session);
            CloseMonths obj;
            obj = Acceser.GetAllCloseMonthsByID(wsi.CloseMonth.CloseMonth, false);
            if (obj != null)
            {
                wsi.CloseMonth = obj;
                //wsi.BeginingAmount = obj.BeginAmount.ToString();
                //wsi.ReceiptAmount = obj.ReceiptAmount.ToString();
                //wsi.PaymentAmount = obj.PaidAmount.ToString();
                //wsi.EndingAmount = obj.EndAmount.ToString();
                //wsi.CreatedDate = obj.CreatedDate.ToString();
                //wsi.CreatedBy = obj.CreatedBy;
                wsi.WsiError = "";
                //IEnumerable<CloseMonthCashBalanceDetails> Details = obj.DetailList.Cast<CloseMonthCashBalanceDetails>();
                //foreach (CloseMonthCashBalanceDetails Item in Details)
                //{
                //    wsi.BalanceIdList.Add(Item.BalanceId);
                //    wsi.CloseMonthList.Add(Item.CloseMonth);
                //    wsi.IssuedIdList.Add(Item.IssueId);
                //    wsi.IssuedDateList.Add(Item.IssueDate);
                //    wsi.ReceiptAmountList.Add(Item.ReceiptAmount);
                //    wsi.PaymentAmountList.Add(Item.PaymentAmount);
                //    wsi.BalanceAmountList.Add(Item.BalanceAmount);
                //    wsi.DescriptionList.Add(Item.DescriptionInVietnamese);
                //    wsi.CreatedDateList.Add(Item.CreatedDate);
                //    wsi.CreatedByList.Add(Item.CreatedBy);
                //}
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMCloseMonthsWSI GetDetails(NMCloseMonthsWSI wsi)
        {
            MonthlyGeneralJournalsAccesser Accesser = new MonthlyGeneralJournalsAccesser(Session);
            IList<MonthlyGeneralJournals> objs;
            objs = Accesser.GetAllDetailsByCloseMonthIdAndAccountId(wsi.CloseMonth.CloseMonth, wsi.AccountId, true);
            if (objs != null)
            {
                wsi.WsiError = "";
                wsi.Details = objs.ToList();
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMCloseMonthsWSI GetICDetails(NMCloseMonthsWSI wsi)
        {
            ProductInStocksAccesser Accesser = new ProductInStocksAccesser(Session);
            ProductsAccesser ProductAccesser = new ProductsAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(Session);
            IList<ProductInStocks> objs;

            objs = Accesser.GetAllProductinstocks(true);
            foreach (ProductInStocks obj in objs)
            {
                obj.BeginQuantity = mgjAccesser.ICGetCloseMonthBeginAmount(obj.StockId, obj.ProductId, wsi.CloseMonth.CloseMonth);
                obj.ImportQuantity = mgjAccesser.ICGetCloseMonthImportAmount(obj.StockId, obj.ProductId, wsi.CloseMonth.CloseMonth);
                obj.ExportQuantity = mgjAccesser.ICGetCloseMonthExportAmount(obj.StockId, obj.ProductId, wsi.CloseMonth.CloseMonth);

                obj.CostPrice = mgjAccesser.ICExportPriceCalculate(obj.StockId, obj.ProductId, NMCommon.GetProductType(obj.ProductId));                
            }

            wsi.PIS = objs.ToList();

            return wsi;
        }

        public NMCloseMonthsWSI SaveObject(NMCloseMonthsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                CloseMonthsAccesser Accesser = new CloseMonthsAccesser(Session);
                CloseMonths obj = Accesser.GetAllCloseMonthsByID(wsi.CloseMonth.CloseMonth, true);
                if (obj != null)
                {
                    Session.Evict(obj);

                    wsi.CloseMonth.CreatedDate = obj.CreatedDate;
                    wsi.CloseMonth.CreatedBy = obj.CreatedBy;
                    wsi.CloseMonth.ModifiedDate = DateTime.Now;
                    wsi.CloseMonth.ModifiedBy = wsi.Filter.ActionBy;

                    Accesser.UpdateCloseMonths(wsi.CloseMonth);

                    //ghi log
                    //String rs = wsi.CompareTo(obj);
                    //if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.CloseMonth.CloseMonth, "cập nhật thông tin", rs, wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    wsi.CloseMonth.CreatedDate = DateTime.Now;
                    wsi.CloseMonth.CreatedBy = wsi.Filter.ActionBy;
                    wsi.CloseMonth.ModifiedDate = DateTime.Now;
                    wsi.CloseMonth.ModifiedBy = wsi.Filter.ActionBy;

                    Accesser.InsertCloseMonths(wsi.CloseMonth);

                    NMMessagesBL.SaveMessage(Session, wsi.CloseMonth.CloseMonth, "khởi tạo tài liệu", "", wsi.Filter.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }

                //lấy tất cả record NKC
                NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI mgjWSI = new NMMonthlyGeneralJournalsWSI();
                mgjWSI.Mode = "SRC_OBJ";
                //int year = DateTime.Today.Year;
                //int month = DateTime.Today.Month;
                //WSI.Filter.FromDate = new DateTime(year, month, 01).ToString();
                //WSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

                mgjWSI.Filter.FromDate = wsi.Filter.FromDate;
                mgjWSI.Filter.ToDate = wsi.Filter.ToDate;

                List<NMMonthlyGeneralJournalsWSI> lst = BL.callListBL(mgjWSI);

                // xu ly so lieu chu ky cu

                //CloseMonthDetails closeMonthDetail;
                //CloseMonthDetailsAccesser closeMonthDetailsAccesser = new CloseMonthDetailsAccesser(this.Session);
                MonthlyGeneralJournalsAccesser monthlyGeneralJournalsAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

                foreach (var item in lst)
                {
                    #region chen vao closemonth

                    //closeMonthDetail = new CloseMonthDetails();
                    //closeMonthDetail.CloseMonth = wsi.CloseMonth.CloseMonth;
                    //closeMonthDetail.IssueDate = item.MGJ.IssueDate;
                    //closeMonthDetail.PartnerId = item.MGJ.PartnerId;
                    //closeMonthDetail.IssueId = item.MGJ.IssueId;
                    //closeMonthDetail.PIID = item.MGJ.PIID;
                    //closeMonthDetail.IMID = item.MGJ.IMID;
                    //closeMonthDetail.PADID = item.MGJ.PADID;
                    //closeMonthDetail.SIID = item.MGJ.SIID;
                    //closeMonthDetail.EXID = item.MGJ.EXID;
                    //closeMonthDetail.RPTID = item.MGJ.RPTID;
                    //closeMonthDetail.ProductId = item.MGJ.ProductId;
                    //closeMonthDetail.AccountId = item.MGJ.AccountId;
                    //closeMonthDetail.ActionBy = item.MGJ.ActionBy;
                    //closeMonthDetail.StockId = item.MGJ.StockId;
                    //closeMonthDetail.ImportQuantity = item.MGJ.ImportQuantity;
                    //closeMonthDetail.ExportQuantity = item.MGJ.ExportQuantity;
                    //closeMonthDetail.UnitId = item.MGJ.UnitId;
                    //closeMonthDetail.DebitAmount = item.MGJ.DebitAmount;
                    //closeMonthDetail.CreditAmount = item.MGJ.CreditAmount;
                    //closeMonthDetail.CurrencyId = item.MGJ.CurrencyId;
                    //closeMonthDetail.ExchangeRate = item.MGJ.ExchangeRate;
                    //closeMonthDetail.BankId = item.MGJ.BankId;
                    //closeMonthDetail.Descriptions = item.MGJ.Descriptions;
                    //closeMonthDetail.Status = item.MGJ.Status;
                    //closeMonthDetail.IsBegin = item.MGJ.IsBegin;
                    //closeMonthDetail.CreatedDate = item.MGJ.CreatedDate;
                    //closeMonthDetail.CreatedBy = item.MGJ.CreatedBy;
                    //closeMonthDetail.ModifiedBy = item.MGJ.ModifiedBy;
                    //closeMonthDetail.ModifiedDate = item.MGJ.ModifiedDate;

                    //closeMonthDetailsAccesser.InsertCloseMonthDetails(closeMonthDetail);

                    #endregion

                    item.MGJ.CloseMonth = wsi.CloseMonth.CloseMonth;
                    item.MGJ.Status = 1;
                    monthlyGeneralJournalsAccesser.Updatemonthlygeneraljournals(item.MGJ);

                    #region Xoa du lieu monthly

                    //monthlyGeneralJournalsAccesser.Deletemonthlygeneraljournals(item.MGJ);

                    #endregion
                }

                // tinh toan so dau ky cho chu ky moi
                DateTime startNewPeriod = DateTime.Parse(mgjWSI.Filter.FromDate).AddMonths(1);
                //if (DateTime.Parse(mgjWSI.Filter.FromDate).Month == 12)
                //    startNewPeriod = startNewPeriod.AddYears(1);

                #region tính tồn quỹ

                // tiền mặt
                MonthlyGeneralJournals mgj = new MonthlyGeneralJournals();
                mgj.IssueDate = startNewPeriod;
                mgj.IssueId = "SDDK";
                mgj.PartnerId = "COMPANY";
                mgj.CurrencyId = "VND";
                mgj.ExchangeRate = 1;
                mgj.IsBegin = true;
                mgj.Descriptions = "Số dư đầu kỳ.";
                mgj.AccountId = "1111";
                mgj.DebitAmount = lst.Where(i => i.MGJ.AccountId == mgj.AccountId).Sum(m => m.MGJ.DebitAmount - m.MGJ.CreditAmount);
                mgj.CreatedDate = DateTime.Now;
                mgj.CreatedBy = wsi.Filter.ActionBy;
                mgj.ModifiedDate = DateTime.Now;
                mgj.ModifiedBy = wsi.Filter.ActionBy;
                monthlyGeneralJournalsAccesser.InsertMonthlygeneraljournals(mgj);
                // ngân hàng
                BanksAccesser banksAccesser = new NEXMI.BanksAccesser(this.Session);
                IList<Banks> banks = banksAccesser.GetAllBanks(true);
                double bankBalance = 0;
                foreach (var bank in banks)
                {
                    bankBalance = lst.Where(i => i.MGJ.AccountId == "1121" & i.MGJ.BankId == bank.Id).Sum(m => m.MGJ.DebitAmount - m.MGJ.CreditAmount);
                    if (bankBalance != 0)
                    {
                        mgj = new MonthlyGeneralJournals();
                        mgj.IssueDate = startNewPeriod;
                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = "COMPANY";
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";
                        mgj.AccountId = "1121";
                        mgj.BankId = bank.Id;
                        mgj.DebitAmount = bankBalance;// lst.Where(i => i.MGJ.AccountId == mgj.AccountId & i.MGJ.BankId == bank.Id).Sum(m => m.MGJ.DebitAmount - m.MGJ.CreditAmount);
                        mgj.CreatedDate = DateTime.Now;
                        mgj.CreatedBy = wsi.Filter.ActionBy;
                        mgj.ModifiedDate = DateTime.Now;
                        mgj.ModifiedBy = wsi.Filter.ActionBy;
                        monthlyGeneralJournalsAccesser.InsertMonthlygeneraljournals(mgj);
                    }
                }

                #endregion
                //cong no khach hang / nha cung cap
                CustomersAccesser customersAccesser = new CustomersAccesser();
                IList<Customers> customers = customersAccesser.GetAllCustomers(true);
                double balanced = 0;

                foreach (var item in customers)
                {
                    #region Nợ phải thu

                    balanced = lst.Where(i => i.MGJ.AccountId == "131" & i.MGJ.PartnerId == item.CustomerId).Sum(m => m.MGJ.DebitAmount - m.MGJ.CreditAmount);
                    if (balanced != 0)
                    {
                        mgj = new MonthlyGeneralJournals();
                        mgj.IssueDate = startNewPeriod;
                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = item.CustomerId;
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";
                        mgj.AccountId = "131";
                        mgj.DebitAmount = balanced;
                        mgj.CreatedDate = DateTime.Now;
                        mgj.CreatedBy = wsi.Filter.ActionBy;
                        mgj.ModifiedDate = DateTime.Now;
                        mgj.ModifiedBy = wsi.Filter.ActionBy;
                        monthlyGeneralJournalsAccesser.InsertMonthlygeneraljournals(mgj);
                    }

                    #endregion

                    #region Nợ phải trả

                    balanced = lst.Where(i => i.MGJ.AccountId == "331" & i.MGJ.PartnerId == item.CustomerId).Sum(m => m.MGJ.CreditAmount - m.MGJ.DebitAmount);
                    if (balanced != 0)
                    {
                        mgj = new MonthlyGeneralJournals();
                        mgj.IssueDate = startNewPeriod;
                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = item.CustomerId;
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";
                        mgj.AccountId = "331";
                        mgj.DebitAmount = 0;
                        mgj.CreditAmount = balanced;
                        mgj.CreatedDate = DateTime.Now;
                        mgj.CreatedBy = wsi.Filter.ActionBy;
                        mgj.ModifiedDate = DateTime.Now;
                        mgj.ModifiedBy = wsi.Filter.ActionBy;
                        monthlyGeneralJournalsAccesser.InsertMonthlygeneraljournals(mgj);
                    }
                    #endregion

                }

                #region Tồn kho
                ProductsAccesser productsAccesser = new ProductsAccesser(this.Session);
                IList<Products> products = productsAccesser.GetAllProducts(true);
                StocksAccesser stocksAccesser = new StocksAccesser(this.Session);
                IList<Stocks> stocks = stocksAccesser.GetAllStocks(true);

                foreach (var prod in products)
                {
                    foreach (var stock in stocks)
                    {
                        balanced = lst.Where(i => i.MGJ.AccountId == prod.TypeId & i.MGJ.ProductId == prod.ProductId & i.MGJ.StockId == stock.Id).Sum(m => m.MGJ.ImportQuantity - m.MGJ.ExportQuantity);
                        if (balanced != 0)
                        {
                            mgj = new MonthlyGeneralJournals();
                            mgj.IssueDate = startNewPeriod;
                            mgj.IssueId = "SDDK";
                            mgj.PartnerId = "COMPANY";
                            mgj.ProductId = prod.ProductId;
                            mgj.ActionBy = wsi.Filter.ActionBy;
                            mgj.StockId = stock.Id;
                            mgj.CurrencyId = "VND";
                            mgj.ExchangeRate = 1;
                            mgj.IsBegin = true;
                            mgj.Descriptions = "Số dư đầu kỳ.";
                            mgj.UnitId = prod.ProductUnit;
                            mgj.AccountId = prod.TypeId;
                            mgj.ImportQuantity = balanced;
                            //mgj.ExportQuantity = 0;
                            mgj.DebitAmount = lst.Where(i => i.MGJ.AccountId == prod.TypeId & i.MGJ.ProductId == prod.ProductId & i.MGJ.StockId == stock.Id).Sum(m => m.MGJ.DebitAmount - m.MGJ.CreditAmount);
                            //mgj.CreditAmount = 0;
                            mgj.CreatedDate = DateTime.Now;
                            mgj.CreatedBy = wsi.Filter.ActionBy;
                            mgj.ModifiedDate = DateTime.Now;
                            mgj.ModifiedBy = wsi.Filter.ActionBy;
                            monthlyGeneralJournalsAccesser.InsertMonthlygeneraljournals(mgj);
                        }
                    }
                }

                #endregion

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMCloseMonthsWSI DeleteObject(NMCloseMonthsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                CloseMonthsAccesser Accesser = new CloseMonthsAccesser(Session);
                CloseMonths obj = Accesser.GetAllCloseMonthsByID(wsi.CloseMonth.CloseMonth, false);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteCloseMonths(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục!\nChi tiết lỗi: " + ex.Message;
            }
            return wsi;
        }

        public List<NMCloseMonthsWSI> SearchObject(NMCloseMonthsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            
            if (!string.IsNullOrEmpty(wsi.Filter.FromDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedDate >= :FromDate";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.Filter.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.Filter.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedDate <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.Filter.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.CloseMonth.CreatedBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :CreatedBy";
                ListCriteria.Add("CreatedBy", wsi.CloseMonth.CreatedBy);
            }

            String strCmd = "SELECT O FROM CloseMonths AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMCloseMonthsWSI> ListWSI = new List<NMCloseMonthsWSI>();
            CloseMonthsAccesser Acceser = new CloseMonthsAccesser(Session);
            IList<CloseMonths> objs;
            objs = Acceser.GetCloseMonthsByQuery(query, false);

            foreach (CloseMonths obj in objs)
            {
                wsi = new NMCloseMonthsWSI();
                wsi.CloseMonth = obj;
                wsi.WsiError = "";
                ListWSI.Add(wsi);
            }
            
            if (objs.Count > 0)
                ListWSI[0].Filter.TotalRows = objs.Count;

            return ListWSI;
        }
    }
}
