using UnityEngine;

namespace Assets.Scripts
{
    public class Spectator : MonoBehaviour
    {

        //initial speed
        public int Speed = 30;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            //press shift to move faster
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Speed = 40;

            }
            else
            {
                //if shift is not pressed, reset to default speed
                Speed = 30;
            }
            //For the following 'if statements' don't include 'else if', so that the user can press multiple buttons at the same time
            //move camera to the left
            if (Input.GetKey(KeyCode.A))
            {
                transform.position = transform.position + Camera.main.transform.right * -1 * Speed * Time.deltaTime;
            }

            //move camera backwards
            if (Input.GetKey(KeyCode.S))
            {
                transform.position = transform.position + Camera.main.transform.forward * -1 * Speed * Time.deltaTime;

            }
            //move camera to the right
            if (Input.GetKey(KeyCode.D))
            {
                transform.position = transform.position + Camera.main.transform.right * Speed * Time.deltaTime;

            }
            //move camera forward
            if (Input.GetKey(KeyCode.W))
            {

                transform.position = transform.position + Camera.main.transform.forward * Speed * Time.deltaTime;
            }
            //move camera upwards
            if (Input.GetKey(KeyCode.E))
            {
                transform.position = transform.position + Camera.main.transform.up * Speed * Time.deltaTime;
            }
            //move camera downwards
            if (Input.GetKey(KeyCode.Q))
            {
                transform.position = transform.position + Camera.main.transform.up * -1 * Speed * Time.deltaTime;
            }

        }
    }
}