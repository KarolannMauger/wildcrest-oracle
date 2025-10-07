using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleUISlide : MonoBehaviour
{
    public RectTransform UI;  // Assign in Inspector
    public bool isOpenButton = true; // true for Menu button, false for Continue button
    
    private Button button;
    private static bool isVisible = false;  // static to share between all buttons
    private float slideDistance = 700f; // distance to left/right
    public float speed = 1f;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(HandleButtonClick);

        // Hide UI at startup (only once)
        if (!isVisible)
        {
            UI.anchoredPosition += new Vector2(-slideDistance, 0);
        }
    }

    void HandleButtonClick()
    {
        if (isOpenButton && !isVisible)
        {
            // Menu button: opens only if closed
            OpenMenu();
        }
        else if (!isOpenButton && isVisible)
        {
            // Continue button: closes only if open
            CloseMenu();
        }
        else if (isOpenButton && isVisible)
        {
            // Menu button used to close as well
            CloseMenu();
        }
    }

    void OpenMenu()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(UI.anchoredPosition, UI.anchoredPosition + new Vector2(slideDistance, 0)));
        Time.timeScale = 0f;
        isVisible = true;
    }

    void CloseMenu()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(UI.anchoredPosition, UI.anchoredPosition + new Vector2(-slideDistance, 0)));
        Time.timeScale = 1f;
        isVisible = false;
    }

    IEnumerator Slide(Vector2 start, Vector2 end)
    {
        float t = 0;
        while (t < 1)
        {
            // Use unscaledDeltaTime so UI continues to move even when paused
            t += Time.unscaledDeltaTime * speed;
            UI.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
        UI.anchoredPosition = end;
    }
}
