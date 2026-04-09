[← 返回 README](../README.md)

# 资源规则管理工具（ResRule）

## 说明

以 **规则驱动** 的方式批量管理 Unity 工程内贴图、模型、音频三类资源的导入设置（Import Settings）。  
核心思路：为工程创建一个 `ResRuleManagerAsset`，在其中维护若干条 `ResRuleData`，每条规则通过 **正则关键字 + 影响路径** 匹配一批资源，并记录该资源类型应有的导入参数；执行规则时批量将匹配资源的 ImporterSettings 对齐到规则记录值。

---

## 菜单路径

| 窗口 | 菜单路径 |
|---|---|
| 规则管理面板 | `MHT/ResTools/ResRulePanel` |
| 资源查询面板 | `MHT/ResTools/ResCheckPanel` |
| 清空视图缓存 | `MHT/ResTools/ClearViewDataCache` |

---

## 快速开始

### 1. 创建规则管理资产

打开 `MHT/ResTools/ResRulePanel`，点击工具栏 **File → 为当前工程创建规则管理**。

工具会在以下路径生成 `.asset` 文件：

```
Assets/LingRen/Script/ResTools/EditorConfig/Projects/<工程名>/<工程名>.asset
```

> 提示：`ResRuleDefine.RULE_PROJECT_DIR` 常量定义了保存路径，可按需修改。

### 2. 选择规则管理资产

面板右上角 **RuleMgr** 下拉框会自动扫描工程内所有 `ResRuleManagerAsset`，优先选中与当前工程同名的资产。

### 3. 新建规则

点击 **File → New Rule** 为当前选中的资源类型（Texture / Model / Audio）新建一条规则。

### 4. 配置规则

每条规则的配置项：

| 字段 | 说明 |
|---|---|
| **Rule Name** | 规则名称，不可重复 |
| **Key Word** | 匹配资源路径的正则表达式（默认 `.*` 匹配全部） |
| **Exclude Word** | 排除关键字，空格分隔，路径包含时跳过 |
| **Extra Rule** | 下拉选择额外处理逻辑（由工程自定义 `IExtraProcess` 实现提供） |
| **Extra Rule Param** | 额外处理的参数列表（空格分隔） |
| **影响路径** | 规则生效的 Assets 路径列表（至少一条） |
| **排除路径** | 即使路径在影响范围内，也跳过的路径列表 |
| **禁用规则** | 勾选后该规则不参与执行 |

### 5. 执行规则

- 单条规则：点击规则标题行的 **执行规则** 按钮。
- 全部规则：**Edit → 执行所有规则**。

---

## 界面布局

```
┌──────────────────────────────────────────────────────────────────────────┐
│  [File ▼]  [Edit ▼]                              RuleMgr: [下拉选择] │  ← 顶部菜单栏
├──────────────────────────────────────────────────────────────────────────┤
│  [Texture]  [Model]  [Audio]                                             │  ← 资源类型切换
├──────────────────────────────────────────────────────────────────────────┤
│  ┌── 规则 1 ── [禁用] [刷新] [执行] [↑] [↓] [☰]                         │
│  │   Rule Name / Key Word / Exclude Word / Extra Rule / 影响路径 …       │  ← 规则列表区（可拖拽高度）
│  └─────────────────────────────────────────────────────                  │
├──────────────────────────────────────────────────────────────────────────┤
│  [Asset 列表滚动视图，显示匹配当前规则的资源]                              │  ← 资源预览区
└──────────────────────────────────────────────────────────────────────────┘
```

规则列表区与资源预览区之间有一条 **拖拽分隔条**，拖动可调整各区高度。

---

## 资源查询面板（ResCheckPanel）

打开 `MHT/ResTools/ResCheckPanel`，可以在不执行规则的情况下**按规则筛选查询资源**。

操作流程：

1. 右上角 **RuleMgr** 下拉选择规则管理资产
2. **Rule** 下拉选择要查询的规则
3. 资源类型 tab 切换 Texture / Model / Audio
4. 视图中展示所有匹配该规则的资源，点击高亮到 Project 窗口

---

## 资源检查功能

通过 **Edit → 检查所有资源** 可对工程内全部资源执行导入参数一致性检查，结果输出到控制台并保存为文本文件。

---

## 扩展自定义处理（IExtraProcess）

如需对特定资源执行规则之外的额外处理（如修改 AnimatorController、特殊压缩格式等），在 **项目中**（非包内）实现 `ExtraProcessingBase<T>`：

```csharp
using ST.Tool;
using UnityEditor;

// 示例：自定义贴图额外处理
public class MyTextureExtraProcess : ExtraProcessingBase<MyTextureExtraProcess>
{
    public override Type GetImporterType() => typeof(TextureImporter);
    public override string GetName() => "MyTextureProcess";
    public override string GetDesc() => "自定义贴图处理";

    public override void OnExecute(ResRuleData ruleData, AssetImporter assetImporter)
    {
        var ti = assetImporter as TextureImporter;
        if (ti == null) return;
        // 自定义处理逻辑 ...
        ti.SaveAndReimport();
    }

    public override bool OnEqual(ResRuleData ruleData, AssetImporter assetImporter)
    {
        // 判断资源当前参数是否已与规则一致，返回 true 表示一致（无需处理）
        return true;
    }

    public override bool GetIsOverride(string key) => false;
}
```

`ExtraProcessingSelect` 在面板初始化时通过反射扫描同 Assembly 内所有 `ExtraProcessingBase<T>` 实现类，自动填充下拉列表，无需额外注册。

---

## 规则数据持久化

规则保存在 `ResRuleManagerAsset`（ScriptableObject）中，修改后通过 **File → Save** 或工具栏自动提示保存。  
`.asset` 文件可提交到版本控制，团队共享同一套规则配置。

---

## 路径配置说明

`ResRuleDefine.cs` 定义了工具使用的默认路径，如需修改请直接编辑该文件：

| 常量 | 默认值 | 说明 |
|---|---|---|
| `RULE_PROJECT_DIR` | `Assets/LingRen/Script/ResTools/EditorConfig/Projects` | 规则资产保存目录 |
| `RULE_TEMPLATE_DIR` | `Assets/LingRen/Script/ResTools/EditorConfig/ResTemplate` | 模板文件目录 |
| `CHECK_NORMAL_DIR` | `Assets` | 默认检查的根目录 |

---

[← 返回 README](../README.md)