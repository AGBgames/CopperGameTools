
let LogSavePath: string = "launcher/log/";
let LogSavePathLatest: string = `${LogSavePath}latest.log`;
let LogSavePathPreviousLatest: string = `${LogSavePath}prev-latest.log`;

let logString: string = "";

function Log(message: string): void {
    const fullMessage =
        new Date().toLocaleString() + `: ${message}\n`;
    logString += fullMessage;
    printConsole(fullMessage);
}

function SaveLog(): void {
    Log("Saving Log.");
    
    if (FileExists(LogSavePathLatest)) {
        Log("Previous Log Found!");
        WriteFile(LogSavePathPreviousLatest, ReadFile(LogSavePathLatest));
    }
    
    WriteFile(LogSavePathLatest, logString);
}