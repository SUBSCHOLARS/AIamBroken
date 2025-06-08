using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private RectTransform historyContent;//ScrollView内のContentのRectTransform
    [SerializeField] private GameObject historyTextPrefab;
    [SerializeField] private ScrollRect scrollRect;
    // Start is called before the first frame update
    void Start()
    {
        //InputFieldでEnterが押された時のイベントを登録
        commandInput.onEndEdit.AddListener(OnCommandSubmit);
        
    }
    private void OnCommandSubmit(string command)
    {
        if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;

        if (string.IsNullOrWhiteSpace(command))
        {
            //入力欄を再フォーカス
            RefocusInput();
            return;
        }
        //コマンド履歴を追加
        AddHistoryEntry("> " + command);
        //コマンドを処理する
        ProcessCommand(command);
        //入力欄をクリアして再フォーカス
        RefocusInput();
    }
    private void ProcessCommand(string command)
    {
        string response = "Command received: " + command;
        //..コマンドに応じた処理
        AddHistoryEntry(response);
    }
    private void AddHistoryEntry(string text)
    {
        //テキストプレファブから新しいインスタンスを生成
        GameObject newEntry = Instantiate(historyTextPrefab, historyContent);
        newEntry.GetComponent<TextMeshProUGUI>().text = text;
        //スクロールバーを最下部に移動
        StartCoroutine(ForceScrollDown());
    }
    private void RefocusInput()
    {
        commandInput.text = "";
        commandInput.ActivateInputField();
    }
    IEnumerator ForceScrollDown()
    {
        //Content Size FitterとLayout Groupの更新を待つ
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f; //最下部にスクロール
    }
    // Update is called once per frame
    void Update()
    {

    }
}
