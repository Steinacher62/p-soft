	var webServicePath = "";
	var wsUserMoveComplete = null;
	var wsUserCopyComplete = null;
	var wsUserSetFileProperties = null;
	var wsStartScript = "";

	function callWebservice(funcName, params) {
		var ret;
		$.ajax({
			type: "POST",
			url: webServicePath + "WebService/PsoftService.asmx/" + funcName,
			cache: false,
			async: false,
			contentType: "application/json; charset=utf-8",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data, status) { ret = data; },
			error: function (data, status) {
				alert(xmlRequest.status + ' \n\r ' + xmlRequest.statusText + '\n\r' +xmlRequest.responseText);
			}
		});

		return ret;
	}

	function wsFileProperties(file) {
		var retValue = true;
		
		var params = new Object();
		params.file = file;

		retValue = callWebservice("fileProperties", params);
		if (wsUserSetFileProperties) wsUserSetFileProperties(retValue);
	}
	
	function wsMoveRow(ownerTable,ownerId,sourceTable,sourceId) {
		var retValue = true;

		var params = new Object();
		params.ownerColumn = ownerTable;
		params.newOwnerID = ownerId;
		params.table = sourceTable;
		params.ID = sourceId;

		retValue = callWebservice("move", params);
		if (wsUserMoveComplete) wsUserMoveComplete(retValue);
	}

	function wsCopyRow(ownerTable,ownerId,sourceTable,sourceId,cascade) {
		var retValue = true;

		var params = new Object();
		params.ownerColumn = ownerTable;
		params.newOwnerID = ownerId;
		params.table = sourceTable;
		params.ID = sourceId;
		params.cascade = cascade;

		retValue = callWebservice("copy", params);
		if (wsUserCopyComplete) wsUserCopyComplete(retValue);

	}

	// delete row in db-table
	function wsDeleteRow(tableName,dbId) {
		var retValue = true;
		
		var params = new Object();
		params.tableName = tableName;
		params.id = dbId;
		params.rootEnable = false;

		retValue = callWebservice("deleteTableRow", params);
		
		return retValue;
	}
	
	// delete row in db-table with confirm text
	function wsDeleteRowConfirm(target,backUrl,deleteConfirmMessage,tableName,rowId,dbId) {
		highlightElement('','',rowId);
		if (window.confirm(deleteConfirmMessage)) {
			wsDeleteRow(tableName,dbId);
			target.location.href = backUrl;
		}
		else {
			document.location.reload(true);
			return false;
			
		}
	}

	// delete row in db-table with confirm text and failure url (in case of failure)
	function wsDeleteRowConfirmExt(target, backUrl, errorUrl, deleteConfirmMessage, deleteErrorMessage, tableName,rowId,dbId) {
		highlightElement('','',rowId);
		
		if (window.confirm(deleteConfirmMessage)) {
			if(wsDeleteRow(tableName,dbId)) {
				target.location.href = backUrl;
			} else {
				if(deleteErrorMessage!=="") {
					alert (deleteErrorMessage);
				}
				target.location.href = errorUrl;
			}          
		}
		else {
			document.location.reload(true);
			return false;
			
		}
	}

	function wsCheckOutDocument(documentID)
	{
		var retValue = true;

		var params = new Object();
		params.documentID = documentID;

		retValue = callWebservice("checkOutDocument", params);

		return retValue;
	}

	function wsCheckInDocument(documentID)
	{
		var retValue = true;

		var params = new Object();
		params.documentID = documentID;

		retValue = callWebservice("checkInDocument", params);

		return retValue;
	}

	function wsCheckOutUndoDocument(documentID)
	{
		var retValue = true;

		var params = new Object();
		params.documentID = documentID;

		retValue = callWebservice("checkOutUndoDocument", params);

		return retValue;
	}
	function wsExtendTree(envName,parentId)
	{
		var retValue = true;

		var params = new Object();
		params.envName = envName;
		params.parentId = parentId;

		retValue = callWebservice("extendTree", params);
		if (retValue !== "") {
			window.execScript(retValue, "javascript");
		}
	}

