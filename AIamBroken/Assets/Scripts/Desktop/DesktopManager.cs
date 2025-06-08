using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    [SerializeField] private GameObject cliWindowPrefab;
    [SerializeField] private Transform canvasTransform;
    // Start is called before the first frame update
    public void OpenTerminalWindow()
    {
        if (cliWindowPrefab != null)
        {
            Instantiate(cliWindowPrefab, canvasTransform);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
