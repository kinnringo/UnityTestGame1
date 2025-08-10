// IfTile.cs (全面的に書き換えます)

using UnityEngine;
using TMPro; // TextMeshProを扱うために必要

// 比較演算子の種類を拡張
public enum ComparisonOperator
{
    LessThan,           // <
    GreaterThan,        // >
    EqualTo,            // ==
    NotEqual,           // !=
    LessThanOrEqual,    // ≦
    GreaterThanOrEqual  // ≧
}

public class IfTile : MonoBehaviour
{
    [Header("条件設定（スクリプトから自動設定）")]
    public VariableType variableToCheck;
    public ComparisonOperator comparison;
    public int valueToCompare;

    [Header("ビジュアル部品の参照")]
    [Tooltip("変数のアイコンを表示するSpriteRenderer")]
    public SpriteRenderer iconDisplay;
    [Tooltip("比較演算子を表示するTextMeshPro")]
    public TextMeshPro operatorText;
    [Tooltip("比較対象の数値を表示するTextMeshPro")]
    public TextMeshPro valueText;

    [Header("アイコンリソース")]
    public Sprite rubyIcon;
    public Sprite sapphireIcon;
    public Sprite emeraldIcon;

    /// <summary>
    /// MapGeneratorからタイルの詳細設定を受け取るためのメソッド
    /// </summary>
    public void Initialize(VariableType type, ComparisonOperator op, int value)
    {
        this.variableToCheck = type;
        this.comparison = op;
        this.valueToCompare = value;
        UpdateVisuals(); // 設定が適用されたら、見た目を更新
    }

    /// <summary>
    /// このタイルの設定に基づいて、アイコンとテキストを更新する
    /// </summary>
    private void UpdateVisuals()
    {
        // 1. アイコンを設定
        if (iconDisplay != null)
        {
            switch (variableToCheck)
            {
                case VariableType.Ruby:    iconDisplay.sprite = rubyIcon;    break;
                case VariableType.Sapphire:  iconDisplay.sprite = sapphireIcon;  break;
                case VariableType.Emerald:   iconDisplay.sprite = emeraldIcon;   break;
                default: iconDisplay.sprite = null; break;
            }
        }

        // 2. 比較演算子のテキストを設定
        if (operatorText != null)
        {
            string opStr = "";
            switch (comparison)
            {
                case ComparisonOperator.LessThan:           opStr = "<";  break;
                case ComparisonOperator.GreaterThan:        opStr = ">";  break;
                case ComparisonOperator.EqualTo:            opStr = "=="; break;
                case ComparisonOperator.NotEqual:           opStr = "!="; break;
                case ComparisonOperator.LessThanOrEqual:    opStr = "≦"; break;
                case ComparisonOperator.GreaterThanOrEqual: opStr = "≧"; break;
            }
            operatorText.text = opStr;
        }

        // 3. 比較対象の数値を設定
        if (valueText != null)
        {
            valueText.text = valueToCompare.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMover mover = other.GetComponent<PlayerMover>();
            if (mover == null) return;
            
            if (mover.ShouldSkipNextTileEffect())
            {
                Debug.Log("前のIf文の効果により、このIfTileの効果をスキップしました。");
                return;
            }

            bool conditionMet = CheckCondition();
            Debug.Log($"If文タイル: {variableToCheck} {comparison} {valueToCompare} ? -> 結果: {conditionMet}");

            if (!conditionMet)
            {
                mover.SetSkipNextTileEffectFlag();
                Debug.Log("条件を満たさなかったので、次のタイルの効果をスキップします。");
            }
        }
    }

    private bool CheckCondition()
    {
        int currentValue = VariableManager.Instance.GetValue(variableToCheck);
        switch (comparison)
        {
            case ComparisonOperator.LessThan:           return currentValue < valueToCompare;
            case ComparisonOperator.GreaterThan:        return currentValue > valueToCompare;
            case ComparisonOperator.EqualTo:            return currentValue == valueToCompare;
            case ComparisonOperator.NotEqual:           return currentValue != valueToCompare;
            case ComparisonOperator.LessThanOrEqual:    return currentValue <= valueToCompare;
            case ComparisonOperator.GreaterThanOrEqual: return currentValue >= valueToCompare;
            default:                                    return false;
        }
    }


    
    // 以下変数タイルと一部共通の編集フェーズ用メソッド群
    public void SetVariableType(VariableType newType)
    {
        variableToCheck = newType;
        UpdateVisuals();
    }

    public void SetOperation(ComparisonOperator newOp)
    {
        comparison = newOp;
        UpdateVisuals();
    }

    public void SetValue(int newValue)
    {
        valueToCompare = newValue;
        UpdateVisuals();
    }


    /*
    public void SetVariableType(VariableType newType)
    {
        variableType = newType;
        UpdateVisuals(); // 見た目を更新
        // SpawnGemModel(); // 宝石モデルも更新
    }

    public void SetOperation(OperationType newOp)
    {
        operation = newOp;
        UpdateVisuals();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateVisuals();
    }
    */

}