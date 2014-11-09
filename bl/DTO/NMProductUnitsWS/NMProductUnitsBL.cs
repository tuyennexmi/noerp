
// NMProductUnitsBL.cs

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
    public class NMProductUnitsBL
    {
         private readonly ISession Session = SessionFactory.GetNewSession();
         public NMProductUnitsBL()
         {

         }

         public NMProductUnitsWSI callSingleBL(NMProductUnitsWSI wsi)
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

         public List<NMProductUnitsWSI> callListBL(NMProductUnitsWSI wsi)
         {
             switch (wsi.Mode)
             {
                 case "SRC_OBJ":
                     return SearchObject(wsi);
                 default:
                     return new List<NMProductUnitsWSI>();
             }
         }

        public NMProductUnitsWSI SelectObject(NMProductUnitsWSI wsi)
        {
            ProductUnitsAccesser Accesser = new ProductUnitsAccesser(Session);
            ProductUnits obj;
            obj = Accesser.GetAllProductUnitsByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.ProductCategoryId = obj.ProductCategoryId;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMProductUnitsWSI SaveObject(NMProductUnitsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProductUnitsAccesser Accesser = new ProductUnitsAccesser(Session);
                ProductUnits obj = Accesser.GetAllProductUnitsByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    if (!string.IsNullOrEmpty(wsi.ProductCategoryId))
                    {
                        obj.ProductCategoryId = wsi.ProductCategoryId;
                    }
                    Accesser.UpdateProductUnits(obj);
                }
                else
                {
                    obj = new ProductUnits();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId("ProductUnits");
                    obj.Name = wsi.Name;
                    if (!string.IsNullOrEmpty(wsi.ProductCategoryId))
                    {
                        obj.ProductCategoryId = wsi.ProductCategoryId;
                    }
                    Accesser.InsertProductUnits(obj);
                    //AutomaticValueAccesser.UpdateLastId("ProductUnits", obj.Id);
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

        public NMProductUnitsWSI DeleteObject(NMProductUnitsWSI wsi)
        {

            ITransaction tx = Session.BeginTransaction();
            ProductUnitsAccesser Accesser = new ProductUnitsAccesser(Session);
            ProductUnits obj = Accesser.GetAllProductUnitsByID(wsi.Id, true);
            if (obj != null)
            {
                Accesser.DeleteProductUnits(obj);
                try
                {
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
                }
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public List<NMProductUnitsWSI> SearchObject(NMProductUnitsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.ProductCategoryId != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ProductCategoryId = :ProductCategoryId";
                ListCriteria.Add("ProductCategoryId", wsi.ProductCategoryId);
            }
            if (wsi.Name != "")
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Name LIKE :Name";
                ListCriteria.Add("Name", "%" + wsi.Name + "%");
            }
            String strCmd = "SELECT O FROM ProductUnits AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMProductUnitsWSI> ListWSI = new List<NMProductUnitsWSI>();
            ProductUnitsAccesser Accesser = new ProductUnitsAccesser(Session);
            IList<ProductUnits> objs;
            objs = Accesser.GetProductUnitsByQuery(query, false);
            foreach (ProductUnits obj in objs)
            {
                wsi = new NMProductUnitsWSI();
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.ProductCategoryId = obj.ProductCategoryId;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
