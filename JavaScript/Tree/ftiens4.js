//To customize the tree, overwrite these variables in the configuration file (demoFramesetNode.js, etc.)
var STARTALLOPEN = 0;
var USEICONS = 1;
var WRAPTEXT = 0;
var PERSERVESTATE = 0;
var PERSERVEHIGHLIGHT = 0;
var ICONPATH = '';
var HIGHLIGHT = 0;
var HIGHLIGHT_DRAGDROP = 0;
var HIGHLIGHT_COLOR = 'white';
var HIGHLIGHT_BG = 'blue';
var BUILDALL = 0;
var ROOT_OPEN_ICON = "RootOpen.gif";
var ROOT_CLOSE_ICON = "RootClosed.gif";
var LAST_ROOT_OPEN_ICON = "LastRootOpen.gif";
var LAST_ROOT_CLOSE_ICON = "LastRootClosed.gif";
var NODE_OPEN_ICON = "BranchOpen.gif";
var NODE_CLOSE_ICON = "BranchClosed.gif";
var LAST_NODE_OPEN_ICON = "LastBranchOpen.gif";
var LAST_NODE_CLOSE_ICON = "LastBranchClosed.gif";
var ITEM_OPEN_ICON = "LeafOpen.gif";
var ITEM_CLOSE_ICON = "LeafClosed.gif";
var LAST_ITEM_OPEN_ICON = "LastLeafOpen.gif";
var LAST_ITEM_CLOSE_ICON = "LastLeafClosed.gif";
var DRAGNODE = 0;
var DROPNODE = 0;
var DRAGITEM = 0;
var DROPITEM = 0;
var TARGET_FRAME = null;
var MOVE = dummyMove;
var COPY = dummyCopy;
var OPENLINK = 1;
var CLOSELINK = 0;
var TREE_DEBUG = 0;
var DOWNLOAD_ENABLE = 0;


//Other variables
var hashTableSize = 113;
var foldersTree = null; // root
var lastClicked = null;
var lastClickedColor;
var lastClickedBgColor;
var indexOfEntries = new Array(hashTableSize);
for (i=0; i < indexOfEntries; i++) { indexOfEntries[i] = null; }
var nEntries = 0;
var lastEntry = null;
var browserVersion = 0;
var selectedFolder = 0;
var lastOpenedFolder = null;
var t = 5;
var doc = document;
var supportsDeferral = false;
var cookieCutter = '^'; //You can change this if you need to use ^ in your xID or treeID values
var LAST_NODE_LINE = "LastNode.gif";
var MINUS_LAST_NODE_LINE = "MinusLastNode.gif"
var PLUS_LAST_NODE_LINE = "PlusLastNode.gif";
var NODE_LINE = "Node.gif";
var MINUS_NODE_LINE = "MinusNode.gif";
var PLUS_NODE_LINE = "PlusNode.gif";
 
// Definition of class Folder 
// ***************************************************************** 
function Folder(folderDescription, hreference, toolTip) //constructor 
{ 
  //constant data 
  this.desc = folderDescription && folderDescription != "" ? folderDescription : null;
  this.hreference = hreference && hreference != "" ? hreference : null;
  this.toolTip = toolTip && toolTip != "" ? toolTip : null;
  this.id = -1;
  this.navObj = 0;
  this.iconImg = 0; 
  this.nodeImg = 0;
  this.iconOpen = ICONPATH + NODE_OPEN_ICON;
  this.iconClosed = ICONPATH + NODE_CLOSE_ICON;
  this.iconLastOpen = ICONPATH + LAST_NODE_OPEN_ICON;
  this.iconLastClosed = ICONPATH + LAST_NODE_CLOSE_ICON;
  this.children = new Array;
  this.nChildren = 0;
  this.level = 0;
  this.leftSideCoded = "";
  this.isLastNode=false;
  this.parentObj = null;
  this.maySelect=true;
  this.prependHTML = "";
  this.dragEnable = DRAGNODE != 0;
  this.dropEnable = DROPNODE != 0;
  this.cssClass = 0;
  this.nextEntry = null;
  this.isFolder = true;
  this.openLink = OPENLINK != 0;
  this.closeLink = CLOSELINK != 0;
  this.initialOpen = false;
  this.downloadChild = DOWNLOAD_ENABLE;

  //dynamic data 
  this.isOpen = false;
  this.isLastOpenedFolder = false;
  this.isLastClickedFolder = false;
  this.isRendered = 0;
  this.dropLocked = false;
 
  //methods 
  this.initialize = initializeFolder;
  this.setState = setStateFolder;
  this.addChild = addChild;
  this.createIndex = createEntryIndex;
  this.escondeBlock = escondeBlock;
  this.esconde = escondeFolder;
  this.folderMstr = folderMstr;
  this.renderOb = drawFolder;
  this.totalHeight = totalHeight;
  this.subEntries = folderSubEntries;
  this.linkHTML = linkFolderHTML;
  this.blockStartHTML = blockStartHTML;
  this.blockEndHTML = blockEndHTML;
  this.nodeImageSrc = nodeImageSrc;
  this.iconImageSrc = iconImageSrc;
  this.getID = getID;
  this.forceOpeningOfAncestorFolders = forceOpeningOfAncestorFolders;
  this.setInitialOpen = setInitialFolderOpen;
} 
function setInitialFolderOpen(parent) {
    if (!parent) this.initialOpen = true;
    if (this.parentObj) this.parentObj.setInitialOpen(false);
}
function initializeFolder(level, lastNode, leftSide, childOnly) 
{ 
  var j=0;
  var i=0;
  nc = this.nChildren;
   
  if (!childOnly) {
	if (this.id == -1) this.createIndex(); 
	this.level = level;
	this.leftSideCoded = leftSide;
	this.isLastNode = lastNode;
  }
  this.isOpen = browserVersion == 0 || STARTALLOPEN==1;

  this.isLastOpenedFolder = false;
  
  if (level>0) {
    if (lastNode) leftSide = leftSide + "0";
	else leftSide = leftSide + "1";
  }
 
  if (nc > 0) 
  { 
    level = level + 1;
    for (i=0 ; i < this.nChildren; i++)  
    { 
      if (i == this.nChildren-1) 
        this.children[i].initialize(level, 1, leftSide, false);
      else 
        this.children[i].initialize(level, 0, leftSide, false);
    } 
  } 
  if (!childOnly) {
	if (this.navObj) this.navObj.outerHTML = "";
	this.isRendered = 0;
  }
} 
 
