using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class BlurTransition : ITransitionKitDelegate
    {
        public float duration = 0.5f;
        public int nextScene = -1;
        public float blurMin = 0.0f;
        public float blurMax = 0.01f;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Blur");
        }

        public Mesh MeshForDisplay()
        {
            return null;
        }

        public Texture2D TextureForDisplay()
        {
            return null;
        }

        public IEnumerator OnScreenObscured(TransitionManager transitionKit)
        {
            transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;

            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += transitionKit.DeltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                var blurAmount = Mathf.Lerp(blurMin, blurMax, step);

                transitionKit.material.SetFloat("_BlurSize", blurAmount);

                yield return null;
            }

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));
        }
    }
}