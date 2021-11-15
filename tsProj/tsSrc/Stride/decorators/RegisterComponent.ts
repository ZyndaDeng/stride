

/**将脚本注册到引擎 */
 function RegisterComponent(name:string):ClassDecorator{
    return function(ctor:Function){
        const coms=__environment.Components;
        const meta={ctor:ctor};
        coms[name]=meta;
        __ScriptMap.set(ctor,meta);
    }
} 

function getMetaData(array: Array<PropertyMetaData>, keyName: string|symbol): PropertyMetaData {
    for (var i = 0; i < array.length; i++) {
        if (array[i].name === keyName) {
            return array[i];
        }
    }
    array.push({name:keyName,type:undefined});
    return array[array.length - 1];
}

 function DisplayProperty(type:PropertyType):PropertyDecorator{
    return function (target, propertyKey) {
        if(!target||!propertyKey)return;
        const scriptMeta=__ScriptMap.get(target.constructor);
        if(scriptMeta){
            if(!scriptMeta.propertys)scriptMeta.propertys=[];
            const propertys=scriptMeta.propertys;
            const propertyMeta=getMetaData(propertys,propertyKey);
            propertyMeta.type=type;
        }
    }
}