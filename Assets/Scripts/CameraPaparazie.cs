using UnityEngine;
using System.Collections;
using System.IO;
public class CameraPaparazie : MonoBehaviour
{
    public int resWidth = 2550;
    public int resHeight = 3300;

    private Transform target;
    private bool takeHiResShot = false;

    public float cameraPositionVariantYMin = 11;
    public float cameraPositionVariantYMax = 17;

    public float cameraPositionVariantZMin = -18;
    public float cameraPositionVariantZMax = -10;

    public float cameraPositionVariantXMin = -10;
    public float cameraPositionVariantXMax = 10;

    public void Start()
    {
        target = transform.parent;
        if (!Directory.Exists(Application.dataPath + "/screenshots"))
        {
            Directory.CreateDirectory(Application.dataPath + "/screenshots");
        }
    }
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",Application.dataPath, width, height,System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()
    {

        if (takeHiResShot)
        {
            ChooseRandomPositionBeforeScreen();
            transform.LookAt(target);
            float randomYVariation = Random.Range(-35, 35);
            Vector3 newRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z) + new Vector3(0, randomYVariation, 0);
            transform.eulerAngles += newRotation;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            GetComponent<Camera>().targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }

    public void ChooseRandomPositionBeforeScreen()
    {
        float yNewPos = Random.Range(cameraPositionVariantYMin, cameraPositionVariantYMax);
        float zNewPos = Random.Range(cameraPositionVariantZMin, cameraPositionVariantZMax);
        float xNewPos = Random.Range(cameraPositionVariantXMin, cameraPositionVariantXMax);

        Vector3 newPosition = target.position + new Vector3(xNewPos, yNewPos, zNewPos);
        transform.position = newPosition;
    }
}
