using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    /// <summary>
    /// the maskTexture will show the background image (screen grab) where it is transparent and the backgroundColor where it is not.
    /// it zooms to a point in the center of the screen then fades back in after the new scene loads.
    /// </summary>
    public class ImageMaskTransition : ITransitionKitDelegate
    {
        public Texture2D maskTexture;
        public Color backgroundColor = Color.black;
        public float duration = 0.9f;
        public int nextScene = -1;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Mask");
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
            transitionKit.material.color = backgroundColor;
            transitionKit.material.SetTexture("_MaskTex", maskTexture);

            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            // this does the zoom/rotation
            yield return transitionKit.StartCoroutine(transitionKit.TickProgressPropertyInMaterial(duration));

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));

            // now that the new scene is loaded we zoom the mask back out
            transitionKit.MakeTextureTransparent();

            yield return transitionKit.StartCoroutine(transitionKit.TickProgressPropertyInMaterial(duration, true));
        }
    }
}