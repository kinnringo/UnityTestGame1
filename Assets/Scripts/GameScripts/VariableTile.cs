// VariableTile.cs

using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using System.Collections; // コルーチンを使うために必要

public class VariableTile : MonoBehaviour
{
    [Header("タイル設定")]
    public VariableType variableType;
    public OperationType operation;
    public int value;

    [Header("コンポーネント参照")]
    [Tooltip("タイルの表面に表示するテキスト")]
    public TextMeshPro textDisplay;

    [Header("宝石モデル設定")]
    [Tooltip("宝石モデルを配置する場所の目印")]
    public Transform gemAnchor;
    
    [Tooltip("ルビーのプレハブ")]
    public GameObject rubyPrefab;
    [Tooltip("サファイアのプレハブ")]
    public GameObject sapphirePrefab;
    [Tooltip("エメラルドのプレハブ")]
    public GameObject emeraldPrefab;

    // 生成した宝石を管理するための変数
    private GemAnimator currentGem;
    private Coroutine gemRespawnCoroutine; // 再表示コルーチンを管理する変数



    void Awake()
    {
        // ゲーム開始時にタイルの見た目を更新する
        UpdateVisuals();
    }

    /// <summary>
    /// MapGeneratorからタイルの詳細設定を受け取るためのメソッド
    /// </summary>
    public void Initialize(VariableType type, OperationType op, int val)
    {
        this.variableType = type;
        this.operation = op;
        this.value = val;
        UpdateVisuals(); // 設定が適用されたら、再度見た目を更新

        SpawnGemModel(); // ★宝石を生成するメソッドを呼び出す

    }

    /// <summary>
    /// このタイルの設定に基づいて、テキスト表示と色を更新する
    /// </summary>
    private void UpdateVisuals()
    {
        if (textDisplay == null)
        {
            Debug.LogWarning("VariableTileにtextDisplayが設定されていません。", this.gameObject);
            return;
        }

        string operatorSymbol = "";
        Color textColor = Color.white; // デフォルト色

        switch (operation)
        {
            case OperationType.Add:
                operatorSymbol = "+";
                textColor = new Color(0.5f, 0.8f, 1f); // 水色っぽい青
                break;
            case OperationType.Subtract:
                operatorSymbol = "-";
                textColor = new Color(1f, 0.5f, 0.5f); // 薄い赤
                break;
            case OperationType.Multiply:
                operatorSymbol = "×"; // "x"や"*"より見やすい
                textColor = new Color(0.5f, 0.8f, 1f); // 水色っぽい青
                break;
            case OperationType.Divide:
                operatorSymbol = "÷";
                textColor = new Color(1f, 0.5f, 0.5f); // 薄い赤
                break;
            case OperationType.Assign:
                operatorSymbol = "=";
                textColor = new Color(0.6f,0.6f,0.6f); // 灰色
                break;

        }

        textDisplay.text = $"{operatorSymbol}{value}";
        textDisplay.color = textColor;
    }

    /// <summary>
    /// variableTypeに応じた宝石モデルを生成する
    /// </summary>
    private void SpawnGemModel()
    {
        if (gemAnchor == null) return;

        // もし既に宝石が存在するなら、それを破棄する
        if (currentGem != null)
        {
            // gemAnchorの子オブジェクトになっているはずなので、そのGameObjectを破棄する
            Destroy(currentGem.gameObject);
            currentGem = null; // 参照をクリアしておく
        }

        // 対応するプレハブを選択
        GameObject prefabToSpawn = null;
        switch (variableType)
        {
            case VariableType.Ruby:
                prefabToSpawn = rubyPrefab;
                break;
            case VariableType.Sapphire:
                prefabToSpawn = sapphirePrefab;
                break;
            case VariableType.Emerald:
                prefabToSpawn = emeraldPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            // gemAnchorの位置に宝石を生成し、子オブジェクトにする
            GameObject gemObject = Instantiate(prefabToSpawn, gemAnchor.position, Quaternion.identity, gemAnchor);
            currentGem = gemObject.GetComponent<GemAnimator>();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerMoverを取得
            PlayerMover mover = other.GetComponent<PlayerMover>();
            
            // debug スピード上げてみる用
            // mover.ChangeSpeed(1);

            // ----If文のチェック 
            if (mover != null && mover.ShouldSkipNextTileEffect())
            {
                Debug.Log("If文の効果により、VariableTileの効果をスキップしました。");
                return;
            }
            
            VariableManager.Instance.OperateValue(variableType, operation, value);
            // ------

            // 宝石の消滅処理
            if (currentGem != null)
            {
                // 既に再表示のコルーチンが動いていたら停止する
                if (gemRespawnCoroutine != null)
                {
                    StopCoroutine(gemRespawnCoroutine);
                }

                // 宝石を非表示にする
                currentGem.SetVisible(false);

                // プレイヤーの移動速度に基づいて、再表示までの時間を計算
                float respawnDelay = (mover != null) ? mover.TimeToCrossOneTile : 1.0f; // moverがなければ1秒固定

                // 計算した時間後に再表示するコルーチンを開始
                gemRespawnCoroutine = StartCoroutine(RespawnGemAfterDelay(respawnDelay));
            }
        }
    }

    /// <summary>
    /// 指定された秒数後に宝石を再表示するコルーチン
    /// </summary>
    /// <param name="delay">遅延させる秒数</param>
    private IEnumerator RespawnGemAfterDelay(float delay)
    {
        // 指定された時間だけ待機する
        yield return new WaitForSeconds(delay);

        // 時間が来たら宝石を再表示する
        if (currentGem != null)
        {
            currentGem.SetVisible(true);
        }

        // コルーチンの管理変数をリセット
        gemRespawnCoroutine = null;
    }
    

    // 以下if文タイルと一部共通の編集フェーズ用メソッド群
    public void SetVariableType(VariableType newType)
    {
        variableType = newType;
        UpdateVisuals(); // 見た目を更新
        SpawnGemModel(); // 宝石モデルも更新
    }

    public void SetOperation(OperationType newOp)
    {
        operation = newOp;
        UpdateVisuals();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateVisuals();
    }

}