using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    public class TransitionManager : PersistentSingleton<TransitionManager>
    {
        /// <summary>
        /// fired when the screen has been fully obscured. You are clear to "do stuff" if need be when this fires
        /// </summary>
        public static event Action OnScreenObscured;

        /// <summary>
        /// fired when the transition is complete and TransitionKit has destroyed all of its objects
        /// </summary>
        public static event Action OnTransitionComplete;

        private ITransitionKitDelegate m_TransitionKitDelegate;

        /// <summary>
        /// provides easy access to the camera used to obscure the screen. Handy when you want to change the clear flags for example.
        /// </summary>
        [HideInInspector]
        public Camera transitionKitCamera;

        /// <summary>
        /// material access for delegates so they can mess with shader/material properties
        /// </summary>
        [HideInInspector]
        public Material material;

        /// <summary>
        /// sets whether TransitionKit will use unscaledDeltaTime or standard deltaTime
        /// </summary>
        [HideInInspector]
        public bool useUnscaledDeltaTime = false;

        /// <summary>
        /// helper property for use by all TransitionKitDelegates so they use the proper deltaTime
        /// </summary>
        /// <value>The delta time.</value>
        public float DeltaTime
        {
            get { return useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime; }
        }

        /// <summary>
        /// stick whatever you want in there so that when the events fire you can grab it and avoid the Action allocations
        /// </summary>
        public object context;

        private T GetOrAddComponent<T>() where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.AddComponent<T>();

            return component;
        }

        private void Initialize()
        {
            // create the MeshFilter
            var meshFilter = GetOrAddComponent<MeshFilter>();
            meshFilter.mesh = m_TransitionKitDelegate.MeshForDisplay() ?? GenerateQuadMesh();

            // create the Material
            material = GetOrAddComponent<MeshRenderer>().material;
            material.shader = m_TransitionKitDelegate.ShaderForTransition() ?? Shader.Find("Transitions/Texture With Alpha");

            // snapshot the main camera before proceeding
            Instance.StartCoroutine(Instance.SetupCameraAndTexture());
        }

        private Mesh GenerateQuadMesh()
        {
            var halfHeight = 5f; // 5 is the camera.orthoSize which is the half height
            var halfWidth = halfHeight * (Screen.width / (float)Screen.height);

            var mesh = new Mesh
            {
                vertices = new Vector3[]
            {
                new Vector3( -halfWidth, -halfHeight, 0 ),
                new Vector3( -halfWidth, halfHeight, 0 ),
                new Vector3( halfWidth, -halfHeight, 0 ),
                new Vector3( halfWidth, halfHeight, 0 )
            },
                uv = new Vector2[]
            {
                new Vector2( 0, 0 ),
                new Vector2( 0, 1 ),
                new Vector2( 1, 0 ),
                new Vector2( 1, 1 )
            },
                triangles = new int[] { 0, 1, 2, 3, 2, 1 }
            };

            return mesh;
        }

        private IEnumerator SetupCameraAndTexture()
        {
            yield return new WaitForEndOfFrame();

            // load up the texture
            material.mainTexture = m_TransitionKitDelegate.TextureForDisplay() ?? GetScreenshotTexture();

            // create our camera to cover the screen
            transitionKitCamera = GetOrAddComponent<Camera>();

            // always reset these in case a transition messed with them
            transitionKitCamera.orthographic = true;
            transitionKitCamera.nearClipPlane = -1f;
            transitionKitCamera.farClipPlane = 1f;
            transitionKitCamera.depth = float.MaxValue;
            transitionKitCamera.cullingMask = 1 << 31;
            transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
            transitionKitCamera.allowMSAA = false;
            transitionKitCamera.enabled = true;

            OnScreenObscured?.Invoke();

            yield return StartCoroutine(m_TransitionKitDelegate.OnScreenObscured(this));

            Cleanup();
        }

        private Texture2D GetScreenshotTexture()
        {
            var screenSnapshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, false);
            screenSnapshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            screenSnapshot.Apply();

            return screenSnapshot;
        }

        /// <summary>
        /// this method signals a cleanup (duh) and notifies event listeners
        /// </summary>
        private void Cleanup()
        {
            if (Instance == null)
                return;

            OnTransitionComplete?.Invoke();

            m_TransitionKitDelegate = null;
            context = null;
            GetComponent<MeshRenderer>().material.mainTexture = null;
            GetComponent<MeshFilter>().mesh = null;
            gameObject.SetActive(false);
            transitionKitCamera.enabled = false;
        }

        private void Start()
        {
        }

        /// <summary>
        /// starts up a transition with the given delegate
        /// </summary>
        /// <param name="transitionKitDelegate">Transition kit delegate.</param>
        public static void TransitionWithDelegate(ITransitionKitDelegate transitionKitDelegate)
        {
            Instance.gameObject.SetActive(true);
            Instance.m_TransitionKitDelegate = transitionKitDelegate;
            Instance.Initialize();
        }

        /// <summary>
        /// makes a single pixel Texture2D with a transparent pixel and sets it on the current Material. Useful for fading from obscured to a
        /// new scene. Note that of course your shader must support transparency for this to be useful
        /// </summary>
        public void MakeTextureTransparent()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.clear);
            tex.Apply();

            Instance.material.mainTexture = tex;
        }

        /// <summary>
        /// helper for delegates that returns control back when the given level has loaded. Very handy when using async loading.
        /// </summary>
        /// <returns>The for level to load.</returns>
        /// <param name="level">Level.</param>
        public IEnumerator WaitForLevelToLoad(int level)
        {
            while (SceneManager.GetActiveScene().buildIndex != level)
                yield return null;
        }

        /// <summary>
        /// the most common type of transition seems to be one that ticks progress from 0 - 1. This method takes care of that for you
        /// if your transition needs to have a _Progress property ticked after the scene loads.
        /// </summary>
        /// <param name="duration">duration</param>
        /// <param name="reverseDirection">if true, _Progress will go from 1 to 0. If false, it goes form 0 to 1</param>
        public IEnumerator TickProgressPropertyInMaterial(float duration, bool reverseDirection = false)
        {
            var start = reverseDirection ? 1f : 0f;
            var end = reverseDirection ? 0f : 1f;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Instance.DeltaTime;
                var step = Mathf.Lerp(start, end, Mathf.Pow(elapsed / duration, 2f));
                Instance.material.SetFloat("_Progress", step);

                yield return null;
            }
        }
    }
}