using UnityEngine;

public class ArrowTile : MonoBehaviour
{
    public enum Direction
    {
        Up,    // 0 Z+
        Right, // 1 X+
        Down,  // 2 Z-
        Left   // 3 X-
    }
    
    public Direction direction;
    private Transform arrowVisual; // ArrowVisualの参照を保持

    void Start()
    {
        // ArrowVisualの参照を取得して保持
        arrowVisual = transform.Find("ArrowVisual");
        
        // エラーログ
        if (arrowVisual == null)
        {
            Debug.LogError($"ArrowVisual not found in {gameObject.name}");
            return;
        }
        
        // 初期の見た目を設定
        UpdateArrowVisual();
    }

    public Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case Direction.Up:    return Vector3.forward;
            case Direction.Right: return Vector3.right;
            case Direction.Down:  return Vector3.back;
            case Direction.Left:  return Vector3.left;
            default:              return Vector3.forward;
        }
    }

    public Direction GetCurrentDirection()
    {
        return direction;
    }

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        
        // 見た目も更新
        UpdateArrowVisual();
    }

    // 見た目を更新するメソッド（Start()とSetDirection()の両方から呼び出す）
    private void UpdateArrowVisual()
    {
        if (arrowVisual == null) return;
        
        float angle = GetRotationAngle(direction);
        arrowVisual.localRotation = Quaternion.Euler(90f, angle, 0f);
    }

    private float GetRotationAngle(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:    return 0f;
            case Direction.Right: return 90f;
            case Direction.Down:  return 180f;
            case Direction.Left:  return 270f;
            default:              return 0f;
        }
    }
}