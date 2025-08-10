// VariableManager.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OperationType
{
    Add,        // +
    Subtract,   // -
    Multiply,   // /
    Divide,     // *
    Assign      // =
}

public enum VariableType
{
    Ruby, Sapphire, Emerald
}

public class VariableManager : MonoBehaviour
{
    public static VariableManager Instance { get; private set; }
    private Dictionary<VariableType, int> variables = new Dictionary<VariableType, int>();
    public UnityEvent<VariableType, int> OnVariableChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVariables();
        }
        else
        {
            Destroy(gameObject);
        }

        if (OnVariableChanged == null)
        {
            OnVariableChanged = new UnityEvent<VariableType, int>();
        }
    }
    

    /*
    private void Start()
    {
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.AddListener(HandlePhaseChange);
        }
    }

    // OnDestroyで購読解除
    private void OnDestroy()
    {
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.OnPhaseChanged.RemoveListener(HandlePhaseChange);
        }
    }
    
    // フェーズ変更をハンドルするメソッド
    private void HandlePhaseChange(GamePhaseManager.GamePhase newPhase)
    {
        // 編集段階に入った時にリセット
        if (newPhase == GamePhaseManager.GamePhase.Editing)
        {
            ResetVariables();
        }
    }
    */


    /// <summary>
    /// すべての変数を初期値（0）にリセットします。
    /// </summary>
    public void ResetVariables()
    {
        Debug.Log("すべての変数をリセットします。");
        // InitializeVariablesメソッドを再利用してリセット処理を行う
        InitializeVariables();

        // UIにもリセットを通知する
        // 登録されている全ての変数タイプに対して、新しい値(0)を通知
        foreach (VariableType type in System.Enum.GetValues(typeof(VariableType)))
        {
            OnVariableChanged?.Invoke(type, GetValue(type));
        }
    }

    private void InitializeVariables()
    {
        foreach (VariableType type in System.Enum.GetValues(typeof(VariableType)))
        {
            variables[type] = 0;
        }
    }

    public int GetValue(VariableType type)
    {
        return variables.ContainsKey(type) ? variables[type] : 0;
    }

    // 変数の値を変更する (加算)
    public void AddValue(VariableType type, int amount)
    {
        if (variables.ContainsKey(type))
        {
            variables[type] += amount;
            Debug.Log($"{type} の値が {amount} 変化しました。現在値: {variables[type]}");
            OnVariableChanged?.Invoke(type, variables[type]);
        }
    }

    /// <summary>
    /// 指定された演算方法で変数の値を操作する
    /// </summary>
    public void OperateValue(VariableType type, OperationType op, int value)
    {
        if (!variables.ContainsKey(type)) return;

        int currentValue = variables[type];
        int newValue = currentValue;

        switch (op)
        {
            case OperationType.Add:
                newValue = currentValue + value;
                break;
            case OperationType.Subtract:
                newValue = currentValue - value;
                break;
            case OperationType.Multiply:
                newValue = currentValue * value;
                break;
            case OperationType.Divide:
                if (value != 0)
                {
                    newValue = currentValue / value;
                }
                else
                {
                    Debug.LogWarning("0で除算しようとしました。処理をスキップします。");
                }
                break;
            case OperationType.Assign:
                newValue = value; // 現在の値を無視して、指定された値で上書き
                break;
        }

        variables[type] = newValue;
        Debug.Log($"{type} を {op} {value} しました。結果: {variables[type]}");
        OnVariableChanged?.Invoke(type, variables[type]);
    }
}