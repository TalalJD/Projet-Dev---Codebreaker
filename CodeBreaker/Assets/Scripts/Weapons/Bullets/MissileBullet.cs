using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBullet : MonoBehaviour
{
    public float speed = 10f;
    public float arcHeight = 2f;
    private Vector2 startPos;
    private Vector2 targetPos;
    private float journeyLength;
    private float startTime;

    public void Initialize(Vector2 target)
    {
        startPos = transform.position;
        targetPos = target;
        startTime = Time.time;
        journeyLength = Vector2.Distance(startPos, targetPos);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fraction = distCovered / journeyLength;

        // simple parabolic arc
        Vector2 currentPos = Vector2.Lerp(startPos, targetPos, fraction);
        currentPos.y += Mathf.Sin(fraction * Mathf.PI) * arcHeight;

        transform.position = currentPos;

        
    }
}
