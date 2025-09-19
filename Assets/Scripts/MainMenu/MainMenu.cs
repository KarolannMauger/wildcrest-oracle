using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MainMenu : MonoBehaviour
{
    private Button btn;

    public float reloadDelay = 2f;
    public string sceneToLoad = "Overworld";

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (reloadDelay <= 0f) SceneManager.LoadScene(sceneToLoad);
        else StartCoroutine(CoDelay(sceneToLoad, reloadDelay));
    }

    IEnumerator CoDelay(string name, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SceneManager.LoadScene(name);
    }
}
