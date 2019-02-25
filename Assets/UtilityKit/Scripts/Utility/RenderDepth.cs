using UnityEngine;

namespace MCFramework
{
    [ExecuteInEditMode]
    public class RenderDepth : MonoBehaviour
    {
        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        }
    }
}

