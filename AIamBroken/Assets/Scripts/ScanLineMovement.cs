using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScanLineMovement : MonoBehaviour
{
    public RawImage scanlineImage;
    public float moveAmount = 100f; // 移動幅
    public float moveDuration = 5f; // 移動にかける秒数

    void Start()
    {
        // anchoredPositionのYだけ上下にループする
        Vector2 originalPos = scanlineImage.rectTransform.anchoredPosition;

        scanlineImage.rectTransform.DOAnchorPosY(originalPos.y + moveAmount, moveDuration)
            .SetLoops(-1,LoopType.Restart)
            .SetEase(Ease.InOutSine);
    }

}
