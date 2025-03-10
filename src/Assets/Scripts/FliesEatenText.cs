using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FliesEatenText : MonoBehaviour
{
    public GameObject frog;
    public Text uiText;

    private void Start()
    {
        if (frog == null)
        {
            frog = GameObject.Find("Frog");
        }

        if (uiText == null)
        {
            GameObject canvasObj = new GameObject("FliesEatenCanvas", typeof(Canvas));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject textObj = new GameObject("FliesEatenText", typeof(Text));
            textObj.transform.SetParent(canvasObj.transform);
            uiText = textObj.GetComponent<Text>();

            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, 0);
        }

        // Set default text properties
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.fontStyle = FontStyle.Bold;
        uiText.fontSize = 24;
        uiText.alignment = TextAnchor.UpperLeft;
        uiText.color = Color.black;

        // Prevent text wrapping
        uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
        uiText.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private void Update()
    {
        if (frog != null)
        {
            var frogScript = frog.GetComponent<Frog>();
            if (frogScript != null)
            {
                int fliesEaten = frogScript.fliesEaten;
                string baseText = "Flies eaten: ";

                // Determine the color for the flies eaten value
                string fliesEatenText = fliesEaten.ToString();
                string color = "";

                if (fliesEaten == 0)
                {
                    color = "<color=#80FF80>";
                }
                else if (fliesEaten == 1)
                {
                    color = "<color=yellow>";
                }
                else if (fliesEaten == 2)
                {
                    color = "<color=red>";
                }

                uiText.text = baseText + color + fliesEatenText + "</color>";
            }
        }
    }
}
