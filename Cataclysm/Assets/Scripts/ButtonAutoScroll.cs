using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class ButtonAutoScroll : MonoBehaviour,ISelectHandler
{
    [SerializeField]
    private ScrollRect          scrollRect;
    private Button[]            buttons;

    public void Start()
    {
        scrollRect        = GetComponent<ScrollRect>();
        buttons           = GetComponentsInChildren<Button>();
    }

	public void OnSelect(BaseEventData eventData)
	{
		for (int idx=0;idx<buttons.Length;idx++)
		{
			if (eventData.selectedObject==buttons[idx].gameObject)
			{
				scrollRect.verticalNormalizedPosition = 1f - ((float)idx / (buttons.Length - 1));
			}
		}
	}

}
