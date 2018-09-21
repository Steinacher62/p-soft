var dataSource;
var folderId;
var type;


Sys.Application.add_load(function () {
    setTimeout(setfolderIdAtStart, 100);
    dataform1._element.classList.add("NoAuthorDozent");
    dataform1._element.classList.add("NoDatum");
    dataform1._element.classList.add("Root");
    type = "SEMINAR";
});

$(document).ready(function () {
    ContentResized();
});

$(window).resize(function () {
    ContentResized();
});

function ContentResized() {
    var contentHeight = $(document).height();
    $('.CellFileExplorer').css('height', contentHeight - 200 + 'px');
    $('#dataForms').css('height', contentHeight -200 + 'px');
    $('#ctl00_ContentPlaceHolder1_FileExplorer').css('height', contentHeight - 200 + 'px');
    var grid = $find('ctl00_ContentPlaceHolder1_FileExplorer').get_grid(); 
    $('ctl00_ContentPlaceHolder1_FileExplorer_grid_GridData').css('height', contentHeight - 250 + 'px');
    $find('ctl00_ContentPlaceHolder1_FileExplorer').get_grid().get_element().style.height = contentHeight - 263 + 'px';
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_FileExplorer_paneTree').css('height', contentHeight - 263 + 'px');
    $('#ctl00_ContentPlaceHolder1_FileExplorer_splitter').css('height', contentHeight - 263 + 'px')
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_FileExplorer_paneGrid').css('height', contentHeight - 263 + 'px');
    $('#ctl00_ContentPlaceHolder1_FileExplorer_grid_GridData').css('height', contentHeight - 263 + 'px')
    grid.repaint();
}


function setfolderIdAtStart() {
    while (folderId == null) {
        try {
            dataSource = $find('ContentPlaceHolder1_ClientDataSourceDataForm');

            if ($("#ContentPlaceHolder1_seminarUID")[0].value > "") {
                folderId = $("#ContentPlaceHolder1_seminarUID")[0].value;
            }
            else {
                folderId = dataSource.getItemByIndex(0).UID;
            }
        } catch (err) {
            //console.log(err);
            setTimeout(setfolderIdAtStart(), 200);
            return;
        }
    }
    setMenues();
}

function CBSeminarsIndexChanged(sender, args) {
    window.location.href = window.location.origin + window.location.pathname + "?SEMINAR=" + args._item._control._value;
}

function FileExplorerClientLoad(explorer, args) {
    explorer.get_fileList().get_grid().add_rowDragStarted(RowDragStarted)
}

function RowDragStarted(grid, args) {
    if (type == "FOLDER" || type == "SEMINAR") {
        args.set_cancel(true);
    }
    
}

function DataFormSeminarSetValues(data) {
    $telerik.findElement(document, "ViewUID").innerHTML = data[0].UID;
    $telerik.findElement(document, "ViewFolderText").innerHTML = data[0].FOLDER;
    $telerik.findElement(document, "ViewTitleText").innerHTML = data[0].TITLE;
    $telerik.findElement(document, "ViewDescriptionText").innerHTML = data[0].DESCRIPTION;
    $telerik.findElement(document, "ViewAuthorText").innerHTML = data[0].AUTHOR;
    $telerik.findElement(document, "ViewDozentText").innerHTML = data[0].DOZENT;
    $telerik.findElement(document, "ViewDatumText").innerHTML = data[0].DATUM;
    var date = "";
    if (data[0].RELEASE instanceof Date) {
        date = data[0].RELEASE.toLocaleString();
    }
    $telerik.findElement(document, "ViewReleaseText").innerHTML = date;
    $telerik.findElement(document, "EditUID").innerHTML = data[0].UID;
    $telerik.findElement(document, "EditFolderText").innerHTML = data[0].FOLDER;
    $telerik.findControl(document, "EditTitleTextBox").set_value(data[0].TITLE);
    $telerik.findControl(document, "EditDescriptionTextBox").set_value(data[0].DESCRIPTION);
    $telerik.findControl(document, "EditAuthorTextBox").set_value(data[0].AUTHOR);
    $telerik.findControl(document, "EditDozentTextBox").set_value(data[0].DOZENT);
    $telerik.findControl(document, "EditDatumTextBox").set_value(data[0].DATUM);
    $telerik.findControl(document, "EditReleaseTextBox").set_value(data[0].RELEASE);

}

