using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloLensCameraStream;

#if WINDOWS_UWP && XR_PLUGIN_OPENXR
using Windows.Perception.Spatial;
#endif

#if WINDOWS_UWP
using Windows.UI.Input.Spatial;
#endif

public class CameraController : MonoBehaviour
{
    public Texture2D pictureTexture;

    public int resolutionWidth;
    public int resolutionHeight;
    private HoloLensCameraStream.Resolution _resolution;
    private VideoCapture videoCapture;

    private IntPtr spatialCoordinateSystemPtr;

    public Matrix4x4 camera2WorldMatrix;
    public Matrix4x4 projectionMatrix;

    Matrix4x4 camera2WorldMatrix_local;
    Matrix4x4 projectionMatrix_local;

    int ticks = 0;

#if WINDOWS_UWP && XR_PLUGIN_OPENXR
    SpatialCoordinateSystem _spatialCoordinateSystem;
#endif

    private byte[] _latestImageBytes;

#if WINDOWS_UWP
    private SpatialInteractionManager _spatialInteraction;
#endif

    private class SampleStruct
    {
        public float[] camera2WorldMatrix, projectionMatrix;
        public byte[] data;
    }

    void Start()
    {
#if WINDOWS_UWP

#if XR_PLUGIN_WINDOWSMR

        _spatialCoordinateSystemPtr = UnityEngine.XR.WindowsMR.WindowsMREnvironment.OriginSpatialCoordinateSystem;

#elif XR_PLUGIN_OPENXR

        _spatialCoordinateSystem = Microsoft.MixedReality.OpenXR.PerceptionInterop.GetSceneCoordinateSystem(UnityEngine.Pose.identity) as SpatialCoordinateSystem;

#elif BUILTIN_XR

#if UNITY_2017_2_OR_NEWER
        _spatialCoordinateSystemPtr = UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
#else
        _spatialCoordinateSystemPtr = UnityEngine.VR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr();
#endif

#endif

#endif

        CameraStreamHelper.Instance.GetVideoCaptureAsync(OnVideoCaptureCreated);
    }

    private void OnVideoCaptureCreated(VideoCapture v)
    {
        if (v == null)
        {
            Debug.LogError("No VideoCapture found");
            return;
        }
        videoCapture = v;

#if WINDOWS_UWP

#if XR_PLUGIN_OPENXR
        CameraStreamHelper.Instance.SetNativeISpatialCoordinateSystem(_spatialCoordinateSystem);
#elif XR_PLUGIN_WINDOWSMR || BUILTIN_XR
        CameraStreamHelper.Instance.SetNativeISpatialCoordinateSystemPtr(_spatialCoordinateSystemPtr);
#endif

#endif

        _resolution = CameraStreamHelper.Instance.GetLowestResolution();
        Debug.Log($"Resolution: {_resolution.ToString()}");

        _resolution = new HoloLensCameraStream.Resolution(resolutionWidth, resolutionHeight);
        float frameRate = CameraStreamHelper.Instance.GetHighestFrameRate(_resolution);


        videoCapture.FrameSampleAcquired += OnFrameSampleAcquired;

        CameraParameters cameraParams = new CameraParameters();
        cameraParams.cameraResolutionHeight = _resolution.height;
        cameraParams.cameraResolutionWidth = _resolution.width;
        cameraParams.frameRate = Mathf.RoundToInt(frameRate);
        cameraParams.pixelFormat = CapturePixelFormat.BGRA32;

        UnityEngine.WSA.Application.InvokeOnAppThread(() => { pictureTexture = new Texture2D(_resolution.width, _resolution.height, TextureFormat.BGRA32, false); }, false);

        videoCapture.StartVideoModeAsync(cameraParams, OnVideoModeStarted);

        Debug.LogWarning($"{_resolution.height},  {_resolution.width}, {cameraParams.frameRate}");
    }

    private void OnVideoModeStarted(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not start video mode.");
            return;
        }

        Debug.Log("Video capture started.");
    }

    private void OnFrameSampleAcquired(VideoCaptureSample sample)
    {
        // Allocate byteBuffer
        if (_latestImageBytes == null || _latestImageBytes.Length < sample.dataLength)
            _latestImageBytes = new byte[sample.dataLength];

        // Fill frame struct 
        SampleStruct s = new SampleStruct();
        sample.CopyRawImageDataIntoBuffer(_latestImageBytes);
        s.data = _latestImageBytes;

        // Get the cameraToWorldMatrix and projectionMatrix
        if (!sample.TryGetCameraToWorldMatrix(out s.camera2WorldMatrix) || !sample.TryGetProjectionMatrix(out s.projectionMatrix))
            return;

        sample.Dispose();

        camera2WorldMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(s.camera2WorldMatrix);
        projectionMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(s.projectionMatrix);

        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            pictureTexture.LoadRawTextureData(s.data);
            pictureTexture.Apply();

            Vector3 inverseNormal = -camera2WorldMatrix.GetColumn(2);
            // Position the canvas object slightly in front of the real world web camera.
            Vector3 imagePosition = camera2WorldMatrix.GetColumn(3) - camera2WorldMatrix.GetColumn(2);

#if XR_PLUGIN_WINDOWSMR || XR_PLUGIN_OPENXR

            Camera unityCamera = Camera.main;
            Matrix4x4 invertZScaleMatrix = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 localToWorldMatrix = camera2WorldMatrix * invertZScaleMatrix;
            unityCamera.transform.localPosition = localToWorldMatrix.GetColumn(3);
            unityCamera.transform.localRotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));
#endif
        }, false);
    }

     void Update()
    {

    }

    public Texture2D getImage()
    {
        return pictureTexture;
    }
}



