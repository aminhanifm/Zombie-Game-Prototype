using UnityEngine;
using TMPro;

namespace ZGP.Game
{
    public class FPSCounter : MonoBehaviour
    {
        private TMP_Text fpsText;
        private int frames = 0;
        private float deltaTime = 0.0f;
        [Tooltip("FPS Update Delay")]
        public float updateDelay = 0.5f;


        private void Awake()
        {
            fpsText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            deltaTime += Time.deltaTime;
            frames++;

            if (deltaTime >= updateDelay)
            {
                float fps = frames / deltaTime;

                fpsText.SetText($"FPS: {Mathf.Round(fps)}");

                frames = 0;
                deltaTime = 0.0f;
            }
        }
    }
}
