<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" ValidateRequest="false" Inherits="NeoEditor.EditFile" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML lang="<%= PageLang %>" xml:lang="<%= PageLang %>">
	<HEAD>
		<title><%=m_PageTitle%></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href='<%=PathToCSS%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorCss%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorDialogCss%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorHintCss%>' type=text/css rel=stylesheet>
		<script src="<%=PathTojQuery%>"></script>
		<script src="<%=PathTojQueryUi%>"></script>
		<script src='<%=PathToCodemirrorJs%>'></script>
		<script src="<%=PathToCodemirrorContinuelistJs%>"></script>
		<script src="<%=PathToCodemirrorXmlJs%>"></script>
		<script src="<%=PathToCodemirrorJsJs%>"></script>
		<script src='<%=PathToCodemirrorHintJs%>'></script>
		<script src='<%=PathToCodemirrorXmlHintJs%>'></script>
		<script src='<%=PathToCodemirrorJsHintJs%>'></script>
		<script src='<%=PathToCodemirrorDialogJs%>'></script>
		<script src='<%=PathToCodemirrorSearchcursorJs%>'></script>
		<script src='<%=PathToCodemirrorSearchJs%>'></script>
		<script src='<%=PathToCodemirrorHtmlHintJs%>'></script>
		<script src='<%=PathToCodemirrorAnywordHintJs%>'></script>
		<style>.CodeMirror { border: 1px solid #000; }</style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table2" cellSpacing="1" cellPadding="1" width="100%" border="0">
				<TBODY>
					<tr>
						<td>
							<TABLE cellSpacing="1" cellPadding="1" width="100%" border="0">
								<TR>
									<TD style="padding-bottom: 18px;">
									    <asp:FileUpload id="uplTheFile" runat="server" CssClass="Btn" onchange="$('#btnOpen').click();"></asp:FileUpload>
                                        <asp:label id="lblFiles" AssociatedControlId="ddlFiles" runat="server"></asp:label>
                                        <asp:dropdownlist id="ddlFiles" runat="server" CssClass="WebControl"></asp:dropdownlist>
									    <label class="Icon Icon_Upload_Toggle">i<asp:button id="btnOpen" runat="server" CssClass="Btn_Primary" Text="Open"></asp:button></label>&nbsp; 
										<asp:checkbox id="chkEditInline" runat="server" CssClass="WebControl"></asp:checkbox>
							            <asp:label id="lblEditInline" AssociatedControlId="chkEditInline" runat="server"></asp:label>&nbsp;
										<label class="Icon Icon_Save_Toggle">i<asp:button id="btnSave" CssClass="Btn_Primary" Text="Save" runat="server"></asp:button></label>&nbsp;
										<asp:checkbox id="chkSaveAndClose" runat="server" CssClass="WebControl"></asp:checkbox>
							            <asp:label id="lblSaveAndClose" AssociatedControlId="chkSaveAndClose" runat="server"></asp:label>&nbsp;&nbsp;&nbsp;
                                        <div style="float:right;">
                                            <label class="Icon Icon_Refresh">i<INPUT id="btnRefresh" class="Btn" type="button" size="20" value="Refresh" runat="server"></label>&nbsp;
                                            <label class="Icon Icon_Cancel">i<INPUT id="btnClose" class="Btn" type="button" size="20" value="Close Window" runat="server"></label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										</div>
                                        <INPUT type="hidden" id="hdnFileName" runat="server">
									    <INPUT type="hidden" id="hdnFileType" runat="server">
                                        <INPUT type="hidden" id="hdnOwnLock" runat="server">
									</TD>
								</TR>
                                <TR>
                                    <TD>
                                        <label class="Icon Icon_Close">i<asp:button id="btnCloseFile" CssClass="Btn" Text="Close File" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Undo">i<asp:button id="btnUndo" CssClass="Btn" Text="Undo" runat="server"></asp:button></label>&nbsp;
										<label class="Icon Icon_Redo">i<asp:button id="btnRedo" CssClass="Btn" Text="Redo" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Clear">i<asp:button id="btnClear" CssClass="Btn" Text="Clear Change History" runat="server"></asp:button></label>&nbsp;
										<label class="Icon Icon_Print">i<INPUT id="btnPrint" class="Btn" type="button" value="Print" runat="server"></label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <img id="imgHelp" title="Click for Help" class="HelpImage" src="images/help.jpg" name="imgHelp" width="18" height="18" border="0">
                                    </TD>
                                </TR>
							</TABLE>
						</td>
					</tr>
					<TR>
						<TD><asp:label id="lblError" runat="server" CssClass="ErrorText" Visible="False"></asp:label></TD>
					</TR>
					<TR>
						<TD>
							<TABLE id="Table1" cellSpacing="1" cellPadding="1" border="0" width="100%">
							    <tr>
							        <td>
							        <asp:label id="lblFileName" runat="server"></asp:label>&nbsp;
							        <asp:label id="lblFilePath" runat="server"></asp:label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							        <asp:label id="lblChangeCount" runat="server"></asp:label>
							        </td>
                                    <td></td>
							    </tr>
								<tr>
									<td><div id="divEditFile" name="divEditFile" runat="server" style="WIDTH:100%">
											<asp:textbox id="txtFileContent" runat="server" CssClass="WebControl" TextMode="MultiLine" Height="700" Width="100%"></asp:textbox>
										</div>
                                    </td>
								</tr>
							</TABLE>
						</TD>
					</TR>
                    <TR>
                        <TD>
				            <hr width="60%" align="center">
				            <div class="FooterText"><span>Copyright (c) 2024-2025</span></div>
                        </TD>
                    </TR>
			    </TBODY>
			</TABLE>
            <div id="divInstructions" class="FloatingTip" runat="server" name="divInstructions">
                <asp:Literal ID="ltInstructions" runat="server">
                    <dl>
                        <dt>Ctrl-F / Cmd-F</dt><dd>Start searching</dd>
                        <dt>Ctrl-G / Cmd-G</dt><dd>Find next</dd>
                        <dt>Shift-Ctrl-G / Shift-Cmd-G</dt><dd>Find previous</dd>
                        <dt>Shift-Ctrl-F / Cmd-Option-F</dt><dd>Replace</dd>
                        <dt>Shift-Ctrl-R / Shift-Cmd-Option-F</dt><dd>Replace all</dd>
                        <dt>Ctrl-Z / Cmd-Z</dt><dd>Undo</dd>
                        <dt>Ctrl-Y / Shift-Cmd-Z</dt><dd>Redo</dd>
                        <dt>Ctrl-Space</dt><dd>Auto complete</dd>
                    </dl>
                </asp:Literal>
            </div>
		</form>
<script src="<%= PathToPageScript %>"></script>
	</body>
</HTML>
