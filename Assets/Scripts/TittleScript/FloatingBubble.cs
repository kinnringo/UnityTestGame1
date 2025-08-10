using UnityEngine;

public class FloatingBubble : MonoBehaviour
{
    public float floatSpeed = 50f;                 // 上昇速度（Spawnerから渡される）
    public float lifeTime = 12f;                    // 最大生存時間（秒）
    public float popChancePerSecond = 0.005f;        // 1秒あたりの弾ける確率（0.5%）

    private RectTransform rect;
    private float timer = 0f;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 上方向に移動
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        // 毎秒の確率で弾ける
        if (Random.value < popChancePerSecond * Time.deltaTime)
        {
            Pop();
            return;
        }

        // 上限Y（画面外）または寿命オーバーで自動削除
        if (rect.anchoredPosition.y > 1000f || timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void Pop()
    {
        // ここにパーティクルやアニメ追加も可能
        Destroy(gameObject);
    }
}
