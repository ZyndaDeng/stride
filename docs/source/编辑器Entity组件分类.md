
`StrideDefaultAssetsPlugin`在InitializeSession的时候注册了`EntityHierarchyAssetNodeUpdater` 其在调用UpdateNode时候调用了以下代码
```
                var types = typeof(EntityComponent).GetInheritedInstantiableTypes()
                    .Where(x => Attribute.GetCustomAttribute(x, typeof(NonInstantiableAttribute)) == null &&
                                (EntityComponentAttributes.Get(x).AllowMultipleComponents
                                 || ((EntityComponentCollection)node.Value).All(y => y.GetType() != x)))
                    .OrderBy(DisplayAttribute.GetDisplayName)
                    .Select(x => new AbstractNodeType(x)).ToArray();
                node.AttachedProperties.Add(EntityHierarchyData.EntityComponentAvailableTypesKey, types);
                
                //TODO: Choose a better grouping method.
                var typeGroups =                     
                    types.GroupBy(t => ComponentCategoryAttribute.GetCategory(t.Type))
                    .OrderBy(g => g.Key)
                    .Select(g => new AbstractNodeTypeGroup(g.Key, g.ToArray())).ToArray();
```
`EntityComponentAttributes` 的ComponentAttributes记录了有`AllowMultipleComponentsAttribute`的Component
```
[DataContract("ScriptComponent", Inherited = true)]
    [DefaultEntityComponentProcessor(typeof(ScriptProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [Display(Expand = ExpandRule.Once)]
    [AllowMultipleComponents]
    [ComponentOrder(1000)]
    [ComponentCategory("Scripts")]
    public abstract class ScriptComponent : EntityComponent, ICollectorHolder
```
比如这里的ScriptComponent的ComponentCategory就是GameStudio里面对该类Component分类为Scripts