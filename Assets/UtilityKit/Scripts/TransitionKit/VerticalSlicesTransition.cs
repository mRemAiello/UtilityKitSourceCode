using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MCFramework
{
    public class VerticalSlicesTransition : ITransitionKitDelegate
    {
        public float duration = 0.5f;
        public int nextScene = -1;
        public int divisions = 5;

        private QuadSlice[] m_QuadSlices;

        private class QuadSlice
        {
            private readonly int[] m_VertIndices = new int[4];
            private readonly Vector3[] m_InitialPositions = new Vector3[4];

            public QuadSlice(int firstVertIndex, Vector3[] verts)
            {
                for (var i = 0; i < 4; i++)
                {
                    m_VertIndices[i] = firstVertIndex + i;
                    m_InitialPositions[i] = verts[m_VertIndices[i]];
                }
            }

            public void shiftVerts(Vector3 offset, Vector3[] verts)
            {
                for (var i = 0; i < 4; i++)
                    verts[m_VertIndices[i]] = m_InitialPositions[i] + offset;
            }
        }

        public Shader ShaderForTransition()
        {
            return null;
        }

        public Mesh MeshForDisplay()
        {
            // we need at least 2 divisions
            if (divisions < 2)
                divisions = 2;

            m_QuadSlices = new QuadSlice[divisions];
            var mesh = new Mesh();

            // figure out how many verts and triangles we need
            var vertsPerRow = divisions * 2;
            var numTriangles = divisions * 6; // 2 tris per division slice with 3 verts each
            var numVertices = vertsPerRow * 2; // top and bottom rows

            var verts = new Vector3[numVertices];
            var uvs = new Vector2[numVertices];
            var tris = new int[numTriangles];

            // so, our verts need to go from -halfWidth to halfWidth and -halfHeight to halfHeight
            var halfHeight = 5f; // 5 is the camera.orthoSize which is half the screen height
            var halfWidth = halfHeight * (Screen.width / (float)Screen.height);
            var width = halfWidth * 2f;
            var divisionWidth = (1.0f / divisions) * width;
            var divisionWidthFraction = divisionWidth / width; // width of a slice normalized from 0 to 1 for uv generation

            // create our verts, tris and uvs
            var index = 0;
            var triIndex = 0;
            for (var i = 0; i < divisions; i++)
            {
                var rootVertIndex = i * 4; // first vert index in each loop iteration
                var xMin = i * divisionWidth - halfWidth;
                var xMax = xMin + divisionWidth;
                var uvMin = i * divisionWidthFraction;
                var uvMax = uvMin + divisionWidthFraction;

                verts[index++] = new Vector3(xMin, -halfHeight, 0);
                verts[index++] = new Vector3(xMin, halfHeight, 0);
                verts[index++] = new Vector3(xMax, -halfHeight, 0);
                verts[index++] = new Vector3(xMax, halfHeight, 0);

                tris[triIndex++] = 0 + rootVertIndex;
                tris[triIndex++] = 1 + rootVertIndex;
                tris[triIndex++] = 2 + rootVertIndex;
                tris[triIndex++] = 3 + rootVertIndex;
                tris[triIndex++] = 2 + rootVertIndex;
                tris[triIndex++] = 1 + rootVertIndex;

                uvs[rootVertIndex + 0] = new Vector2(uvMin, 0);
                uvs[rootVertIndex + 1] = new Vector2(uvMin, 1);
                uvs[rootVertIndex + 2] = new Vector2(uvMax, 0);
                uvs[rootVertIndex + 3] = new Vector2(uvMax, 1);

                m_QuadSlices[i] = new QuadSlice(rootVertIndex, verts);
            }

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;

            return mesh;
        }

        public Texture2D TextureForDisplay()
        {
            return null;
        }

        public IEnumerator OnScreenObscured(TransitionManager transitionKit)
        {
            transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
            {
                SceneManager.LoadSceneAsync(nextScene);
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));
            }

            var transitionDistance = transitionKit.GetComponent<Camera>().orthographicSize * 2f; // 2x our camera.orthoSize so we move the slices off screen
            var elapsed = 0f;
            var mesh = transitionKit.GetComponent<MeshFilter>().mesh;
            var verts = mesh.vertices;

            while (elapsed < duration)
            {
                elapsed += transitionKit.DeltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                var offset = Mathf.Lerp(0, transitionDistance, step);

                // transition our QuadSlices
                for (var i = 0; i < m_QuadSlices.Length; i++)
                {
                    // odd ones move up, even down
                    var sign = (i % 2 == 0) ? 1f : -1f;
                    m_QuadSlices[i].shiftVerts(new Vector3(0, offset * sign), verts);
                }

                // reassign our verts
                mesh.vertices = verts;

                yield return null;
            }
        }
    }
}