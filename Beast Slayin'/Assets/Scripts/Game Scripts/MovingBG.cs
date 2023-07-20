using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBG : MonoBehaviour
{
    [SerializeField] private float scrollSpeed; // Speed at which the background will scroll
    private float length;
    private float startPos;

    private void Start()
    {
        startPos = transform.position.x;
        length = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }

    private void Update()
    {
        // Move the background to the left based on the scroll speed
        float newPos = Mathf.Repeat(Time.time * scrollSpeed, length);
        transform.position = new Vector3(startPos - newPos, transform.position.y, transform.position.z);
    }
}
