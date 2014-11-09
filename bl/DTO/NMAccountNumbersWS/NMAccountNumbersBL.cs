// NMAccountNumbersBL.cs

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
    public class NMAccountNumbersBL
    {
        private readonly ISession Session;
        public NMAccountNumbersBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMAccountNumbersBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMAccountNumbersWSI callSingleBL(NMAccountNumbersWSI wsi)
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

        public List<NMAccountNumbersWSI> callListBL(NMAccountNumbersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ": 
                    return SearchObject(wsi);
                default:
                    return new List<NMAccountNumbersWSI>();
            }
        }

        private NMAccountNumbersWSI SelectObject(NMAccountNumbersWSI wsi)
        {
            AccountNumbersAccesser Accesser = new AccountNumbersAccesser(Session);
            AccountNumbers obj;
            obj = Accesser.GetAllAccountnumbersByID(wsi.AccountNumber.Id, false);
            if (obj != null)
            {
                wsi.AccountNumber = obj;
                //wsi.SubAccountNumbers = obj.SubAccountNumbersList.Cast<SubAccountNumbers>().ToList();
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        private NMAccountNumbersWSI SaveObject(NMAccountNumbersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AccountNumbersAccesser Accesser = new AccountNumbersAccesser(Session);
                AccountNumbers obj = Accesser.GetAllAccountnumbersByID(wsi.AccountNumber.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);
                    wsi.AccountNumber.CreatedDate = obj.CreatedDate;
                    wsi.AccountNumber.CreatedBy = obj.CreatedBy;
                    Accesser.UpdateAccountnumbers(wsi.AccountNumber);
                }
                else
                {
                    wsi.AccountNumber.CreatedDate = DateTime.Now;
                    wsi.AccountNumber.CreatedBy = wsi.Filter.ActionBy;
                    Accesser.InsertAccountnumbers(obj);
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

        private NMAccountNumbersWSI DeleteObject(NMAccountNumbersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AccountNumbersAccesser Accesser = new AccountNumbersAccesser(Session);
                AccountNumbers obj = Accesser.GetAllAccountnumbersByID(wsi.AccountNumber.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteAccountnumbers(obj);
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

        private List<NMAccountNumbersWSI> SearchObject(NMAccountNumbersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            
            if (!string.IsNullOrEmpty(wsi.AccountNumber.ParentId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ParentId = :ParentId";
                ListCriteria.Add("ParentId", wsi.AccountNumber.ParentId);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.AccountTypeId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.AccountTypeId = :AccountTypeId";
                ListCriteria.Add("AccountTypeId", wsi.AccountNumber.AccountTypeId);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.ForPayment))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ForPayment = :ForPayment";
                ListCriteria.Add("ForPayment", wsi.AccountNumber.ForPayment);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.ForInvoice))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ForInvoice = :ForInvoice";
                ListCriteria.Add("ForInvoice", wsi.AccountNumber.ForInvoice);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.ForReceipt))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ForReceipt = :ForReceipt";
                ListCriteria.Add("ForReceipt", wsi.AccountNumber.ForReceipt);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.IsDiscontinued.ToString()))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.IsDiscontinued = :IsDiscontinued";
                ListCriteria.Add("IsDiscontinued", wsi.AccountNumber.IsDiscontinued);
            }
            if (!string.IsNullOrEmpty(wsi.AccountNumber.NoBalances.ToString()))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.NoBalances = :NoBalances";
                ListCriteria.Add("NoBalances", wsi.AccountNumber.NoBalances);
            }

            String strCmd = "SELECT O FROM AccountNumbers AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMAccountNumbersWSI> ListWSI = new List<NMAccountNumbersWSI>();
            AccountNumbersAccesser Acceser = new AccountNumbersAccesser(Session);
            IList<AccountNumbers> objs;
            objs = Acceser.GetAccountnumbersByQuery(query, false);
            foreach (AccountNumbers obj in objs)
            {
                wsi = new NMAccountNumbersWSI();
                wsi.AccountNumber = obj;
                //wsi.SubAccountNumbers = obj.SubAccountNumbersList.Cast<SubAccountNumbers>().ToList();
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }
    }
}
