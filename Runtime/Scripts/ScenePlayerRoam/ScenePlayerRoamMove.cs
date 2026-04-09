using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

namespace ST.Tool
{
    /// <summary>
    /// 场景漫游移动控制器：根据输入方向与前向模式计算位移，
    /// 并通过射线检测使角色贴合地形高度。
    /// </summary>
    public class ScenePlayerRoamMove
    {
        /// <summary>
        /// 前向参考模式：摄像机方向、玩家自身方向或世界轴方向。
        /// </summary>
        public enum ForwardMode
        {
            Camera,
            Player,
            World
        };

        /// <summary>
        /// 基础移动速度（内部固定值，实际速度受 <see cref="playerNormalMoveSpeed"/> 缩放）。
        /// </summary>
        float m_Speed;

        /// <summary>
        /// 速度阻尼系数，控制加速与减速的平滑程度。
        /// </summary>
        float m_VelocityDamping;

        /// <summary>
        /// 当前前向参考模式。
        /// </summary>
        ForwardMode m_InputForward;

        /// <summary>
        /// 是否在移动时旋转角色朝向速度方向。
        /// </summary>
        bool m_RotatePlayer = true;

        /// <summary>
        /// 当前平滑后的速度向量。
        /// </summary>
        Vector3 m_CurrentVelocity;

        /// <summary>
        /// 水平方向输入值（左/右）。
        /// </summary>
        float m_MoveX;

        /// <summary>
        /// 垂直方向输入值（前/后）。
        /// </summary>
        float m_MoveY;

        /// <summary>
        /// 是否正在按住 Shift 加速。
        /// </summary>
        bool m_isUseSpeedUp;

        /// <summary>
        /// 角色的 Transform 引用。
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
        /// 初始化移动控制器，重置速度状态与运动参数。
        /// </summary>
        /// <param name="gameObject">角色 GameObject，用于获取 Transform。</param>
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
        /// 每帧执行移动逻辑。
        /// </summary>
        /// <param name="deltaTime">帧间隔时间。</param>
        public void LateUpdate(float deltaTime)
        {
            Move(m_MoveX, m_MoveY, m_isUseSpeedUp);
        }

        /// <summary>
        /// 设置本帧的移动输入参数。
        /// </summary>
        /// <param name="x">水平输入（左右）。</param>
        /// <param name="y">垂直输入（前后）。</param>
        /// <param name="isSpeedUp">是否启用加速。</param>
        public void SetMoveParam(float x, float y, bool isSpeedUp)
        {
            m_MoveX = x;
            m_MoveY = y;
            m_isUseSpeedUp = isSpeedUp;
        }

        /// <summary>
        /// 根据当前前向模式返回水平面内的前向向量（y 分量归零后归一化）。
        /// </summary>
        /// <returns>归一化的前向方向向量。</returns>
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
        /// 根据输入与前向方向计算目标速度，使用阻尼平滑后更新角色位置与朝向。
        /// </summary>
        /// <param name="x">水平输入。</param>
        /// <param name="z">垂直输入。</param>
        /// <param name="isUseSpeedUp">是否加速。</param>
        /// <param name="fwd">当前帧的前向方向。</param>
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
        /// 从角色上方向下射线检测 TERRAIN/FLOOR 层，将角色 Y 坐标吸附到地表。
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
        /// 执行一次完整的移动：获取前向、更新 Transform、校正高度。
        /// </summary>
        /// <param name="x">水平输入。</param>
        /// <param name="z">垂直输入。</param>
        /// <param name="isUseSpeedUp">是否加速。</param>
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
