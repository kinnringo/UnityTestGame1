// MapGenerator.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    // 他スクリプトからアクセス可能な静的変数
    public static HashSet<Vector3Int> validTilePositions = new HashSet<Vector3Int>();
    public static TeleportTile[] teleportDestinations = new TeleportTile[10];

    // マテリアルを保持する変数
    private Material outlineMaterialInstance;

    [Header("プレハブ参照")]
    public GameObject tilePrefab;
    public GameObject startTilePrefab;
    public GameObject goalTilePrefab;
    public GameObject arrowTilePrefab;
    public GameObject teleportTilePrefab;
    public GameObject variableTilePrefab;
    public GameObject ifTilePrefab;
    public GameObject decorationTilePrefab;

    [Header("親オブジェクト参照")]
    public Transform tileParent;
    public PlayerSpawner playerSpawner;

    [Header("タイル配置設定")]
    public float tileSize = 1.0f;
    public float tileSpacing = 0.05f;

    void Start()
    {
        StageData stageToLoad = GameSession.selectedStage;

        if (stageToLoad == null)
        {
            Debug.LogError("プレイするステージが選択されていません！ GameSession.SetStage() が呼ばれていません。");
            return;
        }

        // ステージデータにマップ定義プレハブが設定されているか確認
        if (stageToLoad.mapDefinitionPrefab == null)
        {
            Debug.LogError($"ステージ '{stageToLoad.name}' に Map Definition Prefab が設定されていません！");
            return;
        }

        // Resourcesフォルダからマテリアルを読み込む
        outlineMaterialInstance = Resources.Load<Material>("OutlineMaterial");
        if (outlineMaterialInstance == null)
        {
            Debug.LogError("Resourcesフォルダに OutlineMaterial が見つかりません！");
            return;
        }

        // 1. マップ定義プレハブを一時的にシーンに生成する
        MapDefinitionBase mapDefinitionInstance = Instantiate(stageToLoad.mapDefinitionPrefab);

        // 2. 生成したインスタンスから2次元マップ配列と編集マップを取得する
        int[,] map = mapDefinitionInstance.Map;
        int[,] editableMap = mapDefinitionInstance.EditableMap;


        // 3. マップ生成を実行する
        GenerateMap(map, editableMap, stageToLoad);

        // 4. 役割を終えたマップ定義インスタンスは破棄する
        Destroy(mapDefinitionInstance.gameObject);
    }

    void GenerateMap(int[,] map,int[,] editableMap, StageData data)
    {
        validTilePositions.Clear();
        for (int i = 0; i < teleportDestinations.Length; i++) { teleportDestinations[i] = null; }

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileCode = map[y, x];
                if (tileCode == 0) continue;

                Vector3 pos = new Vector3(x * tileSize, 0, -y * tileSize);
                Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(pos.x), 0, Mathf.RoundToInt(pos.z));
                validTilePositions.Add(gridPos);

                // 黒いアウトライン下地を生成
                GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
                border.transform.position = pos;
                border.transform.localScale = new Vector3(tileSize, 0.19f, tileSize);
                // 読み込んだマテリアルをセットする
                Renderer borderRenderer = border.GetComponent<Renderer>(); // Rendererを取得
                if (borderRenderer != null && outlineMaterialInstance != null)
                {
                    borderRenderer.material = outlineMaterialInstance;
                }
                border.transform.parent = tileParent;



                // 適切なプレハブを選択
                GameObject selectedPrefab = tilePrefab;
                if (tileCode == 2) selectedPrefab = startTilePrefab;
                else if (tileCode == 3) selectedPrefab = goalTilePrefab;
                else if (tileCode >= 4 && tileCode <= 7) selectedPrefab = arrowTilePrefab;
                else if ((tileCode >= 10 && tileCode <= 19) || (tileCode >= 90 && tileCode <= 99)) selectedPrefab = teleportTilePrefab;
                else if (tileCode >= 20 && tileCode <= 29) selectedPrefab = variableTilePrefab;
                else if (tileCode >= 30 && tileCode <= 39) selectedPrefab = ifTilePrefab;
                else if (tileCode >= -32 && tileCode <= -1) selectedPrefab = decorationTilePrefab;

                Vector3 offsetPos = pos + new Vector3(0, 0.01f, 0);
                GameObject tile = Instantiate(selectedPrefab, offsetPos, Quaternion.identity, tileParent);
                tile.transform.localScale = new Vector3(0.95f, 0.2f, 0.95f);

                // 生成したタイルに、編集可能情報を設定する
                TileEditor editor = tile.GetComponent<TileEditor>();
                if (editor != null)
                {
                    // 対応する座標の編集タイプを取得
                    int editType = editableMap[y, x];
                    editor.Initialize(editType, borderRenderer); // Rendererを渡す
                }

                // タグ設定
                if (tileCode == 2) tile.tag = "StartTile";
                else if (tileCode == 3) tile.tag = "GoalTile";
                else if (tileCode >= 4 && tileCode <= 7) tile.tag = "ArrowTile";
                else if ((tileCode >= 10 && tileCode <= 19) || (tileCode >= 90 && tileCode <= 99)) tile.tag = "TeleportTile";
                else if (tileCode >= 20 && tileCode <= 29) tile.tag = "VariableTile";
                else if (tileCode >= 30 && tileCode <= 39) tile.tag = "IfTile";
                // 装飾タイルは特定のタグを持たなくても良いが、もし必要なら設定（やるならヒエラルキーからタグを追加するところから）
                // else if (tileCode >= -32 && tileCode <= -1) tile.tag = "DecorationTile";

                // 矢印タイル設定
                if (tileCode >= 4 && tileCode <= 7)
                {
                    // (ArrowTile.cs に Initialize を作るとより良い)
                    // tile.GetComponent<ArrowTile>().direction = (ArrowTile.Direction)(tileCode - 4);

                    ArrowTile arrow = tile.GetComponent<ArrowTile>();
                    if (arrow != null)
                    {
                        // tileCode 4,5,6,7 を Direction 0,1,2,3 に対応させる
                        arrow.SetDirection((ArrowTile.Direction)(tileCode - 4));
                    }

                }

                // テレポートタイル設定
                else if ((tileCode >= 10 && tileCode <= 19) || (tileCode >= 90 && tileCode <= 99))
                {
                    bool isDestination = (tileCode >= 10 && tileCode <= 19);
                    int groupId = isDestination ? (tileCode - 10) : (tileCode - 90);
                    TeleportTile tp = tile.GetComponent<TeleportTile>();
                    if (tp != null)
                    {
                        tp.Initialize(groupId, isDestination);
                        if (isDestination)
                        {
                            if (teleportDestinations[groupId] == null) teleportDestinations[groupId] = tp;
                            else Debug.LogError($"テレポートエラー: グループ {groupId} の出口タイルが複数あります！");
                        }
                    }
                    Renderer rend = tile.GetComponent<Renderer>();
                    if(rend != null)
                    {
                        int colorIndex = isDestination ? (tileCode - 10) : (tileCode - 90);
                        Material mat = new Material(rend.sharedMaterial);
                        mat.color = teleportColors[colorIndex];
                        rend.material = mat;
                    }
                }

                // 変数タイル設定
                else if (tileCode >= 20 && tileCode <= 29)
                {
                    VariableTileSetting setting = data.variableTileSettings.FirstOrDefault(s => s.tileCode == tileCode);
                    if (setting != null)
                    {
                        tile.GetComponent<VariableTile>()?.Initialize(setting.variableType, setting.operation, setting.value);
                    }
                }

                // If文タイル設定
                else if (tileCode >= 30 && tileCode <= 39)
                {
                    IfTileSetting setting = data.ifTileSettings.FirstOrDefault(s => s.tileCode == tileCode);
                    if (setting != null)
                    {
                        tile.GetComponent<IfTile>()?.Initialize(setting.variableToCheck, setting.comparison, setting.valueToCompare);
                    }
                }

                // 装飾タイルの色設定
                if (tileCode >= -32 && tileCode <= -1)
                {
                    Renderer rend = tile.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        // マップコードから色のインデックスを計算 (-1 -> 0, -2 -> 1, ...)
                        int colorIndex = Mathf.Abs(tileCode) - 1;
                        
                        // インデックスが色の配列の範囲内かチェック
                        if (colorIndex >= 0 && colorIndex < decorationColors.Length)
                        {
                            // マテリアルを複製して色を変更
                            Material mat = new Material(rend.sharedMaterial);
                            mat.color = decorationColors[colorIndex];
                            rend.material = mat;
                        }
                    }
                }
            }
        }
        
        // プレイヤーのスポーン処理（スタートタイル上に配置）
        if (playerSpawner != null)
        {
            // "StartTile" タグが付いているオブジェクトを探す
            GameObject startTileObject = GameObject.FindWithTag("StartTile");

            if (startTileObject != null)
            {
                // 見つかったオブジェクトのTransformを渡す
                playerSpawner.SpawnPlayerAtStartTile(startTileObject.transform);
            }
            else
            {
                Debug.LogError("StartTile が見つかりません！ マップにスタートタイルが配置されているか、プレハブにタグが設定されているか確認してください。");
            }
        }
        else
        {
            Debug.LogError("PlayerSpawner が設定されていません！");
        }
    }

    private static readonly Color[] teleportColors = new Color[]
    {
        Color.red, Color.blue, Color.green, new Color(0.678f, 0.847f, 0.902f), Color.magenta,
        Color.cyan, new Color(1f, 0.5f, 0f), new Color(0.5f, 0f, 1f),
        new Color(0.3f, 0.8f, 0.2f), new Color(1f, 0.75f, 0.8f)
    };

    private static readonly Color[] decorationColors = new Color[]
    {
        // ここに16色を定義する (例)
        // 基本色
        new Color(0.8f, 0.2f, 0.2f), // 濃い赤
        new Color(0.2f, 0.8f, 0.2f), // 濃い緑
        new Color(0.2f, 0.2f, 0.8f), // 濃い青
        new Color(0.8f, 0.8f, 0.2f), // 黄色
        // パステルカラー
        new Color(1.0f, 0.7f, 0.7f), // ピンク
        new Color(0.7f, 1.0f, 0.7f), // ミント
        new Color(0.7f, 0.7f, 1.0f), // ラベンダー
        new Color(1.0f, 0.9f, 0.7f), // クリーム
        // ダークカラー
        new Color(0.5f, 0.0f, 0.0f), // ワインレッド
        new Color(0.0f, 0.5f, 0.0f), // フォレストグリーン
        new Color(0.0f, 0.0f, 0.5f), // ネイビー
        new Color(0.5f, 0.3f, 0.0f), // ブラウン
        // その他
        new Color(1.0f, 0.5f, 0.0f), // オレンジ
        new Color(0.5f, 0.0f, 1.0f), // 紫
        new Color(0.0f, 0.8f, 0.8f), // ターコイズ
        new Color(0.5f, 0.5f, 0.5f), // グレー



        //Stage3使用 -17番から
        new Color(0.2f, 0.8f, 1.0f), // 青と水色の中間(水色寄り)
        new Color(0.1f, 0.65f, 0.85f), // 青と水色の中間(青寄り)
        new Color(0.0f, 0.5f, 0.7f), // 青(クジラ用に作成)
        new Color(0.7f, 0.9f, 1.0f), // 水色(クジラの潮用に作成)
        
        //Stage7使用 -21番から
        new Color(0.25f, 0.13f, 0.0f), //濃いこげ茶色
        new Color(0.5f, 0.3f, 0.0f), // こげ茶色
        new Color(0.7f, 0.45f, 0.2f), // 茶色
        new Color(1.0f, 0.9f, 0.85f), // 肌色(帆の色)
        new Color(0.82f, 0.77f, 0.75f), // 灰色(帆の影の色)


        new Color(0.0f, 0.5f, 0.0f), // フォレストグリーン
        new Color(0.0f, 0.0f, 0.5f), // ネイビー
        new Color(0.5f, 0.3f, 0.0f), // ブラウン
        // その他
        new Color(1.0f, 0.5f, 0.0f), // オレンジ
        new Color(0.5f, 0.0f, 1.0f), // 紫
        new Color(0.0f, 0.8f, 0.8f), // ターコイズ
        new Color(0.5f, 0.5f, 0.5f)  // グレー
    };

}