using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PostRequest : MonoBehaviour
{
    public string screenShotURL = "https://www.argon-key-218614.appspot.com/transcribe/";

    // Use this for initialization
    void Start()
    {
        StartCoroutine(UploadPNG());
    }

    IEnumerator UploadPNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        string image64Array = Convert.ToBase64String(bytes);

        // Create a Web Form
        WWWForm form = new WWWForm();
        form.AddField("image", image64Array);

        // Upload to a cgi script
        using (var w = UnityWebRequest.Post(screenShotURL, form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                print(w.error);
            }
            else
            {
                print("Finished Uploading Screenshot");
            }
        }
    }
}