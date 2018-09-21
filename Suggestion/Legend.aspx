<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="Legend.aspx.cs" Inherits="ch.appl.psoft.Suggestion.Legend" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:GridView ID="legend" runat="server" DataSourceID="LegendItems"  
        AutoGenerateColumns="False" CellPadding="4" BorderWidth="0px">
        <HeaderStyle BackColor="Aqua" />
        
        <Columns>            
            <asp:TemplateField HeaderText="title" SortExpression="title">
                <ItemTemplate>
                    <asp:Label ID="titleLabel" runat="server" Text='<%# Eval("title") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="color" SortExpression="color">
                <ItemTemplate>
                    <asp:Panel ID="colorPanel" runat="server" BackColor='<%# System.Drawing.Color.FromArgb((int) Eval("color")) %>' CssClass="colorPanel"></asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:LinqDataSource ID="LegendItems" runat="server" 
        ContextTypeName="SEEK_LINQ.Suggestion.SuggestionDataContext" 
        OrderBy="ordnumber" Select="new (title, color)" TableName="Status">
    </asp:LinqDataSource>

   </asp:Content>