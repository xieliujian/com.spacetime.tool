[← 返回 README](../README.md)

# 材质贴图替换编辑器

## 说明

批量将工程内所有 `.mat` 文件中指定源贴图的 GUID 引用，替换为目标贴图的 GUID。  
操作直接修改 `.mat` 文件的文本内容，不依赖 Unity 材质 API，兼容大批量材质场景。

## 使用方式

1. 打开窗口：菜单 `SpaceTime/Tool/材质贴图替换编辑器`
2. 在 **源贴图** 字段拖入需要被替换的贴图
3. 在 **目标贴图** 字段拖入替换后使用的贴图
4. 点击 **替换贴图** 按钮，等待进度条完成

## 界面说明

| 字段 | 说明 |
|---|---|
| 源贴图 | 需要被替换的贴图资产 |
| 目标贴图 | 替换后使用的贴图资产 |
| 替换贴图（按钮） | 触发批量替换，处理完成后自动刷新资产数据库 |

## 注意事项

- 替换操作直接读写 `.mat` 文件，**建议操作前先备份工程**
- 替换基于 GUID 匹配，若源贴图未被任何材质引用则跳过
- 进度条显示当前处理进度，完成后自动关闭

## 核心代码

```csharp
// 读取 .mat 文件文本，将源贴图 GUID 替换为目标贴图 GUID
var absMatPath = EditorUtils.AssetsPath2ABSPath(matPath);
var text = File.ReadAllText(absMatPath);
if (!text.Contains(srcTexGUID.ToString()))
    continue;

text = text.Replace(srcTexGUID.ToString(), dstTexGUID.ToString());
File.WriteAllText(absMatPath, text);
```

---

[← 返回 README](../README.md)