var menu = API.createMenu("Gas Station", "Refueling vehicle", 0, 0, 3);
menu.AddItem(API.createMenuItem("Refuel full", "Refuel your vehicle to 100%"));
menu.AddItem(API.createMenuItem("Refuel amount", "Refuel vehicle by amount"));
menu.AddItem(API.createMenuItem("Cancel", "Cancel refueling"));
menu.OnItemSelect.connect(function (sender, item, index) {
    switch (index) {
        case 0:
            API.triggerServerEvent("refuel_full");
            break;
        case 1:
            let a;
            if ((a = TryParseInt(API.getUserInput("", 3), -1)) != -1) {
                API.sendChatMessage(a.toString());
                API.triggerServerEvent("refuel_amount", a.toString());
            } else {
                API.sendChatMessage("Wrong value. Try again");
                menu.Visible = true;
            }
            break;
        default:
    }
    menu.Visible = false;
});
function TryParseInt(str, defaultValue){
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}
API.onUpdate.connect(function () {
    if (menuPool != null)
        menuPool.ProcessMenus();
});
API.onServerEventTrigger.connect(function (eventName, args) {
    switch (eventName) {
        case "hide_menu":
            menu.Visible = false;
            break;
        case "show_menu":
            var idList = new Array();
            menuPool = API.getMenuPool();
            menuPool.Add(menu);
            menu.Visible = true;
            
            break;
    }
})
var menuPool = null;