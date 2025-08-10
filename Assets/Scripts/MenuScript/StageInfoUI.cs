using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageInfoUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI learnedAlgoText;
    public Button startButton;

    public StagePanelAnimator panelAnimator;


    private string[] stageNames = {
        "ステージ1：\n\n方向変換タイルの編集",
        "ステージ2：\n\n条件分岐",
        "ステージ3：\n方向タイルと条件分岐",
        "ステージ4：\n関数の仕様説明",
        "ステージ5：\n2重ループと先読み",
        "ステージ6：\n2重ループと先読み",
        "ステージ7：\n絶賛制作中！！",

    };

    private string[] stageAlgorithms = {
        "\n進行方向の制御\n矢印による動作の変化",
        "\nif文\nelse文",
        "\n進行方向の制御\nif文",
        "\nテレポートタイルによる\n疑似的な関数を実装しています",
        "\nfor (int i = 0; i < 3; i++)\nfor (int j = 0; j < 3; j++)",
        "\nfor (int i = 0; i < 3; i++)\nfor (int j = 0; j < 3; j++)\nStage5の解答です",
        "\n制作中につきしばらくお待ちください",
    };

    public void ShowStageInfo(int index)
    {   
        index--; // 配列スタート0番に合わせるためデクリメント

        panelAnimator.ShowPanel();  // ← 追加

        panel.SetActive(true);
        stageNameText.text = stageNames[index];
        learnedAlgoText.text = $"このステージで\n" + $"学べる内容：\n{stageAlgorithms[index]}";

        startButton.onClick.RemoveAllListeners();
        panel.SetActive(true);
        panelAnimator.ShowPanel();  // ←アニメーション付きで表示

    }

    public void HidePanel()
    {
         panelAnimator.HidePanel(); // ← アニメーションで非表示
    }
}



