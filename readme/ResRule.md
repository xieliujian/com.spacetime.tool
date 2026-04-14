[← 返回 README](../README.md)

# 资源规则管理工具（ResRule）

## 概述

以 **规则驱动** 的方式批量管理 Unity 工程内 **贴图（Texture）、模型（Model）、音频（Audio）** 三类资源的导入设置（Import Settings）。

核心思路：

1. 为工程创建一个 `ResRuleManagerAsset`（ScriptableObject）
2. 在其中维护若干条 `ResRuleData`，每条规则通过 **正则关键字 + 影响路径** 匹配一批资源
3. 规则内嵌一份模板资源的 ImporterSettings 作为"标准参数"
4. 执行规则时，批量将匹配资源的导入参数对齐到模板值
5. `ResAfterTreatment`（AssetPostprocessor）在资源导入时自动应用匹配规则

---

## 菜单入口

| 窗口 | 菜单路径 | 说明 |
|---|---|---|
| 规则管理面板 | `SpaceTime/ResTools/ResRulePanel` | 主面板，创建/编辑/执行规则 |
| 资源查询面板 | `SpaceTime/ResTools/ResCheckPanel` | 按规则筛选查询资源 |
| 清空视图缓存 | `SpaceTime/ResTools/ClearViewDataCache` | 清除资源视图的缓存数据 |

---

## 快速开始

### 第一步：创建规则管理资产

打开 `SpaceTime/ResTools/ResRulePanel`，点击工具栏 **File → 为当前工程创建规则管理**。

工具会自动在以下路径生成 `.asset` 文件：

```
Assets/SpaceTime/ResTools/EditorConfig/Projects/<工程名>/<工程名>.asset
```

### 第二步：选择规则管理资产

面板右上角 **RuleMgr** 下拉框会自动扫描工程内所有 `ResRuleManagerAsset`，优先选中与当前工程同名的资产。如果没有找到任何资产，会显示红色错误提示。

### 第三步：新建规则

1. 在资源类型 Tab 中选择要管理的类型（Texture / Model / Audio）
2. 点击 **File → New Rule**
3. 工具会从模板目录复制一份对应类型的模板文件（`template.png` / `template.FBX` / `template.mp3`）到工程的规则目录，并创建一条新的规则数据

### 第四步：配置规则

展开新建的规则，配置以下字段：

| 字段 | 说明 |
|---|---|
| **Rule Name** | 规则名称，同一管理资产内不可重复 |
| **Key Word** | 匹配资源路径的正则表达式（默认 `.*` 匹配全部）；需全路径匹配 |
| **Exclude Word** | 排除关键字列表（空格分隔），资源路径包含任一关键字时跳过 |
| **Extra Rule** | 下拉选择额外处理逻辑（由工程自定义 `IExtraProcess` 实现提供） |
| **Extra Rule Param** | 额外处理的参数列表（空格分隔） |
| **影响路径** | 规则生效的 Assets 路径列表，至少添加一条；点击 `+` 按钮或文件夹图标添加 |
| **排除路径** | 即使路径在影响范围内，也跳过的路径列表 |
| **禁用规则** | 勾选后该规则不参与执行，也不在资源导入时自动应用 |

### 第五步：配置模板导入参数

每条规则底部有一个可折叠的 **Importer 面板**，直接使用 Unity 内置的 Inspector 绘制对应类型的导入设置。在此处修改的参数即为"标准值"，执行规则时会将匹配资源的参数对齐到这些值。

### 第六步：执行规则

- **单条规则**：点击规则标题行右侧的 **执行规则** 按钮，或点击齿轮图标选择 **执行规则**
- **全部规则**：工具栏 **Edit → 执行所有规则**
- **保存修改**：工具栏 **File → Save**

---

## 界面布局

### ResRulePanel（规则管理面板）

