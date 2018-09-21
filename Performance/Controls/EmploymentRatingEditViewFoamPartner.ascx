<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="EmploymentRatingEditViewFoamPartner.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.EmploymentRatingEditViewFoamPartner" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Src="~/Training/Controls/AdvancementEditCtrl.ascx" TagPrefix="uc1" TagName="AdvancementEditCtrl" %>
<%@ Register Src="~/Training/Controls/TrainingCatalogTreeCtrl.ascx" TagPrefix="uc1" TagName="TrainingCatalogTreeCtrl" %>

<script src="../JavaScript/Tree/ftiens4.js"></script>
<script src="../JavaScript/Tree/ua.js"></script>
<script type="text/javascript">

    function saveClick(sender, args) {
        if ($("#ContentPlaceHolder1__pl__cl__erev_potentialList").length > 0 && $("#ContentPlaceHolder1__pl__cl__erev_potentialList")[0].value == 0) {
            $("#ctl00_ContentPlaceHolder1__pl__cl__erev_potentialMissingError")[0].style.display = '';
            args._cancel = true;
        }
        else if ($("#ContentPlaceHolder1__pl__cl__erev_Input-PERFORMANCERATING_ITEMS_V-ARGUMENTS")[0].value.length == 0) {
            $find("ctl00_ContentPlaceHolder1__pl__cl__erev_ErrorWindow").show();
            args._cancel = true;
        }
        else if ($("#ContentPlaceHolder1__pl__cl__erev_Input-PERFORMANCERATING_ITEMS_V-LEVEL_REF")[0].value.length == 0){
            $("#ctl00_ContentPlaceHolder1__pl__cl__erev_missingRating")[0].style.display = '';
            args._cancel = true;
        }
        else {
            args._cancel = false;
        }
    }

    function closeMissingReason() {
        $find('ctl00_ContentPlaceHolder1__pl__cl__erev_ErrorWindow').close();
    }

    function addMesureClick(sender, args) {
        $find('ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow').show();
    }

    function TrainigWindowBeforShow(sender, args) {
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_toBedoneDateValidator")[0].enabled = true;
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_designationValidation")[0].enabled = true;
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_responsibileValidator")[0].enabled = true;
    }

    function cancelTrainig() {
        $find('ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow').close();
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_toBedoneDateValidator")[0].enabled = false;
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_designationValidation")[0].enabled = false;
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_responsibileValidator")[0].enabled = false;
    }


    $(document).ready(function () {
        if ($("#ContentPlaceHolder1__pl__cl__erev_Input-PERFORMANCERATING_ITEMS_V-ARGUMENTS").length !== 0) {
            $("#ContentPlaceHolder1__pl__cl__erev_Input-PERFORMANCERATING_ITEMS_V-ARGUMENTS")[0].cols = 100;
            $("#ctl00_ContentPlaceHolder1__pl__cl__erev_missingRating")[0].style.display = 'none';
        }
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_potentialMissingError")[0].style.display = 'none';
    });

    function SelectPotential() {
        $("#ctl00_ContentPlaceHolder1__pl__cl__erev_potentialMissingError")[0].style.display = 'none';
    }

</script>


<style type="text/css">
    .RadDropDownList {
        width: auto !important;
        min-width: 150px;
    }

    #ctl00_ContentPlaceHolder1__pl__cl__erev_TrainigWindow_C_trainigneeds_DropDown {
        width: auto !important;
    }

    #ctl00_ContentPlaceHolder1__pl__cl__erev_missingRating{
        color:red;
        font-weight:bold;
        font:bold;
    }
</style>

<asp:Table ID="TitleTable" runat="server" CellSpacing="0" CellPadding="2" BorderWidth="0">
    <asp:TableRow Height="10px">
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell ID="titleCell" runat="server">
            <asp:Label ID="TITLE_VALUE" CssClass="section_title" runat="server"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>

