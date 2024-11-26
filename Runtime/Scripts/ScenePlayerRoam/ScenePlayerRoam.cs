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
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class ScenePlayerRoam : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机焦点的偏移")]
        public Vector3 lookatPosOffset = new Vector3(0f, 1f, 0f);

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机距离角色的最小最大距离（模拟值）")]
        public Vector2 camDisRange = new Vector2(1f, 5f);

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机垂直角度范围")]
        public Vector2 camVerticalAngleRange = new Vector2(-60f, 30f);

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机水平移动速度")]
        public float camHorizontalAimingSpeed = 400f;

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机垂直移动速度")]
        public float camVerticalAimingSpeed = 400f;

        /// <summary>
        /// 
        /// </summary>
        [Header("人物移动的普通速度")]
        public float playerNormalMoveSpeed = 3.5f;

        /// <summary>
        /// 
        /// </summary>
        [Header("人物移动按住SHIFT的加速速度")]
        public float playerSpeedUpMult = 20f;

        /// <summary>
        /// .
        /// </summary>
        ScenePlayerRoamAnim m_Anim = new ScenePlayerRoamAnim();

        /// <summary>
        /// .
        /// </summary>
        ScenePlayerRoamMove m_Move = new ScenePlayerRoamMove();

        /// <summary>
        /// .
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
        /// 
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
        /// 
        /// </summary>
        void OnDestroy()
        {
            UnityEditor.SceneView.duringSceneGui -= OnSceneGui;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDisable()
        {
            UnityEditor.SceneView.duringSceneGui -= OnSceneGui;
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (m_SceneView == null)
            {
                m_SceneView = SceneView.lastActiveSceneView;
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        void OnEditorAppUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneView"></param>
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
        /// 
        /// </summary>
        /// <param name="sceneView"></param>
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
        /// 
        /// </summary>
        /// <param name="sceneView"></param>
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
        /// 
        /// </summary>
        /// <param name="stage"></param>
        void WalkForward(ShortcutStage stage)
        {
            m_CurrentInputVector.y = (stage == ShortcutStage.Begin) ? 0.5f : ((m_CurrentInputVector.y > 0f) ? 0f : m_CurrentInputVector.y);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        void WalkBackward(ShortcutStage stage)
        {
            m_CurrentInputVector.y = (stage == ShortcutStage.Begin) ? -0.5f : ((m_CurrentInputVector.y < 0f) ? 0f : m_CurrentInputVector.y);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        void WalkLeft(ShortcutStage stage)
        {
            m_CurrentInputVector.x = (stage == ShortcutStage.Begin) ? -0.5f : ((m_CurrentInputVector.x < 0f) ? 0f : m_CurrentInputVector.x);

            if (m_SceneView != null)
            {
                m_SceneView.Repaint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
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