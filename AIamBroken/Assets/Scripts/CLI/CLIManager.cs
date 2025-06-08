using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private RectTransform historyContent;//ScrollView内のContentのRectTransform
    [SerializeField] private GameObject historyTextPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Initial Setup Sequence")]
    [Tooltip("起動時にセットアップシーケンスを再生するか")]
    [SerializeField] private bool playSetupSequence = true;
    
    [Tooltip("起動シーケンスで表示するログの行")]
    [TextArea(5, 15)] // Inspectorで複数行入力しやすくするための属性
    [SerializeField] private string[] setupLogLines;
    
    [Tooltip("各行を表示する最小の遅延時間")]
    [SerializeField] private float minDelay = 0.1f;
    
    [Tooltip("各行を表示する最大の遅延時間")]
    [SerializeField] private float maxDelay = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        //InputFieldでEnterが押された時のイベントを登録
        commandInput.onEndEdit.AddListener(OnCommandSubmit);
         // 起動シーケンスを再生
        if (playSetupSequence)
        {
            StartCoroutine(RunSetupSequence());
        }
        else
        {
            // すぐに入力可能にする場合
            commandInput.interactable = true;
            RefocusInput();
        }
        
    }
    // --- 起動シーケンス用のコルーチン ---
    private IEnumerator RunSetupSequence()
    {
        // シーケンス中は入力を不可にする
        commandInput.interactable = false;

        // 定義されたログを一行ずつ、ランダムな間隔をあけて表示
        foreach (string line in setupLogLines)
        {
            AddHistoryEntry(line);
            float randomDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomDelay);
        }

        // 完了メッセージを追加
        AddHistoryEntry("\nInitialization complete. Awaiting command...");

        // シーケンス完了後、入力を可能にする
        commandInput.interactable = true;
        RefocusInput();
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
