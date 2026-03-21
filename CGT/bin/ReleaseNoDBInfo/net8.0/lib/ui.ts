
function IsMouseOnButton(buttonNode: any, buttonWidth: Number, buttonHeight: Number) {
    const mouseX: Number = ccbGetMousePosX();
    Log("MouseX: " + mouseX);
    const mouseY: Number = ccbGetMousePosY();
    Log("MouseY: " + mouseY);

    const screenWidth: Number = ccbGetScreenWidth();
    const screenHeight: Number = ccbGetScreenHeight();

    const buttonPosXPercent: Number = ccbGetSceneNodeProperty(buttonNode, "Pos X (percent)");
    const buttonPosYPercent: Number = ccbGetSceneNodeProperty(buttonNode, "Pos Y (percent)");

    const buttonX = screenWidth * (buttonPosXPercent / 100.0);
    Log("buttonX: " + buttonX);
    const buttonY = screenHeight * (buttonPosYPercent / 100.0);
    Log("buttonY: " + buttonY);

    const halfWidth: Number = buttonWidth / 2.0;
    const halfHeight: Number = buttonHeight / 2.0;

    const left: Number = buttonX - halfWidth;
    const right: Number = buttonX + halfWidth;
    const top: Number = buttonY - halfHeight;
    const bottom: Number = buttonY + halfHeight;

    const xDiff: Number = Math.abs(mouseX - buttonX);
    const yDiff: Number = Math.abs(mouseY - buttonY);

    const maxXDiff: Number = screenWidth * 0.5;
    const maxYDiff: Number = screenHeight * 0.1;

    return xDiff <= maxXDiff && yDiff <= maxYDiff;
}
