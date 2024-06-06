using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
//using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
    [Header("�J���ҋ@�\")]
    [SerializeField, Tooltip("�p�X���[�h���͗p�t�B�[���h")]
    InputField devPassInputField;
    [SerializeField, Tooltip("�f�[�^�폜�{�^��")]
    Button devDataDeleteButton;
    [SerializeField, Tooltip("�p�X���[�h����")]
    GameObject passHidedButton;
    [SerializeField, Tooltip("�p�X���[�h�s����")]
    GameObject passNoHidedButton;

    [Header("�w�i�A�j���[�V����")]

    [SerializeField, Tooltip("�X���C���I�u�W�F�N�g")]
    GameObject[] slimes;
    [SerializeField, Tooltip("���̎q�I�u�W�F�N�g")]
    GameObject girl;
    [Tooltip("���̎q�̃A�j���[�^�[")]
    Animator girlAnimator;

    [Header("�X�g�[���[")]
    [SerializeField, Tooltip("�X�N���[������X�g�[���[�e�L�X�g")]
    GameObject storyText;
    [SerializeField, Tooltip("")]
    GameObject storyPanel;


    [Header("UI")]
    [SerializeField, Tooltip("�^�C�g���̕���")]
    GameObject titleText;
    [SerializeField, Tooltip("�Q�[���X�^�[�g�{�^��")]
    GameObject StartButton;
    [SerializeField, Tooltip("�^�C�g���p�l��")]
    GameObject startPanel;
    [SerializeField, Tooltip("���j���[�p�l��")]
    GameObject menuPanel;
    [SerializeField, Tooltip("���j���[�J���{�^��")]
    GameObject menuButton;

    [Header("���̑�")]
    [SerializeField, Tooltip("FPS�����p�e�L�X�g")]
    Text fpsText;
    // txt�t�@�C��
    [SerializeField, Tooltip("�X�g�[���[�̃e�L�X�g")]
    TextAsset storyTextAsset;

    [Header("SE")]
    [SerializeField, Tooltip("SE�p")]
    AudioSource audioSource;

    // �X�g�[���[�e�L�X�g�̊Ǘ�
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


    // �X�L�b�v����Ă邩�ǂ���
    bool isSkiped;
    // �^�b�v����Ă��邩�ǂ���
    bool isTap;

    // �X�^�[�g�������ǂ���
    bool isStart;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ������
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        fpsText.text = (1f / Time.deltaTime).ToString();

        // �X�L�b�v����Ă��邩�ǂ���
        // ����ĂȂ���΃v�����[�O�𗬂�
        if (!isSkiped)
        {
            Prologue();
        }
        // �^�C�g����ʂ̏���
        else
        {
            Title();
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    void Init()
    {
        // �v�����[�O�̒���
        // true �F�v�����[�O�I��
        // false�F�v�����[�O�I�t
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
    /// �X�L�b�v�����܂ł̏���
    /// </summary>
    void Prologue()
    {
        // �X�g�[���[�̃e�L�X�g�̍��W��ڕW�n�_�܂ňړ�����������
        if (storyText.transform.position.y < storyEndPos.y)
        {
            storyText.transform.position += storyMoveSp * Time.deltaTime;
        }
        else
        {
            // �ڕW���B�ŋ����X�L�b�v
            StorySkip();
        }
    }

    /// <summary>
    /// �^�C�g���̐���
    /// </summary>
    void Title()
    {
        // ���͎�t
        PlayerInput();

        // �L�����𓮂���
        CharacterMove();

        // �^�C�g���𓮂���
        UIMove();

        if (isStart)
        {
            // ���X�Ƀu���b�N�A�E�g
            GameStart();
        }

    }

    /// <summary>
    /// �v���C���[�����͂����l�ɂ���ď�����ύX
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
    /// �L�����N�^�[�������A�j���[�V����������
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
    /// �^�C�g���Ƃ��^�b�v�X�^�[�g�Ƃ��̐���
    /// </summary>
    void UIMove()
    {
        if (titleText.transform.position.x < 0)
        {
            titleText.transform.position += titleTextMoveSp * Time.deltaTime;
        }
        else
        {
            // �ړ��I��������^�C�g���̃A�j���[�V�����X�L�b�v�������Ƃɂ���
            TitleTap();
        }

    }


    /// <summary>
    /// �^�C�g���̃^�b�v�ŉ��o�̃X�L�b�v����
    /// </summary>
    void TitleTap()
    {
        isTap = true;
        titleText.transform.position = new Vector3(0, titleText.transform.position.y, titleText.transform.position.z);
        StartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    /// <summary>
    /// �X�g�[���[�̃X�L�b�v�����ƃ^�C�g���̏�����
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
    /// �X�^�[�g�{�^���������ꂽ���ʂ��u���b�N�A�E�g�����ăV�[���؂�ւ�
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
    /// �Q�[���J�n�{�^��
    /// </summary>
    public void GameStartButton()
    {
        isStart = true;
        startPanel.SetActive(true);
        PlayPushSound();
    }


    /// <summary>
    /// ���j���[�p�l�����I��
    /// </summary>
    public void MenuOpenButton()
    {
        menuPanel.SetActive(true);
        PlayPushSound();
    }

    /// <summary>
    /// ���j���[�p�l�����I�t
    /// </summary>
    public void MenuCloseButton()
    {
        menuPanel.SetActive(false);
        PlayPushSound();
    }

    /// <summary>
    /// �f�[�^�폜�{�^������
    /// �p�X���[�h���Q�Ƃ��ăf�[�^�̃L�[��j��
    /// </summary>
    public void PushDeleteButton()
    {
        if (devPassInputField.text == Common.PASSWORD)
        {
            print("�����A�f�[�^���폜");
            PlayerPrefs.DeleteKey(Common.KEY_STAGE_DATA);

            devPassInputField.text = string.Empty;
            devDataDeleteButton.GetComponent<Image>().color = Color.cyan;
            devDataDeleteButton.GetComponentInChildren<Text>().text = "�F��";

            Invoke("ResetPassButton", 1.0f);

        }
        else
        {
            print("���s�A�f�[�^���폜�ł��܂���ł���");
            devPassInputField.text = string.Empty;
            devDataDeleteButton.GetComponent<Image>().color = Color.gray;
            devDataDeleteButton.GetComponentInChildren<Text>().text = "���s";

            Invoke("ResetPassButton", 1.0f);
            ReloadInputField();
        }
    }

    void ResetPassButton()
    {
        devDataDeleteButton.GetComponent<Image>().color = Color.red;
        devDataDeleteButton.GetComponentInChildren<Text>().text = "�f�[�^�폜";
    }

    /// <summary>
    /// �B���Ȃ�
    /// </summary>
    public void PushHidedButton()
    {
        passNoHidedButton.SetActive(true);
        passHidedButton.SetActive(false);
        devPassInputField.contentType = InputField.ContentType.Password;
        StartCoroutine(ReloadInputField());
    }

    /// <summary>
    /// �B��
    /// </summary>
    public void PushNoHidedButton()
    {
        passNoHidedButton.SetActive(false);
        passHidedButton.SetActive(true);
        devPassInputField.contentType = InputField.ContentType.Standard;
        StartCoroutine(ReloadInputField());
    }

    /// <summary>
    /// InputField�̃R���e���c�^�C�v�𔽉f�A
    /// ���͏�Ԃ��ێ�
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadInputField()
    {
        devPassInputField.ActivateInputField();
        yield return null;
        devPassInputField.MoveTextEnd(true);
    }

    /// <summary>
    /// SE�炷�p
    /// </summary>
    void PlayPushSound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
