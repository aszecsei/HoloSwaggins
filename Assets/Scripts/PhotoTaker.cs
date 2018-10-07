using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity.InputModule;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.XR.WSA.WebCam;

public class PhotoTaker : MonoBehaviour 
{
	[FormerlySerializedAs("shader")] public Shader Shader;
	[FormerlySerializedAs("screenShotURL")] public string ScreenShotUrl = "http://www.argon-key-218614.appspot.com/transcribe";
	public GameObject TextMesh;
	public Transform Cursor;
    public string lang;
    public bool modelOn = false;
    public string ModelUrl = "http://www.argon-key-218614.appspot.com/transcribe/model";


    private PhotoCapture _photoCapture;
	private Texture2D _targetTexture;
	private Resolution _cameraResolution;
	
	// Use this for initialization
	private void Start ()
	{
		_cameraResolution =
			PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
		_targetTexture = new Texture2D(_cameraResolution.width, _cameraResolution.height, TextureFormat.RGB24, true);
    }
	
	// Update is called once per frame
	private void Update () { }

	private void TakePhoto()
	{
		// Create a PhotoCapture object
		PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject)
		{
			_photoCapture = captureObject;
			var cameraParameters = new CameraParameters
			{
				hologramOpacity = 0.0f,
				cameraResolutionWidth = _cameraResolution.width,
				cameraResolutionHeight = _cameraResolution.height,
				pixelFormat = CapturePixelFormat.BGRA32
			};

			// Activate the camera
			_photoCapture.StartPhotoModeAsync(cameraParameters, delegate(PhotoCapture.PhotoCaptureResult result)
			{
				// Take a picture
				_photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
			});
		});
	}

	private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
	{
       


        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(_targetTexture);
		
		// Create a GameObject to which the texture can be applied
		var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		var quadRenderer = quad.GetComponent<Renderer>();
		quadRenderer.material = new Material(Shader);

		quad.transform.parent = this.transform;
		quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
		
		quadRenderer.material.SetTexture("_MainTex", _targetTexture);
		
		// Upload the photo
		StartCoroutine(UploadPNG());
		
		// Deactivate the camera
		_photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
	}

	private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
	{
		// Shutdown the photo capture resource
		_photoCapture.Dispose();
		_photoCapture = null;
	}
	
	private IEnumerator UploadPNG()
	{
		// We should only read the screen after all rendering is complete
		yield return new WaitForEndOfFrame();

		var pos = Cursor.position;

		// Encode texture into PNG
		var bytes = _targetTexture.EncodeToJPG();
		var image64Array = Convert.ToBase64String(bytes);
        // string bodyJsonString = "{\"image\": \"" + image64Array + "\"}";
        // Debug.Log("Body JSON: " + bodyJsonString);


        // Create formData for web request
        var formData = new List<IMultipartFormSection>
		{
			new MultipartFormDataSection("image", image64Array)
        };

        // Create a web request
        //RequestText(formData);
        var request = UnityWebRequest.Post(ScreenShotUrl + "/" + lang, formData);
        request.chunkedTransfer = false;

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.isNetworkError ? "NETWORK ERROR" : "HTTP ERROR");
            Debug.LogError("STATUS: " + request.responseCode);
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            var text = Instantiate<GameObject>(TextMesh);
            text.transform.position = Cursor.position;
            var tm = text.GetComponent<TextMeshPro>();
            tm.SetText(request.downloadHandler.text);
        }


        // Create a web request
        if (modelOn)
        {
            //RequestModels(formData);
            var modelRequest = UnityWebRequest.Post(ModelUrl, formData);
            modelRequest.chunkedTransfer = false;

            yield return modelRequest.SendWebRequest();

            if (modelRequest.isNetworkError || modelRequest.isHttpError)
            {
                Debug.LogError(modelRequest.isNetworkError ? "NETWORK ERROR" : "HTTP ERROR");
                Debug.LogError("STATUS: " + modelRequest.responseCode);
                Debug.LogError(modelRequest.error);
                Debug.LogError(modelRequest.downloadHandler.text);
            }
            else
            {
                //Debug.Log(modelRequest.downloadHandler.text);
                //var text = Instantiate<GameObject>(TextMesh);
                //text.transform.position = Cursor.position;
                //var tm = text.GetComponent<TextMeshPro>();
                //tm.SetText(modelRequest.downloadHandler.text);
            }
        }
    }

	public void OnScan()
	{
		TakePhoto();
	}
}