```
┌──────────────────────────────────────────────────────────────────────────┐
│  [File ▼]  [Edit ▼]                              RuleMgr: [下拉选择]  │  ← 顶部菜单栏
├──────────────────────────────────────────────────────────────────────────┤
│  [Texture]  [Model]  [Audio]                                           │  ← 资源类型切换 Tab
├──────────────────────────────────────────────────────────────────────────┤
│  ┌── 规则 1 ── [禁用] [刷新] [执行] [↑] [↓] [☰]                       │
│  │   Rule Name / Key Word / Exclude Word                               │
│  │   Extra Rule / Extra Rule Param                                     │
│  │   影响路径列表 / 排除路径列表                                         │
│  │   ▶ Importer 面板（模板导入参数）                                    │  ← 规则列表区
│  ├── 规则 2 ── ...                                                     │    （可折叠，可拖拽高度）
│  └─────────────────────────────────────────────────────────────────────  │
│  ═══════════════════════════════════════ ← 拖拽分隔条                   │
├──────────────────────────────────────────────────────────────────────────┤
│  RuleName │ AssetPath │ CompressSize │ Size │ Format │ ...              │
│  ────────────────────────────────────────────────                       │
│  规则名    │ 资源路径   │ 压缩大小     │ 尺寸  │ 格式   │ ...           │  ← 资源列表区
│  （颜色：白色=匹配 / 红色=不匹配）                                       │    （TreeView，可排序）
└──────────────────────────────────────────────────────────────────────────┘
```

**操作说明**：

- 规则列表区与资源列表区之间有一条 **拖拽分隔条**，上下拖动可调整各区域高度
- 资源列表中双击某条资源，自动在 Project 窗口中定位并高亮
- 资源列表的 **RuleName** 列：白色表示资源参数与规则一致，红色表示不一致

### File 菜单

| 菜单项 | 说明 |
|---|---|
| 为当前工程创建规则管理 | 在 `EditorConfig/Projects/<工程名>/` 下创建 `.asset` 文件 |
| Save | 保存当前规则管理资产的修改 |
| New Rule | 为当前选中的资源类型创建一条新规则 |
| 退出 | 关闭面板窗口 |

### Edit 菜单

| 菜单项 | 说明 |
|---|---|
| 执行所有规则 | 对所有未禁用的规则执行批量导入参数对齐 |
| 检查所有资源 | 检查工程内全部资源的导入参数一致性，结果输出 CSV |
| 检查所有资源-场景 | 仅检查场景关联的资源 |
| 打开查询工具... | 打开 ResCheckPanel |

### 规则右键菜单（齿轮图标）

| 菜单项 | 说明 |
|---|---|
| 执行规则 | 执行当前规则 |
| 刷新统计信息 | 刷新资源列表视图 |
| 复制当前规则 | 克隆当前规则（含模板参数） |
| 删除规则 | 删除规则及其对应的模板资源文件 |
| 正则表达式帮助 | 打开正则表达式在线参考页面 |

---

## 资源查询面板（ResCheckPanel）

通过 `SpaceTime/ResTools/ResCheckPanel` 或主面板 **Edit → 打开查询工具...** 打开。

```
┌──────────────────────────────────────────────────────────────────────────┐
│  RuleMgr: [下拉]  ResType: [Texture▼]  Rule: [规则筛选▼]   [搜索框]   │
├──────────────────────────────────────────────────────────────────────────┤
│  [Refresh]                                                              │
├──────────────────────────────────────────────────────────────────────────┤
│  RuleName │ AssetPath │ CompressSize │ ...                              │
│  资源列表（同 ResRulePanel 的 TreeView）                                 │
└──────────────────────────────────────────────────────────────────────────┘
```

**操作流程**：

1. 选择 **RuleMgr**（规则管理资产）
2. 选择 **ResType**（Texture / Model / Audio）
3. 通过 **Rule** 下拉筛选要查看的规则（支持多选 Mask）
4. 点击 **Refresh** 刷新视图
5. 在搜索框中输入关键字进一步过滤
6. 双击资源行，自动定位到 Project 窗口

