using UnityEngine;
using UnityEngine.UI;
using Const;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// �`���[�g���A���̏�Ԑ���
    /// </summary>
    public enum TutorialState
    {
        FASE0,// �ړI����
        FASE1,// �L�����𐶐�������
        FASE2,// �������x������
        FASE3,// �������
        END   // �I��
    }


    [Tooltip("�Q�[���̏�ԁi�����A����...�j")]
    GameState state;
    [Tooltip("�`���[�g���A���̐i�s�x")]
    [SerializeField] TutorialState tutorialState;



    [Header("�����n")]

    [SerializeField, Tooltip("�E��̏������\���pText")] Text moneyText;
    [SerializeField, Tooltip("�������x���\��,LEVEL1�`")] Text moneyLvText;
    [SerializeField, Tooltip("���̃��x���A�b�v�ɕK�v�ȋ��z�\��")] Text lvUpMoneyText;
    [SerializeField, Tooltip("�������x���A�b�v�{�^��")] Button moneyLvButton;
    [SerializeField, Tooltip("�������x���A�b�v�{�^��obj")] GameObject moneyLvUpButtonObj;



    [Header("��")]

    [SerializeField, Tooltip("������I�u�W�F�N�g")] GameObject friendCastle;
    [Tooltip("������f�[�^")] Friend fCasData;
    [SerializeField, Tooltip("�G��I�u�W�F�N�g")] GameObject enemyCastle;
    [Tooltip("�G��f�[�^")] Enemy eCasData;
    [SerializeField, Tooltip("������HP�e�L�X�g")] Text fCastleHPText;
    [SerializeField, Tooltip("�G��HP�e�L�X�g")] Text eCastleHPText;



    [Header("UI")]

    [SerializeField, Tooltip("�o���������b�Z�[�W")] Text errorMessageText;
    [SerializeField, Tooltip("�L�����{�^����Layout")] Transform ButtonLayout;
    [SerializeField, Tooltip("�L�����̃{�^��Prefab")] GameObject charaButtonPrefab;
    [SerializeField, Tooltip("�Q�[���I�����̃p�l��")] GameObject resultPanel;
    [SerializeField, Tooltip("���U���g�̃e�L�X�g")] Text resultText;
    [SerializeField, Tooltip("�|�[�Y��ʂ̃p�l��")] GameObject panelPause;
    [SerializeField, Tooltip("�|�[�Y�J�n�{�^���̉摜�I�u�W�F�N�g")] GameObject pauseStartButtonImgObj;
    [SerializeField, Tooltip("�|�[�Y�J�n�{�^���̃I�u�W�F�N�g")] GameObject pauseStartButtonObj;
    [SerializeField, Tooltip("�|�[�Y�����{�^���̉摜�I�u�W�F�N�g")] GameObject pauseEndButtonImgObj;
    [SerializeField, Tooltip("�|�[�Y�����{�^���̃I�u�W�F�N�g")] GameObject pauseEndButtonObj;
    [SerializeField, Tooltip("�|�[�Y�{�^���̉摜")] Sprite[] pauseSprites;
    [Tooltip("�|�[�Y��ԕϐ�")] bool isPause;



    [Header("�I�u�W�F�N�g")]

    [SerializeField, Tooltip("�v���C���[���")] PlayerManager pm;
    [SerializeField, Tooltip("�L�����̐����I�u�W�F")] Generator generator;



    [Header("�L�����̐�����")]

    [SerializeField, Tooltip("�����L�����̐e")] GameObject friendGroup;
    [SerializeField, Tooltip("�G�L�����̐e")] GameObject enemyGroup;

    [Header("���ʉ�")]
    [SerializeField, Tooltip("�L�����̍U������SE")]
    AudioClip charaAttackSE;
    [SerializeField, Tooltip("�{�^���pSE")]
    AudioClip pushButtonSE;
    [SerializeField, Tooltip("�L�����{�^����������SE")]
    AudioClip charaButtonSE;
    [SerializeField, Tooltip("�������̃W���O��")]
    AudioClip loseJing;
    [SerializeField, Tooltip("�s�k���̃W���O��")]
    AudioClip winJing;
    [SerializeField, Tooltip("SE�p�̃X�s�[�J�[")]
    AudioSource[] audioSources;
    [SerializeField, Tooltip("�W���O���p")]
    AudioSource audioSourceJing;

    float seVol = 0.1f;
    float jingVol = 0.7f;



    [Header("�`���[�g���A����p�ϐ�")]

    [SerializeField, Tooltip("�`���[�g���A���p�̃p�l��")] GameObject tutorialPanel;
    [SerializeField, Tooltip("�`���[�g���A���̃e�L�X�g")] Text tutorialText;
    [SerializeField, Tooltip("�`���[�g���A���p�{�^��")] GameObject tutorialButton;
    [Tooltip("�`���[�g���A����p�̃L�����{�^��")] GameObject tutorialCharaButton;
    [Tooltip("�`���[�g���A����p�L����")] GameObject tutorialChara;
    [Tooltip("�`���[�g���A����p�L�����f�[�^")] Friend tutorialCharaData;
    [SerializeField, Tooltip("�L�����o�����")] GameObject arrowCharaButtonObj;
    [SerializeField, Tooltip("���z���")] GameObject arrowMoneyObj;
    [SerializeField, Tooltip("���z���x���A�b�v���")] GameObject arrowMoneyButtonObj;



    [Header("�`���[�g���A���J�ڊǗ��p�̕ϐ�")]

    bool isFase0;
    bool isFase1;
    bool isFase2;
    bool isFase3;
    bool blinkFlg;
    bool isGame;
    float blinkTimer;
    const float BLINK_LIMIT = 0.2f;
    const int ID_TUTORIAL = 0;



    [Header("�X�e�[�W���I�u�W�F�N�g")]

    Stage stage;
    Data allStagesData;
    int stageID;

    bool isGameEnd;


    private void Awake()
    {
        // Prefs����f�[�^��ǂݍ���
        try
        {
            allStagesData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
            stageID = PlayerPrefs.GetInt(Common.KEY_STAGE_ID);
            // �X�s�[�J�[����
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
            // �ǂݍ��݃G���[�Ȃ�f�[�^���Ȃ��̂Ńf�[�^�̍Đݒ���s��
            print("�ǂݍ��݃G���[");
            Common.LoadScene("StageSelectScene");
        }




        // �G�f�[�^�ǂݍ���
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


        // �����ڂ������̂ň�UUI����������ԂŎn�߂�
        pauseStartButtonObj.SetActive(false);
        moneyLvUpButtonObj.SetActive(false);

        Invoke("Init", 1.25f);
    }


    // Start is called before the first frame update
    void Start()
    {
        // �Q�[��������
        //Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGame)
        {
            // UI�\��
            UIDisp();

            // ���s����
            StateCheck();

            // �G�̐���
            EnemyController();

            if (stageID == ID_TUTORIAL)
            {
                // �`���[�g���A��
                TutorialFunc();
            }

        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Init()
    {
        // ������UI��߂�
        pauseStartButtonObj.SetActive(true);
        moneyLvUpButtonObj.SetActive(true);
        moneyLvUpButtonObj.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

        isGame = true;
        pm.ResetMoney();

        if (stageID == ID_TUTORIAL)
        {
            // 1�L������������

            tutorialCharaButton = Instantiate(charaButtonPrefab, transform.position, Quaternion.identity, ButtonLayout);
            tutorialCharaButton.GetComponent<CharacterButton>().SetPrefabData(tutorialChara);
            Time.timeScale = 0;
            tutorialPanel.SetActive(true);
        }
        else
        {
            // �L�����Ґ���ǂݍ���Ń{�^���𐶐�
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


        // �ėp�ϐ�������

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
    /// �`���[�g���A����p�֐�
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
            case TutorialState.FASE0:// �������.�{�^���������ƃQ�[�����n�܂�
                if (!isFase0)
                {
                    isFase0 = true;
                    // ���\��
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(true);

                    // �C�x���g�ǉ�
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                    pauseStartButtonObj.GetComponent<Button>().enabled = false;
                }
                string tmpTxt = "<color=" + txtColor + ">�X���C���̑����󂻂��I</color>\n\n�܂��͂��������܂�܂ő҂Ƃ��I";
                tutorialText.text = tmpTxt;

                break;
            case TutorialState.FASE1:// �L�����̐����t�F�[�Y.�L�����𐶐������FASE2��

                moneyLvUpButtonObj.GetComponent<Button>().enabled = false;
                // �L�����̐������\�ɂȂ�ƒ�~���đ���
                if (pm.GetMoney() > tutorialCharaData.GetCost())
                {
                    // ��x�̂݌Ă�
                    if (!isFase1)
                    {
                        // ����\��
                        arrowMoneyObj.SetActive(false);
                        arrowCharaButtonObj.SetActive(true);


                        // �C�x���g�폜
                        ButtonRemoveLisner(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                        pauseStartButtonObj.GetComponent<Button>().enabled = false;


                        isFase1 = true;
                        Time.timeScale = 0;
                        // UI�\��
                        tutorialPanel.SetActive(true);

                        tutorialButton.SetActive(false);
                        // �C�x���g�ǉ�
                        ButtonAddListener(tutorialCharaButton.GetComponent<Button>(), PushMoveFaseButton);
                    }
                    tmpTxt = "<color=" + txtColor + ">�L�������o��</color>\n\n�A�C�R�����^�b�v����΃L�������o���I";
                    tutorialText.text = tmpTxt;
                }
                break;
            case TutorialState.FASE2:   // �������x���̐���.OK������FASE3��
                                        // ��x�̂݌Ă�
                if (!isFase2)
                {
                    // �O�t�F�[�Y�Œǉ������C�x���g�̍폜
                    ButtonRemoveLisner(tutorialCharaButton.GetComponent<Button>(), PushMoveFaseButton);
                    pauseStartButtonObj.GetComponent<Button>().enabled = false;


                    // �s�v�Ȗ��I�t
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(false);



                    isFase2 = true;
                    Time.timeScale = 0;
                    // UI�\��
                    tutorialPanel.SetActive(true);

                    // �C�x���g�ǉ�
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);
                    // ���\��
                    arrowMoneyButtonObj.SetActive(true);
                    arrowMoneyObj.SetActive(true);
                }
                tmpTxt = "<color=" + txtColor + ">�����̃{�^����������..?</color>\n\n�����������鑬�x���オ���I\n\n�����̍ő�l�������邩��ϋɓI�ɏグ�悤�I";
                tutorialText.text = tmpTxt;
                break;
            case TutorialState.FASE3:// �������,OK�ŏI��
                moneyLvUpButtonObj.GetComponent<Button>().enabled = true;
                if (!isFase3)
                {
                    isFase3 = true;
                    Time.timeScale = 0;


                    pauseStartButtonObj.GetComponent<Button>().enabled = false;


                    // �C�x���g�ǉ�
                    ButtonAddListener(tutorialButton.GetComponent<Button>(), PushMoveFaseButton);


                    // �s�v�Ȗ��I�t
                    arrowCharaButtonObj.SetActive(false);
                    arrowMoneyObj.SetActive(false);
                    arrowMoneyButtonObj.SetActive(false);

                    // UI�\��
                    tutorialPanel.SetActive(true);

                }
                tmpTxt = "<color=" + txtColor + ">�J�����𑀍삵�悤!</color>\n\n2�{�̎w�ŃY�[���ł����I\n\n�X���C�v�ŉ��Ɉړ����ł����";
                tutorialText.text = tmpTxt;
                break;
        }

    }


    /// <summary>
    /// �`���[�g���A����p.
    /// ���������点��
    /// </summary>
    void TutorialTextBlink()
    {
        // �`���[�g���A������timeScale��0�Ȃ̂Ōv�Z
        // blinkTimer += Time.deltaTime;

        // 1�t���[���Ɍo�߂��邨�悻�̕b�������~�b�g�Ő��K��
        float d = 0.016f * BLINK_LIMIT;
        // d��s�Ŋ��邱�Ƃŉ��b�œ_�ł���ݒ�
        float s = 0.1f;
        blinkTimer += d / s;

        if (blinkTimer > BLINK_LIMIT)
        {
            blinkTimer = 0;
            blinkFlg = !blinkFlg;
        }
    }

    /// <summary>
    /// �`���[�g���A����p�{�^��
    /// </summary>
    public void PushMoveFaseButton()
    {
        pauseStartButtonObj.GetComponent<Button>().enabled = true;


        tutorialPanel.SetActive(false);

        // �ŏI�t�F�[�Y���v�Z
        var lastFase = Enum.GetValues(typeof(TutorialState)).Length;

        // �ŏI�t�F�[�Y����Ȃ���Ύ��̃t�F�[�Y��
        if ((int)tutorialState != lastFase)
        {
            tutorialState++;
        }
        Time.timeScale = 1.0f;
        PlaySE(pushButtonSE);
    }

    /// <summary>
    /// �{�^���̃I�u�W�F�N�g�������ɁA�N���b�N�C�x���g��ǉ�����֐�
    /// </summary>
    void ButtonAddListener(Button _btn, UnityAction _func)
    {
        _btn.onClick.AddListener(_func);
        _btn.gameObject.SetActive(true);
    }

    /// <summary>
    /// �{�^���̃I�u�W�F�N�g�������ɁA�N���b�N�C�x���g���폜����֐�
    /// </summary>
    void ButtonRemoveLisner(Button _btn, UnityAction _func)
    {
        _btn.onClick.RemoveListener(_func);
        //_btn.gameObject.SetActive(false);
    }


    /// <summary>
    /// �G�̐�������.�X�e�[�W�̏�񂩂琶���Ώۂ𔻒�
    /// </summary>
    void EnemyController()
    {
        // �X�e�[�W�ɓo�^���ꂽ�G�̐��������[�v
        foreach (var data in stage.enemyInfos)
        {
            // �o�ߎ��Ԃ𑫂�
            data.deltaTime += Time.deltaTime;

            // �o���ő吔�ɖ����Ȃ�
            if (data.cnt < data.num)
            {
                // �o���Ԋu���߂��Ă���
                if (data.deltaTime > (data.startTime + data.interval * data.cnt))
                {

                    // �G�̐���
                    if (generator.EnemyGenrate(data.prefab))
                    {
                        data.cnt++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �Q�[���X�e�[�^�X�̐���.���s�̌���
    /// </summary>
    void StateCheck()
    {
        int eHP = eCasData.GetHP();
        int fHP = fCasData.GetHP();

        if (eHP <= 0)
        {
            eHP = 0;
            state = GameState.WIN;
            resultText.text = "�����I�I";
            GameFinish(enemyGroup.transform);
        }

        if (fHP <= 0)
        {
            fHP = 0;
            state = GameState.LOSE;
            resultText.text = "�s�k...";
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
    /// UI�\���n
    /// </summary>
    void UIDisp()
    {
        // ��������\��
        moneyText.text = (int)pm.GetMoney() + "/" + pm.GetMoneyInfo().GetMaxMoney() + "�~";

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


        // �������x���{�^���̕\��
        moneyLvText.text = pm.GetMoneyLevel().ToString();
        // ���x�����}�b�N�X�Ȃ玟�̃��x���A�b�v�ɕK�v�ȋ��z�̕\����ς���
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
    /// ���g�p��AudioSource������
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

        return null; //���g�p��AudioSource�͂Ȃ�����
    }

    /// <summary>
    /// SE�Đ��p
    /// ���g�p��AudioSource�Ŗ炷�A�Ȃ���Ζ�Ȃ�
    /// </summary>
    void PlaySE(AudioClip clip)
    {
        var audioSource = GetUnusedAudioSource();
        //���g�p���Ȃ���Ώ����I��
        if (audioSource == null)
        {
            return;
        }
        audioSource.clip = clip;
        audioSource.volume = seVol;
        audioSource.Play();
    }

    /// <summary>
    /// �W���O���Đ��p
    /// </summary>
    void PlayJingle(AudioClip clip)
    {
        var audioSource = audioSourceJing;
        audioSource.clip = clip;
        audioSource.volume = jingVol;
        audioSource.Play();
    }


    /// <summary>
    /// �L�����̍U�����ɃL����������Ăяo��
    /// </summary>
    public void CharaAttackSE()
    {
        if (state == GameState.GAMING)
        {
            PlaySE(charaAttackSE);
        }

    }

    /// <summary>
    /// �L������������SE
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
    /// ���x���A�b�v�{�^��������
    /// </summary>
    public void PushMoneyLevelUpButton()
    {
        // �������x�����}�b�N�X�łȂ�
        if (Enum.GetNames(typeof(MoneyLevel)).Length - 1 > (int)pm.GetMoneyLevel())
        {
            // ���x���A�b�v�ɕK�v�ȋ��z������
            if (pm.GetMoney() >= pm.GetMoneyInfo().GetLvUpMoney())
            {
                pm.MoneyLevelUp();
                PlaySE(charaButtonSE);
            }
        }
        
    }

    /// <summary>
    /// �I�������Z���N�g
    /// </summary>
    public void PushResultButton()
    {
        if (state == GameState.WIN)
        {
            stage.isClear = true;

            // �G�f�[�^�ǂݍ���
            allStagesData.stages[stageID] = stage;
            var tmpData = JsonUtility.ToJson(allStagesData);
            PlayerPrefs.SetString(Common.KEY_STAGE_DATA, tmpData);
            PlayerPrefs.Save();
            print("Json�^" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        }
        PlaySE(pushButtonSE);
        Invoke("LoadSelectScene", 0.2f);
    }

    /// <summary>
    /// �|�[�Y�{�^���������Ƃ�
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
    /// �Q�[���I������
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