function drawFolder(insertAtObj) 
{ 
  var nodeName = "";
  var auxEv = "";
  var docW = "";
  
  if (this.nChildren == 0 && this.downloadChild) {
    this.downloadChild = false;
    downloadTree(this);
    this.initialize(this.level,this.isLastNode,this.leftSideCoded,true);
  }
  var leftSide = leftSideHTML(this.leftSideCoded);

  if (browserVersion > 0) 
    auxEv = "<a href='javascript:clickOnNode(\""+this.getID()+"\")'>";
  else 
    auxEv = "<a>";

  nodeName = this.nodeImageSrc();
 
  if (this.level>0) 
    if (this.isLastNode) //the last child in the children array 
	    //leftSide = leftSide + "<td valign=top>" + auxEv + "<img name='nodeIcon" + this.id + "' id='nodeIcon" + this.id + "' src='" + nodeName + "' width=16 height=22 border=0></a></td>"
	    leftSide = leftSide + auxEv + "<img align='absmiddle' name='nodeIcon" + this.id + "' id='nodeIcon" + this.id + "' src='" + nodeName + "' width=16 border=0></a>";
    else 
      //leftSide = leftSide + "<td valign=top background=" + ICONPATH + "VertLine.gif>" + auxEv + "<img name='nodeIcon" + this.id + "' id='nodeIcon" + this.id + "' src='" + nodeName + "' width=16 height=22 border=0></a></td>"
      leftSide = leftSide + auxEv + "<img align='absmiddle' name='nodeIcon" + this.id + "' id='nodeIcon" + this.id + "' src='" + nodeName + "' width=16 border=0></a>";

  this.isRendered = 1;

  if (browserVersion == 2) { 
    if (!doc.yPos) 
      doc.yPos=20;
  } 

  docW = this.blockStartHTML("folder");

  //docW = docW + "<tr>" + leftSide + "<td valign=top>";
  docW = docW + leftSide;
  if (USEICONS)
  {
    //docW = docW + this.linkHTML(false) 
    docW = docW + "<img align='absmiddle' id='foldIcon" + this.getID() + "' name='foldIcon" + this.getID() + "' src='" + this.iconImageSrc() + "' border=0";
    if (browserVersion == 1 && navigator.version >= 5) {
		if (this.dragEnable) docW = docW + " ondragend=\"dragEnd()\" ondragstart=\"dragStart()\"";
		if (this.dropEnable) docW = docW + " ondrop=\"drop()\" ondragenter=\"dragEnter()\" ondragleave=\"dragLeave()\" ondragover=\"dragOver()\"";
    }
    if (browserVersion > 0) {
	    docW = docW + " onClick='javascript:clickOnFolder(\""+this.getID()+"\")'";
    }
    //docW = docW + "></a>";
    docW = docW + ">";

  }
  else if (this.prependHTML == "")
    docW = docW + "<img align='absmiddle' src=" + ICONPATH + "Blank.gif height=2 width=2>";
  /*
  if (WRAPTEXT)
	  docW = docW + "</td>"+this.prependHTML+"<td valign=middle width=100%>"
  else
	  docW = docW + "</td>"+this.prependHTML+"<td valign=middle nowrap width=100%>"
  */
  docW = docW + this.prependHTML;
  if (this.hreference || this.desc) {
     docW = docW + this.linkHTML(true) + this.desc + "</a>";
  }
  else this.maySelect = false;
  //docW = docW + "</td>"

  docW = docW + this.blockEndHTML();

  if (insertAtObj == null)
  {
	  if (supportsDeferral) {
		  insertAtObj = getElById("domRoot_"+foldersTree.getID());
		  if (insertAtObj == null) {
		    doc.write("<div id=domRoot_"+foldersTree.getID()+"></div>"); //transition between regular flow HTML, and node-insert DOM DHTML
		    if (TREE_DEBUG) alert("drawFolder="+"<div id=domRoot_"+foldersTree.getID()+"></div>");
		  }
		  insertAtObj = getElById("domRoot_"+foldersTree.getID());
		  insertAtObj.insertAdjacentHTML("beforeEnd", docW);
		  if (TREE_DEBUG) alert("drawFolder: insert before="+docW);
	  }
	  else {
		  doc.write(docW);
		  if (TREE_DEBUG) alert("drawFolder="+docW);
	  }

  }
  else
  {
      insertAtObj.insertAdjacentHTML("afterEnd", docW);
	  if (TREE_DEBUG) alert("drawFolder: insert after="+docW);
  }
 
  if (browserVersion == 2) 
  { 
    this.navObj = doc.layers["folder"+this.id];
    if (USEICONS)
      this.iconImg = this.navObj.document.images["foldIcon"+this.getID()];
    this.nodeImg = this.navObj.document.images["nodeIcon"+this.id];
    doc.yPos=doc.yPos+this.navObj.clip.height;
  } 
  else if (browserVersion != 0)
  { 
    this.navObj = getElById("folder"+this.id);
    //alert("drawFolder: navObj = "+this.navObj.outerHTML)
    if (USEICONS)
      this.iconImg = getElById("foldIcon"+this.getID());
    this.nodeImg = getElById("nodeIcon"+this.id);
  } 
} 
 
function setStateFolder(isOpen) 
{ 
  var subEntries;
  var totalHeight;
  var fIt = 0;
  var i=0;
  var currentOpen;
 
  if (isOpen == this.isOpen) 
    return;
 
  if (browserVersion == 2)  
  { 
    totalHeight = 0;
    for (i=0; i < this.nChildren; i++)
      totalHeight = totalHeight + this.children[i].navObj.clip.height;
      subEntries = this.subEntries();
    if (this.isOpen) 
      totalHeight = 0 - totalHeight;
    // todo
    //for (fIt = this.id + subEntries + 1; fIt < nEntries; fIt++) 
      //indexOfEntries[fIt].navObj.moveBy(0, totalHeight) 
  }  
  this.isOpen = isOpen;

  if (this.getID()!=foldersTree.getID() && PERSERVESTATE && !this.isOpen) //closing
  {
     currentOpen = GetCookie("clickedFolder");
     if (currentOpen != null) {
         currentOpen = currentOpen.replace(this.getID()+cookieCutter, "");
         SetCookie("clickedFolder", currentOpen);
     }
  }
	
  if (!this.isOpen && this.isLastOpenedfolder)
  {
		lastOpenedFolder = null;
		this.isLastOpenedfolder = false;
  }
  propagateChangesInState(this);
} 
 
