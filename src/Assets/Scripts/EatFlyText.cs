using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatFlyText : MonoBehaviour
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
            GameObject canvasObj = new GameObject("EatFlyCanvas", typeof(Canvas));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject textObj = new GameObject("EatFlyText", typeof(Text));
            textObj.transform.SetParent(canvasObj.transform);
            uiText = textObj.GetComponent<Text>();

            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(1, 0);
            rectTransform.anchoredPosition = new Vector2(-20, -36);
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
                bool flyInRange = frogScript.flyInRange;
                string baseText = "Eat Fly ";

                if (fliesEaten < 2 && flyInRange)
                {
                    uiText.text = baseText + "<color=#80FF80>[F]</color>";
                }
                else if (fliesEaten > 1)
                {
                    uiText.text = baseText + "<color=red>[F]</color>";
                }
                else
                {
                    uiText.text = baseText + "<color=black>[F]</color>";
                }
            }
        }
    }
}
