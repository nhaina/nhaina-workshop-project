using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiControlPanel : MonoBehaviour
{
	private const float k_Padding = 7;

	private void Start()
	{
		RectTransform rectTransform = (RectTransform)transform;
		float childrenHeight = 0;
		foreach (Transform child in transform)
		{
			childrenHeight += RectTransformUtility.CalculateRelativeRectTransformBounds(child).max.y + k_Padding;
		}

		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, childrenHeight + 2 * k_Padding);
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, childrenHeight + 2 * k_Padding);
	}
}
