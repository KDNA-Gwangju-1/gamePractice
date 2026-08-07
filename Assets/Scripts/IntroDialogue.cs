using UnityEngine;
using UnityEngine.InputSystem;

public class IntroDialogue : MonoBehaviour
{
    public GameObject panel;

    void Awake()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            if (panel != null) panel.SetActive(false);
            enabled = false;
        }
    }
}
