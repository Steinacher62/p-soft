/*
* ig_htmleditor.js
* Version 14.2.20142.1028
* Copyright(c) 2001-2014 Infragistics, Inc. All Rights Reserved.
*/


//vs 03/14/14
// NOTE: the contents of old ig_htmleditor_ie.js and ig_htmleditor_moz.js files are located at the end of this file
if(typeof iged_all!="object")
	var iged_all=new Object();
function iged_getById(id)
{
	if(!id)return null;
	var o=iged_all[id];
	if(o!=null)return o;
	for(var i in iged_all)if((o=iged_all[i])!=null)if(o.ID==id||o._elem==id)return o;
	return null;
}
function iged_new(id,ta,tb,p1,p2,p3,p4,p5)
{
	if(iged_all[id])
	{
		iged_closePop();
		delete iged_all._clrNum;
		var clr=iged_all._clr;
		if(clr&&clr.parentNode){clr.parentNode.removeChild(clr);}
		delete iged_all._clr;
	}
	this.ID=id;
	this._ta=ta;
	this._tb=tb;
	
	
	
	this._foc=-1;
	this._elem0=iged_el(id);
	this._addLsnr=function(e,n,f)
	{
		try{if(e&&e.addEventListener){e.addEventListener(n,f,false);return 1;}}catch(i){}
		try{if(e&&e.attachEvent){e.attachEvent("on"+n,f);return 1;}}catch(i){}
		try
		{
		eval("var old=e.on"+n);
		var sF=f.toString();var i=sF.indexOf("(")+1;
		if((typeof old=="function")&&i>10)
		{
			old=old.toString();
			var args=old.substring(old.indexOf("(")+1,old.indexOf(")"));
			while(args.indexOf(" ")>0)args=args.replace(" ","");
			if(args.length>0)args=args.split(",");
			old=old.substring(old.indexOf("{")+1,old.lastIndexOf("}"));
			sF=sF.substring(9,i);
			if(old.indexOf(sF)>=0)return;
			var s="f=new Function(";
			for(i=0;i<args.length;i++){if(i>0)sF+=",";s+="\""+args[i]+"\",";sF+=args[i];}
			sF+=");"+old;
			eval(s+"sF)");
		}
		eval("e.on"+n+"=f");
		return 1;
		}catch(i){}
		return 0;
	}
	this._alert=function(s){iged_closePop();window.alert(s?s:this._cs(11));}
	
	this._prop=p1.split("|");
	for(var i=9;i<this._prop.length;i++)this._prop[i]=parseInt(this._prop[i]);
	this._butS=p2.split("|");
	this._ddImgClr=p3.split("|");
	this._dlgS=p4.split("|");
	this._exp=p5;
	if(!iged_all._clrNum)iged_all._clrNum=this._prop[11];
	this._fixDlgS=function(x)
	{
		x=x.split("|");
		var i=0,s=this._dlgS,d=x[1]!="";
		if(d&&s[0]!="")x[1]=s[0]+" "+x[1];
		while(++i<14){if(x[i]==""&&(!d||i>8))x[i]=s[i-1];if(x[i]=="")x[i]=null;}
		return x;
	}
	
	this._addMenu=function()
	{
		if(this._prop[13]==0||iged_all._onMenu)return;
		iged_all._onMenu=true;
		this._addLsnr(this._doc(),"contextmenu",iged_mainEvt);
	}
	this._fire=function(id,act,p1,p2,p3,p4,p5,p6)
	{
		var p,e=this._evt;
		id=this._prop[id];
		if(e==null)e=this._evt=new Object();
		e.p4=e.p5=e.p6=e.p7=e.p8=e.key=e.act=null;
		e.cancelPostBack=e.needPostBack=e.cancel=false;
		e.event=this._evt0;
		if(id&&id.length>0)
		{
			p=id=iged_replaceS(iged_replaceS(id,'&quot;','"'),'&pipe;','|');
			if(p.indexOf('=')<0&&p.indexOf(';')<0&&p.indexOf(',')<0&&p.indexOf('(')<0&&p.indexOf('{')<0&&p.indexOf('[')<0&&p.indexOf('"')<0&&p.indexOf("'")<0&&p.indexOf(':')<0)
				p+="(this,act,e,p1,p2,p3,p4,p5,p6)";
			try{eval(p);}catch(i){window.status="Can't eval "+id;}
		}
		p=e.needPostBack?true:(this._post&&!e.cancelPostBack&&!e.cancel);
		this._post=false;
		if(p)if((p=this._ta.form)!=null)
		{
			iged_update();
			this._ta.value+="\03"+act+"\04";
			this._posted=true;
			if(typeof __doPostBack=="function")__doPostBack(this.ID,"");
			else p.submit();
			return true;
		}
		return e.cancel==true;
	}
	this._swapS=function(s1,s2)
	{
		if(s1)s1=s1.style;if(s2)s2=s2.style;
		if(!s1||!s2)return;
		var bk=s1.backgroundColor,bc=s1.borderColor,c=s1.color,bs=s1.borderStyle;
		s1.backgroundColor=s2.backgroundColor;s1.borderColor=s2.borderColor;s1.color=s2.color;s1.borderStyle=s2.borderStyle;
		s2.backgroundColor=bk;s2.borderColor=bc;s2.color=c;s2.borderStyle=bs;
	}
	this._format=function(act,mod,restore,foc)
	{
		iged_closePop(act);
		this.focus();
		iged_all._noUndo=false;
		var e=this._elem,f=this._ie!=null;
		try{
		if(f)
		{
			if(foc)e.setActive();
			if(restore&&iged_all._curRange)
				iged_all._curRange.select();
			e=e.ownerDocument||e.document;
		}
		else
		{	
			if(foc)e.contentWindow.focus();
			e=e.contentDocument;
		}
		e.execCommand(act,f,(!mod||mod=="")?null:mod);
		}catch(e){}
	}
	
	this.hasFocus=function(){return this._foc>0&&this._focs;}
	
	this.focus=function(f){if(this._foc<2||f)try{if(this._ie)this._elem.focus();else this._win().focus();}catch(i){}}
	this.setText=function(txt,ie)
	{
		var o=this,e=this._elem;
		if(o._ie)
		{
			if(o._html)e.innerText=o._decode(txt);
			else o._txt(txt);
		}
		else
		{
			e=o._doc().body;
			if(!o._html)e.innerHTML=txt;
			else
			{
				e.innerHTML="";
				e.appendChild(document.createTextNode(txt));
			}
		}
		o._mod=true;
		if(ie!=1)this._update();
	}
	this._onSelFont=function(e,act)
	{
		this._format(act,e.id,this._ie&&iged_all._curRange&&iged_all._curRange.boundingWidth!=0,true);
		this._syncBullets();
		this._afterSel(e);
	}
	this._afterSel=function(e)
	{
		iged_closePop(3);
		if(!this._ie)return;
		var elem=iged_el(e.name),t=e.innerText;
		if(elem&&t&&t!="")elem.firstChild.firstChild.firstChild.innerText=t;
	}
	this._onSubSup=function(s1,s2)
	{
		try
		{
			if(this._ie)
			{
				iged_all._curRange=this._range();
				if(iged_all._curRange.queryCommandState(s2))
					this._format(s2,"",false,true);
			}
			else if(this._elem.contentDocument.queryCommandState(s2))
				this._format(s2,"",false,true);
			this._format(s1,"",false,true);
		}catch(s1){}
	}
	this._onToggleBdr=function()
	{
		var t=null,i=-1,toggle=this._toggle;
		try
		{
			if(this._ie)t=this._getCurTable();
			else if(this._inTbl())t=this._getTag(this._cont(),'TABLE');
		}catch(e){}
		if(t){this._tblBdr(t,t.border==0);return;}
		t=this._ie?this._elem:this._doc();
		t=t.getElementsByTagName('TABLE');
		while(++i<t.length)
		{
			if(i==0)
			{
				if(toggle==null)toggle=t[i].border!=0;
				this._toggle=toggle=!toggle;
			}
			this._tblBdr(t[i],toggle);
		}
	}
	this._tblBdr=function(t,b)
	{
		var old=t._oldB;
		if(!old){old=t.border;if(old==0)old=1;t._oldB=old;}
		t.border=b?old:0;
	}
	this._onClr=function(clr)
	{
		iged_closePop("clr");
		if(!clr)clr="";
		var f=this._clrTarget,k=this._key;
		delete this._key;
		if(f)
		{
			try
			{
				f.value=clr;
				f.style.backgroundColor=(clr=="")?"#F0F0F0":clr;
			}catch(f){}
			return;
		}
		f=this._popF==1;
		if(k&&this._fire(1,k))return;
		if(this._ie)this._format(f?"forecolor":"backcolor",clr,true,true);
		else this._format(f?"forecolor":"hilitecolor",clr,false,true);
		if(f)this._syncBullets();
		this._fire(2,k);
	}
	this._fixPop=function(e,rc,skip)
	{
		if(!e||!e.getAttribute||(e._oldP&&!rc))return;
		e._oldP=true;
		var a=e.getAttribute("act");
		if(a&&a.length>0)this._choiceAct=a;
		a=e.getAttribute("sts");
		if(a&&a.length>0)this._itemStyle=a.split("?");
		a=e.getAttribute("igf");
		if(a&&a.length==1)
		{
			var f=this._itemStyle,s=e.style;
			if(a=="m"&&rc)
			{
				
				if(!e.onmouseup)return;
				
				var f=e.getAttribute("igm");
				
				s.display=(rc==3&&f=="i")||(rc==2&&f!="i")||(rc==1&&!f)?"":"none";
			}
			if(a=="c"||a=="m"||a=="l")
			{
				if(skip)return;
				s.cursor=(a=="m")?"pointer":"default";
				this._addLsnr(e,"mouseover",iged_choiceEvt);
				this._addLsnr(e, "mouseout", iged_choiceEvt);
                // K.D. June 9th, 2014 Bug #171570 The mousedown event needs to be canceled to not lose selection in IE
				this._addLsnr(e, "mousedown", iged_cancelEvt);
				e._b=e._b2=s.backgroundColor;
				e._f=e._f2=s.color;
				if(a=="m"){if(f)e._b2=f[0];}
				else
				{
					e._b2=this._ddImgClr[3];e._f2=this._ddImgClr[4];
					this._addLsnr(e,"click",iged_choiceEvt);
					if(a=="l")
					{
						s.fontWeight="bold";
						s.fontFamily="verdana,tahoma";
						s.fontSize="12px";
						return;
					}
					e._act=this._choiceAct;
					e.noWrap=true;
					if((!s.fontFamily||s.fontFamily=="")&&f&&f[0]!="")s.fontFamily=f[0];
					if((!s.fontSize||s.fontSize=="")&&f&&f[1]!="")s.fontSize=f[1];
				}
			}
			else try{eval(this._prop[8]);}catch(a){}
			return;
		}
		e=e.childNodes;
		if(e)for(var i=0;i<e.length;i++)this._fixPop(e[i],rc,skip);
	}
	this._fixMouse=function(e,p)
	{
		if(!e||!e.getAttribute||(e._old&&p))return;
		var i=e.tagName,a=(p==1)?e.getAttribute("mm"):null;
		if(!i||i=="INPUT"||i=="SELECT")return;
		e.unselectable="on";
		e._old=true;
		if(!a||a.length<1)
		{
			e=e.childNodes;i=-1;
			if(e)while(++i<e.length)this._fixMouse(e[i],p);
			return;
		}
		var dd=a=="t";
		if(dd)e.mm="x";
		else
		{
			var m=a.split("|"),s=e.style,b=this._butS;
			if((i=b.length)>m.length){a=null;m=new Array();}
			while(i-->0)if(!a||m[i].length<2)
			{
				m[i]=b[i];
				if(i==0)s.backgroundColor=b[i];
				if(i==1)s.borderColor=b[i];
				if(i==2)s.borderStyle=b[i];
			}
			this._addLsnr(e,"mouseup",iged_mEvt);
			e.mm=m;
		}
		a=e.getAttribute("im2");
		if(a&&a.length>5)e.imgs=a.split("|");
		if(dd)for(i=1;i<4;i++)if(e.imgs[i]=="")e.imgs[i]=this._ddImgClr[i-1];
		this._addLsnr(e,"mouseover",iged_mEvt);
		this._addLsnr(e,"mouseout",iged_mEvt);
		this._addLsnr(e,"mousedown",iged_mEvt);
		this._fixMouse(e);
	}
	this._clrDrop=function(e,doc)
	{
		if(!e)return;
		this._clrTarget=e;
		this._pos(e,this._clrInit0(iged_all._doc0=doc),8);
	}
	this._wait=function(e)
	{
		var pan=this._pan('_');
		if(!pan)return;
		var s=pan.style;s.height="15px";s.width="200px";s.border="";
		pan.innerHTML="<div style='padding:7px;font-family:verdana;font-size:10pt;font-weight:bold;color:#005000;background:#E0FFF0;border:1px solid #005080'>"+this._cs(14)+"</div>";
		
		iged_all._popID=null;
		this._pos(e,iged_all._pop=pan);
	}
	this._delay=function(){iged_all._canCloseCur=false;window.setTimeout("iged_all._canCloseCur=true",100);}
	this._pop=function(id,x,evt,flag,h,rc)
	{
		if(this._isKnown(id)&&this._sel()&&this._ie)iged_all._curRange=this._range();
		iged_closePop();
		var pan;
		this._clrTarget=null;
		if(id=="iged_0_clr"){pan=this._clrInit0();this._popF=x;x=null;}
		else pan=iged_el(id);
		if(!pan)return;
		this._choiceAct=this._itemStyle=null;
		if(!pan._igf||flag==3){this._fixPop(pan,rc,pan._igf);pan._igf=true;}
		var s=pan.style;
		if(x)
		{
			x=x.split("?");
			if(h!=null)x[7]=h;
			if(x[0])s.backgroundColor=x[0];if(x[1])s.borderColor=x[1];
			if(x[2])s.borderStyle=x[2];if(x[3])s.borderWidth=x[3];
			if(x[4])s.fontFamily=x[4];if(x[5])s.fontSize=x[5];if(x[6])s.color=x[6];
			if(x[7])s.height=x[7];if(x[8]){s.width=x[8];s.paddingLeft="2px";}
		}
		this._fixMouse(pan,2);
		this._pos(evt,pan,flag);
		iged_all._pop=pan;
		iged_all._popID=this.ID;
		this._doValid(this.ID);
		this._delay();
	}
	this._doValid=function(id)
	{
		if(iged_all._popValid){window.clearInterval(iged_all._popValid);delete iged_all._popValid;}
		if(id)iged_all._popValid=window.setInterval('iged_valid("'+id+'")',300);
	}
	this._valid=function()
	{
		var e=this._elem0;if(!e)e=this._elem;
		if(e&&e.offsetHeight==0){iged_closePop();return false;}
		return true;
	}
	this._isKnown=function(id)
	{
		var i=id.length;
		return this._ie&&(id.indexOf("iged_0_")==0||id.indexOf("_iged_dlg")==i-9);
	}
	this._body=function(){return this._doc().body;}
	this._pan=function(id)
	{
		var e=iged_el(id='iged_0_div'+(id?id:''));
		if(!e)try
		{
			e=document.createElement("DIV");
			e.style.display='none';
			this._elem.parentNode.appendChild(e);
			e.id=id;
		}catch(i){}
		return e;
	}
	this._getSelImg=function()
	{
		if(!this._ie)return iged_all._curImg;
		if(this._sel().type!="Control")return null;
		iged_all._curRange=this._range();
		if(iged_all._curRange.item(0).tagName=="IMG")return iged_all._curRange.item(0);
		return null;
	}
	this._fixListFormat=function()
	{
		if(this._ie)iged_all._curRange=this._range();
	}
	this._cssFont=function(f)
	{
		try
		{
			if(f.length==1)f=parseInt(f,10);
			if(f==1)return "xx-small";if(f==2)return "x-small";if(f==3)return "small";
			if(f==4)return "medium";if(f==5)return "large";if(f==6)return "x-large";if(f>6)return "xx-large";
		}catch(i){return "";}
		return f;
	}
	this._syncBullets=function()
	{
		var lis=document.getElementsByTagName("li");
		for(var i=0;i<lis.length;i++)
		{
			var li=lis[i];
			if(li.children.length>0)
			{
				if(li.children[0].tagName.toLowerCase()=="font")
				{
					var e=li.children[0];
					if(e.size!="")li.style.fontSize=this._cssFont(e.size);
					li.style.fontFamily=e.face;
					li.style.color=e.color;
				}
			}
		}
		iged_all._needSync=false;
	}
	this._setOl=function()
	{
		var sel;
		if(this._ie)
		{
			iged_all._curRange=this._range();
			try{sel=iged_all._curRange.parentElement();}catch(sel){return;}
		}
		else sel=this._cont();
		var ol=this._getTag(sel,'OL');
		if(!ol)return;
		var i=iged_nestCount(ol,"OL")%3;
		if(i==0)ol.type="i";
		if(i==1)ol.type="1";
		if(i==2)ol.type="a";
	}
	this._cleanWord=function(txt)
	{
		// new additions:
		// remove <!-- any content -->
		var ch='\03';
		if(txt.indexOf('<!--')>=0&&txt.indexOf(ch)<0)
			txt=txt.replace(/-->/g,ch).replace(new RegExp('<!--[^'+ch+']*'+ch,'g'),'').replace(new RegExp(ch,'g'),'-->');
		// \s* any number of optional spaces
		// [^";:]* any number of characters besides ";:
		txt=txt.replace(/\s*class=\s*"\s*mso[^"]*"/ig,'');
		// can not remove margin: list left-margings will be lost
		//txt=txt.replace(/\s*margin:\s*0[^:";]*\s[^:";]*\s[^:";]*;/ig,'');
		//txt=txt.replace(/\s*margin:\s*0[^:";]*\s[^:";]*\s[^:";]*"/ig,'"');
		txt=txt.replace(/\s*line-height:[^:";]*;/ig,'');
		txt=txt.replace(/\s*line-height:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*text-indent:[^:";]*;/ig,'');
		txt=txt.replace(/\s*text-indent:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*font-variant:[^:";]*;/ig,'');
		txt=txt.replace(/\s*font-variant:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*font-size-adjust:[^:";]*;/ig,'');
		txt=txt.replace(/\s*font-size-adjust:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*font-stretch:[^:";]*;/ig,'');
		txt=txt.replace(/\s*font-stretch:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*face="times new roman"/ig,'');
		txt=this._cwS(txt,'/normal');
		txt=this._cwS(txt,'"Times New Roman"');
		txt=this._cwS(txt,'Times New Roman');
		txt=txt.replace(/\s*dir="ltr"/ig,'');
		txt=txt.replace(/\s*mso-[^";:]*:[^";:]*;/ig,'');
		txt=txt.replace(/\s*mso-[^";:]*:[^":]*"/ig,'"');
		txt=txt.replace(/\s*tab-stops:[^:;"]*;/ig,'');
		txt=txt.replace(/\s*tab-stops:[^:"]*"/ig,'"');
		txt=txt.replace(/\s*page-break-before:[^:;"]*;/ig,'');
		txt=txt.replace(/\s*page-break-before:[^:"]*"/ig,'"');
		// do not remove spans with possible "style" (color...)
		txt=txt.replace(/<span[^style>]*>/ig,'<span>');
		txt=txt.replace(/\s*style="\s*"/ig,'');
		txt=txt.replace(/<o:p>\s*<\/o:p>/ig,'');
		txt=txt.replace(/<o:p>.*?<\/o:p>/ig,'&nbsp;');
		// <(b|u|i|strike)>\s*&nbsp;\s*<\/\1> is the same as <(b|u|i|strike)>\s*&nbsp;\s*<\/(b|u|i|strike)>
		txt=txt.replace(/<(b|u|i|strike)>\s*&nbsp;\s*<\/\1>/ig,'&nbsp;');
		//
		txt=txt.replace(/<(\/)?strong>/ig,"<$1B>");
		txt=txt.replace(/<(\/)?em>/ig,"<$1I>").replace(/<P class=[^>]*>/gi, "<P>").replace(/<LI class=[^>]*>/gi, "<LI>");
		txt=txt.replace(/<\\?\??xml[^>]*>/gi,"").replace(/<\/?\w+:[^>]*>/gi,"");
		







		
		txt=escape(txt);
		txt=iged_replaceS(txt,"%u2019","'");txt=iged_replaceS(txt,"%u201C","\"");txt=iged_replaceS(txt,"%u201D","\"");
		txt=iged_replaceS(txt,"%u2022","");txt=iged_replaceS(txt,"%u2026","...");
		return unescape(txt);
	}
	// remove v from "<xx style='xxvxx'>xx<xx"
	this._cwS=function(t,v)
	{
		var gt,i=0;
		while((i=t.indexOf(v,i+5))>0)
		{
			gt=t.indexOf('>',i);
			if(gt>0&&i+80>gt&&gt<t.indexOf('<',i))
			{
				gt=t.lastIndexOf(' style=',i);
				if(gt>0&&gt+80>i&&gt>t.lastIndexOf('>',i)&&gt>t.lastIndexOf('<',i))
					t=t.substring(0,i)+t.substring(i+v.length);
			}
		}
		return t;
	}
	this._toClr=function(c)
	{
		var a=Math.floor(c);
		c-=a;if(a>=15)c=a=15;
		if(c>0.8)c="F";else if(c>0.6)c="A";else if(c>0.3)c=5;else c=0;
		return ((a<10)?a:String.fromCharCode(55+a))+""+c;
	}
	this._clrInit=function()
	{
		var cols=iged_all._clrNum;
		if(!cols)cols=11;
		for(var i=0;i<cols;i++)for(var j=0;j<13;j++)
		{
			var e=iged_el("iged_clr_"+i+"_"+j);
			if(!e)continue;
			e.clr=e.style.backgroundColor=e.style.borderColor=iged_all._cur._clrGet(i,j,cols);
		}
	}
	this._clrGet=function(i,j,x)
	{
		var r,g,b,v=iged_all._clrRGB;
		if(v==null)v=iged_all._clrRGB=0;
		if(j--==0)r=g=b=(i==0)?0:(1+i*14.5/(x-1));
		else
		{
			var m=Math.floor(j/4),c=(x-1)/2;
			var z=[(i<=c)?0:15*(i-c+3)/(c+3),15-(j%4)*3,(i>c)?9:15*(c-i)/c];
			g=z[m%3];r=z[(m+1)%3];b=z[(m+2)%3];
		}
		if(v>0){r+=(15-r)*v;g+=(15-g)*v;b+=(15-b)*v;}
		if(v<0){v=-v;r/=v;g/=v;b/=v;}
		return "#"+this._toClr(r)+this._toClr(g)+this._toClr(b);
	}
	this._newE=function(doc,t,p){p.appendChild(t=doc.createElement(t));return t;}
	this._clrInit0=function(doc)
	{
		var o=doc,clr=doc?doc.getElementById("iged_clr0_id"):iged_all._clr;
		if(clr)
		{
			try{iged_el("iged_c_c0").style.backgroundColor="";iged_el("iged_c_c1").value="";}catch(o){}
			return clr;
		}
		if(!doc)doc=window.document;
		clr=doc.createElement("DIV");
		if(!o)iged_all._clr=clr;
		doc.body.insertBefore(clr,doc.body.firstChild);
		clr.id="iged_clr0_id";
		var s=clr.style;o=iged_all._cur;
		s.position="absolute";s.display="none";s.zIndex=100001;
		var sp,t0=doc.createElement("TABLE");
		clr.appendChild(t0);
		s=t0.style;s.backgroundColor="#E0E0E0";s.border="solid 1px #808080";
		t0.border=0;t0.cellSpacing=3;
		var tb0=doc.createElement("TBODY");
		t0.appendChild(tb0);
		var r=doc.createElement("TR");
		tb0.appendChild(r);
		var c0=o._newE(doc,"TD",r);
		c0.noWrap=true;c0.align="center";
		for(var i=0;i<9;i++)
		{
			sp=o._newE(doc,"SPAN",c0);
			var c=(i==4)?"&nbsp;0":((i<4)?("-"+(4-i)):("+"+(i-4)));
			sp.innerHTML=""+c;
			s=sp.style;
			s.border="solid 1px #90C0C0";s.cursor="pointer";s.fontSize="11px";s.fontFamily="courier new";
			r=4;
			if(i==4)iged_all._clrSelBut=sp;
			else r=(i<4)?5:6;
			sp.title=this._cs(r);
			s.color=(i==4)?"red":"black";s.margin="1px";
			c=6+i*1.2;
			s.backgroundColor="#"+o._toClr(c)+o._toClr(c)+o._toClr(c);
			sp.id="iged_c_b"+i;
			sp.setAttribute("c","b"+i);
			o._addLsnr(sp,"mouseout",iged_clrEvt);
			o._addLsnr(sp,"mouseover",iged_clrEvt);
			o._addLsnr(sp,"click",iged_clrEvt);
		}
		r=doc.createElement("TR");
		tb0.appendChild(r);
		var c1=o._newE(doc,"TD",r);
		c1.style.border="solid blue 1px";
		r=doc.createElement("TR");
		tb0.appendChild(r);
		var c2=o._newE(doc,"TD",r);
		c2.noWrap=true;c2.align="center";
		sp=o._newE(doc,"SPAN",c2);
		s=sp.style;
		s.border="solid 1px black";s.fontSize="16px";s.fontFamily="courier new";s.cursor="default";
		sp.title=this._cs(2);
		sp.innerHTML="&nbsp;&nbsp;";
		sp.id="iged_c_c0";
		sp=o._newE(doc,"SPAN",c2);
		sp.innerHTML="&nbsp;";
		var f=doc.createElement("INPUT");
		f.title=this._cs(3);
		c2.appendChild(f);
		o._addLsnr(f,"keydown",iged_clrEvt);
		o._addLsnr(f,"keyup",iged_clrEvt);
		s=f.style;s.fontSize="12px";s.width="130px";
		f.id="iged_c_c1";
		var t=doc.createElement("TABLE");
		c1.appendChild(t);
		t.cellSpacing=1;
		var i,j,tb=doc.createElement("TBODY"),tt=this._cs(1);
		t.appendChild(tb);
		var cols=iged_all._clrNum,w=r=12;
		if(!cols)cols=11;
		if(cols<11)w=19;else if(cols>11)r=w=10;
		w=r+'px '+w+'px 0px 1px';
		for(j=0;j<13;j++)
		{
			r=doc.createElement("TR");
			tb.appendChild(r);
			for(i=0;i<cols;i++)
			{
				var c=o._newE(doc,"TD",r);
				c.innerHTML="&nbsp;";
				c.id="iged_clr_"+i+"_"+j;
				c.title=tt;
				s=c.style;
				s.cursor="pointer";s.padding=w;s.fontSize="1px";s.border="solid 1px blue";
				c.setAttribute("c","c");
				o._addLsnr(c,"mouseout",iged_clrEvt);
				o._addLsnr(c,"mouseover",iged_clrEvt);
				o._addLsnr(c,"click",iged_clrEvt);
			}
		}
		this._clrInit();
		return clr;
	}
	this._pos=function(evt,pan,flag)
	{
		if(!evt)if((evt=window.event)==null)return;
		var x=0,y=0,x0=0,y0=0,e=(flag==8)?evt:evt.target;
		if(!e)if((e=evt.srcElement)==null)return;
		var elem=e,doc=iged_all._doc0;if(!doc)doc=window.document;
		// IE11 loses selection when iframe loses focus (mess)
		// to get around save old selection/range and restore it when pop-up is closed
		var panID=pan.id;
		if(!this._ie&&panID!="iged_0_itm"&&panID!="iged_0_ito"&&panID!="iged_0_rcm")
		{
			delete this._closeNoClr;
			// clr dialog when another drop-down is opened
			if(!this._oldRange||flag!=8)
			{
				this._oldRange=(this.getCaret()>=0)?this._range():null;
				if(this._oldRange)
					iged_all._cur_Drop=this;
			}
			else if(flag==8)
				this._closeNoClr=true;
		}
		
		if(iged_all._pop)while(e)
		{
			if(e.getAttribute&&e.getAttribute('igf')=='m'){elem=iged_all._lastPopParent;break;}
			e=e.parentNode;
		}
		e=elem;
		if(flag==2)
		{
			while(e&&e.nodeName!='TABLE')e=e.parentNode;
			if(!e)e=elem;
		}
		var eH=e.offsetHeight,s=pan.style;
		if(!eH)eH=20;
		var p,sx=0,ee=e,x=0,y=0,x1=0,y1=0,e0=this._elem0,ed=this._elem;
		var ep=ed.parentNode,saf=this._saf&&flag==3;
		
		if(flag==3&&!this._ie)ee=this._cDoc().body;
		
		if(flag==9&&elem.nodeName!='IMG')
		{
			
			ee=e=ep;
			y1+=25-ep.offsetHeight;
			x1+=50;
		}
		
		while(ee&&ee!=e0)
		{
			p=ee.parentNode;
			if(flag==3&&(ee==ed||ee==ep||(!this._ie&&ee.nodeName=='BODY')))
			{
				
				if(!this._ie){x1=evt.layerX;y1=evt.layerY;}
				x1=x1||evt.x||evt.offsetX||0;
				y1=y1||evt.y||evt.offsetY||0;
				eH=0;
				e=(p.nodeName=='TD'||!this._ie)?ed:ee;
				x1+=x+10;
				y1+=y+10;
				if(!this._ie){x1-=ee.scrollLeft;y1-=ee.scrollTop;break;}
				if(y==0&&!this._ie7)y1+=ee.scrollTop;
				sx=ep.scrollLeft;
				break;
			}
			
			


			if(ee.offsetParent)ee=(p==e0)?e0:ee.offsetParent;
			else ee=p;
		}
		ee=elem;
		
		






		
		pan._moved=pan.parentNode;
		pan._pan=e.parentNode;
		iged_all._lastPopParent=e;
		s.position='absolute';
		s.zIndex=100000;
		s.marginLeft='0px';
		
		if(pan.parentNode)pan.parentNode.removeChild(pan);
		e.parentNode.insertBefore(pan,e);
		s.display='';
		s.visibility='visible';
		var panW=pan.offsetWidth,panH=pan.offsetHeight,w0=e0.offsetWidth,h0=e0.offsetHeight;
		
		ee=e;
		x=y=0;
		while(ee&&ee!=e0)
		{
			if(ee==this._elem)break;
			x+=ee.offsetLeft;
			y+=ee.offsetTop;
			ee=ee.offsetParent;
		}
		if(!ee)x=y=0;
		x+=x1;y+=(y1+=eH);
		if(flag!=3||this._ie)
		{
			if((x+=(panW+=30)-w0-sx)>0)x1-=Math.min(x,panW);
			if(flag==3&&(y=y1-ep.scrollTop+(panH+=5)-ed.offsetHeight)>0)y1-=Math.min(y,panH);
		}
		s.marginLeft=x1+'px';
		s.marginTop=y1+'px';
	}
	this._cellProp=function()
	{
		var c=iged_all._curCell;
		if(!c)return;
		var alignH=iged_el("iged_cp_ha").value,alignV=iged_el("iged_cp_va").value;
		var w=iged_el("iged_cp_w").value,h=iged_el("iged_cp_h").value,noWrap=iged_el("iged_cp_nw").checked;
		var clrBk=iged_el("iged_cp_bk1").value,clrBd=iged_el("iged_cp_bd1").value;
		if(alignH&&alignH!="default")c.align=alignH;
		if(alignV&&alignV!="default")c.vAlign=alignV;
		if(w)c.width=w;
		if(h)c.height=h;
		if(noWrap)c.noWrap=noWrap;
		c.bgColor=clrBk;
		if(this._ie)c.borderColor=clrBd;
		else{c.setAttribute("bc",clrBd);if(clrBd!="")clrBd+=" solid 1px";c.style.border=clrBd;}
		iged_all._curCell = null;
        // K.D. July 1st, 2014 Bug #171570 The postback value is not updated.
		this._update();
		iged_closePop();
	}
	this._decode=function(t){return t;}
	this._fixStr=function(v,s)
	{
		v=iged_replaceS(iged_replaceS(v,'%3E','>',s),'%3C','<',s);
		if(s)v=iged_replaceS(v,'%22','%3C=!=%3E');
		v=iged_replaceS(v,'%22','"',s);
		return s?v:iged_replaceS(v,'<=!=>','%22');
	}
	this._update=function()
	{
		
		if(this._mod||this._foc>0)this._ta.value=this._fixStr(this._decode(this.getText()),true);
		this._mod=false;
	}
	this._onInsert=function(e)
	{
		var v=iged_replaceS(iged_replaceS(e.id,"&quot1;","'"),"&quot;","\"");
		if(this._ie)iged_insText(v,false,true);
		else
		{
			var n=document.createElement("SPAN");
			n.innerHTML=v;
			iged_insNodeAtSel(n);
		}
		this._afterSel(e);
	}
	this._amp=function(t)
	{
		var i,i1,i2,x=8,n=' href="';
		while(--x>5)
		{
			i=t.length;
			while((i2=i-1)>2)if((i=t.lastIndexOf(n,i2))>=0)
			{
				i1=t.indexOf('"',i+x);
				if(i1<i||i1>i2)continue;
				var t1=t.substring(i+x,i1);
				while(t1.indexOf('&amp;')>=0){t1=t1.replace('&amp;','&');i2=0;}
				if(i2==0)t=t.substring(0,i+x)+t1+t.substring(i1);
			}
			n=' src="';
		}
		return t;
	}
	this._getTag=function(e,t1,t2)
	{
	    if (!e) return e;
	    var t = e.tagName;
        // K.D. June 20th, 2014 Bug #173647 The element can be a text node and it has no tagName.
        if (t) {
		    if(t==t1||t==t2)return e;
		    if(e.id==iged_all._cur._elem.id||t=='BODY')return null;
        }
		return this._getTag(e.parentNode,t1,t2);
	}
	this._cs=function(i)
	{
		var o=this._csA;
		if(!o)
		{
			o=iged_el(this.ID);
			if(o)o=o.getAttribute('ig_cs');
			if(o)this._csA=o=o.split('|');
			else return '';
		}
		return o[i]?o[i]:'';
	}
	this._fixMouse(tb,1);
	iged_all._cur=iged_all[id]=this;
	try
	{
		id=navigator.userAgent.toLowerCase();
		this._chrome=id.indexOf('chrome')>=0;
		this._saf=this._chrome||id.indexOf('safari')>=0||id.indexOf('webkit')>0;
		this._opr=id.indexOf('opera')>=0;
		this._mac=id.indexOf('mac')>=0;
		this._ffox=id.indexOf('firefox')>=0;
		var i1=id.indexOf('msie ')+5;
		
		if(i1<5)i1=id.indexOf('trident')>0?id.indexOf('rv:')+3:0;
		if(i1>3)
		{
			id=id.substring(i1);
			i1=id.indexOf('.');
			i1=(i1>0&&i1<3)?parseInt(id.substring(0,i1)):0;
			
			var m=parseFloat(document.documentMode);
			if(!isNaN(m)&&m>6&&m<i1)i1=m;
			this._ieV=i1;
			this._ie7=i1==6||i1==7;
		}
	}catch(ex){}
	this._maxZ=function(e,zi)
	{
		if(!zi)zi=9999;
		while(e)
		{
			if(e.nodeName=='BODY'||e.nodeName=='FORM')break;
			z=this._getStyle(e,'zIndex');
			if(z&&z.substring)z=(z.length>4&&z.charCodeAt(0)<58)?parseInt(z):0;
			if(z&&z>=zi)zi=z+1;
			e=e.parentNode;
		}
		return zi;
	}
	this._onPaste=function()
	{
		var me=this;
		if(me._skipPaste)return;
		me._skipPaste=true;
		setTimeout(function(){try{iged_act('cleanword');}catch(e){}delete me._skipPaste;},10);
	}
	this._getStyle=function(e,p)
	{
		var s=e.currentStyle;
		if(!s)
		{
			var win=document.defaultView;
			if(!win)win=window;
			if(win.getComputedStyle)s=win.getComputedStyle(e,'');
			if(!s)s=e.style;
		}
		var val=s[p];
		if(!val||!s.getPropertyValue)return val;
		return s.getPropertyValue(p);
	}
	this._validate=function(blur)
	{
		var o=this,e=o._elem0,v=o._vld;
		if(!v)
		{
			if(blur)return;
			var inp=e.getElementsByTagName('INPUT'),i=inp.length;
			while(window.ValidatorOnChange&&!v&&i-->0)if(inp[i].Validators)v=inp[i];
			o._vld=v=v||'x';
		}
		if(v!='x')setTimeout(function()
		{
			var t=o.getText();
			if(t.length<100)
			{
				var tags=['div','span','p','font','strong','u','sup','sub','em','i','b','h1','h2','h3','h4','h5'],i=tags.length,j=i;
				t=t.toLowerCase().replace('&nbsp;','').replace(/ /g,'').replace(/\n/g,'').replace(/\r/g,'');
				while(j-->0)
					t=t.replace('<'+tags[j]+'>','').replace('</'+tags[j]+'>','');
				while(i-->0)if(t.indexOf('<'+tags[i])==0)
					t=t.substring(t.indexOf('>')+1);
			}
			e.value=(t=='<br>'||t=='<br/>')?'':t;
			window.ValidatorOnChange({srcElement:v});
		}, 10);
	}
	if(iged_all._submit)return;
	iged_all._submit=true;
	this._addLsnr(ta.form,"submit",iged_update);
	this._addLsnr(ta.form,"click",iged_update);
}
function iged_popTimer()
{
	var o=iged_all._ip;
	if(!o||!o.f)return;
	var d,s=o.pan.style,h=o.h,pH=o.panH;
	if(++o.i>1)o.i=0;
	if(h&&pH&&(((o.f&2)==0)||o.i==0))
	{
		d=(pH>>5)+(h>>3)+2;
		if((h+=d)>=pH){h=pH;o.panH=null;}
		o.h=h;
		s.height=h+"px";
		if(o.y>=0)s.top=(o.y+pH-h)+"px";
	}
	d=o.o;
	if(d&&(((o.f&8)==0)||o.i==0))
	{
		if((o.o+=0.06)>=1.0){d=1.0;o.o=null;}
		s.opacity=d;
		s.filter="progid:DXImageTransform.Microsoft.Alpha(opacity="+(d*100)+")";
	}
	if(o.o||o.panH)return;
	window.clearInterval(o.fn);
	delete o.fn;
	delete iged_all._ip;
	if(o.h){s.height=o.oldH;if(o.overF)s.overflow=o.overF;}
}
function iged_mainEvt(e)
{
	if(!e)if((e=window.event)==null)return;
	var o=iged_all._pop,src=e.target;
	if(!src)if((src=e.srcElement)==null)src=this;
	var ee=src,t=src.ownerDocument;
	if(o)while(ee)
	{
		if(ee==o||ee==iged_all._clr)
		{
			if(e.keyCode==27){iged_cancelEvt(e);iged_closePop();}
			return;
		}
		ee=ee.parentNode;
	}
	ee=src;
	var inElem=iged_getById(ee);
	if(t)t=t.id;if(!t)t=src.id;
	
	if(!t)try{t=src.document||src.ownerDocument;t=t.id;}catch(ex){t=null;}
	o=null;
	if(t&&t.indexOf("ig_d_")==0)o=iged_getById(t.substring(5));
	if(o){iged_all._curImg=src.nodeName=="IMG"?src:null;}
	else while((ee=ee.parentNode)!=null)try
	{
		if(iged_getById(ee))inElem=true;
		if(ee.getAttribute)if((o=ee.getAttribute("ig_id"))!=null)
		if((o=iged_getById(o))!=null)break;
	}
	catch(i){}
	if(!o)return;
	var elem=src,d=(new Date()).getTime(),lastE=o._lastE;
	if(lastE&&lastE.d==d&&lastE.t==e.type&&lastE.e==src)return;
	o._lastE={d:d,t:e.type,e:src};
	while(elem){if(o._elem==elem){src=elem;break;}elem=elem.parentNode;}
	if(o._ie)o._ie();
	if(o._zero)o._edit(true);
	switch(e.type)
	{
		case "paste":o._onPaste();o._validate();return;
		case "keydown":t=3;o._focs=1;o._validate();break;
		case "keypress":t=4;o._validate();break;
		case "focus":t=5;if(o._foc>0)o._focs=1;break;
	    case "blur": t = 6; o._validate(1); break;
        // K.D. June 9th, 2014 Bug #171827 The context menu shows only after a keydown event
		case "contextmenu":t=7;break;
		case "mousedown":t=8;iged_all._click=src.onclick?o:null;
			
			if(o._foc==-1)o._foc=0;if(o._foc==1)o._foc=2;
			if(o._elem==src)o._focs=1;
			o._click=d;if(!o._ed0)return;break;
		default:return;
	}
	if(t<7)
	{
		var k=e.keyCode;
		if(!k||k==0)k=e.which;
		if(iged_all._pop){iged_cancelEvt(e);if(k==27)iged_closePop();return;}
		o._evt0=e;
		ee=o._fire(t,k);
		delete o._evt0;
		if(t<5)
		{
			
			o._foc=2;
			if(ee){if(!o._posted)iged_cancelEvt(e);}
			else
			{
				if(!e.ctrlKey&&!e.altKey&&(k=o._evt.key)!=null)e.keyCode=k;
				if(t==3&&o._onKey)o._onKey(e);
			}
			return;
		}
	}
	if(t==7)
	{
		if((t=o._prop[13])!=0)
		{
			iged_cancelEvt(e);
			if(t==2)iged_act("RightClick:pop","","",e,"r");
		}
		return;
	}
	
	if(t==6){o._update(e);o._foc=0;return;}
	if(o._ie)try
	{
		iged_all._curRange=o._range();
		if(!o._2D)o._doc().execCommand("2D-Position",true,o._2D=true);
	}catch(i){}
	if(iged_all._canCloseCur)iged_closePop();
	
	if(t==5){o._mod=true;if(o._foc<1)o._foc+=2;}
	
	if(t==8&&!inElem&&(o._foc<1)&&!(src.unselectable&&iged_all._cur==o)&&src.id!=o._ids[0]&&src.id!=o._ids[1])window.setTimeout("iged_all._cur.focus()",0);
	iged_all._cur=o;
}
function iged_mEvt(e)
{
	if(!e)if((e=window.event)==null)return;
	var m=null,i=0,el=e.target;
	if(!el)if((el=e.srcElement)==null)el=this;
	if(e.type=="mousedown")try
	{
		var o,j=0,ee=el;
		while(!o&&j++<26&&(ee=ee.parentNode)!=null)
			if(ee.getAttribute&&(o=ee.getAttribute("ig_id"))!=null)
				o=iged_getById(o);
		if(o&&o!=iged_all._cur)
		{
			iged_all._cur=o;
			o.focus();
		}
	}catch(ex){}
	while(++i<7&&el)try{if((m=el.mm)!=null)break;el=el.parentNode;}catch(ex){}
	if(!m)return;
	var i0=0,s=el.style,t=e.type.substring(5);
	var im=el.imgs;
	if(im)i=im.length;
	if(t=="over"){i0=3;i=(i<4)?1:3;}
	else if(t=="down"){iged_cancelEvt(e);i0=6;i=2;}
	else if(t=="up"){i0=9;i=(i<4)?1:3;}
	else i=1;
	if(m.length>9)try{eval("s.backgroundColor=m["+i0+"];s.borderColor=m["+(i0+1)+"];s.borderStyle=m["+(i0+2)+"];");}catch(e){}
	if(im)if((e=iged_el(im[0]))!=null)e.src=im[i];
}
function iged_choiceEvt(e)
{
	if(!e)if((e=window.event)==null)return;
	var a,s=0,el=e.target;
	if(!el)if((el=e.srcElement)==null)el=this;
	while(++s<6&&el)try
	{
		a=el.getAttribute;if(a)a=el.getAttribute("igf");if(a=="c"||a=="m"||a=="l")break;
		el=el.parentNode;
	}catch(i){}
	if(s>5)return;
	var p=el.innerHTML,cells=el.parentNode.cells;
	s=el.style;
	switch(e.type)
	{
		case "mouseover":s.backgroundColor=el._b2;
			//get around IE11-bugs related to TD.style.color
			if(iged_all._cur._ieV!=11||!cells)s.color=el._f2;
			//I could not find a fix for table with multiple columns: do not set TD-color at all
			else if(cells.length==1){s.display="none";s.color=el._f2;s.display="block";}
			return;
		case "click":a=(a=="l")?"characterdialog":el._act;if(a&&a!="none")iged_act(a,el,p,"select");//continue to mouseout
		case "mouseout":s.backgroundColor=el._b;s.color=el._f;
	}
}
function iged_clrEvt(e)
{
	if(!e)if((e=window.event)==null)return;
	var el=e.target,o=iged_all._cur;
	if(!el)if((el=e.srcElement)==null)el=this;
	var a=el.getAttribute("c"),s=el.style;
	if(a=="c"||(a&&a.length<2))a=null;
	var c=a?"#90C0C0":el.clr,d=iged_el("iged_c_c0"),f=iged_el("iged_c_c1");
	if(o)try{switch(e.type)
	{
		case "click":if(!a){s.borderColor=c;o._evt0=e;o._onClr(c);delete o._evt0;}
			else
			{
				a=parseInt(a.substring(1))-4;
				if(a<0)a=(a*2-5.5)/7;if(a>0)a=(a*3+1)/15;
				iged_all._clrSelBut.style.color="black";
				s.color="red";iged_all._clrSelBut=el;
				iged_all._clrRGB=a;o._clrInit();
			}return;
		case "mouseout":s.borderColor=c;return;
		case "mouseover":
			if(!a)d.style.backgroundColor=f.value=c;
			s.borderColor="black";
			try{f.focus();}catch(a){}
			return;
		case "keyup":try{d.style.backgroundColor=el.value;o._clrNew=el.value;}catch(a){};return;
		case "keydown":
			var k=e.keyCode;c=o._clrNew;
			if(!k||k==0)k=e.which;
			if(k==27){o._evt0=e;o._onClr("");delete o._evt0;}
			if(k==13){o._evt0=e;o._onClr(c);iged_cancelEvt(e);delete o._evt0;}
			return;
	}}catch(a){}
}
function iged_act(key,p1,p2,p3,p4,p5)
{
	var i,o=iged_all._cur,o1=iged_all._click,act=key.toLowerCase();
	if(!o||!o._valid())return;
	if(o1&&o1!=o){i=(new Date()).getTime();if(i<o1._click+4000&&(!o._click||o._click<o1._click))o=o1;}
	i=act.indexOf(":");
	if(o._prop[12]<1)if(!(act.indexOf('print')==0||act.indexOf('find')==0||act.indexOf('word')==0||act.indexOf('view')>0||act.indexOf('custom')==0||act.indexOf('zoom')==0))return;
	if(i>0){o._key=key=key.substring(0,i);act=act.substring(i+1);if(act=="_0"){act=key;o._post=true;}}
	if(o._fire(1,key,p1,p2,p3,p4,p5,act))return;
	i=o._evt;
	if(i.p4)p1=i.p4;if(i.p5)p2=i.p5;if(i.p6)p3=i.p6;if(i.p7)p4=i.p7;if(i.p8)p5=i.p8;if(i.act)act=i.act;
	switch(act)
	{
		case "fontname":case "fontsize":o._onSelFont(p1,act);break;
		case "fontformatting":o._onSelFont(p1,"formatblock");break;
		case "fontstyle":o._onApplyStyle(p1);break;
		case "insert":o._onInsert(p1);break;
		case "superscript":o._onSubSup(act,"subscript");break;
		case "subscript":o._onSubSup(act,"superscript");break;
		case "bold":case "italic":case "underline":case "strikethrough":
		case "justifyleft":case "justifycenter":case "justifyright":case "justifyfull":
		case "redo":
			o._format(act,"",false,true);break;
		case "print":o.print();break;
		case "undo":if(iged_all._noUndo)return;o._format(act,"",false,true);break;
		case "indent":o._format(act,"",false,true);o._setOl();break;
		case "outdent":o._format(act,"",false,true);o._setOl();break;
		case "removelink":o._format("unlink","",false,true);break;
		case "insertlink":if(o._ie&&!o.hasFocus()){o._alert(o._cs(12));return;}o._onLink();break;
		case "togglepositioning":if(!o._ie){o._alert();return;}
			var sElem=o._getSelElem();
			if(!sElem){o._alert(o._cs(12));return;}
			sElem.style.position=(sElem.style.position=="absolute")?"static":"absolute";
			break;
		case "sendbackward":case "bringforward":
			if(!o._ie){o._alert();return;}
			var sElem=o._getSelElem();
			if(sElem)sElem.style.zIndex+=((act=="sendbackward")?1:-1);
			else{o._alert(o._cs(12));return;}
			break;
		case "toggleborders":o._onToggleBdr();break;
		case "wordcount":
			var txt=iged_replaceS(iged_getEditTxt(),"<BR>"," ");
			txt=iged_replaceS(txt,"<P>","");txt=iged_replaceS(txt,"</P>"," ");
			txt=iged_stripTags(txt);
			txt=iged_replaceS(iged_replaceS(txt,"\n"," "),"\r","");
			txt=iged_replaceS(txt,"&nbsp;"," ");
			var words=0,chars=txt.length,clean=iged_replaceS(txt," ","").length;
			txt=iged_replaceS(txt,"  "," ");
			if(chars>0)
			{
				txt=txt.split(" ");
				for(i=0;i<txt.length;i++)if(txt[i].length>0)words++;
			}
			var t="\t "+o._cs(7)+"\r\n_______________________________\t\r\n\r\n ";
			txt=o._cs(8);if(txt.length<8)txt+="\t";t+=txt+"\t\t"+words+"\r\n "+o._cs(9)+"\t\t"+chars+"\r\n ";
			t+=o._cs(10)+"\t"+clean;
			o._alert(t);
			break;
		case "orderedlist":case "unorderedlist":
			o._fixListFormat();
			o._format("insert"+act,"",false,true);
			iged_all._needSync=true;
			break;
		case "copy":case "cut":case "paste":
			try{if(!o._ie&&window.netscape)netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");}
			catch(i){o._alert();return;}
			o._format(act,"",p1,!p1);
			if(act=="paste")o._onPaste();
			break;
		case "preview":
			var win=window.open(o._prop[7],"_blank","width=800, height=600, location=no, menubar=no, status=no, toolbar=no, scrollbars=yes, resizable=yes",false);
			var doc=o._ie?o._elem:o._body();
			win.document.write("<html><head><title>"+o._cs(0)+"</title></head><body topmargin='0' leftmargin='0'>"+doc.innerHTML+"</body></html>");
			break;
		case "pastehtml":
			try
			{
				var cd=window.clipboardData;
				if(cd)
				{
					cd="<P>"+cd.getData("Text").toString()+"</P>";
					cd=o._cleanWord(cd);
					iged_insText(cd,false,true);
					break;
				}
			}catch(i){}
			o._alert();
			return;
		case "cleanword":
			var old,txt;
			if(o._ie)
			{
				txt=o._cleanWord(old=!o._html?o._elem.innerHTML:o._elem.innerText);
				if(txt!=old)
				{
					var c=o.getCaret();
					o.setText(txt);
					o.setCaret(c);
				}
				break;
			}
			var body=o._body();
			if(!o._html)
			{
				txt=o._cleanWord(old=body.innerHTML);
				if(txt!=old)
					body.innerHTML=txt;
			}
			else
			{
				var html=body.ownerDocument.createRange();
				html.selectNodeContents(body);
				txt=o._cleanWord(old=html.toString());
				if(txt!=old)
				{
					html=document.createTextNode(txt);
					body.innerHTML="";
					body.appendChild(html);
				}
			}
			break;
		case "zoom25":case "zoom50":case "zoom75":case "zoom100":case "zoom200":case "zoom300":case "zoom400":case "zoom500":case "zoom600":
			if(o._ie)o._elem.style.zoom=act.substring(4)+"%";
			else {o._alert();return;}
			iged_closePop("Zoom");
			break;
		case "insertcolumnright":case "insertcolumnleft":o._insCol(act.substring(12));break;
		case "insertrowabove":case "insertrowbelow":o._insRow(act.substring(9));break;
		case "increasecolspan":case "decreasecolspan":o._colSpan(act.substring(0,8));break;
		case "increaserowspan":case "decreaserowspan":o._rowSpan(act.substring(0,8));break;
		case "deleterow":o._delRow();break;
		case "deletecolumn":o._delCol();break;
		case "insertimage":
			var img=o._getSelImg();
			if(img)iged_all._curImg=img;
		case "insertwindowsmedia":case "insertflash":case "upload":case "open":
			iged_closePop();
			o._wait(p4);
			p5=iged_modal(p1,p2,p3,p4);break;
		case "pop":
			var p6=null;
			if(p4=="r")
			{
				p1="iged_0_rcm";p4=3;p6=1;
				if((iged_all._curImg=o._getSelImg())!=null)p6=3;
				else if(o._inTbl())p6=2;
			}
			if(p4=="t")
			{
				var tbl=o._inTbl();
				p4=null;p1=tbl?"iged_0_itm":"iged_0_ito";
				if(o._ie)
				{
					o._elem.setActive();
					iged_all._curRange=o._range();
					iged_all._curMenuCell=iged_all._curMenuTable=iged_all._curMenuRow=null;
					if(tbl)try
					{
						var par=iged_all._curRange.parentElement();
						iged_all._curMenuCell=o._getTag(par,'TD','TH');
						iged_all._curMenuTable=o._getTag(par,'TABLE');
						iged_all._curMenuRow=o._getTag(par,'TR');
					}catch(tbl){}
				}
			}
			if(p1&&p1.length>0)o._pop(p1,p2,p3,p4,p5,p6);
			break;
		case "popwin":if(o._opr&&p2&&p2.indexOf("_0_fr")>0){o._alert();return;}o._findOld=null;o._popWin(p1,p2,p3,p4,p5);break;
		case "ruledialog":o._insRule(p1,p2,p3,p4,p5);break;
		case "bookmarkdialog":o._insBook(p1);break;
		case "characterdialog":iged_insText(p2,true,true);break;
		case "tabledialog":o._insTbl();break;
		case "celldialog":o._cellProp();break;
		case "finddialog":o._find0(1);break;
		case "replacedialog":o._find0(2);break;
		case "replacealldialog":o._find0(3);break;
		case "changeview":o._showHtml(p1);break;
		case "spellcheck":i=(typeof ig_getWebControlById=="function")?ig_getWebControlById(p1):null;
			if(i&&i.checkTextComponent)i.checkTextComponent(o.ID);
			else o._alert(o._cs(13).replace('{0}',p1));
			break;
		default:return;
	}
	o._fire(2,key,p1,p2,p3,p4,p5,act);
	o._validate();
}
function iged_getEditTxt()
{
	var o=iged_all._cur;
	if(!o)return "";
	if(!o._ie)return o._body().innerHTML;
	return o._html?o._elem.innerText:o._decode(o._elem.innerHTML);
}
function iged_imgMouseAct(id,img){id=iged_el(id);if(id)id.src=img;}
function iged_changeSt(elem,clrBk,clrBd,stBd){var s=elem.style;s.backgroundColor=clrBk;s.borderColor=clrBd;s.borderStyle=stBd;}
function iged_moveBack(p)
{
	if(!p)return;
	var m=p._moved,par=p.parentNode;
	p._moved=null;
	if(!m||!par||!par.removeChild)return;
	p.style.display='none';
	par.removeChild(p);
	m.appendChild(p);
}
function iged_closePop(s)
{
	iged_moveBack(iged_all._clr);
	// restore selection before open popup (IE11 bugs)
	var doc=iged_all._doc0,o=iged_all._cur_Drop;
	if(o&&!(o._closeNoClr&&s=="clr"))
	{
		delete iged_all._cur_Drop;
		if(o._elem.parentNode&&o._oldRange&&o.getCaret()<0)
			o._sel().addRange(o._oldRange);
		delete o._oldRange;
	}
	if(doc)if((doc=doc.getElementById("iged_clr0_id"))!=null)doc.style.display="none";
	iged_all._doc0=null;
	if(s=="clr")return;
	var p=iged_all._pop;
	if(!p||p.style.visibility=="hidden")return;
	iged_moveBack(p);
	o=iged_getById(iged_all._popID);
	var foc=s!=null;if(s==3||!s)s="";
	if(o){if(o._fire(1,"ClosePopup",s))if(s)return;o._doValid();}
	p.style.visibility="hidden";
	iged_all._pop=null;
	if(o)o._fire(2,"ClosePopup",s);
	iged_all._canCloseCur=false;
	if(o&&foc)o.focus(1);
}
function iged_stripTags(html) 
{
   html=html.replace(/\n/g, ".$!$.")
   var aScript=html.split(/\/script>/i);
   for(i=0;i<aScript.length;i++)
      aScript[i]=aScript[i].replace(/\<script.+/i,"");
   html=aScript.join("");
   html=html.replace(/\<[^\>]+\>/g,"").replace(/\.\$\!\$\.\r\s*/g,"").replace(/\.\$\!\$\./g,"");
   return html.replace(/\r\ \r/g,"");
} 
function iged_nestCount(elem,tag)
{
	var i=0,id=iged_all._cur._elem.id;
	while(elem&&elem.id!=id)
	{
		if(elem.tagName==tag)i++;
		elem=elem.parentNode;
	}
	return i;
}
function iged_el(id,o)
{
	if(o)
	{
		o=iged_all._dlgs=iged_all._dlgs||{};
		if(o[id])return o[id];
	}
	var e=iged_all._doc0||window.document;
	e=e.getElementById(id);
	if(o&&e)e.parentNode.removeChild(o[id]=e);
	return e;
}
function iged_replaceS(s1,s2,s3,r)
{
	try{return s1.replace(new RegExp(r?s3:s2,'g'),r?s2:s3);}catch(e){}
	if(r){r=s2;s2=s3;s3=r;}
	while(s1.indexOf(s2)>=0)s1=s1.replace(s2,s3);
	return s1;
}
function iged_cancelEvt(e)
{
	if(e==null)if((e=window.event)==null)return;
	if(e.stopPropagation)e.stopPropagation();
	if(e.preventDefault)e.preventDefault();
	e.cancelBubble=true;
	e.returnValue=false;
}
function iged_clearText()
{
	var o=iged_all._cur;if(!o)return;
	if(o._ie)o._elem.innerHTML="";
	else o._elem.contentDocument.clear();
}
function iged_loadCell()
{
	var c=iged_all._curCell,e=iged_el("iged_cp_ha");
	if(!c||!e)return;
	e=e.options;
	iged_el("iged_cp_w").value=c.width;
	iged_el("iged_cp_h").value=c.height;
	iged_el("iged_cp_nw").checked=c.noWrap;
	for(i=0;i<e.length;i++)if(e[i].value==c.align)e[i].selected=true;
	e=iged_el("iged_cp_va").options;
	for(i=0;i<e.length;i++)if(e[i].value==c.vAlign)e[i].selected=true;
	iged_updateClr(c.bgColor,"iged_cp_bk1");
	var bc=c.borderColor;if(!bc)bc=c.getAttribute("bc");
	iged_updateClr(bc?bc:c.getAttribute("bc"),"iged_cp_bd1");
}
function iged_loadTable()
{
	var t=iged_all._curTable,e=iged_el("iged_tp_rr");
	if(!t||!e)return;
	e.disabled=true;e.value=t.rows.length;
	e=iged_el("iged_tp_cc");
	e.disabled=true;e.value=t.rows[0]?t.rows[0].cells.length:0;
	e=iged_el("iged_tp_al").options;
	for(var i=0;i<e.length;i++)if(e[i].value==t.align)e[i].selected=true;
	iged_el("iged_tp_cp").value=t.cellPadding;
	iged_el("iged_tp_cs").value=t.cellSpacing;
	iged_el("iged_tp_w").value=t.width;
	iged_el("iged_tp_bds").value=t.border;
	iged_updateClr(t.bgColor,"iged_tp_bk1");
	var bc=t.borderColor;
	iged_updateClr(bc?bc:t.getAttribute("borderColor"),"iged_tp_bd1");
}
function iged_updateClr(c,id)
{
	if(!c)c="";
	id=iged_el(id);
	id.style.backgroundColor=(c=="")?"#F0F0F0":c;
	id.value=c;
}
function iged_dragEvt(e)
{
	if(!e)if((e=window.event)==null)return;
	var src=e.target,o=iged_all._cur,p=iged_all._pop;
	if(p)p=p.style;else return;
	if(!src)if((src=e.srcElement)==null)src=this;
	if(e.type=="mouseup"){iged_all._dragOn=false;return;}
	if(e.type=="mousemove")
	{
		if(!iged_all._dragOn)return;
		iged_cancelEvt(e);
		var x=iged_all._dragX+e.clientX-iged_all._dragX0,y=iged_all._dragY+e.clientY-iged_all._dragY0;
		if(!isNaN(x))p.marginLeft=x+'px';
		if(!isNaN(y))p.marginTop=y+'px';
		return;
	}
	if(!o||src.id!="titleBar")return;
	iged_all._dragX0=e.clientX;iged_all._dragY0=e.clientY;
	if(isNaN(iged_all._dragX=parseInt(p.marginLeft)))return;
	if(isNaN(iged_all._dragY=parseInt(p.marginTop)))return;
	iged_all._dragOn=true;
	if(iged_all._dragE)return;
	iged_all._dragE=true;
	o._addLsnr(document,"mousemove",iged_dragEvt);
	o._addLsnr(document,"mouseup",iged_dragEvt);
}
function iged_update(e)
{
	var o=null;
	if(e&&e.type=='click')if((o=e.srcElement)==null)o=e.target;
	if(o&&o.type!='submit')return;
	var t1=new Date().getTime(),t=iged_all._lastT;
	if(!t||t+99<t1)for(var i in iged_all)
		if(i!='_cur'&&(o=iged_all[i])!=null)if(o._update)o._update();
	iged_all._lastT=t1;
}
function iged_valid(id){var o=iged_all[id];if(o)o._valid();}


var iged_moz;
function iged_init(ids, p1, p2, p3, p4, p5, ie)
{
    //T.P. 2014 August 28th Bug #176979 In case of async postback, we need to clean the flag so the context menu gets created correctly and contextmenu event is attached correctly
    if (iged_all._onMenu) {
        iged_all._onMenu = null;
    }
	
    var ie10_plus = ie ? document.documentMode : null;
    //T.P. 2014 June 12th. Bug #171163 Last parameter in iged_init_moz is used to replace div container with iFrame and apply the correct css classes. In IE10 there is a div into the editor, so we need the same logic ot replace the div with iFrame. 
    ie10_plus = ie10_plus ? ie10_plus >= 10 : false;
	if (!ie || iged_moz || ie10_plus)
	    return iged_init_moz(ids, p1, p2, p3, p4, p5, ie10_plus);
	ids=ids.split("|");
	var id=ids[0];
	var elem=iged_el(ids[1]),ta=iged_el(id+"_t_a"),tb=iged_el(ids[3]);
	if(!ta)return;
	var o=new iged_new(id,ta,tb,p1,p2,p3,p4,p5);
	o._ids=[ids[4]+"_d",ids[4]+"_h"];
	o._elem=elem;
	o._ie=function(flag)
	{
		var e0=o._elem0;
		if(!e0)return;
		//init
		if(flag==1&&tb)try
		{
			var i,img=tb.getElementsByTagName('IMG');
			i=img?img.length:0;
			if(!i)
			{
				img=iged_el(id+"_ts");
				if(img)img=img.getElementsByTagName('IMG');
				i=img?img.length:0;
			}
			if(i>0)
			{
				img=img[i-1];
				var e_t=function(e)
				{
					img.detachEvent("onload",e_t);
					setTimeout(function(){o._ie(3);},300);
				};
				img.attachEvent("onload",e_t);
			}
		}catch(ex){}
		// N.I 2/19/2014 Bug:164051-The _td_ height is not getting set correctly for small 'px' values
		if(((flag&&flag>1)||(o._ieV&&o._ieV<11))&&!o._htSet)try
		{
			var h=e0.offsetHeight,div=elem.parentNode;
			if(!h&&div)
			{
				//first paint
				if(!o._tmr)o._tmr=setInterval(function(){o._ie(2);},200);
				return;
			}
			if(o._tmr)clearInterval(o._tmr);
			delete o._tmr;
			if(!div)return;
			var td=iged_el(id+"_td_"),val=iged_el(id+"_ts");
			val=val?val.offsetHeight:0;
			val+=tb?tb.offsetHeight:0;
			val=(h-val)*100/h+0.2;
			h=parseFloat(td.height);
			if(val>h)td.height=val+"%";
			o._htSet=1;
			//vert scrollbar bugs
			div.style.width=div.parentNode.clientWidth+'px';
			elem.style.width='auto';
			if(o._ieV==8)
			{
				val=parseFloat(elem.style.height);
				elem.style.height=(50+(!val||isNaN(val)?85:val)/2)+'%';
			}
		}catch(ex){}
	}
	o._doc=function(){return this._elem.ownerDocument||this._elem.document;}
	o._sel=function(){return this._doc().selection;}
	o._range=function(){return this._sel().createRange();}
	o.getText=function(){return this._html?this._elem.innerText:this._elem.innerHTML;}
	o._onLink=function()
	{
		var r=this._sel();
		//IE9:exception CreateLink+Control, and wrong href for empty range
		try{r=(r&&r.type!="Control")?r.createRange().htmlText:null;}catch(ex){r=null;}
		if(r)this._format("CreateLink");
		else this._alert(this._cs(12));
	}
	o.print=function()
	{
		var s=this._elem.outerHTML;
		var i=s.indexOf(" style=");
		if(i>3)
		{
			var s0=s.substring(i+7,i+8);
			s=s.substring(i+8);
			s=s.substring(0,s.indexOf(s0));
		}
		var win=window.open("","","width=10,height=10");
		win.document.write("<html><body style='"+s+"'>"+this._elem.innerHTML+"</body></html>");
		win.document.close();
		win.print();
		win.close();
	}
	o._inTbl=function()
	{
		var cr=iged_all._curRange=this._range();
		if(!cr||this._sel().type=="Control"||!cr.parentElement)return false;
		return this._getTag(cr.parentElement(),'TD','TH');
	}
	o._insBook=function(n){iged_insText("<a name='"+n+"'>"+(this._bmTxt||'')+"</a>",false,true,true);}
	o._insRule=function(align,w,clr,size,noShad)
	{
		var t="<hr";
		if(align&&align!="default")t+=" align='"+align+"'";
		if(w&&w!="")t+=" width='"+w+"'";
		if(clr&&clr!="")t+=" color='"+clr+"'";
		if(size&&size!="")t+=" size='"+size+"'";
		if(noShad)t+=" NOSHADE";
		t+=" />";
		iged_insText(t,false,true,true);
	}
	o.getSelectedText=function()
	{
		try{var r=this._sel();if(r&&r.type!="Control")return r.createRange().text;}catch(ex){}
		return '';
	}
	o._onApplyStyle=function(e)
	{
		var str=e.id||'',cr=iged_all._curRange;
		if(!cr||!cr.select)return;
		var txt=cr.htmlText;
		if(!txt)
		{
			cr.expand("word");
			txt=cr.htmlText;
			if(!txt)return;
		}
		txt=txt.replace(/\n/g, "").replace(/\r/g, "");
		if(str.indexOf(":")>1)str="<font style='"+str+"'>"+txt+"</font>";
		else if(str.length>1)str="<font class='"+str+"'>"+txt+"</font>";
		this._format("removeFormat");
		if(str.length>1)iged_insText(str,false,true);
		this._afterSel(e);
	}
	o._popWin=function(cap,x,img,evt)
	{
		x=this._fixDlgS(x);
		var id=x[0],h=x[14],w=x[15],flag=x[16];
		if(id.length<1)return;
		if(flag=="t")if((iged_all._curTable=this._getCurTable())==null)return;
		if(flag=="c")if((iged_all._curCell=this._getCurCell())==null)return;
		if(this._isKnown(id))iged_all._curRange=this._range();
		this._bmTxt=(id=='iged_0_bm')?this.getSelectedText():'';
		iged_closePop();
		var elem=iged_el(id,1),pan=this._pan();
		if(!elem||!pan)return;
		this._choiceAct=this._itemStyle=null;
		if(!elem._igf){elem._igf=true;this._fixPop(elem);}
		if(x[1])pan.className=x[1];
		var s=pan.style;
		if(x[2])s.backgroundColor=x[2];if(x[3])s.borderColor=x[3];
		if(x[4])s.borderStyle=x[4];if(x[5])s.borderWidth=x[5];
		if(x[6])s.fontFamily=x[6];if(x[7])s.fontSize=x[7];if(x[8])s.color=x[8];
		if(h)s.height=h;if(w)s.width=w;
		var tbl=document.createElement("TABLE");
		tbl.border=tbl.cellPadding=tbl.cellSpacing=0;
		tbl.insertRow();tbl.insertRow();
		if(w)tbl.style.width=w;
		var r0=tbl.rows[0];
		r0.insertCell();
		tbl.rows[1].insertCell();
		var c0=r0.cells[0];var s0=c0.style;
		s0.width="100%";s0.cursor="pointer";
		c0.id="titleBar";
		if(x[10])s0.backgroundColor=x[10];
		var txt="<table border='0' cellpadding='2' cellspacing='0' width='100%'><tr><td id='titleBar' width='100%'>";
		if(img!="")txt+="<img id='titleBar' align='absmiddle' src='"+img+"'></img>";
		txt+="&nbsp;<b id='titleBar' style='";
		if(x[11])txt+="font-family:"+x[11]+";";
		if(x[12])txt+="font-size:"+x[12]+";";
		if(x[13])txt+="color:"+x[13]+";";
		txt+="'>"+cap+"</b></td><td>";
		if(x[9])txt+="<img onclick='iged_closePop(3)' align='absmiddle' src='"+x[9]+"'></img>";
		txt+="</td></tr></table>";
		c0.innerHTML=txt;
		tbl.rows[1].cells[0].innerHTML=elem.innerHTML;
		pan.innerHTML=tbl.outerHTML;
		iged_all._pop=pan;
		iged_all._popID=this.ID;
		this._pos(evt,pan,9);
		this._delay();
		if(flag=="c")iged_loadCell();
		if(flag=="t")iged_loadTable();
		if(!pan._mde){this._addLsnr(pan,"mousedown",iged_dragEvt);pan._mde=true;}
	}
	o._decode=function(t){return t.replace(/<!--###@@@/gi, "<").replace(/@@@###-->/gi, ">");}
	o._encode=function(txt)
	{
		var exp=/<form(.*?)>/i;
		do{txt=txt.replace(exp,"@@@&&&###").replace(/@@@&&&###/,"<!--###@@@form"+RegExp.$1+"@@@###-->");}
		while(exp.test(txt));
		txt=txt.replace(/<\/form>/gi,"<!--###@@@/form@@@###-->");
		if(txt.indexOf("<!--###@@@")==0)txt="&nbsp;"+txt;
		return txt;
	}
	o._showHtml=function(html)
	{
		var o=this,e=this._elem;
		if(e._html!=null)o._html=e._html;
		if(html!==true&&html!==false)html=o._html!=true;
		if(html==(o._html==true))return;
		e._html=html;
		var tb=o._tb?o._tb.rows:null,i=tb?tb.length:0;
		o._swapS(iged_el(o._ids[html?0:1]),iged_el(o._ids[html?1:0]));
		while(i-- > 0)
			tb[i].style.visibility=html?"hidden":"visible";
		if(html)
		{
			iged_all._noUndo=true;
			o._html=true;
			o.setText(o._amp(o._elem.innerHTML),1);
			o.focus();
			return;
		}
		o._html=false;
		o.setText(o._elem.innerText,1);
		o.focus();
		o._syncBullets();
	}
	
	o._find0=function(x)
	{
		var txt=iged_el('iged_f_fw').value,txt2=iged_el('iged_f_rw').value||'';
		if(!txt)return;
		var flag=(iged_el('iged_f_mc').checked?4:0)^(iged_el('iged_f_mw').checked?2:0);
		if(x==1)return this._find(txt,flag);
		this._findOld=null;
		this._findNum=0;
		while(this._find(txt,flag,txt2))if(x==2)return;
	}
	o._find=function(txt,flag,txt2)
	{
		this._delay();
		var s,old=this._findOld==txt,fr=this._findRange;
		if(txt2!=null&&fr)fr.text=txt2;
		this._findOld=txt;
		if(!fr)
		{
			fr=this._findRange=this._body().createTextRange();
			fr.moveToElementText(this._elem);
			this._findLen=fr.text.length;
		}
		else fr.collapse(false);
		if(fr.findText(txt,this._findLen,flag))try
		{
			s=fr.parentElement();
			while(s)
			{
				if(s==this._elem){fr.select();this._findNum++;return true;}
				s=s.parentNode;
			}
		}catch(s){}
		txt="\""+txt+"\"";s=15;
		if(!old)this._findOld=null;
		else if(txt2==null)s=16;
		else{s=17;txt=this._findNum;}
		alert(this._cs(s).replace('{0}',txt));
		fr.collapse(false);
		this._findRange=null;
		return false;
	}
	o._txt=function(txt,paste)
	{
		if(!paste)txt=this._encode(txt);
		var i=0,css=new Array();
		while(this._ie7)
		{
			var i0=txt.indexOf('<style ',i),i1=txt.indexOf('<style>',i),i2=txt.indexOf('<STYLE ',i),i3=txt.indexOf('<STYLE>',i);
			if(i>i0||(i1>=i&&i0>i1))i0=i1;if(i>i0||(i2>=i&&i0>i2))i0=i2;if(i>i0||(i3>=i&&i0>i3))i0=i3;
			i1=txt.indexOf('>',i0);
			if(i>i0||i0>i1)break;
			i2=txt.indexOf('</style>',i0);i3=txt.indexOf('</STYLE>',i0);
			if(i1>i2||(i3>i1&&i2>i3))i2=i3;
			if(i1>i2)break;
			css[css.length]=txt.substring(i1+1,i2);
			txt=txt.substring(0,i0)+txt.substring(i2+8);
			i=i0;
		}
		var x,ii,e=this._elem,vv=new Array(),n="<a ",nx=" href",q="_$$_",t0=txt,t=txt.toLowerCase();
		while(true)
		{
			i=t.length-1;
			while(i>2)
			{
				if((ii=t.lastIndexOf(n,i))>=0)if((x=this._attrVal(t,ii+1,nx,t0))!=null)
				{
					txt=txt.substring(0,ii+n.length)+q+"='"+vv.length+"' "+txt.substring(ii+n.length);
					vv[vv.length]=x;
				}
				i=ii-1;
			}
			if(n.length>3)break;
			n="<img ";nx=" src";t0=txt;t=txt.toLowerCase();
		}
		if(paste)try{this._range().pasteHTML(txt);}catch(x){return;}
		else e.innerHTML=txt;
		x="A";
		while(true)
		{
			n=e.getElementsByTagName(x);
			for(i=0;i<n.length;i++)
			{
				if((nx=n[i].getAttribute(q))==null)continue;
				try{if((nx=vv[parseInt(nx)])!=null)if(x.length==1){if(n[i].href!=nx)n[i].href=nx;}else if(n[i].src!=nx)n[i].src=nx;}
				catch(ii){}
				n[i].removeAttribute(q);
			}
			if(x.length>1)break;
			x="IMG";
		}
		i=css.length;
		while(i-->0)try
		{
			var s=document.createElement('STYLE');
			e.insertBefore(s,e.firstChild);
			s.type='text/css';
			s.styleSheet.cssText=css[i];
		}catch(ii){}
	}
	o._attrVal=function(t,i0,a,t0)
	{
		var c,s=0,x=a.length,i=i0,i1=t.length;
		while(++i<i1)if(t.charCodeAt(i)==62)break;//62'>'
		if(i==i1)return null;
		t=t.substring(i0,i);t0=t0.substring(i0,i);
		i1=t.length;i=-1;
		while(++i<i1)
		{
			i0=t.indexOf(a,i);
			if(i0<0||i0+x+2>i1)return null;
			i=i0+x;
			while((c=t.charCodeAt(i))==32)if(++i>=i1)return null;
			if(c==61){i++;break;}//61'='
		}
		t=t0.substring(i);
		i1=t.length;i=0;
		while(i<i1&&(c=t.charCodeAt(i))==32)i++;
		if(c==39||c==34)i++;else c=32;//39''',34'"'
		i0=i;
		while(i<i1)
		{
			x=t.charCodeAt(i++);
			if(x==c)i1=--i;
			else if(s==0&&x==32)i0++;else s++;
		}
		if(i1!=i)return null;
		while(i>i0)if(t.charCodeAt(i-1)==32)i--;else break;
		return t.substring(i0,i);
	}	
	o._getSelElem=function()
	{
		if(this._sel().type=="Control")
			return this._range().item(0);
		iged_all._curRange=this._range();
		if(iged_all._curRange.boundingWidth>0)
		{
			var txt=iged_all._curRange.text;
			var id="pwCurrentlySelectedText";
			txt="<span id='"+id+"'>"+txt+"</span>";
			iged_all._curRange.pasteHTML(txt);
			var obj=this._doc().getElementById(id);
			obj.id="";
			return obj;
		}
		return null;
	}
	o._getCurCell=function()
	{
		return iged_all._curMenuCell?iged_all._curMenuCell:this._getTag(iged_all._curRange.parentElement(),'TD','TH');
	}
	o._getCurTable=function()
	{
		return iged_all._curMenuTable?iged_all._curMenuTable:this._getTag(iged_all._curRange.parentElement(),'TABLE');
	}
	o._getCurRow=function()
	{
		return iged_all._curMenuRow?iged_all._curMenuRow:this._getTag(iged_all._curRange.parentElement(),'TR');
	}
	o._insTbl=function()
	{
		var o=this;
		var iRows=iged_el("iged_tp_rr").value,iCols=iged_el("iged_tp_cc").value;
		var align=iged_el("iged_tp_al").value,cellPd=iged_el("iged_tp_cp").value;
		var cellSp=iged_el("iged_tp_cs").value,bdSize=iged_el("iged_tp_bds").value;
		var clrBg=iged_el("iged_tp_bk1").value,clrBd=iged_el("iged_tp_bd1").value,w=iged_el("iged_tp_w").value;
		var i,t,t0=iged_all._curTable;
		if(t0)t=t0;
		else
		{
			t=o._doc().createElement("TABLE");
			for(i=0;i<iRows;i++)
			{
				var r=t.insertRow();
				for(j=0;j<iCols;j++)r.insertCell();
			}
		}
		if(align!="default")t.align=align;
		t.border=bdSize;
		t.cellPadding=cellPd;
		t.cellSpacing=cellSp;
		if(clrBg&&clrBg!="")t.bgColor=clrBg;
		if(clrBd&&clrBd!="")t.borderColor=clrBd;
		t.width=w;
		if(t0)t0.outerHTML=t.outerHTML;
		else iged_insText(t.outerHTML,false,true,true);
		iged_all._curTable=null;
		iged_closePop();
	}
	o._delCol=function()
	{
		var c=this._getCurCell(),t=this._getCurTable();
		var i,ii=c.cellIndex;
		if(t&&c)for(i=0;i<t.rows.length;i++)if(t.rows[i].cells.length>ii)
			t.rows[i].deleteCell(ii);
		iged_closePop();
	}
	o._delRow=function()
	{
		var r=this._getCurRow(),t=this._getCurTable();
		if(t&&r)t.deleteRow(r.rowIndex);
		iged_closePop();
	}
	o._insRow=function(act)
	{
		var r=this._getCurRow(),t=this._getCurTable();
		iged_closePop();
		if(!t||!r)return;
		var i,r2=t.insertRow(r.rowIndex+((act=="below")?1:0));
		for(i=0;i<r.cells.length;i++)r2.insertCell(i);
	}
	o._insCol=function(act)
	{
		var c=this._getCurCell(),t=this._getCurTable();
		iged_closePop();
		if(!t||!c)return;
		var i,t2=t.cloneNode(true);
		for(i=0;i<t2.rows.length;i++)
			t2.rows[i].insertCell(c.cellIndex+((act=="right")?1:0));
		t.outerHTML=t2.outerHTML;
	}
	o._colSpan=function(act)
	{
		var c=this._getCurCell(),r=this._getCurRow();
		iged_closePop();
		if(!c||!r)return;
		if(act=="increase")
		{
			if(r.cells[c.cellIndex+1])
			{
				c.colSpan+=1;
				r.deleteCell(c.cellIndex+1);
			}
		}
		else if(c.colSpan>1) 
		{
			c.colSpan-=1;
			r.insertCell(c.cellIndex);
		}
	}
	o._rowSpan=function(act)
	{
		var c=this._getCurCell(),r=this._getCurRow(),t=this._getCurTable();
		iged_closePop();
		if(!c||!r)return;
		var nextRow=null;
		if(t.rows.length>r.rowIndex)nextRow=t.rows[r.rowIndex+c.rowSpan];
		if(act=="increase")
		{
			if(!nextRow)return;
			c.rowSpan+=1;
			nextRow.deleteCell(c.cellIndex);
		}
		else
		{
			if(c.rowSpan==1)return;
			c.rowSpan-=1;
			nextRow=t.rows[r.rowIndex+c.rowSpan];
			nextRow.insertCell(c.cellIndex);
		}
	}
	o._onKey=function(e)
	{
		var k=e.keyCode,p=(this._prop[12]&2)!=0;
		if(p&&e.shiftKey&&k==13)
		{
			iged_insText("<P />", false, false);
			iged_cancelEvt(e);
			return;
		}
		if(k==9){iged_insText("&nbsp;&nbsp; ", false, false);iged_cancelEvt(e);}
		if(k==13&&iged_all._needSync)window.setTimeout("iged_all._cur._syncBullets()", 100);
		if(k>=37&&k<=40)iged_all._curRange=this._body().createTextRange();
		if(!p)return;
		if(k==13)
		{
			this._syncBullets();
			var range=this._range();
			if(range.queryCommandState("insertorderedlist")||range.queryCommandState("insertunorderedlist"))
			{
				if(!iged_all._terminateList)iged_all._terminateList=true;
				else
				{
					e.keyCode=8;
					iged_all._terminateList=false;
				}
				return;
			}
			iged_insText("<BR />",false,false);
			iged_cancelEvt(e);
			range.select();
			range.collapse(false);
			range.select();
		}
		else iged_all._terminateList=false;
	}
	o.getCaret=function()
	{
		try
		{
			var range=o._range();
			var temp=document.createElement("SPAN");
			temp.style.position="absolute";
			temp.style.zIndex=-1;
			o._elem.insertBefore(temp, o._elem.firstChild);
			var tr=range.duplicate();
			tr.moveToElementText(temp);
			tr.setEndPoint("EndToEnd",range);
			tr=tr.text;
			tr=tr.replace(/\r/g, '').replace(/\n\n/g, ' ');
			o._elem.removeChild(temp);
			return tr.length;
		}
		catch(ex){}
		return -1;
	}
	o.setCaret=function(pos,keepNewLines)
	{
		if(pos<0)return;
		try
		{
			var r=o._range();
			r.moveToElementText(o._elem);
			var t=r.text,t1=t;
			pos=Math.min(pos,t.length);
			if(pos<t.length&&!keepNewLines)
				pos=t.substring(0,pos).replace(/\r/g, '').replace(/\n\n/g, ' ').length;
			r.move('character', pos);
			r.select();
		}catch(ex){}
	}
	o.insertAtCaret=function(o)
	{if(o)iged_insText(o.outerHTML?o.outerHTML:''+o,null,null,null,this);}
	ta=o._fixStr(ta.value);
	var s=elem.style;
	var w=(ta.length>20)?elem.offsetWidth:0,sw=s.width;
	if(w>10)s.width=w+"px";
	o.setText(ta,1);
	if(w>10)s.width=sw;
	var edit=o._prop[12];
	if(!elem._oldE)
	{
		elem._oldE=true;
		o._addLsnr(iged_el(id),"mousedown",iged_mainEvt);
		if(edit>0)
		{
		o._ed0=true;
		o._addLsnr(elem,"paste",iged_mainEvt);
		o._addLsnr(elem,"focus",iged_mainEvt);
		o._addLsnr(elem,"blur",iged_mainEvt);
		elem.contentEditable=true;
		s.layout="layout-grid: both fixed 12px 12px";
		o._addLsnr(iged_el(id),"keydown",iged_mainEvt);
		o._addLsnr(elem,"keypress",iged_mainEvt);
		o._addMenu();
		}
	}
	o._ie(1);
	elem.content=false;
	if(o._ie7)
	{
		s.position='';
		var timer=setInterval(function()
		{
			sw=elem.parentNode;
			if(!sw||elem.offsetWidth>10)
			{
				clearInterval(timer);
				if(sw)s.position='relative';
			}
		}, 200);
	}
	if(o._prop[10]==1)o._showHtml(true);
	else if((edit&4)!=0)o.focus();
	o._fire(0);
}
function iged_fixValids()
{
	for(var i=0;i<document.all.length;i++)
	{
		var e=document.all[i];
		if(e.href&&e.href.indexOf("Page_ClientValidate()")>-1)
			e.href="javascript:iged_update(1);"+e.href;
		if(e.onclick&&e.onclick.toString().indexOf("Page_ClientValidate()")>-1)
		{
			var v=e.onclick.toString().replace("function anonymous()","").replace("{","").replace("}","");
			v=iged_replaceS(v,"\r","");
			iged_all._validFunc=iged_replaceS(v,"\n","");
			e.onclick=iged_doValidsSubmit;
		}
	}
}
function iged_doValidsSubmit(){iged_update(1);try{eval(iged_all._validFunc);}catch(i){}}
function iged_insText(txt,strip,restore,sel,a4)
{
	
	if(iged_moz)
		return iged_insText_moz(txt, strip, restore, sel, a4);
	var o=a4?a4:iged_all._cur,cr=iged_all._curRange;
	if(!txt||txt==""||!o||!cr)return;
	var t=o.hasFocus()?o._elem.parentNode.scrollTop:0,h=o._elem.scrollHeight;
	if(!(cr.offsetLeft==12&&cr.offsetTop==17))if(restore)
	{
		if(cr.boundingWidth > 0||sel)cr.select();
		if(cr.boundingWidth==null)cr.select();
	}
	if(o._sel().type=="Control")o._sel().clear();
	iged_closePop(a4?null:"InsertText");
	o._elem.setActive();
	if(strip)txt=iged_stripTags(txt);
	o._txt(txt,true);
	o._mod=true;
	if(t>0)setTimeout('var o=iged_all._cur;if(o)o._elem.parentNode.scrollTop=Math.max('+t+'+o._elem.scrollHeight-'+h+',0)',0);
}
function iged_doImgUpdate(txt)
{
	
	if(iged_moz)
		return iged_doImgUpdate_moz(url, h, w, evt);
	var o=iged_all._cur;
	if(o&&iged_all._curRange)try
	{
		iged_all._curRange.select();
		o._sel().clear();
		iged_insText(txt,false,false);
		iged_all._curImg=null;
	}catch(o){}
}
function iged_modal(url,h,w,evt)
{
	
	if(iged_moz)
		return iged_modal_moz(url, h, w, evt);
	var str="";
	url=iged_replaceS(url,"&amp;","&");
	if(h)str+="dialogHeight:"+h+";";
	if(w&&w!="500px")str+="dialogWidth:"+w+";";
	str+="dialogLeft:200;dialogTop:150;scroll:no;status:no;help:no;center:no;";
	
	url+=((url.indexOf("?")>-1)?"&num=":"?num=")+Math.round(Math.random()*100000000);
	url+="&parentId="+iged_all._cur._elem.id;
	return window.showModalDialog(url,window,str);
}




function iged_init_moz(ids, p1, p2, p3, p4, p5, div)
{
	iged_moz = true;
	ids=ids.split("|");
	var id=ids[0];
	var divStyle, td, elem = iged_el(ids[1]), ta = iged_el(id + "_t_a"), tb = iged_el(ids[3]);
	if (!ta || !elem)
		return;
	
	if (div)
	{
		div = elem;
		divStyle = div.style;
		elem = document.createElement("IFRAME");
		elem.width = elem.height = "100%";
		elem.frameBorder = "0";
		div = div.parentNode;
		td = div.parentNode;
		td.removeChild(div);
		elem.id = ids[1];
		td.insertBefore(elem, td.firstChild);
	}
	var o=new iged_new(id,ta,tb,p1,p2,p3,p4,p5);
	o._ids=[ids[4]+"_d",ids[4]+"_h"];
	o._elem=elem;
	o._cDoc=function(){return this._elem.contentDocument;}
	o._edit=function(edit)
	{
		if(this._elem.offsetWidth==0){this._zero=edit;return;}
		if(edit||this._zero)try{this._zero=false;this._cDoc().designMode="On";}catch(i){}
	}
	var doc=o._cDoc(),edit=o._prop[12];
	o._edit(edit>0);
	doc.open();
	var s=ta.value;
	doc.write(o._fixStr(s));
	doc.close();
	
	var i=(s.indexOf('%3Cbody')==0)?s.indexOf('%3E'):0,i1=s.lastIndexOf('%3C/body%3E');
	if(i>4&&i1>i)ta.value=s.substring(i+3,i1);
	o.getText=function()
	{
		var body=this._body();
		if(!this._html)return body.innerHTML;
		var html=body.ownerDocument.createRange();
		html.selectNodeContents(body);
		return html.toString();
	}
	o._win=function(){return this._elem.contentWindow;}
	o._doc=function(){return this._win().document;}
	o._sel=function(){return this._win().getSelection();}
	o._range=function()
	{
		var sel=this._sel();
		try{return sel.rangeCount?sel.getRangeAt(0):null;}catch(ex){}
	}
	o.print=function(){this._win().print();}
	o._onLink=function()
	{
		var s=prompt(this._cs(18)+"\n"+this._cs(19)+"\n"+this._cs(20), "http://");
		if(s)this._format("CreateLink",s,false);
	}
	
	o._inTbl=function(){return this._getTag(this._cont(),'TD','TH');}
	o._insBook=function(n)
	{
		var b=this._doc().createElement("A");
		b.setAttribute("name",n);
		b.innerHTML=this._bmTxt||'';
		iged_insNodeAtSel(b);
	}
	o._insRule=function(align,w,clr,size,noShad)
	{
		var t=this._doc().createElement("HR");
		if(align&&align!="default")t.setAttribute("align",align);
		if(w&&w!="")t.setAttribute("width",w);
		if(clr)t.setAttribute("color",clr);
		if(size&&size!="")t.setAttribute("size",size);
		iged_insNodeAtSel(t);
	}
	o.getSelectedText=function()
	{
		var range=this._range();
		return range?range.toString():'';
	}
	o._removeDummy=function(e)
	{
		var n = e ? e.nodeName : null;
		//Opera
		if(n == '#text' && !e.nodeValue)
		{
			e = e.nextSibling;
			n = e ? e.nodeName : null;
		}
		//SPAN-Firefox, DIV-Chrome, P-Opera
		var span = n == 'SPAN';
		if (!span && n != 'DIV' && n != 'P')
			return;
		var txt = e.innerHTML.replace(/\n/g, "").replace(/\r/g, "");
		//<br>:IE11, <p></p>:Opera
		if(txt && txt != ' ' && txt != '<br>' && txt != '<br/>' && txt != '<p></p>')
			return;
		if(span || (!span && !e.getAttribute('style')))
			e.parentNode.removeChild(e);
	}
	o._onApplyStyle=function(e)
	{
		var str=e.id,range=this._range();
		if(!range)
			return;
		if(!str || str.length < 2)
		{
			this._format("removeFormat");
			return this._afterSel(e);
		}
		var count, i = -1, n = document.createElement("SPAN");
		// all selected elements append to n
		try
		{
			var nodes = range.extractContents ? range.extractContents().childNodes : null;
			count = nodes ? nodes.length : 0;
			// same element was selected
			if(count == 1 && nodes[0].nodeName == "SPAN")
				n = nodes[0].cloneNode(true);
			else while(++i < count)
				n.appendChild(nodes[i].cloneNode(true));
		}
		catch(ex)
		{
			n = document.createElement("SPAN");
			n.innerHTML = iged_stripTags(this._range().toString());
			count = 0;
		}
		if(str.indexOf(":")>1)n.setAttribute("style",str);
		else n.className=str;
		iged_insNodeAtSel(n);
		//try to remove dummy elements
		if(count)
		{
			this._removeDummy(n.previousSibling);
			this._removeDummy(n.nextSibling);
			this._removeDummy(n.firstChild);
			this._removeDummy(n.lastChild);
		}
		this._afterSel(e);
	}
	o._popWin=function(cap,x,img,evt)
	{
		x=this._fixDlgS(x);
		var cont,id=x[0],h=x[14],w=x[15],flag=x[16];
		if(id.length<1)return;
		if(flag=="c")
		{
			if((cont=this._cont())==null)return;
			if((iged_all._curCell=this._inTbl())==null)return;
		}
		if(flag=="t")
		{
			if((cont=this._cont())==null)return;
			if((iged_all._curTable=this._getTag(cont,'TABLE'))==null)return;
		}
		this._bmTxt=(id=='iged_0_bm')?this.getSelectedText():'';
		iged_closePop();
		var elem=iged_el(id,1),pan=this._pan();
		if(!elem||!pan)return;
		this._choiceAct=this._itemStyle=null;
		if(!elem._igf){elem._igf=true;this._fixPop(elem);}
		if(x[1])pan.className=x[1];
		var s=pan.style;
		if(x[2])s.backgroundColor=x[2];if(x[3])s.borderColor=x[3];
		if(x[4])s.borderStyle=x[4];if(x[5])s.borderWidth=x[5];
		if(x[6])s.fontFamily=x[6];if(x[7])s.fontSize=x[7];if(x[8])s.color=x[8];
		if(h)s.height=h;if(w)s.width=w;
		var tbl=document.createElement("TABLE");
		tbl.cellSpacing=0;
		tbl.insertRow(0);tbl.insertRow(0);
		if(w)tbl.style.width=w;
		var r0=tbl.rows[0];
		r0.insertCell(0);tbl.rows[1].insertCell(0);
		var c0=r0.cells[0];var s0=c0.style;
		s0.width="100%";s0.cursor="pointer";
		c0.id="titleBar";
		if(x[10])s0.backgroundColor=x[10];
		var txt="<table border='0' cellpadding='1' cellspacing='0' width='100%'><tr><td id='titleBar' width='100%'>";
		if(img)txt+="<img id='titleBar' align='absmiddle' src='"+img+"'></img>";
		txt+="&nbsp;<b id='titleBar' style='";
		if(x[11])txt+="font-family:"+x[11]+";";
		if(x[12])txt+="font-size:"+x[12]+";";
		if(x[13])txt+="color:"+x[13]+";";
		txt+="'>"+cap+"</b></td><td>";
		if(x[9])txt+="<img onclick='iged_closePop(3)' align='absmiddle' src='"+x[9]+"'></img>";
		txt+="</td></tr></table>";
		c0.innerHTML=txt;
		tbl.rows[1].cells[0].innerHTML=elem.innerHTML;
		pan.innerHTML=iged_getOuterHtml(tbl);
		iged_all._pop=pan;
		iged_all._popID=this.ID;
		this._pos(evt,pan,9);
		this._delay();
		if(flag=="c")iged_loadCell();
		if(flag=="t")iged_loadTable();
		if(!pan._mde){this._addLsnr(pan,"mousedown",iged_dragEvt);pan._mde=true;}
	}
	o._showHtml=function(html)
	{
		var o=this,e=this._elem;
		if(e._html!=null)o._html=e._html;
		if(html!==true&&html!==false)html=o._html!=true;
		if(html==(o._html==true))return;
		e._html=html;
		var tb=o._tb?o._tb.rows:null,i=tb?tb.length:0,body=o._doc().body;
		o._swapS(iged_el(o._ids[html?0:1]),iged_el(o._ids[html?1:0]));
		while(i-- > 0)
			tb[i].style.visibility=html?"hidden":"visible";
		if(html)
		{
			o._html=true;
			html=body.innerHTML;
			body.innerHTML="";
			var lower=html?html.toLowerCase():"";
			if(lower&&lower!="<br>"&&lower!="<br/>"&&lower!="<div><br></div>"&&lower!="<div><br/></div>")
			{
				html=document.createTextNode(o._amp(html));
				body.appendChild(html); 
			}
			o.focus();
			return;
		}
		o._html=false;
		var html=body.ownerDocument.createRange();
		html.selectNodeContents(body);
		body.innerHTML=html.toString(); 
		o.focus();
	}
	o._cont=function(){var r=this._range();return r?r.startContainer:null;}
	o._getSelElem=function()
	{
		var cont=this._cont();
		return (cont.nodeType==1)?cont:null;
	}
	o._insTbl=function()
	{
		var iRows=iged_el("iged_tp_rr").value,iCols=iged_el("iged_tp_cc").value;
		var align=iged_el("iged_tp_al").value,cellPd=iged_el("iged_tp_cp").value;
		var cellSp=iged_el("iged_tp_cs").value,bdSize=iged_el("iged_tp_bds").value;
		var clrBg=iged_el("iged_tp_bk1").value,clrBd=iged_el("iged_tp_bd1").value,w=iged_el("iged_tp_w").value;
		var t,t0=iged_all._curTable;
		if(t0)t=t0;
		else
		{
			t=this._doc().createElement("TABLE");
			var i,tbody=this._doc().createElement("TBODY");
			t.appendChild(tbody);
			for(i=0;i<iRows;i++)
			{
				var r2=this._doc().createElement("TR");
				for(j=0;j<iCols;j++)
				{
					var c2=this._doc().createElement("TD");
					var br=this._doc().createElement("BR");
					c2.appendChild(br);
					r2.appendChild(c2);
				}
				tbody.appendChild(r2);
			}
		}
		if(align!="default")t.setAttribute("align",align);
		t.setAttribute("border",bdSize);
		t.setAttribute("cellpadding",cellPd);
		t.setAttribute("cellspacing",cellSp);
		t.setAttribute("bgcolor",clrBg);
		t.setAttribute("bordercolor",clrBd);
		t.setAttribute("width",w);
		iged_all._curTable=null;
		iged_closePop();
		iged_insNodeAtSel(t);
	}
	o._delCol=function()
	{
		var cont=this._cont();
		var i,c=this._inTbl(),t=this._getTag(cont,'TABLE');
		if(t&&c)for(i=0;i<t.rows.length;i++)t.rows[i].deleteCell(c.cellIndex);
		iged_closePop();
	}
	o._delRow=function()
	{
		var cont=this._cont();
		var r=this._getTag(cont,'TR'),t=this._getTag(cont,'TABLE');
		if(t&&r)t.deleteRow(r.rowIndex);
		iged_closePop();
	}
	o._insRow=function(act)
	{
		var cont=this._cont();
		var r=this._getTag(cont,'TR'),t=this._getTag(cont,'TABLE');
		iged_closePop();
		if(!t||!r)return;
		var i,r2=t.insertRow(r.rowIndex+((act=="below")?1:0));
		for(i=0;i<r.cells.length;i++)
		{
			var c2=r2.insertCell(i);
			if(c2)c2.appendChild(this._doc().createElement("BR"));
		}
	}
	o._insCol=function(act)
	{
		var cont=this._cont();
		var c=this._inTbl(),t=this._getTag(cont,'TABLE');
		iged_closePop();
		if(!t||!c)return;
		var i,ii=c.cellIndex+((act=="right")?1:0);
		for(i=0;i<t.rows.length;i++)
		{
			var c2=t.rows[i].insertCell(ii);
			if(c2)c2.appendChild(this._doc().createElement("BR"));
		}
	}
	o._colSpan=function(act)
	{
		var cont=this._cont();
		var c=this._inTbl(),t=this._getTag(cont,'TABLE'),r=this._getTag(cont,'TR');
		iged_closePop();
		if(!c||!r)return;
		if(act=="increase")
		{
			if(r.cells[c.cellIndex+1])
			{
				c.colSpan+=1;
				r.deleteCell(c.cellIndex+1);
			}
		}
		else if(c.colSpan>1) 
		{
			c.colSpan-=1;
			var c2=r.insertCell(c.cellIndex);
			if(c2)c2.appendChild(this._doc().createElement("BR"));
		}
	}
	o._rowSpan=function(act)
	{
		var r2=null,cont=this._cont();
		var c=this._inTbl(),t=this._getTag(cont,'TABLE'),r=this._getTag(cont,'TR');
		iged_closePop();
		if(!c||!r)return;
		if(t.rows.length>r.rowIndex)r2=t.rows[r.rowIndex+c.rowSpan];
		if(act=="increase")
		{
			if(!r2)return;
			c.rowSpan+=1;
			r2.deleteCell(c.cellIndex);
		}
		else
		{
			if(c.rowSpan==1)return;
			c.rowSpan-=1;
			r2=t.rows[r.rowIndex+c.rowSpan];
			var c2=r2.insertCell(c.cellIndex);
			if(c2)c2.appendChild(this._doc().createElement("BR"));
		}
	}
	o.getCaret=function()
	{
		var range=o._range();
		return range?range.endOffset:-1;
	}
//	o.setCaret=function(pos)
//	{
//		if(pos<0)return;
//		try
//		{
//			var sel=this._sel(),doc=this._cDoc(),cont=doc.body;
//			pos=Math.min(pos,cont.innerHTML.length);
//			if(this._saf)cont=sel.getRangeAt(0).startContainer;
//			var range=doc.createRange();
//			sel.removeAllRanges();
//			range.setStart(cont,pos);
//			range.setEnd(cont,pos);
//			sel.addRange(range);
//			this.focus();
//		}catch(ex){}
//	}
	o.insertAtCaret=function(o)
	{
		if(!o)return;
		if(typeof o=='string')iged_insText(o,null,null,null,this);
		else iged_insNodeAtSel(o,this);
	}
	
	o._find0=function(x)
	{
		var s=null,txt=iged_el('iged_f_fw').value,txt2=iged_el('iged_f_rw').value||'',
			mc=iged_el('iged_f_mc').checked,up=iged_el('iged_f_mw'),doc=this._cDoc(),win=this._win();
		
		if(!win.find){this._alert();return;}
		if(!txt||(x>1&&!doc))return;
		if(x==1)this._findOld=null;
		this._findNum=0;
		if(this._findEnd)
			up.checked=false;
		up=up.checked;
		
		if(x==3||this._findEnd) try
		{
			var sel=this._sel(),range=doc.createRange();
			sel.removeAllRanges();
			range.setStart(doc.body,0);
			range.setEnd(doc.body,0);
			sel.addRange(range);
			up=false;
		}catch(e){}
		delete this._findEnd;
		while(!s)
		{
			if(!this._findOld)
			{
				if(win.find(txt,mc,up))
					this._findOld=txt;
				else
				{
					s=14+x;
					if(x<3)txt='"'+txt+'"';
					else txt=this._findNum;
				}
			}
			if(x==1||!this._findOld||this._findOld!=txt)break;
			iged_insNodeAtSel(doc.createTextNode(txt2),this,1);
			this._findNum++;
			this._findOld=null;
			if(x==2&&win.find(txt,mc,up))
			{
				this._findOld=txt;
				break;
			}
		}
		if(s)alert(this._cs(s).replace('{0}',this._findEnd=txt));
	}
	
	o._htSet=function()
	{
		var e0=o._elem0,h=e0?e0.style.height:null,td=iged_el(id+"_td_"),val=iged_el(id+"_ts"),iframetw=iged_el(id+"_tw");
		if(td&&e0&&tb&&h&&h.indexOf('px')>0)try
		{
			var h0=e0.offsetHeight;
			if(!h0)
			{
				//first paint
				if(!o._tmr)o._tmr=setInterval(function(){o._htSet();},200);
				return;
			}
			delete o._htSet;
			val=val?val.offsetHeight:0;
			val+=tb?tb.offsetHeight:0;
			h=parseFloat(h);
			// N.I 2/19/2014 Bug:164051-The td and iframe heights are not getting set correctly for small 'px' values
			var newHt=h>val?h-val:2;
			if(val+td.offsetHeight>h)td.height=newHt+"px";
			if(val+iframetw.offsetHeight>h) iframetw.height=newHt+"px";
		}catch(ex){}
		if(o._tmr)clearInterval(o._tmr);
		delete o._tmr;
	}
	o._htSet();
	var d=o._doc();
	if(!elem._oldE)
	{
		elem._oldE=true;
		o._addLsnr(iged_el(id),"mousedown",iged_mainEvt);
		if(edit>0)
		{
		o._ed0=true;
		o._addLsnr(d,"mousedown",iged_mainEvt);
		o._addLsnr(d,"focus",iged_mainEvt);
		o._addLsnr(d,"blur",iged_mainEvt);
		o._addLsnr(o._win(),"focus",iged_mainEvt);
		o._addLsnr(o._win(),"blur",iged_mainEvt);
		o._addLsnr(d,"keydown",iged_mainEvt);
		o._addLsnr(d,"keypress",iged_mainEvt);
		o._addLsnr(d,"paste",iged_mainEvt);
		o._addMenu();
		}
	}
	d.id="ig_d_"+id;
	
	if (div)
	{
		var bk = divStyle.backgroundColor, clr = divStyle.color;
		var bStyle = d.body.style;
		bStyle.margin = "0px";
		bStyle.fontFamily = divStyle.fontFamily;
		bStyle.fontSize = divStyle.fontSize;
		while((!bk || !clr) && td)
		{
			bk = bk || td.style.backgroundColor;
			clr = clr || td.style.color;
			if(td == o._elem0)
				break;
			td = td.parentNode;
		}
		bStyle.backgroundColor = bk || "white";
		bStyle.color = clr || "black";
	}
	if(o._prop[10]==1)o._showHtml(true);
	else if((edit&4)!=0)o.focus();
	o._fire(0);
}
function iged_insNodeAtSel(insertNode,o,fr)
{
	
	if(!fr)iged_closePop(o?null:3);
	o=o||iged_all._cur;
	var sel=o._sel(),range=o._range();
	if(!fr)sel.removeAllRanges();
	if(range)try{range.deleteContents();}catch(e){}
	var src,afterNode,cont=range.startContainer,pos=range.startOffset;
	range=o._doc().createRange();
	try
	{
		if(cont.nodeType==3&&insertNode.nodeType==3)
		{
			cont.insertData(pos,insertNode.nodeValue);
			range.setEnd(cont,pos+insertNode.length);
			range.setStart(cont,pos+insertNode.length);
		}
		else
		{
			src=o._chrome&&insertNode.nodeName=="IMG"?insertNode.src:null;
			if(cont.nodeType==3)
			{
				var textNode=cont;
				cont=textNode.parentNode;
				var text=textNode.nodeValue;
				var textBefore=text.substr(0,pos),textAfter=text.substr(pos);
				var beforeNode=document.createTextNode(textBefore);
				var afterNode=document.createTextNode(textAfter);
				cont.insertBefore(afterNode,textNode);
				cont.insertBefore(insertNode,afterNode);
				cont.insertBefore(beforeNode,insertNode);
				cont.removeChild(textNode);
			}
			else
			{
				afterNode=cont.childNodes[pos];
				cont.insertBefore(insertNode,afterNode);
			}
			if(afterNode)
			{
				range.setEnd(afterNode,0);
				range.setStart(afterNode,0);
			}
		}
	}catch(e){}
	sel.addRange(range);
	o._mod=true;
	
	if(src)setTimeout(function(){try
	{
		insertNode.src='';
		insertNode.src=src;
	}catch(e){}},0);
}
function iged_insText_moz(txt,strip,a2,a3,a4)
{
	iged_closePop(a4?null:"InsertText");
	if(strip)txt=iged_stripTags(txt);
	var o=a4?a4:iged_all._cur;
	if(o)if(o=o._cDoc())iged_insNodeAtSel(o.createTextNode(txt));
}
function iged_doImgUpdate_moz(img)
{
	var range=iged_all._cur._range();
	if(range)range.extractContents();
	if(img)iged_insNodeAtSel(img);
	iged_all._curImg=null;
}
function iged_modal_moz(url,h,w,evt)
{
	url=iged_replaceS(url,"&amp;","&");
	var str="";
	if(h)str+="Height="+h+",";
	if(w)str+="Width="+w+",";
	str+="scrollbars=no";
	url+=((url.indexOf("?")>-1)?"&num=":"?num=")+Math.round(Math.random()*100000000);
	url+="&parentId="+iged_all._cur._elem.id;
	return window.open(url,window,str);
}
function iged_getOuterHtml(n) 
{
	var i,html="";
	if(n.nodeType==Node.ELEMENT_NODE)
	{
		html+="<";
		html+= n.nodeName;
		if(n.nodeName!="TEXTAREA") 
		{
			for(i=0;i<n.attributes.length;i++)
				html+=" "+n.attributes[i].nodeName.toUpperCase()+"=\""+n.attributes[i].nodeValue+"\">";
			var s=n.nodeName;
			if(s!="HR"&&s!="BR"&&s!="IMG"&&s!="INPUT")
				html+=n.innerHTML+"<\/"+s+">";
		}
		else
		{
			var txt="";
			for(i=0;i<n.attributes.length;i++)
			{
				if(n.attributes[i].nodeName.toLowerCase()!="value")
					html+=" "+n.attributes[i].nodeName.toUpperCase()+"=\""+ n.attributes[i].nodeValue+"\"";
				else txt=n.attributes[i].nodeValue;
			}
			html+=">"+txt+"<\/"+n.nodeName+">";
		}
	}
	else if(n.nodeType==Node.TEXT_NODE)html+=n.nodeValue;
	else if(n.nodeType==Node.COMMENT_NODE)html+="<!--"+n.nodeValue+"-->";
	return html;
}
