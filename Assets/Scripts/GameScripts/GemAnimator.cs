// GemAnimator.cs

using UnityEngine;

public class GemAnimator : MonoBehaviour
{
    [Header("浮遊アニメーション設定")]
    [SerializeField] private float rotationSpeed = 50f;   // 回転速度 (度/秒)
    [SerializeField] private float bobbingSpeed = 2f;     // 上下動の速さ
    [SerializeField] private float bobbingHeight = 0.1f;  // 上下動の幅

    private Vector3 startPosition;
    private GameObject modelObject; // 実際に表示/非表示を切り替えるモデル部分

    void Start()
    {
        // 自身のローカルポジションを基準点とする
        startPosition = transform.localPosition;

        // 子オブジェクトにモデルがあると想定
        if (transform.childCount > 0)
        {
            modelObject = transform.GetChild(0).gameObject;
        }
        else
        {
            // 子がいなければ自身をモデルとする
            modelObject = this.gameObject;
        }
    }

    void Update()
    {
        // Y軸周りに回転
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 上下にふわふわ動かす (Sin波を利用)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }

    /// <summary>
    /// 宝石のモデルを表示/非表示にする
    /// </summary>
    /// <param name="isVisible">trueなら表示, falseなら非表示</param>
    public void SetVisible(bool isVisible)
    {
        if (modelObject != null)
        {
            modelObject.SetActive(isVisible);
        }
    }
}