using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragTouchScroll : MonoBehaviour, IDragHandler {

	ScrollRect scroll;

	void Start(){
		scroll = this.GetComponent<ScrollRect> ();
	}

	public void OnDrag(PointerEventData data)
	{
		scroll.verticalNormalizedPosition -= data.delta.y * 0.001f;
	}

}
