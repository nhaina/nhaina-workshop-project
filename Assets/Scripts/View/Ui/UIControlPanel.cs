using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIControlPanel : MonoBehaviour
{
	private const float k_Padding = 7;
	[SerializeField]
	private Text titleLabelText;

	private void OnEnable()
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

	public void SetTitle(string i_Title)
	{
		titleLabelText.text = i_Title;
	}

#if UNITY_EDITOR
	public void ClearChildren()
	{
		Transform[] children = transform.Cast<Transform>().ToArray();
		foreach (Transform child in children)
		{
			if (child.GetInstanceID() != titleLabelText.transform.GetInstanceID())
			{
				DestroyImmediate(child.gameObject);
			}
		}
	}
#else
		public void ClearChildren()
		{
			Transform[] children = transform.Cast<Transform>().ToArray();
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}
#endif
}
