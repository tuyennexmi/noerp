// ManufactureController.cs

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
using System.Web;
using System.Web.Mvc;

namespace NEXMI.Controllers
{
    public class ManufactureController : Controller
    {
        //
        // GET: /Manufacture/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManufacturingOrders()
        {
            
            return PartialView();
        }

        public ActionResult ManufacturingOrderList(string pageNum, string categoryId, string keyword)
        {
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }

            NMManufactureOrdersBL BL = new NMManufactureOrdersBL();
            NMManufactureOrdersWSI WSI = new NMManufactureOrdersWSI();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Project, "ViewAll") == false)
            {
                WSI.Filter.ActionBy = User.Identity.Name;
            }
            WSI.Filter.PageNum = page - 1;
            WSI.Filter.PageSize = NMCommon.PageSize();
            WSI.Filter.Keyword = NMCryptography.base64Decode(keyword);
            List<NEXMI.NMManufactureOrdersWSI> WSIs = BL.callListBL(WSI);

            ViewData["WSIs"] = WSIs;
            ViewData["Page"] = page;
            if (WSIs.Count > 0)
                ViewData["TotalRows"] = WSIs[0].Filter.TotalRows;
            else
                ViewData["TotalRows"] = "";
            ViewData["ViewType"] = "kanban";
            ViewData["Keyword"] = keyword;

            return PartialView();
        }

        public ActionResult ManufactureOrder(string id, string mode)
        {
            NMManufactureOrdersBL BL = new NMManufactureOrdersBL();
            NMManufactureOrdersWSI WSI = new NMManufactureOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.ManufactureOrder.Id = id;
            ViewData["MO"] = BL.callSingleBL(WSI);

            if (string.IsNullOrEmpty(id))
            {
                WSI.ManufactureOrder.StartDate = DateTime.Today;
                WSI.ManufactureOrder.EndDate = DateTime.Today.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) - DateTime.Today.Day);
                ViewData["Product"] = "";
                ViewData["Amount"] = 0;
                WSI.ManufactureOrder.Status = NMConstant.MOStatuses.Draft;
                Session["BoMs"] = new List<MOMaterialDetails>();
            }
            else
            {
                Session["BoMs"] = WSI.ManufactureOrder.MaterialDetails.ToList();
                //string product = WSI.ManufactureOrder.MaterialDetails.ToList()[0].ProductId;
                //ViewData["Product"] = product;                
            }

            if (mode == "detail")
            {
                //lấy danh sách khách hàng
                NMCustomersBL CBL = new NMCustomersBL();
                NMCustomersWSI CWSI = new NMCustomersWSI();
                CWSI.Mode = "SRC_OBJ";

                ViewData["Partners"] = CBL.callListBL(CWSI);
                return PartialView("ManufactureOrderDetail");
            }

            return PartialView();
        }

        public JsonResult SaveManufactureOrder(string id, string product, string start, string description,string quantity,
                                                string reference, string managedBy, string status)
        {
            NMManufactureOrdersBL BL = new NMManufactureOrdersBL();
            NMManufactureOrdersWSI WSI = new NMManufactureOrdersWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.ManufactureOrder.Id = id;
            WSI.ManufactureOrder.ProductId = product;
            WSI.ManufactureOrder.StartDate = DateTime.Parse(start);
            WSI.ManufactureOrder.EndDate = DateTime.Today;
            WSI.ManufactureOrder.Descriptions = description;
            WSI.ManufactureOrder.Quantity = double.Parse(quantity);
            WSI.ManufactureOrder.SaleReference = reference;
            WSI.ManufactureOrder.ManagedBy = managedBy;
            WSI.ManufactureOrder.Status = status;
            WSI.ManufactureOrder.MaterialDetails = (List<MOMaterialDetails>)Session["BoMs"];

            WSI.Filter.ActionBy = User.Identity.Name;

            WSI = BL.callSingleBL(WSI);
            
            return Json(new { id = WSI.ManufactureOrder.Id, error = WSI.WsiError });
        }

        public ActionResult MaterialCalculate(string productId, string quantity)
        {   
            if(!string.IsNullOrEmpty(productId))
            {
                NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
                NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
                WSI.Mode = "SEL_OBJ";

                WSI.Product = new Products();
                WSI.Product.ProductId = productId;
                WSI = BL.callSingleBL(WSI);

                List<MOMaterialDetails> moList = new List<MOMaterialDetails>();// (List<MOMaterialDetails>)Session["BoMs"];
                MOMaterialDetails mo = new MOMaterialDetails();
                foreach (ProductBOMs item in WSI.BoMs)
                {
                    mo.ProductId = item.ProductId;
                    mo.Quantity = item.Quantity * double.Parse(quantity);
                    moList.Add(mo);
                }
                Session["BoMs"] = moList;
            }
            //ViewData["BoMs"] = WSI.BoMs;
            //ViewData["Quantity"] = quantity;

            return PartialView();
        }

        public JsonResult DeleteMO(string id)
        {
            NMManufactureOrdersBL BL = new NMManufactureOrdersBL();
            NMManufactureOrdersWSI WSI = new NMManufactureOrdersWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.ManufactureOrder.Id = id;
            WSI = BL.callSingleBL(WSI);

            return Json("");
        }
    }
}
