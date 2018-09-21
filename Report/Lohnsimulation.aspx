<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="Lohnsimulation.aspx.cs" Inherits="ch.appl.psoft.Report.Lohnsimulation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div>
        <asp:Label ID="paramSimLabel" runat="server" Text=""></asp:Label><br />
        <br />
        <table>
            <tr>
                <td>OE:&nbsp;<asp:DropDownList ID="lstOE" runat="server"></asp:DropDownList></td>
                <td>
                    <asp:CheckBox ID="chkSubOEs" runat="server" Checked="True" Text="Sub-OEs" /></td>
            </tr>
            <tr>
                <td style="width: 225px;">
                    <asp:RadioButton ID="chkStandard" GroupName="simulation" runat="server"
                        Text="Standard Simulation" Checked="True" />
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="chkWertPunkt" GroupName="simulation" runat="server"
                        Text="Wert/Punkt" />
                </td>
                <td>

                    <telerik:RadNumericTextBox ID="txtWertPunkt" runat="server" Value="0" Width="100px"></telerik:RadNumericTextBox>
                    &nbsp;<asp:Label
                        ID="valPointUnitLabel" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="totalSumCorrLabel" runat="server" Text=""></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="chkKorrekturabsolut" GroupName="korrektur" Text="Absolut"
                        runat="server" Checked="True" />
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtKorrekturabsolut" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;<asp:Label
                        ID="absUnitLabel" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="chkKorrekturrelativ" GroupName="korrektur" Text="Relativ" runat="server" />
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtKorrekturrelativ" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="fixChangeLabel" runat="server" Text=""></asp:Label></td>
                <td>&nbsp;</td>
            </tr>
            <asp:Panel ID="pFix" runat="server">
                <tr>
                    <td>
                        <asp:RadioButton ID="chkAbsolut" GroupName="aenderung" Text="Absolut" runat="server" />
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtAbsolut" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;CHF
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton ID="chkRelativ" GroupName="aenderung" Text="Relativ" runat="server" />
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtRelativ" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pRel" runat="server">
                <tr>
                    <td>
                        <asp:Label ID="diffNomActLabel" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="changePercentLabel" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&lt;&nbsp;<telerik:RadNumericTextBox ID="txtAbw0" Value="10" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtAbw0p" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                </tr>
                <tr>
                    <td>&lt;&nbsp;<telerik:RadNumericTextBox ID="txtAbw1" Value="15" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtAbw1p" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                </tr>
                <tr>
                    <td>&lt;&nbsp;<telerik:RadNumericTextBox ID="txtAbw2" Value="20" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtAbw2p" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                </tr>
                <tr>
                    <td>&lt;&nbsp;<telerik:RadNumericTextBox ID="txtAbw3" Value="100" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtAbw3p" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="incBaseSalaryLabel" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtBasislohn" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>&nbsp;%
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label ID="maxIncreaseLabel" runat="server"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtMaxIncrease" Value="0" runat="server" Width="100px"></telerik:RadNumericTextBox>
                    <asp:Label
                        ID="increaseUnitLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="roundLabel" runat="server"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtRunden" Value="5" runat="server" Width="100px"></telerik:RadNumericTextBox>
                    <asp:Label
                        ID="roundUnitLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="expotrOptions" runat="server" Text=""></asp:Label></td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td>
                    <asp:CheckBox ID="checkBoxCorrectionExport" runat="server" Checked="True"
                        Text="ExportCorrection" Visible="False" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="checkBoxAddressExport" runat="server" Text="ExportAdderss" />
                </td>
            </tr>

        </table>
        <br />
        <asp:Label ID="lblFehler" runat="server" Text="" ForeColor="Red" Font-Bold="True"></asp:Label>
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Anzeigen" OnClick="cmdOk_Click" />
        <br />
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style2 {
            height: 4px;
        }
    </style>
</asp:Content>
