using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Stage1Button : MonoBehaviour 
{
        public void Stage1_button() //change_buttonという名前にします
        {
            SceneManager.LoadScene("Honma_TestScene");//secondを呼び出します
        }
}