using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StagePanelAnimator : MonoBehaviour
{
    // アニメーション対象のパネル（RectTransform 必須）
    public RectTransform panel;

    // アニメーションの所要時間（秒）
    public float slideDuration = 1.0f;

    // パネルの非表示位置（画面右外）
    public Vector2 hiddenPosition = new Vector2(1000, 0);

    // パネルの表示位置（通常は中央）
    public Vector2 visiblePosition = new Vector2(0, 0);

    // 現在のスライドアニメーション（同時に複数走らせないため）
    private Coroutine currentAnim;

    void Awake()
    {
        // 起動時に完全に非表示の位置に固定する（SetActive されていても右外）
        if (panel != null)
        {
            panel.anchoredPosition = hiddenPosition;
            panel.gameObject.SetActive(false);  // 初期状態では非表示にしておく
        }
        else
        {
            Debug.LogError("StagePanelAnimator: panel が Inspector で設定されていません！");
        }
    }

    // パネルを表示する（右外からスライドで登場）
    public void ShowPanel()
    {
        if (panel == null) return;

        // 一旦非表示の位置に固定
        panel.anchoredPosition = hiddenPosition;

        // 描画フレームのタイミングでチラ見えしないようレイアウト更新
        Canvas.ForceUpdateCanvases();

        // SetActive は位置をリセットしたあとに呼ぶ
        panel.gameObject.SetActive(true);

        // すでにアニメーションが動いていれば停止
        if (currentAnim != null) StopCoroutine(currentAnim);

        // アニメーション開始（visiblePosition に向けてスライド）
        currentAnim = StartCoroutine(SlidePanel(visiblePosition));
    }

    // パネルを隠す（スライドして右外へ移動）
    public void HidePanel()
    {
        if (panel == null) return;

        if (currentAnim != null) StopCoroutine(currentAnim);

        // アニメーション開始（hiddenPosition に向けてスライド）
        currentAnim = StartCoroutine(SlidePanel(hiddenPosition));
    }

    // スライドアニメーション本体（表示 or 非表示）
    IEnumerator SlidePanel(Vector2 targetPos)
    {
        Vector2 startPos = panel.anchoredPosition;
        float time = 0f;

        // アニメーションループ：現在位置から targetPos へ徐々に移動
        while (time < slideDuration)
        {
            float t = time / slideDuration;
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        // 最終位置をしっかり固定（Lerp の端数誤差を防止）
        panel.anchoredPosition = targetPos;

        // 非表示アニメーション終了後は完全に消す（描画されないように）
        if (targetPos == hiddenPosition)
        {
            yield return new WaitForEndOfFrame(); // ← これが重要！最後の1フレームをちゃんと表示
            panel.gameObject.SetActive(false);
        }
    }
}
