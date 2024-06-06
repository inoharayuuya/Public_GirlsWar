using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
//using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
    [Header("開発者機能")]
    [SerializeField, Tooltip("パスワード入力用フィールド")]
    InputField devPassInputField;
    [SerializeField, Tooltip("データ削除ボタン")]
    Button devDataDeleteButton;
    [SerializeField, Tooltip("パスワード可視化")]
    GameObject passHidedButton;
    [SerializeField, Tooltip("パスワード不可視化")]
    GameObject passNoHidedButton;

    [Header("背景アニメーション")]

    [SerializeField, Tooltip("スライムオブジェクト")]
    GameObject[] slimes;
    [SerializeField, Tooltip("女の子オブジェクト")]
    GameObject girl;
    [Tooltip("女の子のアニメーター")]
    Animator girlAnimator;

    [Header("ストーリー")]
    [SerializeField, Tooltip("スクロールするストーリーテキスト")]
    GameObject storyText;
    [SerializeField, Tooltip("")]
    GameObject storyPanel;


    [Header("UI")]
    [SerializeField, Tooltip("タイトルの文字")]
    GameObject titleText;
    [SerializeField, Tooltip("ゲームスタートボタン")]
    GameObject StartButton;
    [SerializeField, Tooltip("タイトルパネル")]
    GameObject startPanel;
    [SerializeField, Tooltip("メニューパネル")]
    GameObject menuPanel;
    [SerializeField, Tooltip("メニュー開くボタン")]
    GameObject menuButton;

    [Header("その他")]
    [SerializeField, Tooltip("FPS可視化用テキスト")]
    Text fpsText;
    // txtファイル
    [SerializeField, Tooltip("ストーリーのテキスト")]
    TextAsset storyTextAsset;

    [Header("SE")]
    [SerializeField, Tooltip("SE用")]
    AudioSource audioSource;

    // ストーリーテキストの管理
    Vector3 storyStaPos = new Vector3(0, -9, 0);
    Vector3 storyEndPos = new Vector3(0, 73, 0);
    Vector3 storyMoveSp = new Vector3(0, 1f, 0);

    Vector3[] slimeStaPositions = new Vector3[]
        {
            new Vector3(-14, -4.36f, 0),
            new Vector3(-17, -4.36f, 0),
            new Vector3(-21, -4.36f, 0)
        };
    Vector3 girlStaPos = new Vector3(-70.0f, -4.36f, 0);
    Vector3 slimesMoveSp = new Vector3(1, 0, 0);
    Vector3 girlMoveSp = new Vector3(2.5f, 0, 0);

    Vector3 titleTextStaPos = new Vector3(-17, 2.7f, 0);
    Vector3 titleTextMoveSp = new Vector3(6, 0, 0);


    float panelAlpha;
    float panelDeltaAlpha = 1.0f;


    // スキップされてるかどうか
    bool isSkiped;
    // タップされているかどうか
    bool isTap;

    // スタートしたかどうか
    bool isStart;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        fpsText.text = (1f / Time.deltaTime).ToString();

        // スキップされているかどうか
        // されてなければプロローグを流す
        if (!isSkiped)
        {
            Prologue();
        }
        // タイトル画面の処理
        else
        {
            Title();
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Init()
    {
        // プロローグの調整
        // true ：プロローグオン
        // false：プロローグオフ
#if false
        isSkiped = false;
        isTap = false;
        isStart = false;
        titleText.SetActive(false);
        storyPanel.SetActive(true);
        string loadTxt = storyTextAsset.text;
        storyText.GetComponent<Text>().text = loadTxt;
        storyText.transform.position = storyStaPos;
#else
        StorySkip();
#endif

        startPanel.SetActive(false);
        menuPanel.SetActive(false);
        menuButton.SetActive(false);

        panelAlpha = 0;

        for (int i = 0; i < slimes.Length; i++)
        {
            slimes[i].transform.position = slimeStaPositions[i];
        }
        girl.transform.position = girlStaPos;
        girlAnimator = girl.GetComponent<Animator>();
        girlAnimator.SetBool("isRun", true);


        devPassInputField.contentType = InputField.ContentType.Password;
        passHidedButton.SetActive(false);
        passNoHidedButton.SetActive(true);

        
    }

    /// <summary>
    /// スキップされるまでの処理
    /// </summary>
    void Prologue()
    {
        // ストーリーのテキストの座標を目標地点まで移動させ続ける
        if (storyText.transform.position.y < storyEndPos.y)
        {
            storyText.transform.position += storyMoveSp * Time.deltaTime;
        }
        else
        {
            // 目標到達で強制スキップ
            StorySkip();
        }
    }

    /// <summary>
    /// タイトルの制御
    /// </summary>
    void Title()
    {
        // 入力受付
        PlayerInput();

        // キャラを動かす
        CharacterMove();

        // タイトルを動かす
        UIMove();

        if (isStart)
        {
            // 徐々にブラックアウト
            GameStart();
        }

    }

    /// <summary>
    /// プレイヤーが入力した値によって処理を変更
    /// </summary>
    void PlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTap)
            {
                //Common.LoadScene("StageSelectScene");
            }
            else
            {
                TitleTap();
            }
        }
    }

    /// <summary>
    /// キャラクターたちをアニメーションさせる
    /// </summary>
    void CharacterMove()
    {
        for (int i = 0; i < slimes.Length; i++)
        {
            slimes[i].transform.position += slimesMoveSp * Time.deltaTime;
            if (slimes[i].transform.position.x > 30)
            {
                slimes[i].transform.position = slimeStaPositions[0];
            }
        }


        girl.transform.position += girlMoveSp * Time.deltaTime;
        if (girl.transform.position.x > 110 + girlStaPos.x)
        {
            girl.transform.position = girlStaPos;
        }
    }

    /// <summary>
    /// タイトルとかタップスタートとかの制御
    /// </summary>
    void UIMove()
    {
        if (titleText.transform.position.x < 0)
        {
            titleText.transform.position += titleTextMoveSp * Time.deltaTime;
        }
        else
        {
            // 移動終了したらタイトルのアニメーションスキップしたことにする
            TitleTap();
        }

    }


    /// <summary>
    /// タイトルのタップで演出のスキップ処理
    /// </summary>
    void TitleTap()
    {
        isTap = true;
        titleText.transform.position = new Vector3(0, titleText.transform.position.y, titleText.transform.position.z);
        StartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    /// <summary>
    /// ストーリーのスキップ処理とタイトルの初期化
    /// </summary>
    public void StorySkip()
    {
        isSkiped = true;
        storyPanel.SetActive(false);
        titleText.SetActive(true);
        StartButton.SetActive(false);

        titleText.transform.position = titleTextStaPos;
    }

    /// <summary>
    /// スタートボタンが押されたら画面をブラックアウトさせてシーン切り替え
    /// </summary>
    void GameStart()
    {
        var c = startPanel.GetComponent<Image>().color;
        if (c.a >= 1)
        {
            Common.LoadScene("HomeScene");
        }
        c.a = panelAlpha;
        startPanel.GetComponent<Image>().color = c;
        panelAlpha += panelDeltaAlpha * Time.deltaTime;
    }


    /// <summary>
    /// ゲーム開始ボタン
    /// </summary>
    public void GameStartButton()
    {
        isStart = true;
        startPanel.SetActive(true);
        PlayPushSound();
    }


    /// <summary>
    /// メニューパネルをオン
    /// </summary>
    public void MenuOpenButton()
    {
        menuPanel.SetActive(true);
        PlayPushSound();
    }

    /// <summary>
    /// メニューパネルをオフ
    /// </summary>
    public void MenuCloseButton()
    {
        menuPanel.SetActive(false);
        PlayPushSound();
    }

    /// <summary>
    /// データ削除ボタン押下
    /// パスワードを参照してデータのキーを破棄
    /// </summary>
    public void PushDeleteButton()
    {
        if (devPassInputField.text == Common.PASSWORD)
        {
            print("成功、データを削除");
            PlayerPrefs.DeleteKey(Common.KEY_STAGE_DATA);

            devPassInputField.text = string.Empty;
            devDataDeleteButton.GetComponent<Image>().color = Color.cyan;
            devDataDeleteButton.GetComponentInChildren<Text>().text = "認証";

            Invoke("ResetPassButton", 1.0f);

        }
        else
        {
            print("失敗、データを削除できませんでした");
            devPassInputField.text = string.Empty;
            devDataDeleteButton.GetComponent<Image>().color = Color.gray;
            devDataDeleteButton.GetComponentInChildren<Text>().text = "失敗";

            Invoke("ResetPassButton", 1.0f);
            ReloadInputField();
        }
    }

    void ResetPassButton()
    {
        devDataDeleteButton.GetComponent<Image>().color = Color.red;
        devDataDeleteButton.GetComponentInChildren<Text>().text = "データ削除";
    }

    /// <summary>
    /// 隠さない
    /// </summary>
    public void PushHidedButton()
    {
        passNoHidedButton.SetActive(true);
        passHidedButton.SetActive(false);
        devPassInputField.contentType = InputField.ContentType.Password;
        StartCoroutine(ReloadInputField());
    }

    /// <summary>
    /// 隠す
    /// </summary>
    public void PushNoHidedButton()
    {
        passNoHidedButton.SetActive(false);
        passHidedButton.SetActive(true);
        devPassInputField.contentType = InputField.ContentType.Standard;
        StartCoroutine(ReloadInputField());
    }

    /// <summary>
    /// InputFieldのコンテンツタイプを反映、
    /// 入力状態を維持
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadInputField()
    {
        devPassInputField.ActivateInputField();
        yield return null;
        devPassInputField.MoveTextEnd(true);
    }

    /// <summary>
    /// SE鳴らす用
    /// </summary>
    void PlayPushSound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
