using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class FadeTransition : ITransitionKitDelegate
    {
        public Color fadeToColor = Color.black;
        public float duration = 0.5f;
        /// <summary>
        /// the effect looks best when it pauses before fading back. When not doing a scene-to-scene transition you may want
        /// to pause for a breif moment before fading back.
        /// </summary>
        public float fadedDelay = 0f;
        public int nextScene = -1;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Fader");
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
            transitionKit.material.color = fadeToColor;

            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            yield return transitionKit.StartCoroutine(transitionKit.TickProgressPropertyInMaterial(duration));

            transitionKit.MakeTextureTransparent();

            if (fadedDelay > 0)
                yield return new WaitForSeconds(fadedDelay);

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));

            yield return transitionKit.StartCoroutine(transitionKit.TickProgressPropertyInMaterial(duration, true));
        }
    }
}