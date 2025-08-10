// ConfirmButton.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmButton : MonoBehaviour
{
    [Tooltip("ゲームシーンの名前")]
    public string gameSceneName = "GameScene";

    /// <summary>
    /// 確認ボタンが押された時に呼び出されるメソッド
    /// </summary>
    public void OnConfirm()
    {
        // GameSessionにステージデータがセットされているか確認
        if (GameSession.selectedStage == null)
        {
            Debug.LogError("ステージが選択されていません。確認ボタンを押す前にステージボタンが押される必要があります。");
            return;
        }

        // セットされているステージデータを使ってゲームシーンに遷移
        Debug.Log($"'{GameSession.selectedStage.name}' でゲームを開始します。");
        SceneManager.LoadScene(gameSceneName);
    }
}