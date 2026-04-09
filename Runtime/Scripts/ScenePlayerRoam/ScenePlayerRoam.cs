using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.ShortcutManagement;

namespace ST.Tool
{
    /// <summary>
    /// 场景漫游主控组件（仅编辑器）：协调动画、移动、摄像机三个子控制器，
    /// 监听 WASD 键盘输入与鼠标右键拖拽，实现 SceneView 中的第三人称漫游。
    /// 按 <c>Ctrl+L</c> 可临时屏蔽/恢复输入。
    /// </summary>
    [ExecuteInEditMode]
    public class ScenePlayerRoam : MonoBehaviour
    {
        /// <summary>
        /// 摄像机焦点的偏移
        /// </summary>
        [Header("摄像机焦点的偏移")]
        public Vector3 lookatPosOffset = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// 摄像机距离角色的最小最大距离（模拟值）
        /// </summary>
        [Header("摄像机距离角色的最小最大距离（模拟值）")]
        public Vector2 camDisRange = new Vector2(1f, 5f);

        /// <summary>
        /// 摄像机垂直角度范围
        /// </summary>
        [Header("摄像机垂直角度范围")]
        public Vector2 camVerticalAngleRange = new Vector2(-60f, 30f);

        /// <summary>
        /// 摄像机水平移动速度
        /// </summary>
        [Header("摄像机水平移动速度")]
        public float camHorizontalAimingSpeed = 400f;

        /// <summary>
        /// 摄像机垂直移动速度
        /// </summary>
        [Header("摄像机垂直移动速度")]
        public float camVerticalAimingSpeed = 400f;

        /// <summary>
        /// 人物移动的普通速度
        /// </summary>
        [Header("人物移动的普通速度")]
        public float playerNormalMoveSpeed = 3.5f;

        /// <summary>
        /// 人物移动按住SHIFT的加速速度
        /// </summary>
        [Header("人物移动按住SHIFT的加速速度")]
        public float playerSpeedUpMult = 20f;

        /// <summary>
        /// 动画子控制器。
        /// </summary>
        ScenePlayerRoamAnim m_Anim = new ScenePlayerRoamAnim();

        /// <summary>
        /// 移动子控制器。
        /// </summary>
        ScenePlayerRoamMove m_Move = new ScenePlayerRoamMove();

        /// <summary>
        /// 摄像机子控制器。
        /// </summary>
        ScenePlayerRoamCam m_Camera = new ScenePlayerRoamCam();

        /// <summary>
        /// 当前输入向量
        /// </summary>
        Vector3 m_CurrentInputVector;

        /// <summary>
        /// 方向键按下
        /// </summary>
        HashSet<KeyCode> m_DirKeysDown = new HashSet<KeyCode>();

        /// <summary>
        /// 方向键函数绑定
        /// </summary>
        Dictionary<KeyCode, Action<ShortcutStage>> m_DirKeyBindings = new Dictionary<KeyCode, Action<ShortcutStage>>();

        /// <summary>
        /// SceneView
        /// </summary>
        private SceneView m_SceneView;

        /// <summary>
        /// 是否可以使用鼠标
        /// </summary>
        bool m_CanUseInput = true;

        /// <summary>
        /// 注册 SceneView 与 EditorApplication 回调，初始化三个子控制器并绑定方向键。
        /// </summary>
        void OnEnable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
            SceneView.duringSceneGui += OnSceneGui;

            EditorApplication.update -= OnEditorAppUpdate;
            EditorApplication.update += OnEditorAppUpdate;

            m_Anim.Init(gameObject);
            m_Move.Init(gameObject);
            m_Camera.Init(gameObject);

            m_DirKeyBindings.Clear();
            m_DirKeyBindings.Add(KeyCode.W, WalkForward);
            m_DirKeyBindings.Add(KeyCode.S, WalkBackward);
            m_DirKeyBindings.Add(KeyCode.A, WalkLeft);
            m_DirKeyBindings.Add(KeyCode.D, WalkRight);
        }

        /// <summary>
        /// 取消 SceneView 回调注册。
        /// </summary>
        void OnDestroy()
        {
            UnityEditor.SceneView.duringSceneGui -= OnSceneGui;
        }

        /// <summary>
        /// 取消 SceneView 回调注册。
        /// </summary>
        void OnDisable()
        {
            UnityEditor.SceneView.duringSceneGui -= OnSceneGui;
        }

        /// <summary>
        /// 每帧缓存最近活跃的 SceneView 引用。
        /// </summary>
        void Update()
        {
            if (m_SceneView == null)
            {
                m_SceneView = SceneView.lastActiveSceneView;
            }
        }

