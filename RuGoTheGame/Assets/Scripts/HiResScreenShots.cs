using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour
{
    public int resWidth = 1024;
    public int resHeight = 768;
    private Camera mScreenShotCamera;
    //public  Camera camera;

    private string SCREENSHOT_SAVE_DIR = "screenshots/";

    public static string CreateScreenshotFilename(int width, int height)
    {
        return string.Format("screen_{0}x{1}_{2}.png",
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private void Awake()
    {
        mScreenShotCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown("k"))
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

            mScreenShotCamera.targetTexture = rt;

            mScreenShotCamera.Render();

            RenderTexture.active = rt;

            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

            // Read pixels from the screen
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

            // Do clean up
            mScreenShotCamera.targetTexture = null;
            RenderTexture.active = null; // nullify active to avoid error
            Destroy(rt);

            // Write the screenshot file
            byte[] bytes = screenShot.EncodeToPNG();
            string filePath = SCREENSHOT_SAVE_DIR + CreateScreenshotFilename(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filePath, bytes);
            Debug.Log(string.Format("Saved screenshot to: {0}", filePath));
        }
    }

}