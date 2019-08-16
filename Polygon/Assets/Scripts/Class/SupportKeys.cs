using UnityEngine;

namespace Assets.Scripts.Class
{
    public class SupportKeys : MonoBehaviour {
	
        // Update is called once per frame
        void Update () {
            if (Input.GetKey("escape"))
                Application.Quit();
        }
    }
}
