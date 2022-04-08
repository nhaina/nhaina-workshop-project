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
	[SerializeField]
	private GameObject subPanel;
	private LayoutGroup subPanelLayoutGroup;

	private void Awake()
	{
		subPanelLayoutGroup = subPanel.GetComponent<LayoutGroup>();
		if (subPanelLayoutGroup == null)
		{
			Debug.LogError("SubpanelLayoutGoup is null!");
		}
	}

	private void OnEnable()
	{
		RectTransform rectTransform = (RectTransform)transform;
		float childrenHeight = 0;
		foreach (Transform child in subPanel.transform)
		{
			childrenHeight += RectTransformUtility.CalculateRelativeRectTransformBounds(child).max.y + k_Padding;
		}

		// rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0.5f * childrenHeight + k_Padding);
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, childrenHeight + 2 * k_Padding);
		subPanelLayoutGroup.enabled = true;
	}

	private void OnDisable()
	{
		ClearChildren();
		if (subPanelLayoutGroup != null)
		{
			subPanelLayoutGroup.enabled = false;
		}
		else
		{
			Debug.Log(subPanel);
			Debug.Log(subPanelLayoutGroup);
			Debug.Log(transform.childCount);
		}
	}

	public void SetTitle(string i_Title)
	{
		titleLabelText.text = i_Title;
	}

	public void AddChild(GameObject i_Child)
	{
		i_Child.transform.SetParent(subPanel.transform, false);
	}

	public void ClearChildren()
	{
		Transform[] children = subPanel.transform.Cast<Transform>().ToArray();
		foreach (Transform child in children)
		{
#if UNITY_EDITOR
			DestroyImmediate(child.gameObject);
#else
			Destroy(child.gameObject);
#endif
		}
	}
}
