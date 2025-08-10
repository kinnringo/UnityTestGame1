using UnityEngine;
using System.Collections;

/// <summary>
/// プレイヤーの自動移動・落下・ゴール停止・テレポート対応を制御
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("現在の移動スピード")]
    [SerializeField] // privateな変数をインスペクターに表示させるための属性
    private float moveSpeed = 2.0f; // 移動スピード（初期値はヒエラルキーの値に依存する）
    
    public Vector3 moveDirection = Vector3.forward; // 初期移動方向（後天的に移動方向を代入しないとここの初期値はヒエラルキーの値に上書きされて移動停止してしまう。）
    private bool skipNextTileEffect = false; // If文用のフラグ
    private Rigidbody rb;
    private bool isMoving = false; // 初期値は必ず false

    private Vector3 initialPosition; // 初期位置を保存する変数
    private Quaternion initialRotation; // 初期向きを保存する変数

    // テレポート後のクールダウン（移動無効化時間）
    private float teleportCooldown = 0f;


    /// <summary>
    /// プレイヤーの移動速度を指定した量だけ変更します。
    /// </summary>
    /// <param name="amount">変化させる量（正の値で加速、負の値で減速）</param>
    public void ChangeSpeed(float amount)
    {
        moveSpeed += amount;
        // 速度がマイナスにならないように制御
        if (moveSpeed < 0)
        {
            moveSpeed = 0;
        }
        // 速度が制限を超過しないように制御
        if (moveSpeed > 10)
        {
            moveSpeed = 10;
        }
        Debug.Log($"速度が {amount} 変化しました。現在の速度: {moveSpeed}");
    }

    /// <summary>
    /// プレイヤーの移動速度を特定の値に直接設定します。
    /// </summary>
    /// <param name="newSpeed">新しい速度</param>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = (newSpeed >= 0) ? newSpeed : 0; // 負の値は0にする
        
        // 速度が制限を超過しないように制御
        if (moveSpeed > 10)
        {
            moveSpeed = 10;
        }

        Debug.Log($"速度が {newSpeed} に設定されました。");
    }

    /// <summary>
    /// 現在の移動速度を取得します。
    /// </summary>
    /// <returns>現在の移動速度</returns>
    public float GetSpeed()
    {
        return moveSpeed;
    }

    /// <summary>
    /// キャラクターがタイル1マス分を移動するのにかかる時間
    /// </summary>
    public float TimeToCrossOneTile
    {
        get
        {
            // tileSize は MapGenerator から取得するのが理想だが、
            // 簡単のため、一般的なタイルサイズ 1.0f を使う。
            // 速度が0だとゼロ除算になるため、最小値を設けて安全性を確保。
            if (moveSpeed <= 0) return float.MaxValue; // 停止している場合は無限大の時間を返す
            float tileSize = 1.0f; // MapGeneratorのtileSizeと合わせる
            return tileSize / moveSpeed;
        }
    }


    /// <summary>
    /// If文タイルから呼び出され、次のタイル効果をスキップするよう設定する
    /// </summary>
    public void SetSkipNextTileEffectFlag()
    {
        skipNextTileEffect = true;
    }

    /// <summary>
    /// 各タイルが効果を発動する前に、このメソッドを呼んでスキップすべきか確認する
    /// 確認後、フラグは自動的にリセットされる
    /// </summary>
    public bool ShouldSkipNextTileEffect()
    {
        if (skipNextTileEffect)
        {
            skipNextTileEffect = false; // フラグは一度使ったらリセット
            return true;
        }
        return false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        // 生成された瞬間の位置と向きを保存しておく
        initialPosition = transform.position;
        initialRotation = transform.rotation;

    }
    
    void Start()
    {
        if (GamePhaseManager.Instance != null)
        {
            Debug.Log("PlayerMover.Start()でイベントを購読します。");
            GamePhaseManager.Instance.OnPhaseChanged.AddListener(HandlePhaseChange);
            
            // Start時点で状態を同期する
            HandlePhaseChange(GamePhaseManager.Instance.CurrentPhase);

        }
        else
        {
            Debug.LogError("PlayerMover.Start()の時点でGamePhaseManager.Instanceがnullです。");
        }
    }
    

    // 念のため、OnDestroyで購読解除
    private void OnDestroy()
    {
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.RemoveListener(HandlePhaseChange);
        }
    }


    /// <summary>
    /// ゲームフェーズの変更に応じて自身の状態を切り替える
    /// </summary>
    private void HandlePhaseChange(GamePhaseManager.GamePhase newPhase)
    {
        Debug.Log($"=== HandlePhaseChange呼び出し: {newPhase} ===");
        Debug.Log($"現在位置: {transform.position}");
        Debug.Log($"初期位置: {initialPosition}");
        Debug.Log($"rb.position: {(rb != null ? rb.position.ToString() : "rb is null")}");
        
        if (newPhase == GamePhaseManager.GamePhase.Running)
        {
            isMoving = true;

            if(rb != null)
            {
                moveDirection = Vector3.forward; // 後天的に移動方向を入力しないと初期値はヒエラルキーの値に上書きされて止まってしまう。
            }

            Debug.Log("プレイヤー: 実行段階開始。移動を開始します。");
        }
        else // GamePhase.Editing
        {
            isMoving = false;
            
            if(rb != null)
            {
                Debug.Log($"リセット実行前 - transform: {transform.position}, rb: {rb.position}");

                // 初期位置に戻す                
                transform.position = initialPosition;
                transform.rotation = initialRotation;
                
                // velocityもクリア
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                moveDirection = Vector3.forward;
                
                // 重力を無効にし、全ての動きと回転を止める
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                // 再び回転とY軸移動を凍結する
                rb.constraints = RigidbodyConstraints.FreezeRotationX |
                                RigidbodyConstraints.FreezeRotationY |
                                RigidbodyConstraints.FreezeRotationZ;

                Debug.Log($"リセット実行後 - transform: {transform.position}, rb: {rb.position}");
            }
            
            Debug.Log("プレイヤー: 編集段階開始。移動を停止し、位置をリセットします。");
        }
        
        Debug.Log($"=== HandlePhaseChange終了 ===");
    }
    


    /* 英霊（PlayerMoverはバグが多いので後の参考用としてしばらく残す）
    private void HandlePhaseChange(GamePhaseManager.GamePhase newPhase)
    {
        if (newPhase == GamePhaseManager.GamePhase.Running)
        {
            // 【実行段階になった時の処理】
            // Rigidbodyの位置と向きを直接リセットする
            if(rb != null)
            {
                rb.position = initialPosition;
                rb.rotation = initialRotation;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                moveDirection = Vector3.forward; // 移動方向も必ず初期状態に戻す
                rb.WakeUp(); // 念のため
            }
            else // rbがnullならtransformを操作する（フォールバック）
            {
                // ログで確認したところ、ここが実行されることはなさそう
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }


            // 2. 移動を開始する
            isMoving = true;
            Debug.Log("プレイヤー: 実行段階開始。移動を開始します。");
        }
        else // if (newPhase == GamePhaseManager.GamePhase.Editing)
        {
            // 【編集段階になった時の処理】
            // 1. 移動を停止する
            isMoving = false;
            moveDirection = Vector3.forward; // 念のため方向をリセットしておく
            Debug.Log("プレイヤー: 編集段階開始。移動を停止します。");
        }
    }
    */
    

    void FixedUpdate()
    {
        // テレポート直後は移動停止（ズレ防止）
        if (teleportCooldown > 0f)
        {
            teleportCooldown -= Time.fixedDeltaTime;
            return;
        }

        if (!isMoving){
            //Debug.Log("動いてないね");
            return;
        }

        if (!IsTileBelow())
        {
            Debug.Log("落下！落下！");
            HandleFall(); // 足場なし：落下処理
            return;
        }
        

        // 通常移動処理
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = targetVelocity;

        // 旧式（こちらの方がいいこともあるので必要に応じて切り替える） 
        // rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// 足元にタイルが存在するかを整数座標ベースで判定
    /// </summary>
    bool IsTileBelow()
    {
        Vector3 pos = transform.position;
        Vector3Int gridPos = new Vector3Int(
            Mathf.RoundToInt(pos.x),
            0,
            Mathf.RoundToInt(pos.z)
        );

        bool isTileValid = MapGenerator.validTilePositions.Contains(gridPos);

        // デバッグ用のログ
        if (!isTileValid && isMoving) // isMovingがtrueの時だけログを出す
        {
            Debug.Log($"落下判定: 足元にタイルがありません！ プレイヤー座標: {pos}, グリッド座標: {gridPos}");

            // validTilePositionsの中身も見てみる（多すぎると大変なので最初の5件だけ）
            string validPositionsLog = "有効なタイル座標 (最初の5件): ";
            int count = 0;
            foreach (var validPos in MapGenerator.validTilePositions)
            {
                validPositionsLog += validPos.ToString() + ", ";
                count++;
                if (count >= 5) break;
            }
            Debug.Log(validPositionsLog);
        }

        return isTileValid;

    }

    /// <summary>
    /// 崖に落ちた時の落下処理
    /// </summary>
    void HandleFall()
    {
        Debug.Log("落下検知 → 停止 & 物理落下開始");
        isMoving = false;

        if (rb != null)
        {
        // 1. 現在の水平速度を保存（慣性として使用）
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // 2. 重力を有効にする
        rb.useGravity = true;

        // 3. 回転を自由にする（完全自由落下）
        rb.constraints = RigidbodyConstraints.None;

        // 4. 慣性を適用（水平方向の速度を維持）
        rb.linearVelocity = new Vector3(currentHorizontalVelocity.x, rb.linearVelocity.y, currentHorizontalVelocity.z);

        // 5. 回転を追加する（複数のパターンから選択）
        AddFallRotation();

        Debug.Log($"落下開始時の速度: {rb.linearVelocity}, 角速度: {rb.angularVelocity}");
        }
    }

    /// <summary>
    /// 落下時の回転演出を追加
    /// </summary>
    void AddFallRotation()
    {
        // パターン1: ランダムな回転（推奨）
        // X, Y, Z軸それぞれにランダムな回転速度を設定
        /*
        float rotationStrength = 5f; // 回転の強さ（調整可能）
        Vector3 randomRotation = new Vector3(
            Random.Range(-rotationStrength, rotationStrength),
            Random.Range(-rotationStrength, rotationStrength),
            Random.Range(-rotationStrength, rotationStrength)
        );
        rb.angularVelocity = randomRotation;
        */

        /* パターン2: 移動方向に応じた回転
        // 移動方向に基づいて「転がる」ような回転
        Vector3 fallRotation = Vector3.Cross(moveDirection, Vector3.down) * 3f;
        rb.angularVelocity = fallRotation;
        */
        
        /* パターン3: 特定軸での回転
        // Z軸を中心とした「くるくる回転」
        rb.angularVelocity = new Vector3(0, 0, 10f);
        */
        
        // パターン4: 移動速度に応じた回転
        // 移動速度が速いほど激しく回転
        float speedFactor = rb.linearVelocity.magnitude;
        Vector3 speedBasedRotation = new Vector3(
            Random.Range(-speedFactor, speedFactor),
            Random.Range(-speedFactor, speedFactor),
            Random.Range(-speedFactor, speedFactor)
        );
        rb.angularVelocity = speedBasedRotation;
        
    }



    /// <summary>
    /// ゴールや矢印、テレポートタイルなどのタイル接触処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving) return;

        // ゴールタイル処理 (修正)
        if (other.CompareTag("GoalTile"))
        {
            if (ShouldSkipNextTileEffect())
            {
                Debug.Log("If文の効果により、GoalTileの効果をスキップしました。");
                return;
            }
            Debug.Log("ゴール到達 → 停止");
            isMoving = false;
            
            // Rigidbodyの速度を明示的に停止
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // UIManagerにゲームクリアUIの表示を依頼する
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameClearPanel();
            }

        }

        // 矢印タイル処理 (修正)
        if (other.CompareTag("ArrowTile"))
        {
            if (ShouldSkipNextTileEffect())
            {
                Debug.Log("If文の効果により、ArrowTileの効果をスキップしました。");
                return;
            }
            
            ArrowTile arrow = other.GetComponent<ArrowTile>();
            if (arrow != null)
            {
                SnapPositionToGridXZ(); // debug
                moveDirection = arrow.GetDirectionVector();
                Debug.Log("矢印タイル → 方向変更: " + moveDirection);
            }
        }
        // ★★★ 他のタイル（VariableTile, TeleportTileなど）も同様の修正が必要 ★★★
        // 各タイルのOnTriggerEnterの先頭で ShouldSkipNextTileEffect() を呼び出し、
        // trueが返ってきたら return するようにしてください。

    }
    // テレポート処理後に呼び出すことで、移動を一時停止
    public void SetTeleportCooldown(float duration)
    {
        teleportCooldown = duration;
    }
    
    // 現在位置を、XZ軸だけ最も近い整数座標に補正（Yはそのまま）
    void SnapPositionToGridXZ()
    {
        Vector3 pos = transform.position;
        float snappedX = Mathf.Round(pos.x);
        float snappedZ = Mathf.Round(pos.z);
        transform.position = new Vector3(snappedX, pos.y, snappedZ);

    }

}
