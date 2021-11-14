## ScriptComponent
[ScriptComponent](../../sources/engine/Stride.Engine/Engine/ScriptComponent.cs)有个[ScriptProcessor](../../sources/engine/Stride.Engine/Engine/Processors/ScriptProcessor.cs) 由[EntityManager](../../sources/engine/Stride.Engine/Engine/EntityManager.cs)管理，
EntityManager的`Update` 调用ScriptProcessor的`Update`
```
 public virtual void Update(GameTime gameTime)
        {
            foreach (var processor in processors)
            {
                if (processor.Enabled)
                {
                    using (processor.UpdateProfilingState = Profiler.Begin(processor.UpdateProfilingKey, "Entities: {0}", entities.Count))
                    {
                        processor.Update(gameTime);
                    }
                }
            }
        }
```

ScriptProcessor重写了`OnEntityComponentAdding`和`OnEntityComponentRemoved`直接将ScriptComponent丢给了[ScriptSystem](../../sources/engine/Stride.Engine/Engine/Processors/ScriptSystem.cs)。

`OnEntityComponentAdding`调用了ScriptComponent的`Initialize`并对SyncScript注册`Update`
## ClearScript
ClearScript应该参考ScriptComponent 需要有个ScriptProcessor和ScriptSystem管理`Initialize`、`Update` 还需要增加Destroy、Enable 设计有以下几点

1. ClearScript的scriptObj的Entity赋值时机放在Initialize之前 
2. ClearScript通过scriptObj判断类型
3. scriptObj的Destroy要找到时机
4. scriptObj的装饰器要给ClearScript分类和列举在GameStudio上
5. scripteObj的装饰器要给ClearScript列举属性