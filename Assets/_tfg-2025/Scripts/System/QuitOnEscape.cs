using UnityEngine;
using UnityEngine.InputSystem;

public class QuitOnEscape : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}
