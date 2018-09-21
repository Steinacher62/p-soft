<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KnowledgeViewSimpleCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgeViewSimple" %>
<telerik:RadWindow ID="RadWindow2" runat="server" VisibleOnPageLoad="false" Modal="false" VisibleStatusbar="false" Title="Wissenselement" Behaviors="Move,Resize" Width="860px" Height="600px">
    <ContentTemplate>
        <div style="display: table; height: 100%; width: 100%">
            <div style="display: table-row;">
                <div style="display: table-cell; padding: 10px; height:30px; font-weight:bold;" id="TextContentTitle">
                </div>
            </div>
            <div ">
                <div style=" padding: 10px; overflow:auto; max-height: 380px;" id="TextContent">
                </div>
            </div>
            <div style="display: table-row;">
                <div style="display: table-cell; padding: 10px; height:30px;">
                    <telerik:RadButton ID="CloseKnowledgeView"  runat="server" OnClientClicked="CloseKnowledgeViewWindowClicked" Text="Schliessen"  AutoPostBack="false"></telerik:RadButton>
                </div>
            </div>
        </div>
    </ContentTemplate>
</telerik:RadWindow>