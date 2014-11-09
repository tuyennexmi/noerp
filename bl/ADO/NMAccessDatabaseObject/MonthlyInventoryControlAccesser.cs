// MonthlyInventoryControlAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyê?n Quang Tuyê?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class MonthlyInventoryControlAccesser
    {
        ISession session;

        public MonthlyInventoryControlAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMonthlyinventorycontrol(MonthlyInventoryControl monthlyinventorycontrolX)
        {
            session.Merge(monthlyinventorycontrolX);
        }

        public void UpdateMonthlyinventorycontrol(MonthlyInventoryControl monthlyinventorycontrolX)
        {
            session.Update(monthlyinventorycontrolX);
        }

        public void DeleteMonthlyinventorycontrol(MonthlyInventoryControl monthlyinventorycontrolX)
        {
            session.Delete(monthlyinventorycontrolX);
        }

        public IList<MonthlyInventoryControl> GetAllMonthlyinventorycontrol(Boolean evict)
        {
            IQuery query = session.CreateQuery("select m from MonthlyInventoryControl as m");
            IList<MonthlyInventoryControl> list = query.List<MonthlyInventoryControl>();
            if (evict)
            {
                foreach (MonthlyInventoryControl s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MonthlyInventoryControl GetAllMonthlyinventorycontrolByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MonthlyInventoryControl as c where c.ProductId = :x");
            query.SetString("x", id);
            MonthlyInventoryControl s = (MonthlyInventoryControl)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }
        public MonthlyInventoryControl GetAllMonthlyinventorycontrolByStockIdAndProductId(String stockId, String productId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MonthlyInventoryControl as c where c.StockId = :x and c.ProductId = :y");
            query.SetString("x", stockId);
            query.SetString("y", productId);
            MonthlyInventoryControl s = (MonthlyInventoryControl)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }
        
        public IList<MonthlyInventoryControl> GetMonthlyinventorycontrolByQuery(IQuery query, Boolean evict)
        {
            IList<MonthlyInventoryControl> list = query.List<MonthlyInventoryControl>();
            if (evict)
            {
                foreach (MonthlyInventoryControl s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
