using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    public class PixelateTransition : ITransitionKitDelegate
    {
        public enum PixelateFinalScaleEffect
        {
            ToPoint,
            Zoom,
            Horizontal,
            Vertical
        }

        // settings for the pixellate shader
        public float pixellateMin = 0.001f;
        public float pixellateMax = 0.08f;
        public float duration = 0.6f;
        public float pixelatedDelay = 0f;
        public PixelateFinalScaleEffect finalScaleEffect = PixelateFinalScaleEffect.ToPoint;
        public int nextScene = -1;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Pixelate");
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
            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;

            var startValue = pixellateMin;
            var endValue = pixellateMax;

            transitionKit.material.SetFloat("_WidthAspectMultiplier", 1f / Camera.main.aspect);

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += transitionKit.DeltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                transitionKit.material.SetFloat("_CellSize", Mathf.Lerp(startValue, endValue, step));

                yield return null;
            }

            if (pixelatedDelay > 0)
                yield return new WaitForSeconds(pixelatedDelay);

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));

            var desiredScale = Vector3.zero;
            switch (finalScaleEffect)
            {
                case PixelateFinalScaleEffect.ToPoint:
                    desiredScale = new Vector3(0f, 0f, transitionKit.transform.localScale.z);
                    break;
                case PixelateFinalScaleEffect.Zoom:
                    desiredScale = new Vector3(transitionKit.transform.localScale.x * 5f, transitionKit.transform.localScale.y * 5f, transitionKit.transform.localScale.z);
                    break;
                case PixelateFinalScaleEffect.Horizontal:
                    desiredScale = new Vector3(transitionKit.transform.localScale.x, 0, transitionKit.transform.localScale.z);
                    break;
                case PixelateFinalScaleEffect.Vertical:
                    desiredScale = new Vector3(0, transitionKit.transform.localScale.y, transitionKit.transform.localScale.z);
                    break;
            }

            yield return transitionKit.StartCoroutine(AnimateScale(transitionKit, duration * 0.5f, desiredScale));
        }

        public IEnumerator AnimateScale(TransitionManager transitionKit, float duration, Vector3 desiredScale)
        {
            var originalScale = transitionKit.transform.localScale;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += transitionKit.DeltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                transitionKit.transform.localScale = Vector3.Lerp(originalScale, desiredScale, step);

                yield return null;
            }
        }
    }
}
