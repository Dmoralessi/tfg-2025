using UnityEngine;

public class DebugNPCOverlay : MonoBehaviour
{
    public Vector3 labelOffset = new Vector3(0f, 2.2f, 0f);
    public Color textColor = Color.white;

    BasicNPCController basicNPC;
    AdvancedNPCController advancedNPC;

    void Awake()
    {
        basicNPC = GetComponent<BasicNPCController>();
        advancedNPC = GetComponent<AdvancedNPCController>();
    }

    void OnGUI()
    {
        if (DebugOverlayManager.Instance == null)
            return;

        var settings = DebugOverlayManager.Instance.debugSettings;

        if (!settings.showOverlay)
            return;

        if (settings.showState)
        {
            DrawStateLabel();
        }
    }

    void DrawStateLabel()
    {
        string label = GetLabelText();
        if (string.IsNullOrEmpty(label))
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 worldPos = transform.position + labelOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
            return;

        GUI.color = textColor;

        float width = 160f;
        float height = 50f;

        Rect rect = new Rect(
            screenPos.x - width * 0.5f,
            Screen.height - screenPos.y - 20f,
            width,
            height
        );

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(rect, label, style);
    }

    string GetLabelText()
    {
        if (basicNPC != null)
        {
            return $"BASIC NPC\n{basicNPC.currentState}";
        }

        if (advancedNPC != null)
        {
            return $"ADVANCED NPC\n{advancedNPC.currentState}";
        }

        return "";
    }
}