function DataFormSeminarCreated(sender, args) {
    sender._clientDataKeyNames = "UID";
    window.dataform1 = sender;
}

function ParameterMap(sender, args) {
    if (args.get_type() != "read" && args.get_data()) {
        args.set_parameterFormat({ userJSON: kendo.stringify(sender._schema.model) });
    }
}

function Parse(sender, args) {
    var response = args.get_response().d;
    if (response) {
        args.set_parsedData(JSON.parse(response));
    }
}

function DataFormSaveData(sender, args) {

    if (args.get_commandName() == "Update") {
        params = getDataDataForm();
        var formData = $.toJSON(params);
        $.ajax({
            type: "POST",
            url: "../WebService/SBSService.asmx/UpdateData",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (Data, TextStatus, XHR) {
                $("#Edit").hide();
                $("#View").show();
            },
            error: function (XHR, TextDescription, ErrorThrown) {
                var responseText = JSON.parse(XHR.responseText);

            }
        });
    }
}

function getDataDataForm() {
    params = {};
    params.UID = folderId;
    params.ID = dataSource.getDataItemById(folderId).ID;
    params.URL = dataSource.getDataItemById(folderId).URL;
    params.TITLE = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditTitleTextBox").value;
    params.DESCRIPTION = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditDescriptionTextBox").value;
    params.SEMINAR_ID = dataSource.getDataItemById(folderId).SEMINAR_ID;
    params.PARENT_ID = dataSource.getDataItemById(folderId).PARENT_ID;
    params.AUTHOR = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditAuthorTextBox").value;
    params.DOZENT = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditDozentTextBox").value;
    params.FOLDER_ID = dataSource.getDataItemById(folderId).FOLDER_ID;
    params.DOCUMENT_ID = dataSource.getDataItemById(folderId).DOCUMENT_ID;
    params.DATUM = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditDatumTextBox").value;
    params.RELEASE = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctl02_EditReleaseTextBox").value;
    if (params.SEMINAR_ID == null) {
        params.SEMINAR_ID = 0;
    }
    if (params.PARENT_ID == null) {
        params.PARENT_ID = 0;
    }
    if (params.FOLDER_ID == null) {
        params.FOLDER_ID = 0;
    }
    if (params.DOCUMENT_ID == null) {
        params.DOCUMENT_ID = 0;
    }


    return params;
}

function EditButton_Click(sender, args) {
    dataform1.editItem(dataSource.get_currentPageIndex());
}

function DataFormOnSetValues(sender, args) {
    var dataItem = dataform1._dataSource[0];
    $telerik.findElement(document, "ViewUID").innerHTML = dataItem.UID;
    $telerik.findElement(document, "ViewFolderText").innerHTML = dataItem.NAME;
    $telerik.findElement(document, "ViewTitleText").innerHTML = dataItem.TITLE;
    $telerik.findElement(document, "ViewDescriptionText").innerHTML = dataItem.DESCRIPTION;
    $telerik.findElement(document, "ViewAuthorText").innerHTML = dataItem.AUTHOR;
    $telerik.findElement(document, "ViewDozentText").innerHTML = dataItem.DOZENT;
    $telerik.findElement(document, "ViewDatumText").innerHTML = dataItem.DATUM;
    var date = "";
    if (dataItem.RELEASE instanceof Date) {
        date = dataItem.RELEASE.toLocaleString();
    }
    $telerik.findElement(document, "ViewReleaseText").innerHTML = date;
    $telerik.findElement(document, "EditUID").innerHTML = dataItem.UID;
    $telerik.findElement(document, "EditFolderText").innerHTML = dataItem.NAME;
    $telerik.findControl(document, "EditTitleTextBox").set_value(dataItem.TITLE);
    $telerik.findControl(document, "EditDescriptionTextBox").set_value(dataItem.DESCRIPTION);
    $telerik.findControl(document, "EditAuthorTextBox").set_value(dataItem.AUTHOR);
    $telerik.findControl(document, "EditDozentTextBox").set_value(dataItem.DOZENT);
    $telerik.findControl(document, "EditDatumTextBox").set_value(dataItem.DATUM);
    $telerik.findControl(document, "EditReleaseTextBox").set_value(dataItem.RELEASE);
}

