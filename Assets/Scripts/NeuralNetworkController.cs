using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using System;
using System.Linq;
using System.Collections;


public class BoundingBox
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
}

public class DetectionResult
{
    public BoundingBox Bbox { get; set; }
    public string Label { get; set; }
    public int LabelIdx { get; set; }
    public float Confidence { get; set; }

    public Rect Rect
    {
        get { return new Rect(Bbox.X, Bbox.Y, Bbox.Width, Bbox.Height); }
    }

    public override string ToString()
    {
        return $"{Label}:{Confidence}";
    }
}

public class NeuralNetworkController : MonoBehaviour
{
    public CameraController camCtrl;

    public int inferenceSize = 640;
    public float confidenceThreshold = 0.5f;

    public ModelAsset model_0;
    public ModelAsset model_1;
    public ModelAsset model_2;
    public ModelAsset model_3;
    public ModelAsset model_4;

    private Model runtimeModel;
    private IWorker worker;

    private ITensorAllocator allocator;
    private Ops ops;

    private TextureTransform textureTransform;
    private Texture2D croppedTexture;

    List<DetectionResult> boxes;
    public int lastModelId;

    private ExperimentHandler experimentHandler;

    void Start()
    {
        lastModelId = 0;
        runtimeModel = ModelLoader.Load(model_0);
        worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel);

        allocator = new TensorCachingAllocator();
        ops = WorkerFactory.CreateOps(BackendType.CPU, allocator);

        textureTransform = new TextureTransform();
        textureTransform.SetCoordOrigin(CoordOrigin.BottomLeft);

        croppedTexture = new Texture2D(inferenceSize, inferenceSize, TextureFormat.RGB24, false);

        boxes = new List<DetectionResult>();

        experimentHandler = new ExperimentHandler();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LogMessage(string data)
    {
        experimentHandler.LogMessage(data);
        Debug.Log($"Logged: {data}");
    }


    private Color[] CropTexture(Texture2D sourceTexture, int cropWidth, int cropHeight)
    {
        int centerX = sourceTexture.width / 2 - cropWidth / 2;
        int centerY = sourceTexture.height / 2 - cropHeight / 2;
        Color[] pixels = sourceTexture.GetPixels(centerX, centerY, cropWidth, cropHeight);

        return pixels;
    }

    private void ParseYoloOutput(TensorFloat tensor, float confidenceThreshold, List<DetectionResult> boxes)
    {
        int nClasses = tensor.shape[1] - 4;
        for (int batch = 0; batch < tensor.shape[0]; batch++)
        {
            for (int i = 0; i < tensor.shape[2]; i++)
            {
                var (label, confidence) = GetClassIdx(tensor, i, batch, nClasses);
                if (confidence < confidenceThreshold)
                {
                    continue;
                }
                BoundingBox box = ExtractBoundingBox(tensor, i, batch);
                boxes.Add(new DetectionResult
                {
                    Bbox = box,
                    Confidence = confidence,
                    Label = label.ToString(),
                    LabelIdx = label
                });

            }
        }
    }

    private BoundingBox ExtractBoundingBox(TensorFloat tensor, int row, int batch)
    {
        return new BoundingBox
        {
            X = tensor[batch, 0, row],
            Y = tensor[batch, 1, row],
            Width = tensor[batch, 2, row],
            Height = tensor[batch, 3, row]
        };

    }

    private ValueTuple<int, float> GetClassIdx(TensorFloat tensor, int row, int batch, int numClasses)
    {
        int classIdx = 0;
        float maxConf = tensor[batch, 4, row];
        for (int i = 0; i < numClasses; i++)
        {
            if (tensor[batch, 4 + i, row] > maxConf)
            {
                maxConf = tensor[batch, 4 + i, row];
                classIdx = i;
            }
        }

        return (classIdx, maxConf);
    }