        /// <summary>
        /// 将摄像机参数同步至子控制器，并在输入开启时驱动动画、移动与摄像机更新。
        /// </summary>
        void LateUpdate()
        {
            ///摄像机焦点的偏移
            m_Camera.sceneView = m_SceneView;
            m_Camera.lookatPosOffset = lookatPosOffset;
            m_Camera.camDisRange = camDisRange;
            m_Camera.camVerticalAngleRange = camVerticalAngleRange;
            m_Camera.camHorizontalAimingSpeed = camHorizontalAimingSpeed;
            m_Camera.camVerticalAimingSpeed = camVerticalAimingSpeed;
            // 移动体素检测
            m_Move.playerNormalMoveSpeed = playerNormalMoveSpeed;
            m_Move.playerSpeedUpMult = playerSpeedUpMult;

            float deltaTime = Time.deltaTime;

            if (m_CanUseInput)
            {
                m_Anim.LateUpdate(deltaTime);
                m_Move.LateUpdate(deltaTime);
                m_Camera.LateUpdate(deltaTime);
            }
        }

        /// <summary>
        /// 驱动 PlayerLoop 强制刷新，保证编辑器模式下 Update/LateUpdate 持续调用。
        /// </summary>
        void OnEditorAppUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();
        }

        /// <summary>
        /// SceneView GUI 回调：处理 <c>Ctrl+L</c> 输入开关、WASD 移动及鼠标拖拽旋转。
        /// </summary>
        /// <param name="sceneView">触发回调的 SceneView。</param>
        void OnSceneGui(SceneView sceneView)
        {
            bool isKeyDown = Event.current.type == EventType.KeyDown;
            bool isCtrl = Event.current.control;
            if (isCtrl && isKeyDown && Event.current.keyCode == KeyCode.L)
            {
                m_CanUseInput = !m_CanUseInput;
            }

            if (m_CanUseInput)
            {
                WADSKeysProcess(sceneView);
                MouseProcess(sceneView);
            }
        }

        /// <summary>
        /// 处理鼠标右键拖拽事件，将增量传给摄像机控制器旋转视角。
        /// </summary>
        /// <param name="sceneView">当前 SceneView。</param>
        void MouseProcess(SceneView sceneView)
        {
            var evt = Event.current;
            bool isMidMouse = evt.isMouse && evt.button == 1;
            var mouseDelta = evt.delta;
            if (isMidMouse)
            {
                m_Camera.OnCamDrag(mouseDelta);
            }
        }

        /// <summary>
        /// 处理 WASD 键盘事件：KeyDown 时触发对应行走动作并更新移动参数，
        /// KeyUp 时移除方向键记录，所有方向键释放后停止移动。
        /// </summary>
        /// <param name="sceneView">当前 SceneView。</param>
        void WADSKeysProcess(SceneView sceneView)
        {
            var evt = Event.current;

            bool isSpeedUp = evt.shift;
            Action<ShortcutStage> action;

            switch (evt.type)
            {
                case EventType.KeyDown:
                    {
                        KeyCode keyCode = evt.keyCode;

                        if (m_DirKeyBindings.TryGetValue(keyCode, out action))
                        {
                            action(ShortcutStage.Begin);
                            m_Anim.Run();
                            m_Move.SetMoveParam(m_CurrentInputVector.x, m_CurrentInputVector.y, isSpeedUp);
                            m_DirKeysDown.Add(keyCode);
                            evt.Use();
                        }
                    }
                    break;
                case EventType.KeyUp:
                    {
                        KeyCode keyCode = evt.keyCode;

                        if (m_DirKeyBindings.TryGetValue(keyCode, out action))
                        {
                            action(ShortcutStage.End);
                            m_Move.SetMoveParam(m_CurrentInputVector.x, m_CurrentInputVector.y, isSpeedUp);

                            m_DirKeysDown.Remove(keyCode);

                            if (m_DirKeysDown.Count == 0)
                            {
                                m_CurrentInputVector = Vector3.zero;
                                m_Anim.Idle();
                                m_Move.SetMoveParam(0, 0, false);
                            }

                            evt.Use();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 按下/松开 W 键时更新前向输入分量。
        /// </summary>
        /// <param name="stage">按键阶段（Begin/End）。</param>
        void WalkForward(ShortcutStage stage)
        {
            m_CurrentInputVector.y = (stage == ShortcutStage.Begin) ? 0.5f : ((m_CurrentInputVector.y > 0f) ? 0f : m_CurrentInputVector.y);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 按下/松开 S 键时更新后向输入分量。
        /// </summary>
        /// <param name="stage">按键阶段（Begin/End）。</param>
        void WalkBackward(ShortcutStage stage)
        {
            m_CurrentInputVector.y = (stage == ShortcutStage.Begin) ? -0.5f : ((m_CurrentInputVector.y < 0f) ? 0f : m_CurrentInputVector.y);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 按下/松开 A 键时更新左向输入分量。
        /// </summary>
        /// <param name="stage">按键阶段（Begin/End）。</param>
        void WalkLeft(ShortcutStage stage)
        {
            m_CurrentInputVector.x = (stage == ShortcutStage.Begin) ? -0.5f : ((m_CurrentInputVector.x < 0f) ? 0f : m_CurrentInputVector.x);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 按下/松开 D 键时更新右向输入分量。
        /// </summary>
        /// <param name="stage">按键阶段（Begin/End）。</param>
        void WalkRight(ShortcutStage stage)
        {
            m_CurrentInputVector.x = (stage == ShortcutStage.Begin) ? 0.5f : ((m_CurrentInputVector.x > 0f) ? 0f : m_CurrentInputVector.x);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }
    }
}

#endif
