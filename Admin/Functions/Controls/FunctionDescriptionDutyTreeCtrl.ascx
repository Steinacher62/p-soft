<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDescriptionDutyTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionDescriptionDutyTreeCtrl" %>
<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="DutyTree" CssClass="Tree" runat="server" Height="200px" EnableDragAndDrop="true" OnClientNodeDropping="DutyTreeNodeDropping" OnClientNodeClicked="DutyTreeNodeClicked">
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>