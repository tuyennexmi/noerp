
// NMCategoriesBL.cs

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
    public class NMCategoriesBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();
        public NMCategoriesBL()
        {

        }

        public NMCategoriesWSI callSingleBL(NMCategoriesWSI wsi)
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

        public List<NMCategoriesWSI> callListBL(NMCategoriesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMCategoriesWSI>();
            }
        }

        public NMCategoriesWSI SelectObject(NMCategoriesWSI wsi)
        {
            CategoriesAccesser Accesser = new CategoriesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            Categories obj;
            Categories Category;
            obj = Accesser.GetAllCategoriesByID(wsi.Category.Id, false);
            if (obj != null)
            {
                wsi.Category = obj;
                wsi.Translation = TranslationAccesser.GetAllTranslationsByID(NMCommon.GetLanguageDefault(), obj.Id, true);
                if (wsi.Translation == null)
                    wsi.Translation = new Translations();
                wsi.FullName = wsi.Translation.Name;
                if (!string.IsNullOrEmpty(obj.ParentId) & obj.Id != obj.ParentId)
                {
                    string parentId = obj.ParentId;
                    Translations Trans;
                    while (!string.IsNullOrEmpty(parentId))
                    {
                        Category = Accesser.GetAllCategoriesByID(parentId, false);
                        Trans = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, Category.Id, true);
                        if (Trans == null)
                            Trans = new Translations();
                        if (Category != null)
                        {
                            parentId = Category.ParentId;
                            if (parentId == obj.ParentId)
                                break;
                            wsi.FullName = Trans.Name + " / " + wsi.FullName;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMCategoriesWSI SaveObject(NMCategoriesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                CategoriesAccesser Accesser = new CategoriesAccesser(Session);
                Categories obj = Accesser.GetAllCategoriesByID(wsi.Category.Id, true);
                if (obj != null)
                {
                    if (string.IsNullOrEmpty(wsi.Category.Image))
                    {
                        wsi.Category.Image = obj.Image;
                    }
                    wsi.Category.CreatedDate = obj.CreatedDate;
                    wsi.Category.CreatedBy = obj.CreatedBy;
                    Accesser.UpdateCategories(wsi.Category);
                }
                else
                {
                    wsi.Category.Id = AutomaticValueAccesser.AutoGenerateId("Categories");
                    wsi.Category.CreatedDate = DateTime.Now;
                    wsi.Category.CreatedBy = wsi.ActionBy;
                    Accesser.InsertCategories(wsi.Category);
                }
                tx.Commit();
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
                tx.Rollback();
            }
            return wsi;
        }

        public NMCategoriesWSI DeleteObject(NMCategoriesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            CategoriesAccesser Accesser = new CategoriesAccesser(Session);
            Categories obj = Accesser.GetAllCategoriesByID(wsi.Category.Id, true);
            if (obj != null)
            {
                Accesser.DeleteCategories(obj);
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

        public List<NMCategoriesWSI> SearchObject(NMCategoriesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Category != null)
            {
                if (!string.IsNullOrEmpty(wsi.Category.CustomerCode))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerCode = :CustomerCode";
                    ListCriteria.Add("CustomerCode", wsi.Category.CustomerCode);
                }
                else
                {
                    if (wsi.Category.ParentId != "")
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        if (wsi.Category.ParentId == null)
                        {
                            strCriteria += " O.ParentId is null";
                        }
                        else
                        {
                            strCriteria += " O.ParentId = :ParentId";
                            ListCriteria.Add("ParentId", wsi.Category.ParentId);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(wsi.Category.ObjectName))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ObjectName = :ObjectName";
                    ListCriteria.Add("ObjectName", wsi.Category.ObjectName);
                }
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (select T.Name + ';' from Translations T where T.OwnerId = O.Id and T.LanguageId = '" + wsi.LanguageId + "') LIKE :Keyword";
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Categories AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Id ASC";
            }
            String strCmd = "SELECT O FROM Categories AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMCategoriesWSI> ListWSI = new List<NMCategoriesWSI>();
            CategoriesAccesser Accesser = new CategoriesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            IList<Categories> objs;
            Categories Category;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetCategoriesByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetCategoriesByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            NMCategoriesWSI ItemWSI;
            foreach (Categories obj in objs)
            {
                ItemWSI = new NMCategoriesWSI();
                ItemWSI.Category = obj;
                ItemWSI.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.Id, true);
                if (ItemWSI.Translation == null)
                    ItemWSI.Translation = new Translations();
                ItemWSI.FullName = ItemWSI.Translation.Name;
                if (!string.IsNullOrEmpty(obj.ParentId) & obj.Id != obj.ParentId)
                {
                    string parentId = obj.ParentId;
                    Translations Trans;
                    while (!string.IsNullOrEmpty(parentId))
                    {
                        Category = Accesser.GetAllCategoriesByID(parentId, false);
                        Trans = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, Category.Id, true);
                        if (Trans == null)
                            Trans = new Translations();
                        if (Category != null)
                        {
                            parentId = Category.ParentId;
                            if (parentId == obj.ParentId)
                                break;
                            wsi.FullName = Trans.Name + " / " + wsi.FullName;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
                //NMProductsBL ProductBL = new NMProductsBL();
                //NMProductsWSI ProductWSI = new NMProductsWSI();
                //ProductWSI.Mode = "SRC_OBJ";
                //ProductWSI.Product = new Products();
                //ProductWSI.Product.CategoryId = obj.Id;
                //ProductWSI.Product.Discontinued = false;
                //ItemWSI.ProductWSIs = ProductBL.callListBL(ProductWSI);
                ListWSI.Add(ItemWSI);
            }
            if(ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;
            return ListWSI;
        }
    }
}
