# com.spacetime.tool

Unity 编辑器工具包，提供复制场景、材质批量编辑、材质贴图替换、资源规则管理、编辑器协程及场景漫游等功能。

---

## 功能列表

| 功能 | 菜单路径 | 文档 |
|---|---|---|
| 复制场景编辑器 | `SpaceTime/Tool/复制场景` | [文档](readme/CopyScene.md) |
| 材质编辑器 | `SpaceTime/Tool/材质编辑器` | [文档](readme/MaterialSearchModify.md) |
| 材质贴图替换编辑器 | `SpaceTime/Tool/材质贴图替换编辑器` | [文档](readme/MaterialTexReplace.md) |
| 资源规则管理 | `SpaceTime/ResTools/ResRulePanel` | [文档](readme/ResRule.md) |
| 编辑器协程 | — | [文档](readme/EditorCoroutine.md) |
| 场景视图角色漫游 | — | [文档](readme/ScenePlayerRoam.md) |

---

## 复制场景编辑器

将源场景目录完整复制到目标目录，并自动将所有 Asset 引用从源路径重定向到目标路径。

[→ 查看详细文档](readme/CopyScene.md)

---

## 材质编辑器

以 Shader 为维度浏览工程内全部材质，支持按 Keyword 或属性值筛选，并提供批量修改与无效 Keyword 清理功能。

[→ 查看详细文档](readme/MaterialSearchModify.md)

---

## 材质贴图替换编辑器

指定源贴图与目标贴图，批量替换工程内所有 `.mat` 文件中对源贴图的 GUID 引用。

[→ 查看详细文档](readme/MaterialTexReplace.md)

---

## 编辑器协程

在 `EditorApplication.update` 驱动下，支持在编辑器模式中使用协程语法（yield return）执行异步逻辑。

[→ 查看详细文档](readme/EditorCoroutine.md)

---

## 资源规则管理

以规则驱动的方式批量管理贴图、模型、音频三类资源的导入设置，支持按正则路径匹配规则、执行导入参数对齐及一致性检查。

[→ 查看详细文档](readme/ResRule.md)

---

## 场景视图角色漫游

在 Unity SceneView 中实现第三人称漫游，通过 WASD 移动、鼠标右键旋转视角，适用于边漫游边编辑场景。

[→ 查看详细文档](readme/ScenePlayerRoam.md)