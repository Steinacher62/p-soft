<%@ Page MasterPageFile="~/Framework.Master" language="c#" CodeBehind="Bonus.aspx.cs" AutoEventWireup="True"  Inherits="ch.appl.psoft.Laufenburg.Bonus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style="display: table; padding-left:20px;">
<div  style="display: table-row">
        <div style="display: table-cell; font-weight:bold; font-size:larger">
            Parameter Prämienberechnung
        </div>
    </div>
</div>
<div style="display: table; padding:20px;">
<div  style="display: table-row">
        <div style="display: table-cell; padding-right:10px;">
           <asp:Label ID="Label15" runat="server" Text="Leistungsbewertungen von:"></asp:Label> <telerik:RadDatePicker ID="LBVon" runat="server" ></telerik:RadDatePicker>
        </div>
    <div style="display: table-cell;">
           <asp:Label ID="Label16" runat="server" Text="Leistungsbewertungen bis:"></asp:Label> <telerik:RadDatePicker ID="LBBis" runat="server"></telerik:RadDatePicker>
        </div>
    </div>
</div>
<div style="display: table; padding:10px;">
    <div style="display: table-row; width:300px; ">
        <div style="display: table-cell; padding:10px; font-weight:bold;">
            <asp:Label ID="Label1" runat="server" Text="Anzahl Punkte"></asp:Label>
        </div>
        <div style="display: table-cell; padding:10px; font-weight:bold;">
            <asp:Label ID="Label2" runat="server" Text="Prämie in % des Jahresgehalts"></asp:Label>
        </div>
    </div>
    <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label3" runat="server" Text="">1</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV1" Type="Number" Value="0.1" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label4" runat="server" Text="">2</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV2" Type="Number" Value="0.15" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label5" runat="server" Text="">3</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV3" Type="Number" Value="0.2" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label6" runat="server" Text="">4</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV4" Type="Number" Value="0.3" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label7" runat="server" Text="">5</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV5" Type="Number" Value="0.5" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label8" runat="server" Text="">6</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV6" Type="Number" Value="0.6" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label9" runat="server" Text="">7</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV7" Type="Number" Value="0.7" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label10" runat="server" Text="">8</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV8" Type="Number" Value="0.8" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label11" runat="server" Text="">9</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV9" Type="Number" Value="0.9" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label12" runat="server" Text="">10</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV10" Type="Number" Value="1" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label13" runat="server" Text="">11</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV11" Type="Number" Value="1.2" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; text-align:center">
            <asp:Label ID="Label14" runat="server" Text="">12</asp:Label>
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           <telerik:RadNumericTextBox ID="BV12" Type="Number" Value="1.6" EnabledStyle-HorizontalAlign="Right" runat="server" Width="100px" MinValue="0" NumberFormat-DecimalDigits="2" NumberFormat-PositivePattern="n%" Font-Size="Small"></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small; ">
            <telerik:RadButton ID="CalculateBonus" runat="server" Text="Prämien Berechnen" OnClick="CalculateBonus_Click"></telerik:RadButton>  
        </div>
        <div style="display: table-cell; padding:10px 10px 5px 5px; font-size:small">
           
        </div>
    </div>
</div>
</asp:Content>