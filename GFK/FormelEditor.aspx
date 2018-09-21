<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="FormelEditor.aspx.cs" Inherits="ch.appl.psoft.GFK.FormelEditor" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div>

        <p class="auto-style1">
            <asp:Label ID="OutputLabel" runat="server" Font-Italic="True" ForeColor="#FF3300"></asp:Label>
        </p>
        <telerik:RadTabStrip ID="WebTab1" runat="server" Height="194px" Width="619px"></telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="MultiPage">
            <telerik:RadPageView ID="Quickview" runat="server">
                <p class="auto-style1">
                    <asp:Table ID="Table6" runat="server" Width="521px">
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">File name:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:TextBox ID="FileNameBox" runat="server" Width="337px"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyButton" runat="server" OnClick="Button1_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Report typ:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:DropDownList ID="ReportTypList" runat="server" Width="337px">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyReportTyp" runat="server" OnClick="GetProducts_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Product:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:DropDownList ID="ProductsList" runat="server" Width="337px">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyProduct" runat="server" OnClick="GetSegments_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Segment:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:DropDownList ID="SegmentsList" runat="server" Width="337px">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplySegment" runat="server" OnClick="GetFacts_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Fact:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:DropDownList ID="FactsList" runat="server" Width="337px"></asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyFact" runat="server" OnClick="ApplyFact_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </p>
            </telerik:RadPageView>
            <telerik:RadPageView ID="Excel" runat="server">
                <p class="auto-style1">
                    <asp:Table ID="Table5" runat="server" Height="20px" Width="536px">
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Excel file name:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:TextBox ID="ExcelFileBox" runat="server" Width="337px"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyExcelFile" runat="server" OnClick="ApplyExcelFile_Click" Text="Apply" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Excel sheet:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:DropDownList ID="ExcelSheetList" runat="server" Width="337px"></asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Tablename 1:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:TextBox ID="Title1Box" runat="server" Width="337px"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server">Tablename 2:</asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:TextBox ID="Title2Box" runat="server" Width="337px"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell runat="server">
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server"></asp:TableCell>
                            <asp:TableCell runat="server"></asp:TableCell>
                            <asp:TableCell runat="server">
                                <asp:Button ID="ApplyTablenames" runat="server" OnClick="ApplyTablenames_Click" Text="Apply" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </p>
            </telerik:RadPageView>
