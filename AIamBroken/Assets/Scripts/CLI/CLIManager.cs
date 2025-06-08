using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLIManager : MonoBehaviour
{
    [Header("UI Components")]
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
    [Header("Input Settings")]
    [SerializeField] private string promptSymbol = " > ";
    [SerializeField] private float cursorBlinkRate = 0.5f;
    private AudioSource audioSource;
    // --- 内部変数 ---
    private TextMeshProUGUI currentInputLineText; // 現在入力中の行のTextコンポーネント
    private string currentInputString = "";       // 現在入力中の文字列
    // 状態管理
    private enum CLIState { AwaitingInput, Processing }
    private CLIState currentState = CLIState.Processing; // 最初は入力不可

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioSource.clip);
        // 起動シーケンスを再生
        if (playSetupSequence)
        {
            StartCoroutine(RunSetupSequence());
        }
        else
        {
            // すぐに入力可能にする場合
            AddHistoryEntry(" > 初期完了. コマンドを入力してください...");
            CreateNewActiveLine();
            currentState = CLIState.AwaitingInput; // 入力待ち状態に変更
        }

        // カーソル点滅を開始
        StartCoroutine(CursorBlinkCoroutine());
    }
    void Update()
    {
        if (currentState == CLIState.AwaitingInput)
        {
            HandleDirectKeyInput();
        }
    }
    private void HandleDirectKeyInput()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (currentInputString.Length > 0)
                {
                    currentInputString = currentInputString.Substring(0, currentInputString.Length - 1);
                }
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                ProcessCommand(currentInputString);
                return; // 1フレームに1コマンド
            }
            else
            {
                currentInputString += c;
            }
        }
        // 入力中の行の表示を更新
        UpdateActiveLineDisplay();
    }

    // --- 起動シーケンス用のコルーチン ---
    private IEnumerator RunSetupSequence()
    {

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
        CreateNewActiveLine();
        currentState = CLIState.AwaitingInput; // 入力待ち状態に変更
    }

    private void ProcessCommand(string command)
    {
        // 状態を「処理中」にして、キー入力をブロック
        currentState = CLIState.Processing;

        // 現在の入力行を、コマンドとして確定表示
        UpdateActiveLineDisplay(); // isFinal = true
        currentInputString = ""; // 内部の入力文字列をクリア

        // コマンド処理と応答生成はコルーチンで行う
        StartCoroutine(ProcessCommandCoroutine(command));
    }
    private IEnumerator ProcessCommandCoroutine(string command)
    {
        // コマンドが空の場合はすぐに入力待ちに戻る
        if (string.IsNullOrWhiteSpace(command))
        {
            CreateNewActiveLine();
            currentState = CLIState.AwaitingInput;
            yield break; // コルーチンを終了
        }

        string response;
        // コマンドに応じた処理
        if (command == "--help window")
        {
            response = " > Available commands:\n > --help window: Display this help message.";
        }
        else
        {
            response = " > " + command + " はコマンドリストに存在しない不正な呼び出しです.";
        }

        yield return new WaitForSeconds(Random.Range(0.1f, 0.4f)); // 処理している感を出す
        AddHistoryEntry(response);
        
        // 次の入力行を準備
        CreateNewActiveLine();
        
        // 再び入力待ち状態に戻す
        currentState = CLIState.AwaitingInput;
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

    private IEnumerator ForceScrollDown()
    {
        yield return new WaitForEndOfFrame(); // Content Size FitterとLayout Groupの更新を待つ
        scrollRect.verticalNormalizedPosition = 0f; // 最下部にスクロール
    }
    // <<< ADDED: 新しいアクティブな入力行を作成するメソッド >>>
    private void CreateNewActiveLine()
    {
        GameObject newEntry = Instantiate(historyTextPrefab, historyContent);
        currentInputLineText = newEntry.GetComponent<TextMeshProUGUI>();
        UpdateActiveLineDisplay(); // プロンプト記号を表示
        StartCoroutine(ForceScrollDown());
    }

    // <<< ADDED: アクティブな行の表示を更新するメソッド >>>
    private void UpdateActiveLineDisplay()
    {
        if (currentInputLineText != null)
        {
            string textToShow = promptSymbol + currentInputString;
            currentInputLineText.text = textToShow;
        }
    }


    private IEnumerator CursorBlinkCoroutine()
    {
        string cursorSymbol = "_";
        while (true)
        {
            if (currentState == CLIState.AwaitingInput && currentInputLineText != null)
            {
                string baseText = promptSymbol + currentInputString;
                currentInputLineText.text = baseText + cursorSymbol; // カーソル表示
                yield return new WaitForSeconds(cursorBlinkRate);

                currentInputLineText.text = baseText; // カーソル非表示
                yield return new WaitForSeconds(cursorBlinkRate);
            }
            else
            {
                yield return null; // 入力待ちでなければ何もしない
            }
        }
    }
}
