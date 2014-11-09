// AutomaticValuesAccesser.cs

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

namespace NEXMI
{
    public class AutomaticValuesAccesser
    {
        ISession session;

        public AutomaticValuesAccesser(ISession session)
        {
            this.session = session;
        }
        public void InsertAutomaticValues(AutomaticValues AutomaticValuesX)
        {
            session.Merge(AutomaticValuesX);
        }

        public void UpdateAutomaticValues(AutomaticValues AutomaticValuesX)
        {
            session.Update(AutomaticValuesX);
        }

        public void DeleteAutomaticValues(AutomaticValues AutomaticValuesX)
        {
            session.Delete(AutomaticValuesX);
        }

        public IList<AutomaticValues> GetAllAutomaticValues(Boolean evict)
        {
            IQuery query = session.CreateQuery("Select u from AutomaticValues as u");
            IList<AutomaticValues> list = query.List<AutomaticValues>();
            if (evict)
            {
                foreach (AutomaticValues s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public AutomaticValues GetAutomaticValuesByObjectName(String ObjectName, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from AutomaticValues as c where c.ObjectName = :x");
            query.SetString("x", ObjectName);
            AutomaticValues s = (AutomaticValues)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<AutomaticValues> GetAutomaticValuesByQuery(IQuery query, Boolean evict)
        {
            IList<AutomaticValues> list = query.List<AutomaticValues>();
            if (evict)
            {
                foreach (AutomaticValues s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public String AutoGenerateId(String TableName)
        {
            String newId = "";
            AutomaticValues obj = GetAutomaticValuesByObjectName(TableName, true);
            if (obj != null)
            {
                string prefix = obj.PrefixOfDefaultValueForId;
                string sign = (obj.Sign == null) ? "-" : obj.Sign;
                string dateFormat = (obj.DateFormat == null) ? "dd/MM/yyyy" : obj.DateFormat;
                int length = obj.LengthOfDefaultValueForId;
                string lastValue = obj.LastValueOfColumnId;
                string suffix = "";
                string day = "", month = "", year = "";
                string nextNumber = "";

                if (obj.ResetChar != null)
                {
                    if (obj.ResetDate == null)
                    {
                        switch (obj.ResetChar)
                        {
                            case "D":
                            case "M":
                                obj.ResetDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day - 1);
                                break;
                            case "Y":
                                obj.ResetDate = new DateTime(DateTime.Today.Year, 12, 31);
                                break;
                        }
                    }
                    DateTime resetDate = obj.ResetDate.Value;
                    switch (obj.ResetChar)
                    {
                        case "D":
                            if (resetDate < DateTime.Today)
                            {
                                nextNumber = "1";
                                obj.ResetDate = DateTime.Today;
                            }
                            day = obj.ResetDate.Value.ToString("dd");
                            month = obj.ResetDate.Value.ToString("MM");
                            if ((obj.FullYear == null) || (obj.FullYear == false))
                                year = obj.ResetDate.Value.ToString("yy");
                            else
                                year = obj.ResetDate.Value.ToString("yyyy");
                            break;
                        case "M":
                            if (resetDate.Month != DateTime.Today.Month)
                            {
                                nextNumber = "1";
                                obj.ResetDate = DateTime.Today;
                            }
                            month = obj.ResetDate.Value.ToString("MM");
                            if ((obj.FullYear == null) || (obj.FullYear == false))
                                year = obj.ResetDate.Value.ToString("yy");
                            else
                                year = obj.ResetDate.Value.ToString("yyyy");
                            break;
                        case "Y":
                            if (resetDate < DateTime.Today)
                            {
                                nextNumber = "1";
                                obj.ResetDate = new DateTime(resetDate.Year + 1, resetDate.Month, resetDate.Day);
                            }
                            if ((obj.FullYear == null) || (obj.FullYear == false))
                                year = obj.ResetDate.Value.ToString("yy");
                            else
                                year = obj.ResetDate.Value.ToString("yyyy");
                            break;
                    }
                    
                    switch (dateFormat)
                    {
                        case "dd/MM/yyyy":
                            suffix = day + month + year;
                            break;
                        case "MM/dd/yyyy":
                            suffix = month + day + year;
                            break;
                        case "yyyy/MM/dd":
                            suffix = year + month + day;
                            break;
                    }
                }
                if (nextNumber == "")
                {
                    try
                    {
                        nextNumber = (Convert.ToInt32(lastValue.Substring(lastValue.Length - length)) + 1).ToString();
                    }
                    catch
                    {
                        nextNumber = "1";
                    }
                }
                newId = "00000000000000000" + nextNumber;

                if ((obj.Invert == null) || (obj.Invert.Value == false))
                {
                    newId = prefix + suffix + sign + newId.Substring(newId.Length - length);
                }
                else
                {
                    newId = newId.Substring(newId.Length - length) + sign + suffix + sign + prefix;
                }
                obj.LastValueOfColumnId = newId;
                UpdateAutomaticValues(obj);
            }
            return newId;
        }
    }
}