</telerik:RadMultiPage>

            <p class="auto-style1" style="font-weight: bold">
                Second Table:
            </p>
            <p class="auto-style1" style="font-weight: normal">

                <asp:CheckBox ID="SecondTableEnabledBox" runat="server" Text="Enable second table" />
            </p>
            <p class="auto-style1" style="font-weight: normal">

                <asp:CheckBox ID="PreviousYearBox" runat="server" Text="Take previous year" />
            </p>
            <telerik:RadTabStrip ID="WebTab2" runat="server" Height="194px" Width="619px"></telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPage1">
                <telerik:RadPageView ID="RadPageView1" runat="server">
                    <p class="auto-style1">
                        <asp:Table ID="Table9" runat="server" Width="521px">
                            <asp:TableRow ID="TableRow21" runat="server">
                                <asp:TableCell ID="TableCell49" runat="server">File name:</asp:TableCell>
                                <asp:TableCell ID="TableCell50" runat="server">
                                    <asp:TextBox ID="FileNameBox0" runat="server" Width="337px"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell51" runat="server">
                                    <asp:Button ID="ApplyButton0" runat="server" OnClick="Apply2_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow22" runat="server">
                                <asp:TableCell ID="TableCell52" runat="server">Report typ:</asp:TableCell>
                                <asp:TableCell ID="TableCell53" runat="server">
                                    <asp:DropDownList ID="ReportTypList0" runat="server" Width="337px">
                                    </asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell54" runat="server">
                                    <asp:Button ID="ApplyReportTyp0" runat="server" OnClick="GetProducts2_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow23" runat="server">
                                <asp:TableCell ID="TableCell55" runat="server">Product:</asp:TableCell>
                                <asp:TableCell ID="TableCell56" runat="server">
                                    <asp:DropDownList ID="ProductsList0" runat="server" Width="337px">
                                    </asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell57" runat="server">
                                    <asp:Button ID="ApplyProduct0" runat="server" OnClick="GetSegments2_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow24" runat="server">
                                <asp:TableCell ID="TableCell58" runat="server">Segment:</asp:TableCell>
                                <asp:TableCell ID="TableCell59" runat="server">
                                    <asp:DropDownList ID="SegmentsList0" runat="server" Width="337px">
                                    </asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell60" runat="server">
                                    <asp:Button ID="ApplySegment0" runat="server" OnClick="GetFacts2_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow25" runat="server">
                                <asp:TableCell ID="TableCell61" runat="server">Fact:</asp:TableCell>
                                <asp:TableCell ID="TableCell62" runat="server">
                                    <asp:DropDownList ID="FactsList0" runat="server" Width="337px"></asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell63" runat="server">
                                    <asp:Button ID="ApplyFact0" runat="server" OnClick="ApplyFact2_Click" Style="margin-left: 4px" Text="Apply" Width="65px" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </p>

                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageView2" runat="server">

                    <p class="auto-style1">
                        <asp:Table ID="Table8" runat="server" Height="20px" Width="536px">
                            <asp:TableRow ID="TableRow26" runat="server">
                                <asp:TableCell ID="TableCell64" runat="server">Excel file name:</asp:TableCell>
                                <asp:TableCell ID="TableCell65" runat="server">
                                    <asp:TextBox ID="ExcelFileBox0" runat="server" Width="337px"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell66" runat="server">
                                    <asp:Button ID="ApplyExcelFile0" runat="server" OnClick="ApplyExcelFile2_Click" Text="Apply" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow27" runat="server">
                                <asp:TableCell ID="TableCell67" runat="server">Excel sheet:</asp:TableCell>
                                <asp:TableCell ID="TableCell68" runat="server">
                                    <asp:DropDownList ID="ExcelSheetList0" runat="server" Width="337px"></asp:DropDownList>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell69" runat="server"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow28" runat="server">
                                <asp:TableCell ID="TableCell70" runat="server">Tablename 1:</asp:TableCell>
                                <asp:TableCell ID="TableCell71" runat="server">
                                    <asp:TextBox ID="Title1Box0" runat="server" Width="337px"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell72" runat="server"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow29" runat="server">
                                <asp:TableCell ID="TableCell73" runat="server">Tablename 2:</asp:TableCell>
                                <asp:TableCell ID="TableCell74" runat="server">
                                    <asp:TextBox ID="Title2Box0" runat="server" Width="337px"></asp:TextBox>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell75" runat="server">
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow ID="TableRow30" runat="server">
                                <asp:TableCell ID="TableCell76" runat="server"></asp:TableCell>
                                <asp:TableCell ID="TableCell77" runat="server"></asp:TableCell>
                                <asp:TableCell ID="TableCell78" runat="server">
                                    <asp:Button ID="ApplyTablenames0" runat="server" OnClick="ApplyTablenames2_Click" Text="Apply" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </p>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
            <p class="auto-style1" style="font-weight: bold">
                &nbsp;
            </p>
            <p class="auto-style1" style="font-weight: bold">
                Total Value
            </p>
            <p class="auto-style1">

                <asp:CheckBox ID="EnableTotal" runat="server" OnCheckedChanged="CheckBox1_CheckedChanged" Text="Enable total value" AutoPostBack="True" />
                &nbsp;
            </p>
            <p class="auto-style1">
                &nbsp;
            </p>
            <p class="auto-style1">
                Formula:<br />
                <asp:TextBox ID="FormulaBoxTotal" runat="server" Height="54px" Width="328px" TextMode="MultiLine"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unit&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="UnitBoxTotal" runat="server" Style="margin-bottom: 0px"></asp:TextBox>
            </p>
            <p class="auto-style1">

                <asp:ListBox ID="FieldsList3" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToTotal" runat="server" OnClick="AddToFormulaTotal_Click" Text="add" />
                &nbsp;&nbsp;&nbsp;
        <asp:ListBox ID="FieldsList4" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToFormula3" runat="server" OnClick="AddToFormulaTotal2_Click" Text="add" />
            </p>
            <p class="auto-style1">
                Second total value:
            </p>
            <p class="auto-style1">

                <asp:CheckBox ID="SecondTotalEnabledBox" runat="server" Text="Enable second total value" />
            </p>
            <p class="auto-style1">

                <asp:CheckBox ID="EnableSecondTotalHidden" runat="server" OnCheckedChanged="CheckBox1_CheckedChanged" Text="Hide Second Total" AutoPostBack="True" />
            </p>
            <p class="auto-style1">
                Formula:<br />
                <asp:TextBox ID="FormulaBoxTotal0" runat="server" Height="54px" Width="328px" TextMode="MultiLine"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unit&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="UnitBoxTotal0" runat="server" Style="margin-bottom: 0px"></asp:TextBox>
            </p>
            <p class="auto-style1">

                <asp:ListBox ID="FieldsList7" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToTotal0" runat="server" OnClick="AddToFormulaTotal12_Click" Text="add" />
                &nbsp;&nbsp;&nbsp;
        <asp:ListBox ID="FieldsList8" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToFormula6" runat="server" OnClick="AddToFormulaTotal22_Click" Text="add" />
            </p>
            <p class="auto-style1" style="font-weight: bold">
                First value<br />
            </p>
            <p class="auto-style1">

                <asp:CheckBox ID="EnableCount" runat="server" Text="Count" AutoPostBack="True" OnCheckedChanged="EnableCount_CheckedChanged" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Number of entries:
            <asp:TextBox ID="NumberOfEntriesBox" runat="server"></asp:TextBox>
                &nbsp;
            </p>
            <p class="auto-style1">
                &nbsp;<br />
                Formula:<br />
                <asp:TextBox ID="FormulaBox1" runat="server" Height="54px" Width="328px" TextMode="MultiLine"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unit&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="UnitBox1" runat="server" Style="margin-bottom: 0px"></asp:TextBox>
                <br />
                <br />
                Available Fields:<br />
                <asp:ListBox ID="FieldsList1" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToFormula1" runat="server" OnClick="AddToFormula1_Click" Text="add" />
                &nbsp;&nbsp;&nbsp;
        <asp:ListBox ID="FieldsList5" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToFormula4" runat="server" OnClick="AddToFormula3_Click" Text="add" />
                <br />
                <br />
            </p>
            <p class="auto-style1" style="font-weight: bold">

                <br />
                Second value
            </p>
            <p class="auto-style1">
                <br />
                <br />
                <asp:CheckBox ID="EnableSecondValue" runat="server" OnCheckedChanged="EnableSecondValue_CheckedChanged" Text="Enable second value" TextAlign="Left" AutoPostBack="True" />
                <br />
                <br />
                Formula<br />
                <br />
                <asp:TextBox ID="FormulaBox2" runat="server" Height="54px" Width="328px" Enabled="False" TextMode="MultiLine"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unit&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="UnitBox2" runat="server" Enabled="False"></asp:TextBox>
                <br />
                <br />
                Available Fields:<br />
                <br />
                <asp:ListBox ID="FieldsList2" runat="server" Width="335px" Enabled="False"></asp:ListBox>
                <asp:Button ID="AddToFormula2" runat="server" OnClick="AddToFormula2_Click" Text="add" />
                &nbsp;&nbsp;&nbsp;
        <asp:ListBox ID="FieldsList6" runat="server" Width="335px"></asp:ListBox>
                <asp:Button ID="AddToFormula5" runat="server" OnClick="AddToFormula4_Click" Text="add" />
                <br />
            </p>
            <p class="auto-style1">
                &nbsp;
            </p>
            <p class="auto-style1" style="font-weight: bold">
                Sorting:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </p>
            <p class="auto-style1" style="font-weight: normal">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
                <asp:RadioButton ID="SortedHighestFirst" runat="server" Checked="True" GroupName="ValueFirst" Text="Highest value first" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:RadioButton ID="SortedLowestFirst" runat="server" GroupName="ValueFirst" Text="Lowest value first" />
            </p>
            <p class="auto-style1">
                <asp:CheckBox ID="SortingOnSecondValueBox" runat="server" Text="Sorting on second value" />
            </p>
            <p class="auto-style1">
                <asp:CheckBox ID="BorderEnabledBax" runat="server" Text="Enable Border" />
            </p>
            <p class="auto-style1">
                Highest value:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="HighestValueBox" runat="server"></asp:TextBox>
            </p>
            <p class="auto-style1">
                Lowest value:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="LowestValueBox" runat="server" Style="margin-left: 3px"></asp:TextBox>
                <br />
            </p>
            <p class="auto-style1" style="font-weight: bold">
                <br />
                Coloration:
            </p>
            <p class="auto-style1">
                <asp:RadioButton ID="HighestIsGreen" runat="server" GroupName="ColorOrder" Text="Highest value is green" Checked="True" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:RadioButton ID="LowestIsGreen" runat="server" GroupName="ColorOrder" Text="Lowest value is green" />
            </p>
            <p class="auto-style1">
                <asp:CheckBox ID="ColorationOnSecondValueBox" runat="server" Text="Coloration on second value" />
            </p>
            <p class="auto-style1">
                Coloration Total Value:
            </p>
            <p class="auto-style1">
                <asp:CheckBox ID="ColorationSecondTotalBox" runat="server" Text="Coloration on second total value" />
            </p>
            <p class="auto-style1">
                <asp:Table ID="Table7" runat="server" Height="29px" Width="640px" Style="margin-left: 105px">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Coloration border values:</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Green / Light green</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="greenLightgreenBoxTotal" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Light green / Yellow</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="lightgreenYellowBoxTotal" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Yellow / Orange</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="yellowOrangeBoxTotal" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Orange / Red</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="orangeRedBoxTotal" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
            </p>
            <p class="auto-style1">
                Coloration Default:<br />
            </p>
            <p class="auto-style1">
                <asp:Table ID="Table4" runat="server" Height="29px" Width="640px" Style="margin-left: 105px">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Coloration border values:</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Green / Light green</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="greenLightgreenBox" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Light green / Yellow</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="lightgreenYellowBox" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Yellow / Orange</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="yellowOrangeBox" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Orange / Red</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="orangeRedBox" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </p>

            <p class="auto-style1">
                <br />
                &nbsp;&nbsp;&nbsp;
            <asp:Button ID="OkButton" runat="server" Text="Save" OnClick="OkButton_Click" Width="58px" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </p>
    </div>
    <p>
        &nbsp;
    </p>
</asp:Content>

<%--<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .auto-style1 {
            margin-left: 80px;
        }
    </style>
</asp:Content>--%>