function propagateChangesInState(folder) 
{   
  var i=0;
  //Change icon
  if (folder.nChildren > 0 && folder.level>0)  //otherwise the one given at render stays
    folder.nodeImg.src = folder.nodeImageSrc();

  //Change node
  if (USEICONS)
    folder.iconImg.src = folder.iconImageSrc();

  //Propagate changes
  for (i=folder.nChildren-1; i>=0; i--) {
 	if (folder.isOpen)
	    folder.children[i].folderMstr(folder.navObj);
	else
	    folder.children[i].esconde();
  }
} 
 
function escondeFolder() 
{ 
  this.escondeBlock();
  this.setState(0);
} 
 
function linkFolderHTML(isTextLink) 
{ 
  var docW = "<a ";
  if (this.cssClass) docW = docW + "class = '" + this.cssClass + "' ";
  if (this.toolTip) docW = docW + "title = '" + this.toolTip + "' ";

  if (this.hreference) {
	  docW = docW + "href='#' TARGET=_self ";
  }
  if (isTextLink) docW += "id=\"itemTextLink"+this.id+"\" ";
  
  if (browserVersion > 0) {
	if (isTextLink) docW = docW + "onClick='onHighlightFolderLink(\""+this.getID()+"\");'";
	else docW = docW + "onClick='clickOnFolder(\""+this.getID()+"\");'";
  }
  docW = docW + ">";

  return docW;
} 
 
function addChild(childNode) 
{ 
  this.children[this.nChildren] = childNode;
  childNode.parentObj = this;
  this.nChildren++;
  return childNode;
} 
 
function folderSubEntries() 
{ 
  var i = 0;
  var se = this.nChildren;
 
  for (i=0; i < this.nChildren; i++){ 
    if (this.children[i].children) //is a folder 
      se = se + this.children[i].subEntries();
  } 
 
  return se;
} 

function nodeImageSrc() {
  var srcStr = "";

  if (this.isLastNode) //the last child in the children array 
  { 
    if (this.nChildren == 0)
      srcStr = ICONPATH + LAST_NODE_LINE;
    else
      if (this.isOpen)
        srcStr = ICONPATH + MINUS_LAST_NODE_LINE;
      else
        srcStr = ICONPATH + PLUS_LAST_NODE_LINE;
  } 
  else 
  { 
    if (this.nChildren == 0)
      srcStr = ICONPATH + NODE_LINE;
    else
      if (this.isOpen)
        srcStr = ICONPATH + MINUS_NODE_LINE;
      else
        srcStr = ICONPATH + PLUS_NODE_LINE;
  }   
  return srcStr;
}

function iconImageSrc() {
  var srcStr = "";
  if (this.isLastClickedFolder)
  {
    if (this.isOpen)
        srcStr = this.iconOpen;
    else
        srcStr = this.iconLastOpen;
  }
  else
  {
    if (this.isOpen)
        srcStr = this.iconClosed;
    else
        srcStr = this.iconLastClosed;
  }
  return srcStr;
} 
// Definition of class Item (a document or link inside a Folder) 
// ************************************************************* 
 
function Item(itemDescription, itemLink, target, toolTip) // Constructor 
{ 
  // constant data 
  this.desc = itemDescription && itemDescription != "" ? itemDescription : null;
  this.link = itemLink && itemLink != "" ? itemLink : null;
  this.toolTip = toolTip && toolTip != "" ? toolTip : null;
  this.id = -1; //initialized in initalize() 
  this.navObj = 0; //initialized in render() 
  this.iconImg = 0; //initialized in render() 
  this.iconOpen = ICONPATH + ITEM_OPEN_ICON;
  this.iconClosed = ICONPATH + ITEM_CLOSE_ICON;
  this.iconLastOpen = ICONPATH + LAST_ITEM_OPEN_ICON;
  this.iconLastClosed = ICONPATH + LAST_ITEM_CLOSE_ICON;
  this.isRendered = 0;
  this.isLastNode = false;
  this.isOpen = false;
  this.level = 0;
  this.leftSideCoded = "";
  this.nChildren = 0;
  this.target = target;
  this.parentObj = null;
  this.maySelect=true;
  this.prependHTML = "";
  this.dragEnable = DRAGITEM != 0;
  this.dropEnable = DROPITEM != 0;
  this.cssClass = 0;
  this.nextEntry = null;
  this.isFolder = false;
  this.dropLocked = false;
  // methods 
  this.initialize = initializeItem;
  this.createIndex = createEntryIndex;
  this.escondeBlock = escondeBlock;
  this.esconde = escondeBlock;
  this.folderMstr = folderMstr;
  this.renderOb = drawItem;
  this.totalHeight = totalHeight;
  this.blockStartHTML = blockStartHTML;
  this.blockEndHTML = blockEndHTML;
  this.iconImageSrc = iconImageSrc;
  this.getID = getID;
  this.forceOpeningOfAncestorFolders = forceOpeningOfAncestorFolders;
  this.setInitialOpen = setInitialItemOpen;
} 
function setInitialItemOpen(parent) {
    if (this.parentObj) this.parentObj.setInitialOpen(false);
}
 
function initializeItem(level, lastNode, leftSide, childOnly) 
{  
  if (this.id == -1) this.createIndex();
  this.level = level;
  this.leftSideCoded = leftSide;
  this.isLastNode = lastNode;
  if (this.navObj) this.navObj.outerHTML = "";
  this.isRendered = 0;
} 
 
