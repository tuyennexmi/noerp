// NMDepartmentsBL.cs

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
    public class NMDepartmentsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMDepartmentsBL()
        {
            
        }

        public NMDepartmentsWSI callSingleBL(NMDepartmentsWSI wsi)
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

        public List<NMDepartmentsWSI> callListBL(NMDepartmentsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMDepartmentsWSI>();
            }
        }

        public NMDepartmentsWSI SelectObject(NMDepartmentsWSI wsi)
        {
            DepartmentsAccesser Acceser = new DepartmentsAccesser(Session);
            Departments obj;
            obj = Acceser.GetAllDepartmentsByID(wsi.Department.Id, true);
            if (obj != null)
            {
                wsi.Department = obj;
                
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMDepartmentsWSI SaveObject(NMDepartmentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                DepartmentsAccesser Accesser = new DepartmentsAccesser(Session);
                Departments obj = Accesser.GetAllDepartmentsByID(wsi.Department.Id, false);
                if (obj != null)
                {
                    Session.Evict(obj);

                    wsi.Department.CreatedDate = obj.CreatedDate;
                    wsi.Department.CreatedBy = obj.CreatedBy;
                    wsi.Department.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Department.ModifiedDate = DateTime.Now;

                    Accesser.UpdateDepartments(wsi.Department);
                }
                else
                {
                    wsi.Department.Id = AutomaticValueAccesser.AutoGenerateId("Departments");
                    wsi.Department.CreatedDate = DateTime.Now;
                    wsi.Department.CreatedBy = wsi.Filter.ActionBy;
                    wsi.Department.ModifiedBy = wsi.Filter.ActionBy;
                    wsi.Department.ModifiedDate = DateTime.Now;

                    Accesser.InsertDepartments(wsi.Department);
                }

                tx.Commit();

                // tao phan quyen mac dinh neu la Department moi
                if (obj == null)
                {
                    NMModulesBL ModuleBL = new NMModulesBL();
                    NMModulesWSI ModuleWSI = new NMModulesWSI();
                    ModuleWSI.Mode = "SRC_OBJ";
                    List<NMModulesWSI> ModuleWSIs = ModuleBL.callListBL(ModuleWSI);

                    NMFunctionsBL FunctionBL = new NMFunctionsBL();
                    NMFunctionsWSI FunctionWSI = new NMFunctionsWSI();
                    FunctionWSI.Mode = "SRC_OBJ";

                    NMPermissionsBL PermissionBL = new NMPermissionsBL();
                    NMPermissionsWSI PermissionWSI = new NMPermissionsWSI();
                    PermissionWSI.Mode = "SAV_OBJ";

                    foreach (NMModulesWSI ModulesWSI in ModuleWSIs)
                    {
                        // Get all function of module
                        FunctionWSI.ModuleId = ModulesWSI.Module.Id;
                        FunctionWSI.Active = true.ToString();
                        List<NMFunctionsWSI> AllFunctionsWSIs = FunctionBL.callListBL(FunctionWSI);

                        foreach (NMFunctionsWSI function in AllFunctionsWSIs)
                        {
                            PermissionWSI.UserGroupId = wsi.Department.Id;
                            PermissionWSI.UserId = null;
                            PermissionWSI.FunctionId = function.Id;
                            PermissionWSI.PSelect = "N";
                            PermissionWSI.PInsert = "N";
                            PermissionWSI.PUpdate = "N";
                            PermissionWSI.Reconcile = "N";
                            PermissionWSI.PDelete = "N";
                            PermissionWSI.ViewPrice = "N";
                            PermissionWSI.PPrint = "N";
                            PermissionWSI.Export = "N";
                            PermissionWSI.Duplicate = "N";
                            PermissionWSI.Approval = "N";
                            PermissionWSI.ViewAll = "N";
                            PermissionWSI.Calculate = "N";
                            PermissionWSI.History = "N";
                            PermissionWSI.CreatedBy = wsi.Filter.ActionBy;
                            PermissionWSI = PermissionBL.callSingleBL(PermissionWSI);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục. " + ex.Message ;
            }
            return wsi;
        }

        public NMDepartmentsWSI DeleteObject(NMDepartmentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                DepartmentsAccesser Accesser = new DepartmentsAccesser(Session);
                Departments obj = Accesser.GetAllDepartmentsByID(wsi.Department.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteDepartments(obj);
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

        public List<NMDepartmentsWSI> SearchObject(NMDepartmentsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.Department.Name))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Name LIKE :Name";
                ListCriteria.Add("Name", "%" + wsi.Department.Name + "%");
            }
            String strCmd = "SELECT O FROM Departments AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMDepartmentsWSI> ListWSI = new List<NMDepartmentsWSI>();
            DepartmentsAccesser Acceser = new DepartmentsAccesser(Session);
            IList<Departments> objs;
            objs = Acceser.GetDepartmentsByQuery(query, false);
            foreach (Departments obj in objs)
            {
                wsi = new NMDepartmentsWSI();
                wsi.Department = obj;
                
                ListWSI.Add(wsi);
            }
            
            return ListWSI;
        }       
    }
}
