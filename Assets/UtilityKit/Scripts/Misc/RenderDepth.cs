using UnityEngine;

namespace UtilityKit
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

