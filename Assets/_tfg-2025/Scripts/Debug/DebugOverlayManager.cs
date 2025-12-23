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
    }

    public DebugSettings debugSettings = new DebugSettings
    {
        showOverlay = true,
        showState = true
    };

    void Awake()
    {
        Debug.Log("DebugOverlayManager Awake");

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
    }
}
