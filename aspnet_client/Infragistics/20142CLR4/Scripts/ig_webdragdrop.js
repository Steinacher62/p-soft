/*
* ig_webdragdrop.js
* Version 14.2.20142.1028
* Copyright(c) 2001-2014 Infragistics, Inc. All Rights Reserved.
*/


//alert('ig_webdragdrop.js');
ig_ScheduleDragDrop = function(info)
{
	this._info = info;
	this.addView = function(view)
	{
//alert('view:'+view._clientID+':'+this._info._clientID);
	}
}
