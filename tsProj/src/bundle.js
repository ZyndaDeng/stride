"use strict";
class Environment {
    constructor() {
        this.Components = {};
    }
}
const __ScriptMap = new Map();
const __environment = new Environment();
globalThis.__stride = __environment;
class ClearScript {
    Initialize(args) {
        this.Services = args.Services;
        this.graphicsDeviceService = args.graphicsDeviceService;
        this.Game = args.Game;
        this.Content = args.Content;
        this.Input = args.Input;
        this.SceneSystem = args.SceneSystem;
        this.EffectSystem = args.EffectSystem;
        this.Audio = args.Audio;
        this.SpriteAnimation = args.SpriteAnimation;
        this.GameProfiler = args.GameProfiler;
        this.DebugText = args.DebugText;
        this.Streaming = args.Streaming;
        this.Start();
    }
}
/**将脚本注册到引擎 */
function RegisterComponent(name) {
    return function (ctor) {
        const coms = __environment.Components;
        const meta = { ctor: ctor };
        coms[name] = meta;
        __ScriptMap.set(ctor, meta);
    };
}
function getMetaData(array, keyName) {
    for (var i = 0; i < array.length; i++) {
        if (array[i].name === keyName) {
            return array[i];
        }
    }
    array.push({ name: keyName, type: undefined });
    return array[array.length - 1];
}
function DisplayProperty(type) {
    return function (target, propertyKey) {
        if (!target || !propertyKey)
            return;
        const scriptMeta = __ScriptMap.get(target.constructor);
        if (scriptMeta) {
            if (!scriptMeta.propertys)
                scriptMeta.propertys = [];
            const propertys = scriptMeta.propertys;
            const propertyMeta = getMetaData(propertys, propertyKey);
            propertyMeta.type = type;
        }
    };
}
//# sourceMappingURL=bundle.js.map