function drawItem(insertAtObj) 
{ 
  var leftSide = leftSideHTML(this.leftSideCoded);
  var docW = "";
  var fullLink = null;
  
  if (this.link) fullLink = "href=\""+substComment(this.link,'"')+"\" target=\""+this.target+"\" onClick=\"clickOnLink('"+this.getID()+"','"+transferComment(this.link,"'")+"','"+this.target+"');return false;\"";
  
  this.isRendered = 1;

  if (this.level>0) {
    if (this.isLastNode) //the last 'brother' in the children array 
    { 
      //leftSide = leftSide + "<td valign=top><img src='" + ICONPATH + LAST_NODE_LIN + "' width=16 height=22></td>"
      leftSide = leftSide + "<img align='absmiddle' src='" + ICONPATH + LAST_NODE_LINE + "' width=16 >";
    } 
    else 
    { 
      //leftSide = leftSide + "<td valign=top background=" + ICONPATH + "VertLine.gif><img src='" + ICONPATH + NODE_LINE + "' width=16 height=22></td>"
      leftSide = leftSide + "<img align='absmiddle' src='" + ICONPATH + NODE_LINE + "' width=16 >";
    } 
  }
  docW = docW + this.blockStartHTML("item");

  //docW = docW + "<tr>" + leftSide + "<td valign=top>"
  docW = docW + leftSide;
  if (USEICONS) {
      docW = docW + "<img align='absmiddle' id='itemIcon"+this.getID()+"' " + "src='"+this.iconImageSrc()+"' border=0";
      if (browserVersion == 1 && navigator.version >= 5) {
		if (this.dragEnable) docW = docW + " ondragend=\"dragEnd()\" ondragstart=\"dragStart()\"";
		if (this.dropEnable) docW = docW + " ondrop=\"drop()\" ondragenter=\"dragEnter()\" ondragleave=\"dragLeave()\" ondragover=\"dragOver()\"";
      }
      docW = docW +  ">";
  }
  else if (this.prependHTML == "")
    docW = docW + "<img align='absmiddle' src='" + ICONPATH + "Blank.gif' height=2 width=3>";

  //if (WRAPTEXT)
    //docW = docW + "</td>"+this.prependHTML+"<td valign=middle width=100%>"
    //docW = docW + this.prependHTML;
  //else
    //docW = docW + "</td>"+this.prependHTML+"<td valign=middle nowrap width=100%>"
  docW = docW + this.prependHTML;

  if (fullLink || this.desc) {
    docW = docW + "<a ";
    if (this.toolTip) docW = docW + "title='" + this.toolTip + "' ";
    if (this.cssClass) docW = docW + "class = '" + this.cssClass + "' ";
    docW = docW + " id=\"itemTextLink"+this.id+"\" ";
    if (fullLink) docW = docW + fullLink;
    docW = docW + ">" + this.desc + "</a>";
  }
  else this.maySelect = false;
  //docW = docW + "</td>"

  docW = docW + this.blockEndHTML();
 
  if (insertAtObj == null)
  {
	  doc.write(docW);
	  if (TREE_DEBUG) alert("drawItem="+docW);
  }
  else
  {
      insertAtObj.insertAdjacentHTML("afterEnd", docW);
	  if (TREE_DEBUG) alert("drawItem: insert after="+docW);
  }

  if (browserVersion == 2) { 
    this.navObj = doc.layers["item"+this.id]; 
    if (USEICONS)
      this.iconImg = this.navObj.document.images["itemIcon"+this.getID()]; 
    doc.yPos=doc.yPos+this.navObj.clip.height; 
  } else if (browserVersion != 0) { 
    this.navObj = getElById("item"+this.id);
    if (USEICONS)
      this.iconImg = getElById("itemIcon"+this.getID());
  } 
} 
// ' -> \"; " -> \'
function substComment(str,comment) {
    var strElements = str.split(comment);
    var result = strElements[0];
    var subst = (comment == "'" ? '\"' : "\'");
    for (i=1; i<strElements.length; i++) {
        result = result + subst + strElements[i];
    }
    return result;
}
// ' -> \'; " -> \"
function transferComment(str,comment) {
    var strElements = str.split(comment);
    var result = strElements[0];
    for (i=1; i<strElements.length; i++) {
        result = result + "\\" + comment + strElements[i];
    }
    return result;
}
// Methods common to both objects (pseudo-inheritance) 
// ******************************************************** 
 
function forceOpeningOfAncestorFolders() {
  if (this.parentObj == null || this.parentObj.isOpen)
    return;
  else {
    this.parentObj.forceOpeningOfAncestorFolders();
    clickOnNodeObj(this.parentObj);
  }
}

function escondeBlock() 
{ 
  if (browserVersion == 1 || browserVersion == 3) { 
    if (this.navObj.style.display == "none") 
      return;
    this.navObj.style.display = "none";
  } else { 
    if (this.navObj.visibility == "hidden") 
      return;
    this.navObj.visibility = "hidden";
  }     
} 
 
function folderMstr(domObj) 
{ 
  /*
  if (browserVersion == 1 || browserVersion == 3) { 
    if (t==-1)
      return
    var str = new String(doc.links[t])
    if (str.slice(36,38) != "rh")
      return
  }
  */
  if (!this.isRendered)
     this.renderOb(domObj);
  else
    if (browserVersion == 1 || browserVersion == 3) 
      this.navObj.style.display = "block";
    else 
      this.navObj.visibility = "show";
} 

function blockStartHTML(idprefix) {
  var idParam = "id='" + idprefix + this.id + "'";
  var docW = "";

  if (browserVersion == 2) 
    docW = "<layer "+ idParam + " top=" + doc.yPos + " visibility=show";
  else if (browserVersion != 0)
    docW = "<div " + idParam + " style='display:block; position:block;'";
  //docW = docW + "<table border=0 cellspacing=0 cellpadding=0 width=100% >"
  if (!WRAPTEXT) docW = docW + " nowrap >";
  else docW = docW + " >";
  return docW;
}

function blockEndHTML() {
  var docW = "";

  //docW = "</table>"
   
  if (browserVersion == 2) 
    docW = docW + "</layer>";
  else if (browserVersion != 0)
    docW = docW + "</div>";

  return docW;
}
 
function createEntryIndex() 
{ 
  this.id = nEntries;
  insertObj(this); 
  nEntries++;
} 
 
// total height of subEntries open 
function totalHeight() //used with browserVersion == 2 
{ 
  var h = this.navObj.clip.height;
  var i = 0;
   
  if (this.isOpen) //is a folder and _is_ open 
    for (i=0 ; i < this.nChildren; i++)  
      h = h + this.children[i].totalHeight();
 
  return h 
} 


