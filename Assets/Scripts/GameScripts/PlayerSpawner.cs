// PlayerSpawner.cs (修正)

using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerParent;

    private GameObject playerInstance; // 生成したプレイヤーのインスタンスを保持

    public void SpawnPlayerAtStartTile(Transform startTile)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab が設定されていません！");
            return;
        }

        Vector3 spawnPos = startTile.position + Vector3.up * 0.5f;
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity, playerParent);

        Debug.Log("プレイヤー生成 -> " + startTile.name);
        
        // プレイヤーを生成した直後に、現在のゲームフェーズをチェックして表示状態を同期させる
        if (GamePhaseManager.Instance != null)
        {
            HandlePhaseChange(GamePhaseManager.Instance.CurrentPhase);
        }
    }
    
    private void Start()
    {
        // Start()の時点では、他のどのAwake()も完了していることが保証されている
        if (GamePhaseManager.Instance != null)
        {
            Debug.Log("PlayerSpawner.Start()でイベントを購読します。");
            GamePhaseManager.Instance.OnPhaseChanged.AddListener(HandlePhaseChange);

            // Start時点での初期状態を反映させておく
            HandlePhaseChange(GamePhaseManager.Instance.CurrentPhase);
        }
        else
        {
            Debug.LogError("PlayerSpawner.Start()の時点でGamePhaseManager.Instanceがnullです。ありえないエラーです。");
        }
    }
    
    // 念のため、OnDestroyで購読解除
    private void OnDestroy()
    {
        // シーン終了時などに購読解除を行う
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.RemoveListener(HandlePhaseChange);
        }
    }

    private void HandlePhaseChange(GamePhaseManager.GamePhase newPhase)
    {
        // 生成されたプレイヤーインスタンスが存在する場合のみ処理
        if (playerInstance != null)
        {
            // 実行段階なら表示、編集段階なら非表示
            bool shouldBeActive = (newPhase == GamePhaseManager.GamePhase.Running);
            playerInstance.SetActive(shouldBeActive);
        }
    }
}