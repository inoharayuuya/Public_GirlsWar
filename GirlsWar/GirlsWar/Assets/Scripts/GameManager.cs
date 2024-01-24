using UnityEngine;
using UnityEngine.UI;
using Const;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// チュートリアルの状態説明
    /// </summary>
    public enum TutorialState
    {
        FASE0,// 目的説明
        FASE1,// キャラを生成させる
        FASE2,// お金レベル説明
        FASE3,// 操作説明
        END   // 終了
    }


    [Tooltip("ゲームの状態（勝ち、負け...）")]
    GameState state;
    [Tooltip("チュートリアルの進行度")]
    [SerializeField] TutorialState tutorialState;



    [Header("お金系")]

    [SerializeField, Tooltip("右上の所持金表示用Text")] Text moneyText;
    [SerializeField, Tooltip("お金レベル表示,LEVEL1～")] Text moneyLvText;
    [SerializeField, Tooltip("次のレベルアップに必要な金額表示")] Text lvUpMoneyText;
    [SerializeField, Tooltip("お金レベルアップボタン")] Button moneyLvButton;
    [SerializeField, Tooltip("お金レベルアップボタンobj")] GameObject moneyLvUpButtonObj;



    [Header("城")]

    [SerializeField, Tooltip("味方城オブジェクト")] GameObject friendCastle;
    [Tooltip("味方城データ")] Friend fCasData;
    [SerializeField, Tooltip("敵城オブジェクト")] GameObject enemyCastle;
    [Tooltip("敵城データ")] Enemy eCasData;
    [SerializeField, Tooltip("味方城HPテキスト")] Text fCastleHPText;
    [SerializeField, Tooltip("敵城HPテキスト")] Text eCastleHPText;



    [Header("UI")]

    [SerializeField, Tooltip("出撃制限メッセージ")] Text errorMessageText;
    [SerializeField, Tooltip("キャラボタンのLayout")] Transform ButtonLayout;
    [SerializeField, Tooltip("キャラのボタンPrefab")] GameObject charaButtonPrefab;
    [SerializeField, Tooltip("ゲーム終了時のパネル")] GameObject resultPanel;
    [SerializeField, Tooltip("リザルトのテキスト")] Text resultText;
    [SerializeField, Tooltip("ポーズ画面のパネル")] GameObject panelPause;
    [SerializeField, Tooltip("ポーズ開始ボタンの画像オブジェクト")] GameObject pauseStartButtonImgObj;
    [SerializeField, Tooltip("ポーズ開始ボタンのオブジェクト")] GameObject pauseStartButtonObj;
    [SerializeField, Tooltip("ポーズ解除ボタンの画像オブジェクト")] GameObject pauseEndButtonImgObj;
    [SerializeField, Tooltip("ポーズ解除ボタンのオブジェクト")] GameObject pauseEndButtonObj;
    [SerializeField, Tooltip("ポーズボタンの画像")] Sprite[] pauseSprites;
    [Tooltip("ポーズ状態変数")] bool isPause;



    [Header("オブジェクト")]

    [SerializeField, Tooltip("プレイヤー情報")] PlayerManager pm;
    [SerializeField, Tooltip("キャラの生成オブジェ")] Generator generator;



    [Header("キャラの生成先")]

    [SerializeField, Tooltip("味方キャラの親")] GameObject friendGroup;
    [SerializeField, Tooltip("敵キャラの親")] GameObject enemyGroup;

    [Header("効果音")]
    [SerializeField, Tooltip("キャラの攻撃時のSE")]
    AudioClip charaAttackSE;
    [SerializeField, Tooltip("ボタン用SE")]
    AudioClip pushButtonSE;
    [SerializeField, Tooltip("キャラボタン押下時のSE")]
    AudioClip charaButtonSE;
    [SerializeField, Tooltip("勝利時のジングル")]
    AudioClip loseJing;
    [SerializeField, Tooltip("敗北時のジングル")]
    AudioClip winJing;
    [SerializeField, Tooltip("SE用のスピーカー")]
    AudioSource[] audioSources;
    [SerializeField, Tooltip("ジングル用")]
    AudioSource audioSourceJing;

    float seVol = 0.1f;
    float jingVol = 0.7f;



    [Header("チュートリアル専用変数")]

    [SerializeField, Tooltip("チュートリアル用のパネル")] GameObject tutorialPanel;
    [SerializeField, Tooltip("チュートリアルのテキスト")] Text tutorialText;
    [SerializeField, Tooltip("チュートリアル用ボタン")] GameObject tutorialButton;
    [Tooltip("チュートリアル専用のキャラボタン")] GameObject tutorialCharaButton;
    [Tooltip("チュートリアル専用キャラ")] GameObject tutorialChara;
    [Tooltip("チュートリアル専用キャラデータ")] Friend tutorialCharaData;
    [SerializeField, Tooltip("キャラ出撃矢印")] GameObject arrowCharaButtonObj;
    [SerializeField, Tooltip("財布矢印")] GameObject arrowMoneyObj;
    [SerializeField, Tooltip("財布レベルアップ矢印")] GameObject arrowMoneyButtonObj;



    [Header("チュートリアル遷移管理用の変数")]

    bool isFase0;
    bool isFase1;
    bool isFase2;
    bool isFase3;
    bool blinkFlg;
    bool isGame;
    float blinkTimer;
    const float BLINK_LIMIT = 0.2f;
    const int ID_TUTORIAL = 0;



    [Header("ステージ情報オブジェクト")]

    Stage stage;
    Data allStagesData;
    int stageID;

    bool isGameEnd;


    private void Awake()
    {
        // Prefsからデータを読み込み
        try
        {
            allStagesData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
            stageID = PlayerPrefs.GetInt(Common.KEY_STAGE_ID);
            // スピーカー生成
            audioSources = new AudioSource[10];
            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
            }

            //stageID = ID_TUTORIAL;
            //stageID = 1;
        }
        catch
        {
            // 読み込みエラーならデータがないのでデータの再設定を行う
            print("読み込みエラー");
            Common.LoadScene("StageSelectScene");
        }




        // 敵データ読み込み
        stage = allStagesData.stages[stageID];

        tutorialChara = (GameObject)Resources.Load("Prefabs/CollegeStudent");
        tutorialCharaData = tutorialChara.GetComponent<Friend>();

        var sceneChange = (GameObject)Resources.Load("Prefabs/scene_change_reverse_prefab");
        var mainCamera = Camera.main;
        var pos = new Vector3(mainCamera.transform.position.x, -7f, 10f);
        var sceneChangeObject = Instantiate(sceneChange, pos, Quaternion.identity);
        sceneChangeObject.transform.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        sceneChangeObject.GetComponent<Animator>().SetTrigger("Fade_Out");
        isGame = false;


        // 見た目が悪いので一旦UIを消した状態で始める
        pauseStartButtonObj.SetActive(false);
        moneyLvUpButtonObj.SetActive(false);

        Invoke("Init", 1.25f);
    }


    // Start is called before the first frame update
    void Start()
    {
        // ゲーム初期化
        //Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGame)
        {
            // UI表示
            UIDisp();

            // 勝敗判定
            StateCheck();

            // 敵の生成
            EnemyController();

            if (stageID == ID_TUTORIAL)
            {
                // チュートリアル
                TutorialFunc();
            }

        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init()
    {
        // 消したUIを戻す
        pauseStartButtonObj.SetActive(true);
        moneyLvUpButtonObj.SetActive(true);
        moneyLvUpButtonObj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

        isGame = true;
        pm.ResetMoney();

        if (stageID == ID_TUTORIAL)
        {
            // 1キャラだけ生成

            tutorialCharaButton = Instantiate(charaButtonPrefab, transform.position, Quaternion.identity, ButtonLayout);
            tutorialCharaButton.GetComponent<CharacterButton>().SetPrefabData(tutorialChara);
            Time.timeScale = 0;
            tutorialPanel.SetActive(true);
        }
        else
        {
            // キャラ編成を読み込んでボタンを生成
            foreach (GameObject obj in pm.GetFormation())
            {
                GameObject tmp = Instantiate(charaButtonPrefab, transform.position, Quaternion.identity, ButtonLayout);
                tmp.GetComponent<CharacterButton>().SetPrefabData(obj);
            }
            arrowCharaButtonObj.SetActive(false);
            arrowMoneyButtonObj.SetActive(false);
            arrowMoneyObj.SetActive(false);
            tutorialPanel.SetActive(false);
        }


        // 汎用変数初期化

        state = GameState.GAMING;
        tutorialState = TutorialState.FASE0;
        resultPanel.SetActive(false);
        fCasData = friendCastle.GetComponent<Friend>();
        eCasData = enemyCastle.GetComponent<Enemy>();

        winJing = (AudioClip)Resources.Load("Sounds/SE/se_win");
        loseJing = (AudioClip)Resources.Load("Sounds/SE/se_lose");

        eCasData.CastleInit(stage.castleHp);

        isPause = false;
        panelPause.SetActive(false);


        isFase0 = false;
        isFase1 = false;
        isFase2 = false;
        isFase3 = false;


        blinkFlg = false;
        blinkTimer = 0;

        isGameEnd = false;
    }


    /// <summary>
    /// チュートリアル専用関数
    /// </summary>
    void TutorialFunc()
    {

        string txtColor;
        if (blinkFlg)
        {
            txtColor = "yellow";
        }
        else
        {
            txtColor = "magenta";
        }

        TutorialTextBlink();


        switch (tutorialState)
        {
            case TutorialState.FASE0:// 初期状態.ボタンを押すとゲームが始まる
                if (!isFase0)
                {
                    isFase0 = true;
                    // 矢印表示
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(true);

                    // イベント追加
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                    pauseStartButtonObj.GetComponent<Button>().enabled = false;
                }
                string tmpTxt = "<color=" + txtColor + ">スライムの巣を壊そう！</color>\n\nまずはお金が溜まるまで待とう！";
                tutorialText.text = tmpTxt;

                break;
            case TutorialState.FASE1:// キャラの生成フェーズ.キャラを生成するとFASE2へ

                moneyLvUpButtonObj.GetComponent<Button>().enabled = false;
                // キャラの生成が可能になると停止して促す
                if (pm.GetMoney() > tutorialCharaData.GetCost())
                {
                    // 一度のみ呼ぶ
                    if (!isFase1)
                    {
                        // 矢印非表示
                        arrowMoneyObj.SetActive(false);
                        arrowCharaButtonObj.SetActive(true);


                        // イベント削除
                        ButtonRemoveLisner(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                        pauseStartButtonObj.GetComponent<Button>().enabled = false;


                        isFase1 = true;
                        Time.timeScale = 0;
                        // UI表示
                        tutorialPanel.SetActive(true);

                        tutorialButton.SetActive(false);
                        // イベント追加
                        ButtonAddListener(tutorialCharaButton.GetComponent<Button>(), PushMoveFaseButton);
                    }
                    tmpTxt = "<color=" + txtColor + ">キャラを出撃</color>\n\nアイコンをタップすればキャラが出現！";
                    tutorialText.text = tmpTxt;
                }
                break;
            case TutorialState.FASE2:   // お金レベルの説明.OK押せばFASE3へ
                                        // 一度のみ呼ぶ
                if (!isFase2)
                {
                    // 前フェーズで追加したイベントの削除
                    ButtonRemoveLisner(tutorialCharaButton.GetComponent<Button>(), PushMoveFaseButton);
                    pauseStartButtonObj.GetComponent<Button>().enabled = false;


                    // 不要な矢印オフ
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(false);



                    isFase2 = true;
                    Time.timeScale = 0;
                    // UI表示
                    tutorialPanel.SetActive(true);

                    // イベント追加
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                    // 矢印表示
                    arrowMoneyButtonObj.SetActive(true);
                    arrowMoneyObj.SetActive(true);
                }
                tmpTxt = "<color=" + txtColor + ">左下のボタンを押すと..?</color>\n\nお金が増える速度が上がるよ！\n\nお金の最大値も増えるから積極的に上げよう！";
                tutorialText.text = tmpTxt;
                break;
            case TutorialState.FASE3:// 操作説明,OKで終了
                moneyLvUpButtonObj.GetComponent<Button>().enabled = true;
                if (!isFase3)
                {
                    isFase3 = true;
                    Time.timeScale = 0;


                    pauseStartButtonObj.GetComponent<Button>().enabled = false;


                    // イベント追加
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);


                    // 不要な矢印オフ
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(false);
                    arrowMoneyButtonObj.SetActive(false);

                    // UI表示
                    tutorialPanel.SetActive(true);

                }
                tmpTxt = "<color=" + txtColor + ">カメラを操作しよう!</color>\n\n2本の指でズームできるよ！\n\nスワイプで横に移動もできるよ";
                tutorialText.text = tmpTxt;
                break;
        }

    }


    /// <summary>
    /// チュートリアル専用.
    /// 文字を光らせる
    /// </summary>
    void TutorialTextBlink()
    {
        // チュートリアル中はtimeScaleが0なので計算
        // blinkTimer += Time.deltaTime;

        // 1フレームに経過するおよその秒数をリミットで正規化
        float d = 0.016f * BLINK_LIMIT;
        // dをsで割ることで何秒で点滅かを設定
        float s = 0.1f;
        blinkTimer += d / s;

        if (blinkTimer > BLINK_LIMIT)
        {
            blinkTimer = 0;
            blinkFlg = !blinkFlg;
        }
    }

    /// <summary>
    /// チュートリアル専用ボタン
    /// </summary>
    public void PushMoveFaseButton()
    {
        pauseStartButtonObj.GetComponent<Button>().enabled = true;


        tutorialPanel.SetActive(false);

        // 最終フェーズを計算
        var lastFase = Enum.GetValues(typeof(TutorialState)).Length;

        // 最終フェーズじゃなければ次のフェーズへ
        if ((int)tutorialState != lastFase)
        {
            tutorialState++;
        }
        Time.timeScale = 1.0f;
        PlaySE(pushButtonSE);
    }

    /// <summary>
    /// ボタンのオブジェクトを引数に、クリックイベントを追加する関数
    /// </summary>
    void ButtonAddListener(Button _btn, UnityAction _func)
    {
        _btn.onClick.AddListener(_func);
        _btn.gameObject.SetActive(true);
    }

    /// <summary>
    /// ボタンのオブジェクトを引数に、クリックイベントを削除する関数
    /// </summary>
    void ButtonRemoveLisner(Button _btn, UnityAction _func)
    {
        _btn.onClick.RemoveListener(_func);
        //_btn.gameObject.SetActive(false);
    }


    /// <summary>
    /// 敵の生成制御.ステージの情報から生成対象を判定
    /// </summary>
    void EnemyController()
    {
        // ステージに登録された敵の数だけループ
        foreach (var data in stage.enemyInfos)
        {
            // 経過時間を足す
            data.deltaTime += Time.deltaTime;

            // 出撃最大数に満たない
            if (data.cnt < data.num)
            {
                // 出撃間隔を過ぎている
                if (data.deltaTime > (data.startTime + data.interval * data.cnt))
                {

                    // 敵の生成
                    if (generator.EnemyGenrate(data.prefab))
                    {
                        data.cnt++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ゲームステータスの制御.勝敗の決定
    /// </summary>
    void StateCheck()
    {
        int eHP = eCasData.GetHP();
        int fHP = fCasData.GetHP();

        if (eHP <= 0)
        {
            eHP = 0;
            state = GameState.WIN;
            resultText.text = "勝利！！";
            GameFinish(enemyGroup.transform);
        }

        if (fHP <= 0)
        {
            fHP = 0;
            state = GameState.LOSE;
            resultText.text = "敗北...";
            GameFinish(friendGroup.transform);
        }


        if (state == GameState.WIN || state == GameState.LOSE)
        {
            if(!isGameEnd)
            {
                SoundManager.smInstance.StopBGM();
                if (state == GameState.LOSE)
                {
                    PlayJingle(loseJing);
                }
                else
                {
                    PlayJingle(winJing);
                }
                isGameEnd = true;
            }
            
            resultPanel.SetActive(true);
        }


        fCastleHPText.text = fHP.ToString() + "/" + fCasData.GetMaxHP().ToString();
        eCastleHPText.text = eHP.ToString() + "/" + eCasData.GetMaxHP().ToString();
    }



    /// <summary>
    /// UI表示系
    /// </summary>
    void UIDisp()
    {
        // 所持金を表示
        moneyText.text = (int)pm.GetMoney() + "/" + pm.GetMoneyInfo().GetMaxMoney() + "円";

        if ((int)pm.GetMoneyLevel() == Enum.GetNames(typeof(MoneyLevel)).Length - 1)
        {
            moneyLvUpButtonObj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (pm.GetMoney() < pm.GetMoneyInfo().GetLvUpMoney())
            {
                moneyLvUpButtonObj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            else
            {
                moneyLvUpButtonObj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }


        // お金レベルボタンの表示
        moneyLvText.text = pm.GetMoneyLevel().ToString();
        // レベルがマックスなら次のレベルアップに必要な金額の表示を変える
        if ((int)pm.GetMoneyLevel() == Enum.GetNames(typeof(MoneyLevel)).Length - 1)
        {
            lvUpMoneyText.text = "MAX";
            moneyLvUpButtonObj.GetComponent<Button>().enabled = false;
        }
        else
        {
            lvUpMoneyText.text = pm.GetMoneyInfo().GetLvUpMoney().ToString();
        }
    }

    /// <summary>
    /// 未使用のAudioSourceを検索
    /// </summary>
    AudioSource GetUnusedAudioSource()
    {
        for (int i = 0; i < audioSources.Length; ++i)
        {
            if (audioSources[i].isPlaying == false)
            {
                return audioSources[i];
            }
        }

        return null; //未使用のAudioSourceはなかった
    }

    /// <summary>
    /// SE再生用
    /// 未使用のAudioSourceで鳴らす、なければ鳴らない
    /// </summary>
    void PlaySE(AudioClip clip)
    {
        var audioSource = GetUnusedAudioSource();
        //未使用がなければ処理終了
        if (audioSource == null)
        {
            return;
        }
        audioSource.clip = clip;
        audioSource.volume = seVol;
        audioSource.Play();
    }

    /// <summary>
    /// ジングル再生用
    /// </summary>
    void PlayJingle(AudioClip clip)
    {
        var audioSource = audioSourceJing;
        audioSource.clip = clip;
        audioSource.volume = jingVol;
        audioSource.Play();
    }


    /// <summary>
    /// キャラの攻撃時にキャラ側から呼び出し
    /// </summary>
    public void CharaAttackSE()
    {
        if (state == GameState.GAMING)
        {
            PlaySE(charaAttackSE);
        }

    }

    /// <summary>
    /// キャラ生成時のSE
    /// </summary>
    public void PushCharaButtonSE()
    {
        PlaySE(charaButtonSE);
    }



    public GameState GetGameState() { return state; }

    public void DispErrorMsg(string _msg)
    {

        errorMessageText.text = _msg;
        errorMessageText.gameObject.SetActive(true);
    }

    public void DeleteErrorMsg()
    {
        errorMessageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// レベルアップボタン押下時
    /// </summary>
    public void PushMoneyLevelUpButton()
    {
        // お金レベルがマックスでない
        if (Enum.GetNames(typeof(MoneyLevel)).Length - 1 > (int)pm.GetMoneyLevel())
        {
            // レベルアップに必要な金額がある
            if (pm.GetMoney() >= pm.GetMoneyInfo().GetLvUpMoney())
            {
                pm.MoneyLevelUp();
                PlaySE(charaButtonSE);
            }
        }
        
    }

    /// <summary>
    /// 終わったらセレクト
    /// </summary>
    public void PushResultButton()
    {
        if (state == GameState.WIN)
        {
            stage.isClear = true;

            // 敵データ読み込み
            allStagesData.stages[stageID] = stage;
            var tmpData = JsonUtility.ToJson(allStagesData);
            PlayerPrefs.SetString(Common.KEY_STAGE_DATA, tmpData);
            PlayerPrefs.Save();
            print("Json型" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        }
        PlaySE(pushButtonSE);
        Invoke("LoadSelectScene", 0.2f);
    }

    /// <summary>
    /// ポーズボタン押したとき
    /// </summary>
    public void PushPauseButton()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            panelPause.SetActive(true);
            pauseStartButtonImgObj.GetComponentInChildren<Image>().sprite = pauseSprites[1];
            pauseEndButtonImgObj.GetComponentInChildren<Image>().sprite = pauseSprites[1];
        }
        else
        {
            Time.timeScale = 1;
            panelPause.SetActive(false);
            pauseStartButtonImgObj.GetComponentInChildren<Image>().sprite = pauseSprites[0];
            pauseEndButtonImgObj.GetComponentInChildren<Image>().sprite = pauseSprites[0];
        }
        PlaySE(pushButtonSE);
    }

    public void PushBackSelectButton()
    {
        Time.timeScale = 1;
        PlaySE(pushButtonSE);

        Invoke("LoadSelectScene", 0.2f);
    }

    void LoadSelectScene()
    {
        Common.LoadScene("StageSelectScene");
    }

    public void PushBackTitleButton()
    {
        Time.timeScale = 1;
        PlaySE(pushButtonSE);

        Invoke("LoadTitleScene", 0.2f);
    }

    void LoadTitleScene()
    {
        Common.LoadScene("Title");
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    void GameFinish(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.GetComponent<CharacterBase>().isCastle)
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }
}
