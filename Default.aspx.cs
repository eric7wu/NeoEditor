using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using DiffMatchPatch;
using System.Collections.Generic;

namespace NeoEditor
{
    /// <summary>
    /// Summary description for EditFile.
    /// </summary>
    public partial class EditFile : System.Web.UI.Page
    {
        public string m_PageTitle;

        protected System.Web.UI.WebControls.Label lblError;
        protected System.Web.UI.WebControls.Label lblFileName;
        protected System.Web.UI.WebControls.Label lblFilePath;
        protected System.Web.UI.WebControls.Label lblChangeCount;
        protected System.Web.UI.WebControls.Button btnSave;
        protected System.Web.UI.WebControls.Button btnUndo;
        protected System.Web.UI.WebControls.Button btnRedo;
        protected System.Web.UI.WebControls.Button btnCloseFile;
        protected System.Web.UI.WebControls.Button btnDownloadFile;
        protected System.Web.UI.WebControls.Button btnClear;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divEditFile;
        protected System.Web.UI.WebControls.TextBox txtFileContent;
        protected System.Web.UI.WebControls.Literal ltInstructions;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnPrint;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnRefresh;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnClose;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnFileName;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnFileType;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnOwnLock;
        protected System.Web.UI.WebControls.FileUpload uplTheFile;
        protected System.Web.UI.WebControls.Button btnOpen;
        protected System.Web.UI.WebControls.CheckBox chkEditInline;
        protected System.Web.UI.WebControls.Label lblEditInline;
        protected System.Web.UI.WebControls.CheckBox chkSaveAndClose;
        protected System.Web.UI.WebControls.Label lblSaveAndClose;
        protected System.Web.UI.WebControls.Label lblFiles;
        protected System.Web.UI.WebControls.DropDownList ddlFiles;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divInstructions;

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCloseFile.Click += new System.EventHandler(this.btnCloseFile_Click);
            this.btnDownloadFile.Click += new System.EventHandler(this.btnDownloadFile_Click);
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Localize();
                divInstructions.Attributes["style"] = "visibility:hidden";

                Data.FilePatches.CreateTable(ViewState_RootDir);
                Data.FileLocks.CreateTable(ViewState_RootDir);

                string sFileName;
                ddlFiles.Items.Add(new ListItem("[None]", "0"));

                foreach (string sFilePath in Directory.GetFiles(Server.MapPath(Config.FilesDir), "*.*", SearchOption.TopDirectoryOnly))
                {
                    sFileName = Path.GetFileName(sFilePath);
                    ddlFiles.Items.Add(new ListItem(sFileName, sFileName));
                }

