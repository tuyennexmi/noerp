// CustomersController.cs

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
using NEXMI;

namespace nexmiStore.Controllers
{
    public class CustomersController : Controller
    {
        //
        // GET: /Customers/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerMap(string longitude, string latitude)
        {
            ViewData["Longitude"] = longitude;
            ViewData["Latitude"] = latitude;
            return PartialView();
        }

        public ActionResult CustomerTag(List<NEXMI.Customers> cusList, String groupId)
        {
            ViewData["cusList"] = cusList;
            ViewData["GroupId"] = groupId;
            return PartialView();
        }

        public ActionResult ManufacturersGrid()
        {
            ViewData["GroupId"] = NMConstant.CustomerGroups.Manufacturer;
            ViewData["ViewType"] = "list";
            return PartialView();
        }

        public ActionResult SuppliersGrid()
        {
            ViewData["GroupId"] = NMConstant.CustomerGroups.Supplier;
            ViewData["ViewType"] = "list";
            return PartialView();
        }

        #region ContactPersons
        public ActionResult ContactPersonForm(String id, string windowId)
        {
            List<Customers> ContactPersons = (List<Customers>)Session["ContactPersons"];
            if (ContactPersons != null)
            {
                for (int i = 0; i < ContactPersons.Count; i++)
                {
                    if (ContactPersons[i].CustomerId == id)
                    {
                        ViewData["id"] = ContactPersons[i].CustomerId;
                        ViewData["name"] = ContactPersons[i].CompanyNameInVietnamese;
                        ViewData["position"] = ContactPersons[i].JobPosition;
                        ViewData["phoneNumber"] = ContactPersons[i].Telephone;
                        ViewData["cellphone"] = ContactPersons[i].Cellphone;
                        ViewData["email"] = ContactPersons[i].EmailAddress;
                    }
                }
            }
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveContactPerson(string id, string name, string position, string phoneNumber, string cellphone, string email)
        {
            string error = "";
            try
            {
                List<Customers> ContactPersons = (List<Customers>)Session["ContactPersons"];
                if (ContactPersons == null)
                {
                    ContactPersons = new List<Customers>();
                }
                if (id == "")
                {
                    try
                    {
                        int num;
                        id = (int.Parse((from c in ContactPersons where int.TryParse(c.CustomerId, out num) select c.CustomerId).Max()) + 1).ToString();
                    }
                    catch
                    {
                        id = "1";
                    }
                }
                int index = -1;
                for (int i = 0; i < ContactPersons.Count; i++)
                {
                    if (ContactPersons[i].CustomerId == id)
                    {
                        index = i;
                        break;
                    }
                }
                Customers ContactPerson = new Customers();
                ContactPerson.CustomerId = id;
                ContactPerson.CompanyNameInVietnamese = name;
                ContactPerson.JobPosition = position;
                ContactPerson.Telephone = phoneNumber;
                ContactPerson.Cellphone = cellphone;
                ContactPerson.EmailAddress = email;
                ContactPerson.IsContact = true;
                if (index == -1)
                {
                    ContactPersons.Add(ContactPerson);
                }
                else
                {
                    ContactPersons[index] = ContactPerson;
                }
                Session["ContactPersons"] = ContactPersons;
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return Json(new { result = id, error = error });
        }

        public JsonResult DeleteContactPerson(String id)
        {
            string result = "";
            try
            {
                List<Customers> ContactPersons = (List<Customers>)Session["ContactPersons"];
                if (ContactPersons == null)
                {
                    ContactPersons = new List<Customers>();
                }
                for (int i = 0; i < ContactPersons.Count; i++)
                {
                    if (ContactPersons[i].CustomerId == id)
                    {
                        ContactPersons.RemoveAt(i);
                        break;
                    }
                }
                Session["ContactPersons"] = ContactPersons;
            }
            catch (Exception ex)
            {
                result = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return Json(result);
        }
        #endregion
    }
}
