[← 返回 README](../README.md)

# 场景视图角色漫游

> 参考文章：[场景视图角色漫游（知乎）](https://zhuanlan.zhihu.com/p/606785263)

## 说明

在 Unity **编辑器模式**下，通过挂载 `ScenePlayerRoam` 组件，实现在 SceneView 中以第三人称方式漫游场景。  
适用于「边漫游边编辑场景」的工作流，漫游时摄像机自动跟随角色，支持地形高度吸附。

## 快速开始

1. 在场景中选择或创建一个角色 GameObject（需带 `Animator` 组件）
2. 挂载 `ScenePlayerRoam` 组件
3. 在 Inspector 中填入 `Animator` 组件及相关参数（见下方参数说明）
4. 进入 **编辑器 Play 模式** 或直接在 **Edit 模式**下，切换到 SceneView 窗口
5. 使用 WASD 移动，鼠标右键拖拽旋转视角
6. 按 `Ctrl+L` 临时屏蔽/恢复键盘输入

## 操控说明

| 操作 | 效果 |
|---|---|
| `W` / `A` / `S` / `D` | 向前 / 左 / 后 / 右移动 |
| 鼠标右键拖拽 | 水平 / 垂直旋转摄像机 |
| 鼠标滚轮 | 摄像机与角色距离缩放 |
| `Ctrl+L` | 临时屏蔽/恢复键盘输入 |

## Inspector 参数说明

| 参数 | 说明 |
|---|---|
| `lookatPosOffset` | 摄像机注视点相对角色的偏移（默认 `(0, 1, 0)`） |
| `camDisRange` | 摄像机与角色的最小/最大距离（默认 `(1, 5)`） |
| `camVerticalAngleRange` | 摄像机垂直角度范围（默认 `(-60°, 30°)`） |
| `camHorizontalAimingSpeed` | 摄像机水平旋转速度（默认 `400`） |
| `camVerticalAimingSpeed` | 摄像机垂直旋转速度（默认 `400`） |

## 组件架构

```
ScenePlayerRoam          ← 主组件，协调以下三个子控制器
├── ScenePlayerRoamAnim  ← 动画控制（Idle / Run 状态切换）
├── ScenePlayerRoamMove  ← 移动控制（WASD 输入 + 地形高度吸附）
└── ScenePlayerRoamCam   ← 摄像机控制（SceneView pivot / rotation / size）
```

### ScenePlayerRoamMove 关键参数

| 字段 | 说明 |
|---|---|
| `ForwardMode` | 前进方向模式（跟随摄像机朝向 / 自身朝向） |
| 地面射线检测 | 自动将角色吸附到地面/地形高度 |
| 速度阻尼 | 移动速度平滑过渡，避免突变 |

### ScenePlayerRoamCam 关键常量

| 常量 | 说明 |
|---|---|
| 旋转阻尼 | 摄像机旋转平滑系数 |
| 距离步长 | 滚轮每步缩放量 |

## 依赖

- 角色 GameObject 需附带 `Animator` 组件及对应动画状态机（含 Idle、Run 动画片段）
- 需在 **Unity Editor** 下运行（使用 `#if UNITY_EDITOR` 编译条件保护）
- 运行时通过 `UnityEditor.ShortcutManagement` 注册 `Ctrl+L` 快捷键

## 注意事项

- 本组件仅在 **编辑器模式** 下工作，打包时自动剔除
- 漫游过程中 SceneView 会被接管，若需切回正常编辑请按 `Ctrl+L` 暂停输入
- 动画 `AnimationMode` 在漫游启动时开启，停止时自动还原

---

[← 返回 README](../README.md)