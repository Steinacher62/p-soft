<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionRatingRatingDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionRatingRatingDetailCtrl" %>
<telerik:RadGrid ID="RatingDetail" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" ClientSettings-Scrolling-ScrollHeight="150px" ItemStyle-Height="10" AlternatingItemStyle-Height="10" ClientSettings-ClientEvents-OnRowContextMenu="RowContextMenu">
    <MasterTableView>
        <Columns>
            <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
            <telerik:GridBoundColumn DataField="BEZEICHNUNG" HeaderText="Ausgewählte Anforderungen" DataType="System.String">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="STUFE_PUNKTEZAHL" HeaderText="Punkte" DataType="System.Single">
                <HeaderStyle Width="100px" />
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <ClientSettings>
        <Selecting AllowRowSelect="true" />
        <ClientEvents OnCommand="function(){}" />
    </ClientSettings>
</telerik:RadGrid>

<telerik:RadContextMenu ID="RadMenu1" runat="server" EnableRoundedCorners="true" EnableShadows="true" OnClientItemClicked ="RadMenu1Clicked">
    <Items>
        <telerik:RadMenuItem Value="Delete" Text="Merkmal entfernen">
        </telerik:RadMenuItem>
        <telerik:RadMenuItem Value="ShowReference" Text="Funktionsbewertungen mit diesem Merkmal anzeigen">
        </telerik:RadMenuItem>
        <telerik:RadMenuItem Value="Description" Text="Beschreibung anzeigen">
        </telerik:RadMenuItem>
    </Items>
</telerik:RadContextMenu>

