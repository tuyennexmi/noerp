<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%--<ul>
    <%
        String moduleId = Request.Params["moduleId"];
        String functionId = Request.Params["functionId"];
        if (moduleId != null)
        {
            NEXMI.NMModulesBL ModuleBL = new NEXMI.NMModulesBL();
            NEXMI.NMModulesWSI ModuleWSI = new NEXMI.NMModulesWSI();
            ModuleWSI.Mode = "SEL_OBJ";
            ModuleWSI.Id = moduleId;
            ModuleWSI = ModuleBL.callSingleBL(ModuleWSI);
    %>
    <li class="SecondLast"><%=ModuleWSI.Name%></li>
    <%
        }
        if (functionId != null)
        {
            NEXMI.NMFunctionsBL FunctionBL = new NEXMI.NMFunctionsBL();
            NEXMI.NMFunctionsWSI FunctionWSI = new NEXMI.NMFunctionsWSI();
            FunctionWSI.Mode = "SEL_OBJ";
            FunctionWSI.Id = functionId;
            FunctionWSI = FunctionBL.callSingleBL(FunctionWSI);
    %>
    <li class="Last"><%=FunctionWSI.Name%></li>
    <%
        }
    %>
</ul>--%>