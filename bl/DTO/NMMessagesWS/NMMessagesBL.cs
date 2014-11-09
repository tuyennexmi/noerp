// NMMessagesBL.cs

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
    public class NMMessagesBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMMessagesBL()
        {

        }

        public NMMessagesWSI callSingleBL(NMMessagesWSI wsi)
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

        public List<NMMessagesWSI> callListBL(NMMessagesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMMessagesWSI>();
            }
        }

        public NMMessagesWSI SelectObject(NMMessagesWSI wsi)
        {
            MessagesAccesser Acceser = new MessagesAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            Messages obj;
            obj = Acceser.GetAllMessagesByID(wsi.Message.MessageId, true);
            if (obj != null)
            {
                wsi.Message = obj;
                wsi.WsiError = "";
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMMessagesWSI SaveObject(NMMessagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                MessagesAccesser Accesser = new MessagesAccesser(Session);
                Messages obj = Accesser.GetAllMessagesByID(wsi.Message.MessageId, true);
                if (obj != null)
                {
                    Accesser.UpdateMessage(wsi.Message);
                }
                else
                {
                    wsi.Message.MessageId = AutomaticValueAccesser.AutoGenerateId("Messages");
                    wsi.Message.CreatedDate = DateTime.Now;
                    wsi.Message.CreatedBy = wsi.ActionBy;
                    Accesser.InsertMessage(wsi.Message);
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

        public NMMessagesWSI DeleteObject(NMMessagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                MessagesAccesser Accesser = new MessagesAccesser(Session);
                Messages obj = Accesser.GetAllMessagesByID(wsi.Message.MessageId, true);
                if (obj != null)
                {
                    Accesser.DeleteMessage(obj);
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

        public List<NMMessagesWSI> SearchObject(NMMessagesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Message != null)
            {
                if (!string.IsNullOrEmpty(wsi.Message.Owner))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Owner = :Owner";
                    ListCriteria.Add("Owner", wsi.Message.Owner);
                }
                if (!string.IsNullOrEmpty(wsi.Message.StatusId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StatusId = :StatusId";
                    ListCriteria.Add("StatusId", wsi.Message.StatusId);
                }
                if (!string.IsNullOrEmpty(wsi.Message.TypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TypeId = :TypeId";
                    ListCriteria.Add("TypeId", wsi.Message.TypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Message.SendTo))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.SendTo like :SendTo";
                    ListCriteria.Add("SendTo", "%" + wsi.Message.SendTo + "%");
                }
            }
            if (!string.IsNullOrEmpty(wsi.IsRead))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.IsRead = :IsRead";
                ListCriteria.Add("IsRead", bool.Parse(wsi.IsRead));
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :CreatedBy";
                ListCriteria.Add("CreatedBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Description like :Keyword or O.CreatedBy in (select C.CustomerId from Customers C where C.CompanyNameInVietnamese like :Keyword or C.CustomerId like :Keyword or C.Code like :Keyword or C.EmailAddress like :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.MessageId) FROM Messages AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.CreatedDate DESC";
            }
            String strCmd = "SELECT O FROM Messages AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            List<NMMessagesWSI> ListWSI = new List<NMMessagesWSI>();
            MessagesAccesser Accesser = new MessagesAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            IList<Messages> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetMessagesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetMessagesByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Messages obj in objs)
            {
                wsi = new NMMessagesWSI();
                wsi.Message = obj;
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }

        public static void SaveMessage(ISession session, string owner, string name, string content, string createdBy, string categoryId, string groupId, string typeId)
        {
            if (NMCommon.GetSetting("LOG_INFO_CHANGE"))
            {
                AutomaticValuesAccesser AutoIDAccesser = new AutomaticValuesAccesser(session);
                MessagesAccesser Accesser = new MessagesAccesser(session);
                Messages Message = new Messages();
                Message.MessageId = AutoIDAccesser.AutoGenerateId("Messages");
                Message.MessageName = name;
                Message.Owner = owner;
                Message.Description = content;
                Message.CreatedDate = DateTime.Now;
                if (!String.IsNullOrEmpty(createdBy))
                    Message.CreatedBy = createdBy;
                if (!String.IsNullOrEmpty(categoryId))
                    Message.CategoryId = categoryId;
                if (!String.IsNullOrEmpty(groupId))
                    Message.GroupId = groupId;
                if (!String.IsNullOrEmpty(typeId))
                    Message.TypeId = typeId;
                Accesser.InsertMessage(Message);
            }
        }
    }
}
