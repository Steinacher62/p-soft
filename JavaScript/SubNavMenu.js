var prevMenuGroup = null;
var prevSrcElement = null;

function ShowMenuGroup(menuGroup)
{
    var srcElement = event.srcElement;
    
    if (prevSrcElement == srcElement)
    {
        return;
    }
    
    if (prevMenuGroup)
    {
        SetItemVisible(prevMenuGroup, false, true);
        prevMenuGroup = null;
    }
    
    if (prevSrcElement && prevSrcElement.innerText)
    {
        if (prevSrcElement.innerText.charCodeAt(0) == 0x25bc)
            prevSrcElement.innerText = String.fromCharCode(0x25c6) + prevSrcElement.innerText.substring(1);
        prevSrcElement = null;
    }
    
    SetItemVisible(menuGroup, true, true);
    SetItemVisible(menuGroup, true, false);
    prevMenuGroup = menuGroup;

    if (srcElement && srcElement.innerText)
    {
        srcElement.innerText = String.fromCharCode(0x25bc) + srcElement.innerText.substring(1);
        prevSrcElement = srcElement;
    }
}

function SetItemVisible(item, visible, up)
{
    if (item.isMenuGroup == 'True' && item.style)
    {
        if (visible)
            item.style.display = '';
        else
            item.style.display = 'none';
    }

    if (up)
    {
        var parentItem = item.parentElement;
        if (parentItem  && !(parentItem.root == 'True'))
            SetItemVisible(parentItem, visible, true);
    }
    else
    {
        for (var child in item.childNodes)
        {
            SetItemVisible(child, visible, false);
        }
    }
}
