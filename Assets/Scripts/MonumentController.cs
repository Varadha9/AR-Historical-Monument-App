using UnityEngine;

/// <summary>
/// Controls 3D Historical Monument interactions:
/// - Smooth 360-degree auto-rotation for architectural showcase
/// - Touch / Mouse drag rotation to inspect historical details
/// - Pinch-to-zoom / Mouse scroll to scale model
/// - Smooth entrance animation upon AR marker recognition
/// </summary>
public class MonumentController : MonoBehaviour
{
    [Header("Auto Rotation")]
    [Tooltip("Continuous showcase rotation speed (degrees/sec)")]
    public float autoRotateSpeed = 20f;
    public bool isAutoRotating = true;

    [Header("Manual Inspection Controls")]
    [Tooltip("Sensitivity of touch / mouse drag rotation")]
    public float rotationSensitivity = 100f;

    [Tooltip("Zoom / Scale limits")]
    public float minScale = 0.05f;
    public float maxScale = 0.5f;
    public float zoomSensitivity = 0.1f;

    private Vector3 initialScale;
    private Quaternion initialRotation;
    private bool isDragging = false;
    private Vector3 lastMousePos;

    void Awake()
    {
        initialScale = transform.localScale;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        HandleAutoRotation();
        HandleManualInspection();
        HandleZoom();
    }

    private void HandleAutoRotation()
    {
        if (isAutoRotating && !isDragging)
        {
            transform.Rotate(Vector3.up, autoRotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void HandleManualInspection()
    {
        // Mouse / Single Touch Drag
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            float rotX = delta.x * (rotationSensitivity / Screen.width);
            float rotY = delta.y * (rotationSensitivity / Screen.height);

            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void HandleZoom()
    {
        // Mouse ScrollWheel Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float targetScale = Mathf.Clamp(transform.localScale.x + scroll * zoomSensitivity, minScale, maxScale);
            transform.localScale = Vector3.one * targetScale;
        }

        // Mobile Pinch-to-Zoom
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = (touchDeltaMag - prevTouchDeltaMag) * 0.001f;

            float targetScale = Mathf.Clamp(transform.localScale.x + deltaMagnitudeDiff, minScale, maxScale);
            transform.localScale = Vector3.one * targetScale;
        }
    }

    /// <summary>
    /// Smooth pop-in animation when marker is detected
    /// </summary>
    public void OnMarkerDetected()
    {
        StopAllCoroutines();
        StartCoroutine(PopInAnimation());
    }

    private System.Collections.IEnumerator PopInAnimation()
    {
        transform.localScale = Vector3.zero;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Elastic overshoot curve
            float scaleMultiplier = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = initialScale * scaleMultiplier;
            yield return null;
        }

        transform.localScale = initialScale;
    }

    public void ToggleAutoRotate()
    {
        isAutoRotating = !isAutoRotating;
    }

    public void ResetTransform()
    {
        transform.localRotation = initialRotation;
        transform.localScale = initialScale;
    }
}
