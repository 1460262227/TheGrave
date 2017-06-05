using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HandleClickOnBlock : MonoBehaviour, IPointerClickHandler {

    public Ground ScreenPos2BlockPos = null;
    public Action<int, int> OnClickGorund = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        int bx;
        int by;
        var pt = eventData.position;
        if (ScreenPos2BlockPos.ToBlockPos(pt.x, pt.y, out bx, out by))
            OnClickGorund.SC(bx, by);
    }
}
