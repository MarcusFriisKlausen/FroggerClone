using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    public Text uiText;

    private void Start()
    {
        if (uiText == null)
        {
            GameObject canvasObj = new GameObject("TimeCanvas", typeof(Canvas));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject textObj = new GameObject("TimeText", typeof(Text));
            textObj.transform.SetParent(canvasObj.transform);
            uiText = textObj.GetComponent<Text>();

            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, 0);
        }

        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.fontStyle = FontStyle.Bold;
        uiText.fontSize = 24;
        uiText.alignment = TextAnchor.UpperRight;
        uiText.color = Color.green;

        // Prevent text wrapping
        uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
        uiText.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private void Update()
    {
        float timeSinceLoad = Time.timeSinceLevelLoad;

        if (timeSinceLoad >= 40f)
        {
            uiText.color = Color.red;
        }
        else if (timeSinceLoad >= 20f)
        {
            uiText.color = Color.yellow;
        }
        else
        {
            uiText.color = Color.green;
        }

        uiText.text = timeSinceLoad.ToString("F2");
    }
}
