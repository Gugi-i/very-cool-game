using UnityEngine;

public class BackgroundFollowX : MonoBehaviour
{
    public Transform cam; // Drag your Main Camera here

    void LateUpdate()
    {
        // Lock X to the camera, Keep Y and Z exactly where they are
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);
    }
}