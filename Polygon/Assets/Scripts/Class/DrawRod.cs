using UnityEngine;

namespace Assets.Scripts.Class
{
    public class DrawRod
    {

        public void DrawOneRod(Vector3 startPosition, Vector3 endPosition, string rodId, string rodName, float dia, Color color)
        {
            GameObject o = new GameObject
            {
                name = string.Format("{0} {1}", rodId, rodName),
                tag = "ObjectInSpace"
            };
            LineRenderer lr = o.AddComponent<LineRenderer>();

            Vector3[] positions = new Vector3[2];
            positions[0] = startPosition;
            positions[1] = endPosition;

            lr.material.color = color;

            lr.startWidth = dia;
            lr.endWidth = dia;

            lr.positionCount = positions.Length;
            lr.SetPositions(positions);
            
            lr.numCapVertices = 90;
        }
    }
}
