﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UtilityKit
{
    public class TriangleSlicesTransition : ITransitionKitDelegate
    {
        public float duration = 0.7f;
        public int nextScene = -1;
        public int divisions = 5;

        private TriangleSlice[] m_TriangleSlices;

        private class TriangleSlice
        {
            private readonly int[] m_VertIndices = new int[3];
            private readonly Vector3[] m_InitialPositions = new Vector3[3];

            public TriangleSlice(int firstVertIndex, Vector3[] verts)
            {
                for (var i = 0; i < 3; i++)
                {
                    m_VertIndices[i] = firstVertIndex + i;
                    m_InitialPositions[i] = verts[m_VertIndices[i]];
                }
            }

            public void ShiftVerts(Vector3 offset, Vector3[] verts)
            {
                for (var i = 0; i < 3; i++)
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

            m_TriangleSlices = new TriangleSlice[divisions * 2];
            var mesh = new Mesh();

            // figure out how many verts and triangles we need
            var numTriangles = divisions * 6; // 2 tris per division slice with 3 verts each
            var numVertices = numTriangles * 3; // 3 verts per tri and we need them all separate

            var verts = new Vector3[numVertices];
            var uvs = new Vector2[numVertices];
            var tris = new int[numTriangles];

            // so, our verts need to go from -halfWidth to halfWidth and -halfHeight to halfHeight
            var halfHeight = 5f; // 5 is the camera.orthoSize which is half the screen height
            var halfWidth = halfHeight * (Screen.width / (float)Screen.height);
            var width = halfWidth * 2f;
            var divisionWidth = 1.0f / divisions * width;
            var divisionWidthFraction = divisionWidth / width; // width of a slice normalized from 0 to 1 for uv generation

            // create our verts, tris and uvs
            var index = 0;
            var triIndex = 0;
            for (var i = 0; i < divisions; i++)
            {
                var rootVertIndex = i * 6; // first vert index in each loop iteration
                var xMin = i * divisionWidth - halfWidth;
                var xMax = xMin + divisionWidth;
                var uvMin = i * divisionWidthFraction;
                var uvMax = uvMin + divisionWidthFraction;

                verts[index++] = new Vector3(xMin, -halfHeight, 0); // 0
                verts[index++] = new Vector3(xMin, halfHeight, 0); // 1
                verts[index++] = new Vector3(xMax, -halfHeight, 0); // 2

                verts[index++] = new Vector3(xMax, halfHeight, 0); // 3
                verts[index++] = new Vector3(xMax, -halfHeight, 0); // 2 dupe
                verts[index++] = new Vector3(xMin, halfHeight, 0); // 1 dupe

                tris[triIndex++] = 0 + rootVertIndex;
                tris[triIndex++] = 1 + rootVertIndex;
                tris[triIndex++] = 2 + rootVertIndex;
                tris[triIndex++] = 3 + rootVertIndex;
                tris[triIndex++] = 4 + rootVertIndex;
                tris[triIndex++] = 5 + rootVertIndex;

                uvs[rootVertIndex + 0] = new Vector2(uvMin, 0);
                uvs[rootVertIndex + 1] = new Vector2(uvMin, 1);
                uvs[rootVertIndex + 2] = new Vector2(uvMax, 0);

                uvs[rootVertIndex + 3] = new Vector2(uvMax, 1);
                uvs[rootVertIndex + 4] = new Vector2(uvMax, 0);
                uvs[rootVertIndex + 5] = new Vector2(uvMin, 1);

                m_TriangleSlices[i * 2] = new TriangleSlice(rootVertIndex, verts);
                m_TriangleSlices[i * 2 + 1] = new TriangleSlice(rootVertIndex + 3, verts);
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

            if (nextScene >= 0)
                SceneManager.LoadSceneAsync(nextScene);

            // we dont transition back to the new scene unless it is loaded
            if (nextScene >= 0)
                yield return transitionKit.StartCoroutine(transitionKit.WaitForLevelToLoad(nextScene));

            var transitionDistance = 10f; // 2x our camera.orthoSize so we move the slices off screen
            var elapsed = 0f;
            var mesh = transitionKit.GetComponent<MeshFilter>().mesh;
            var verts = mesh.vertices;

            while (elapsed < duration)
            {
                elapsed += transitionKit.DeltaTime;
                var step = Mathf.Pow(elapsed / duration, 2f);
                var offset = Mathf.Lerp(0, transitionDistance, step);

                // transition our TriangleSlices
                for (var i = 0; i < m_TriangleSlices.Length; i++)
                {
                    // odd ones move down, even up
                    var sign = (i % 2 == 0) ? -1f : 1f;
                    m_TriangleSlices[i].ShiftVerts(new Vector3(0, offset * sign), verts);
                }

                // reassign our verts
                mesh.vertices = verts;

                yield return null;
            }
        }
    }
}