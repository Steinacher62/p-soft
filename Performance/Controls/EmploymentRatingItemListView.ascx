<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EmploymentRatingItemListView.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.EmploymentRatingItemListView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Table Runat="server" id="Table1" Height="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="100%">
		<asp:TableCell>
			<DIV class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
			</DIV>
            <DIV class="ListVariable">
                <asp:Table ID="tableInterviewDone" runat="server" Visible="false">
                    <asp:TableRow Height="10px">
                    </asp:TableRow>
                    <asp:TableRow>
                         <asp:TableCell>
                              </asp:TableCell>
                        <asp:TableCell>
                             <asp:CheckBox ID="CBInterviewDone" runat="server" AutoPostBack="true"/>
                        </asp:TableCell>
                         </asp:TableRow>
                    </asp:Table> 
                    <asp:Table ID="tableInterviewDate" runat="server" CellPadding ="6" Visible="false">
                    <asp:TableRow>
                         <asp:TableCell >
                                 <asp:Label ID="Label1" runat="server" Text="Gesprächsdatum:"></asp:Label>   
                             </asp:TableCell>
                        <asp:TableCell HorizontalAlign ="Left" >
                            <telerik:RadDatePicker ID="DateInterviewDone" runat="server" MinDate="01.01.0001"></telerik:RadDatePicker>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell >
                                 <asp:Label ID="DateNeed" runat="server" Text="Eingabe erforderlich" Font-Bold="true" ></asp:Label>   
                             </asp:TableCell>
                    </asp:TableRow>
                </asp:Table> 
			</DIV>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>

 