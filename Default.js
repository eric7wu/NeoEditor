var ft = getelembyid('hdnFileType').value;
var mdt;
switch(ft) {
    case '.C':
    case '.CC':
    case '.CPP':
    case '.CS':
    case '.JAVA':
    case '.M':
    case '.H':
        mdt = 'clike';
        break;
    case '.PY':
        mdt = 'python';
        break;
    case '.PHP':
        mdt = 'php';
        break;
    case '.PL':
        mdt = 'perl';
        break;
    case '.XML':
    case '.XSLT':
        mdt = 'xml';
        break;
    case '.JS':
    case '.TS':
    case '.JSON':
        mdt = 'javascript';
        break;
    case '.CSS':
        mdt = 'css';
        break;
    case '.HTML':
    case '.HTM':
        mdt = 'htmlmixed';
        break;
    case '.ASPX':
        mdt = 'htmlembedded';
        break;
    case '.MD':
        mdt = 'markdown';
        break;
    default:
        mdt = null;
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
            CodeMirror.showHint(cm, CodeMirror.hint.html);
            break;
        case '.JS':
            CodeMirror.showHint(cm, CodeMirror.hint.javascript);
            break;
        case '.CSS':
            CodeMirror.showHint(cm, CodeMirror.hint.css);
            break;
        default:
            CodeMirror.showHint(cm, CodeMirror.hint.anyword);
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
$("#imgHelp").click(function() { ShowHideDiv(this,'divInstructions'); });
$("#divInstructions").click(function() { this.style.visibility='hidden'; });
$("#divRunResults").click(function() { this.style.visibility='hidden'; });
$("#divViewFile").click(function() { this.style.visibility='hidden'; });
$("#btnOpen").click(function() { return validate(this.form); });
$("#btnCloseFile").click(function() { this.form.target="_self"; return true; });
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
$("#btnBrowse").click(function() {
    $("#uplTheFile").click();
});
$("#btnDeleteFile").click(function() {
    var delFileName = $("#ddlFiles").val();
    if(delFileName == '0')
        return ShowMsg("ddlFiles","No File selected");
    else
        return confirm("Are you sure you want to delete this file? [" + delFileName + "]");
});
$("#btnExec").click(function() {
    let ct = getelembyid('hdnFileType').value;
    if(ct == '.JS')
    {
        let jsCode = document.getElementById('txtFileContent').value;
        jsCode = jsCode.replaceAll('console.log','document.write(\'<\/p><p>\');document.write');
        jsCode = jsCode.replace('<\/p>','');
        let htmlCode = '<script>' + jsCode + '<\/script><\/p>';
        $("#ifRunResults").attr('srcdoc', htmlCode);
        ShowHideDiv(this,'divRunResults');
        return false;
    }
    else
        return ShowMsg("ddlFiles","Invalid Code");
});
$("#ifRunResults").on("load", function() {
    $(this).contents().on("click", function() {
        $("#divRunResults").click();
    });
});
$("#btnView").click(function() {
    let ct = getelembyid('hdnFileType').value;
    if(ct == '.MD')
    {
        let mdCode = document.getElementById('txtFileContent').value;
        let converter = new showdown.Converter();
        $("#divViewFile").html(converter.makeHtml(mdCode));
        ShowHideDiv(this,'divViewFile');
        getelembyid('divViewFile').style.left = 2;
        return false;
    }
    else
        return ShowMsg("ddlFiles","Invalid File");
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
function ShowHideDiv( SourceCtl, DivID )
{
    var div = document.getElementById( DivID );
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

// Chat Bot starts here
const chatbotToggler = document.querySelector(".chatbot-toggler");
const closeBtn = document.querySelector(".close-btn");
const chatbox = document.querySelector(".chatbox");
const chatInput = document.querySelector(".chat-input textarea");
const sendChatBtn = document.querySelector(".chat-input span");

let userMessage = null; // Variable to store user's message
const inputInitHeight = chatInput.scrollHeight;

// API configuration
const API_KEY = "XXXXX"; // Your API key here
const API_URL = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${API_KEY}`;

const createChatLi = (message, className) => {
    // Create a chat <li> element with passed message and className
    const chatLi = document.createElement("li");
    chatLi.classList.add("chat", `${className}`);
    let chatContent = className === "outgoing" ? `<p></p>` : `<span class="material-symbols-outlined">smart_toy</span><p></p>`;
    chatLi.innerHTML = chatContent;
    chatLi.querySelector("p").textContent = message;
    return chatLi; // return chat <li> element
};

const generateResponse = async (chatElement) => {
    const messageElement = chatElement.querySelector("p");

    // Define the properties and message for the API request
    const requestOptions = {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            contents: [
              {
                  role: "user",
                  parts: [{ text: userMessage }],
              },
            ],
        }),
    };

    // Send POST request to API, get response and set the reponse as paragraph text
    try {
        const response = await fetch(API_URL, requestOptions);
        const data = await response.json();
        if (!response.ok) throw new Error(data.error.message);

        // Get the API response text and update the message element
        messageElement.textContent = data.candidates[0].content.parts[0].text.replace(/\*\*(.*?)\*\*/g, "$1");
    } catch (error) {
        // Handle error
        messageElement.classList.add("error");
        messageElement.textContent = error.message;
    } finally {
        chatbox.scrollTo(0, chatbox.scrollHeight);
    }
};

const handleChat = () => {
    userMessage = chatInput.value.trim(); // Get user entered message and remove extra whitespace
    if (!userMessage) return;

    // Clear the input textarea and set its height to default
    chatInput.value = "";
    chatInput.style.height = `${inputInitHeight}px`;

    // Append the user's message to the chatbox
    chatbox.appendChild(createChatLi(userMessage, "outgoing"));
    chatbox.scrollTo(0, chatbox.scrollHeight);

    setTimeout(() => {
        // Display "Thinking..." message while waiting for the response
        const incomingChatLi = createChatLi("Thinking...", "incoming");
    chatbox.appendChild(incomingChatLi);
    chatbox.scrollTo(0, chatbox.scrollHeight);
    generateResponse(incomingChatLi);
    }, 600);
};

chatInput.addEventListener("input", () => {
    // Adjust the height of the input textarea based on its content
    chatInput.style.height = `${inputInitHeight}px`;
    chatInput.style.height = `${chatInput.scrollHeight}px`;
});

chatInput.addEventListener("keydown", (e) => {
    // If Enter key is pressed without Shift key and the window
    // width is greater than 800px, handle the chat
    if (e.key === "Enter" && !e.shiftKey && window.innerWidth > 800) {
        e.preventDefault();
        handleChat();
    }
});

sendChatBtn.addEventListener("click", handleChat);
closeBtn.addEventListener("click", () => document.body.classList.remove("show-chatbot"));
chatbotToggler.addEventListener("click", () => document.body.classList.toggle("show-chatbot"));
