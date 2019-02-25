using TMPro;
using UnityEngine;

namespace MCFramework
{
    public class HUDFPS : MonoBehaviour
    {
        public float updateInterval = 0.5F;

        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval

        void Start()
        {
            timeleft = updateInterval;
        }

        void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = accum / frames;
                int antialiasing = QualitySettings.antiAliasing;
                int vsync = QualitySettings.vSyncCount;

                TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
                text.text = fps + " FPS";

                if (fps < 30)
                {
                    text.color = Color.yellow;
                }                    
                else
                {
                    text.color = fps < 10 ? Color.red : Color.green;
                }

                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
    }
}