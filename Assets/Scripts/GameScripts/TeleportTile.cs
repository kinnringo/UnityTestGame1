// TeleportTile.cs

using UnityEngine;

public class TeleportTile : MonoBehaviour
{
    private int groupId;      // 自分のグループID
    private bool isDestination; // 自分が出口タイルかどうか

    [Header("ビジュアル参照")]
    [Tooltip("出口タイルに表示するマーカー画像")]
    public GameObject destinationMarkerObject; // ここでSpriteRendererからGameObjectに変更

    [Tooltip("入口タイルに表示するマーカー")]
    public GameObject sourceMarkerObject;

    /// <summary>
    /// MapGeneratorからタイルの情報を初期設定してもらう
    /// </summary>
    public void Initialize(int grpId, bool isDest)
    {
        this.groupId = grpId;
        this.isDestination = isDest;
        
        // 自分が出口タイルなら出口マーカーを、入口タイルなら入口マーカーを表示する
        if (destinationMarkerObject != null)
        {
            destinationMarkerObject.SetActive(isDestination);
        }
        if (sourceMarkerObject != null)
        {
            sourceMarkerObject.SetActive(!isDestination); // isDestinationがfalseの時にtrueになる
        }
    }
    
    /// <summary>
    /// このタイルのグループIDを外部から取得するためのメソッド
    /// </summary>
    public int GetGroupId()
    {
        return this.groupId;
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー以外、または自分が出口タイルなら何もしない
        if (!other.CompareTag("Player") || isDestination)
        {
            return;
        }

        // --- If文のチェック ---
        PlayerMover mover = other.GetComponent<PlayerMover>();
        if (mover != null && mover.ShouldSkipNextTileEffect())
        {
            Debug.Log("If文の効果により、TeleportTileの効果をスキップしました。");
            return;
        }

        // 対応する出口を探す
        TeleportTile destination = MapGenerator.teleportDestinations[groupId];

        // 出口が見つからなかったら警告を出して何もしない
        if (destination == null)
        {
            Debug.LogWarning($"テレポートエラー: グループ {groupId} の出口タイルが見つかりません！");
            return;
        }

        // ワープ処理
        Transform player = other.transform;
        float currentY = player.position.y;
        Vector3 targetPos = destination.transform.position;
        Vector3 newPos = new Vector3(targetPos.x, currentY, targetPos.z);
        player.position = newPos;

        Debug.Log($"テレポート成功: グループ {groupId} の入口から出口へ");

        // テレポート後の移動クールダウンを設定
        if (mover != null)
        {
            mover.SetTeleportCooldown(0.05f);
        }
    }

}