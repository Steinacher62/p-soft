<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ImportExportControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.ImportExportControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript" id="telerikClientEvents1">
    var selectedItemName;
	function FileExplorer_ItemSelected(sender,args)
	{
	    selectedItemName = args.get_item()._name;
	}

	function ImportClick(sender,args)
	{
	    if (typeof selectedItemName != 'undefined') {
	        __doPostBack("<%= Import.UniqueID %>", selectedItemName);
	    }
	    else {
	        __doPostBack("<%= Import.UniqueID %>", "noFile");
	    }
	}
</script>


<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td height="20"></td>
	</tr>
	<tr>
		<td>
		    <table id="inputExportTab" border="0" CellSpacing="0" CellPadding="2">
                <tr>
                    <td>
                        <telerik:RadFileExplorer ID="FileExplorer" runat="server" DisplayUpFolderItem="True" Language="de-DE" OnClientItemSelected="FileExplorer_ItemSelected" EnableCreateNewFolder="False" Configuration-AllowMultipleSelection="false" Configuration-MaxUploadFileSize="104857600" FilterTextBoxLabel="Filter By" EnableOpenFile="true">
<Configuration SearchPatterns="*.xls, *.xlsx" AllowMultipleSelection="False" MaxUploadFileSize="104857600"></Configuration>
                        </telerik:RadFileExplorer>
                    </td>
                </tr>
		    </table>
		</td>
	</tr>
	
	<tr>
		<td height="10"></td>
	</tr>
    <tr>
        <td height="20" valign="bottom"><asp:Button id="Import" runat="server" CssClass="ButtonL" OnClientClick ="ImportClick()"></asp:Button></td>
    </tr>
	<tr>
		<td>
		    <asp:Table id="ExportButtonTable" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="0">
		    </asp:Table>
		</td>
	</tr>
</TABLE>

<telerik:RadWindowManager ID="ImportExportWindowManager" runat="server" ></telerik:RadWindowManager>
