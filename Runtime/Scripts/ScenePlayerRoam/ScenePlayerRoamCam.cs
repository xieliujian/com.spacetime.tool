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
        public const string ERROR_UN_PLAYER = "ThirdPersonCam�ű�û��ָ�����";

        /// <summary>
        /// �����
        /// </summary>
        SceneView m_SceneView;

        /// <summary>
        /// ���transform
        /// </summary>
        Transform m_Player;

        /// <summary>
        /// ˮƽ��ת�ĽǶ�
        /// </summary>
        private float m_AngleH = 0.0f;

        /// <summary>
        /// ��ֱ��ת�ĽǶ�
        /// </summary>
        private float m_AngleV = -30.0f;

        /// <summary>
        /// ����������ƫ��
        /// </summary>
        public Vector3 lookatPosOffset;

        /// <summary>
        /// ����������ɫ����С�����루ģ��ֵ��
        /// </summary>
        public Vector2 camDisRange;

        /// <summary>
        /// �������ֱ�Ƕȷ�Χ
        /// </summary>
        public Vector2 camVerticalAngleRange;

        /// <summary>
        /// ˮƽ��׼�ٶ�
        /// </summary>
        public float camHorizontalAimingSpeed;

        /// <summary>
        /// ��ֱ��׼�ٶ�
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

            // ��ֱ�Ƕȵķ�Χ
            m_AngleV = Mathf.Clamp(m_AngleV, camVerticalAngleRange.x, camVerticalAngleRange.y);

            // ģ������������������С���ֵ
            m_SceneView.size = Mathf.Clamp(m_SceneView.size, camDisRange.x, camDisRange.y);

            // ������ͼ����ת�����ĵ�
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