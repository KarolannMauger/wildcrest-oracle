using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackToMainMenu : MonoBehaviour
{
    private Button btn;

    public float reloadDelay = 1f;
    public string mainMenuScene = "Launch";

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (reloadDelay <= 0f) SceneManager.LoadScene(mainMenuScene);
        else StartCoroutine(CoDelay(mainMenuScene, reloadDelay));
    }

    IEnumerator CoDelay(string name, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(name);
    }
}