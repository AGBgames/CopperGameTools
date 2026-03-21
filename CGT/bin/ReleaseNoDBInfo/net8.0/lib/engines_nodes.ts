
interface IEngineNode {

    getSceneNode(): any;

    setProperty(propertyName: string, propertyValue: any): boolean;
    getProperty(propertyName: string): any;
};

class EngineNode implements IEngineNode {

    private name: string;
    private sceneNode: any;

    constructor(sceneNode: any) {
        this.sceneNode = sceneNode;
        this.name = ccbGetSceneNodeProperty(this.sceneNode, "Name");
    }

    getName(): string {
        return this.name;
    }

    getSceneNode(): any {
        return this.sceneNode;
    }

    setProperty(propertyName: string, propertyValue: any): boolean {
        if (this.getProperty(propertyName) == null)
            return false;
        SetSceneNodeProperty(this.name, propertyName, propertyValue);
        return true;
    }
    getProperty(propertyName: string) {
        return GetSceneNodeProperty(this.name, propertyName);
    }
}

function EngineNodeFromSceneNodeName(sceneNodeName: string): EngineNode | null {
    const sceneNode = GetSceneNode(sceneNodeName);
    if (sceneNode == null)
        return null;
    return new EngineNode(sceneNode);
}
