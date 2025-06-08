using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DoubleClickDetector : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("ダブルクリックと判定するクリック間の最大時間（秒）")]
    [SerializeField] private float doubleClickThreshold = 0.3f;
    [Tooltip("ダブルクリック時に呼び出すイベント")]
    public UnityEvent onDoubleClick;
    private float lastClickTime;
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= doubleClickThreshold)
        {
            //ダブルクリック成功
            onDoubleClick.Invoke();
            //閾値をリセットして3回連続クリックなどを防止
            lastClickTime = 0f;
        }
        else
        {
            //シングルクリックとして時間を記録
            lastClickTime = Time.time;
        }
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
