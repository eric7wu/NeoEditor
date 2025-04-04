<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" ValidateRequest="false" Inherits="NeoEditor.EditFile" %>
<HTML lang="<%= PageLang %>" xml:lang="<%= PageLang %>" dir="ltr">
	<HEAD>
		<title><%=m_PageTitle%></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
		<link href='<%=PathToCSS%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorCss%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorDialogCss%>' type=text/css rel=stylesheet>
		<link href='<%=PathToCodemirrorHintCss%>' type=text/css rel=stylesheet>
        <link href='<%=PathToGoogleFontLinkIcon1%>' type=text/css rel=stylesheet>
		<link href='<%=PathToGoogleFontLinkIcon2%>' type=text/css rel=stylesheet>
		<script src="<%=PathTojQuery%>"></script>
		<script src="<%=PathTojQueryUi%>"></script>
        <script src='<%=PathToShowdownJs%>'></script>
		<script src='<%=PathToCodemirrorJs%>'></script>
        <script src="<%=PathToCodemirrorContinuelistJs%>"></script>
		<script src="<%=PathToCodemirrorHtmlmixedJs%>"></script>
        <script src="<%=PathToCodemirrorHtmlembeddedJs%>"></script>
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
        <script src="<%=PathToCodemirrorClikeJs%>"></script>
        <script src="<%=PathToCodemirrorPythonJs%>"></script>
        <script src="<%=PathToCodemirrorPhpJs%>"></script>
        <script src="<%=PathToCodemirrorPerlJs%>"></script>
        <script src="<%=PathToCodemirrorMdJs%>"></script>
        <script src="<%=PathToCodemirrorCssJs%>"></script>
        <script src='<%=PathToCodemirrorCssHintJs%>'></script>
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
                                        <label class="Icon Icon_Browse_Toggle">i<div id="btnBrowse" runat="server" class="Btn_Primary">Browse
                                            <asp:FileUpload id="uplTheFile" runat="server" CssClass="Btn_Hidden" onchange="$('#ddlFiles').val('0');$('#btnOpen').click();"></asp:FileUpload>
                                        </div></label>&nbsp;&nbsp;
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
                                            <label class="Icon Icon_Cancel">i<INPUT id="btnClose" class="Btn" type="button" size="20" value="Close Window" runat="server"></label>&nbsp;&nbsp;&nbsp;
										</div>
                                        <INPUT type="hidden" id="hdnFileName" runat="server">
									    <INPUT type="hidden" id="hdnFileType" runat="server">
                                        <INPUT type="hidden" id="hdnOwnLock" runat="server">
									</TD>
								</TR>
                                <TR>
                                    <TD>
                                        <label class="Icon Icon_Close">i<asp:button id="btnCloseFile" CssClass="Btn" Text="Close File" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Download">i<asp:button id="btnDownloadFile" CssClass="Btn" Text="Download File" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Delete">i<asp:button id="btnDeleteFile" CssClass="Btn" Text="Delete File" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Undo">i<asp:button id="btnUndo" CssClass="Btn" Text="Undo" runat="server"></asp:button></label>&nbsp;
										<label class="Icon Icon_Redo">i<asp:button id="btnRedo" CssClass="Btn" Text="Redo" runat="server"></asp:button></label>&nbsp;
                                        <label class="Icon Icon_Clear">i<asp:button id="btnClear" CssClass="Btn" Text="Clear Change History" runat="server"></asp:button></label>&nbsp;
										<label class="Icon Icon_Print">i<INPUT id="btnPrint" class="Btn" type="button" value="Print" runat="server"></label>&nbsp;
                                        <label class="Icon Icon_Exec">i<INPUT id="btnExec" class="Btn" type="button" value="Run JavaScript code snippet" runat="server"></label>&nbsp;
                                        <label class="Icon Icon_Display">i<INPUT id="btnView" class="Btn" type="button" value="View Markdown File" runat="server"></label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
							        <asp:label id="lblFilePath" CssClass="TitleText" runat="server"></asp:label>&nbsp;&nbsp;&nbsp;
                                    <img id="imgLock" title="File is locked for editing by another user" class="HelpImage" src="images/lock.png" name="imgLock" width="18" height="18" border="0" runat="server">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							        <asp:label id="lblChangeCount" CssClass="TitleText" runat="server"></asp:label>
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
				            <hr width="100%" align="center">
				            <div class="FooterText"><span>Copyright (c) 2024-2025 Eric Wu Powered by </span>
                                <a href="https://github.com/codemirror" target="_blank"><img title="CodeMirror" class="HelpImage" src="images/codemirror.png" width="16" height="16" border="0"></a>
                                <a href="https://github.com/google/diff-match-patch" target="_blank"><img title="google-diff-match-patch" class="HelpImage" src="images/github.png" width="16" height="16" border="0"></a>
				                <a href="https://aistudio.google.com/" target="_blank"><img title="Google Gemini" class="HelpImage" src="images/gemini.png" width="16" height="16" border="0"></a>
                            </div>
                        </TD>
                    </TR>
			    </TBODY>
			</TABLE>
            <div id="divInstructions" class="FloatingTip" runat="server" name="divInstructions">
                <asp:Literal ID="ltInstructions" runat="server">
                    <dl>
                        <dt>Ctrl-F / Cmd-F</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Start searching</dd>
                        <dt>Ctrl-G / Cmd-G</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Find next</dd>
                        <dt>Shift-Ctrl-G / Shift-Cmd-G</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Find previous</dd>
                        <dt>Shift-Ctrl-F / Cmd-Option-F</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Replace</dd>
                        <dt>Shift-Ctrl-R / Shift-Cmd-Option-F</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Replace all</dd>
                        <dt>Ctrl-Z / Cmd-Z</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Undo</dd>
                        <dt>Ctrl-Y / Shift-Cmd-Z</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Redo</dd>
                        <dt>Ctrl-Space</dt><dd>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Auto complete</dd>
                    </dl>
                </asp:Literal>
            </div>
            <div id="divRunResults" class="FloatingConsole" runat="server" name="divRunResults">
                <p>Console Output:</p>
                <iframe id="ifRunResults" class="FloatingConsoleFrame" name="ifRunResults">
                    <!DOCTYPE html>
                    <html>
                        <head>
                        </head>
                        <body>
                        </body>
                    </html>
                </iframe>
            </div>
            <div id="divViewFile" class="FloatingView" runat="server" name="divViewFile">
            </div>
		</form>
        <button class="chatbot-toggler">
          <span class="material-symbols-rounded">chat_apps_script</span>
          <span class="material-symbols-outlined">close</span>
        </button>
        <div class="chatbot">
          <header>
            <h2>Your Coding Guru</h2>
            <span class="close-btn material-symbols-outlined">close</span>
          </header>
          <ul class="chatbox">
            <li class="chat incoming">
              <span class="material-symbols-outlined">smart_toy</span>
              <p>Welcome to NeoEditor 👋<br />What brought you here today?</p>
            </li>
          </ul>
          <div class="chat-input">
            <textarea placeholder="Write a reply..." spellcheck="false" required></textarea>
            <span id="send-btn" class="material-symbols-rounded">send</span>
          </div>
        </div>
<script src="<%= PathToPageScript %>" defer></script>
	</body>
</HTML>
