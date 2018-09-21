<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionCriteriaTypeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.FunctionCriteriaTypeCtrl" %>
<div style="height: 100%; overflow: auto;">
    <div style="display: table; vertical-align:middle; height:100%; width:500px" class="FunctionCriteriaRypTable">

        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <telerik:RadCheckBox ID="FunctionCriteriaTyp" runat="server" AutoPostBack="false" OnClientCheckedChanged="FunctionCriteriaTypChanged"></telerik:RadCheckBox>
            </div>
        </div>
        
    </div>
</div>
