using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenZoomCam : MonoBehaviour
{
    public float zoomDuration = 1f;

    public void Focus(Transform startTrans, Transform endTrans)
    {
        transform.position = startTrans.position;
        transform.rotation = startTrans.rotation;

        transform.DOMove(endTrans.position, zoomDuration);
        transform.DORotate(endTrans.rotation.eulerAngles, zoomDuration);
    }
    public void Unfocus(Transform startTrans, Transform endTrans)
    {
        transform.DOMove(endTrans.position, zoomDuration);
        transform.DORotate(endTrans.rotation.eulerAngles, zoomDuration);
    }
}
