function IsMouseOnButton(node, buttonWidth, buttonHeight) {
    var mouseX = ccbGetMousePosX();
    Log("MouseX: " + mouseX);
    var mouseY = ccbGetMousePosY();
    Log("MouseY: " + mouseY);

    var screenWidth = ccbGetScreenWidth();
    var screenHeight = ccbGetScreenHeight();

    // Prozentuale Button-Position abrufen
    var buttonPosXPercent = ccbGetSceneNodeProperty(node, "Pos X (percent)");
    var buttonPosYPercent = ccbGetSceneNodeProperty(node, "Pos Y (percent)");

    // In Pixel umrechnen
    var buttonX = screenWidth * (buttonPosXPercent / 100.0);
    Log("buttonX: " + buttonX);
    var buttonY = screenHeight * (buttonPosYPercent / 100.0);
    Log("buttonY: " + buttonY);

    var halfWidth = buttonWidth / 2.0;
    var halfHeight = buttonHeight / 2.0;

    var left = buttonX - halfWidth;
    var right = buttonX + halfWidth;
    var top = buttonY - halfHeight;
    var bottom = buttonY + halfHeight;

    var xDiff = Math.abs(mouseX - buttonX);
    var yDiff = Math.abs(mouseY - buttonY);

    var maxXDiff = screenWidth * 0.5;
    var maxYDiff = screenHeight * 0.1;

    return xDiff <= maxXDiff && yDiff <= maxYDiff;
}