function leftSideHTML(leftSideCoded) {
	var i;
	var retStr = "";

	for (i=0; i<leftSideCoded.length; i++)
	{
		if (leftSideCoded.charAt(i) == "1")
		{
			//retStr = retStr + "<td valign=top background=" + ICONPATH + "VertLine.gif><img src='" + ICONPATH + "VertLine.gif' width=16 height=22></td>"
			retStr = retStr + "<img align='absmiddle' src='" + ICONPATH + "VertLine.gif' width=16 >";
		}
		if (leftSideCoded.charAt(i) == "0")
		{
			//retStr = retStr + "<td valign=top><img src='" + ICONPATH + "Blank.gif' width=16 height=22></td>"
			retStr = retStr + "<img align='absmiddle' src='" + ICONPATH + "Blank.gif' width=16 >";
		}
	}
	return retStr;
}

function getID()
{
  //define a .xID in all nodes (folders and items) if you want to PERVESTATE that
  //work when the tree changes. The value eXternal value must be unique for each
  //node and must node change when other nodes are added or removed
  //The value may be numeric or string, but cannot have the same char used in cookieCutter
  var id = externalID(this);
  return id ? id : this.id;
}
function externalID(obj) {
  if (typeof obj.xID != "undefined") return obj.xID;
  else return null;
}
 
// Events 
// ********************************************************* 
 
function clickOnFolder(folderId) 
{ 
    var clicked = findObj(folderId);

    //if (!clicked.isOpen) {
      clickOnNodeObj(clicked);
    //}

    if (lastOpenedFolder != null && lastOpenedFolder != folderId)
      clickOnNode(lastOpenedFolder); //sets lastOpenedFolder to null

    if (clicked.nChildren==0) {
      lastOpenedFolder = folderId;
      clicked.isLastOpenedfolder = true;
    }

    if (clicked && isLinked(clicked.hreference)) {
        highlightTextLink(clicked);
        if ((clicked.isOpen && clicked.openLink) || (!clicked.isOpen && clicked.closeLink))
        {
            if (clicked.hreference.toLowerCase().indexOf('javascript:') == 0 && (!TARGET_FRAME || (TARGET_FRAME && TARGET_FRAME == "_self")))
                window.execScript(clicked.hreference);
            else
                window.open(clicked.hreference,TARGET_FRAME);
        }
    }
} 
function onHighlightFolderLink(folderId) 
{ 
    var clicked = findObj(folderId);

    if (clicked && isLinked(clicked.hreference)) {
        highlightTextLink(clicked);
        if ((clicked.isOpen && clicked.openLink) || (!clicked.isOpen && clicked.closeLink))
        {
            if (clicked.hreference.toLowerCase().indexOf('javascript:') == 0 && (!TARGET_FRAME || (TARGET_FRAME && TARGET_FRAME == "_self")))
                window.execScript(clicked.hreference);
            else
                window.open(clicked.hreference,TARGET_FRAME);
        }
    }
} 
 
function clickOnNode(folderId) 
{ 
  clickOnNodeObj(findObj(folderId));
}

function clickOnNodeObj(folderObj) 
{ 
  var state = 0;
  var currentOpen;
 
  state = folderObj.isOpen;
  folderObj.setState(!state); //open<->close  

  if (folderObj.id!=foldersTree.id && PERSERVESTATE)
  {
    currentOpen = GetCookie("clickedFolder");
    if (currentOpen == null)
      currentOpen = "";

    if (!folderObj.isOpen) //closing
    {
      currentOpen = currentOpen.replace(folderObj.getID()+cookieCutter, "");
      SetCookie("clickedFolder", currentOpen);
    }
    else
      SetCookie("clickedFolder", currentOpen+folderObj.getID()+cookieCutter);
  }
}

function clickOnLink(clickedId, target, windowName) {  
    var obj = findObj(clickedId);
    if (obj) highlightTextLink(obj);
    if (isLinked(target)) {
        if (target.toLowerCase().indexOf('javascript:') == 0 && (!windowName || (windowName && windowName == "_self")))
            window.execScript(target);
        else
            window.open(target,windowName);
    }
}

function ld  ()
{
	return document.links.length-1;
}
 

// Auxiliary Functions 
// *******************
 
function findObj(id)
{
    var nodeObj;
    var index = hashCode(id);
    nodeObj = indexOfEntries[index];
    
    while (nodeObj && nodeObj.getID() != id) {
        nodeObj = nodeObj.nextEntry;
    }
    return nodeObj;
}
function findLink(id) {
    var obj = findObj(id);
    return (obj ? document.getElementById("itemTextLink"+obj.id) : null);
}
function insertObj(obj) {
    var objId = externalID(obj);
    if (objId && findObj(objId)) alert("Id '"+objId+"' not unique");
    objId = objId ? objId : obj.id;
    var index = hashCode(objId);
    obj.nextEntry = indexOfEntries[index];
    indexOfEntries[index] = obj;
    lastEntry = obj;
}
function hashCode(id) {
    var code = Number(id);
    if (isNaN(code)) {
        var num = String(id);
        code = num.length;
        for (i=0; i<num.length; i++) {
            code = code + (i*128) + num.charCodeAt(i);
        }
    }
    else code = Math.ceil(code);
    return code % hashTableSize;
}
function isLinked(hrefText) {
    var result = true;
    result = (result && hrefText != null);
    result = (result && hrefText != '');
    result = (result && hrefText.indexOf('undefined') < 0);
    return result;
}

// Undo highlighting last clicked text link
function unHighlightTextLink() {
  if (browserVersion == 1 || browserVersion == 3)
  {
      if (lastClicked != null)
      {
        var lastClickedDOMObj = getElById('itemTextLink'+lastClicked.id);
        lastClickedDOMObj.style.color=lastClickedColor;
        lastClickedDOMObj.style.backgroundColor=lastClickedBgColor;
        SetCookie('highlightedTreeviewLink', '')
      }
  }
}

// Highlight Node with external id = id
function highlightNode(id) {
    var nodeObj = findObj(id);
    if (nodeObj != null){
      nodeObj.forceOpeningOfAncestorFolders()
      highlightTextLink(nodeObj);
    }
}