<asp:Table ID="listTab" CssClass="InputMask" BorderWidth="0" runat="server" Width="66%" Style="margin-bottom: 0px">
    <asp:TableRow>
        <asp:TableCell>
            <asp:Label runat="server" ID="CriteriaTitle" Font-Bold="true"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell></asp:TableCell>
        <asp:TableCell>
            <telerik:RadTabStrip ID="TabStripExpectations" runat="server" Width="120%" OnTabClick="TabStripExpectations_TabClick">
            </telerik:RadTabStrip>

            <telerik:RadMultiPage runat="server" ID="MultiPage">
                <telerik:RadPageView ID="clarificationView" runat="server" EnableViewState="false">
                    <table style="width: 100%">
                        <tr>
                            <td style="background-color: lavenderblush">
                                <asp:Label ID="clarificationTitle" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="background-color: lavenderblush">
                                <asp:Label ID="questionTitle" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTextBox runat="server" ID="clarificationText" Height="150" Width="100%" TextMode="MultiLine" Font-Size="Medium" ReadOnly="true"></telerik:RadTextBox>
                            </td>
                            <td>
                                <telerik:RadTextBox runat="server" ID="questionText" Height="150" Width="100%" TextMode="MultiLine" Font-Size="Medium" ReadOnly="true"></telerik:RadTextBox>
                            </td>
                        </tr>
                    </table>
                </telerik:RadPageView>
                <telerik:RadPageView ID="clarificationLeadershipView" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="background-color: lavenderblush">
                                <asp:Label ID="clarificationTitleLeader" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="background-color: lavenderblush">
                                <asp:Label ID="questionTitleLeader" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTextBox runat="server" ID="clarificationLeadershipText" Height="150" Width="100%" TextMode="MultiLine" Font-Size="Medium" ReadOnly="true"></telerik:RadTextBox>
                            </td>
                            <td>
                                <telerik:RadTextBox runat="server" ID="questionLeadershipText" Height="150" Width="100%" TextMode="MultiLine" Font-Size="Medium" ReadOnly="true"></telerik:RadTextBox>
                            </td>
                        </tr>

                    </table>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </asp:TableCell>
    </asp:TableRow>

</asp:Table>

<asp:Table ID="editTab" CssClass="InputMask" runat="server" BorderWidth="1" GridLines="Both">
</asp:Table>

<asp:Table ID="argumentTab" CssClass="InputMask" runat="server" BorderWidth="0">
</asp:Table>
<asp:Table ID="developmentTab" CssClass="InputMask" runat="server" BorderWidth="0">
    <asp:TableRow>
        <asp:TableCell>
            <asp:Label ID="potentialTitle" runat="server" class="InputMask_Label" Visible="false"></asp:Label>
        </asp:TableCell>
        <asp:TableCell>
            <asp:DropDownList ID="potentialList" runat="server" Visible="false" OnSelectedIndexChanged="potentialList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><telerik:RadTextBox ID="potentialMissingError" runat="server" Font-Bold="true" ForeColor="Red" ReadOnly="true" Text="Benötigt!" BorderStyle="None" Font-Size="Larger" ClientEvents-OnValueChanged="SelectPotential"></telerik:RadTextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:Label ID="develpomentTittle" runat="server" class="InputMask_Label"></asp:Label>
        </asp:TableCell>
        <asp:TableCell Style="width: 100%">
            <telerik:RadTextBox runat="server" ID="developmentMeasuresText" Height="100" Width="100%" TextMode="MultiLine" Font-Size="Medium" ReadOnly="true" Visible="false"></telerik:RadTextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<br>

<table>
    <tr>
        <td>
            <telerik:RadButton ID="apply" CssClass="Button" Font-Bold="true" runat="server" OnClientClicking="saveClick" OnClick="apply_Click"></telerik:RadButton>
        </td>
        <td>
            <telerik:RadButton ID="AddMesure" CssClass="Button" Font-Bold="true" runat="server" OnClientClicking="addMesureClick" AutoPostBack="false"></telerik:RadButton>
        </td>
    </tr>
