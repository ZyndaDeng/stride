# 源码分析

## 入口
`GameStudio` stride的项目编辑器 类似vs 主要用于编辑代码

[Program.cs](../../sources/editor/Stride.GameStudio/Program.cs) 
Startup方法里面如果有`initialSessionPath`直接打开[GameStudioWindow](./GameStudioWindow.md)
```
                if (!UPath.IsNullOrEmpty(initialSessionPath))
                {
                    var sessionLoaded = await editor.OpenInitialSession(initialSessionPath);
                    if (sessionLoaded == true)
                    {
                        var mainWindow = new GameStudioWindow(editor);
                        Application.Current.MainWindow = mainWindow;
                        WindowManager.ShowMainWindow(mainWindow);
                        return;
                    }
                }

```

没有则通过[ProjectSelectionWindow](../../sources/editor/Stride.GameStudio/ProjectSelectionWindow.xaml.cs) 选择了项目之后打开[GameStudioWindow](./GameStudioWindow.md)