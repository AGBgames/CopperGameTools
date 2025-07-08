


declare function ccbGetSceneNodeFromName(sceneNodeName: string): any;
/**
 * Gets a SceneNode by name.
 * @param sceneNodeName Name of the SceneNode to get.
 */
function GetSceneNode(sceneNodeName: string): any {
    return ccbGetSceneNodeFromName(sceneNodeName);
}

declare function ccbGetSceneNodeProperty(sceneNode: any, propertyName: string): any;
/**
 * Edit a given property of a given SceneNode.
 * @param sceneNode SceneNode to edit the prop. of.
 * @param propertyName Name of the property to edit.
 */
function GetSceneNodeProperty(sceneNode: string, propertyName: string):any {
    return ccbGetSceneNodeProperty(sceneNode, propertyName);
}

declare function ccbSetSceneNodeProperty(sceneNode: any, propertyName: string, value: any): any;
/**
 * Edit a given property of a given SceneNode.
 * @param sceneNode SceneNode to edit the prop. of.
 * @param propertyName Name of the property to edit.
 * @param value New value of the property.
 */
function SetSceneNodeProperty(sceneNode: string, propertyName: string, value: any):any {
    return ccbSetSceneNodeProperty(sceneNode, propertyName, value);
}

declare function ccbCloneSceneNode(sceneNode: any): any;
/**
 * Returns the clone of a given SceneNode.
 * @param sceneNode SceneNode to clone.
 */
function CloneSceneNode(sceneNode: any): any {
    return ccbCloneSceneNode(sceneNode);
}

declare function ccbGetActiveCamera(): any;
/**
 * Returns the active camera as a SceneNode.
 */
function GetActiveCamera() {
    return ccbGetActiveCamera();
}

declare function ccbSetActiveCamera(newCameraSceneMode: any): void;
/**
 * Sets the new active camera.
 * @param newCameraSceneMode Camera SceneNode.
 */
function SetActiveCamera(newCameraSceneMode: any): any {
    return ccbSetActiveCamera(newCameraSceneMode);
}


declare function ccbGetCopperCubeVariable(variableName: string): any;
/**
 * Get the value of an CopperCube Variable.
 * @param name Name of the CopperCube Variable to get.
 */
function GetVariable(name: string): any {
    return ccbGetCopperCubeVariable(name);
}
declare function ccbSetCopperCubeVariable(variableName: string, value: any): void;
function SetVariable(name: string, value: any) {
    ccbSetCopperCubeVariable(name, value);
}

declare function ccbRegisterOnFrameEvent(func: any): void;
function RegisterFrameEvent(func: any): void {
    ccbRegisterOnFrameEvent(func);
}
declare function ccbUnregisterOnFrameEvent(func: any): void;
function UnregisterFrameEvent(func: any): void {
    ccbUnregisterOnFrameEvent(func);
}

declare function ccbRegisterKeyUpEvent(funcString: string): void;
declare function ccbRegisterKeyDownEvent(funcString: string): void;

// ------ FILES -------
declare function ccbReadFileContent(fileName: string): string;
function ReadFile(fileName: string): string {
    if (!FileExists(fileName)) 
        return "undefined";
    return ccbReadFileContent(fileName);
}
declare function ccbWriteFileContent(fileName: string, fileContent: string): void;
function WriteFile(fileName: string, fileContent: string): void {
    ccbWriteFileContent(fileName, fileContent);
    Log(`Wrote ${fileContent.length} characters of data to ${fileName}`);
}
declare function ccbFileExist(fileName: string): boolean;
function FileExists(fileName: string): boolean {
    return ccbFileExist(fileName);
}
declare function ccbFileDelete(fileName: string): void;
function FileDelete(fileName: string): void {
    ccbFileDelete(fileName);
}

function DirectoryCreate(directoryName: string): void {
    system("mkdir " + directoryName, true);
}
function DirectoryDelete(directoryName: string): void {
    system("del " + directoryName, true);
}

function FileCopy(oldPathpath: string, newFilepath: string): void {
    system("cp " + oldPathpath + " " + newFilepath, true);
}

/**
 * Reads an GFF file and returns its containing data in an array.
 * @param fileName Name of the GFF File to read.
 * @returns An array of type string.
 */
function ReadGFFFile(fileName: string): string[] {
    const dataRaw = ReadFile(fileName);
    return dataRaw.split(":");
}
function WriteGFFFile(fileName: string, fileContent: any[]): void {
    let toWrite = "";
    fileContent.forEach(content => toWrite 
        += content + fileContent.indexOf(content) != fileContent.length - 1 ? ":" : "" );
    WriteFile(fileName, toWrite);
}

// ------- UTILS ------
function printConsole(text: string): void {
    //@ts-ignore
    print(text);
}

declare function ccbDoHTTPRequest(url: string, callback): any;
function DoHTTPRequest(url: string, callback: any): any {
    return ccbDoHTTPRequest(url, callback);
}

declare function ccbSetCursorVisible(visible: boolean): void;
function SetCursorVisible(visible: boolean): void {
    ccbSetCursorVisible(visible);
}

declare function ccbSwitchToScene(sceneName: string): void;

declare function ccbSwitchToFullscreen(cursorLock: boolean, element: any, backToWindow: boolean): void;

declare function system(command: string, hideConsole: boolean): void;

declare function ccbEndProgram(): void;
function QuitProgram(): void {
    ccbEndProgram();
}
