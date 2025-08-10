// BackToMenuButton.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    [Tooltip("戻る先のステージセレクトシーンの名前")]
    public string menuSceneName = "MenuScene";

    /// <summary>
    /// ボタンがクリックされた時に呼び出されるpublicなメソッド
    /// </summary>
    public void GoBackToMenu()
    {
        Debug.Log($"シーン '{menuSceneName}' に戻ります。");
        SceneManager.LoadScene(menuSceneName);
    }
}