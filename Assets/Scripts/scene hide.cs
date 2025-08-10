using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneHide : MonoBehaviour
{
    [Header("ボタン設定")]
    [SerializeField] private Button hideButton;
    [SerializeField] private Button showButton;
    
    [Header("隠すオブジェクト")]
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();
    
    [Header("設定")]
    [SerializeField] private bool hideAllRootObjects = false; // シーン内のすべてのルートオブジェクトを隠すかどうか
    [SerializeField] private bool hideChildrenOnly = false; // 子オブジェクトのみを隠すかどうか
    
    private List<GameObject> hiddenObjects = new List<GameObject>();
    private bool isHidden = false;
    
    void Start()
    {
        // ボタンのクリックイベントを設定
        if (hideButton != null)
            hideButton.onClick.AddListener(HideScene);
        
        if (showButton != null)
            showButton.onClick.AddListener(ShowScene);
        
        // もしボタンが設定されていない場合は、このオブジェクトがボタンかチェック
        if (hideButton == null)
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                hideButton = button;
                hideButton.onClick.AddListener(ToggleScene);
            }
        }
        
        // 隠すオブジェクトが設定されていない場合の処理
        if (objectsToHide.Count == 0 && hideAllRootObjects)
        {
            FindAllRootObjects();
        }
    }
    
    /// <summary>
    /// シーンを隠す
    /// </summary>
    public void HideScene()
    {
        if (isHidden) return;
        
        hiddenObjects.Clear();
        
        if (hideAllRootObjects)
        {
            FindAllRootObjects();
        }
        
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                if (hideChildrenOnly)
                {
                    // 子オブジェクトのみを隠す
                    HideChildren(obj);
                }
                else
                {
                    // オブジェクト全体を隠す
                    obj.SetActive(false);
                    hiddenObjects.Add(obj);
                }
            }
        }
        
        isHidden = true;
        Debug.Log("シーンを隠しました");
    }
    
    /// <summary>
    /// シーンを表示する
    /// </summary>
    public void ShowScene()
    {
        if (!isHidden) return;
        
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                if (hideChildrenOnly)
                {
                    // 子オブジェクトを表示
                    ShowChildren(obj);
                }
                else
                {
                    // オブジェクト全体を表示
                    obj.SetActive(true);
                }
            }
        }
        
        hiddenObjects.Clear();
        isHidden = false;
        Debug.Log("シーンを表示しました");
    }
    
    /// <summary>
    /// シーンの表示/非表示を切り替える
    /// </summary>
    public void ToggleScene()
    {
        if (isHidden)
        {
            ShowScene();
        }
        else
        {
            HideScene();
        }
    }
    
    /// <summary>
    /// すべてのルートオブジェクトを見つける
    /// </summary>
    private void FindAllRootObjects()
    {
        objectsToHide.Clear();
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject obj in rootObjects)
        {
            // このスクリプトがアタッチされているオブジェクトとUIキャンバスは除外
            if (obj != this.gameObject && !obj.name.Contains("Canvas"))
            {
                objectsToHide.Add(obj);
            }
        }
    }
    
    /// <summary>
    /// 子オブジェクトを隠す
    /// </summary>
    private void HideChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child.activeInHierarchy)
            {
                child.SetActive(false);
                hiddenObjects.Add(child);
            }
        }
    }
    
    /// <summary>
    /// 子オブジェクトを表示する
    /// </summary>
    private void ShowChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (hiddenObjects.Contains(child))
            {
                child.SetActive(true);
            }
        }
    }
    
    void OnDestroy()
    {
        // メモリリークを防ぐためにイベントを解除
        if (hideButton != null)
            hideButton.onClick.RemoveListener(HideScene);
        
        if (showButton != null)
            showButton.onClick.RemoveListener(ShowScene);
    }
}