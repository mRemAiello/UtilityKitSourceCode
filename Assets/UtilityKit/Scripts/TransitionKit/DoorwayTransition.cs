﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    /// <summary>
    /// the Doorway shader can have _Progress run from 0 to -1 or 0 to 1. The runEffectInReverse controls that.
    /// </summary>
    public class DoorwayTransition : ITransitionKitDelegate
    {
        public float duration = 0.5f;
        public int nextScene = -1;
        public float perspective = 1.5f;
        public float depth = 3.0f;
        public bool runEffectInReverse = false;

        public Shader ShaderForTransition()
        {
            return Shader.Find("Transitions/Doorway");
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
            transitionKit.material.SetFloat("_Perspective", perspective);
            transitionKit.material.SetFloat("_Depth", depth);
            transitionKit.material.SetInt("_Direction", runEffectInReverse ? 1 : 0);

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
