using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class ARSceneSetup
{
    [MenuItem("AR Exam/Setup AR Monument Scene")]
    public static void CreateScene()
    {
        // 1. Create a new empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Main Camera
        GameObject camObj = new GameObject("AR_Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f, 1f);
        cam.tag = "MainCamera";
        camObj.transform.position = new Vector3(0, 1.8f, -2.8f);
        camObj.transform.rotation = Quaternion.Euler(28f, 0, 0);
        camObj.AddComponent<AudioListener>();

        // 3. Directional Light
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1f, 0.96f, 0.88f);
        light.intensity = 1.3f;
        lightObj.transform.rotation = Quaternion.Euler(45f, -30f, 0);

        // Ambient Fill Light
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.45f);

        // 4. UI Canvas
        GameObject canvasObj = new GameObject("AR_UI_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Camera Feed Background (RawImage)
        GameObject bgObj = new GameObject("WebCamBackground");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RawImage bgImage = bgObj.AddComponent<RawImage>();
        bgImage.color = new Color(0.15f, 0.16f, 0.2f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Header Panel / Status Badge
        GameObject headerObj = new GameObject("HeaderPanel");
        headerObj.transform.SetParent(canvasObj.transform, false);
        Image headerBg = headerObj.AddComponent<Image>();
        headerBg.color = new Color(0.05f, 0.08f, 0.15f, 0.85f);
        RectTransform headerRect = headerObj.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 70);

        // Status Text
        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(headerObj.transform, false);
        Text statusText = statusObj.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        statusText.fontSize = 20;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        statusText.text = "<color=#00FF88>● Marker Recognized [AR Tracking Active]</color>";
        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchorMin = Vector2.zero;
        statusRect.anchorMax = Vector2.one;
        statusRect.sizeDelta = Vector2.zero;

        // Bottom Historical Information Card
        GameObject infoCardObj = new GameObject("MonumentInfoCard");
        infoCardObj.transform.SetParent(canvasObj.transform, false);
        Image cardBg = infoCardObj.AddComponent<Image>();
        cardBg.color = new Color(0.08f, 0.1f, 0.16f, 0.9f);
        RectTransform cardRect = infoCardObj.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.05f, 0.03f);
        cardRect.anchorMax = new Vector2(0.95f, 0.28f);
        cardRect.pivot = new Vector2(0.5f, 0);
        cardRect.sizeDelta = Vector2.zero;

        // Info Title
        GameObject titleObj = new GameObject("MonumentTitle");
        titleObj.transform.SetParent(infoCardObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = statusText.font;
        titleText.fontSize = 20;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = new Color(1f, 0.85f, 0.4f);
        titleText.text = "🏛️ Historical Monument: Classical Temple Reconstruction";
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.03f, 0.65f);
        titleRect.anchorMax = new Vector2(0.97f, 0.95f);
        titleRect.sizeDelta = Vector2.zero;

        // Info Details
        GameObject descObj = new GameObject("MonumentDescription");
        descObj.transform.SetParent(infoCardObj.transform, false);
        Text descText = descObj.AddComponent<Text>();
        descText.font = statusText.font;
        descText.fontSize = 15;
        descText.color = new Color(0.9f, 0.92f, 0.95f);
        descText.text = "• Era: 5th Century BCE (Classical Antiquity)\n• Architecture: Hexastyle Doric Peripteral Temple with Crepidoma Base\n• Working Principle: Camera Feed → Marker Tracking → 6-DOF Pose → 3D Overlay\n• Interaction: Drag to Rotate 360° | Scroll/Pinch to Zoom | Press [Space] to Toggle Tracking";
        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.03f, 0.05f);
        descRect.anchorMax = new Vector2(0.97f, 0.65f);
        descRect.sizeDelta = Vector2.zero;

        // 5. Image Target Anchor (World Space)
        GameObject targetAnchor = new GameObject("ImageTarget_Monument");
        targetAnchor.transform.position = Vector3.zero;

        // Marker Graphic Quad (shows what the physical image marker looks like on the table/floor)
        GameObject markerQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        markerQuad.name = "Marker_Image_Reference";
        markerQuad.transform.SetParent(targetAnchor.transform, false);
        markerQuad.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        markerQuad.transform.localScale = new Vector3(2.5f, 2.5f, 1f);

        // Assign Marker Texture to Quad
        Texture2D markerTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/MonumentMarker.png");
        if (markerTex != null)
        {
            Material markerMat = new Material(Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse"));
            markerMat.mainTexture = markerTex;
            AssetDatabase.CreateAsset(markerMat, "Assets/Materials/MarkerMaterial.mat");
            markerQuad.GetComponent<MeshRenderer>().material = markerMat;
        }

        // 6. 3D Monument Reconstruction (Child of Image Target!)
        GameObject monumentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/HistoricalMonument.obj");
        GameObject monumentInstance = null;
        if (monumentPrefab != null)
        {
            monumentInstance = (GameObject)PrefabUtility.InstantiatePrefab(monumentPrefab);
            monumentInstance.name = "3D_Monument_Reconstruction";
            monumentInstance.transform.SetParent(targetAnchor.transform, false);
            monumentInstance.transform.localPosition = new Vector3(0, 0.05f, 0);
            monumentInstance.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            
            // Attach Monument Controller
            MonumentController controller = monumentInstance.AddComponent<MonumentController>();
            controller.autoRotateSpeed = 22f;
            controller.isAutoRotating = true;
        }

        // 7. AR Tracking Manager & Simulation
        ARMarkerTrackManager trackManager = targetAnchor.AddComponent<ARMarkerTrackManager>();
        trackManager.monument3DModel = monumentInstance;
        trackManager.infoPanel = infoCardObj;
        trackManager.statusText = statusText;
        trackManager.monumentTitleText = titleText;
        trackManager.monumentDescriptionText = descText;

        // WebCam AR Simulator
        WebCamARSimulator sim = camObj.AddComponent<WebCamARSimulator>();
        sim.cameraBackgroundDisplay = bgImage;
        sim.trackerManager = trackManager;

        // Auto-trigger marker detection so it appears immediately on launch
        trackManager.OnTrackingFound();

        // 8. Save Scene
        string scenePath = "Assets/Scenes/AR_Monument_Scene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[AR Exam Setup] Scene created and saved to: " + scenePath);

        // Set in EditorBuildSettings
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[] {
            new EditorBuildSettingsScene(scenePath, true)
        };
    }

    [MenuItem("AR Exam/Build Standalone Player")]
    public static void BuildStandalone()
    {
        string buildDir = "Builds/Linux";
        if (!System.IO.Directory.Exists(buildDir))
        {
            System.IO.Directory.CreateDirectory(buildDir);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/AR_Monument_Scene.unity" };
        buildPlayerOptions.locationPathName = buildDir + "/AR_Monument_App.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.None;

        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEditor.Build.Reporting.BuildSummary summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("[AR Exam Build] Build succeeded: " + summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("[AR Exam Build] Build failed with result: " + summary.result);
        }
    }
}