    private List<DetectionResult> NonMaxSuppression(float threshold, List<DetectionResult> boxes)
    {
        var results = new List<DetectionResult>();
        if (boxes.Count == 0)
        {
            return results;
        }
        var detections = boxes.OrderByDescending(b => b.Confidence).ToList();
        results.Add(detections[0]);

        for (int i = 1; i < detections.Count; i++)
        {
            bool add = true;
            for (int j = 0; j < results.Count; j++)
            {
                float iou = IoU(detections[i].Rect, results[j].Rect);
                if (iou > threshold)
                {
                    add = false;
                    break;
                }
            }
            if (add)
                results.Add(detections[i]);
        }

        return results;
    }

    private float IoU(Rect boundingBoxA, Rect boundingBoxB)
    {
        float intersectionArea = Mathf.Max(0, Mathf.Min(boundingBoxA.xMax, boundingBoxB.xMax) - Mathf.Max(boundingBoxA.xMin, boundingBoxB.xMin)) *
                        Mathf.Max(0, Mathf.Min(boundingBoxA.yMax, boundingBoxB.yMax) - Mathf.Max(boundingBoxA.yMin, boundingBoxB.yMin));

        float unionArea = boundingBoxA.width * boundingBoxA.height + boundingBoxB.width * boundingBoxB.height - intersectionArea;

        if (unionArea == 0)
        {
            return 0;
        }

        return intersectionArea / unionArea;
    }

    public IEnumerator Wait(float duration)
    {
        Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        Debug.Log($"Ended at {Time.time}");
    }

    public int Validate()
    {
        List<DetectionResult> boxes_tmp = new List<DetectionResult>();
        if (camCtrl.pictureTexture)
        {
            var currentImage = camCtrl.pictureTexture;
            var crop = CropTexture(currentImage, inferenceSize, inferenceSize);
            croppedTexture.SetPixels(crop);
            croppedTexture.Apply();
            var tensor = TextureConverter.ToTensor(croppedTexture, textureTransform);

            worker.Execute(tensor);
            TensorFloat result = worker.PeekOutput() as TensorFloat;

            result.TakeOwnership();

            boxes_tmp.Clear();
            boxes.Clear();

            ParseYoloOutput(result, confidenceThreshold, boxes_tmp);
            boxes = NonMaxSuppression(0.6f, boxes_tmp);

            string dets = $"";

            foreach (var l in boxes)
            {
                dets += $"{l.Label}:{l.Confidence.ToString("0.00")}\n";
            }
            Debug.Log(dets);

            if (boxes.Count > 0)
            {
                Debug.Log($"Detected: {boxes[0].LabelIdx}");
                return boxes[0].LabelIdx;
            }
            else
            {
                Debug.Log("No detection!");
            }
        }
        return -1;
    }

    public void ReloadModel(int modelIndex)
    {
        if (modelIndex != lastModelId)
        {
            switch (modelIndex)
            {
                case 0:
                    runtimeModel = ModelLoader.Load(model_0);
                    lastModelId = modelIndex;
                    Debug.Log("Model 0");
                    break;
                case 1:
                    runtimeModel = ModelLoader.Load(model_1);
                    lastModelId = modelIndex;
                    Debug.Log("Model 1");
                    break;
                case 2:
                    runtimeModel = ModelLoader.Load(model_2);
                    lastModelId = modelIndex;
                    Debug.Log("Model 2");
                    break;
                case 3:
                    runtimeModel = ModelLoader.Load(model_3);
                    lastModelId = modelIndex;
                    Debug.Log("Model 3");
                    break;
                case 4:
                    runtimeModel = ModelLoader.Load(model_4);
                    lastModelId = modelIndex;
                    Debug.Log("Model 4");
                    break;
                default:
                    runtimeModel = ModelLoader.Load(model_0);
                    lastModelId = 0;
                    break;
            }

            worker.Dispose();
            worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel);
        }
    }

    public void LogPrediction()
    {

    }

}
