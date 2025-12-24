using UnityEngine;

public class DebugNPCOverlay : MonoBehaviour
{
    public Vector3 labelOffset = new Vector3(0f, 2.2f, 0f);
    public Color textColor = Color.white;

    BasicNPCController basicNPC;
    AdvancedNPCController advancedNPC;
    WaypointMovement waypointMovement;
    NavMeshMovement navMeshMovement;
    ProximityPerception proximityPerception;
    VisionPerception visionPerception;
    LineRenderer navLine;
    LineRenderer perceptionLine;
    LineRenderer visionConeLine;

    void Awake()
    {
        basicNPC = GetComponent<BasicNPCController>();
        advancedNPC = GetComponent<AdvancedNPCController>();
        waypointMovement = GetComponent<WaypointMovement>();
        navMeshMovement = GetComponent<NavMeshMovement>();
        proximityPerception = GetComponent<ProximityPerception>();
        visionPerception = GetComponent<VisionPerception>();

        CreateNavigationLine();
        CreatePerceptionLine();
        CreateVisionConeLine();
    }

    void OnGUI()
    {
        if (DebugOverlayManager.Instance == null)
            return;

        var settings = DebugOverlayManager.Instance.debugSettings;

        if (settings.showOverlay && settings.showState)
        {
            DrawStateLabel();
        }

        if (settings.showOverlay && settings.showNavigation)
        {
            UpdateNavigationLine();
        }
        else
        {
            navLine.enabled = false;
        }

        if (settings.showOverlay && settings.showPerception)
        {
            if (basicNPC != null)
            {
                visionConeLine.enabled = false;
                DrawBasicPerception(proximityPerception);
            }
            else if (advancedNPC != null)
            {
                perceptionLine.enabled = false;
                DrawAdvancedPerception(visionPerception);
            }
        }
        else
        {
            perceptionLine.enabled = false;
            visionConeLine.enabled = false;
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

    void CreatePerceptionLine()
    {
        GameObject lineObj = new GameObject("PerceptionLine");
        lineObj.transform.SetParent(transform);

        perceptionLine = lineObj.AddComponent<LineRenderer>();
        perceptionLine.loop = true;
        perceptionLine.useWorldSpace = true;

        perceptionLine.startWidth = 0.03f;
        perceptionLine.endWidth = 0.03f;

        perceptionLine.material = new Material(Shader.Find("Sprites/Default"));
        perceptionLine.startColor = Color.red;
        perceptionLine.endColor = Color.red;

        perceptionLine.enabled = false;
    }

    void DrawBasicPerception(ProximityPerception perception)
    {
        int segments = 40;
        float radius = perception.detectionRadius;

        perceptionLine.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                0.02f,
                Mathf.Sin(angle) * radius
            );

            perceptionLine.SetPosition(i, transform.position + pos);
        }

        perceptionLine.enabled = true;
    }

    void DrawAdvancedPerception(VisionPerception perception)
    {
        int arcSegments = 30;
        float viewDistance = perception.viewDistance;
        float halfAngle = perception.viewAngle * 0.5f;

        Vector3 origin = transform.position;
        origin.y = 0.02f;

        visionConeLine.positionCount = arcSegments + 4;
        Vector3 leftDir = Quaternion.Euler(0f, -halfAngle, 0f) * transform.forward;
        //Vector3 rightDir = Quaternion.Euler(0f, halfAngle, 0f) * transform.forward;

        int index = 0;
        visionConeLine.SetPosition(index++, origin);
        visionConeLine.SetPosition(index++, origin + leftDir * viewDistance);

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = (float)i / arcSegments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            visionConeLine.SetPosition(index++, origin + dir * viewDistance);
        }

        visionConeLine.SetPosition(index, origin);
        visionConeLine.enabled = true;
    }

    void CreateVisionConeLine()
    {
        GameObject lineObj = new GameObject("VisionConeLine");
        lineObj.transform.SetParent(transform);

        visionConeLine = lineObj.AddComponent<LineRenderer>();
        visionConeLine.useWorldSpace = true;
        visionConeLine.loop = false;

        visionConeLine.startWidth = 0.03f;
        visionConeLine.endWidth = 0.03f;

        visionConeLine.material = new Material(Shader.Find("Sprites/Default"));
        visionConeLine.startColor = Color.green;
        visionConeLine.endColor = Color.green;

        visionConeLine.enabled = false;
    }
}
