using System;
using System.Web.Configuration;

namespace NeoEditor
{
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	public static class Config
	{
        private const string CONNECTION_STRING = "ConnectionString";
        private const string FILES_DIR = "Files_Dir";
        private const string PATH_TO_JQUERY = "PATH_TO_JQUERY";
        private const string PATH_TO_JQUERY_UI = "PATH_TO_JQUERY_UI";
        private const string PATH_TO_CODEMIRROR_JS = "PATH_TO_CODEMIRROR_JS";
        private const string PATH_TO_CODEMIRROR_CONTINUELIST_JS = "PATH_TO_CODEMIRROR_CONTINUELIST_JS";
        private const string PATH_TO_CODEMIRROR_HTMLMIXED_JS = "PATH_TO_CODEMIRROR_HTMLMIXED_JS";
        private const string PATH_TO_CODEMIRROR_HTMLEMBEDDED_JS = "PATH_TO_CODEMIRROR_HTMLEMBEDDED_JS";
        private const string PATH_TO_CODEMIRROR_XML_JS = "PATH_TO_CODEMIRROR_XML_JS";
        private const string PATH_TO_CODEMIRROR_HINT_JS = "PATH_TO_CODEMIRROR_HINT_JS";
        private const string PATH_TO_CODEMIRROR_XML_HINT_JS = "PATH_TO_CODEMIRROR_XML_HINT_JS";
        private const string PATH_TO_CODEMIRROR_DIALOG_JS = "PATH_TO_CODEMIRROR_DIALOG_JS";
        private const string PATH_TO_CODEMIRROR_SEARCHCURSOR_JS = "PATH_TO_CODEMIRROR_SEARCHCURSOR_JS";
        private const string PATH_TO_CODEMIRROR_SEARCH_JS = "PATH_TO_CODEMIRROR_SEARCH_JS";
        private const string PATH_TO_CODEMIRROR_CSS = "PATH_TO_CODEMIRROR_CSS";
        private const string PATH_TO_CODEMIRROR_DIALOG_CSS = "PATH_TO_CODEMIRROR_DIALOG_CSS";
        private const string PATH_TO_CODEMIRROR_HINT_CSS = "PATH_TO_CODEMIRROR_HINT_CSS";
        private const string PATH_TO_CODEMIRROR_JS_JS = "PATH_TO_CODEMIRROR_JS_JS";
        private const string PATH_TO_CODEMIRROR_JS_HINT_JS = "PATH_TO_CODEMIRROR_JS_HINT_JS";
        private const string PATH_TO_CODEMIRROR_HTML_HINT_JS = "PATH_TO_CODEMIRROR_HTML_HINT_JS";
        private const string PATH_TO_CODEMIRROR_ANYWORD_HINT_JS = "PATH_TO_CODEMIRROR_ANYWORD_HINT_JS";
        private const string PATH_TO_CODEMIRROR_CLIKE_JS = "PATH_TO_CODEMIRROR_CLIKE_JS";
        private const string PATH_TO_CODEMIRROR_PYTHON_JS = "PATH_TO_CODEMIRROR_PYTHON_JS";
        private const string PATH_TO_CODEMIRROR_PHP_JS = "PATH_TO_CODEMIRROR_PHP_JS";
        private const string PATH_TO_CODEMIRROR_PERL_JS = "PATH_TO_CODEMIRROR_PERL_JS";
        private const string PATH_TO_CODEMIRROR_MD_JS = "PATH_TO_CODEMIRROR_MD_JS";
        private const string PATH_TO_CODEMIRROR_CSS_JS = "PATH_TO_CODEMIRROR_CSS_JS";
        private const string PATH_TO_CODEMIRROR_CSS_HINT_JS = "PATH_TO_CODEMIRROR_CSS_HINT_JS";
        private const string PATH_TO_GOOGLE_FONT_LINK_ICON1 = "PATH_TO_GOOGLE_FONT_LINK_ICON1";
        private const string PATH_TO_GOOGLE_FONT_LINK_ICON2 = "PATH_TO_GOOGLE_FONT_LINK_ICON2";
        private const string PATH_TO_SHOWDOWN_JS = "PATH_TO_SHOWDOWN_JS";

		private static string GetSetting(string IN_sKey) {
			string sResult = System.Web.Configuration.WebConfigurationManager.AppSettings[IN_sKey];
			if (null == sResult) {
				throw new ApplicationException("Application Configuration Key is missing: " + IN_sKey + ". Please contact support");
			}
			return sResult;
		}

        public static string ConnectionString { get { return GetSetting(CONNECTION_STRING); } }
        public static string PathToJquery { get { return GetSetting(PATH_TO_JQUERY); } }
        public static string PathToJqueryUi { get { return GetSetting(PATH_TO_JQUERY_UI); } }
        public static string PathToCodemirrorJs     { get { return GetSetting(PATH_TO_CODEMIRROR_JS); } }
        public static string PathToCodemirrorContinuelistJs { get { return GetSetting(PATH_TO_CODEMIRROR_CONTINUELIST_JS); } }
        public static string PathToCodemirrorHtmlmixedJs { get { return GetSetting(PATH_TO_CODEMIRROR_HTMLMIXED_JS); } }
        public static string PathToCodemirrorHtmlembeddedJs { get { return GetSetting(PATH_TO_CODEMIRROR_HTMLEMBEDDED_JS); } }
        public static string PathToCodemirrorXmlJs { get { return GetSetting(PATH_TO_CODEMIRROR_XML_JS); } }
        public static string PathToCodemirrorHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_HINT_JS); } }
        public static string PathToCodemirrorXmlHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_XML_HINT_JS); } }
        public static string PathToCodemirrorDialogJs { get { return GetSetting(PATH_TO_CODEMIRROR_DIALOG_JS); } }
        public static string PathToCodemirrorSearchcursorJs { get { return GetSetting(PATH_TO_CODEMIRROR_SEARCHCURSOR_JS); } }
        public static string PathToCodemirrorSearchJs { get { return GetSetting(PATH_TO_CODEMIRROR_SEARCH_JS); } }
        public static string PathToCodemirrorCss    { get { return GetSetting(PATH_TO_CODEMIRROR_CSS); } }
        public static string PathToCodemirrorDialogCss { get { return GetSetting(PATH_TO_CODEMIRROR_DIALOG_CSS); } }
        public static string PathToCodemirrorHintCss    { get { return GetSetting(PATH_TO_CODEMIRROR_HINT_CSS); }}
        public static string PathToCodemirrorJsJs   { get { return GetSetting(PATH_TO_CODEMIRROR_JS_JS); } }
        public static string PathToCodemirrorJsHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_JS_HINT_JS); } }
        public static string PathToCodemirrorHtmlHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_HTML_HINT_JS); } }
        public static string PathToCodemirrorAnywordHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_ANYWORD_HINT_JS); } }
        public static string PathToCodemirrorClikeJs { get { return GetSetting(PATH_TO_CODEMIRROR_CLIKE_JS); } }
        public static string PathToCodemirrorPythonJs { get { return GetSetting(PATH_TO_CODEMIRROR_PYTHON_JS); } }
        public static string PathToCodemirrorPhpJs { get { return GetSetting(PATH_TO_CODEMIRROR_PHP_JS); } }
        public static string PathToCodemirrorPerlJs { get { return GetSetting(PATH_TO_CODEMIRROR_PERL_JS); } }
        public static string PathToCodemirrorMdJs { get { return GetSetting(PATH_TO_CODEMIRROR_MD_JS); } }
        public static string PathToCodemirrorCssJs { get { return GetSetting(PATH_TO_CODEMIRROR_CSS_JS); } }
        public static string PathToCodemirrorCssHintJs { get { return GetSetting(PATH_TO_CODEMIRROR_CSS_HINT_JS); } }
        public static string PathToGoogleFontLinkIcon1 { get { return GetSetting(PATH_TO_GOOGLE_FONT_LINK_ICON1); } }
        public static string PathToGoogleFontLinkIcon2 { get { return GetSetting(PATH_TO_GOOGLE_FONT_LINK_ICON2); } }
        public static string PathToShowdownJs { get { return GetSetting(PATH_TO_SHOWDOWN_JS); } }

		public static string FilesDir
		{ 
			get 
			{ 
				string dir = GetSetting(FILES_DIR);
				if (!dir.EndsWith(@"\")) dir += @"\";
				return dir;
			}
		}
	}

    public static class StringExtensions
    {
        public static bool HasValue(this string str)
        {
            return (str != null && str.Trim().Length > 0);
        }

        public static string SubStr(this string str, int startIndex)
        {
            return SubStr(str, startIndex, int.MaxValue);
        }

        public static string SubStr(this string str, int startIndex, int length)
        {
            if (str == null) return string.Empty;
            int start = Math.Max(Math.Min(startIndex, str.Length - 1), 0);
            int len = Math.Max(Math.Min(length, str.Length - start), 0);
            return str.Substring(start, len);
        }
    }
}
