using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    private Transform cam;
    public float parallaxEffect;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }

    void Update()
    {
        float RePos = cam.transform.position.x * (1 - parallaxEffect);
        float Distance = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startpos + Distance, transform.position.y, transform.position.z);

        if (RePos > startpos + length)
        {
            startpos += length;
        }
        else if (RePos < startpos - length)
        {
            startpos -= length;
        }
    }
}
