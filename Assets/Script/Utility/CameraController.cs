using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float minZoom = 50f;
    [SerializeField] private float maxZoom = 800f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // 矢印キーやWASDでカメラを移動
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime * (cam.orthographicSize / 200f);
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime * (cam.orthographicSize / 200f);
        
        transform.position += new Vector3(moveX, moveY, 0);

        // マウスホイールでズームイン・ズームアウト
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }
}