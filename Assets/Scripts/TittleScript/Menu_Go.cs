using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Unityエンジンのシーン管理プログラムを利用する

public class Menu_Go : MonoBehaviour 
{
        public void Change_button() 
        {
            SceneManager.LoadScene("MenuScene");
        }
}