using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class PhotoTaker : MonoBehaviour
{
	public Shader shader;
	public string screenShotURL = "http://www.argon-key-218614.appspot.com/transcribe";
	
	private PhotoCapture photoCapture = null;
	private Texture2D targetTexture = null;
	private Resolution cameraResolution;
	
	// Use this for initialization
	void Start ()
	{
		cameraResolution =
			PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
		targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			TakePhoto();
		}
	}

	void TakePhoto()
	{
		// Create a PhotoCapture object
		PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject)
		{
			photoCapture = captureObject;
			CameraParameters cameraParameters = new CameraParameters();
			cameraParameters.hologramOpacity = 0.0f;
			cameraParameters.cameraResolutionWidth = cameraResolution.width;
			cameraParameters.cameraResolutionHeight = cameraResolution.height;
			cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
			
			// Activate the camera
			photoCapture.StartPhotoModeAsync(cameraParameters, delegate(PhotoCapture.PhotoCaptureResult result)
			{
				// Take a picture
				photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
			});
		});
	}

	void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
	{
		// Copy the raw image data into the target texture
		photoCaptureFrame.UploadImageDataToTexture(targetTexture);
		
		// Create a GameObject to which the texture can be applied
		GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Renderer quadRenderer = quad.GetComponent<Renderer>();
		quadRenderer.material = new Material(shader);

		quad.transform.parent = this.transform;
		quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
		
		quadRenderer.material.SetTexture("_MainTex", targetTexture);
		
		// Upload the photo
		StartCoroutine(UploadPNG());
		
		// Deactivate the camera
		photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
	}

	void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
	{
		// Shutdown the photo capture resource
		photoCapture.Dispose();
		photoCapture = null;
	}
	
	IEnumerator UploadPNG()
	{
		// We should only read the screen after all rendering is complete
		yield return new WaitForEndOfFrame();

		// Encode texture into PNG
		byte[] bytes = targetTexture.EncodeToPNG();
		string image64Array = Convert.ToBase64String(bytes);
		// string bodyJsonString = "{\"image\": \"" + image64Array + "\"}";
		// Debug.Log("Body JSON: " + bodyJsonString);

		// Create a web request
		List<IMultipartFormSection> formData = new List<IMultipartFormSection>
		{
			new MultipartFormDataSection("image=" + image64Array)
		};
		
		UnityWebRequest request = UnityWebRequest.Post(screenShotURL, formData);
		request.chunkedTransfer = false;
		
		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Debug.LogError("STATUS: " + request.responseCode);
			Debug.LogError(request.error);
			if (request.isNetworkError)
			{
				Debug.LogError("NETWORK ERROR");
				Debug.LogError("ERROR: " + request.error);
				Debug.LogError(request.downloadHandler.text);
				Debug.LogError(request.url);
			}
			else
			{
				Debug.LogError("HTTP ERROR");
			}
		}
		else
		{
			Debug.Log("STATUS: " + request.responseCode);
			Debug.Log(request.downloadHandler.text);
		}
	}
}
