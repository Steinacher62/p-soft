<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SuggestionEvaluationMatrixCtrl.ascx.cs" Inherits="ch.appl.psoft.Suggestion.Controls.SuggestionEvaluationMatrixCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import namespace="System.Drawing" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<script type="text/javascript" language="javascript">
    // Move an element directly on top of another element (and optionally
    // make it the same size)
    function Cover(bottom, top, ignoreSize) {
        var location = Sys.UI.DomElement.getLocation(bottom);
        top.style.position = 'absolute';
        top.style.top = location.y + 'px';
        top.style.left = location.x + 'px';
        if (!ignoreSize) {
            top.style.height = bottom.offsetHeight + 'px';
            top.style.width = bottom.offsetWidth + 'px';
        }
    }
    
    /*
    function MoveRandom(behaviour, div) {
        var animation = behaviour._onMouseOver._animation._animations[1];
        animation._horizontal = getRandom();
        animation._vertical = getRandom();
    }
    
    function getRandom() {
		var i = Math.random();
		var multi = 1;
		if(i < 0.5)
		    multi = -1;
	    return (Math.floor(Math.random()*40)+10)*multi;
    }
    */
</script>
<DIV class="ListVariable"><br/> 
    <table>
        <tr>
            <td>
            <asp:Label ID="suggestiondescription" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                 <div id="legendDIV" style="display: none; overflow: hidden; z-index: 2; border: solid 1px #D0D0D0;"></div>
                 <asp:LinkButton ID="btnInfo" runat="server" OnClientClick="return false;" OnLoad='btnInfo_OnLoad'/>
                 <div id="legendContentDIV" style="display: none; z-index: 2; font-size: 12px; border: solid 1px #CCCCCC; padding: 5px;">
                     <asp:GridView ID="legend" runat="server" DataSourceID="LegendItems" OnDataBound='legend_OnDataBound' AutoGenerateColumns="False" CellPadding="0" CellSpacing="2" BorderWidth="0" HeaderStyle-Height="20px">
                        <Columns>
                            <asp:TemplateField HeaderText="color" SortExpression="color" HeaderStyle-CssClass="ListHeader" ItemStyle-CssClass="ListOdd">
                                <ItemTemplate>
                                    <asp:Panel ID="colorPanel" runat="server" BackColor='<%# Color.FromArgb((int) Eval("color")) %>' Width="100%" Height="100%" CssClass="colorPanel"></asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="title" SortExpression="title" HeaderStyle-CssClass="ListHeader" ItemStyle-CssClass="ListOdd">
                                <ItemTemplate>
                                    <asp:Label ID="titleLabel" runat="server" Text='<%# Eval("title") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div style="clear:both; width: 150px;">
                    <div id="btnCloseParent" style=" display: inline; float: left;">
                    <asp:LinkButton id="btnClose" runat="server" OnClientClick="return false;" Text="Schliessen" />
                    </div>
                    </div>
                     <asp:LinqDataSource ID="LegendItems" runat="server" ContextTypeName="SEEK_LINQ.Suggestion.SuggestionDataContext"
                         OrderBy="ordnumber, title" Select="new (title, color)" TableName="Status" 
                         AutoGenerateWhereClause="true">
                     </asp:LinqDataSource>
                </div>
                 <ajaxToolkit:AnimationExtender id="OpenAnimation" runat="server" TargetControlID="btnInfo">
                    <Animations>
                        <OnClick>
                            <Sequence>
                                <EnableAction Enabled="true" AnimationTarget="btnClose" />
                                
                                <EnableAction Enabled="false" />
                                
                                <ScriptAction Script="Cover($get('legendDIV'), $get('legendContentDIV'), false);" />
                                
                                <StyleAction AnimationTarget="legendContentDIV" Attribute="display" Value="block"/>
                                <FadeIn  AnimationTarget="legendContentDIV" Duration=".2" />
                                
                                <Parallel AnimationTarget="legendContentDIV" Duration=".2">
                                
                                    <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#000000" />
                                </Parallel>
                                
                            </Sequence>
                        </OnClick>
                    </Animations>
                </ajaxToolkit:AnimationExtender>
                <ajaxToolkit:AnimationExtender id="CloseAnimation" runat="server" TargetControlID="btnClose" BehaviorID="CloseBehaviour" >
                    <Animations>
                        <OnClick>
                            <Sequence>
                                <EnableAction Enabled="false" />
                                
                                <FadeOut  AnimationTarget="legendContentDIV" Duration=".2" />
                                <StyleAction AnimationTarget="legendContentDIV" Attribute="display" Value="none"/>
                                <EnableAction AnimationTarget="btnInfo" Enabled="true" />
                            </Sequence>
                        </OnClick>
                    </Animations>
                </ajaxToolkit:AnimationExtender>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td><asp:table id="matrixTable" CellSpacing="0" CellPadding="2" runat="server" BorderWidth="0"></asp:table></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:ImageButton id="excelButton" runat="server"></asp:ImageButton>
            </td>
            <td></td>
        </tr>
    </table>
</DIV>