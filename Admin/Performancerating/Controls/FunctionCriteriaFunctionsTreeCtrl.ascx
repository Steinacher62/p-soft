<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionCriteriaFunctionsTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.FunctionCriteriaFunctionsTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveFunctionImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="FunctionTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="FunctionTree_NodeDataBound" OnClientNodeClicked="FunctionTreeNodeClicked">
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getFunctionTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
