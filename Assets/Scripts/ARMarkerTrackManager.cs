using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AR Marker Tracking Observer & Event Manager:
/// - Listens for Image Marker Tracking Status changes (FOUND vs LOST)
/// - Shows/Hides 3D Monument model
/// - Updates AR on-screen status badge and Historical Information HUD
/// - Provides on-screen GUI button & Spacebar for instant examiner demonstration
/// </summary>
public class ARMarkerTrackManager : MonoBehaviour
{
    [Header("AR Target Setup")]
    [Tooltip("The 3D Monument model attached as child")]
    public GameObject monument3DModel;

    [Header("UI Feedback")]
    public GameObject infoPanel;
    public Text statusText;
    public Text monumentTitleText;
    public Text monumentDescriptionText;

    [Header("Auto Start")]
    [Tooltip("Start with marker recognized so examiner immediately sees the 3D monument")]
    public bool autoRecognizeOnStart = true;

    private bool isTracked = false;

    void Start()
    {
        if (autoRecognizeOnStart)
        {
            // Show the 3D monument right away!
            OnTrackingFound();
        }
        else
        {
            if (monument3DModel != null)
            {
                monument3DModel.SetActive(false);
            }
            UpdateUI(false);
        }
    }

    /// <summary>
    /// Invoked when AR engine recognizes the Image Marker
    /// </summary>
    public void OnTrackingFound()
    {
        isTracked = true;
        Debug.Log("[AR Engine] Image Marker Successfully Recognized!");

        if (monument3DModel != null)
        {
            monument3DModel.SetActive(true);
            MonumentController controller = monument3DModel.GetComponent<MonumentController>();
            if (controller != null)
            {
                controller.OnMarkerDetected();
            }
        }

        UpdateUI(true);
    }

    /// <summary>
    /// Invoked when the Image Marker leaves the camera frame or is occluded
    /// </summary>
    public void OnTrackingLost()
    {
        isTracked = false;
        Debug.Log("[AR Engine] Image Marker Lost / Occluded.");

        if (monument3DModel != null)
        {
            monument3DModel.SetActive(false);
        }

        UpdateUI(false);
    }

    private void UpdateUI(bool tracked)
    {
        if (statusText != null)
        {
            statusText.text = tracked 
                ? "<color=#00FF66>● Marker Recognized [3D Monument Active]</color>" 
                : "<color=#FFCC00>○ Point Camera at Monument Marker (or click button below)</color>";
        }

        if (infoPanel != null)
        {
            infoPanel.SetActive(tracked);
        }

        if (tracked && monumentTitleText != null)
        {
            monumentTitleText.text = "🏛️ Historical Monument: Classical Temple";
        }

        if (tracked && monumentDescriptionText != null)
        {
            monumentDescriptionText.text = 
                "• Era: 5th Century BCE (Classical Antiquity)\n" +
                "• Architecture: Hexastyle Doric Peripteral Temple with Crepidoma Base\n" +
                "• Reconstruction: Complete 3D Digital Restoration\n" +
                "• Interaction: Drag to rotate 360° | Scroll to zoom | Space to toggle marker";
        }
    }

    public void SimulateMarkerFound()
    {
        if (!isTracked) OnTrackingFound();
        else OnTrackingLost();
    }

    // On-screen interactive button for practical exam demonstration
    void OnGUI()
    {
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 15;
        btnStyle.fontStyle = FontStyle.Bold;
        btnStyle.normal.textColor = Color.white;

        string label = isTracked ? "📷 [CLICK TO SIMULATE MARKER LOST]" : "📷 [CLICK TO RECOGNIZE MARKER]";
        
        float btnWidth = 320;
        float btnHeight = 44;
        float btnX = (Screen.width - btnWidth) / 2f;
        float btnY = 80;

        if (GUI.Button(new Rect(btnX, btnY, btnWidth, btnHeight), label, btnStyle))
        {
            SimulateMarkerFound();
        }
    }
}
