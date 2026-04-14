# CLAUDE.md — com.spacetime.tool

## 项目概述

Unity 编辑器工具包（UPM Package），提供复制场景、材质批量编辑、材质贴图替换、资源规则管理、编辑器协程及场景漫游等功能。

- **包名**: `com.spacetime.tool`
- **最低 Unity 版本**: 2020.3
- **命名空间**: `ST.Tool`
- **依赖包**: `com.spacetime.core`（命名空间 `ST.Core`）

## 目录结构

```
com.spacetime.tool/
├── Editor/Scripts/           # 编辑器脚本（仅 Editor 平台）
│   ├── CopyScene/            # 复制场景工具
│   ├── EditorCoroutine/      # 编辑器协程
│   ├── MaterialSearchModify/ # 材质搜索修改
│   ├── MaterialTexReplace/   # 材质贴图替换
│   ├── ResRule/              # 资源规则管理（Panel/Module/View/Utils）
│   └── ScenePlayerRoam/      # 场景漫游编辑器部分
├── Runtime/Scripts/          # 运行时脚本
│   └── ScenePlayerRoam/      # 场景漫游运行时部分
├── Tests/Editor/             # 编辑器测试
├── readme/                   # 各功能详细文档（Markdown）
├── README.md                 # 包总览文档
└── package.json
```

## Assembly Definition

| asmdef | 名称 | 平台 | 用途 |
|---|---|---|---|
| `Editor/com.spacetime.tool.editor.asmdef` | `com.spacetime.tool` | Editor only | 引用 core.editor、core.runtime、tool.runtime |
| `Runtime/com.spacetime.tool.runtime.asmdef` | `com.spacetime.tool.runtime` | 全平台 | 运行时脚本 |
| `Tests/Editor/com.spacetime.tool.editor.test.asmdef` | `com.spacetime.tool.editor.test` | 全平台 | 编辑器测试 |

## 编码规范

- **命名空间**: 所有 C# 文件必须位于 `namespace ST.Tool` 内
- **文件编码**: UTF-8 无 BOM
- **注释语言**: 使用中文 XML 文档注释（`/// <summary>...</summary>`）
- **partial class**: 大型 EditorWindow 使用 partial class 拆分（如 `ResRulePanel.cs` / `ResRulePanel_Menu.cs` / `ResRulePanel_Rule.cs`）
- **不使用** `LR` / `LingRen` 前缀（历史遗留已全部清除）
- **不使用** `Debugger.LogXxx`，统一使用 `Debug.Log` / `Debug.LogErrorFormat` 等标准 Unity API

## 核心模块说明

### ResRule（资源规则管理）

分层架构：

| 层级 | 路径 | 职责 |
|---|---|---|
| Panel | `ResRule/Panel/` | UI 窗口（`ResRulePanel`、`ResCheckPanel`），partial class 拆分 |
| UIField | `ResRule/Panel/UIField/` | 下拉选择框等 UI 组件 |
| View | `ResRule/Panel/View/` | TreeView 滚动视图（Texture/Model/Audio 各一套） |
| Module | `ResRule/Module/` | 数据模型（`ResRuleData`、`ResRuleManagerAsset`）、导入器比较、扩展处理 |
| Utils | `ResRule/Utils/` | `EditorFileUtils`（文件/资产工具）、`ResEditorGUI`（GUI 辅助） |

扩展点：`IExtraProcess` / `ExtraProcessingBase<T>` 接口由项目侧实现，包内不含具体实现。

### 其他工具

| 工具 | Editor 入口 | 说明 |
|---|---|---|
| CopyScene | `CopyScene.cs` + `CopySceneEditor.cs` | 场景目录深拷贝并重定向引用 |
| MaterialSearchModify | `MaterialSearchModify.cs` + `MaterialSearchModifyEditor.cs` | 按 Shader/Keyword 批量搜索修改材质 |
| MaterialTexReplace | `MaterialTexReplaceEditor.cs` | 批量替换 .mat 中贴图 GUID 引用 |
| EditorCoroutine | `EditorCoroutine.cs` | 编辑器模式下协程支持 |
| ScenePlayerRoam | Editor + Runtime 两部分 | SceneView 第三人称漫游 |

## 与 com.spacetime.core 的关系

- `ST.Core.EditorUtils`：提供 `AssetsPath2ABSPath`、`ABSPath2AssetsPath`、`FormatPath`、`RunProcess` 等基础编辑器工具方法
- `ST.Core.Packager`：AB 打包流程
- `ST.Tool.EditorFileUtils` 中不重复 `ST.Core.EditorUtils` 已有功能，两者互补

## 常用操作

### 新增功能模块

1. 在 `Editor/Scripts/` 下创建同名文件夹
2. 所有 C# 文件使用 `namespace ST.Tool`
3. 在 `readme/` 下创建对应 `.md` 文档（头尾加 `[← 返回 README](../README.md)` 链接）
4. 更新 `README.md` 功能列表表格及描述区块

### 迁移外部代码到此包

1. 将 `LR` / `LingRen` 前缀全部替换为 `ST.Tool` 命名空间下的等效类
2. `Debugger.LogXxx` 替换为 `Debug.Log` / `Debug.LogWarningFormat` / `Debug.LogErrorFormat`
3. 检查全局命名空间类是否已加 `namespace ST.Tool`
4. 确认文件编码为 UTF-8 无 BOM

## 注意事项

- `ResRuleDefine.cs` 中的路径常量（`RULE_PROJECT_DIR`、`RULE_TEMPLATE_DIR`）仍为硬编码的旧项目路径，实际使用时需按项目调整
- `Menu.cs` 中菜单路径为 `SpaceTime/ResTools/...`，与其他工具保持一级菜单统一
- 场景漫游功能同时有 Editor 和 Runtime 代码，Runtime 部分通过 `#if UNITY_EDITOR` 条件编译引用编辑器 API
