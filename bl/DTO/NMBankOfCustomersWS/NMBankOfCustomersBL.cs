// NMBankOfCustomersBL.cs

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
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMBankOfCustomersBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMBankOfCustomersBL()
        {

        }

        public NMBankOfCustomersWSI callSingleBL(NMBankOfCustomersWSI wsi)
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

        public List<NMBankOfCustomersWSI> callListBL(NMBankOfCustomersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMBankOfCustomersWSI>();
            }
        }

        public NMBankOfCustomersWSI SelectObject(NMBankOfCustomersWSI wsi)
        {
            BankOfCustomersAccesser Accesser = new BankOfCustomersAccesser(Session);
            BanksAccesser BankAccesser = new BanksAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            BankOfCustomers obj;
            obj = Accesser.GetAllBankofcustomersByID(wsi.BankOfCustomer.BankId, wsi.BankOfCustomer.CustomerId, true);
            if (obj != null)
            {
                wsi.BankOfCustomer = obj;
                wsi.Bank = BankAccesser.GetAllBanksByID(obj.BankId, true);
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMBankOfCustomersWSI SaveObject(NMBankOfCustomersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                BankOfCustomersAccesser Accesser = new BankOfCustomersAccesser(Session);
                BankOfCustomers obj = Accesser.GetAllBankofcustomersByID(wsi.BankOfCustomer.BankId, wsi.BankOfCustomer.CustomerId, true);
                if (obj != null)
                {
                    wsi.WsiError = "Số tài khoản này đã tồn tại!";
                }
                else
                {
                    Accesser.InsertBankofcustomers(wsi.BankOfCustomer);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMBankOfCustomersWSI DeleteObject(NMBankOfCustomersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                BankOfCustomersAccesser Accesser = new BankOfCustomersAccesser(Session);
                BankOfCustomers obj = Accesser.GetAllBankofcustomersByID(wsi.BankOfCustomer.BankId, wsi.BankOfCustomer.CustomerId, true);
                if (obj != null)
                {
                    Accesser.DeleteBankofcustomers(obj);
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
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public List<NMBankOfCustomersWSI> SearchObject(NMBankOfCustomersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.BankOfCustomer != null)
            {
                if (!string.IsNullOrEmpty(wsi.BankOfCustomer.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.BankOfCustomer.CustomerId);
                }
                if (!string.IsNullOrEmpty(wsi.BankOfCustomer.BankId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.BankId = :BankId";
                    ListCriteria.Add("BankId", wsi.BankOfCustomer.BankId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.CustomerId LIKE :Keyword OR O.BankId LIKE :Keyword)";
            }
            string strCmdCounter = "SELECT COUNT(O.BankId) FROM BankOfCustomers AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            string strCmd = "SELECT O FROM BankOfCustomers AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMBankOfCustomersWSI> ListWSI = new List<NMBankOfCustomersWSI>();
            BankOfCustomersAccesser Accesser = new BankOfCustomersAccesser(Session);
            BanksAccesser BankAccesser = new BanksAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            IList<BankOfCustomers> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetBankofcustomersByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetBankofcustomersByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (BankOfCustomers obj in objs)
            {
                wsi = new NMBankOfCustomersWSI();
                wsi.BankOfCustomer = obj;
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.Bank = BankAccesser.GetAllBanksByID(obj.BankId, true);
                wsi.WsiError = "";
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
