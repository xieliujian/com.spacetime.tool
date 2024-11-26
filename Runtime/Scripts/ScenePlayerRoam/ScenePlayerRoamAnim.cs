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
    public class ScenePlayerRoamAnim
    {
        const string IDLE_CLIP_NAME = "idle";

        const string RUN_CLIP_NAME = "run";

        Animator m_Anim;

        AnimationClip m_IdleClip;

        AnimationClip m_RunClip;

        bool m_IsInIdle = true;

        float m_AnimPlayTime;

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
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
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
        /// 
        /// </summary>
        public void Run()
        {
            m_IsInIdle = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Idle()
        {
            m_IsInIdle = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animationClips"></param>
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
        /// 
        /// </summary>
        /// <param name="animationClips"></param>
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