---

## 资源列表列说明

### Texture 视图

| 列名 | 说明 |
|---|---|
| RuleName | 匹配的规则名（白色=一致，红色=不一致，Null=未匹配） |
| AssetPath | 资源路径 |
| CompressSize(KB) | 压缩后大小 |
| Size | 最大边长 |
| W Format | Windows 平台压缩格式 |
| I Format | iOS 平台压缩格式 |
| A Format | Android 平台压缩格式 |
| TextureType | 贴图类型 |
| AlphaSource | Alpha 来源 |
| sRGB | 是否 sRGB |
| Read\|Write | 是否可读写 |
| GenerateMipMaps | 是否生成 MipMaps |
| WrapMode | 环绕模式 |

### Model 视图

| 列名 | 说明 |
|---|---|
| RuleName | 匹配的规则名 |
| AssetPath | 资源路径 |
| 顶点数 | 网格顶点数 |
| 面数量 | 网格三角面数 |
| 蒙皮数 | SkinnedMeshRenderer 数量 |
| 骨骼数 | 骨骼节点数 |
| 动作时长 | 第一个动画 Clip 的时长 |
| 循环 | 是否循环播放 |
| 动作大小 | AnimationClip 运行时内存大小 |

### Audio 视图

| 列名 | 说明 |
|---|---|
| RuleName | 匹配的规则名 |
| AssetPath | 资源路径 |
| MemorySize | 压缩后内存大小 |
| TimeLength | 音频时长 |
| Channels | 声道数 |
| SampleRate | 采样率 |
| ForceToMono | 是否强制单声道 |
| LoadInBackground | 是否后台加载 |
| Ambisonic | 是否 Ambisonic 音频 |
| SampleRateSetting | 采样率设置模式 |

---

## 资源一致性检查

通过 **Edit → 检查所有资源** 执行全量一致性检查。

检查逻辑：

1. 扫描 `Assets` 目录下所有贴图、模型、音频资源
2. 跳过工具目录（`Assets/SpaceTime/ResTools`）下的资源
3. 对每个资源，查找匹配的规则并比对导入参数
4. 生成 CSV 报告，列为：`规则名, 资源类型, 错误信息, 资源路径`

错误类型：

| 错误信息 | 含义 |
|---|---|
| 资源和规则匹配 | 资源参数与规则一致，无需处理 |
| 资源未匹配任何规则 | 资源路径不在任何规则的影响范围内 |
| 资源匹配了多个规则 | 资源路径被多条规则覆盖（需检查规则配置） |
| 资源信息和规则不匹配 | 资源参数与匹配规则不一致 |

CSV 文件保存路径：`<规则管理资产目录>/CheckLog/RecCheck_<时间戳>.csv`

---

## 自动应用规则（AssetPostprocessor）

`ResAfterTreatment` 继承自 `AssetPostprocessor`，在资源导入时自动执行：

- **OnPreprocessTexture** / **OnPreprocessModel** / **OnPreprocessAudio** / **OnPreprocessAnimation**：当资源路径匹配某条规则时，自动将模板的导入参数拷贝到资源上
- **OnPostprocessAllAssets**：新增或移动的资源，若匹配规则则触发重新导入

这意味着：一旦配置好规则，后续新导入或修改的资源会 **自动对齐** 到规则设定的标准参数，无需手动执行。

---

## 规则匹配详解

### 匹配流程

对于一个资源路径，规则按以下顺序判断是否匹配：

1. **资源类型**：资源的 Importer 类型必须与规则的 Importer 类型一致
2. **排除路径**：资源路径以排除路径开头 → 跳过
3. **影响路径**：资源路径必须以至少一条影响路径开头
4. **排除关键字**：资源路径包含任一排除关键字 → 跳过
5. **正则匹配**：资源完整路径必须 **全匹配** `Key Word` 正则表达式（`Match.Value == 完整路径`）

