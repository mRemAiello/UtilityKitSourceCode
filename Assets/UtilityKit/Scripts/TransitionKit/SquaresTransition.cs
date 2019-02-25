using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class SquaresTransition : ITransitionKitDelegate
    {
        public Color squareColor = Color.black;
        public float duration = 1.0f;
        public float fadedDelay = 0f;
        public int nextScene = -1;
        public Vector2 squareSize = new Vector2(13f, 9f);
        public float smoothness = 0.5f;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Squares");
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
            transitionKit.material.color = squareColor;
            transitionKit.material.SetFloat("_Smoothness", smoothness);
            transitionKit.material.SetVector("_Size", squareSize);

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