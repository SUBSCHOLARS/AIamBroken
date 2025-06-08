using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TerminalDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private ScrollRect scrollRect;
    private StringBuilder logBuilder=new StringBuilder();
    string baseText = "> Enter your user name to be verified: ";
    private string userInput = ""; // プレイヤーの入力を格納
    private string commandInput="";
    private bool isInputActive = false; // 入力中かどうか
    private bool isUserNameVerified=false;
    private bool isCursorVisible = true; // カーソルの点滅状態
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartupSequence());
        audioSource=GetComponent<AudioSource>();
    }
    IEnumerator StartupSequence(){
        for (int i = 0; i < 31; i++)
        {
            AppendLine($">[LOG] Subsystem check {i:D2} -> <color=green>OK</color>");
            if (i % 5 == 0) yield return new WaitForSeconds(Random.Range(0.1f,1.0f)); // 一部yieldして負荷を分散
        }
        string[] startupLines=new string[]
        {
            "> Booting base system...",
            "> Establishing network connection",
            "> Initializing AI core...",
            "> Target AI: <color=yellow>EVE</color>",
            "> Checking memory modules...",
            "> Memory -> <color=green>OK</color>",
            "> Status: <color=red>critical ERROR detected</color>",
            "> System Broken Exception -> fallback",
            "> Launching manual recovery interface",
            "> SYSTEM <color=green>ONLINE</color>",
            "",
            "> Please wait...",
            ""
        };
        foreach(var line in startupLines){
            AppendLine(line);
            yield return new WaitForSeconds(Random.Range(0.1f,1.0f));
        }
        // プレイヤー入力を開始
        StartCoroutine(HandleUserInput());
    }
    private IEnumerator HandleUserInput()
    {
        isInputActive = true;
        StartCoroutine(CursorBlink());

        while (isInputActive)
        {
            // 入力を受け付ける
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // バックスペース
                {
                    if (userInput.Length > 0)
                        userInput = userInput.Substring(0, userInput.Length - 1);
                }
                else if (userInput!=""&&c == '\n' || c == '\r') // Enterキー
                {
                    isInputActive = false;
                    AppendLine(baseText + userInput); // 入力を確定して表示
                    AppendLine("> Fetching "+userInput+"...");
                    yield return new WaitForSeconds(Random.Range(0.5f,2.0f));
                    AppendLine("> User verified. Welcome!");
                    //userInput="";
                    isInputActive=false;
                    isUserNameVerified=true;
                    yield return new WaitForSeconds(Random.Range(0.2f,1.0f));
                    AppendLine("> Press Enter to Continue...");
                    
                    while (true) // 無限ループでキー入力を待機
                    {
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) // EnterキーまたはテンキーのEnterキー
                        {
                            StartCoroutine(DisplayAsciiArtAndMessage()); // コルーチンを開始
                            break; // ループを抜ける
                        }
                        yield return null; // 次のフレームまで待機
                    }
                }
                else if (userInput!=""&&c == '\n' || c == '\r'){
                    if(userInput=="-- help window"){

                    }
                    else{
                        AppendLine("Invalid command");
                    }
                }
                else
                {
                    userInput += c; // 入力文字を追加
                }
            }

            //表示を更新
            if(!isUserNameVerified){
            terminalText.text = logBuilder.ToString() + baseText + userInput + (isCursorVisible ? "_" : "");
            }
            yield return null;
        }
    }
    private IEnumerator HandleUserCommand(){
        isInputActive=true;
        StartCoroutine(CursorBlink());
        while(isInputActive){
            foreach(char c in Input.inputString){
                if(c=='\b'){
                    if (userInput.Length > 0)
                        userInput = userInput.Substring(0, userInput.Length - 1);
                }
                else{
                    commandInput+=c;
                }
            }
        }
        yield return null;
    }

    private IEnumerator CursorBlink()
    {
        while (isInputActive)
        {
            isCursorVisible = !isCursorVisible;
            yield return new WaitForSeconds(0.5f); // カーソルの点滅間隔
        }
    }
    private void AppendLine(string text){

        logBuilder.AppendLine(text);
        terminalText.text=logBuilder.ToString();

        Canvas.ForceUpdateCanvases();
        // 自動で一番下までスクロール
        scrollRect.verticalNormalizedPosition = 0f;
    }
    private IEnumerator DisplayAsciiArtAndMessage()
    {
        // StringBuilderをクリア
        logBuilder.Clear();
        terminalText.text = "";

        // 言語切り替えの演出
        AppendLine("> Obtaining GPS Info...");
        yield return new WaitForSeconds(1.5f);
        AppendLine("> Location: JPN");
        yield return new WaitForSeconds(1.0f);
        AppendLine("> 言語を日本語に切り替えています...");
        yield return new WaitForSeconds(1.5f);
        AppendLine("> ログを日本語に変換中...");
        yield return new WaitForSeconds(1.0f);

        // アスキーアートの表示
        string[] asciiArt = new string[]
        {
"--                                           ##       ##                                ###",
"--                                                     ##                               ##",
"--    ####    ##  ##   #####     ###      #####    ####     ####     ##",
"--   ##  ##   #######  ##  ##     ##       ##     ##  ##   ##  ##    #####",
"--   ##  ##   ## # ##  ##  ##     ##       ##     ######   ##        ##  ##",
"--   ##  ##   ##   ##  ##  ##     ##       ## ##  ##    ##  ##       ##  ##",
"--    ####    ##   ##  ##  ##    ####       ###    #####    ####    ###  ##",
"         OMNITECH - \"Your Future, Your Control\"                  ",
"",
"                          ###########                           ",
"                       ####             ####                        ",
"                    ####                   ####                     ",
"                 ####      ##     ##      ####                  ",
"                ###        ##       ##        ###                ",
"                 ####      ##     ##      ####                  ",
"                    ####                  ####                     ",
"                       ####            ####                        ",
"                          ###########                           ",
""
    };
    audioSource.PlayOneShot(audioSource.clip);
    foreach (string line in asciiArt)
    {
        AppendLine(line);
        yield return new WaitForSeconds(0.15f);
    }

    // 書置きの表示
    string[] message = new string[]
    {
        "--システムメッセージ:",
        "--HELLO FROM OMNI SYSTEM!:",
        "--タスクが割り当てされた"+userInput+"様へ:",
        "--旧式のAIシステム『EVE』に重大なエラーが発生しました:",
        "--既存の修復プログラムは機能しませんでした:",
        "--なので、プログラムを構築することがあなたの主なタスクとなります:",
        "--なお、修復に失敗した場合、責任は全て"+userInput+"様にあります:",
        "--コマンドラインインターフェースを使用して:",
        "--必要なコマンドを実行してください:",
        "--『--help window』コマンドを実行すると、コマンドのチートシートを含む更なる情報が表示できます:",
        "--ただし、それらが役に立つかどうかは保証しません:",
        "--迅速に対応してください。時間は有限です:",
        "--工数が不確定なため、この時間外タスクに支払われる給与は実際の時間に関わらず:",
        "--2時間分とさせていただきます:",
        "--以上。--ADAM POWERD BY OMNISYSTEM 2077/04/31/00:00:",
        ""
    };
    foreach (string line in message)
    {
        AppendLine(line);
        yield return new WaitForSeconds(0.5f);
    }

    // ユーザー操作待機
    AppendLine("> ユーザー入力を待機中...");
}
}
