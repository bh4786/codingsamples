using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenZoomCam : MonoBehaviour
{
    public float zoomDuration = 1f;

    public void Focus(Transform startTrans, Transform endTrans)
    {
        //set zoom camera to player camera position
        transform.position = startTrans.position;
        transform.rotation = startTrans.rotation;

        //tween camera to screen offset value
        transform.DOMove(endTrans.position, zoomDuration);
        transform.DORotate(endTrans.rotation.eulerAngles, zoomDuration);
    }
    public void Unfocus(Transform startTrans, Transform endTrans)
    {
        //tween zoom camera back to player camera position
        transform.DOMove(endTrans.position, zoomDuration);
        transform.DORotate(endTrans.rotation.eulerAngles, zoomDuration);
    }
}