function DataFormOnGetValues(sender, args) {
    if (args.get_commandName() == "Update") {
        args.set_dataItem(getDataDataForm());
        var data = getDataDataForm();
        var date = null;
        var dateParts = data.RELEASE.split(" ")[0].split(".");
        if (dateParts.length > 1) {

            var timeParts = data.RELEASE.split(" ")[1].split(":");
            date = new Date(dateParts[2], (dateParts[1] - 1), dateParts[0], timeParts[0], timeParts[1]);
        }
        dataSource.update({ "UID": data.UID, "ID": data.ID, "URL": data.URL, "TITLE": data.TITLE, "DESCRIPTION": data.DESCRIPTION, "SEMINAR_ID": data.SEMINAR_ID, "PARENT_ID": data.PARENT_ID, "AUTHOR": data.AUTHOR, "DOZENT": data.DOZENT, "FOLDER_ID": data.FOLDER_ID, "DOCUMENT_ID": data.DOCUMENT_ID, "DATUM": data.DATUM, "RELEASE": date }, folderId);
        dataform1._bindClientDataSource();
        DataFormSaveData(sender, args);
    }

}

function DataUpdateClickedCancel(sender, args) {
    dataform1.cancelUpdate();
}

function FileExplorer_ItemSelected(sender, args) {
    FileExplorerFolderChange(sender, args);
}

function FileExplorerNewFolder(sender, args) {
    params = {};
    params.newFolder = args._newPath;
    if (args._item !== null) {
        params.parentFolder = args._item._name;
    }
    else {
        params.parentFolder = args._newPath;
    }
    if (args._item !== null) {
        params.path = args._item._path;
    }
    else {
        params.path = args._newPath;
    }

    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/AddNewFolder",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            dataSource.add(JSON.parse(Data.d));

        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);
            alert('Error:' + responseText.d);
        }
    });
}

function FileExplorerFolderChange(sender, args) {
    var type = SetFolderIdTyp(args._item._name, args._item.get_path(), args._item.isDirectory());
    var isSubFolder = args._item.get_path().split("/").length > 5;
    var isRoot = args._item.get_path().split("/").length === 2 && type === "SEMINAR";
    FileExplorerFolderChangeSetData(type, isSubFolder, isRoot);
}

