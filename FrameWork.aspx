<%@ Page AspCompat=true language="c#" Codebehind="FrameWork.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.FrameWork" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<TITLE>.: p-soft :.</TITLE>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<meta http-equiv="Content-Type" content="text; charset=iso-8859-1">
		<script>
		    window.name = "p-soft " + window.location.hostname;
		    var noFramesURL = '<%=_noFramesURL%>';
		    var newLocation = self.location;
            if (noFramesURL != ''){
                newLocation = noFramesURL;
            }
		    try{
                if (opener)
                {
                    if (opener.top.name == self.name){
                        opener.top.location = newLocation;
                        window.close();
                    }
                }
                else if (top.location != self.location)
	            {
                    top.location = newLocation;
	            }
            }
            catch(e){
            }

            var highlightColor = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
            var subNavigationEle1 = null;
            var subNavigationEle1ClassName = null;
            var subNavigationEle2 = null;
            var subNavigationEle2ClassName = null;
	        
	        function reloadSubNavigation(pageURL,colorElement,id)
	        {
	            var separator;
	            if (pageURL.indexOf('?') > 0)
	                separator = '&';
	            else
	                separator = '?';
	            if (!id)
	                id = -1;
			    window.frames["subNavFrame"].location.href=pageURL + separator + 'colorItem=' + colorElement + '&id=' + id;
                subNavigationEle1 = null;
                subNavigationEle1ClassName = null;
                subNavigationEle2 = null;
                subNavigationEle2ClassName = null;
            }
 	        function reloadFrame(frame,pageURL,id)
	        {
	            var param = "";
	            if (pageURL.indexOf('?') > 0) param = '&id=';
	            else param = '?id=';
	            if (!id) id = -1;
			    window.frames[frame].location.href=pageURL + param + id;
            }
           
            function highlightSubNavigation(eleId)
            {
                var ele1 = window.frames["subNavFrame"].document.getElementById(eleId);
                var ele2 = null;
                eraseHighlightSubNavigation();
                if (ele1)
                {
                    subNavigationEle1 = ele1;
                    subNavigationEle1ClassName = ele1.className;
                    ele1.className += "_selected";
                    ele2 = ele1.parentElement;
                }
                if (ele2)
                {
                    subNavigationEle2 = ele2;
                    subNavigationEle2ClassName = ele2.className;
                    ele2.className += "_selected";
                }
            }
            
            function eraseHighlightSubNavigation()
            {
                if (subNavigationEle1)
                {
                    subNavigationEle1.className = subNavigationEle1ClassName;
                    subNavigationEle1 = null;
                }
                if (subNavigationEle2)
                {
                    subNavigationEle2.className = subNavigationEle2ClassName;
                    subNavigationEle2 = null;
                }
            }
            
            function highlightModule(module)
            {
                if (window.frames["headerFrame"].HighlightModule)
                    window.frames["headerFrame"].HighlightModule(module);
            }
            
			function reloadBreadcrumbFrame()
	        {
	            window.frames["breadcrumbFrame"].location.reload(true);
	        }
            
            
            var progressBarTimer = null;
            
            function showProgressBarDelayed(timeout)
            {
                if (progressBarTimer == null)
                {
           		    progressBarTimer = setTimeout("showProgressBar()",timeout);
           		}
            }
            
            function showProgressBar()
            {
                if (progressBarTimer != null)
                {
                    if (window.frames["progressFrame"] && window.frames["progressFrame"].StartAnimation)
                    {
                        window.frames["progressFrame"].StartAnimation();
                        window.frames["progressFrame"].frameElement.parentElement.rows="0,*";
                    }
                    clearTimeout(progressBarTimer);
                    progressBarTimer = null;
                }
            }
            
            function hideProgressBar()
            {
                clearTimeout(progressBarTimer);
                progressBarTimer = null;
                if (window.frames["progressFrame"] && window.frames["progressFrame"].StopAnimation)
                {
                    window.frames["progressFrame"].StopAnimation();
                    window.frames["progressFrame"].frameElement.parentElement.rows="*,0";
                }
            }
		</script>
	</HEAD>
	<frameset rows="15,*,15" frameborder="0" framespacing="0">
		<frame name="topFrame" scrolling="no" noresize src="Basics/topBorder.htm" marginwidth="0" marginheight="0">
		<frameset cols="15,*,15" frameborder="0" framespacing="0">
			<frame name="leftFrame" scrolling="no" noresize src="Basics/leftBorder.htm" marginwidth="0" marginheight="0">
			<frameset rows="78,32,1,*" frameborder="0" framespacing="0">
				<frame name="headerFrame" scrolling="no" noresize src="Basics/header.aspx" marginwidth="0" marginheight="0">
				<frame name="breadcrumbFrame" scrolling="no" noresize src="Common/BreadcrumbFrame.aspx" marginwidth="0" marginheight="0">
				<frame name="headerBorderFrame" scrolling="no" noresize src="Basics/centerBorder.htm" marginwidth="0" marginheight="0">
				<frameset cols="161,1,*" frameborder="0" framespacing="0">
					<frameset rows="*,30" frameborder="0" framespacing="0">
						<frame name="subNavFrame" scrolling="yes" noresize src="Basics/leftMenu.aspx" marginwidth="0" marginheight="0">
						<!-- <frame name="subMenuFrame" scrolling="no" noresize src="Basics/subMenu.aspx" marginwidth="0" marginheight="0"> -->
						<frame name="copyrightFrame" scrolling="no" noresize src="Basics/copyright.htm" marginwidth="0" marginheight="0">
					</frameset>
					<frame name="centerFrame" scrolling="no" noresize src="Basics/centerBorder.htm" marginwidth="0" marginheight="0">
					<frameset rows="*,0" frameborder="0" framespacing="0">
						<frame name="contentFrame" noresize src="<%=_contentFrameURL%>" marginwidth="8" marginheight="8">
						<frame name="progressFrame" noresize src="Common/Loading.aspx?autoload=false" marginwidth="8" marginheight="8">
					</frameset>
				</frameset>
			</frameset>
			<frame name="rightFrame" scrolling="no" noresize src="Basics/rightBorder.htm" marginwidth="0" marginheight="0">
		</frameset>
		<frame name="bottomFrame" scrolling="no" noresize src="Basics/bottomBorder.htm" marginwidth="0" marginheight="0">
	</frameset>
</HTML>
