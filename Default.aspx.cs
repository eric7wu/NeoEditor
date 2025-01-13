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
        public string m_ErrorInvalidFile;
        public string m_ErrorInvalidFileToSave;
        public string m_ErrorInvalidFileType;
        public string m_ErrorFileNotExist;

        protected System.Web.UI.WebControls.Label lblError;
        protected System.Web.UI.WebControls.Label lblFileName;
        protected System.Web.UI.WebControls.Label lblFilePath;
        protected System.Web.UI.WebControls.Label lblChangeCount;
        protected System.Web.UI.WebControls.Button btnSave;
        protected System.Web.UI.WebControls.Button btnUndo;
        protected System.Web.UI.WebControls.Button btnRedo;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divEditFile;
        protected System.Web.UI.WebControls.TextBox txtFileContent;
        protected System.Web.UI.WebControls.Literal ltInstructions;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnPrint;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnClose;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hdnFileType;
        protected System.Web.UI.WebControls.FileUpload uplTheFile;
        protected System.Web.UI.WebControls.Button btnOpen;
        protected System.Web.UI.WebControls.CheckBox chkEditInline;
        protected System.Web.UI.WebControls.Label lblEditInline;

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

                Data.FilePatches.CreateTable();
            }
        }

        private void DisplayFileContents()
        {
            string sFileName;
            if ((uplTheFile.PostedFile != null) && (uplTheFile.PostedFile.ContentLength > 0))
            {
                sFileName = System.IO.Path.GetFileName(uplTheFile.PostedFile.FileName);
            }
            else
            {
                ShowErrorMessage(m_ErrorInvalidFile);
                return;
            }

            string sFileType = Path.GetExtension(sFileName).ToUpper();
            hdnFileType.Value = sFileType;

            try
            {
                string sFileContent;
                using (StreamReader inputStreamReader = new StreamReader(uplTheFile.PostedFile.InputStream))
                {
                    sFileContent = inputStreamReader.ReadToEnd();
                }
                txtFileContent.Text = sFileContent;
                ViewState_FileContent = sFileContent;
                ViewState_Original_FileContent = sFileContent;
                lblFilePath.Text = sFileName;
                sFileName = Config.FilesDir + sFileName;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(sFileName, null);
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
                lblFileName.Visible = false;
                txtFileContent.Visible = false;
                btnPrint.Visible = false;
                btnSave.Visible = false;
                btnUndo.Visible = false;
                btnRedo.Visible = false;
                ltInstructions.Visible = false;
                lblError.Text = errMsg;
                lblError.Visible = true;
            }
        }

        private void btnOpen_Click(object sender, System.EventArgs e)
        {
            DisplayFileContents();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            lblError.Text = string.Empty;
            lblChangeCount.Text = string.Empty;
            string sFileName = lblFilePath.Text;
            if (!sFileName.HasValue())
            {
                ShowErrorMessage(m_ErrorInvalidFileToSave);
                return;
            }

            string sErrMsg = null;
            sFileName = Config.FilesDir + sFileName;

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
                    ViewState_VersionNo = Data.FilePatches.Create(sFileName, strPatch);
                    patches = dmp.patch_make(ViewState_Original_FileContent, txtFileContent.Text);
                    strPatch = dmp.patch_toText(patches);
                    Data.FilePatches.Update(sFileName, ViewState_VersionNo - 1, strPatch);
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
                DisplayFileContents();
            }
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            string sFileName = Config.FilesDir + lblFilePath.Text;

            if (ViewState_VersionNo > 0)
            {
                diff_match_patch dmp = new diff_match_patch();
                dmp.Match_Distance = 1000;
                dmp.Match_Threshold = 0.5f;
                dmp.Patch_DeleteThreshold = 0.5f;

                List<Patch> patches = dmp.patch_fromText(ViewState_UndoPatch);
                Object[] results = dmp.patch_apply(patches, ViewState_FileContent);
                txtFileContent.Text = results[0].ToString();
                bool[] bArray = (bool[])results[1];
                lblChangeCount.Text = "Undo Changes: " + bArray.Length.ToString();

                ViewState_VersionNo--;
                ViewState_FileContent = txtFileContent.Text;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(sFileName, ViewState_VersionNo);
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
            }
            else
            {
                lblChangeCount.Text = "Undo Changes: 0";
            }
        }

        private void btnRedo_Click(object sender, System.EventArgs e)
        {
            string sFileName = Config.FilesDir + lblFilePath.Text;

            if (ViewState_VersionNo >= 0 && ViewState_RedoPatch.HasValue())
            {
                diff_match_patch dmp = new diff_match_patch();
                dmp.Match_Distance = 1000;
                dmp.Match_Threshold = 0.5f;
                dmp.Patch_DeleteThreshold = 0.5f;

                List<Patch> patches = dmp.patch_fromText(ViewState_RedoPatch);
                Object[] results = dmp.patch_apply(patches, ViewState_FileContent);
                txtFileContent.Text = results[0].ToString();
                bool[] bArray = (bool[])results[1];
                lblChangeCount.Text = "Redo Changes: " + bArray.Length.ToString();

                ViewState_VersionNo++;
                ViewState_FileContent = txtFileContent.Text;
                Data.FilePatches.DataItem filepatch = Data.FilePatches.Get(sFileName, ViewState_VersionNo);
                ViewState_VersionNo = filepatch.VersionNo;
                ViewState_UndoPatch = filepatch.BackwardPatch;
                ViewState_RedoPatch = filepatch.ForwardPatch;
            }
            else
            {
                lblChangeCount.Text = "Redo Changes: 0";
            }
        }

        private void Localize()
        {
            m_PageTitle = "Edit File";
            ViewState_Save = "Save";
            ViewState_Undo = "Undo";
            ViewState_Redo = "Redo";

            btnOpen.Text = "Open";
            btnPrint.Value = "Print";
            btnClose.Value = "Close Window";
            btnSave.Text = ViewState_Save;
            btnUndo.Text = ViewState_Undo;
            btnRedo.Text = ViewState_Redo;

            lblFileName.Text = "File Name:";
            lblEditInline.Text = "Edit in the current window";
            m_ErrorInvalidFile = "Please specify a file to edit.";
            m_ErrorInvalidFileType = "Please specify an xml, xslt or json file to edit.";
            m_ErrorFileNotExist = "File \"{0}\" doesn't exist.";
            m_ErrorInvalidFileToSave = "Please specify a file to save.";
        }

        private string ViewState_Save
        {
            get { return (string)ViewState["Save"]; }
            set { ViewState["Save"] = value; }
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
