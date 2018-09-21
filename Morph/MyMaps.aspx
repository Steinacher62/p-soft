<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="MyMaps.aspx.cs" Inherits="ch.appl.psoft.Morph.MyMaps" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function GridCreated(sender, args) {
            var containerHeight = $("#content")[0].clientHeight;
            var scrollArea = $find("ctl00_ContentPlaceHolder1_myProductsGrid").GridDataDiv;
            var parent = $get("GridContainer");
            var gridHeader = $find("ctl00_ContentPlaceHolder1_myProductsGrid").GridHeaderDiv;
            scrollArea.style.height = containerHeight -
            gridHeader.clientHeight + "px";
        }

        function Parse(sender, args) {
            var response = args.get_response().d;
            if (response) {
                args.set_parsedData(JSON.parse(response));
            }
        }

        function ParameterMap(sender, args) {
            //If you want to send a parameter to the select call you can modify the if 
            //statement to check whether the request type is 'read':
            //if (args.get_type() == "read" && args.get_data()) {
            if (args.get_type() != "read" && args.get_data()) {
                args.set_parameterFormat({ customersJSON: kendo.stringify(args.get_data().models) });
            }
        }

        function myProductGridRowCreated(sender, eventArgs) {

        }

        $(document).ready(function () {
            var rowHeight = document.documentElement.clientHeight - 181 + 'px';
            $("#GridContainerRow").css('height', rowHeight);
        });

    </script>

    <div id="Container" class="Container1" runat="server">
        <div style="display: table; height: 100%; width: 100%">
            <div style="display: table-row; text-align: center">
                <div style="display: table-cell; padding: 10px">
                    <telerik:RadTextBox ID="titleTb" runat="server" BorderStyle="None" CssClass="myProductsTitle" Width="100%" BackColor="Transparent"></telerik:RadTextBox>
                </div>
            </div>
            <div id="GridContainerRow" style="display: table-row; margin: 0 auto">
                <div style="display: table-cell; opacity: 0.8" id="GridContainer">
                    <telerik:RadGrid ID="myProductsGrid" runat="server" AllowPaging="true" PageSize="50" ClientDataSourceID="ClientDataSource" AllowFilteringByColumn="True" AllowSorting="True" AutoGenerateColumns="False" Culture="de-DE" Width="800px" CssClass="productGrid" ClientSettings-ClientEvents-OnRowCreated="myProductGridRowCreated" OnItemCreated="myProductsGrid_ItemCreated">
                        <GroupingSettings CollapseAllTooltip="Collapse all groups"></GroupingSettings>
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                            <ClientEvents OnGridCreated="GridCreated" />
                        </ClientSettings>

                        <MasterTableView>
                            <Columns>
                                <telerik:GridBoundColumn DataField="ID" Visible="false"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn DataField="TITLE" HeaderText="Bezeichnung" CurrentFilterFunction="Contains" ShowFilterIcon="false" AutoPostBackOnFilter="true" SortExpression="TITLE" FilterControlWidth="350px">
                                    <HeaderStyle Width="400px" />
<%--                                    <FilterTemplate>
                                        Clear filters
                        <asp:ImageButton ID="btnShowAll" runat="server" ImageUrl="Img/filterCancel.gif" AlternateText="Show All"
                            ToolTip="Show All" Style="vertical-align: middle" />
                                    </FilterTemplate>--%>
                                    <ClientItemTemplate>
                                       <a id = "Label1#=ID#"  href="#= matrixLink #" class="linkButton" title ="#= DESCRIPTION#"> #=TITLE#</a>
                                    </ClientItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridDateTimeColumn DataField="CREATIONDATE" HeaderText="Erstellt am:" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" SortExpression="CREATIONDATE" ShowFilterIcon="true" AutoPostBackOnFilter="true">
                                    <HeaderStyle Width="200px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridDateTimeColumn DataField="LASTCHANGE" HeaderText="Letzte Änderung" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" SortExpression="LASTCHANGE" ShowFilterIcon="true" AutoPostBackOnFilter="true">
                                    <HeaderStyle Width="200px" />
                                </telerik:GridDateTimeColumn>

                            </Columns>
                        </MasterTableView>

                    </telerik:RadGrid>
                </div>
            </div>
        </div>
        <telerik:RadClientDataSource runat="server" ID="ClientDataSource">
            <ClientEvents OnCustomParameter="ParameterMap" OnDataParse="Parse" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetMyMaps" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post"  />
                </WebServiceDataSourceSettings>
            </DataSource>
            <Schema>
                <Model ID="ID">
                    <telerik:ClientDataSourceModelField FieldName="ID" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="TITLE" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="matrixLink" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="DESCRIPTION" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="CREATIONDATE" DataType="Date" />
                    <telerik:ClientDataSourceModelField FieldName="LASTCHANGE" DataType="Date" />
                </Model>
            </Schema>
        </telerik:RadClientDataSource>

    </div>


</asp:Content>


