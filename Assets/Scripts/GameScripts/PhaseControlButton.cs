// PhaseControlButton.cs
using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを扱うため
using TMPro; // TextMeshProを扱うため
using System.Reflection; // リフレクション用

public class PhaseControlButton : MonoBehaviour
{
    [Header("UI参照")]
    public Button button;
    public TextMeshProUGUI buttonText;
    public Image buttonIcon; // アイコン画像

    [Header("スプライト")]
    public Sprite runSprite; // 実行(▶)のスプライト
    public Sprite stopSprite; // 中断(■)のスプライト

    [Header("UI Effect")]
    public Component uiEffectComponent; // UI Effectコンポーネント

    void Start()
    {
        // UI Effectコンポーネントが指定されていない場合、自動で取得
        if (uiEffectComponent == null)
        {
            uiEffectComponent = GetComponent("UIEffect") as Component;
        }

        // 開始時にGamePhaseManagerのイベントを購読
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.AddListener(UpdateVisuals);
            // 初期状態を反映
            UpdateVisuals(GamePhaseManager.Instance.CurrentPhase);
        }

        // ボタンのクリックイベントにメソッドを登録
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時に購読解除
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.RemoveListener(UpdateVisuals);
        }
    }

    // ボタンがクリックされた時の処理
    private void OnButtonClick()
    {
        GamePhaseManager.GamePhase current = GamePhaseManager.Instance.CurrentPhase;
        if (current == GamePhaseManager.GamePhase.Editing)
        {
            // 編集段階なら、実行段階へ移行
            GamePhaseManager.Instance.ChangePhase(GamePhaseManager.GamePhase.Running);
        }
        else
        {
            // 実行段階なら、編集段階へ移行
            GamePhaseManager.Instance.ChangePhase(GamePhaseManager.GamePhase.Editing);
        }
    }

    // フェーズに応じて見た目を更新する
    private void UpdateVisuals(GamePhaseManager.GamePhase newPhase)
    {
        if (newPhase == GamePhaseManager.GamePhase.Editing)
        {
            buttonText.text = "実行";
            buttonIcon.sprite = runSprite;
            
            // 編集中は光る効果を停止
            SetEdgeShinySpeed(0f);
        }
        else
        {
            buttonText.text = "中断";
            buttonIcon.sprite = stopSprite;
            
            // 実行中は光る効果を開始
            SetEdgeShinySpeed(0.5f);
        }
    }

    // UI EffectのEdge Shiny速度を設定する
    private void SetEdgeShinySpeed(float speed)
    {
        if (uiEffectComponent != null)
        {
            var type = uiEffectComponent.GetType();
            
            // edgeShinyAutoPlaySpeedプロパティを設定
            var speedProperty = type.GetProperty("edgeShinyAutoPlaySpeed");
            if (speedProperty != null && speedProperty.CanWrite)
            {
                try
                {
                    speedProperty.SetValue(uiEffectComponent, speed);
                    Debug.Log($"Edge Shiny AutoPlay Speed set to: {speed}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to set edgeShinyAutoPlaySpeed: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("edgeShinyAutoPlaySpeed property not found or not writable");
            }
        }
    }
}