using UnityEngine;

public class DebugNPCOverlay : MonoBehaviour
{
    public Vector3 labelOffset = new Vector3(0f, 2.2f, 0f);
    public Color textColor = Color.white;

    BasicNPCController basicNPC;
    AdvancedNPCController advancedNPC;
    WaypointMovement waypointMovement;
    NavMeshMovement navMeshMovement;
    LineRenderer navLine;

    void Awake()
    {
        basicNPC = GetComponent<BasicNPCController>();
        advancedNPC = GetComponent<AdvancedNPCController>();
        waypointMovement = GetComponent<WaypointMovement>();
        navMeshMovement = GetComponent<NavMeshMovement>();

        CreateNavigationLine();
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

        if (settings.showNavigation)
        {
            UpdateNavigationLine();
        }
        else
        {
            navLine.enabled = false;
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

    void CreateNavigationLine()
    {
        GameObject lineObj = new GameObject("NavigationLine");
        lineObj.transform.SetParent(transform);

        navLine = lineObj.AddComponent<LineRenderer>();

        navLine.positionCount = 2;
        navLine.startWidth = 0.05f;
        navLine.endWidth = 0.05f;

        navLine.material = new Material(Shader.Find("Sprites/Default"));
        navLine.startColor = Color.cyan;
        navLine.endColor = Color.cyan;

        navLine.useWorldSpace = true;
        navLine.enabled = false;
    }

    void UpdateNavigationLine()
    {
        if (navLine == null)
            return;

        Vector3 target;

        if (waypointMovement != null)
        {
            target = waypointMovement.GetCurrentNavigationTarget();
        }
        else if (navMeshMovement != null)
        {
            target = navMeshMovement.GetCurrentNavigationTarget();
        }
        else
        {
            navLine.enabled = false;
            return;
        }

        const float GROUND_OFFSET = 0.02f;

        Vector3 start = transform.position;
        Vector3 end = target;

        start.y = GROUND_OFFSET;
        end.y = GROUND_OFFSET;

        navLine.enabled = true;
        navLine.SetPosition(0, start);
        navLine.SetPosition(1, end);
    }
}
