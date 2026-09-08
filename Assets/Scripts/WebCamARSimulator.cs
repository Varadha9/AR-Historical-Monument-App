using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WebCam AR Simulator:
/// - Supports physical webcams, virtual cameras (DroidCam), and simulated AR room backgrounds.
/// - Automatically handles DroidCam 640x480 resolution on Linux.
/// - Allows instant marker detection toggle via [Spacebar] or on-screen button.
/// </summary>
public class WebCamARSimulator : MonoBehaviour
{
    [Header("Camera Feed Background")]
    public RawImage cameraBackgroundDisplay;

    [Header("AR Tracker Manager")]
    public ARMarkerTrackManager trackerManager;

    private WebCamTexture webCamTexture;
    private bool cameraFeedRunning = false;

    void Start()
    {
        StartCoroutine(InitializeCamera());
    }

    private IEnumerator InitializeCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (WebCamTexture.devices.Length > 0)
        {
            // Pick Droidcam or first device
            string targetDevice = WebCamTexture.devices[0].name;
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                if (WebCamTexture.devices[i].name.ToLower().Contains("droid"))
                {
                    targetDevice = WebCamTexture.devices[i].name;
                    break;
                }
            }

            Debug.Log("[WebCam AR] Attempting to connect: " + targetDevice);

            // Droidcam on Linux natively streams at 640x480
            try
            {
                webCamTexture = new WebCamTexture(targetDevice, 640, 480, 30);
                webCamTexture.Play();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[WebCam AR] Failed with 640x480, trying default: " + e.Message);
                webCamTexture = new WebCamTexture(targetDevice);
                webCamTexture.Play();
            }

            // Wait a moment to see if frames arrive
            yield return new WaitForSeconds(0.5f);

            if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.width > 16)
            {
                cameraFeedRunning = true;
                if (cameraBackgroundDisplay != null)
                {
                    cameraBackgroundDisplay.texture = webCamTexture;
                    cameraBackgroundDisplay.color = Color.white;
                }
                Debug.Log("[WebCam AR] Successfully streaming from: " + targetDevice);
            }
            else
            {
                Debug.LogWarning("[WebCam AR] Camera did not stream frames. Using AR Room backdrop.");
                SetupSimulatedARBackground();
            }
        }
        else
        {
            SetupSimulatedARBackground();
        }
    }

    private void SetupSimulatedARBackground()
    {
        if (cameraBackgroundDisplay != null)
        {
            // Clean dark-blue AR studio backdrop so the 3D temple and marker pop out clearly
            cameraBackgroundDisplay.color = new Color(0.12f, 0.14f, 0.19f, 1f);
        }
    }

    void Update()
    {
        // Hotkey [Space] to toggle marker detection during lab exam demo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (trackerManager != null)
            {
                trackerManager.SimulateMarkerFound();
            }
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
