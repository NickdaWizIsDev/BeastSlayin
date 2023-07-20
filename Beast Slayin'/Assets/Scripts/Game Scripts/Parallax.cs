using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float length;
    private float startPos;
    [SerializeField] private float parallaxEffect;
    private GameObject cam;

    private void Start()
    {
        cam = GameObject.Find("CM vcam1");
        startPos = transform.position.x;
        length = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    private void Update()
    {
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        if(temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }
}
