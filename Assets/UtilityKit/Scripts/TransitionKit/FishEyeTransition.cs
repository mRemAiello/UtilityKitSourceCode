using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class FishEyeTransition : ITransitionKitDelegate
    {
        public float duration = 0.5f;
        public int nextScene = -1;
        public float size = 0.2f;
        public float zoom = 100.0f;
        public float colorSeparation = 0.2f;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Fish Eye");
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
            transitionKit.material.SetFloat("_Zoom", zoom);
            transitionKit.material.SetFloat("_ColorSeparation", zoom);

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