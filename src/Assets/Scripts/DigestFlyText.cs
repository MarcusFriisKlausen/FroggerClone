using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigestFlyText : MonoBehaviour
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
            GameObject canvasObj = new GameObject("DigestFlyCanvas", typeof(Canvas));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject textObj = new GameObject("DigestFlyText", typeof(Text));
            textObj.transform.SetParent(canvasObj.transform);
            uiText = textObj.GetComponent<Text>();

            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.anchoredPosition = new Vector2(10, -36);
        }

        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.fontStyle = FontStyle.Bold;
        uiText.fontSize = 24;
        uiText.alignment = TextAnchor.MiddleLeft;
        uiText.color = Color.black;

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
                string baseText = "Digest Fly ";

                if (fliesEaten > 0)
                {
                    uiText.text = baseText + "<color=#80FF80>[E]</color>";
                }
                else
                {
                    uiText.text = baseText + "[E]";
                }
            }
        }
    }
}
