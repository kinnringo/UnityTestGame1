using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    public Transform target; // 注視点
    public float zoomSpeed = 5f;
    public float rotateSpeed = 5f;
    public float panSpeed = 0.5f;
    public float minDistance = 5f;
    public float maxDistance = 30f;

    private float distance;
    private Vector3 lastMousePos;

    void Start()
    {
        if (target == null)
            target = new GameObject("CameraTarget").transform;

        distance = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {
        HandleZoom();
        HandleRotation();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        transform.position = target.position - transform.forward * distance;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            Quaternion yaw = Quaternion.AngleAxis(delta.x * rotateSpeed * Time.deltaTime, Vector3.up);
            Quaternion pitch = Quaternion.AngleAxis(-delta.y * rotateSpeed * Time.deltaTime, transform.right);

            Vector3 dir = transform.position - target.position;
            dir = yaw * pitch * dir;
            transform.position = target.position + dir;
            transform.LookAt(target.position);
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            Vector3 right = transform.right;
            Vector3 forward = Vector3.Cross(Vector3.up, right); // XZ平面のforward

            // 上下方向のドラッグを反転
            Vector3 move = (-right * delta.x + forward * delta.y) * panSpeed * Time.deltaTime;

            move.y = 0; // Y固定
            transform.position += move;
            target.position += move;
        }
    }


}