function FileExplorerFolderChangeSetData(type, isSubFolder, isRoot) {
    var title;
    switch (type) {
        case "SEMINAR":
            title = "Seminar";
            break;
        case "FOLDER":
            title = "Ordner";
            break;
        case "DOCUMENT":
            title = "Dokument";

            break;
        default:
            title = "Seminare";
    }
    $(".sbsHeaderInner").text(title);
    var datasourceData;
    try {
        datasourceData = dataSource.getDataItemById(folderId);
    } catch (err) {
        params = {};
        params.folderId = folderId;
        var formData = $.toJSON(params);
        $.ajax({
            type: "POST",
            url: "../WebService/SBSService.asmx/GetItem",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (Data, TextStatus, XHR) {
                dataSource.add(JSON.parse(Data.d));
                datasourceData = dataSource.getDataItemById(folderId);
            },
            error: function (XHR, TextDescription, ErrorThrown) {
                var responseText = JSON.parse(XHR.responseText);
                alert('Error:' + responseText.d);
            }
        });
    }
    dataform1.set_dataSource([datasourceData]);

    dataform1._currentPageIndex = dataSource._kendoDataSource.indexOf(dataSource._kendoDataSource.get(folderId));
    DataFormSeminarSetValues(dataform1._dataSource);
    dataform1.dataBind();

    if (type == "FOLDER" || type == "SEMINAR") {
        dataform1._element.classList.add("NoAuthorDozent");
    } else {
        dataform1._element.classList.remove("NoAuthorDozent");
    }

    if (type == "DOCUMENT" || type == "SEMINAR" || type == "FOLDER" && isSubFolder) {
        dataform1._element.classList.add("NoDatum");
    } else {
        dataform1._element.classList.remove("NoDatum");
    }
    if (isRoot) {
        dataform1._element.classList.add("Root");
    } else {
        dataform1._element.classList.remove("Root"); 
    }
}

function SetFolderIdTyp(folder, path, isDirectory) {
    params = {};
    params.folder = folder;
    params.path = path;
    params.isDirectory = isDirectory;

    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/GetIdTyp",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            folderId = JSON.parse(Data.d).id;
            type = JSON.parse(Data.d).type;
        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);
            alert('Error:' + responseText.d);
        }
    });
    return type;
}

function FileExplorerFolderLoaded(sender, args) {
    setMenues();
}

function setMenues() {
    var toolBar = $find("ctl00_ContentPlaceHolder1_FileExplorer").get_toolbar();
    var fileExplorer = $find("ctl00_ContentPlaceHolder1_FileExplorer");
    var contextMenu = fileExplorer.get_gridContextMenu();
    var treeView = fileExplorer.get_tree();
    var menus = treeView.get_contextMenus();
    if (dataSource.getDataItemById(folderId).PARENT_ID == 0 || dataSource.getDataItemById(folderId).ROOT_ID == null) {
        toolBar._findItemByValue("Upload").set_enabled(false);
        toolBar._findItemByValue("NewFolder").set_enabled(false);
        toolBar._findItemByValue("Delete").set_enabled(false);
        if (dataSource.getDataItemById(folderId).ROOT_ID == null) {
            toolBar._findItemByValue("Neues Seminar").set_enabled(true);
        }
        else {
            toolBar._findItemByValue("Neues Seminar").set_enabled(false);
            toolBar._findItemByValue("NewFolder").set_enabled(true);
            toolBar._findItemByValue("Delete").set_enabled(true);
        }

        hideMenu("Delete", contextMenu);
        hideMenu("NewFolder", contextMenu);
        hideMenu("Upload", contextMenu);
        hideMenu("Rename", contextMenu);
        for (var i in menus) {
            hideMenu("Delete", menus[i]);
            hideMenu("NewFolder", menus[i]);
            hideMenu("Upload", menus[i]);
            hideMenu("Rename", menus[i]);
        }

        if (dataSource.getDataItemById(folderId).ROOT_ID != null) {
            showMenu("Delete", contextMenu);
            showMenu("NewFolder", contextMenu);
            showMenu("Rename", contextMenu);
            for (var i in menus) {
                showMenu("Delete", menus[i]);
                showMenu("NewFolder", menus[i]);
                showMenu("Rename", menus[i]);
            }
        }
    }
    else {
        toolBar._findItemByValue("Upload").set_enabled(true);
        toolBar._findItemByValue("NewFolder").set_enabled(true);
        toolBar._findItemByValue("Delete").set_enabled(true);
        toolBar._findItemByValue("Neues Seminar").set_enabled(false);

        showMenu("Delete", contextMenu);
        showMenu("NewFolder", contextMenu);
        showMenu("Upload", contextMenu);
        showMenu("Rename", contextMenu);
        for (var i in menus) {
            showMenu("Delete", menus[i]);
            showMenu("NewFolder", menus[i]);
            showMenu("Upload", menus[i]);
            showMenu("Rename", menus[i]);
        }

    }
}

