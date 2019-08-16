using UnityEngine;

// Very simple smooth mouselook modifier for the MainCamera in Unity
// by Francis R. Griffiths-Keam - www.runningdimensions.com

[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class MouseLook : MonoBehaviour
{
    Vector2 v_MouseAbsolute;
    Vector2 v_SmoothMouse;

    public Vector2 ClampInDegrees = new Vector2(360, 180);
    public bool LockCursor;
    public Vector2 Sensitivity = new Vector2(2, 2);
    public Vector2 Smoothing = new Vector2(3, 3);
    public Vector2 TargetDirection;
    public Vector2 TargetCharacterDirection;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject characterBody;

    void Start()
    {
        // Set target direction to the camera's initial orientation.
        TargetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody) TargetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
    }

    void Update()
    {
        // Ensure the cursor is always locked when set
        if (LockCursor)
            Cursor.lockState = CursorLockMode.Locked;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(TargetDirection);
        var targetCharacterOrientation = Quaternion.Euler(TargetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the Sensitivity setting and multiply that against the Smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(Sensitivity.x * Smoothing.x, Sensitivity.y * Smoothing.y));

        // Interpolate mouse movement over time to apply Smoothing delta.
        v_SmoothMouse.x = Mathf.Lerp(v_SmoothMouse.x, mouseDelta.x, 1f / Smoothing.x);
        v_SmoothMouse.y = Mathf.Lerp(v_SmoothMouse.y, mouseDelta.y, 1f / Smoothing.y);

        // Find the absolute mouse movement value from point zero.
        v_MouseAbsolute += v_SmoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (ClampInDegrees.x < 360)
            v_MouseAbsolute.x = Mathf.Clamp(v_MouseAbsolute.x, -ClampInDegrees.x * 0.5f, ClampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (ClampInDegrees.y < 360)
            v_MouseAbsolute.y = Mathf.Clamp(v_MouseAbsolute.y, -ClampInDegrees.y * 0.5f, ClampInDegrees.y * 0.5f);

        var xRotation = Quaternion.AngleAxis(-v_MouseAbsolute.y, targetOrientation * Vector3.right);
        transform.localRotation = xRotation * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(v_MouseAbsolute.x, characterBody.transform.up);
            characterBody.transform.localRotation = yRotation;
            characterBody.transform.localRotation *= targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(v_MouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}