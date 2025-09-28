using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent(typeof(Button))]
public class QuitGame : MonoBehaviour
{
    private Button button;
 
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
    }
 
    private void TaskOnClick()
    {
        Debug.Log("Quitter le jeu !");
        Application.Quit();
 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}