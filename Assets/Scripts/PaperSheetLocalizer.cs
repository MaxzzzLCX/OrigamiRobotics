using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.UnityUtils;
using HoloLensWithOpenCVForUnityExample;
using UnityEngine.UIElements;
using System.Linq;

public class PaperSheetLocalizer : MonoBehaviour
{

    public CameraController cameraController;
    public ColorObject objOfInterest = new ColorObject("white");
    public Material laserMaterial;

    private List<Tuple<GameObject, Renderer>> drawings;
    private Texture2D tex;
    private GameObject picturePreview;

    public int samples = 5;

    public int MIN_OBJECT_AREA = 10000;
    public int MAX_OBJECT_AREA = 1000000;
    private Mat bgraMat;
    private Mat rgbMat;
    private Mat thresholdMat;
    private Mat hsvMat;

    private int tick = 0;


    // Start is called before the first frame update
    void Start()
    {
        drawings = new List<Tuple<GameObject, Renderer>>();

        bgraMat = new Mat(cameraController.resolutionHeight, cameraController.resolutionWidth, CvType.CV_8UC4);
        rgbMat = new Mat(cameraController.resolutionHeight, cameraController.resolutionWidth, CvType.CV_8UC3);
        thresholdMat = new Mat();
        hsvMat = new Mat();

       tex = new Texture2D(cameraController.resolutionWidth, cameraController.resolutionHeight, TextureFormat.RGBA32, false);
      /*picturePreview = GameObject.CreatePrimitive(PrimitiveType.Quad);
        picturePreview.GetComponent<MeshRenderer>().material.mainTexture = tex;
        picturePreview.transform.localScale = new Vector3(1.5F, 1.0F, 1.0F);
        picturePreview.GetComponent<Collider>().enabled = false;

        picturePreview.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f + Camera.main.transform.right * .1f; */
    }

    // Update is called once per frame
    void Update()
    {
        /*if (tick == 90)
        {
            DetectPapersheet();
            tick = 0;
            Debug.Log(cameraController.pictureTexture.updateCount);
        }
        tick++;*/
     /*  picturePreview.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f + Camera.main.transform.right * .1f;
        picturePreview.transform.rotation = Camera.main.transform.rotation; */


    }

