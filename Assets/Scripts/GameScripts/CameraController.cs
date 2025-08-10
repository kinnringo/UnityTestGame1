using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform mapParent; // MapManagerなど
    public float height = 10f;
    public float distance = 10f;

    void Start()
    {
        StartCoroutine(InitCamera());
    }

    IEnumerator InitCamera()
    {
        yield return null; // 1フレーム待機

        Renderer[] renderers = mapParent.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("Rendererが見つかりません。MapManagerが空か、まだ生成されていません。");
            yield break;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        Vector3 center = bounds.center;
        Vector3 offset = new Vector3(0, 0, -1).normalized * distance;

        transform.position = center + offset + Vector3.up * height;
        transform.LookAt(center);
    }
}
