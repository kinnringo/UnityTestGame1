// TileEditPanel.cs (完成版)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // System.Linq を使うために追加

public class TileEditPanel : MonoBehaviour
{
    [Header("UI要素への参照")]
    [SerializeField] private Image variableIcon;
    [SerializeField] private Button variableUpButton, variableDownButton;
    [SerializeField] private TextMeshProUGUI operationText;
    [SerializeField] private Button operationUpButton, operationDownButton;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Button valueUpButton, valueDownButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText; // "変数タイル編集" or "Ifタイル編集"

    [Header("リソース")]
    [Tooltip("変数の種類に対応するスプライト。enumの順番に合わせて設定してください (Ruby, Sapphire, Emerald...)")]
    [SerializeField] private Sprite[] variableSprites;

    // 編集対象のタイル情報を保持
    private VariableTile targetVariableTile;
    private IfTile targetIfTile;

    // enumの値を配列として保持しておくと、インデックス操作が楽になる
    private VariableType[] variableTypes;
    private OperationType[] operationTypes;
    private ComparisonOperator[] comparisonOperators;

    private void Awake()
    {
        // enumの全ての値を配列にキャッシュしておく
        variableTypes = (VariableType[])System.Enum.GetValues(typeof(VariableType));
        operationTypes = (OperationType[])System.Enum.GetValues(typeof(OperationType));
        comparisonOperators = (ComparisonOperator[])System.Enum.GetValues(typeof(ComparisonOperator));

        // 各ボタンにクリックイベントを登録
        variableUpButton.onClick.AddListener(() => ChangeVariable(1));
        variableDownButton.onClick.AddListener(() => ChangeVariable(-1));
        operationUpButton.onClick.AddListener(() => ChangeOperation(1));
        operationDownButton.onClick.AddListener(() => ChangeOperation(-1));
        valueUpButton.onClick.AddListener(() => ChangeValue(1));
        valueDownButton.onClick.AddListener(() => ChangeValue(-1));
        closeButton.onClick.AddListener(ClosePanel);
        
        // 初期状態ではパネルを非表示にしておく
        gameObject.SetActive(false);
    }

    #region パネルの開閉と初期化
    
    /// <summary>
    /// UIManagerから呼ばれ、変数タイル用のパネルを初期化して表示する
    /// </summary>
    public void OpenForVariableTile(VariableTile tile, int editType)
    {
        targetVariableTile = tile;
        targetIfTile = null;
        titleText.text = "変数タイル編集";

        // 編集タイプに応じてボタンの活性状態を設定 (10=All, 11=Var, 12=Op, 13=Val)
        SetButtonsInteractable(editType == 10 || editType == 11,
                               editType == 10 || editType == 12,
                               editType == 10 || editType == 13);
        
        UpdateUI();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIManagerから呼ばれ、If文タイル用のパネルを初期化して表示する
    /// </summary>
    public void OpenForIfTile(IfTile tile, int editType)
    {
        targetVariableTile = null;
        targetIfTile = tile;
        titleText.text = "Ifタイル編集";

        // 編集タイプに応じてボタンの活性状態を設定 (20=All, 21=Var, 22=Comp, 23=Val)
        SetButtonsInteractable(editType == 20 || editType == 21,
                               editType == 20 || editType == 22,
                               editType == 20 || editType == 23);
        
        UpdateUI();
        gameObject.SetActive(true);
    }

    private void SetButtonsInteractable(bool canEditVariable, bool canEditOperation, bool canEditValue)
    {
        variableUpButton.interactable = canEditVariable;
        variableDownButton.interactable = canEditVariable;
        operationUpButton.interactable = canEditOperation;
        operationDownButton.interactable = canEditOperation;
        valueUpButton.interactable = canEditValue;
        valueDownButton.interactable = canEditValue;
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region UIの更新

    /// <summary>
    /// 現在のターゲットタイルの情報に基づいてUI全体を更新する
    /// </summary>
    private void UpdateUI()
    {
        if (targetVariableTile != null)
        {
            variableIcon.sprite = variableSprites[(int)targetVariableTile.variableType];
            operationText.text = GetOperationSymbol(targetVariableTile.operation);
            valueText.text = targetVariableTile.value.ToString();
        }
        else if (targetIfTile != null)
        {
            variableIcon.sprite = variableSprites[(int)targetIfTile.variableToCheck];
            operationText.text = GetComparisonSymbol(targetIfTile.comparison);
            valueText.text = targetIfTile.valueToCompare.ToString();
        }
    }

    #endregion

    #region 値の変更ロジック

    private void ChangeVariable(int direction)
    {
        int currentIndex;
        if (targetVariableTile != null)
        {
            currentIndex = (int)targetVariableTile.variableType;
            int newIndex = (currentIndex + direction + variableTypes.Length) % variableTypes.Length;
            targetVariableTile.SetVariableType(variableTypes[newIndex]);
        }
        else if (targetIfTile != null)
        {
            currentIndex = (int)targetIfTile.variableToCheck;
            int newIndex = (currentIndex + direction + variableTypes.Length) % variableTypes.Length;
            targetIfTile.SetVariableType(variableTypes[newIndex]);
        }
        UpdateUI();
    }

    private void ChangeOperation(int direction)
    {
        if (targetVariableTile != null)
        {
            int currentIndex = System.Array.IndexOf(operationTypes, targetVariableTile.operation);
            int newIndex = (currentIndex + direction + operationTypes.Length) % operationTypes.Length;
            targetVariableTile.SetOperation(operationTypes[newIndex]);
        }
        else if (targetIfTile != null)
        {
            int currentIndex = System.Array.IndexOf(comparisonOperators, targetIfTile.comparison);
            int newIndex = (currentIndex + direction + comparisonOperators.Length) % comparisonOperators.Length;
            targetIfTile.SetOperation(comparisonOperators[newIndex]);
        }
        UpdateUI();
    }

    private void ChangeValue(int direction)
    {
        if (targetVariableTile != null)
        {
            int newValue = targetVariableTile.value + direction;
            targetVariableTile.SetValue(newValue);
        }
        else if (targetIfTile != null)
        {
            int newValue = targetIfTile.valueToCompare + direction;
            targetIfTile.SetValue(newValue);
        }
        UpdateUI();
    }

    #endregion

    #region シンボル変換ヘルパー

    // 演算子のenumを記号に変換する
    private string GetOperationSymbol(OperationType op)
    {
        switch (op)
        {
            case OperationType.Add: return "+";
            case OperationType.Subtract: return "-";
            case OperationType.Multiply: return "×";
            case OperationType.Divide: return "÷";
            case OperationType.Assign: return "=";
            default: return "?";
        }
    }

    // 比較演算子のenumを記号に変換する
    private string GetComparisonSymbol(ComparisonOperator op)
    {
        switch (op)
        {
            case ComparisonOperator.LessThan: return "<";
            case ComparisonOperator.GreaterThan: return ">";
            case ComparisonOperator.EqualTo: return "==";
            case ComparisonOperator.NotEqual: return "!=";
            case ComparisonOperator.LessThanOrEqual: return "≦";
            case ComparisonOperator.GreaterThanOrEqual: return "≧";
            default: return "??";
        }
    }

    #endregion
}