// TileEditor.cs (修正)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileEditor : MonoBehaviour
{
    private int editType = 0;
    private bool isEditable = false;
    private Renderer outlineRenderer; // アウトラインのRenderer
    private Coroutine blinkingCoroutine; // 点滅コルーチンを管理

    // --- タイル本体のRendererと色を保存する変数 ---
    private Renderer mainTileRenderer;
    private Color originalMainTileColor;

    // 編集可能タイルの編集中の色を定数として定義
    private static readonly Color EDITABLE_TILE_TINT_COLOR = new Color(1.0f, 1.0f, 0.6f, 1.0f); // タイル本体用の淡い黄色
    // new Color(1.0f, 1.0f, 0.6f, 1.0f); // 薄黄色



    // MapGeneratorからRendererも受け取る
    public void Initialize(int type, Renderer border)
    {
        this.editType = type;
        this.isEditable = (type > 0);
        this.outlineRenderer = border;

        // タイル本体のRendererを取得
        mainTileRenderer = GetComponent<Renderer>();
        if (mainTileRenderer != null)
        {
            // マテリアルを共有しないようにインスタンス化（重要）
            mainTileRenderer.material = new Material(mainTileRenderer.material);
            originalMainTileColor = mainTileRenderer.material.color;
        }

        // 編集フェーズなら色を変更変更する
        if (isEditable)
        {
            // タイル本体の色を変更
            if (mainTileRenderer != null)
            {
                mainTileRenderer.material.color = EDITABLE_TILE_TINT_COLOR;
            }
        }

    }

    // このオブジェクトが有効になった時に呼ばれる
    private void OnEnable()
    {
        // GamePhaseManagerのイベントを購読
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.AddListener(HandlePhaseChange);

            // 購読開始時に現在のフェーズをチェックし、状態を同期させる
            HandlePhaseChange(GamePhaseManager.Instance.CurrentPhase);
        }
    }

    // このオブジェクトが無効になった時に呼ばれる
    private void OnDisable()
    {
        // 購読を解除（メモリリーク防止）
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.RemoveListener(HandlePhaseChange);
        }
    }

    // フェーズが変更された時に呼ばれるメソッド
    private void HandlePhaseChange(GamePhaseManager.GamePhase newPhase)
    {
        if (isEditable)
        {
            // アウトラインの点滅処理
            if (outlineRenderer != null)
            {
                if (newPhase == GamePhaseManager.GamePhase.Editing)
                {
                    if (blinkingCoroutine == null)
                    {
                        blinkingCoroutine = StartCoroutine(BlinkOutline());
                    }
                }
                else
                {
                    if (blinkingCoroutine != null)
                    {
                        StopCoroutine(blinkingCoroutine);
                        blinkingCoroutine = null;
                    }
                    outlineRenderer.material.color = Color.black;
                }
            }

            // タイル本体の色の切り替え処理
            if (mainTileRenderer != null)
            {
                if (newPhase == GamePhaseManager.GamePhase.Editing)
                {
                    // 編集段階なら黄色にする
                    mainTileRenderer.material.color = EDITABLE_TILE_TINT_COLOR;
                }
                else
                {
                    // 実行段階なら元の色に戻す
                    mainTileRenderer.material.color = originalMainTileColor;
                }
            }


        }

    }

    // アウトラインを点滅させるコルーチン
    private IEnumerator BlinkOutline()
    {
        Color originalColor = Color.black;
        Color blinkColor = Color.yellow; // 点滅色
        float minAlpha = 0.1f; // 点滅の明るさ（最小）
        float maxAlpha = 2.5f; // 点滅の明るさ（最大）
        float speed = 1.0f;    // 点滅の速さ

        while (true)
        {
            // Sin波を使ってアルファ値を滑らかに変化させる
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1) / 2f);
            outlineRenderer.material.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);
            yield return null; // 1フレーム待つ
        }
    }

    private void OnMouseDown()
    {
        if (GamePhaseManager.Instance.CurrentPhase != GamePhaseManager.GamePhase.Editing || !isEditable)
        {
            return;
        }

        // どの種類のタイルか判定
        VariableTile varTile = GetComponent<VariableTile>();
        if (varTile != null)
        {
            UIManager.Instance.OpenPanelForVariableTile(varTile, editType);
            return;
        }
        
        IfTile ifTile = GetComponent<IfTile>();
        if (ifTile != null)
        {
            UIManager.Instance.OpenPanelForIfTile(ifTile, editType);
            return;
        }

        ArrowTile arrowTile = GetComponent<ArrowTile>();
        if (arrowTile != null)
        {
            RotateArrow(arrowTile);
            return;
        }


        Debug.Log($"編集可能なタイルがクリックされました！ タイプ: {editType}");
    }

    // 矢印を回転させるメソッド
    private void RotateArrow(ArrowTile arrowTile)
    {
        ArrowTile.Direction currentDir = arrowTile.GetCurrentDirection();
        ArrowTile.Direction nextDir = currentDir;

        // 次の向きを決定するループ（最大4回試行）
        for (int i = 0; i < 4; i++)
        {
            // 時計回りに90度回転
            nextDir = (ArrowTile.Direction)(((int)nextDir + 1) % 4);

            // 編集タイプの制約をチェック
            bool isAllowed = true;
            switch (editType)
            {
                case 2: // 上向き禁止
                    if (nextDir == ArrowTile.Direction.Up) isAllowed = false;
                    break;
                case 3: // 右向き禁止
                    if (nextDir == ArrowTile.Direction.Right) isAllowed = false;
                    break;
                case 4: // 下向き禁止
                    if (nextDir == ArrowTile.Direction.Down) isAllowed = false;
                    break;
                case 5: // 左向き禁止
                    if (nextDir == ArrowTile.Direction.Left) isAllowed = false;
                    break;
            }

            // もし許可された向きなら、ループを抜ける
            if (isAllowed)
            {
                break;
            }
        }

        // 新しい向きを設定
        arrowTile.SetDirection(nextDir);
        Debug.Log($"矢印の向きを {nextDir} に変更しました。");
    }

}