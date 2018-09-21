<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorisationPermissionCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AuthorisationPermissionCtrl" %>

<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="PermissionTitle" runat="server" Width="100%"></asp:Label>
</div>
<div style=" overflow: auto">
    <div style="display: table; width: 100%;" class="PermissionTable">
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ApplicationPermissionsTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDropDownList ID="ApplicationPermissionsData" runat="server" Width="250px" OnClientItemSelected="ApplicationPermissionsSelected"></telerik:RadDropDownList>
            </div>
        </div>
    </div>
    <div style="display: table;" class="PermissionTable">
        <div style="display: table-row;">
            <div style="display: table-cell; width: 200px;" class="titleLabelCell permissionCell">
                <asp:Label ID="ReadTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell permissionCell">
                <telerik:RadCheckBox ID="ReadDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px; margin-left: 0px;" class="dataLabelCell permissionCell">
                <telerik:RadCheckBox ID="ReadDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell permissionCell">
                <asp:Label ID="InsertTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell permissionCell">
                <telerik:RadCheckBox ID="InsertDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell permissionCell">
                <telerik:RadCheckBox ID="InsertDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="EditTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="EditDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="EditDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="DeleteTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="DeleteDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="DeleteDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="AdminTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="AdminDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="AdminDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;" >
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ExecuteTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="ExecuteDataCB" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
            <div style="display: table-cell; width: 40px;" class="dataLabelCell">
                <telerik:RadCheckBox ID="ExecuteDataCBInherited" runat="server" Enabled="false" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
    </div>
    <div style="padding-left: 28px; padding-top: 15px">
        <telerik:RadCheckBox ID="TakeInherited" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
        <asp:Label ID="TakeInheritedTitle" runat="server"></asp:Label>
    </div>
</div>