### 匹配示例

```
Key Word:     .*UI.*
影响路径:     Assets/Art/Textures
排除路径:     Assets/Art/Textures/Temp
排除关键字:   _bak _old

匹配：Assets/Art/Textures/UI/btn_start.png          ✓
不匹配：Assets/Art/Textures/Temp/UI/test.png         ✗ (在排除路径下)
不匹配：Assets/Art/Textures/UI/btn_start_bak.png     ✗ (包含排除关键字)
不匹配：Assets/Art/Textures/bg_main.png              ✗ (不匹配正则)
```

---

## 规则验证

面板会实时检查每条规则的配置有效性，不合规的规则会标红并显示错误信息：

| 错误提示 | 原因 |
|---|---|
| 未设置规则名 | `Rule Name` 为空 |
| 规则名重复 | 与同资产内其他规则名称相同 |
| 未设置匹配正则表达式 | `Key Word` 为空 |
| 未设置作用路径 | 影响路径列表为空 |
| 有无效的路径 | 影响路径中存在不合法的路径 |
| 关键字+路径有重复使用 | 相同的 Key Word + 影响路径组合在多条规则中出现 |

---

## 模型规则额外选项

模型类型的规则提供以下 Override 开关（在 Importer 面板下方）：

| 选项 | 说明 |
|---|---|
| Override OptimizeGameObjects | 是否覆盖目标资源的 OptimizeGameObjects 设置 |
| Override AnimationCompress | 是否覆盖动画压缩方式 |
| Override ImportBlendShapes | 是否覆盖 BlendShape 导入设置 |
| Override ResampleCurves | 是否覆盖曲线重采样设置 |

当对应开关未勾选时，执行规则不会修改目标资源的该项参数。

## 贴图规则额外选项

| 选项 | 说明 |
|---|---|
| Override NPOT | 是否覆盖 Non-Power of Two 设置 |
| Override Type | 通过标志位选择要覆盖的平台格式（Default / Standalone / iPhone / Android） |

---

## 扩展自定义处理（IExtraProcess）

如需对特定资源执行规则之外的额外处理，在 **项目中**（非包内）实现 `ExtraProcessingBase<T>`：

```csharp
using System;
using ST.Tool;
using UnityEditor;

public class MyTextureExtraProcess : ExtraProcessingBase<MyTextureExtraProcess>
{
    public override Type GetImporterType() => typeof(TextureImporter);
    public override string GetName() => "MyTextureProcess";
    public override string GetDesc() => "自定义贴图处理说明";

    public override void OnExecute(ResRuleData ruleData, AssetImporter assetImporter)
    {
        var ti = assetImporter as TextureImporter;
        if (ti == null) return;
        // 自定义处理逻辑 ...
        ti.SaveAndReimport();
    }

    public override bool OnEqual(ResRuleData ruleData, AssetImporter assetImporter)
    {
        // 判断资源当前参数是否已与规则一致
        // 返回 true 表示一致（无需处理）
        return true;
    }

    public override bool GetIsOverride(string key) => false;
}
```

**自动注册**：面板初始化时通过反射扫描所有 `ExtraProcessingBase<T>` 实现类，自动填充规则中的 **Extra Rule** 下拉列表，无需手动注册。

---

## 贴图 Inspector 扩展（CCTextureImporterInspector）

包内提供了一个贴图 Inspector 扩展，在标准 TextureImporter Inspector 下方增加：

- **OriginalSize**：显示贴图原始尺寸（宽 × 高）
- **快捷按钮**：`2048` / `1024` / `512` / `256` / `Clear`，一键设置移动端（iOS + Android）的最大尺寸限制

设置值通过 `TextureImporter.userData` 自定义字段（`MOBILE_MAX_SIZE`）存储，在规则的 `CCTextureImporter.Copy` 中会自动应用。

