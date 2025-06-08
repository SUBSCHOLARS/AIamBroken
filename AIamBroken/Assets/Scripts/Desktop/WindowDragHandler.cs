using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform windowRectTransform;
    private Canvas canvas;

    private void Awake()
    {
        //親Canvasの参照を取得
        canvas = GetComponentInParent<Canvas>();   
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //クリックされたウィンドウを一番手前に表示
        windowRectTransform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //マウスの移動量に応じてウィンドウを動かす
        windowRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
