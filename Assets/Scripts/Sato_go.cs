using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Unityエンジンのシーン管理プログラムを利用する

public class Sato_go : MonoBehaviour //changeという名前にします
{
        public void change_button() //change_buttonという名前にします
        {
            SceneManager.LoadScene("Sato_TestScene");//secondを呼び出します
        }
}
