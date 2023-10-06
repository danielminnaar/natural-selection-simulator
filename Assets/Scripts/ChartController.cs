using UnityEngine;
using UnityEngine.UI;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
public class ChartController : MonoBehaviour
{
    public Image speedBar;
    public Image senseBar;
    public Image digestionBar;
    public Text speedText;
    public Text senseText;
    public Text digestText;
    private GameObject senseLine;
    private GameObject speedLine;
    private float topPadding = 60;

    // Call this method to update the bar heights
    public void UpdateBars(float speed, float sense, float slowDigestion)
    {
        SetBarHeight(speedBar, speed);
        speedText.text = "SPEED" + Environment.NewLine + speed.ToString("F1");
        SetBarHeight(senseBar, sense);
        senseText.text = "SENSE" + Environment.NewLine + sense.ToString("F1");
        SetBarHeight(digestionBar, slowDigestion);
        digestText.text = "DIGEST" + Environment.NewLine + slowDigestion.ToString("F1");
    }

    public void Reset()
    {
        UpdateBars(0, 0, 0);
    }

    public void UpdateBars(Dictionary<int, List<float>> simStats)
    {
        float totalSpeed = 0f;
        float totalSense = 0f;
        float totalDigest = 0f;
        foreach(var stat in simStats)
        {
            // 0 speed, 1 sense 2 digest
            totalSpeed += stat.Value[0];
            totalSense += stat.Value[1];
            totalDigest += stat.Value[2];
        }
        int totalGenerations = simStats.Last().Key;
        totalSpeed = totalSpeed / totalGenerations;
        totalSense = totalSense / totalGenerations;
        totalDigest = totalDigest / totalGenerations;
        UpdateBars(totalSpeed, totalSense, totalDigest);
    }

    private void SetBarHeight(Image bar, float value)
    {
        // Ensure value does not exceed 10.
        value = Mathf.Min(value, 10);

        // Calculate the relative height considering the screen bounds.
        float maxScreenBound = CalculateMaxBarHeightBasedOnScreen(bar);

        // Normalize the value (0 to 10 -> 0 to 1) and apply it to the max height.
        var size = bar.rectTransform.sizeDelta;
        size.y = (value / 10) * maxScreenBound;

        bar.rectTransform.sizeDelta = size;
    }

    private float CalculateMaxBarHeightBasedOnScreen(Image bar)
    {
        CanvasScaler canvasScaler = bar.canvas.GetComponent<CanvasScaler>();
        float maxScreenBound;

        if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            float ratio = canvasScaler.referenceResolution.y / Screen.height;
            maxScreenBound = Screen.height * ratio - topPadding;
        }
        else
        {
            maxScreenBound = Screen.height - topPadding;
        }

        return maxScreenBound;
    }
}