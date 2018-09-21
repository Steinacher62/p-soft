var ajaxUrl;

$(document).ready(function () {
    $(".Textbox").on('keydown', function (e, args) {
        var c = e.which;
        if (c === 13) {
            e.preventDefault();
        }
    });

    $('.SaveImageButton').click(function (sender) {
        var buttonId = sender.target.id;
        var image = $find(buttonId).get_imageUrl();

        $find(buttonId).set_imageUrl(image.replace('.gif', 'Saved.gif'));

        setTimeout(function () {
            $find(buttonId).set_imageUrl(image);
        }, 1000);
    });
});

function GetAjaxFolder() {
    $.ajax({
        url: '../WebService/AdminService.asmx/FolderCheck',
        type: 'POST',
        async: false,
        error: function () {
            ajaxUrl = '../../WebService/AdminService.asmx/';
        },
        success: function () {
            ajaxUrl = '../WebService/AdminService.asmx/';
        }
    });
}

function GetDateFromJson(date) {
    date = new Date(parseInt(date.substr(6)));
    return date.getFullYear() + "/" + (date.getMonth() + 1 + "/" + date.getDate());
}

function GetDateDEFromJSON(date) {
    date = new Date(parseInt(date.substr(6)));
    return date.getDate() + "." + (date.getMonth() + 1) + "." + date.getFullYear();
}

function GetCopyTreeNodesFromParent(sourceTree, startNodeValue) {
    var nodeSource = sourceTree.findNodeByValue(startNodeValue);
    var nodesFromParent = nodesFromParent = nodeSource.get_parent().get_allNodes();
    var nodesFromParentTmp = {};
    nodesFromParentTmp.nodes = [];
    index = 0;
    nodesFromParent.forEach(function (node) {
        var nodetmp = new Telerik.Web.UI.RadTreeNode();
        var attributes = node.get_attributes();
        for (i = 0; i < attributes.get_count(); i++) {
            key = attributes._keys[i];
            value = attributes.getAttribute(key);
            nodetmp.get_attributes().setAttribute(key, value);
        }
        nodetmp.get_attributes().setAttribute("PARENTID", node.get_parent().get_value());
        nodetmp.set_value(node.get_value());
        nodetmp.set_imageUrl(node.get_imageElement().src);
        nodetmp.set_text(node.get_text());
        nodesFromParentTmp.nodes[index] = nodetmp;
        index++;
    });
    return nodesFromParentTmp;
}

function GetCopyTreeNodes(sourceTree, startNodeValue) {
    var nodeSource = sourceTree.findNodeByValue(startNodeValue);
    var nodes = nodeSource.get_allNodes();
    var nodesTmp = {};
    nodesTmp.nodes = [];
    index = 0;
    nodes.forEach(function (node) {
        var nodetmp = new Telerik.Web.UI.RadTreeNode();
        var attributes = node.get_attributes();
        for (i = 0; i < attributes.get_count(); i++) {
            key = attributes._keys[i];
            value = attributes.getAttribute(key);
            nodetmp.get_attributes().setAttribute(key, value);
        }
        nodetmp.get_attributes().setAttribute("PARENTID", node.get_parent().get_value());
        nodetmp.set_value(node.get_value());
        nodetmp.set_imageUrl(node.get_imageElement().src);
        nodetmp.set_text(node.get_text());
        nodesTmp.nodes[index] = nodetmp;
        index++;
    });
    return nodesTmp;
}

function GetCopyFolderNodes(sourceTree, folderNode) {
    var nodesSource = sourceTree.findNodeByValue(folderNode).get_nodes();

    var nodesCopyTmp = {};
    nodesCopyTmp.nodes = [];
    index = 0;
    nodesSource.forEach(function (node) {
        var nodetmp = new Telerik.Web.UI.RadTreeNode();
        var attributes = node.get_attributes();
        for (i = 0; i < attributes.get_count(); i++) {
            key = attributes._keys[i];
            value = attributes.getAttribute(key);
            nodetmp.get_attributes().setAttribute(key, value);
        }
        nodetmp.get_attributes().setAttribute("PARENTID", node.get_parent().get_value());
        nodetmp.set_value(node.get_value());
        nodetmp.set_imageUrl(node.get_imageElement().src);
        nodetmp.set_text(node.get_text());
        nodesCopyTmp.nodes[index] = nodetmp;
        index++;
    });
    return nodesCopyTmp;
}

