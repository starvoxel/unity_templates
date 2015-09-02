using System;
using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(GUIText))]
    public class FPSCounter : MonoBehaviour
    {
        //const
        const float UPDATE_INTERVAL = 0.5f;
        const string DISPLAY_FORMAT = "{0} FPS";

        //private
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        private GUIText m_GUIText;

        #region Unity API
        private void Awake()
        {
            if (Debug.isDebugBuild)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            m_FpsNextPeriod = Time.realtimeSinceStartup + UPDATE_INTERVAL;
            m_GUIText = GetComponent<GUIText>();
        }

        private void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / UPDATE_INTERVAL);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += UPDATE_INTERVAL;
                m_GUIText.text = string.Format(DISPLAY_FORMAT, m_CurrentFps);
            }
        }
        #endregion
    }
}
