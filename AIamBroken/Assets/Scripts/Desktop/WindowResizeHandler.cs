using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowResizeHandler : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform windowRectTransform;
    [SerializeField] private Vector2 minSize = new Vector2(320, 240);
    [SerializeField] private Vector2 maxSize = new Vector2(1280, 720);
    public void OnDrag(PointerEventData eventData)
    {
        //現在のサイズを取得
        Vector2 currentSize = windowRectTransform.sizeDelta;
        //マウスの移動量に応じてサイズを変更
        Vector2 newSize = currentSize + new Vector2(eventData.delta.x, -eventData.delta.y);
        newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
        newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);
        windowRectTransform.sizeDelta = newSize;
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
