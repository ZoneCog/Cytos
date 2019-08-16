using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Class
{
    public class ListObjects
    {
        public ListObjects(int stepId, string objectId, string objectName, string objectType, List<Vector3> listOfVertexs, Color nameColor, float thickness)
        {
            StepId = stepId;
            ObjectId = objectId;
            ObjectName = objectName;
            ObjectType = objectType;
            ListOfVertexs = listOfVertexs;
            NameColor = nameColor;
            Thickness = thickness;
        }
        public int StepId { set; get; }
        public string ObjectId { set; get; }
        public string ObjectName { set; get; }
        public string ObjectType { set; get; }
        public List<Vector3> ListOfVertexs { set; get; }
        public Color NameColor { set; get; }
        public float Thickness { set; get; }
    }
}
