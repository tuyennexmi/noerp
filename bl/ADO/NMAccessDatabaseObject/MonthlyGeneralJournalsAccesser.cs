// MonthlyGeneralJournalsAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System.Collections.Generic;
using System.Text;
using System;
using NHibernate;
using NHibernate.Cfg;
using NEXMI;

namespace NEXMI
{
    public class MonthlyGeneralJournalsAccesser
    {
        ISession session;

        public MonthlyGeneralJournalsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMonthlygeneraljournals(MonthlyGeneralJournals monthlygeneraljournalsX)
        {
            session.Merge(monthlygeneraljournalsX);
        }

        //public void InsertMonthlygeneraljournals(MonthlyGeneralJournals monthlygeneraljournalsX)
        //{   
        //    string cmd = "select c from MonthlyGeneralJournals as c where c.AccountId = :x ";
                        
        //    /* trường hợp số dư đầu kỳ phải so sánh:   
        //     *      công ty, khách hàng, nhà cung cấp (để phân biệt của ai)
        //     *      sản phẩm, ngân hàng, kho, đơn vị tính (phân biệt cái gì: sp, tiền mặt, tiền gửi...)
        //     *      ngày (để biết kỳ nào)
        //     */
        //    IQuery query;
        //    if (!string.IsNullOrEmpty(monthlygeneraljournalsX.IssueId))
        //    {
        //        cmd += " and c.IssueId = :IssueId";

        //        if (monthlygeneraljournalsX.IssueDate != null)
        //            cmd += " and c.IssueDate = :IssueDate";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PartnerId))
        //            cmd += " and c.PartnerId = :PartnerId";
        //        else
        //            cmd += " and c.PartnerId = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.BankId))
        //            cmd += " and c.BankId = :BankId";
        //        else
        //            cmd += " and c.BankId = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.ProductId))
        //            cmd += " and c.ProductId = :ProductId";
        //        else
        //            cmd += " and c.ProductId = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.StockId))
        //            cmd += " and c.StockId = :StockId";
        //        else
        //            cmd += " and c.StockId = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.UnitId))
        //            cmd += " and c.UnitId = :UnitId";
        //        else
        //            cmd += " and c.UnitId = NULL";

        //        query = session.CreateQuery(cmd);

        //        query.SetString("x", monthlygeneraljournalsX.AccountId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.IssueId))
        //            query.SetString("IssueId", monthlygeneraljournalsX.IssueId);

        //        if (monthlygeneraljournalsX.IssueDate != null)
        //            query.SetDateTime("IssueDate", monthlygeneraljournalsX.IssueDate);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PartnerId))
        //            query.SetString("PartnerId", monthlygeneraljournalsX.PartnerId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.BankId))
        //            query.SetString("BankId", monthlygeneraljournalsX.BankId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.ProductId))
        //            query.SetString("ProductId", monthlygeneraljournalsX.ProductId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.StockId))
        //            query.SetString("StockId", monthlygeneraljournalsX.StockId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.UnitId))
        //            query.SetString("UnitId", monthlygeneraljournalsX.UnitId);

