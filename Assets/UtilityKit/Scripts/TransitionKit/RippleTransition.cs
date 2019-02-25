using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class RippleTransition : ITransitionKitDelegate
    {
        public float duration = 0.5f;
        public int nextScene = -1;
        public float speed = 50.0f;
        public float amplitude = 100.0f;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Ripple");
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
            transitionKit.material.SetFloat("_Speed", speed);
            transitionKit.material.SetFloat("_Amplitude", amplitude);

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