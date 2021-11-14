# 源码分析

## [GameStudioWindow](../../sources/editor/Stride.GameStudio/GameStudioWindow.xaml.cs)

方法`GameStudioLoaded` 

```
Editor.Session.ServiceProvider.Get<IAssetsPluginService>().Plugins.ForEach(x => x.InitializeSession(Editor.Session));
```
这里的IAssetsPluginService就是[StrideEditorPlugin](../../sources/editor/Stride.GameStudio/StrideEditorPlugin.cs)

 StrideEditorPlugin的InitializeSession初始化`GameStudioPreviewService`

 然后方法StrideUIThread初始化了`PreviewGame`