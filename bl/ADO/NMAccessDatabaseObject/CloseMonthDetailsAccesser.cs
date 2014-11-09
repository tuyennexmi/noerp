// CloseMonthDetailsAccesser.cs

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
    public class CloseMonthDetailsAccesser
    {
        ISession session;

        public CloseMonthDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCloseMonthDetails(CloseMonthDetails CloseMonthDetailsX)
        {   
            string cmd = "select c from CloseMonthDetails as c where c.AccountId = :x ";
            
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.IssueId))
                cmd += " and c.IssueId = :iss";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PIID))
                cmd += " and c.PIID = :pi";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.SIID))
                cmd += " and c.SIID = :si";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.IMID))
                cmd += " and c.IMID = :im";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.EXID))
                cmd += " and c.EXID = :ex";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PADID))
                cmd += " and c.PADID = :pa";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.RPTID))
                cmd += " and c.RPTID = :rp";

            if (!string.IsNullOrEmpty(CloseMonthDetailsX.ProductId))
                cmd += " and c.ProductId = :z";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.StockId))
                cmd += " and c.StockId = :k";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.BankId))
                cmd += " and c.BankId = :b";
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PartnerId))
                cmd += " and c.PartnerId = :p";

            IQuery query = session.CreateQuery(cmd);
            query.SetString("x", CloseMonthDetailsX.AccountId);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.IssueId))
                query.SetString("iss", CloseMonthDetailsX.IssueId);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PIID))
                query.SetString("pi", CloseMonthDetailsX.PIID);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.SIID))
                query.SetString("si", CloseMonthDetailsX.SIID);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.IMID))
                query.SetString("im", CloseMonthDetailsX.IMID);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.EXID))
                query.SetString("ex", CloseMonthDetailsX.EXID);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PADID))
                query.SetString("pa", CloseMonthDetailsX.PADID);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.RPTID))
                query.SetString("rp", CloseMonthDetailsX.RPTID);

            if (!string.IsNullOrEmpty(CloseMonthDetailsX.ProductId))
                query.SetString("z", CloseMonthDetailsX.ProductId);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.StockId))
                query.SetString("k", CloseMonthDetailsX.StockId);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.BankId))
                query.SetString("b", CloseMonthDetailsX.BankId);
            if (!string.IsNullOrEmpty(CloseMonthDetailsX.PartnerId))
                query.SetString("p", CloseMonthDetailsX.PartnerId);

            IList<CloseMonthDetails> obj = this.GetCloseMonthDetailsByQuery(query, true);
            // chưa tồn tại => tạo mới
            if (obj.Count == 0)
            {
                session.Merge(CloseMonthDetailsX);
            }
            else if (obj.Count == 1)    //=> cập nhật
            {
                CloseMonthDetailsX.Id = obj[0].Id;
                CloseMonthDetailsX.ModifiedDate = DateTime.Now;
                session.Update(CloseMonthDetailsX);
            }
        }

        public void UpdateCloseMonthDetails(CloseMonthDetails CloseMonthDetailsX)
        {
            session.Update(CloseMonthDetailsX);
        }

        public void DeleteCloseMonthDetails(CloseMonthDetails CloseMonthDetailsX)
        {
            session.Delete(CloseMonthDetailsX);
        }

        public IList<CloseMonthDetails> GetAllCloseMonthDetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from CloseMonthDetails as m");
            IList<CloseMonthDetails> list = query.List<CloseMonthDetails>();
            if (evict)
            {
                foreach (CloseMonthDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<CloseMonthDetails> GetAllDetailsByCloseMonthId(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from CloseMonthDetails as m where m.CloseMonth = :x");
            query.SetString("x", id);
            IList<CloseMonthDetails> list = query.List<CloseMonthDetails>();
            if (evict)
            {
                foreach (CloseMonthDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<CloseMonthDetails> GetAllDetailsByCloseMonthIdAndAccountId(String id, String AccountId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from CloseMonthDetails as m where m.CloseMonth = :x and m.AccountId = :y");
            query.SetString("x", id);
            query.SetString("y", AccountId);
            IList<CloseMonthDetails> list = query.List<CloseMonthDetails>();
            if (evict)
            {
                foreach (CloseMonthDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public CloseMonthDetails GetAllCloseMonthDetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CloseMonthDetails as c where c.Id = :x");
            query.SetString("x", id);
            CloseMonthDetails s = (CloseMonthDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public CloseMonthDetails GetAllCloseMonthDetailsByAccountIdAndIssueId(String AccountId, String IssueId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CloseMonthDetails as c where c.AccountId = :x and c.IssueId = :y");
            query.SetString("x", AccountId);
            query.SetString("y", IssueId);
            CloseMonthDetails s = (CloseMonthDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<CloseMonthDetails> GetCloseMonthDetailsByQuery(IQuery query, Boolean evict)
        {
            IList<CloseMonthDetails> list = query.List<CloseMonthDetails>();
            if (evict)
            {
                foreach (CloseMonthDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
