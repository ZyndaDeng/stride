 type PropertyType=any;
 interface PropertyMetaData{
    name:string|symbol
    type:PropertyType
}

 interface ScriptMetaData{
    ctor:Function
    propertys?:Array<PropertyMetaData>
}



class Environment{
    Components:{[name:string]:ScriptMetaData}

    constructor(){
        this.Components={};
    }
}

 const __ScriptMap=new Map<Function,ScriptMetaData>();
 const __environment=new Environment();
(globalThis as any).__stride=__environment;