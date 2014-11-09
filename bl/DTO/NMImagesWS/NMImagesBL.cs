// NMImagesBL.cs

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
    public class NMImagesBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();

        public NMImagesBL()
        {

        }

        public NMImagesWSI callSingleBL(NMImagesWSI wsi)
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

        public List<NMImagesWSI> callListBL(NMImagesWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMImagesWSI>();
            }
        }

        public NMImagesWSI SelectObject(NMImagesWSI wsi)
        {
            ImagesAccesser Acceser = new ImagesAccesser(Session);
            Images obj;
            obj = Acceser.GetAllImagesByID(wsi.Id, true);
            if (obj != null)
            {
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.Location = obj.Location;
                wsi.Description = obj.Description;
                wsi.Owner = obj.Owner;
                wsi.IsDefault = obj.IsDefault.ToString();
                wsi.TypeId = obj.TypeId;
                wsi.GroupId = obj.GroupId;
                wsi.CategoryId = obj.CategoryId;
                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMImagesWSI SaveObject(NMImagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ImagesAccesser Accesser = new ImagesAccesser(Session);
                Images obj = Accesser.GetAllImagesByName(wsi.Name, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.Location = wsi.Location;
                    obj.Description = wsi.Description;
                    obj.Owner = wsi.Owner;
                    if (!string.IsNullOrEmpty(wsi.IsDefault))
                    {
                        obj.IsDefault = Boolean.Parse(wsi.IsDefault);
                    }
                    if (!string.IsNullOrEmpty(wsi.IsPublish))
                    {
                        obj.IsPublish = Boolean.Parse(wsi.IsPublish);
                    }
                    obj.TypeId = wsi.TypeId;
                    obj.GroupId = wsi.GroupId;
                    obj.CategoryId = wsi.CategoryId;
                    Accesser.UpdateImages(obj);
                }
                else
                {
                    obj = new Images();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId("Images");
                    obj.Name = wsi.Name;
                    obj.Location = wsi.Location;
                    obj.Description = wsi.Description;
                    obj.Owner = wsi.Owner;
                    if (!string.IsNullOrEmpty(wsi.IsDefault))
                    {
                        obj.IsDefault = Boolean.Parse(wsi.IsDefault);
                    }
                    if (!string.IsNullOrEmpty(wsi.IsPublish))
                    {
                        obj.IsPublish = Boolean.Parse(wsi.IsPublish);
                    }
                    obj.TypeId = wsi.TypeId;
                    obj.GroupId = wsi.GroupId;
                    obj.CategoryId = wsi.CategoryId;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = wsi.CreatedBy;
                    Accesser.InsertImages(obj);
                }
                tx.Commit();
                wsi.Id = obj.Id;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMImagesWSI DeleteObject(NMImagesWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ImagesAccesser Accesser = new ImagesAccesser(Session);
                Images obj = Accesser.GetAllImagesByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteImages(obj);
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

        public List<NMImagesWSI> SearchObject(NMImagesWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.Name))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Name LIKE :Name";
                ListCriteria.Add("Name", "%" + wsi.Name + "%");
            }
            if (!string.IsNullOrEmpty(wsi.Owner))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Owner = :Owner";
                ListCriteria.Add("Owner", wsi.Owner);
            }

            if (!string.IsNullOrEmpty(wsi.TypeId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.TypeId = :TypeId";
                ListCriteria.Add("TypeId", wsi.TypeId);
            }

            String strCmd = "SELECT O FROM Images AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMImagesWSI> ListWSI = new List<NMImagesWSI>();
            ImagesAccesser Acceser = new ImagesAccesser(Session);
            Customers createBy;
            CustomersAccesser customersAccesser = new CustomersAccesser(Session);
            IList<Images> objs;
            objs = Acceser.GetImagesByQuery(query, false);
            foreach (Images obj in objs)
            {
                wsi = new NMImagesWSI();
                wsi.Id = obj.Id.ToString();
                wsi.Name = obj.Name;
                wsi.Location = obj.Location;
                wsi.Description = obj.Description;
                wsi.Owner = obj.Owner;
                wsi.IsDefault = obj.IsDefault.ToString();
                wsi.TypeId = obj.TypeId;
                wsi.GroupId = obj.GroupId;
                wsi.CategoryId = obj.CategoryId;
                wsi.CreatedDate = obj.CreatedDate.ToString("dd-MM-yyyy");
                
                createBy = customersAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                if (createBy != null)
                    wsi.CreatedBy = "[" + createBy.CustomerId + "] " + createBy.CompanyNameInVietnamese;
                else
                    wsi.CreatedBy = obj.CreatedBy;

                ListWSI.Add(wsi);
            }
            return ListWSI;
        }       
    }
}