> 注：此 Inspector 的 `[CustomEditor(typeof(TextureImporter))]` 特性默认为注释状态，如需启用请取消注释。

---

## 规则数据持久化

- 规则保存在 `ResRuleManagerAsset`（ScriptableObject `.asset` 文件）中
- 每条规则关联一个模板资源文件（如 `TextureImporter-2026-04-09-14-30-00-000.png`），存放在规则管理资产同级目录
- 修改后通过 **File → Save** 手动保存
- `.asset` 文件和模板资源可提交到版本控制，团队共享同一套规则配置
- 可通过 Unity 的 `Create → SpaceTime/ResRule/CreateResRuleManagerAsset` 右键菜单手动创建规则管理资产

---

## 路径配置

`ResRuleDefine.cs` 定义了工具使用的默认路径：

| 常量 | 默认值 | 说明 |
|---|---|---|
| `CHECK_NORMAL_DIR` | `Assets` | 默认资源检查的根目录 |
| `RULE_TOOL_DIR` | `Assets/SpaceTime/ResTools` | 工具目录（检查时跳过此目录下的资源） |
| `RULE_PROJECT_DIR` | `Assets/SpaceTime/ResTools/EditorConfig/Projects` | 规则管理资产保存目录 |
| `RULE_TEMPLATE_DIR` | `Assets/SpaceTime/ResTools/EditorConfig/ResTemplate` | 模板文件目录 |

模板文件说明：

| 文件 | 用途 |
|---|---|
| `template.png` | 创建贴图规则时的模板 |
| `template.FBX` | 创建模型规则时的模板 |
| `template.mp3` | 创建音频规则时的模板 |

---

## 架构概览

```
ResRule/
├── Module/                          数据与逻辑层
│   ├── ResRuleManagerAsset          规则管理资产（ScriptableObject）
│   ├── ResRuleData                  单条规则数据（匹配条件 + 模板路径）
│   ├── ResRuleDefine                路径常量定义
│   ├── ResRuleHelper                规则匹配 / 参数比对 / 参数拷贝
│   ├── Menu                         Unity 菜单注册
│   ├── AfterTreatment/              AssetPostprocessor 自动应用规则
│   ├── CopyEquals/                  三种 Importer 的参数拷贝与比对
│   │   ├── CCTextureImporter
│   │   ├── CCModelImporter
│   │   └── CCAudioImporter
│   ├── ExtraProcessing/             扩展处理接口与选择器
│   │   ├── IExtraProcess
│   │   └── ExtraProcessingSelect
│   └── Inspector/                   贴图 Inspector 扩展
│       ├── CCTextureImporterInspector
│       └── CCTextureImporterUserData
├── Panel/                           UI 层
│   ├── ResRulePanel                 主面板（partial class，分文件）
│   │   ├── _Menu                    菜单栏绘制
│   │   ├── _Rule                    规则列表绘制
│   │   ├── _RuleMenu                规则右键菜单
│   │   └── _ShowViewGUI             底部视图 + 拖拽分隔条
│   ├── ResCheckPanel                资源查询面板
│   ├── UIField/                     UI 组件
│   │   ├── ResRuleMgrSelectField    规则管理资产下拉框
│   │   ├── ResRuleSelectField       规则筛选 MaskField
│   │   └── ResTypeSelectField       资源类型切换
│   └── View/                        资源列表视图（TreeView）
│       ├── IScrollView              抽象基类
│       ├── TextureScrollView        贴图视图（5 个 partial）
│       ├── ModelScrollView          模型视图（5 个 partial）
│       ├── AudioScrollView          音频视图（5 个 partial）
│       └── ViewItemModule/          视图数据项缓存
└── Utils/                           工具类
    ├── EditorFileUtils              文件/资产/路径/反射工具
    └── ResEditorGUI                 GUI 辅助（折叠按钮等）
```

---

[← 返回 README](../README.md)
