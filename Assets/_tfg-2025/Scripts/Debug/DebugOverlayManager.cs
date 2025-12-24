using UnityEngine;
using UnityEngine.InputSystem;

public class DebugOverlayManager : MonoBehaviour
{
    public static DebugOverlayManager Instance { get; private set; }

    [System.Serializable]
    public struct DebugSettings
    {
        public bool showOverlay;
        public bool showState;
        public bool showNavigation;
        public bool showPerception;
    }

    public DebugSettings debugSettings = new DebugSettings
    {
        showOverlay = true,
        showState = true,
        showNavigation = true,
        showPerception = true
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            debugSettings.showOverlay = !debugSettings.showOverlay;
        }

        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            debugSettings.showState = !debugSettings.showState;
        }

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            debugSettings.showNavigation = !debugSettings.showNavigation;
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            debugSettings.showPerception = !debugSettings.showPerception;
        }
    }
}
