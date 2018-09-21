<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.PersonDetailCtrl" %>

<div style="height: 100%;" class="personDetail">
    <asp:HiddenField ID="PersonId" Value="0" runat="server" />
    <div class="CommandRow">
        <div style="display: table-cell;" class="CommandCell">
            <telerik:RadImageButton CssClass="SaveImageButton" ID="SavePersonImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SavePersonClick" AutoPostBack="false"></telerik:RadImageButton>
            <telerik:RadImageButton ID="DeltePersonImageButton" runat="server" Image-Url="../Images/delete_enable.gif" Image-DisabledUrl="../images/delete_disable.gif" Height="20px" OnClientClicking="DeletePersonClick" AutoPostBack="false"></telerik:RadImageButton>
            <telerik:RadImageButton ID="AddPersonImageButton" runat="server" Image-Url="../Images/add_enabled.gif" Image-DisabledUrl="../images/add_disabled.gif" Height="20px" OnClientClicking="AddPersonClick" AutoPostBack="false"></telerik:RadImageButton>
            <telerik:RadImageButton ID="NewAddressPersonImageButton" runat="server" Image-Url="../Images/newAddress_enable.gif" Image-DisabledUrl="../images/newAddress_disable.gif" Height="20px" OnClientClicking="AddAddressPersonClick" AutoPostBack="false"></telerik:RadImageButton>
        </div>
    </div>
    <div style="overflow-y:auto; height: calc(100% - 70px);">
        <div style="display: table; height: 100%; width: 100%;" class="PersonTable">
            <div style="display: table-row;" class="titleRow">
                <div style="display: table-cell; " class="adminTitleCell">
                    <asp:Label ID="PersonTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; ">
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="FirmTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDropDownList ID="FirmData" runat="server"></telerik:RadDropDownList>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="ClipboardTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadImageButton ID="ClipboardData" runat="server" OnClientClicking="PersonClipboardClicking" ></telerik:RadImageButton>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="NameTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="NameData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="PersonNameDataValidator" runat="server" Display="Dynamic" ControlToValidate="NameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="FirstnameTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="FirstnameData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="FirstnameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="MNEMOTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="MNEMOData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="TitleTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="TitleData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="PersonnelnumberTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="PersonnelnumberData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidatorPersonnelnumber" runat="server" Display="Dynamic" ControlToValidate="PersonnelnumberData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false" ></asp:RequiredFieldValidator>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="SexTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDropDownList ID="SexData" runat="server"></telerik:RadDropDownList>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="MartialTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDropDownList ID="MartialData" runat="server"></telerik:RadDropDownList>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="DateOfBirthTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDatePicker ID="DateOfBirthData" runat="server" MinDate="1950.01.01">
                        <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
                    </telerik:RadDatePicker>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="Entrytitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDatePicker ID="EntryData" runat="server" MinDate="1950.01.01">
                        <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
                    </telerik:RadDatePicker>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="LeavingTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadDatePicker ID="LeavingData" runat="server" MinDate="1950.01.01">
                        <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
                    </telerik:RadDatePicker>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="LoginTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="LoginData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="PasswordTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="PasswordData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="EMailTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="EMailData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="PhoneTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="PhoneData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="MobileTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="MobileData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="PhotoTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="PhotoData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="SalutationAddressTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="SalutationAddressData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="SalutationLetterTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="SalutationLetterData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="BeschGradTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadNumericTextBox ID="BeschGradData" runat="server" NumberFormat-DecimalSeparator="."></telerik:RadNumericTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="BerufserfahrungTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadNumericTextBox ID="BerufserfahrungData" runat="server" NumberFormat-DecimalSeparator="."></telerik:RadNumericTextBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="LeaderShipTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadCheckBox ID="LeaderShipData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
                </div>
            </div>
            <div style="display: table-row;" class="dataRow">
                <div style="display: table-cell; " class="titleLabelCell">
                    <asp:Label ID="CommentTitle" runat="server" Text="Label"></asp:Label>
                </div>
                <div style="display: table-cell; " class="dataLabelCell">
                    <telerik:RadTextBox ID="CommentData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine"></telerik:RadTextBox>
                </div>
            </div>
        </div>
    </div>
</div>

