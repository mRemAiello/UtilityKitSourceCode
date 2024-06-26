﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    /// <summary>
    /// this is an example of how you can use a standard MonoBehaviour subclass to be able to edit values via the Inspector.
    /// the twirl shader is pulled directly from Unity's image effects.
    /// </summary>
    public class TwirlTransition : ITransitionKitDelegate
    {
        public Shader twirlShader;
        public float endAngle = 1800f;
        public float duration = 0.5f;
        public Vector2 center = new Vector2(0.5f, 0.5f);
        public Vector2 radius = new Vector2(0.3f, 0.3f);
        public int nextScene = -1;

        private float m_Angle = 0f;

        public Shader ShaderForTransition()
        {
            return twirlShader;
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
                elapsed += Time.deltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                m_Angle = Mathf.Lerp(0f, endAngle, step);

                var rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, m_Angle), Vector3.one);
                transitionKit.material.SetMatrix("_RotationMatrix", rotationMatrix);
                transitionKit.material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
                transitionKit.material.SetFloat("_Angle", m_Angle * Mathf.Deg2Rad);

                yield return null;
            }

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));
        }
    }
}