function hideMenu(text, contextMenu) {
    var menuItem = contextMenu.findItemByValue(text);
    menuItem.hide();
}

function showMenu(text, contextMenu) {
    var menuItem = contextMenu.findItemByValue(text);
    menuItem.show();
}

function FileExplorerContextMenueShowing(sender, args) {
    args.get_node().select();
    var item = $find('ctl00_ContentPlaceHolder1_FileExplorer').get_selectedItem();
    
    var type = SetFolderIdTyp(item.get_name(), item.get_path(), item.isDirectory());
    var isSubFolder = item.get_path().split("/").length > 5;
    var isRoot = item.get_path().split("/").length == 3 && type == "SEMINAR";
    FileExplorerFolderChangeSetData(type, isSubFolder, isRoot);
}

function FileExplorerFileAdded(sender, args) {

    for (var i = 0; i < sender.getUploadedFiles().length; i++) {
        var params = {};
        params.fileName = sender.getUploadedFiles()[i];
        params.path = $find("ctl00_ContentPlaceHolder1_FileExplorer").get_currentDirectory();
        var formData = $.toJSON(params);

        $.ajax({
            type: "POST",
            url: "../WebService/SBSService.asmx/FileUploaded",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (Data, TextStatus, XHR) {
                dataSource.add(JSON.parse(Data.d.substr(0, Data.d.length - 1).substring(1)));
            },
            error: function (XHR, TextDescription, ErrorThrown) {
                var responseText = JSON.parse(XHR.responseText);
            }
        });
    }
}

function FileExplorerFileAddedCancel(sender, args) {
    //if ($("#RadWindowWrapper_ctl00_ContentPlaceHolder1_FileExplorer_windowManagerfileExplorerUpload").css("visibility") == "hidden") {
    //    //args.set_cancel(true);
    //}
    //else {
    //    var params = {};
    //    params.fileName = args.get_fileName();
    //    var formData = $.toJSON(params);

    //    $.ajax({
    //        type: "POST",
    //        url: "../WebService/SBSService.asmx/FileExplorerFileAddedCancel",
    //        cache: false,
    //        async: false,
    //        dataType: 'json',
    //        data: formData,
    //        contentType: 'application/json; charset=utf-8',
    //        success: function (Data, TextStatus, XHR) {

    //        },
    //        error: function (XHR, TextDescription, ErrorThrown) {
    //            var responseText = JSON.parse(XHR.responseText);

    //        }
    //    });
    //}
}

