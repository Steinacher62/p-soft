<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessorListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AccessorListCtrl" %>

<div style="font-weight: bold; padding-left: 5px">
    <div style="display: table-row;">
        <div style="display: table-cell; padding-right:5px" >
            <asp:Label ID="EditableTitle" runat="server"></asp:Label>
        </div>
        <div style="display: table-cell;">
            <asp:Label ID="AccessorTitle" runat="server"></asp:Label>
        </div>
    </div>
</div>
<div>
    <telerik:RadListBox ID="AccessorListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" CheckBoxes="true" OnClientItemChecking="AccessorListBoxChecking"  OnClientSelectedIndexChanged="AccessorListBoxIndexChanged" OnClientContextMenu="AccessorListBoxShowContextMenu" Width="100%" Height="100px">
        

    </telerik:RadListBox>
</div>

<telerik:RadContextMenu ID="AccessorListContextMenu" runat="server" OnClientItemClicked="AccessorListBoxContextMenuItemClicked">
   <Items>
       <telerik:RadMenuItem></telerik:RadMenuItem>
   </Items>
</telerik:RadContextMenu> 