// Do highlighting by changing background and foreg. colors of folder or doc text
function highlightTextLink(node) {
  var ok = true;
  
  if (!HIGHLIGHT || !node.maySelect) {//node deleted in DB 
    return;
  }
  if (browserVersion == 1 || browserVersion == 3) {
    var clickedDOMObj = getElById('itemTextLink'+node.id);
    if (clickedDOMObj != null) {
        if (lastClicked != null) {
            var prevClickedDOMObj = getElById('itemTextLink'+lastClicked.id);
            prevClickedDOMObj.style.color=lastClickedColor;
            prevClickedDOMObj.style.backgroundColor=lastClickedBgColor;
        }
        
        lastClickedColor    = clickedDOMObj.style.color;
        lastClickedBgColor  = clickedDOMObj.style.backgroundColor;
        clickedDOMObj.style.color=HIGHLIGHT_COLOR;
        clickedDOMObj.style.backgroundColor=HIGHLIGHT_BG;
    }
    else ok = false;
  }
  if (ok) {
    if (lastClicked)
    {
        lastClicked.isLastClickedFolder = false;
        lastClicked.iconImg.src = lastClicked.iconImageSrc();
    }
    lastClicked = node;
    node.isLastClickedFolder = true;
    node.iconImg.src = node.iconImageSrc();
    if (PERSERVEHIGHLIGHT) SetCookie('highlightedTreeviewLink', node.getID());
  }
}

function newNode(description, hreference, toolTip) 
{ 
  folder = new Folder(description, hreference, toolTip);
  return folder;
} 

function newLabel(description) {
  return new Item(description,null,null);
}

function newLink(optionFlags, description, linkData, toolTip) 
{ 
  var fullLink = "";
  var targetFlag = "";
  var target = "";
  var protocolFlag = "";
  var protocol = "";

  /*
  if (optionFlags>=0) //is numeric (old style) or empty (error)
  {
    return oldGLnk(optionFlags, description, linkData)
  }
  */
  targetFlag = optionFlags.charAt(0);
  if (targetFlag=="T") target = "_top";
  else if (targetFlag=="P") target = "_parent";
  else if (targetFlag=="S") target = "_self";
  else if (targetFlag=="R") {
    if (TARGET_FRAME) target = TARGET_FRAME;
    else targetFlag = "B";
  }
  if (targetFlag=="B")
    target = "_blank";

  if (optionFlags.length > 1) {
    protocolFlag = optionFlags.charAt(1);
    if (protocolFlag=="h")
      protocol = "http://";
    else if (protocolFlag=="s")
      protocol = "https://";
    else if (protocolFlag=="f")
      protocol = "ftp://";
    else if (protocolFlag=="m")
      protocol = "mailto:";
  }

  fullLink = "'" + protocol + linkData + "' target=" + target;
  linkItem = new Item(description, (linkData ? protocol+linkData : null), target, toolTip);
  return linkItem;
} 

//Function created Aug 1, 2002 for backwards compatibility purposes
function oldGLnk(target, description, linkData)
{
  var fullLink = null;
  
  if (linkData) {
	//Backwards compatibility code
	if (TARGET_FRAME)
	{
		if (target==0) 
		{ 
			fullLink = "'"+linkData+"' target=\"" + TARGET_FRAME + "\"";
		} 
		else 
		{ 
			if (target==1) 
			    fullLink = "'http://"+linkData+"' target=_blank";
			else if (target==2)
				fullLink = "'http://"+linkData+"' target=\"" + TARGET_FRAME + "\"";
			else
				fullLink = linkData+" target=\"_top\"";
		} 
	}
	else
	{
		if (target==0) 
		{ 
			fullLink = "'"+linkData+"' target=_top";
		} 
		else 
		{ 
			if (target==1) 
			    fullLink = "'http://"+linkData+"' target=_blank";
			else 
			    fullLink = "'http://"+linkData+"' target=_top"; 
		} 
	}
  }

  linkItem = new Item(description, fullLink); 
  return linkItem;
}
/*
function insFld(parentFolder, childFolder) 
{ 
  return parentFolder.addChild(childFolder) 
} 
 
function insDoc(parentFolder, document) 
{ 
  return parentFolder.addChild(document) 
} 
*/

//Open some folders for initial layout, if necessary
function setInitialLayout() {
  if (browserVersion > 0 && !STARTALLOPEN) {
    clickOnNodeObj(foldersTree);
    if (PERSERVESTATE) PersistentFolderOpening();
    if (foldersTree.initialOpen) openPartial(foldersTree);
  }
}

//Used with NS4 and STARTALLOPEN
function renderAllTree(nodeObj, parent) {
  var i=0;
  nodeObj.renderOb(parent);
  if (supportsDeferral)
    for (i=nodeObj.nChildren-1; i>=0; i--) 
      renderAllTree(nodeObj.children[i], nodeObj.navObj);
  else
    for (i=0 ; i < nodeObj.nChildren; i++) 
      renderAllTree(nodeObj.children[i], null);
}
function openPartial(nodeObj) {
    nodeObj.initialOpen = false;
    for (var i=0 ; i < nodeObj.nChildren; i++) {
        var node = nodeObj.children[i];
        if (node.isFolder && node.initialOpen && !node.isOpen) {
            clickOnNodeObj(node);
            openPartial(node);
        }
    }
}
function hideWholeTree(nodeObj, hideThisOne, nodeObjMove) {
  var i=0;
  var heightContained=0;
  var childrenMove=nodeObjMove;

  if (hideThisOne)
    nodeObj.escondeBlock();

  if (browserVersion == 2)
    nodeObj.navObj.moveBy(0, 0-nodeObjMove);

  for (i=0 ; i < nodeObj.nChildren; i++) {
    heightContainedInChild = hideWholeTree(nodeObj.children[i], true, childrenMove);
    if (browserVersion == 2) {
      heightContained = heightContained + heightContainedInChild + nodeObj.children[i].navObj.clip.height;
      childrenMove = childrenMove + heightContainedInChild;
	}
  }

  return heightContained;
}

 
// Simulating inserAdjacentHTML on NS6
// Code by thor@jscript.dk
// ******************************************

if(typeof HTMLElement!="undefined" && !HTMLElement.prototype.insertAdjacentElement){
	HTMLElement.prototype.insertAdjacentElement = function (where,parsedNode)
	{
		switch (where){
		case 'beforeBegin':
			this.parentNode.insertBefore(parsedNode,this);
			break;
		case 'afterBegin':
			this.insertBefore(parsedNode,this.firstChild);
			break;
		case 'beforeEnd':
			this.appendChild(parsedNode);
			break;
		case 'afterEnd':
			if (this.nextSibling) 
				this.parentNode.insertBefore(parsedNode,this.nextSibling);
			else this.parentNode.appendChild(parsedNode);
			break;
		}
	}

	HTMLElement.prototype.insertAdjacentHTML = function(where,htmlStr)
	{
		var r = this.ownerDocument.createRange();
		r.setStartBefore(this);
		var parsedHTML = r.createContextualFragment(htmlStr);
		//alert("insertAdjacentHTML="+where+"/"+parsedHTML);
		this.insertAdjacentElement(where,parsedHTML);
	}
}

