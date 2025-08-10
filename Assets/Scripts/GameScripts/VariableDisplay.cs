// VariableDisplay.cs
using UnityEngine;
using TMPro;

public class VariableDisplay : MonoBehaviour
{
    [Header("テキスト参照")]
    [SerializeField] private TextMeshProUGUI rubyValueText;
    [SerializeField] private TextMeshProUGUI sapphireValueText;
    [SerializeField] private TextMeshProUGUI emeraldValueText;

    private void Start()
    {
        // VariableManagerのインスタンスが存在するか確認
        if (VariableManager.Instance == null)
        {
            Debug.LogError("VariableManagerが見つかりません！");
            // このUIを非表示にしてエラーを防ぐ
            gameObject.SetActive(false);
            return;
        }

        // --- イベントの購読 ---
        // OnVariableChangedイベントに、UpdateDisplayメソッドを登録する
        VariableManager.Instance.OnVariableChanged.AddListener(UpdateDisplay);

        // --- 初期の表示を更新 ---
        // ゲーム開始時の各変数の値で、表示を一度リフレッシュする
        UpdateDisplay(VariableType.Ruby, VariableManager.Instance.GetValue(VariableType.Ruby));
        UpdateDisplay(VariableType.Sapphire, VariableManager.Instance.GetValue(VariableType.Sapphire));
        UpdateDisplay(VariableType.Emerald, VariableManager.Instance.GetValue(VariableType.Emerald));
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される時に、イベントの購読を解除（メモリリーク防止）
        if (VariableManager.Instance != null)
        {
            VariableManager.Instance.OnVariableChanged.RemoveListener(UpdateDisplay);
        }
    }

    /// <summary>
    /// VariableManagerから変数の変更通知を受け取った時に呼ばれるメソッド
    /// </summary>
    /// <param name="type">変更された変数の種類</param>
    /// <param name="newValue">新しい値</param>
    private void UpdateDisplay(VariableType type, int newValue)
    {
        // 変更された変数の種類に応じて、対応するテキストだけを更新する
        switch (type)
        {
            case VariableType.Ruby:
                if (rubyValueText != null) rubyValueText.text = newValue.ToString();
                break;
            case VariableType.Sapphire:
                if (sapphireValueText != null) sapphireValueText.text = newValue.ToString();
                break;
            case VariableType.Emerald:
                if (emeraldValueText != null) emeraldValueText.text = newValue.ToString();
                break;
        }
    }
}