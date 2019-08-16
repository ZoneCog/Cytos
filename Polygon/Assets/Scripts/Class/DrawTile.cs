using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Class
{
    public class DrawTile
    {
        // Use this for initialization
        private List<Vector3> v_NodePositions;
        private string v_NameOfTile;
        private string v_IdOfTile;

        #region Public methods
        public void DrawOneTile(List<Vector3> nodePosition, string tileId, string tileName, Color color)
        {

            v_NodePositions = nodePosition;
            v_IdOfTile = tileId;
            v_NameOfTile = tileName;
            CreatePolygon(color);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Create polygon (gameobject) with mesh
        /// </summary>
        /// <param name="color">Color</param>
        private void CreatePolygon(Color color)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject o = new GameObject();
                o.name = string.Format(i == 0 ? "{0} {1} F" : "{0} {1} B", v_IdOfTile, v_NameOfTile);

                MeshFilter mf = o.AddComponent<MeshFilter>();
                MeshRenderer mr = o.AddComponent<MeshRenderer>();

                o.tag = "ObjectInSpace";

                var mesh = CreateMesh(i);

                Material materal = new Material(Shader.Find("Transparent/Diffuse"));
                mr.material = materal;
                mr.material.color = color;
          
                mf.mesh = mesh;
            }
        }

        /// <summary>
        /// Create Mesh
        /// </summary>
        /// <param name="num">Front side or Back side</param>
        /// <returns>Mesh</returns>
        private Mesh CreateMesh(int num)
        {
            int x;
            Mesh mesh = new Mesh();

            Vector3[] vertex = new Vector3[v_NodePositions.Count];
            for (x = 0; x < v_NodePositions.Count; x++)
            {
                vertex[x] = v_NodePositions[x];
            }

            Vector2[] uvs = new Vector2[vertex.Length];
            for (x = 0; x < vertex.Length; x++)
            {
                if (x % 2 == 0)
                {
                    uvs[x] = new Vector2(0, 0);
                }
                else
                {
                    uvs[x] = new Vector2(1, 1);
                }
            }
            int[] tris = new int[3 * (vertex.Length - 2)];
            int c1;
            int c2;
            int c3;

            if (num == 0)
            {
                c1 = 0;
                c2 = 1;
                c3 = 2;

                for (x = 0; x < tris.Length; x += 3)
                {
                    tris[x] = c1;
                    tris[x + 1] = c2;
                    tris[x + 2] = c3;

                    c2++;
                    c3++;
                }
            }
            else
            {
                c1 = 0;
                c2 = vertex.Length - 1;
                c3 = vertex.Length - 2;

                for (x = 0; x < tris.Length; x += 3)
                {
                    tris[x] = c1;
                    tris[x + 1] = c2;
                    tris[x + 2] = c3;

                    c2--;
                    c3--;
                }
            }

            mesh.vertices = vertex;
            mesh.uv = uvs;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mesh.name = "TileMesh";

            return mesh;
        }
    }
    #endregion

}