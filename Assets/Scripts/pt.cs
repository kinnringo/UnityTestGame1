using UnityEngine;
using UnityEngine.UI;

public class SliderSaveManager : MonoBehaviour
{
    [Header("スライダー設定")]
    public Slider slider1;
    public Slider slider2;

    [Header("保存キー")]
    public string slider1Key = "Slider1Value";
    public string slider2Key = "Slider2Value";

    [Header("デフォルト値")]
    public float slider1DefaultValue = 0.5f;
    public float slider2DefaultValue = 0.5f;

    [Header("自動保存")]
    public bool autoSave = true;

    void Start()
    {
        // スライダーの値を読み込み
        LoadSliderValues();

        // スライダーの値が変更されたときのイベントを登録
        if (slider1 != null)
        {
            slider1.onValueChanged.AddListener(OnSlider1Changed);
        }

        if (slider2 != null)
        {
            slider2.onValueChanged.AddListener(OnSlider2Changed);
        }
    }

    void OnDestroy()
    {
        // イベントの登録を解除
        if (slider1 != null)
        {
            slider1.onValueChanged.RemoveListener(OnSlider1Changed);
        }

        if (slider2 != null)
        {
            slider2.onValueChanged.RemoveListener(OnSlider2Changed);
        }
    }

    // スライダー1の値が変更されたときの処理
    void OnSlider1Changed(float value)
    {
        if (autoSave)
        {
            SaveSlider1Value(value);
        }
    }

    // スライダー2の値が変更されたときの処理
    void OnSlider2Changed(float value)
    {
        if (autoSave)
        {
            SaveSlider2Value(value);
        }
    }

    // スライダーの値を読み込み
    public void LoadSliderValues()
    {
        if (slider1 != null)
        {
            float savedValue1 = PlayerPrefs.GetFloat(slider1Key, slider1DefaultValue);
            slider1.value = savedValue1;
        }

        if (slider2 != null)
        {
            float savedValue2 = PlayerPrefs.GetFloat(slider2Key, slider2DefaultValue);
            slider2.value = savedValue2;
        }

        Debug.Log($"スライダーの値を読み込みました: Slider1={slider1?.value}, Slider2={slider2?.value}");
    }

    // スライダーの値を保存
    public void SaveSliderValues()
    {
        if (slider1 != null)
        {
            SaveSlider1Value(slider1.value);
        }

        if (slider2 != null)
        {
            SaveSlider2Value(slider2.value);
        }

        Debug.Log($"スライダーの値を保存しました: Slider1={slider1?.value}, Slider2={slider2?.value}");
    }

    // スライダー1の値を保存
    public void SaveSlider1Value(float value)
    {
        PlayerPrefs.SetFloat(slider1Key, value);
        PlayerPrefs.Save();
    }

    // スライダー2の値を保存
    public void SaveSlider2Value(float value)
    {
        PlayerPrefs.SetFloat(slider2Key, value);
        PlayerPrefs.Save();
    }

    // 保存されたデータをリセット
    public void ResetSliderValues()
    {
        PlayerPrefs.DeleteKey(slider1Key);
        PlayerPrefs.DeleteKey(slider2Key);

        if (slider1 != null)
        {
            slider1.value = slider1DefaultValue;
        }

        if (slider2 != null)
        {
            slider2.value = slider2DefaultValue;
        }

        Debug.Log("スライダーの値をリセットしました");
    }

    // 現在の値を取得
    public float GetSlider1Value()
    {
        return slider1 != null ? slider1.value : slider1DefaultValue;
    }

    public float GetSlider2Value()
    {
        return slider2 != null ? slider2.value : slider2DefaultValue;
    }

    // 値を直接設定
    public void SetSlider1Value(float value)
    {
        if (slider1 != null)
        {
            slider1.value = value;
            if (autoSave)
            {
                SaveSlider1Value(value);
            }
        }
    }

    public void SetSlider2Value(float value)
    {
        if (slider2 != null)
        {
            slider2.value = value;
            if (autoSave)
            {
                SaveSlider2Value(value);
            }
        }
    }
}