function FileExplorerItemDelete(sender, args) {
    var params = {};
    params.path = args._item._path;
    var formData = $.toJSON(params);

    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/FileExplorerDelete",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            dataSource.remove(folderId);
            var ret = JSON.parse(Data.d);
            folderId = ret.parentUid;
            typ = ret.typ;
            window.location.reload();
        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

function FileExplorerButtonClicked(sender, args) {
    if (args._item.get_commandName() == 'newSeminarClicked') {
        $find("ctl00_ContentPlaceHolder1_WindowNewSeminar").show();
    }
    if (args._item.get_commandName() == 'SeminarPreviewClicked') {
        // $find("ctl00_ContentPlaceHolder1_WindowStartpage").show();
        PopupCenter(window.location.href.split("Seminars.aspx")[0] + "SBSSeminarView.aspx?seminarID="+ $("#ContentPlaceHolder1_seminarId")[0].value, "Vorschau", 1200, 570);
        // window.open(, "_blank", "toolbar=no, scrollbars=yes, resizable=yes, location=no, status=no, titlebar=no, width = 1200, height = 570, left="+(screen.width/2)-600+", top="+screen.height/2+285);
    }
}

function PopupCenter(url, title, w, h) {
    // Fixes dual-screen position                         Most browsers      Firefox
    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
    var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

    width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    var left = ((width / 2) - (w / 2)) + dualScreenLeft;
    var top = ((height / 2) - (h / 2)) + dualScreenTop;
    var newWindow = window.open(url, title, 'resizable=yes, location=no, status=no, titlebar=no, scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

    // Puts focus on the newWindow
    if (window.focus) {
        newWindow.focus();
    }
}

function ButtonSaveSeminarClicked() {
    $find("ctl00_ContentPlaceHolder1_WindowNewSeminar").close();
    var newSeminar = $("#ctl00_ContentPlaceHolder1_WindowNewSeminar_C_TbNewSeminar")[0].value;
    //$find("ctl00_ContentPlaceHolder1_FileExplorer").createNewDirectory($find("ctl00_ContentPlaceHolder1_FileExplorer").get_currentDirectory(), newSeminar);

    params = {};
    params.newSeminarName = newSeminar;

    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/addNewSeminar",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            seminarId = Data.d; //JSON object mit eigenschaft "d"
            window.location.href = window.location.origin + window.location.pathname + "?SEMINAR=" + seminarId;

        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

function ButtonUpdateSeminarClicked() {
    params = {};
    params.id = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctrl0_SeminarID").innerHTML;
    params.title = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctrl0_ViewSeminarTitleTextBox").value;
    params.description = $get("ctl00_ContentPlaceHolder1_DataFormSeminar_ctrl0_ViewSeminarDescriptionTextBox").value;

    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/SaveSeminar",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            seminarData = Data.d; //JSON object mit eigenschaft "d"
        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

function ButtonCancelNewSeminarClick() {
    $find("ctl00_ContentPlaceHolder1_WindowNewSeminar").close();
}

function FileExplorerItemMove(sender, args) {
    //check if the event is fired by Rename command
    if (args.get_newPath().search("/") == 0) {
        if (type == "SEMINAR") {
            args._cancel = true;
        }
        else {
            params = {};
            params.oldPath = args.get_path();
            params.newPath = args._newPath;
            params.isDirectory = args.get_item().isDirectory();
            params.itemName = args._item._name;

            var formData = $.toJSON(params);
            $.ajax({
                type: "POST",
                url: "../WebService/SBSService.asmx/FileExplorerItemMove",
                cache: false,
                async: false,
                dataType: 'json',
                data: formData,
                contentType: 'application/json; charset=utf-8',
                success: function (Data, TextStatus, XHR) {
                    seminarData = Data.d; //JSON object mit eigenschaft "d"
                    $.each(Data.d, function (i) {
                        var data = JSON.parse(Data.d)[i];
                        if (data != undefined) {
                            dataSource.update({ "UID": data.UID, "ID": data.ID, "NAME": data.NAME, "URL": data.URL, "FOLDER": data.FOLDER, "TITLE": data.TITLE, "DESCRIPTION": data.DESCRIPTION, "SEMINAR_ID": data.SEMINAR_ID, "PARENT_ID": data.PARENT_ID, "AUTHOR": data.AUTHOR, "DOZENT": data.DOZENT, "FOLDER_ID": data.FOLDER_ID, "DOCUMENT_ID": data.DOCUMENT_ID, "ROOT_ID": data.ROOT_ID }, data.UID);

                        }
                    });

                    //dataform1.dataBind();
                },
                error: function (XHR, TextDescription, ErrorThrown) {
                    var responseText = JSON.parse(XHR.responseText);
                    args._cancel = true;
                    alert(responseText.d);

                }
            });
        }

    }
    else {
        //FileExplorerItemRenamed(sender, args);
    }
}

function FileAdded(sender, args) {

    alert("stop");
}

function FileExplorerItemRenamed(sender, args) {
    //    params = {};
    //    params.path = args.get_path();
    //    params.newName = args._newPath.trim();
    //    params.renamedType = type;

    //    var formData = $.toJSON(params);
    //    $.ajax({
    //        type: "POST",
    //        url: "../WebService/SBSService.asmx/FileExplorerItemRenamed",
    //        cache: false,
    //        async: false,
    //        dataType: 'json',
    //        data: formData,
    //        contentType: 'application/json; charset=utf-8',
    //        success: function (Data, TextStatus, XHR) {
    //            var data = JSON.parse(Data.d);
    //            for (i = 0; i < data.length; i++) {
    //                var dataRec = data[i];
    //                if (dataRec != undefined) {
    //                    dataSource.update({ "UID": dataRec.UID, "ID": dataRec.ID, "NAME": dataRec.NAME, "URL": dataRec.URL, "TITLE": dataRec.TITLE, "DESCRIPTION": dataRec.DESCRIPTION, "SEMINAR_ID": dataRec.SEMINAR_ID, "PARENT_ID": dataRec.PARENT_ID, "AUTHOR": dataRec.AUTHOR, "DOZENT": dataRec.DOZENT, "FOLDER_ID": dataRec.FOLDER_ID, "DOCUMENT_ID": dataRec.DOCUMENT_ID, "ROOT_ID": dataRec.ROOT_ID, "DATUM": dataRec.DATUM }, dataRec.UID);
    //                    //window.location.reload();
    //                }
    //            };
    //            //dataform1._bindClientDataSource();
    //            //dataform1.dataBind();
    //        },
    //        error: function (XHR, TextDescription, ErrorThrown) {
    //            var responseText = JSON.parse(XHR.responseText);
    //            args._cancel = true;
    //            alert(responseText.d);

    //        }
    //    });
    //}
}

function FileExplorerNodeEdited(sender, args) {
    if (args.get_path().length == args.get_path().trim().length) {
        if (existNode(args.get_path(), args._newPath) == false) {
            params = {};
            params.path = args.get_path();
            params.newName = args._newPath;
            params.renamedType = type;

            var formData = $.toJSON(params);
            $.ajax({
                type: "POST",
                url: "../WebService/SBSService.asmx/FileExplorerItemRenamed",
                cache: false,
                async: false,
                dataType: 'json',
                data: formData,
                contentType: 'application/json; charset=utf-8',
                success: function (Data, TextStatus, XHR) {
                    var data = JSON.parse(Data.d);
                    for (i = 0; i < data.length; i++) {
                        var dataRec = data[i];
                        if (dataRec != undefined) {
                            dataSource.update({ "UID": dataRec.UID, "ID": dataRec.ID, "NAME": dataRec.NAME, "URL": dataRec.URL, "TITLE": dataRec.TITLE, "DESCRIPTION": dataRec.DESCRIPTION, "SEMINAR_ID": dataRec.SEMINAR_ID, "PARENT_ID": dataRec.PARENT_ID, "AUTHOR": dataRec.AUTHOR, "DOZENT": dataRec.DOZENT, "FOLDER_ID": dataRec.FOLDER_ID, "DOCUMENT_ID": dataRec.DOCUMENT_ID, "ROOT_ID": dataRec.ROOT_ID, "DATUM": dataRec.DATUM }, dataRec.UID);
                            //window.location.reload();
                        }
                    };
                    //dataform1._bindClientDataSource();
                    //dataform1.dataBind();
                },
                error: function (XHR, TextDescription, ErrorThrown) {
                    var responseText = JSON.parse(XHR.responseText);
                    args._cancel = true;
                    alert(responseText.d);

                }
            });
        }
    }
    else {
        alert("Name kann nicht gespeichert werden! Dieser enthält Leerzeichen Am Anfang oder Schluss des Namens.")
        args.set_cancel(true);
    }

    function existNode(path, newElement) {
        var exist = true;
        var fileExplorer = $find("ctl00_ContentPlaceHolder1_FileExplorer");
        var tree = fileExplorer.get_tree();
        var newPath = path.substr(0, path.lastIndexOf("/") + 1) + newElement;
        if (tree.findNodeByAttribute("Path", newPath) == null) {
            exist = false;
        }
        return exist;
    }
}


