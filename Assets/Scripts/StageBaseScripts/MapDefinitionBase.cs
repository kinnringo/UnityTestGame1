// MapDefinitionBase.cs
using UnityEngine;

// すべてのステージマップ定義クラスが、このクラスを継承するようにする
public abstract class MapDefinitionBase : MonoBehaviour
{
    // 抽象プロパティとして、マップデータを必ず持つように強制する
    public abstract int[,] Map { get; }
    public abstract int[,] EditableMap { get; } // 編集可能マップ

}