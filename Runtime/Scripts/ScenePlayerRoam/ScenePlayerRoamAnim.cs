using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;


namespace ST.Tool
{
    /// <summary>
    /// 场景漫游动画控制器：在编辑器 AnimationMode 下采样 idle / run 动画片段，
    /// 根据移动状态切换播放。
    /// </summary>
    public class ScenePlayerRoamAnim
    {
        const string IDLE_CLIP_NAME = "idle";

        const string RUN_CLIP_NAME = "run";

        Animator m_Anim;

        AnimationClip m_IdleClip;

        AnimationClip m_RunClip;

        bool m_IsInIdle = true;

        float m_AnimPlayTime;

        /// <summary>
        /// 初始化：获取 Animator 组件，查找 idle/run 片段并启动 AnimationMode。
        /// </summary>
        /// <param name="go">挂载 Animator 的根 GameObject。</param>
        public void Init(GameObject go)
        {
            m_Anim = go.GetComponentInChildren<Animator>();

            AnimationClip[] animationClips = m_Anim != null && m_Anim.runtimeAnimatorController != null ?
                m_Anim.runtimeAnimatorController.animationClips : new AnimationClip[0];

            InitIdleClip(animationClips);
            InitRunClip(animationClips);

            AnimationMode.StartAnimationMode();
        }

        /// <summary>
        /// 每帧推进动画播放时间并对当前状态（idle/run）采样对应片段。
        /// </summary>
        /// <param name="deltaTime">帧间隔时间。</param>
        public void LateUpdate(float deltaTime)
        {
            m_AnimPlayTime += deltaTime;

            if (m_IsInIdle)
            {
                if (m_AnimPlayTime > m_IdleClip.length)
                {
                    m_AnimPlayTime -= m_IdleClip.length;
                }

                AnimationMode.SampleAnimationClip(m_Anim.gameObject, m_IdleClip, m_AnimPlayTime);
            }
            else
            {
                if (m_AnimPlayTime > m_RunClip.length)
                {
                    m_AnimPlayTime -= m_RunClip.length;
                }

                AnimationMode.SampleAnimationClip(m_Anim.gameObject, m_RunClip, m_AnimPlayTime);
            }
        }

        /// <summary>
        /// 切换到跑步动画状态。
        /// </summary>
        public void Run()
        {
            m_IsInIdle = false;
        }

        /// <summary>
        /// 切换到待机动画状态。
        /// </summary>
        public void Idle()
        {
            m_IsInIdle = true;
        }

        /// <summary>
        /// 从动画片段列表中查找并缓存名称包含 <c>run</c> 的片段。
        /// </summary>
        /// <param name="animationClips">控制器中的全部动画片段。</param>
        void InitRunClip(AnimationClip[] animationClips)
        {
            if (animationClips == null)
                return;

            foreach (var clip in animationClips)
            {
                if (clip == null)
                    continue;

                if (clip.name.Contains(RUN_CLIP_NAME))
                {
                    m_RunClip = clip;
                    break;
                }
            }
        }

        /// <summary>
        /// 从动画片段列表中查找并缓存名称包含 <c>idle</c> 的片段。
        /// </summary>
        /// <param name="animationClips">控制器中的全部动画片段。</param>
        void InitIdleClip(AnimationClip[] animationClips)
        {
            if (animationClips == null)
                return;

            foreach (var clip in animationClips)
            {
                if (clip == null)
                    continue;

                if (clip.name.Contains(IDLE_CLIP_NAME))
                {
                    m_IdleClip = clip;
                    break;
                }
            }
        }
    }
}


#endif