</table>
<br>
<telerik:RadWindow ID="ErrorWindow" runat="server" Modal="true" VisibleOnPageLoad="false" Height="160px">
    <ContentTemplate>
        <table>
            <tr>
                <td>
                    <asp:Label ID="missingReasonText" runat="server"></asp:Label>
                </td>
            </tr>
            <tr></tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btCloseMissingReason" CssClass="Button" Font-Bold="true" Text="OK" OnClientClick="closeMissingReason(); return false;" />

                </td>
            </tr>
        </table>

    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow ID="TrainigWindow" runat="server" Modal="true" Width="1100" Height="650" OnLoad="TrainigWindow_Load" OnClientBeforeShow="TrainigWindowBeforShow" Behaviors="Move,Resize" >
    <ContentTemplate>
        <table class="TrainingTable">
            <tr class="TrainigTableTitleRow">
                <td class="TrainigTableTitleCell" colspan="2">
                    <telerik:RadLabel ID="formtitle" runat="server" CssClass="TrainigFormTitle" Font-Bold="true" Font-Size="Larger"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell"></td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="designationTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadTextBox ID="designation" runat="server" MaxLength="128" Width="500"></telerik:RadTextBox><asp:RequiredFieldValidator runat="server" ID="designationValidation" ControlToValidate="designation" ErrorMessage="Benötigt" Font-Bold="true" ForeColor="Red" Enabled="false" />
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="descriptionTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadTextBox ID="description" runat="server" Width="500" Height="100" TextMode="MultiLine"></telerik:RadTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="toBedoneDateTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadDatePicker ID="toBedoneDate" runat="server" DateInput-DateFormat="dd.MM.yyy" ViewStateMode="Disabled">
                        <DateInput runat="server" ID="toBedoneDateInput" DateFormat="dd.MM.yyyy"></DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator runat="server" ID="toBedoneDateValidator" ControlToValidate="toBedoneDate" ErrorMessage="Benötigt" Font-Bold="true" ForeColor="Red" Enabled="false" />
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="controllingTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadTextBox ID="controlling" runat="server" Width="500" Height="100" TextMode="MultiLine"></telerik:RadTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="costExternTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadNumericTextBox ID="costExtern" runat="server" NumberFormat-DecimalDigits="0" Value="0"></telerik:RadNumericTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="costInternTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadNumericTextBox ID="costIntern" runat="server" NumberFormat-DecimalDigits="0" Value="0"></telerik:RadNumericTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="courseLocationTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadTextBox ID="courseLocation" runat="server" MaxLength="128" Width="500"></telerik:RadTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="courseleaderTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadTextBox ID="courseleader" runat="server" MaxLength="128" Width="500"></telerik:RadTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="responsibileTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadDropDownList ID="responsibile" runat="server"></telerik:RadDropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="responsibileValidator" ControlToValidate="responsibile" ErrorMessage="Benötigt" Font-Bold="true" ForeColor="Red" Enabled="false" />
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="costSharingTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadNumericTextBox ID="costSharing" runat="server" NumberFormat-DecimalDigits="0" Value="0"></telerik:RadNumericTextBox>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="obligationTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadDatePicker ID="obligation" runat="server" DateInput-DateFormat="dd.MM.yyyy">
                        <DateInput runat="server" ID="obligationInput" DateFormat="dd.MM.yyyy"></DateInput>
                    </telerik:RadDatePicker>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="trainigneedsTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadDropDownList ID="trainigneeds" CssClass="DropDown" runat="server">
                    </telerik:RadDropDownList>
                </td>
            </tr>
            <tr class="TrainigTableRow">
                <td class="TrainigTableTitleCell">
                    <telerik:RadLabel ID="stateTitle" runat="server" CssClass="TrainigTableTitle"></telerik:RadLabel>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadDropDownList ID="state" runat="server"></telerik:RadDropDownList>
                </td>
            </tr>
            <tr class="TrainigTableRow" style="height:40px">
                <td class="TrainigTableTitleCell">
                    <telerik:RadButton ID="saveTraining" runat="server" Text="RadButton" OnClick="saveTraining_Click"></telerik:RadButton>
                </td>
                <td class="TrainigTableDataCell">
                    <telerik:RadButton ID="cancel" runat="server" Text="RadButton" AutoPostBack="false" OnClientClicking="cancelTrainig"></telerik:RadButton>
                </td>
            </tr>
        </table>
        <table style="position:absolute;right:100px; top:30px">
            <tr>
                <td>
                    <uc1:TrainingCatalogTreeCtrl runat="server" ID="TrainingCatalogTree" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</telerik:RadWindow>



