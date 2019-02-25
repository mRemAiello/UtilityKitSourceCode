using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class WindTransition : ITransitionKitDelegate
    {
        /// <summary>
        /// if true, the CurvedWind shader will be used which has the wind come from the top-left to bottom-left then across to the right.
        /// </summary>
        public bool useCurvedWind = false;
        public float duration = 0.5f;
        public int nextScene = -1;
        /// <summary>
        /// how much of the screen horizontally should the transition encompass? Higher numbers mean a wider transition.
        /// </summary>
        public float size = 0.3f;
        /// <summary>
        /// how many vertical sections of "wind" should we use? Higher numbers mean more whispy wind.
        /// </summary>
        public float windVerticalSegments = 100.0f;

        public Shader ShaderForTransition()
        {
            return useCurvedWind ? Shader.Find("Transitions/CurvedWind") : Shader.Find("Transitions/Wind");
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

            // set some material properties
            transitionKit.material.SetFloat("_Size", size);
            transitionKit.material.SetFloat("_WindVerticalSegments", windVerticalSegments);

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
            {
                SceneManager.LoadSceneAsync(nextScene);
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));
            }

            yield return transitionKit.StartCoroutine(transitionKit.TickProgressPropertyInMaterial(duration));
        }
    }
}