    private RaycastHit shootLaserRaycastHit(Vector3 from, Vector3 direction, float length, Material mat = null)
    {
        Ray ray = new Ray(from, direction);
        Vector3 to = from + length * direction;

        // Use this code when hit on mesh surface
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, length))
            to = hit.point;

        return hit;
    }

    private List<Vector3> CalculatePoints(Point[] points)
    {
        var cam2World = cameraController.camera2WorldMatrix;
        var projectionMatrix = cameraController.projectionMatrix;

        var res = new HoloLensCameraStream.Resolution(cameraController.resolutionWidth, cameraController.resolutionHeight);

        Vector3 corner_0 = LocatableCameraUtils.PixelCoordToWorldCoord(cam2World, projectionMatrix, res, new Vector2((float)points[0].x, (float)points[0].y));
        Vector3 corner_1 = LocatableCameraUtils.PixelCoordToWorldCoord(cam2World, projectionMatrix, res, new Vector2((float)points[1].x, (float)points[1].y));
        Vector3 corner_2 = LocatableCameraUtils.PixelCoordToWorldCoord(cam2World, projectionMatrix, res, new Vector2((float)points[2].x, (float)points[2].y));
        Vector3 corner_3 = LocatableCameraUtils.PixelCoordToWorldCoord(cam2World, projectionMatrix, res, new Vector2((float)points[3].x, (float)points[3].y));

        var point_0 = shootLaserFrom(cam2World.GetColumn(3), corner_0, 15f);
        var point_1 = shootLaserFrom(cam2World.GetColumn(3), corner_1, 15f);
        var point_2 = shootLaserFrom(cam2World.GetColumn(3), corner_2, 15f);
        var point_3 = shootLaserFrom(cam2World.GetColumn(3), corner_3, 15f);

        return new List<Vector3>() { point_0, point_1, point_2, point_3 };
    }

    private Vector3 shootLaserFrom(Vector3 from, Vector3 direction, float length, Material mat = null)
    {
        Ray ray = new Ray(from, direction);
        Vector3 to = from + length * direction;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, length))
        {
            to = hit.point;
        }

        return to;
    }

    private List<Vector3> GetPaperSheet(Texture2D image) 
    {
        Utils.fastTexture2DToMat(image, bgraMat, false);
        
        Mat rgbMatCp = rgbMat.clone();
        Imgproc.cvtColor(bgraMat, rgbMatCp, Imgproc.COLOR_BGRA2RGB);
        
        Imgproc.cvtColor(rgbMatCp, hsvMat, Imgproc.COLOR_RGB2HLS);
        Core.inRange(hsvMat, new Scalar(0, (int)160, 0), new Scalar(180, 255, 255), thresholdMat);

        morphOps(thresholdMat);
        var pts = trackFilteredObject(objOfInterest, thresholdMat, hsvMat, rgbMatCp);

        Mat tmp = new Mat();
        Imgproc.cvtColor(rgbMatCp, tmp, Imgproc.COLOR_RGB2RGBA);

        Utils.matToTexture2D(tmp, tex);

        if(pts != null)
        {
            return CalculatePoints(pts);
        }
        else
        {
            return null;
        }
    }

    private Point[] trackFilteredObject(ColorObject theColorObject, Mat threshold, Mat HSV, Mat cameraFeed)
    {

        List<ColorObject> colorObjects = new List<ColorObject>();
        Mat temp = new Mat();
        threshold.copyTo(temp);
        //these two vectors needed for output of findContours
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();
        //find contours of filtered image using openCV findContours function
        Imgproc.findContours(temp, contours, hierarchy, Imgproc.RETR_CCOMP, Imgproc.CHAIN_APPROX_SIMPLE);
        List<Point[]> points = new();
        
        var biggestArea = 0.0;
        var biggestAreaId = -1;

        if (contours.Count > 0)
        {
            int numObjects = contours.Count;

            if (numObjects < 50)
            {

                for (int index = 0; index < numObjects; index++)
                {

                    Moments moment = Imgproc.moments(contours[index]);

                    MatOfPoint2f contour2f = new MatOfPoint2f(contours[index].toArray());
                    RotatedRect minRect = Imgproc.minAreaRect(contour2f);

                    double area = moment.get_m00();

                    var ratio = minRect.size.width / minRect.size.height;
                    // && (0.8 < ratio) && (ratio < 1.2)

                    if ((area > MIN_OBJECT_AREA) && (area < MAX_OBJECT_AREA) && (0.6 < ratio) && (ratio < 1.4))
                    {
                        ColorObject colorObject = new ColorObject();

                        colorObject.setXPos((int)(moment.get_m10() / area));
                        colorObject.setYPos((int)(moment.get_m01() / area));
                        colorObject.setType(theColorObject.getType());
                        colorObject.setColor(theColorObject.getColor());

                        colorObjects.Add(colorObject);

                        Point[] pts = new Point[4];
                        minRect.points(pts);

                        MatOfPoint m = new MatOfPoint(pts);

                        Imgproc.fillConvexPoly(cameraFeed, m, new Scalar(0, 0, 255));

                        points.Add(pts);

                        if (area > biggestArea)
                        {
                            biggestArea = area;
                            biggestAreaId = points.Count - 1;
                        }
                        Debug.Log($"Area: {area}");

                    }
                }
            }
        }
        if(biggestAreaId > -1)
        {
            return points[biggestAreaId];

        }
        return null;
    }

    private void morphOps(Mat thresh)
    {
        //create structuring element that will be used to "dilate" and "erode" image.
        //the element chosen here is a 3px by 3px rectangle
        Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        //dilate with larger element so make sure object is nicely visible
        Mat dilateElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));

        Imgproc.erode(thresh, thresh, erodeElement);
        Imgproc.erode(thresh, thresh, erodeElement);

        Imgproc.dilate(thresh, thresh, dilateElement);
        Imgproc.dilate(thresh, thresh, dilateElement);
    }

    public List<Vector3> DetectPapersheet()
    {
        var s = new List<Vector3>() { Vector3.zero, Vector3.zero , Vector3.zero , Vector3.zero };
        var lastImg = cameraController.pictureTexture.GetPixels();
        for (var i = 0; i < samples; i++)
        {
            var img = cameraController.pictureTexture;

            if ((img is not null) && img.GetPixels() != lastImg)
            {
                try
                {
                    var paper = GetPaperSheet(img);
                    for (var j = 0; j < 4; j++)
                    {
                        s[j] += paper[j];
                    }
                    lastImg = img.GetPixels();
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                Debug.Log("Same picture");
            }
        }
        for (var j = 0; j < 4; j++)
        {
            s[j] /= samples;
        }

        Debug.Log($"Papersheet: {s[0]}, {s[1]}, {s[2]}, {s[3]}");
        bool anyZeroVector = s.Any(p => p == Vector3.zero);

        if (anyZeroVector)
        {
            Debug.Log("Zeroes!");
            return null;
        }

        return s;

    }
}
