// GameSession.cs
// シーン間で「どのステージを遊ぶか」という情報を一時的に保持するためのクラス
using UnityEngine;

public static class GameSession
{
    // staticな変数として、選択されたステージデータを保持する
    public static StageData selectedStage { get; private set; }

    /// <summary>
    /// ステージセレクトシーンから呼び出し、プレイするステージを設定する
    /// </summary>
    public static void SetStage(StageData stage)
    {
        selectedStage = stage;
    }
}