
enum EngineNodeTypes {
    Generic,
    Camera,
    ThreeDModel,
    Light,
    Invalid
};

function EngineNodeTypeFromString(sceneNodeType: string): EngineNodeTypes {
    if (sceneNodeType == null)
        return EngineNodeTypes.Invalid;
    switch (sceneNodeType) {
        case "Camera":
            return EngineNodeTypes.Camera;
        case "3D-Model":
            return EngineNodeTypes.ThreeDModel;
        case "Light":
            return EngineNodeTypes.Light;
        default:
            return EngineNodeTypes.Generic;
    }
}

interface IEngineNode {
    /**
     * Returns the corresponding CopperCube Engine SceneNode.
     */
    getSceneNode(): any;

    getName(): string;
    getType(): EngineNodeTypes;
    /**
     * List of children nodes.
     */
    getChildren(): IEngineNode[];

    setProperty(propertyName: string, propertyValue: any): boolean;
    getProperty(propertyName: string): any;
};

class EngineNode implements IEngineNode {
    private sceneNode: any;

    private name: string;
    private type: EngineNodeTypes;
    private children: IEngineNode[];

    constructor(sceneNode: any) {
        this.sceneNode = sceneNode;
        this.name = ccbGetSceneNodeProperty(this.sceneNode, "Name");
        this.type = EngineNodeTypeFromString(
            String(ccbGetSceneNodeProperty(this.sceneNode, "Type"))
        );
    }

    getSceneNode(): any {
        return this.sceneNode;
    }

    getName(): string {
        return this.name;
    }
    getType(): EngineNodeTypes {
        return this.type;
    }
    getChildren(): IEngineNode[] {
        return this.children;
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