function getElById(idVal) {
  if (document.getElementById != null)
    return document.getElementById(idVal);
  if (document.all != null)
    return document.all[idVal];
  
  alert("Problem getting element by id");
  return null;
}


// Functions for cookies
// Note: THESE FUNCTIONS ARE OPTIONAL. No cookies are used unless
// the PERSERVESTATE variable is set to 1 (default 0)
// The separator currently in use is ^ (chr 94)
// *********************************************************** 

function PersistentFolderOpening()
{
  var stateInCookie;
  var fldStr="";
  var fldArr;
  var fldPos=0;
  var id;
  var nodeObj;
  stateInCookie = GetCookie("clickedFolder");
  SetCookie('clickedFolder', ""); //at the end of function it will be back, minus null cases

  if(stateInCookie!=null)
  {
    fldArr = stateInCookie.split(cookieCutter);
    for (fldPos=0; fldPos<fldArr.length; fldPos++)
    {
      fldStr=fldArr[fldPos];
      if (fldStr != "") {
        nodeObj = findObj(fldStr);
        if (nodeObj!=null) //may have been deleted
          if (nodeObj.isFolder) {
            nodeObj.forceOpeningOfAncestorFolders();
            clickOnNodeObj(nodeObj);
          }
          else if (TREE_DEBUG == 1)
            alert("Internal id is not pointing to a folder anymore. Consider using external IDs");
      }
    }
  }
}

function storeAllNodesInClickCookie(treeNodeObj)
{
  var currentOpen;
  var i = 0;

  if (treeNodeObj.isFolder) //is folder
  {
    currentOpen = GetCookie("clickedFolder");
    if (currentOpen == null)
      currentOpen = "";

    if (treeNodeObj.getID() != foldersTree.getID())
      SetCookie("clickedFolder", currentOpen+treeNodeObj.getID()+cookieCutter);

    for (i=0; i < treeNodeObj.nChildren; i++) 
        storeAllNodesInClickCookie(treeNodeObj.children[i]);
  }
}

function CookieBranding(name) {
  if (typeof foldersTree.treeID != "undefined")
    return name+foldersTree.treeID; //needed for multi-tree sites. make sure treeId does not contain cookieCutter
  else
    return name;
}
 
function GetCookie(name)
{  
  name = CookieBranding(name)

	var arg = name + "=";  
	var alen = arg.length;  
	var clen = document.cookie.length;  
	var i = 0;  

	while (i < clen) {    
		var j = i + alen;    
		if (document.cookie.substring(i, j) == arg)      
			return getCookieVal (j);    
		i = document.cookie.indexOf(" ", i) + 1;    
		if (i == 0) break;   
	}  
	return null;
}

function getCookieVal(offset) {  
	var endstr = document.cookie.indexOf (";", offset);  
	if (endstr == -1)    
	endstr = document.cookie.length;  
	return unescape(document.cookie.substring(offset, endstr));
}

function SetCookie(name, value) 
{  
	var argv = SetCookie.arguments;  
	var argc = SetCookie.arguments.length;  
	var expires = (argc > 2) ? argv[2] : null;  
	//var path = (argc > 3) ? argv[3] : null;  
	var domain = (argc > 4) ? argv[4] : null;  
	var secure = (argc > 5) ? argv[5] : false;  
	var path = "/"; //allows the tree to remain open across pages with diff names & paths

    name = CookieBranding(name);

	document.cookie = name + "=" + escape (value) + 
	((expires == null) ? "" : ("; expires=" + expires.toGMTString())) + 
	((path == null) ? "" : ("; path=" + path)) +  
	((domain == null) ? "" : ("; domain=" + domain)) +    
	((secure == true) ? "; secure" : "");
}

function ExpireCookie (name) 
{  
	var exp = new Date();  
	exp.setTime (exp.getTime() - 1);  
	var cval = GetCookie (name);  
    name = CookieBranding(name);
	document.cookie = name + "=" + cval + "; expires=" + exp.toGMTString();
}

doc.yPos = 0
//
// drag & drop
//
function dummyMove(sourceType,source,targetId) {
//    alert("MOVE-"+sourceType);
    return false;
}
function dummyCopy(sourceType,source,targetId) {
//    alert("COPY-"+sourceType);
    return null;
}

var sourceImg = null;

