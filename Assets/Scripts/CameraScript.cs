using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Vector4 bounds;
    [SerializeField] private Vector3 offset;
    private Vector3 wantedPos;

    private void Update()
    {
        wantedPos = target.position + offset;

        wantedPos = new Vector3(Mathf.Clamp(wantedPos.x, bounds.x, bounds.y),
            Mathf.Clamp(wantedPos.y, bounds.z, bounds.w), offset.z);

        transform.position = Vector3.Lerp(transform.position, wantedPos, lerpSpeed);
    }
}
