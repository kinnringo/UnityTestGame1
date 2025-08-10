using UnityEngine;
using TMPro;

public class FloatingBubbleSpawner : MonoBehaviour
{
    // バブルのプレハブ（事前に設定）
    public GameObject bubblePrefab;

    // バブルを出現させるキャンバス（UIの親）
    public RectTransform canvasRect;

    // バブルに表示するキーワード候補
    public string[] keywords = { "if", "else", "==", "!=", "for", "while", "break" };

    // バブルを生成する間隔（秒）
    public float spawnInterval = 1.2f;

    // バブルの浮遊速度（FloatingBubbleに渡す）
    public float floatSpeed = 50f;

    private float timer = 0f;

    void Update()
    {
        // 一定間隔ごとにバブルを生成するタイマー処理
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBubble();
            timer = 0f;
        }
    }

    void SpawnBubble()
    {
        // バブルをCanvas上に生成
        GameObject bubble = Instantiate(bubblePrefab, canvasRect);

        // 出現位置をキャンバスの幅を使ってランダムに決定（中央を基準に）
        float x = Random.Range(0f, canvasRect.rect.width) - canvasRect.rect.width / 2f;

        // 出現位置（Y）：画面の外（下）から
        float y = -canvasRect.rect.height / 2f - 100f;

        // 実際のバブル位置に反映
        RectTransform rect = bubble.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x, y);

        // ランダムなキーワードを表示に設定
        string keyword = keywords[Random.Range(0, keywords.Length)];
        bubble.GetComponentInChildren<TMP_Text>().text = keyword;

        // バブルをUIの一番背面に移動（他のUIの裏側に配置）
        bubble.transform.SetSiblingIndex(0);

        // 浮遊スクリプトをアタッチ
        FloatingBubble fb = bubble.AddComponent<FloatingBubble>();

        // 浮遊速度を渡す（スクリプトに public フィールドを用意する前提）
        fb.floatSpeed = floatSpeed;
    }
}