        //    }
        //    else
        //    {
        //        /*  Truong hop hach toan binh thuong, phai xac dinh thuoc phieu nao
        //         */
        //        cmd += " and c.IssueId = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PIID))
        //            cmd += " and c.PIID = :PIID";
        //        else
        //            cmd += " and c.PIID = NULL";
        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.SIID))
        //            cmd += " and c.SIID = :SIID";
        //        else
        //            cmd += " and c.SIID = NULL";
        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.IMID))
        //            cmd += " and c.IMID = :IMID";
        //        else
        //            cmd += " and c.IMID = NULL";
        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.EXID))
        //            cmd += " and c.EXID = :EXID";
        //        else
        //            cmd += " and c.EXID = NULL";
        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PADID))
        //            cmd += " and c.PADID = :PADID";
        //        else
        //            cmd += " and c.PADID = NULL";
        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.RPTID))
        //            cmd += " and c.RPTID = :RPTID";
        //        else
        //            cmd += " and c.RPTID = NULL";

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.ProductId))
        //            cmd += " and c.ProductId = :ProductId";
        //        else
        //            cmd += " and c.ProductId = NULL";

        //        query = session.CreateQuery(cmd);

        //        query.SetString("x", monthlygeneraljournalsX.AccountId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.IssueId))
        //            query.SetString("IssueId", monthlygeneraljournalsX.IssueId);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PIID))
        //            query.SetString("PIID", monthlygeneraljournalsX.PIID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.SIID))
        //            query.SetString("SIID", monthlygeneraljournalsX.SIID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.IMID))
        //            query.SetString("IMID", monthlygeneraljournalsX.IMID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.EXID))
        //            query.SetString("EXID", monthlygeneraljournalsX.EXID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.PADID))
        //            query.SetString("PADID", monthlygeneraljournalsX.PADID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.RPTID))
        //            query.SetString("RPTID", monthlygeneraljournalsX.RPTID);

        //        if (!string.IsNullOrEmpty(monthlygeneraljournalsX.ProductId))
        //            query.SetString("ProductId", monthlygeneraljournalsX.ProductId);
        //    }
            
        //    IList<MonthlyGeneralJournals> obj = this.GetmonthlygeneraljournalsByQuery(query, true);
        //    // chưa tồn tại => tạo mới
        //    if (obj.Count == 0)
        //    {
        //        session.Merge(monthlygeneraljournalsX);
        //    }
        //    else if (obj.Count == 1)    //=> cập nhật
        //    {
        //        monthlygeneraljournalsX.JournalId = obj[0].JournalId;
        //        monthlygeneraljournalsX.CreatedDate = obj[0].CreatedDate;
        //        monthlygeneraljournalsX.CreatedBy = obj[0].CreatedBy;

        //        session.Update(monthlygeneraljournalsX);
        //    }
        //}

        public void Updatemonthlygeneraljournals(MonthlyGeneralJournals monthlygeneraljournalsX)
        {
            session.Update(monthlygeneraljournalsX);
        }

        public void Deletemonthlygeneraljournals(MonthlyGeneralJournals monthlygeneraljournalsX)
        {
            session.Delete(monthlygeneraljournalsX);
        }

        public IList<MonthlyGeneralJournals> GetAllmonthlygeneraljournals(Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MonthlyGeneralJournals as m");
            IList<MonthlyGeneralJournals> list = query.List<MonthlyGeneralJournals>();
            if (evict)
            {
                foreach (MonthlyGeneralJournals s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MonthlyGeneralJournals GetAllmonthlygeneraljournalsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MonthlyGeneralJournals as c where c.JournalId = :x");
            query.SetString("x", id);
            MonthlyGeneralJournals s = (MonthlyGeneralJournals)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public MonthlyGeneralJournals GetAllmonthlygeneraljournalsByAccountIdAndIssueId(String AccountId, String IssueId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MonthlyGeneralJournals as c where c.AccountId = :x and c.IssueId = :y");
            query.SetString("x", AccountId);
            query.SetString("y", IssueId);
            MonthlyGeneralJournals s = (MonthlyGeneralJournals)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MonthlyGeneralJournals> GetmonthlygeneraljournalsByQuery(IQuery query, Boolean evict)
        {
            IList<MonthlyGeneralJournals> list = query.List<MonthlyGeneralJournals>();
            if (evict)
            {
                foreach (MonthlyGeneralJournals s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<MonthlyGeneralJournals> GetAllDetailsByCloseMonthId(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MonthlyGeneralJournals as m where m.CloseMonth = :x");
            query.SetString("x", id);
            IList<MonthlyGeneralJournals> list = query.List<MonthlyGeneralJournals>();
            if (evict)
            {
                foreach (MonthlyGeneralJournals s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<MonthlyGeneralJournals> GetAllDetailsByCloseMonthIdAndAccountId(String id, String AccountId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MonthlyGeneralJournals as m where m.CloseMonth = :x and m.AccountId = :y");
            query.SetString("x", id);
            query.SetString("y", AccountId);
            IList<MonthlyGeneralJournals> list = query.List<MonthlyGeneralJournals>();
            if (evict)
            {
                foreach (MonthlyGeneralJournals s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        #region IC ProductsInStocks Close Month

        public double ICGetCloseMonthBeginAmount(string stockId, string productId, string closeMonth)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IssueId = 'SDDK' and m.StockId = :s and m.ProductId = :p and m.CloseMonth = :cm ");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetString("cm", closeMonth);
            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetCloseMonthImportAmount(string stockId, string productId, string closeMonth)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IMID != '' and m.StockId = :s and m.ProductId = :p  and m.CloseMonth = :cm ");
            
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetString("cm", closeMonth);

            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetCloseMonthExportAmount(string stockId, string productId, string closeMonth)
        {
            IQuery query = this.session.CreateSQLQuery("select SUM(m.ExportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p  and m.CloseMonth = :cm ");
            query.SetString("acc", "1561");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetString("cm", closeMonth);
            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        #endregion
        
        #region Open Month

        public double ICGetOpenBeginAmount(string stockId, string productId)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IssueId = 'SDDK' and m.StockId = :s and m.ProductId = :p and m.Status = :st ");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetInt32("st", 0);
            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetOpenImportAmount(string stockId, string productId)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IMID != '' and m.StockId = :s and m.ProductId = :p  and m.Status = :st ");

            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetInt32("st", 0);

            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetOpenExportAmount(string stockId, string productId)
        {
            IQuery query = this.session.CreateSQLQuery("select SUM(m.ExportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p  and m.Status = :st ");
            query.SetString("acc", "1561");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetInt32("st", 0);

            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetBeginAmountInPeriod(string stockId, string productId, string from, string to)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IssueId = 'SDDK' and m.StockId = :s and m.ProductId = :p and m.IssueDate between :from and :to ");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetParameter("from", DateTime.Parse(from));
            query.SetParameter("to", DateTime.Parse(to));
            double Begin = 0;
            try
            {
                Begin = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return Begin;
        }

        public double ICGetImportAmountInPeriod(string stockId, string productId, string from, string to)
        {
            IQuery query = session.CreateSQLQuery("select SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.IMID != '' and m.StockId = :s and m.ProductId = :p  and m.IssueDate between :from and :to ");

            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetParameter("from", DateTime.Parse(from));
            query.SetParameter("to", DateTime.Parse(to));

            double import = 0;
            try
            {
                import = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return import;
        }

        public double ICGetExportAmountInPeriod(string stockId, string productId, string from, string to)
        {
            IQuery query = this.session.CreateSQLQuery("select SUM(m.ExportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p  and m.IssueDate between :from and :to ");
            query.SetString("acc", "1561");
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetParameter("from", DateTime.Parse(from));
            query.SetParameter("to", DateTime.Parse(to));

            double Export = 0;
            try
            {
                Export = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return Export;
        }

        public double ICGetDebitAmountInPeriod(string accountId, string stockId, string productId, string from, string to)
        {
            IQuery query = this.session.CreateSQLQuery("select SUM(m.DebitAmount) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p  and m.IssueDate between :from and :to ");
            query.SetString("acc", accountId);
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetParameter("from", DateTime.Parse(from));
            query.SetParameter("to", DateTime.Parse(to));

            double Debit = 0;
            try
            {
                Debit = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return Debit;
        }

        public double ICGetCreditAmountInPeriod(string accountId, string stockId, string productId, string from, string to)
        {
            IQuery query = this.session.CreateSQLQuery("select SUM(m.CreditAmount) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p  and m.IssueDate between :from and :to ");
            query.SetString("acc", accountId);
            query.SetString("s", stockId);
            query.SetString("p", productId);
            query.SetParameter("from", DateTime.Parse(from));
            query.SetParameter("to", DateTime.Parse(to));

            double Credit = 0;
            try
            {
                Credit = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return Credit;
        }

        #endregion

        public double ICExportPriceCalculate(string stockId, string productId, string accountId)
        {   
            IQuery query = session.CreateSQLQuery("select SUM(m.DebitAmount)/SUM(m.ImportQuantity) as EX "
                    + " from MonthlyGeneralJournals m where m.AccountId = :acc and m.StockId = :s and m.ProductId = :p");
            query.SetString("acc", accountId);
            query.SetString("s", stockId);
            query.SetString("p", productId);
            double price = 0;
            try
            {
                price = double.Parse(query.UniqueResult().ToString());
            }
            catch { }

            return price;
        }

        public double ICGetTransferPrice(string exportId, string productId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select m.CreditAmount/m.ExportQuantity as EX "
                    + " from MonthlyGeneralJournals m where m.EXID = :s and m.ProductId = :p");

            query.SetString("s", exportId);
            query.SetString("p", productId);
            double price = 0;
            try
            {
                price = double.Parse(query.UniqueResult().ToString());
            }
            catch { }
            return price;
        }

        #region delete by master id

        public void DeletemonthlygeneraljournalsByReceipt(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where RPTID = :x");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        public void DeletemonthlygeneraljournalsByPayment(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where PADID = :x");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        public void DeletemonthlygeneraljournalsByImport(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where IMID = :x");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        public void DeletemonthlygeneraljournalsByExport(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where EXID = :x");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        public void DeletemonthlygeneraljournalsBySalesInvoice(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where SIID = :x and RPTID = NULL");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        public void DeletemonthlygeneraljournalsByPurchaseInvoice(String id)
        {
            IQuery query = session.CreateQuery("delete from MonthlyGeneralJournals where PIID = :x and PADID = NULL");
            query.SetString("x", id);
            query.ExecuteUpdate();
        }

        #endregion

    }
}
