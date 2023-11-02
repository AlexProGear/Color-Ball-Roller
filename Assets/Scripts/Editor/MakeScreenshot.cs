using UnityEditor;
using UnityEngine;

public class MakeScreenshot : MonoBehaviour
{
    [MenuItem("Screenshot/Take Screenshot")]
    private static void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("screenshot.png");
    }
}