function Translate(text, scope) {
    var parameter = {};
    parameter.text = text;
    parameter.scope = scope;
    return getData(parameter, 'translate');
}

function GetNewNodePositionInNodes(nodes, nodeText) {
    var position = 0;
    var nodesArray = nodes._array;
    for (i = 0; i < nodesArray.length; i++) {
        if (nodesArray[i].get_text().localeCompare(nodeText) > -1) {
            position = nodesArray[i].get_index();
            break;
        }
    }
    if (position === null) {
        return nodes._array.length;
    }
    else {
        return position;
    }

}

function GetNewFolderPositionInNodes(nodes, nodeText, folderAttribute, folderTyp) {
    var position = 0;
    var nodesArray = nodes._array;
    for (i = 0; i < nodesArray.length; i++) {
        if (nodesArray[i].get_attributes().getAttribute(folderAttribute, folderTyp) === folderTyp) {
            if (nodesArray[i].get_text().localeCompare(nodeText) > -1) {
                position = nodesArray[i].get_index();
                break;
            }
        }
    }
    if (position === null) {
        return nodes._array.length;
    }
    else {
        return position;
    }
}

function GetNewItemPositionInItems(items, itemText) {
    var position = 0;
    var itemArray = items._array;
    for (i = 0; i < itemArray.length; i++) {
        if (itemArray[i].get_text().localeCompare(itemText) > -1) {
            position = itemArray[i].get_index();
            break;
        }
    }
    if (position === 0) {
        return itemArray.length;
    }
    else {
        return position;
    }
}

function TreeCollapseAllNodes(tree) {
    var nodes = tree.get_allNodes();
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].get_nodes() !== null) {
            nodes[i].collapse();
        }
    }
}

function TreeExpandToRoot(tree, node) {
    node1 = tree.findNodeByValue(node.get_value());
    node1 = node1.get_parent();
    while (node1 !== null) {
        if (node1.expand) {
            node1.expand();
        }

        node1 = node1.get_parent();
    }
}

function getData(parameter, serviceName) {
    if (ajaxUrl === undefined) {
        GetAjaxFolder();
    }
    refreshTimeout();

    var formData = JSON.stringify(parameter);
    var ret;
    $.ajax({
        type: "POST",
        url: ajaxUrl + serviceName,
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            ret = JSON.parse(msg.d);
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });


    if (!(ret.errorShortMessage === undefined) && ret.errorShortMessage.length > 0) {
        $("#ctl00_ErrorWindow_C_ErrorTitle")[0].innerHTML = ret.errorTitle;
        $("#ctl00_ErrorWindow_C_ErrorMessageShort")[0].innerHTML = ret.errorShortMessage;
        $("#ctl00_ErrorWindow_C_ErrorMessage")[0].innerHTML = ret.errorMessage;
        $find("ctl00_ErrorWindow").show();
    }

    if (!(ret[0] === undefined) && ret[0].ERROR.length > 0) {
        $("#ctl00_ErrorWindow_C_ErrorTitle")[0].innerHTML = 'Fehler';
        $("#ctl00_ErrorWindow_C_ErrorMessageShort")[0].innerHTML = ret[0].ERROR;
        $find("ctl00_ErrorWindow").show();
    }

    if (ret.data === "[]") {
        return "";
    }
    if (typeof ret.data === "string") {
        return JSON.parse(ret.data);
    }
    else {
        return ret.data;
    }

}

function ValidatorEnable(className) {
    $.each($("." + className + " .FieldValidator"), function (key, value) {
        $("#" + value.id)[0].enabled = true;
    });
}

function ValidatorDisable(className) {
    $.each($("." + className + " .FieldValidator"), function (key, value) {
        $("#" + value.id)[0].enabled = false;
    });

}

function GetImagePath() {
    parameter = {};
    return getData(parameter, "GetImagePath");
}

function ErrorWindowOkClicked(sender, args) {

}

function refreshTimeout() {
    window.clearInterval(timeoutInterval);
    timeoutInterval = window.setInterval('goBack()', 3600 * 1000);
}



