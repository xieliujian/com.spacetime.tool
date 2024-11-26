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
    public class ScenePlayerRoamMove
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ForwardMode
        {
            Camera,
            Player,
            World
        };

        /// <summary>
        /// 
        /// </summary>
        float m_Speed;

        /// <summary>
        /// 
        /// </summary>
        float m_VelocityDamping;

        /// <summary>
        /// 
        /// </summary>
        ForwardMode m_InputForward;

        /// <summary>
        /// 
        /// </summary>
        bool m_RotatePlayer = true;

        /// <summary>
        /// 
        /// </summary>
        Vector3 m_CurrentVelocity;

        /// <summary>
        /// 
        /// </summary>
        float m_MoveX;

        /// <summary>
        /// 
        /// </summary>
        float m_MoveY;

        /// <summary>
        /// 
        /// </summary>
        bool m_isUseSpeedUp;

        /// <summary>
        /// 
        /// </summary>
        Transform transform;

        /// <summary>
        /// 人物移动的普通速度
        /// </summary>
        public float playerNormalMoveSpeed;

        /// <summary>
        /// 人物移动按住SHIFT的加速速度
        /// </summary>
        public float playerSpeedUpMult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public void Init(GameObject gameObject)
        {
            transform = gameObject.transform;
            m_Speed = 5;
            m_InputForward = ForwardMode.Camera;
            m_RotatePlayer = true;
            m_VelocityDamping = 0.5f;
            m_CurrentVelocity = Vector3.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void LateUpdate(float deltaTime)
        {
            Move(m_MoveX, m_MoveY, m_isUseSpeedUp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isSpeedUp"></param>
        public void SetMoveParam(float x, float y, bool isSpeedUp)
        {
            m_MoveX = x;
            m_MoveY = y;
            m_isUseSpeedUp = isSpeedUp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Vector3 GetForward()
        {
            Vector3 fwd;

            switch (m_InputForward)
            {
                case ForwardMode.Camera:
                    fwd = Vector3.forward;

                    var cam = SceneView.lastActiveSceneView.camera;
                    if (cam != null)
                    {
                        fwd = cam.transform.forward;
                    }
                    break;
                case ForwardMode.Player:
                    fwd = transform.forward;
                    break;
                case ForwardMode.World:
                default:
                    fwd = Vector3.forward;
                    break;
            }

            fwd.y = 0;
            fwd = fwd.normalized;

            return fwd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="isUseSpeedUp"></param>
        /// <param name="fwd"></param>
        void SetTransform(float x, float z, bool isUseSpeedUp, Vector3 fwd)
        {
            Quaternion inputFrame = Quaternion.LookRotation(fwd, Vector3.up);
            Vector3 input = new Vector3(x, 0, z);
            input = inputFrame * input;
            var dt = Time.deltaTime;
            var desiredVelocity = input * m_Speed;

            var deltaVel = desiredVelocity - m_CurrentVelocity;
            m_CurrentVelocity += Damper.Damp(deltaVel, m_VelocityDamping, dt);

            var deltaPos = m_CurrentVelocity * dt * (isUseSpeedUp ? playerSpeedUpMult : playerNormalMoveSpeed);
            transform.position += deltaPos;
            if (m_RotatePlayer && m_CurrentVelocity.sqrMagnitude > 0.01f)
            {
                var qA = transform.rotation;
                var qB = Quaternion.LookRotation((m_InputForward == ForwardMode.Player && Vector3.Dot(fwd, m_CurrentVelocity) < 0) ? -m_CurrentVelocity : m_CurrentVelocity);
                transform.rotation = Quaternion.Slerp(qA, qB, Damper.Damp(1, m_VelocityDamping, dt));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void CheckHeight()
        {
            var p = transform.position;

            var rayPos = new Vector3(p.x, p.y + 10f, p.z);
            var rayDir = Vector3.down;
            var rayLayer = 1 << LayerMask.NameToLayer("TERRAIN") | 1 << LayerMask.NameToLayer("FLOOR");
            RaycastHit rayhit;
            if (Physics.Raycast(rayPos, rayDir, out rayhit, 100f, rayLayer))
            {
                p.y = rayhit.point.y;
            }

            transform.position = p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="isUseSpeedUp"></param>
        void Move(float x, float z, bool isUseSpeedUp)
        {
            Vector3 fwd = GetForward();
            if (fwd.sqrMagnitude < 0.01f)
                return;

            SetTransform(x, z, isUseSpeedUp, fwd);
            CheckHeight();
        }
    }
}


#endif