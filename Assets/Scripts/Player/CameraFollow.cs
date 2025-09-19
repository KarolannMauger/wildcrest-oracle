using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; // player

    [Header("Offset camera")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smooth camera movement")]
    public float smoothSpeed = 5f;

    [Header("Zoom camera")]
    public float orthoSize = 3.5f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = orthoSize;

        if (offset.z == 0) offset.z = -10f; // safety check to avoid black screen
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 desired = target.position + offset;

        // Smooth interpolation
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        transform.position = smoothed;
    }
}
