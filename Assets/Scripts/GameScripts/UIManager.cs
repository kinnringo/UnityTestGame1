// UIManager.cs
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public TileEditPanel tileEditPanel;

    [Header("ゲームフローUI")]
    public GameObject gameClearPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // ゲーム開始時は非表示にしておく
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(false);
        }

    }

    public void OpenPanelForVariableTile(VariableTile tile, int editType)
    {
        tileEditPanel.OpenForVariableTile(tile, editType);
    }
    
    public void OpenPanelForIfTile(IfTile tile, int editType)
    {
        tileEditPanel.OpenForIfTile(tile, editType);
    }

    /// <summary>
    /// ゲームクリアパネルを表示する
    /// </summary>
    public void ShowGameClearPanel()
    {
        if (gameClearPanel != null)
        {
            Debug.Log("ゲームクリア！UIを表示します。");
            gameClearPanel.SetActive(true);
        }
    }

}