using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class ScenePlayerRoamCam
    {
        /// <summary>
        /// 
        /// </summary>
        public const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
        public const string ERROR_UN_PLAYER = "ThirdPersonCam脚本没有指定玩家";

        /// <summary>
        /// 摄像机
        /// </summary>
        SceneView m_SceneView;

        /// <summary>
        /// 玩家transform
        /// </summary>
        Transform m_Player;

        /// <summary>
        /// 水平旋转的角度
        /// </summary>
        private float m_AngleH = 0.0f;

        /// <summary>
        /// 垂直旋转的角度
        /// </summary>
        private float m_AngleV = -30.0f;

        /// <summary>
        /// 摄像机焦点的偏移
        /// </summary>
        public Vector3 lookatPosOffset;

        /// <summary>
        /// 摄像机距离角色的最小最大距离（模拟值）
        /// </summary>
        public Vector2 camDisRange;

        /// <summary>
        /// 摄像机垂直角度范围
        /// </summary>
        public Vector2 camVerticalAngleRange;

        /// <summary>
        /// 水平瞄准速度
        /// </summary>
        public float camHorizontalAimingSpeed;

        /// <summary>
        /// 垂直瞄准速度
        /// </summary>
        public float camVerticalAimingSpeed;

        /// <summary>
        /// .
        /// </summary>
        public SceneView sceneView
        {
            get { return m_SceneView; }
            set { m_SceneView = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public void Init(GameObject gameObject)
        {
            SetPlayer(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void OnCamDrag(Vector2 delta)
        {
            m_AngleH += Mathf.Clamp(delta.x / Screen.width, -1.0f, 1.0f) * camHorizontalAimingSpeed;
            m_AngleV -= Mathf.Clamp(delta.y / Screen.height, -1.0f, 1.0f) * camVerticalAimingSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void LateUpdate(float deltaTime)
        {
            if (m_SceneView == null)
                return;

            if (m_Player == null)
            {
                Debug.LogError(ERROR_UN_PLAYER);
                return;
            }

            // 垂直角度的范围
            m_AngleV = Mathf.Clamp(m_AngleV, camVerticalAngleRange.x, camVerticalAngleRange.y);

            // 模拟设置摄像机距离的最小最大值
            m_SceneView.size = Mathf.Clamp(m_SceneView.size, camDisRange.x, camDisRange.y);

            // 场景视图的旋转和中心点
            Quaternion animRotation = Quaternion.Euler(-m_AngleV, m_AngleH, 0.0f);
            m_SceneView.rotation = animRotation;
            m_SceneView.pivot = m_Player.position + lookatPosOffset;

            m_SceneView.Repaint();
        }

        void SetPlayer(GameObject gameObject)
        {
            m_Player = gameObject.transform;
        }
    }
}


#endif