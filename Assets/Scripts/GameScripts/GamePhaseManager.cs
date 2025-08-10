// GamePhaseManager.cs

using UnityEngine;
using UnityEngine.Events; // UnityEventを使う

public class GamePhaseManager : MonoBehaviour
{
    // シングルトンパターンでどこからでもアクセスできるようにする
    public static GamePhaseManager Instance { get; private set; }

    public enum GamePhase { Editing, Running }

    [SerializeField]
    private GamePhase currentPhase;
    public GamePhase CurrentPhase => currentPhase;

    // フェーズが切り替わったことを他のスクリプトに通知するためのイベント
    public UnityEvent<GamePhase> OnPhaseChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 初期フェーズを確定させる
            currentPhase = GamePhase.Editing; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*
    void Start()
    {
        // ゲームシーンが開始されたら、まず変数をリセットする
        if (VariableManager.Instance != null)
        {
            //VariableManager.Instance.ResetVariables();
        }

        Debug.Log($"ゲーム開始。初期フェーズ: {currentPhase}");
        OnPhaseChanged?.Invoke(currentPhase);
    }

    /// <summary>
    /// ゲームのフェーズを切り替える
    /// </summary>
    public void ChangePhase(GamePhase newPhase)
    {
        if (currentPhase == newPhase) return;

        currentPhase = newPhase;
        Debug.Log($"ゲームフェーズが {newPhase} に切り替わりました。");

        // イベントを発火して、フェーズ変更を全体に通知
        OnPhaseChanged?.Invoke(newPhase);
    }
    */

    void Start()
    {
        // ゲームシーンが開始されたら、まず変数をリセットする
        if (VariableManager.Instance != null)
        {
            VariableManager.Instance.ResetVariables();
        }

        Debug.Log($"ゲーム開始。初期フェーズ: {currentPhase}");
        OnPhaseChanged?.Invoke(currentPhase);
    }

    public void ChangePhase(GamePhase newPhase)
    {
        if (currentPhase == newPhase) return;

        currentPhase = newPhase;
        Debug.Log($"ゲームフェーズが {newPhase} に切り替わりました。");

        // もし新しいフェーズが「編集」なら、変数をリセットする
        // (「中断」ボタンが押された時の処理)
        if (newPhase == GamePhase.Editing)
        {
            if (VariableManager.Instance != null)
            {
                VariableManager.Instance.ResetVariables();
            }
        }

        OnPhaseChanged?.Invoke(newPhase);
    }

}