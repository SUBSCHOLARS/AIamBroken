using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private RectTransform historyContent; // ScrollView内のContentのRectTransform
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

    private bool isCursorVisible = true; // カーソルの点滅状態
    private bool isInputActive = false; // 入力中かどうか
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioSource.clip);

        // InputFieldでEnterが押された時のイベントを登録
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

        // カーソル点滅を開始
        StartCoroutine(CursorBlink());
    }
    void Update()
    { 
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
        AddHistoryEntry(" > 初期完了. コマンドを入力してください...");

        // シーケンス完了後、入力を可能にする
        commandInput.interactable = true;
        RefocusInput();
    }

    private void OnCommandSubmit(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        // コマンド履歴を追加
        AddHistoryEntry("> " + command);

        // コマンドを処理する
        ProcessCommand(command);

        // 入力欄をクリアして再フォーカス
        RefocusInput();
    }

    private void ProcessCommand(string command)
    {
        string response = " > Command received: " + command;
        AddHistoryEntry(response);

        // 必要に応じてコマンドに応じた処理を追加
        if (command == "--help window")
        {
            AddHistoryEntry(" > Available commands:");
            AddHistoryEntry(" > --help window: Display this help message.");
            // 他のコマンドを追加
        }
        else
        {
            AddHistoryEntry(" > "+command+" はコマンドリストに存在しない不正な呼び出しです.");
        }
    }

    private void AddHistoryEntry(string text)
    {
        // テキストプレファブから新しいインスタンスを生成
        GameObject newEntry = Instantiate(historyTextPrefab, historyContent);
        newEntry.GetComponent<TextMeshProUGUI>().text = text;

        // Contentのサイズを更新
        LayoutRebuilder.ForceRebuildLayoutImmediate(historyContent);
        // RectTransformのサイズを強制的に更新
        historyContent.sizeDelta = new Vector2(historyContent.sizeDelta.x, historyContent.GetComponent<VerticalLayoutGroup>().preferredHeight);
        // デバッグログを追加
        Debug.Log($"History Content Size: {historyContent.sizeDelta}");
        Debug.Log($"Child Count: {historyContent.childCount}");
        // スクロールバーを最下部に移動
        StartCoroutine(ForceScrollDown());
    }

    private void RefocusInput()
    {
        commandInput.text = "";
        commandInput.ActivateInputField();
    }

    private IEnumerator ForceScrollDown()
    {
        yield return new WaitForEndOfFrame(); // Content Size FitterとLayout Groupの更新を待つ
        scrollRect.verticalNormalizedPosition = 0f; // 最下部にスクロール
    }

    private IEnumerator CursorBlink()
    {
        while (true)
        {
            isCursorVisible = !isCursorVisible;
            commandInput.placeholder.GetComponent<TextMeshProUGUI>().text = isCursorVisible ? "_" : ""; // カーソルの点滅
            yield return new WaitForSeconds(0.5f); // カーソルの点滅間隔
        }
    }
}
