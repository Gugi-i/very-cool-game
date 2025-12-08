using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect; // 1 = moves with cam, 0 = moves normal

    void Start()
    {
        startpos = transform.position.x;
        // Get width of the sprite for looping
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Calculate distance background should move relative to camera
        float dist = (cam.transform.position.x * parallaxEffect);
        // Calculate temporary position to check for looping
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // Move the layer
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // Check if we need to loop the background
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}