                ddlFiles.SelectedValue = "0";
            }
        }

        private void DisplayFileContents()
        {
            string sFileName = string.Empty;
            if ((uplTheFile.PostedFile != null) && (uplTheFile.PostedFile.ContentLength > 0))
            {
                sFileName = System.IO.Path.GetFileName(uplTheFile.PostedFile.FileName);
            }

            if (ddlFiles.SelectedValue != "0")
            {
                sFileName = ddlFiles.SelectedValue;
                if (Data.FileLocks.Check(ViewState_RootDir, sFileName))
                {
                    ShowErrorMessage(ViewState_FileLocked);
                    return;
                }
                else
                {
                    hdnOwnLock.Value = "1";
                    Data.FileLocks.Create(ViewState_RootDir, sFileName);
                }
            }
            else
            {
                if (!File.Exists(Server.MapPath(Config.FilesDir) + sFileName))
                {
                    hdnOwnLock.Value = "1";
                    Data.FileLocks.Create(ViewState_RootDir, sFileName);
                }
                else
                {
                    if (Data.FileLocks.Check(ViewState_RootDir, sFileName))
                    {
                        ShowErrorMessage(ViewState_FileLocked);
                        return;
                    }
                    else
                    {
                        hdnOwnLock.Value = "1";
                        Data.FileLocks.Create(ViewState_RootDir, sFileName);
                    }
                }
            }

            if (!sFileName.HasValue())
            {
                ShowErrorMessage(ViewState_InvalidFile);
                return;
            }

            string sFileType = Path.GetExtension(sFileName).ToUpper();
            hdnFileType.Value = sFileType;

            lblFilePath.Text = sFileName;
            hdnFileName.Value = sFileName;

            sFileName = Server.MapPath(Config.FilesDir) + sFileName;
            if (ddlFiles.SelectedValue != "0" && !File.Exists(sFileName))
            {
                ShowErrorMessage(string.Format(ViewState_FileNotExist, Server.HtmlEncode(sFileName)));
                return;
            }

            try
            {
                string sFileContent;
                if (ddlFiles.SelectedValue != "0")
                {
                    sFileContent = System.IO.File.ReadAllText(sFileName);
                }
                else
                {
                    using (StreamReader inputStreamReader = new StreamReader(uplTheFile.PostedFile.InputStream))
                    {
                        sFileContent = inputStreamReader.ReadToEnd();
                    }
                }
                txtFileContent.Text = sFileContent;
                ViewState_FileContent = sFileContent;
                ViewState_Original_FileContent = sFileContent;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, null);
                if (filepatch == null)
                {
                    ViewState_VersionNo = 0;
                    ViewState_UndoPatch = null;
                    ViewState_RedoPatch = null;
                }
                else
                {
                    ViewState_VersionNo = filepatch.VersionNo;
                    ViewState_UndoPatch = filepatch.BackwardPatch;
                    ViewState_RedoPatch = filepatch.ForwardPatch;
                }
                lblChangeCount.Text = "Version " + (ViewState_VersionNo + 1).ToString() + " out of " + (ViewState_VersionNo + 1).ToString();
            }
            catch (System.Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return;
            }
        }

        private void ShowErrorMessage(string errMsg)
        {
            if (errMsg.HasValue())
            {
                ddlFiles.SelectedValue = "0";
                txtFileContent.Text = string.Empty;
                if (errMsg != ViewState_FileLocked) Data.FileLocks.Delete(ViewState_RootDir, lblFilePath.Text);
                lblFilePath.Text = string.Empty;
                hdnFileName.Value = string.Empty;
                lblChangeCount.Text = string.Empty;
                lblError.Text = errMsg;
                lblError.Visible = true;
                hdnOwnLock.Value = "0";
            }
        }

        private void btnOpen_Click(object sender, System.EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;

            DisplayFileContents();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;
            lblChangeCount.Text = string.Empty;
            string sFileName = hdnFileName.Value;
            if (!sFileName.HasValue())
            {
                ShowErrorMessage(ViewState_InvalidFileToSave);
                return;
            }

            if (hdnOwnLock.Value == "0" && Data.FileLocks.Check(ViewState_RootDir, sFileName))
            {
                ShowErrorMessage(ViewState_FileLocked);
                return;
            }

            string sErrMsg = null;
            sFileName = Server.MapPath(Config.FilesDir) + sFileName;

            try
            {
                File.WriteAllText(sFileName, txtFileContent.Text);
                
                diff_match_patch dmp = new diff_match_patch();
                dmp.Match_Distance = 1000;
                dmp.Match_Threshold = 0.5f;
                dmp.Patch_DeleteThreshold = 0.5f;
                List<Patch> patches = dmp.patch_make(txtFileContent.Text, ViewState_Original_FileContent);
                string strPatch = dmp.patch_toText(patches);
                if (strPatch.HasValue())
                {
                    ViewState_VersionNo = Data.FilePatches.Create(ViewState_RootDir, sFileName, strPatch);
                    patches = dmp.patch_make(ViewState_Original_FileContent, txtFileContent.Text);
                    strPatch = dmp.patch_toText(patches);
                    Data.FilePatches.Update(ViewState_RootDir, sFileName, ViewState_VersionNo - 1, strPatch);
                }
            }
            catch (System.Exception ex)
            {
                sErrMsg = ex.Message;
            }

            if (sErrMsg.HasValue())
            {
                ShowErrorMessage(sErrMsg);
            }
            else
            {
                ddlFiles.Items.Clear();
                ddlFiles.Items.Add(new ListItem("[None]", "0"));

                foreach (string sFilePath in Directory.GetFiles(Server.MapPath(Config.FilesDir), "*.*", SearchOption.TopDirectoryOnly))
                {
                    sFileName = Path.GetFileName(sFilePath);
                    ddlFiles.Items.Add(new ListItem(sFileName, sFileName));
                }

                ddlFiles.SelectedValue = hdnFileName.Value;

                if (chkSaveAndClose.Checked)
                {
                    ddlFiles.SelectedValue = "0";
                    txtFileContent.Text = string.Empty;
                    Data.FileLocks.Delete(ViewState_RootDir, lblFilePath.Text);
                    lblFilePath.Text = string.Empty;
                    hdnFileName.Value = string.Empty;
                    lblChangeCount.Text = string.Empty;
                    hdnOwnLock.Value = "0";
                }
                else
                {
                    ViewState_FileContent = txtFileContent.Text;
                    ViewState_Original_FileContent = txtFileContent.Text;
                    sFileName = Server.MapPath(Config.FilesDir) + hdnFileName.Value;
                    Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, null);
                    if (filepatch == null)
                    {
                        ViewState_VersionNo = 0;
                        ViewState_UndoPatch = null;
                        ViewState_RedoPatch = null;
                    }
                    else
                    {
                        ViewState_VersionNo = filepatch.VersionNo;
                        ViewState_UndoPatch = filepatch.BackwardPatch;
                        ViewState_RedoPatch = filepatch.ForwardPatch;
                    }
                    if (lblFilePath.Text != hdnFileName.Value)
                    {
                        hdnOwnLock.Value = "1";
                        Data.FileLocks.Create(ViewState_RootDir, hdnFileName.Value);
                    }
                    lblFilePath.Text = hdnFileName.Value;
                    lblChangeCount.Text = "Version " + (ViewState_VersionNo + 1).ToString() + " out of " + (ViewState_VersionNo + 1).ToString();
                }
            }
        }

        private void btnCloseFile_Click(object sender, System.EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;
            lblChangeCount.Text = string.Empty;
            ddlFiles.SelectedValue = "0";
            txtFileContent.Text = string.Empty;
            Data.FileLocks.Delete(ViewState_RootDir, lblFilePath.Text);
            lblFilePath.Text = string.Empty;
            hdnFileName.Value = string.Empty;
            hdnOwnLock.Value = "0";
        }

        private void btnDownloadFile_Click(object sender, System.EventArgs e)
        {
            string sFileName = lblFilePath.Text;

            if (sFileName.HasValue())
            {
                try
                {
                    string contentText = txtFileContent.Text;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = "text/plain";
                    Response.AddHeader("Content-disposition", "attachment;filename=" + sFileName);
                    Response.Write(contentText);
                    Response.Flush();
                    Response.End();
                }
                catch (System.Threading.ThreadAbortException ex) { }
                catch (System.Exception ex) { }
            }
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            string sFileName = hdnFileName.Value;
            sFileName = Server.MapPath(Config.FilesDir) + sFileName;

            try
            {
                Data.FilePatches.Delete(ViewState_RootDir, sFileName);
            }
            catch (System.Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            string sFileName = Server.MapPath(Config.FilesDir) + lblFilePath.Text;

            if (ViewState_VersionNo > 0)
            {
                diff_match_patch dmp = new diff_match_patch();
                dmp.Match_Distance = 1000;
                dmp.Match_Threshold = 0.5f;
                dmp.Patch_DeleteThreshold = 0.5f;

                List<Patch> patches = dmp.patch_fromText(ViewState_UndoPatch);
                Object[] results = dmp.patch_apply(patches, ViewState_FileContent);
                txtFileContent.Text = results[0].ToString();

                ViewState_VersionNo--;
                ViewState_FileContent = txtFileContent.Text;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, ViewState_VersionNo);
                if (filepatch == null)
                {
                    ViewState_VersionNo = 0;
                    ViewState_UndoPatch = null;
                    ViewState_RedoPatch = null;
                }
                else
                {
                    ViewState_VersionNo = filepatch.VersionNo;
                    ViewState_UndoPatch = filepatch.BackwardPatch;
                    ViewState_RedoPatch = filepatch.ForwardPatch;
                }
                filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, null);
                lblChangeCount.Text = "Version " + (ViewState_VersionNo + 1).ToString() + " out of " + (filepatch.VersionNo + 1).ToString();
            }
        }

        private void btnRedo_Click(object sender, System.EventArgs e)
        {
            string sFileName = Server.MapPath(Config.FilesDir) + lblFilePath.Text;

            if (ViewState_VersionNo >= 0 && ViewState_RedoPatch.HasValue())
            {
                diff_match_patch dmp = new diff_match_patch();
                dmp.Match_Distance = 1000;
                dmp.Match_Threshold = 0.5f;
                dmp.Patch_DeleteThreshold = 0.5f;

                List<Patch> patches = dmp.patch_fromText(ViewState_RedoPatch);
                Object[] results = dmp.patch_apply(patches, ViewState_FileContent);
                txtFileContent.Text = results[0].ToString();

                ViewState_VersionNo++;
                ViewState_FileContent = txtFileContent.Text;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, ViewState_VersionNo);
                ViewState_VersionNo = filepatch.VersionNo;
                ViewState_UndoPatch = filepatch.BackwardPatch;
                ViewState_RedoPatch = filepatch.ForwardPatch;

                filepatch = Data.FilePatches.Get(ViewState_RootDir, sFileName, null);
                lblChangeCount.Text = "Version " + (ViewState_VersionNo + 1).ToString() + " out of " + (filepatch.VersionNo + 1).ToString();
            }
        }

        private void Localize()
        {
            m_PageTitle = "Edit File";
            ViewState_Save = "Save";
            ViewState_CloseFile = "Close File";
            ViewState_DownloadFile = "Download File";
            ViewState_Clear = "Clear Change History";
            ViewState_Undo = "Undo";
            ViewState_Redo = "Redo";

            btnOpen.Text = "Open";
            btnPrint.Value = "Print";
            btnRefresh.Value = "Refresh";
            btnClose.Value = "Close Window";
            btnSave.Text = ViewState_Save;
            btnCloseFile.Text = ViewState_CloseFile;
            btnDownloadFile.Text = ViewState_DownloadFile;
            btnClear.Text = ViewState_Clear;
            btnUndo.Text = ViewState_Undo;
            btnRedo.Text = ViewState_Redo;

            lblFiles.Text = "or select a file";
            lblFileName.Text = "File Name:";
            lblEditInline.Text = "Edit in the current window";
            lblSaveAndClose.Text = "Save and Close";
            ViewState_InvalidFile = "Please specify a file to edit.";
            ViewState_InvalidFileType = "Please specify an xml, xslt or json file to edit.";
            ViewState_FileNotExist = "File \"{0}\" doesn't exist.";
            ViewState_InvalidFileToSave = "Please specify a file to save.";
            ViewState_FileLocked = "File is locked for editing by another user.";
            ViewState_RootDir = Server.MapPath("~");

            hdnOwnLock.Value = "0";
        }

        private string ViewState_InvalidFile
        {
            get { return (string)ViewState["InvalidFile"]; }
            set { ViewState["InvalidFile"] = value; }
        }

        private string ViewState_InvalidFileToSave
        {
            get { return (string)ViewState["InvalidFileToSave"]; }
            set { ViewState["InvalidFileToSave"] = value; }
        }

        private string ViewState_InvalidFileType
        {
            get { return (string)ViewState["InvalidFileType"]; }
            set { ViewState["InvalidFileType"] = value; }
        }

        private string ViewState_FileNotExist
        {
            get { return (string)ViewState["FileNotExist"]; }
            set { ViewState["FileNotExist"] = value; }
        }

        private string ViewState_FileLocked
        {
            get { return (string)ViewState["FileLocked"]; }
            set { ViewState["FileLocked"] = value; }
        }

        private string ViewState_RootDir
        {
            get { return (string)ViewState["RootDir"]; }
            set { ViewState["RootDir"] = value; }
        }

        private string ViewState_Save
        {
            get { return (string)ViewState["Save"]; }
            set { ViewState["Save"] = value; }
        }

        private string ViewState_CloseFile
        {
            get { return (string)ViewState["CloseFile"]; }
            set { ViewState["CloseFile"] = value; }
        }

        private string ViewState_DownloadFile
        {
            get { return (string)ViewState["DownloadFile"]; }
            set { ViewState["DownloadFile"] = value; }
        }

        private string ViewState_Clear
        {
            get { return (string)ViewState["Clear"]; }
            set { ViewState["Clear"] = value; }
        }

        private string ViewState_Undo
        {
            get { return (string)ViewState["Undo"]; }
            set { ViewState["Undo"] = value; }
        }

        private string ViewState_Redo
        {
            get { return (string)ViewState["Redo"]; }
            set { ViewState["Redo"] = value; }
        }

        private string ViewState_FileContent
        {
            get { return (string)ViewState["FileContent"]; }
            set { ViewState["FileContent"] = value; }
        }

        private string ViewState_Original_FileContent
        {
            get { return (string)ViewState["OriginalFileContent"]; }
            set { ViewState["OriginalFileContent"] = value; }
        }

        private int ViewState_VersionNo
        {
            get { return (int)ViewState["VersionNo"]; }
            set { ViewState["VersionNo"] = value; }
        }

        private string ViewState_UndoPatch
        {
            get { return (string)ViewState["UndoPatch"]; }
            set { ViewState["UndoPatch"] = value; }
        }

        private string ViewState_RedoPatch
        {
            get { return (string)ViewState["RedoPatch"]; }
            set { ViewState["RedoPatch"] = value; }
        }

        public string PathToCSS
        {
            get { return Page.ResolveUrl("~/style.css"); }
        }

        public string PathToCodemirrorCss
        {
            get { return Config.PathToCodemirrorCss; }
        }

        public string PathToCodemirrorDialogCss
        {
            get { return Config.PathToCodemirrorDialogCss; }
        }

        public string PathToCodemirrorHintCss
        {
            get { return Config.PathToCodemirrorHintCss; }
        }

        public string PathToPageScript
        {
            get { return Page.ResolveUrl("~/Default.js"); }
        }

        public string PathToCodemirrorJs
        {
            get { return Config.PathToCodemirrorJs; }
        }

        public string PathToCodemirrorContinuelistJs
        {
            get { return Config.PathToCodemirrorContinuelistJs; }
        }

        public string PathToCodemirrorXmlJs
        {
            get { return Config.PathToCodemirrorXmlJs; }
        }

        public string PathToCodemirrorJsJs
        {
            get { return Config.PathToCodemirrorJsJs; }
        }

        public string PathToCodemirrorHintJs
        {
            get { return Config.PathToCodemirrorHintJs; }
        }

        public string PathToCodemirrorXmlHintJs
        {
            get { return Config.PathToCodemirrorXmlHintJs; }
        }

        public string PathToCodemirrorJsHintJs
        {
            get { return Config.PathToCodemirrorJsHintJs; }
        }

        public string PathToCodemirrorHtmlHintJs
        {
            get { return Config.PathToCodemirrorHtmlHintJs; }
        }

        public string PathToCodemirrorAnywordHintJs
        {
            get { return Config.PathToCodemirrorAnywordHintJs; }
        }

        public string PathToCodemirrorDialogJs
        {
            get { return Config.PathToCodemirrorDialogJs; }
        }

        public string PathToCodemirrorSearchcursorJs
        {
            get { return Config.PathToCodemirrorSearchcursorJs; }
        }

        public string PathToCodemirrorSearchJs
        {
            get { return Config.PathToCodemirrorSearchJs; }
        }

        public string PathToCodemirrorClikeJs
        {
            get { return Config.PathToCodemirrorClikeJs; }
        }

        public string PathToCodemirrorPythonJs
        {
            get { return Config.PathToCodemirrorPythonJs; }
        }

        public string PathToCodemirrorPhpJs
        {
            get { return Config.PathToCodemirrorPhpJs; }
        }

        public string PathToCodemirrorPerlJs
        {
            get { return Config.PathToCodemirrorPerlJs; }
        }

        public string PathToCodemirrorCssJs
        {
            get { return Config.PathToCodemirrorCssJs; }
        }

        public string PathToCodemirrorCssHintJs
        {
            get { return Config.PathToCodemirrorCssHintJs; }
        }

        public string PathTojQuery
        {
            get { return Config.PathToJquery; }
        }

        public string PathTojQueryUi
        {
            get { return Config.PathToJqueryUi; }
        }

        public string PageLang
        {
            get
            {
                return (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("zh")) ? System.Threading.Thread.CurrentThread.CurrentCulture.Name : System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            }
        }

    }
}
