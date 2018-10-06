using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class PhotoTaker : MonoBehaviour
{
	public Shader shader;
	
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
		
		// Deactivate the camera
		photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
	}

	void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
	{
		// Shutdown the photo capture resource
		photoCapture.Dispose();
		photoCapture = null;
	}
}
