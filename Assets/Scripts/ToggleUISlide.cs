using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleUISlide : MonoBehaviour
{
    public RectTransform UI;  // assigner dans l'Inspector
    public bool isOpenButton = true; // true pour bouton Menu, false pour bouton Continuer
    
    private Button button;
    private static bool isVisible = false;  // static pour partager entre tous les boutons
    private float slideDistance = 700f; // distance vers gauche/droite
    public float speed = 1f;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(HandleButtonClick);

        // Met l'UI caché au démarrage (seulement une fois)
        if (!isVisible)
        {
            UI.anchoredPosition += new Vector2(-slideDistance, 0);
        }
    }

    void HandleButtonClick()
    {
        if (isOpenButton && !isVisible)
        {
            // Bouton Menu : ouvre seulement si fermé
            OpenMenu();
        }
        else if (!isOpenButton && isVisible)
        {
            // Bouton Continuer : ferme seulement si ouvert
            CloseMenu();
        }
        else if (isOpenButton && isVisible)
        {
            // Bouton Menu utilisé pour fermer aussi
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
            // ⚡ Utilise unscaledDeltaTime pour que l'UI continue à bouger même en pause
            t += Time.unscaledDeltaTime * speed;
            UI.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
        UI.anchoredPosition = end;
    }
}
