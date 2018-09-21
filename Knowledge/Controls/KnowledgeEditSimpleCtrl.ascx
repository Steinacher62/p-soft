<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KnowledgeEditSimpleCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgeEditSimple" %>
<telerik:RadWindow ID="RadWindow1" runat="server" VisibleOnPageLoad="false" Modal="true" VisibleStatusbar="false" Title="Wissenselement" Behaviors="Move,Resize" Width="860px" Height="600px">
    <ContentTemplate>
        <div style="display: table; height: 100%; width: 100%">
            <div style="display: table-row;">
                <div style="display: table-cell; padding: 10px;">
                    <asp:TextBox ID="KnowledgeTitle" runat="server" Width="100%"></asp:TextBox>
                    </div>
                </div>
            <div style="display: table-row;">
                <div style="display: table-cell; padding: 10px;">
                    <telerik:RadEditor ID="editor" runat="server" Language="de-DE" EditModes="Design,Preview" ToolbarMode="Default">
                        <Tools>
                            <telerik:EditorToolGroup Tag="MainToolbar">
                                <telerik:EditorTool Name="Print" ShortCut="CTRL+P / CMD+P" />
                                <telerik:EditorTool Name="AjaxSpellCheck" />
                                <telerik:EditorTool Name="FindAndReplace" ShortCut="CTRL+F / CMD+F" />
                                <telerik:EditorTool Name="SelectAll" ShortCut="CTRL+A / CMD+A" />
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="Cut" ShortCut="CTRL+X / CMD+X" />
                                <telerik:EditorTool Name="Copy" ShortCut="CTRL+C / CMD+C" />
                                <telerik:EditorTool Name="Paste" ShortCut="CTRL+V / CMD+V" />
                                <telerik:EditorTool Name="InsertLink" />
                                <telerik:EditorSplitButton Name="Undo">
                                </telerik:EditorSplitButton>
                                <telerik:EditorSplitButton Name="Redo">
                                </telerik:EditorSplitButton>
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorDropDown Name="FormatBlock">
                                </telerik:EditorDropDown>
                                <telerik:EditorDropDown Name="FontName">
                                </telerik:EditorDropDown>
                                <telerik:EditorDropDown Name="RealFontSize">
                                </telerik:EditorDropDown>
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="Bold" ShortCut="CTRL+B / CMD+B" />
                                <telerik:EditorTool Name="Italic" ShortCut="CTRL+I / CMD+I" />
                                <telerik:EditorTool Name="Underline" ShortCut="CTRL+U / CMD+U" />
                                <telerik:EditorTool Name="StrikeThrough" />
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="JustifyLeft" />
                                <telerik:EditorTool Name="JustifyCenter" />
                                <telerik:EditorTool Name="JustifyRight" />
                                <telerik:EditorTool Name="JustifyFull" />
                                <telerik:EditorTool Name="JustifyNone" />
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="Indent" />
                                <telerik:EditorTool Name="Outdent" />
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="InsertOrderedList" />
                                <telerik:EditorTool Name="InsertUnorderedList" />
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="ToggleTableBorder" />
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorSplitButton Name="ForeColor">
                                </telerik:EditorSplitButton>
                                <telerik:EditorSplitButton Name="BackColor">
                                </telerik:EditorSplitButton>
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup Tag="DropdownToolbar">
                                <telerik:EditorSplitButton Name="InsertSymbol">
                                </telerik:EditorSplitButton>
                                <telerik:EditorToolStrip Name="InsertTable">
                                </telerik:EditorToolStrip>
                                <telerik:EditorSeparator />
                                <telerik:EditorTool Name="ConvertToLower" />
                                <telerik:EditorTool Name="ConvertToUpper" />
                                <telerik:EditorSeparator />
                                <telerik:EditorDropDown Name="Zoom">
                                </telerik:EditorDropDown>
                                <telerik:EditorTool Name="ToggleScreenMode" ShortCut="F11 / CMD+F11" />
                            </telerik:EditorToolGroup>
                        </Tools>
                        <TrackChangesSettings CanAcceptTrackChanges="False"></TrackChangesSettings>

                    </telerik:RadEditor>
                </div>
                
            </div>
            <div style="display: table-row; text-align:left;">
                <div style="display: table-cell; padding: 10px;">
                    <telerik:RadButton ID="saveKnowledge" runat="server"  Text="Speichern" OnClientClicked="saveKnowledgeClicked" AutoPostBack="false"></telerik:RadButton>
                    <telerik:RadButton ID="closeKnowledge" runat="server" OnClientClicked="closeKnowledgeClicked" Text="Schliessen" AutoPostBack="false"></telerik:RadButton>
                    <telerik:RadButton ID="deleteKnowledge" runat="server" OnClientClicked="deleteKnowledgeClicked" Text="Wissenselement löschen" AutoPostBack="false"></telerik:RadButton>
                </div>

            </div>

        </div>
    </ContentTemplate>
</telerik:RadWindow>

 


