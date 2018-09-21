<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="ch.appl.psoft.Payment.Order" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function DescriptionClicking(sender, args) {
                var dok = $("#" + sender.get_element().id)[0].parentElement.children[1].value;
                var url = window.location.href.replace("Order.aspx", "DetailDescription/") + dok;
                var detailwindow = radopen(url, null);
                detailwindow.setSize(800, 600);
                cetailwindow.center();
                args.set_cancel(true);

            }
        </script>
    </telerik:RadCodeBlock>
    <%--<link href="../Style/Psoft.css" rel="stylesheet" />--%>
    <div class="table" style="width: 90%; min-width: 900px; max-width: 1500px">
        <div class="row">
            <div class="titleCell">
                <asp:Label ID="TitleLabel" runat="server" EnableViewState="False" Text="Produkte" CssClass="titleLabel"></asp:Label>
            </div>
        </div>
        <telerik:RadGrid ID="ProductTable" runat="server" DataSourceID="SqlDataSource" OnItemCommand="ProductTable_ItemCommand" Culture="de-DE">
            <GroupingSettings CollapseAllTooltip="Collapse all groups"></GroupingSettings>

            <MasterTableView EditMode="PopUp" DataSourceID="SqlDataSource" DataKeyNames="ID" AutoGenerateColumns="false">
                <Columns>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="True"
                        UniqueName="Id" Display="False" />
                    <telerik:GridBoundColumn DataField="TITLE" HeaderStyle-CssClass="ProductTableHeader" HeaderText="Produkt" UniqueName="Title" ItemStyle-Width="20%" ColumnValidationSettings-EnableRequiredFieldValidation="true" ColumnEditorID="TitleEdit" ItemStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="DESCRIPTION" HeaderText="Beschreibung" UniqueName="Description" ColumnEditorID="DescriptionEdit" ItemStyle-Width="40%" ItemStyle-Wrap="true" />
                    <telerik:GridTemplateColumn DataField="DESCRIPTION_LINK" >
                        <ItemTemplate>
                            <telerik:RadButton ID="DescriptionButton" runat="server" Text="Details" OnClientClicking="DescriptionClicking"></telerik:RadButton>
                            <asp:HiddenField ID="DescriptionLink" Value='<%# Bind("DESCRIPTION_LINK") %>' runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn DataField="PRICE" HeaderText="Preis" UniqueName="Price" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Right" />
                    <telerik:GridBoundColumn DataField="CURRENCY" HeaderText="Währung" UniqueName="Currency" ItemStyle-Width="10%" />
                    <telerik:GridDropDownColumn DataField="MATRIX_REF" UniqueName="MatrixRef" runat="server" DataSourceID="SqlDataSource1" ListTextField="TITLE" ListValueField="ID" Visible="false" HeaderText="Produktreferenz" DropDownControlType="DropDownList" ColumnEditorID="ProductRefEdit">
                    </telerik:GridDropDownColumn>
                    <telerik:GridBoundColumn DataField="NUMBER_OF_CARDS" HeaderText="Anzahl Karten" UniqueName="NumberOfCards" Visible="false" />
                    <telerik:GridButtonColumn UniqueName="PayButton" CommandName="pay" ButtonType="ImageButton" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <div style="margin-top: 15px">
            <telerik:RadLabel ID="lblNumCards" runat="server" Font-Size="Larger"></telerik:RadLabel>
        </div>
    </div>
    <telerik:GridTextBoxColumnEditor ID="DescriptionEdit" runat="server" TextBoxMode="MultiLine">
        <TextBoxStyle Height="50px" Width="250px" />
    </telerik:GridTextBoxColumnEditor>
    <telerik:GridTextBoxColumnEditor ID="TitleEdit" runat="server" TextBoxMode="SingleLine">
        <TextBoxStyle Width="250px" />
    </telerik:GridTextBoxColumnEditor>
    <telerik:GridDropDownListColumnEditor ID="ProductRefEdit" DropDownStyle-Width="250px"
        runat="server" />

    <asp:SqlDataSource ID="SqlDataSource" runat="server"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM PRODUCTS ORDER BY ID"
        DeleteCommand="DELETE FROM PRODUCTS  WHERE ID = @ID"
        InsertCommand="INSERT INTO PRODUCTS(TITLE, DESCRIPTION, PRICE, CURRENCY, MATRIX_REF, NUMBER_OF_CARDS) VALUES (@TITLE, @DESCRIPTION, @PRICE, @CURRENCY, @MATRIX_REF, @NUMBER_OF_CARDS)"
        UpdateCommand="UPDATE PRODUCTS SET TITLE = @TITLE, DESCRIPTION = @DESCRIPTION, PRICE = @PRICE, CURRENCY = @Currency, MATRIX_REF = @MATRIX_REF, NUMBER_OF_CARDS = @NUMBER_OF_CARDS WHERE ID = @ID">

        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Title" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Price" Type="Decimal" />
            <asp:Parameter Name="Currency" Type="String" />
            <asp:Parameter Name="Matrix_Ref" Type="Int32" />
            <asp:Parameter Name="Number_Of_Cards" Type="Int32" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Id" Type="Int32" />
            <asp:Parameter Name="Title" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Price" Type="Decimal" />
            <asp:Parameter Name="Currency" Type="String" />
            <asp:Parameter Name="Matrix_Ref" Type="Int32" />
            <asp:Parameter Name="Number_Of_Cards" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource runat="server" ID="SqlDataSource1"
        SelectCommand="SELECT ID, TITLE FROM MATRIX ORDER BY TITLE"></asp:SqlDataSource>

    <telerik:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="Window1" runat="server">
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
