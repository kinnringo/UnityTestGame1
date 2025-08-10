// StageData.cs

using UnityEngine;

// Unityエディタの「Assets > Create」メニューに項目を追加するための属性
[CreateAssetMenu(fileName = "StageData_New", menuName = "MyGame/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("マップ構造")]
    [Tooltip("このステージのマップ配列を定義したスクリプトを持つプレハブ")]
    public MapDefinitionBase mapDefinitionPrefab;

    [Header("タイルごとの詳細設定")]
    [Tooltip("変数タイルの設定")]
    public VariableTileSetting[] variableTileSettings;
    
    [Tooltip("If文タイルの設定")]
    public IfTileSetting[] ifTileSettings;
}

// ----- データ構造を定義するための補助クラス -----

// Serializable属性を付けると、インスペクターに表示・編集できるようになる
[System.Serializable]
public class VariableTileSetting
{
    public int tileCode; // 対応するマップコード (例: 20)
    public VariableType variableType;
    public OperationType operation;
    public int value;
}

[System.Serializable]
public class IfTileSetting
{
    public int tileCode; // 対応するマップコード (例: 30)
    public VariableType variableToCheck;
    public ComparisonOperator comparison;
    public int valueToCompare;
}