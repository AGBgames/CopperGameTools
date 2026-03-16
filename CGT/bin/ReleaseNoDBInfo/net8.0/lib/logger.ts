
let LogSavePath: string = "launcher/log/";
let LogSavePathLatest: string = `${LogSavePath}latest.log`;
let LogSavePathPreviousLatest: string = `${LogSavePath}prev-latest.log`;

let logString: string = "";

const Log = function (message: string): void {
    const fullMessage =
        new Date().toLocaleString() + `: ${message}\n`;
    logString += fullMessage;
    printConsole(fullMessage);
}

const SaveLog = function (): void {
    Log("Saving Log.");
    
    if (FileExists(LogSavePathLatest)) {
        Log("Previous Log Found!");
        WriteFile(LogSavePathPreviousLatest, ReadFile(LogSavePathLatest));
    }
    
    WriteFile(LogSavePathLatest, logString);
}