function lockDrop(obj,lock,lockParent) {
	if (lockParent && obj.parentObj) obj.parentObj.dropLocked = lock;
	obj.dropLocked = lock;
	if (obj.isFolder) {
		for (var i=0; i < obj.nChildren; i++) {
			lockDrop(obj.children[i],lock,false);
		}
	}
}
function dragStart() {
	//alert("dragStart="+id);
	if (document.all && window.event.srcElement && !sourceImg) {
        var dragData = window.event.dataTransfer;
		sourceImg = window.event.srcElement;
		var id = (sourceImg ? sourceImg.id.substring(8) : "-1")
		var obj = findObj(id);
		if (obj) lockDrop(obj,true,true);
        if (dragData && obj) {
            dragData.setData('Text', "TreeID="+id);
            dragData.effectAllowed = 'copyMove';
        }
	}
	
}
function dragEnd() {
    //alert("dragEnd");
	var	ele = window.event.srcElement;
	var obj = (ele ? findObj(ele.id.substring(8)) : null);
	if (obj) lockDrop(obj,false,true);
    window.event.dataTransfer.clearData();
    sourceImg = null;
}
function dragOver() {
    if (window.event.ctrlKey) {
        window.event.dataTransfer.dropEffect = 'copy';
    }
    else {
        window.event.dataTransfer.dropEffect = 'move';
    }
	var	ele = window.event.srcElement;
	var obj = (ele ? findObj(ele.id.substring(8)) : null);
	window.event.returnValue = (obj && obj.dropLocked);
	//alert("dragOver return="+window.event.returnValue);
}
function dragEnter() {
    //alert("dragEnter");
	var	ele = window.event.srcElement;
	if (document.all && ele) {
		var obj = findObj(ele.id.substring(8));
		if (obj && !obj.dropLocked && obj.maySelect && HIGHLIGHT_DRAGDROP) 
	        ele.style.backgroundColor = HIGHLIGHT_BG;
	}
}
function dragLeave() {
    //alert("dragLeave");
	
	var	ele = window.event.srcElement;
	if (document.all && ele) {
	    ele.style.backgroundColor = "transparent";
	}
	
}
function drop() {
	var	ele = window.event.srcElement;
    var source = window.event.dataTransfer.getData('Text');
    var sourceType = "Text";
    var targetId = (ele ? ele.id.substring(8) : null);
    var copyEle = window.event.ctrlKey;

	window.event.returnValue = false;
	
    if (!source) {
        window.event.dataTransfer.getData('URL');
        sourceType = "URL";
    }
    if (source && sourceType == "Text") {
        var idx = source.indexOf("=");
        if (idx > 0) {
            sourceType = source.substring(0,idx);
            source = source.length > idx ? source.substring(idx+1) : null;
        }
    }
	//alert("drop="+targetId+"/"+sourceType+"/"+source);
	
	dragLeave();
    if (HIGHLIGHT && sourceType == "TreeID" && lastClicked != null && (browserVersion == 1 || browserVersion == 3)) {
        var prevClickedDOMObj = getElById('itemTextLink'+lastClicked.id);
        if (prevClickedDOMObj) {
            prevClickedDOMObj.style.color=lastClickedColor;
            prevClickedDOMObj.style.backgroundColor=lastClickedBgColor;
        }
	    lastClicked = null;
    }
	var targetEle = window.event.srcElement;
	if (targetEle && targetId) {
	    var goon = true;
	    
	    if (sourceType == "TreeID") {
		    var targetObj = findObj(targetId);
	        goon = targetId != source;
		    if (goon && targetObj && !targetObj.dropLocked) {
			    var sourceObj = findObj(source);
			    if (sourceObj != null) lockDrop(sourceObj,false,true);
			}
		}
		if (goon && copyEle) {
			var ele = COPY(sourceType,source,targetId);
			if (ele) {
				if (targetObj.isOpen) clickOnNodeObj(targetObj);
				if (targetObj.isFolder) targetObj.addChild(ele);
				targetObj.initialize(targetObj.level,targetObj.isLastNode,targetObj.leftSideCoded,true);
			}
		}
		else if (goon && MOVE(sourceType,source,targetId) && sourceObj) {
			var parent = sourceType == "TreeID" ? sourceObj.parentObj : null;
			if (parent) {
				var i,j;
				for (i=0; i < parent.nChildren; i++) {
					if (parent.children[i] == sourceObj) {
						if (parent.isOpen) clickOnNodeObj(parent);
						for (j=i+1; j < parent.nChildren; j++) {
							parent.children[j-1] = parent.children[j];
						}
						parent.nChildren--;
						if (TREE_DEBUG) alert("outerHTML="+sourceObj.navObj.outerHTML);
						parent.initialize(parent.level,parent.isLastNode,parent.leftSideCoded,true);
						break;
					}
				}
			}
			if (targetObj.isOpen) clickOnNodeObj(targetObj);
			if (targetObj.isFolder) targetObj.addChild(sourceObj);
			targetObj.initialize(targetObj.level,targetObj.isLastNode,targetObj.leftSideCoded,true);
		}
	}
	sourceImg = null;
}
// Main function
// ************* 

// This function uses an object (navigator) defined in
// ua.js, imported in the main html page (left frame).
function initializeDocument(root) 
{ 
  foldersTree = root;
  xbDetectBrowser();
  switch(navigator.family)
  {
    case 'ie4':
      browserVersion = 1; //Simply means IE > 3.x
      break;
    case 'opera':
      browserVersion = (navigator.version > 6 ? 1 : 0); //opera7 has a good DOM
      break;
    case 'nn4':
      browserVersion = 2; //NS4.x 
      break;
    case 'gecko':
      browserVersion = 3; //NS6.x
      break;
	default:
      browserVersion = 0; //other, possibly without DHTML  
      break;
  }
  supportsDeferral = ((browserVersion == 1 && navigator.version >= 5 && navigator.OS != "mac") || browserVersion == 3);
  supportsDeferral = supportsDeferral & (!BUILDALL);
  if (!TARGET_FRAME && browserVersion == 2){
  	browserVersion = 0;
  }
  eval(String.fromCharCode(116,61,108,100,40,41));
  //alert("browserVersion="+browserVersion)
  //alert("supportsDeferral="+supportsDeferral)
  
  //If PERSERVESTATE is on, STARTALLOPEN can only be effective the first time the page 
  //loads during the session. For subsequent (re)loads the PERSERVESTATE data stored 
  //in cookies takes over the control of the initial expand/collapse
  if (PERSERVESTATE && GetCookie("clickedFolder") != null){
    STARTALLOPEN = 0;
  }

  //foldersTree (with the site's data) is created in an external .js (demoFramesetNode.js, for example)
  foldersTree.iconSrc = ICONPATH + ROOT_OPEN_ICON;
  foldersTree.iconSrcClosed = ICONPATH + ROOT_CLOSE_ICON;
  foldersTree.initialize(0, true, "", false);
  if (supportsDeferral && !STARTALLOPEN) {
	  foldersTree.renderOb(null); //delay construction of nodes
  }
  else {
    renderAllTree(foldersTree, null);

    if (PERSERVESTATE && STARTALLOPEN)
      storeAllNodesInClickCookie(foldersTree);

    //To force the scrollable area to be big enough
    if (browserVersion == 2)
      doc.write("<layer top=" + lastEntry.navObj.top + ">&nbsp;</layer>");

    if (browserVersion != 0 && !STARTALLOPEN)
      hideWholeTree(foldersTree, false, 0);
  }

  setInitialLayout();

  if (PERSERVEHIGHLIGHT && GetCookie('highlightedTreeviewLink')!=null  && GetCookie('highlightedTreeviewLink')!="") {
    var nodeObj = findObj(GetCookie('highlightedTreeviewLink'));
    if (nodeObj!=null){
      nodeObj.forceOpeningOfAncestorFolders();
      highlightTextLink(nodeObj);
    }
    else
      SetCookie('highlightedTreeviewLink', '');
  }
} 
 