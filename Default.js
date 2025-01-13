var ft = getelembyid('hdnFileType').value;
var mdt;
switch(ft) {
    case '.XML':
        mdt = 'xml';
        break;
    case '.XSLT':
    case '.HTML':
    case '.CSS':
        mdt = 'text/html';
        break;
    default:
        mdt = 'javascript';
}
var WORD = /[\w$]+/, RANGE = 500;
var EXTRAWORDS = ["meta", "header", "lines", "key", "username", "password", "version", "api"];
CodeMirror.registerHelper("hint", "anyword", function(editor, options) {
    var word = options && options.word || WORD;
    var range = options && options.range || RANGE;
    var extraWords = options && options.extraWords || EXTRAWORDS;
    var cur = editor.getCursor(), curLine = editor.getLine(cur.line);
    var end = cur.ch, start = end;
    while (start && word.test(curLine.charAt(start - 1))) --start;
    var curWord = start != end && curLine.slice(start, end);
    var list = options && options.list || [], seen = {};
    var re = new RegExp(word.source, "g");
    for (var dir = -1; dir <= 1; dir += 2) {
        var line = cur.line, endLine = Math.min(Math.max(line + dir * range, editor.firstLine()), editor.lastLine()) + dir;
        for (; line != endLine; line += dir) {
            var text = editor.getLine(line), m;
            while (m = re.exec(text)) {
                if (line == cur.line && m[0] === curWord) continue;
                if ((!curWord || m[0].lastIndexOf(curWord, 0) == 0) && !Object.prototype.hasOwnProperty.call(seen, m[0])) {
                    seen[m[0]] = true;
                    list.push(m[0]);
                }
            }
        }
    }
    list.push(...(extraWords.filter(el => el.startsWith(curWord || ''))));
    return {list: list, from: CodeMirror.Pos(cur.line, start), to: CodeMirror.Pos(cur.line, end)};
});
var dummy = {
    attrs: {
        XmlFieldName: null,
        Type: ["Label", "Text", "Image", "Barcode", "Line", "BGColor", "MathFormula", "TextFormat", "Subtotal", "SplitTotal", 
			   "Total", "Date", "CheckDigit", "PageNumber", "AZTEC", "CODABAR", "CODE_39", "CODE_93", "CODE_128", "DATA_MATRIX", "EAN_8", "EAN_13", 
			   "ITF" , "MAXICODE", "PDF_417", "QR_CODE", "RSS_14" , "RSS_EXPANDED", "UPC_A", "UPC_E", "UPC_EAN_EXTENSION", "MSI", 
			   "All_1D", "PLESSEY"], 
        XPosition: null,
        YPosition: null,
        Width: null,
        Height: null,
        HAlign: ["Left", "Center", "Right"],
        VAlign: ["Bottom", "Middle", "Top"],
        Font: ["Arial", "Courier", "Courier-Bold", "Courier-BoldOblique", "Courier-Oblique", "Helvetica", "Helvetica-Bold", "Helvetica-BoldOblique",
			   "Helvetica-Oblique", "Symbol", "Times", "Times-Roman", "Times-Bold", "Times-BoldItalic", "Times-Italic", "ZapfDingbats"],
        FontFile: null,
        FontSize: null,
        FontWeight: ["Normal", "Bold", "BoldItalic", "Italic"],
        FontColor: ["RGB 255,255,255", "White", "Black", "Red", "Yellow", "Pink", "Orange",
					"Magenta", "Light_Gray", "Green", "Dark_Gray", "Blue", "Cyan"],
        TextWrap: ["True", "False"],
        FixedLeading: null,
        MultipliedLeading: null,
        Occurrance: ["All", "Even", "Odd", "First", "NotFirst"],
        ImageRoation: null,
        Hidden: ["1", "0"],
        ContentType: null
    },
    children: []
};
var labels = {
    Layout: {
        attrs: {
            PageSize: ["Letter", "Legal", "Tabloid", "A1", "A2", "A3", "A4", "A5"],
			PageWidth: null,
			PageHeight: null,
			PageHGutter: null,
			PageVGutter: null,
			TopMargin: null,
			BottomMargin: null,
			LeftMargin: null,
			RightMargin: null,
			LabelWidth: null,
			LabelHeight: null,
			Rows: null,
			Cols: null,
			MaxThumbnailDim: null,
			LogoFile: null,
			LogoRoation: null,
			DefaultBGColor: ["RGB 255,255,255", "White", "Black", "Red", "Yellow", "Pink", "Orange",
							 "Magenta", "Light_Gray", "Green", "Dark_Gray", "Blue", "Cyan"],
			BarcodePadding: null,
			LabelQtyColName: null,
			PageSplitKeyCol: null,
			ReportPrepend: null,
			ReportAppend: null,
			PageSplitAppend: null,
			PageAppend: null
        },
        children: ["Header", "Footer", "Fields"]
    },
    Header: {
        attrs: {
			Height: null,
			Border: null,
			XPosition: null,
			YPosition: null,
			DefaultColumnWidth: null,
			DefaultColumnHeight: null
        },
        children: ["HeaderField"]
    },
    Footer: {
        attrs: {
			Height: null,
			Border: null,
			XPosition: null,
			YPosition: null,
			DefaultColumnWidth: null,
			DefaultColumnHeight: null
        },
        children: ["FooterField"]
    },
    Fields: {
        attrs: {},
        children: ["LabelField"]
    },
    HeaderField: dummy, FooterField: dummy, LabelField: dummy
};
function completeAfter(cm, pred) {
    var cur = cm.getCursor();
    if (!pred || pred()) setTimeout(function() {
        if (!cm.state.completionActive)
            cm.showHint({completeSingle: false});
    }, 100);
    return CodeMirror.Pass;
}
function completeIfAfterLt(cm) {
    return completeAfter(cm, function() {
        var cur = cm.getCursor();
        return cm.getRange(CodeMirror.Pos(cur.line, cur.ch - 1), cur) == "<";
    });
}
function completeIfInTag(cm) {
    return completeAfter(cm, function() {
        var tok = cm.getTokenAt(cm.getCursor());
        if (tok.type == "string" && (!/['"]/.test(tok.string.charAt(tok.string.length - 1)) || tok.string.length == 1)) return false;
        var inner = CodeMirror.innerMode(cm.getMode(), tok.state).state;
        return inner.tagName;
    });
}
if(document.getElementById('txtFileContent')) {
CodeMirror.commands.autocomplete = function(cm) { 
    switch(ft) {
        case '.XML':
            CodeMirror.showHint(cm, CodeMirror.hint.xml);
            break;
        case '.XSLT':
        case '.HTML':
        case '.CSS':
            CodeMirror.showHint(cm, CodeMirror.hint.html);
            break;
        case '.JSON':
            CodeMirror.showHint(cm, CodeMirror.hint.anyword);
            break;
        default:
            CodeMirror.showHint(cm, CodeMirror.hint.javascript);
    }
}
var editor = CodeMirror.fromTextArea(document.getElementById('txtFileContent'), {
    lineNumbers: true,
    lineWrapping: true,
    matchBrackets: true,
    extraKeys: {
        "'<'": completeAfter,
        "'/'": completeIfAfterLt,
        "' '": completeIfInTag,
        "'='": completeIfInTag,
        "Ctrl-Space": "autocomplete"
    },
    mode: {name: mdt, globalVars: true, matchClosing: true, alignCDATA: true}
});
editor.setSize(null, '700') ;
editor.setOption('indentUnit', 4);
editor.setOption('indentWithTabs', true);
if(ft=='.XML') editor.setOption('hintOptions', {schemaInfo: labels});
}
function printFile() {
    nw = window.open('','','location=no,scrollbars=no,menubar=no,toolbar=no,width=800,height=500');
    nw.document.open();
    nw.document.write('<html><head></head><body>');
    nw.document.write('<pre>'+$('<div>').text(document.getElementById('txtFileContent').value).html().replace(/\n/gi,'<br>')+'</pre>');
    nw.document.write('</body></html>');
    nw.print();
    nw.document.close();
    nw.close();
}
$("#btnPrint").click(function() { printFile(); });
$("#btnRefresh").click(function() { window.location.reload(); });
$("#imgHelp").click(function() { ShowHideTip(this,'divInstructions'); });
$("#divInstructions").click(function() { this.style.visibility='hidden'; });
$("#btnOpen").click(function() { return validate(this.form); });
$("#btnClose").click(function() {
    if ($.trim($("#hdnFileName").val()) == "") window.close();
    else alert("You need to close file before closing window");
});
$("#btnSave").click(function() {
    var curFileName = $("#lblFilePath").text();
    fileName = prompt("Save File As", curFileName);
    if (fileName == null) {
        return false;
    } else {
        $("#hdnFileName").val(fileName);
        return true;
    }
});
function ShowMsg(ctlId,errMsg){alert(errMsg);getelembyid(ctlId).focus();return false;}
function getelembyid(i){return document.getElementById(i);}
function validate(theform)
{
	var editfile = getelembyid("uplTheFile");
	var editmode = getelembyid("chkEditInline");
	if(editmode.checked)
		theform.target="_self";
	else
		theform.target="_blank";	
	if(editfile.files.length === 0 && $("#ddlFiles").val() == '0')
		return ShowMsg("uplTheFile","No File selected");
	return true;
}
function ShowHideTip( SourceCtl, TipDivID )
{
    var div = document.getElementById( TipDivID );
    if ((div.style.visibility==null) || (div.style.visibility=="hidden"))
    {
        div.style.visibility = "visible";
    }
    else
    {
        div.style.visibility = "hidden";
    }
    var x = 0, y = 0;
    var obj = SourceCtl;
    while( obj != null )	{
        x += obj.offsetLeft;
        obj = obj.offsetParent;
    }
    obj = SourceCtl;
    while( obj != null )	{
        y += obj.offsetTop;
        obj = obj.offsetParent;
    }
    div.style.left = x;
    div.style.top = y + SourceCtl.offsetHeight;
}
