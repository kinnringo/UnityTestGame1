// StageSelectButton.cs
using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    [Tooltip("このボタンが担当するステージのデータ")]
    public StageData stageDataToLoad;

    public void OnClick()
    {
        if (stageDataToLoad == null)
        {
            Debug.LogError("読み込むステージデータが設定されていません！");
            return;
        }

        // 1. GameSessionにプレイする予定のステージデータをセット
        GameSession.SetStage(stageDataToLoad);
        Debug.Log($"ステージ '{stageDataToLoad.name}' が選択されました。");

    }
}