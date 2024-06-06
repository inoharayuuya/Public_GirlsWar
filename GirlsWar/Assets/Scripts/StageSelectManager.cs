using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Const;

// todo stageRelease�ɃX�e�[�W�N���X����Q�Ƃ��Ď擾����
// todo ���̃A�j���[�V�����������̂��C������

public class StageSelectManager : MonoBehaviour
{
    #region  �ϐ��錾

    /* �V���A���������ϐ� */
    [SerializeField, Tooltip("�X�e�[�W�����i�[����Ă���I�u�W�F�N�g���i�[")]
    private GameObject stage;
    [SerializeField, Tooltip("�X�e�[�W���̃v���n�u���i�[")]
    private GameObject stageNamePrefab;
    [SerializeField, Tooltip("�\������X�e�[�W�����i�[")]
    private string[] stageName;
    [SerializeField, Tooltip("�X�e�[�W��̃L�������i�[")]
    private GameObject collegeStudent;
    [SerializeField, Tooltip("�X�e�[�W(�Ԋ�)���i�[")]
    private GameObject[] stageObject;
    [SerializeField, Tooltip("UI�̒��S���W���i�[")]
    private GameObject CenterUI;
    [SerializeField, Tooltip("�x���e�L�X�g���i�[")]
    private GameObject[] errorTexts;
    [SerializeField, Tooltip("�X�e�[�W�̉��ɕ\����������i�[")]
    private GameObject[] arrowImages;
    [SerializeField, Tooltip("�X�e�[�W�̊Ԃɒu���ۂ��i�[")]
    private GameObject circle;
    [SerializeField, Tooltip("�퓬�J�n�e�L�X�g�̐e�I�u�W�F�N�g���i�[")]
    private GameObject battleTextObject;

    /* �p�u���b�N�ϐ� */
    public int stageRelease;  // ���݉������Ă���X�e�[�W��

    /* �v���C�x�[�g�ϐ� */
    private GameObject[] stageNameObject;  // �X�e�[�W���I�u�W�F�N�g���i�[

    private Camera mainCameraObject;   // �J�����̃I�u�W�F�N�g

    private Vector3 startPos;  // �^�b�v���ꂽ���W
    private Vector3 endPos;    // �����Ă���Ԃ̍��W

    private bool isTap;   // �^�b�v���Ă��邩�ǂ���
    private bool isMove;  // �����邩�ǂ���

    private float speed;             // �X�e�[�W���I�u�W�F�N�g�𓮂����X�s�[�h
    private float startPosTime;      // startPosTime�̃R�s�[��ۑ�
    private float direction;         // �^�b�v���ꂽ���W���牟���ꂽ���W���������l���i�[
    private float[] stageNamePos;    // �X�e�[�W�̍��W���i�[
    private float[] stageNameSpace;  // �X�e�[�W���I�u�W�F�N�g�ǂ����̊�

    private int count = 0;
    private int stageIndex = 0;
    private int stageSpaceIndex = 0;
    private int clearCnt;  // �N���A���Ă���X�e�[�W�����i�[

    private Color halfAlpha = new Color(1.0f, 1.0f, 1.0f, 0.5f);  // ���l�������̕ϐ�
    private Color maxAlpha = new Color(1.0f, 1.0f, 1.0f, 1.0f);  // ���l���ő�̕ϐ�(��������Ȃ��Ȃ�)

    private Animator animator;
    private AudioSource audioSource;

    private Data loadData;

    private int battleTextIndex;

    private UnityEngine.Object[] battleObjects = new UnityEngine.Object[5];

    #endregion

    private void Awake()
    {
        // �X�e�[�W����ۑ�����
        SaveStage();
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
        // �f�o�b�N�p
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            print("�f�[�^���폜���܂���");
            PlayerPrefs.DeleteKey(Common.KEY_STAGE_DATA);
        }

        // ��ʂ�������Ă��邩�𔻒肷��
        TapCheck();

        // �X�e�[�W�̈ړ�����
        StageMove();

        // �X�e�[�W���̒��S���W���擾
        GetCenter();
    }

    /// <summary>
    /// �X�e�[�W���̕ۑ�
    /// </summary>
    void SaveStage()
    {
        if (PlayerPrefs.HasKey(Common.KEY_STAGE_DATA))
        {
            loadData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
            print("�f�[�^��������");

            // �e�X�g��Json�f�[�^��\��
            print("Json�^" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        }
        else
        {
            loadData = new Data();
            loadData.stages = new Stage[4];
            print("�f�[�^�Ȃ�������A������V�����������");

            // �e�X�e�[�W��
            EnemyInfo[] tmpEnemyInfos;

            tmpEnemyInfos = new EnemyInfo[]
            {
            new EnemyInfo(EnemyID.NORMAL, 10, 20, int.MaxValue)
            };

            SetStage(STAGE_ID.TUTORIAL, 10, true, tmpEnemyInfos);

            tmpEnemyInfos = new EnemyInfo[]
            {
            new EnemyInfo(EnemyID.NORMAL, 0, 15, int.MaxValue),
            new EnemyInfo(EnemyID.LIGHTNING, 60, 20, 1)
            };

            SetStage(STAGE_ID.STAGE1, 100, true, tmpEnemyInfos);

            tmpEnemyInfos = new EnemyInfo[]
            {
            new EnemyInfo(EnemyID.NORMAL, 0, 10, int.MaxValue),
            new EnemyInfo(EnemyID.LIGHTNING, 15, 20, 10)
            };

            SetStage(STAGE_ID.STAGE2, 200, true, tmpEnemyInfos);

            tmpEnemyInfos = new EnemyInfo[]
            {
            new EnemyInfo(EnemyID.NORMAL, 0, 10, int.MaxValue),
            new EnemyInfo(EnemyID.LIGHTNING, 60, 20, 3),
            new EnemyInfo(EnemyID.FIRE, 100, 50, 3),
            };

            SetStage(STAGE_ID.STAGE3, 300, true, tmpEnemyInfos);

            string tmp = JsonUtility.ToJson(loadData);

            PlayerPrefs.SetString(Common.KEY_STAGE_DATA, tmp);
            PlayerPrefs.Save();

            // �f�[�^�\��
            print("Json�^" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        }
    }

    void SetStage(STAGE_ID _id, int _hp, bool _flg, EnemyInfo[] _enemyInfos)
    {
        loadData.stages[(int)_id] = new Stage();
        loadData.stages[(int)_id].castleHp = _hp;
        loadData.stages[(int)_id].isInstall = _flg;
        loadData.stages[(int)_id].enemyInfos = _enemyInfos;
    }

    //EnemyInfo SetEnemy(EnemyID _id, float _st, float _interval, int _n)
    //{
    //    return new EnemyInfo(EnemyID.NORMAL, 10, 10, int.MaxValue);
    //}

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        // �t���[�����[�g�ő�l��60�ɐݒ�
        Application.targetFrameRate = 60;

        // CollegeStudent�̃A�j���[�^�[��ϐ��ɃZ�b�g
        animator = collegeStudent.GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();

        startPos = Vector3.zero;  // �^�b�v���ꂽ�n�߂̈ʒu
        endPos = Vector3.zero;  // �^�b�v���I������Ƃ��̈ʒu

        speed = Common.SS_INIT_SPEED;             // �ݒ肵�������X�s�[�h�ŏ�����
        startPosTime = Common.SS_START_POS_TIME;  // startPos�̒l���X�V����X�V�p�x
        direction = 0;

        // �X�e�[�W�ƃL�����A�J�����̍��W�Ɏg���ϐ�
        var pos = stageObject[0].transform.position;  // ������stageObject�̃C���f�b�N�X��ς���΃X�e�[�W�̏������W���ς��

        // �X�e�[�W�̏����l���Z�b�g
        var stagePos = stage.transform.position;
        stagePos.x = -pos.x;
        stage.transform.position = stagePos;

        stageNameObject = new GameObject[Common.SS_MAX_STAGE];
        stageNamePos = new float[Common.SS_MAX_STAGE];
        stageNameSpace = new float[Common.SS_MAX_STAGE - 1];

        isTap = false;
        isMove = false;

        // �X�e�[�W�f�[�^�ǂݍ���
        loadData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        clearCnt = 0;

        // isClear�̐����J�E���g
        foreach (var stage in loadData.stages)
        {
            if (stage.isClear)
            {
                clearCnt++;
            }
            else
            {
                print("isClear��false�̂Ƃ��낪����܂����B");
                break;
            }
        }

        print("�f�o�b�O�F" + clearCnt);

        // �S�X�e�[�W�𐶐����A�q�G�����L�[�̖��O���ύX
        for (int i = 0; i < Common.SS_MAX_STAGE; i++)
        {
            GameObject instance = Instantiate(stageNamePrefab, stage.transform);
            stageNameObject[i] = instance;
        }

        if (clearCnt == Common.SS_MAX_STAGE)
        {
            clearCnt--;
        }

        // �\������X�e�[�W�����\��
        for (int i = 0; i < clearCnt + 1; i++)
        {
            stageNameObject[i].GetComponentInChildren<Text>().text = stageName[i];
        }

        // �ŏ��ɑI������Ă���X�e�[�W�̃��l��ς���
        stageNameObject[0].GetComponent<Image>().color = maxAlpha;

        // �������ĂȂ��X�e�[�W���I�u�W�F�N�g���\���ɂ���
        for (int i = clearCnt + 1; i < Common.SS_MAX_STAGE; i++)
        {
            stageNameObject[i].SetActive(false);
            stageObject[i].SetActive(false);
        }

        // �L�����̏������W���Z�b�g
        var collegeStudentPos = collegeStudent.transform.position;
        collegeStudentPos.x = pos.x;
        collegeStudent.transform.position = collegeStudentPos;

        // �J�����I�u�W�F�N�g�擾
        mainCameraObject = Camera.main;

        // �J�����̏������W���Z�b�g
        var cameraPos = mainCameraObject.transform.position;
        cameraPos.x = pos.x;
        mainCameraObject.transform.position = cameraPos;

        battleTextIndex = 0;
        stageRelease = clearCnt + 1;

        // �X�e�[�W�̊Ԃ̊ۂ𐶐�
        CreateCircle();
        //Invoke("CreateCircle", 0.5f);
    }

    /// <summary>
    /// circle�𐶐�
    /// </summary>
    void CreateCircle()
    {
        // �X�e�[�W�I�u�W�F�N�g�̊ԂɊԊu��������circle�𐶐�
        for (int i = 0; i < clearCnt; i++)
        {
            var pos = stageObject[i].transform.position;
            var nextPos = stageObject[i + 1].transform.position;
            var distance = pos - nextPos;

            var spaceX = Mathf.Abs(distance.x / Common.SS_CIRCLE_SPACE);
            var spaceY = Mathf.Abs(distance.y / Common.SS_CIRCLE_SPACE);

            if (spaceX > spaceY)
            {
                spaceY = Mathf.Abs(distance.y / spaceX);
                spaceX = Common.SS_CIRCLE_SPACE;

                if (distance.y == 0)
                {
                    spaceY = Common.SS_CIRCLE_SPACE;
                }
            }
            else if (spaceX < spaceY)
            {
                spaceX = Mathf.Abs(distance.x / spaceY);
                spaceY = Common.SS_CIRCLE_SPACE;

                if (distance.x == 0)
                {
                    spaceX = Common.SS_CIRCLE_SPACE;
                }
            }
            else
            {
                spaceX = Common.SS_CIRCLE_SPACE;
                spaceY = Common.SS_CIRCLE_SPACE;
            }

            int cnt = 0;
            var minDistance = 0.5f;
            while (Mathf.Abs(distance.x) > minDistance || Mathf.Abs(distance.y) > minDistance)
            {
                if (cnt > 100)
                {
                    break;
                }

                if (distance.x > 0)
                {
                    pos.x += -spaceX;
                    distance.x += -spaceX;
                }
                else if (distance.x < 0)
                {
                    pos.x += spaceX;
                    distance.x += spaceX;
                }
                if (distance.y > 0)
                {
                    pos.y += -spaceY;
                    distance.y += -spaceY;
                }
                else if (distance.y < 0)
                {
                    pos.y += spaceY;
                    distance.y += spaceY;
                }

                Instantiate(circle, pos, Quaternion.identity);
                cnt++;
            }
        }
    }

    /// <summary>
    /// �^�b�v�`�F�b�N����
    /// </summary>
    void TapCheck()
    {
        // ��ʂ��^�b�v���ꂽ�Ƃ�
        if (Input.GetMouseButtonDown(0))
        {
            // startPos�ɉ�ʂ������ꂽ���W��������
            startPos = Input.mousePosition;

            // ��ʂ�������Ă���̂�true�ɂ���
            isTap = true;

            // ������悤�ɂȂ�̂�true
            isMove = true;
        }

        // ��ʂ��^�b�v����Ă����
        if (Input.GetMouseButton(0))
        {
            // endPos�ɂ͖��t���[����ʂ�������Ă�����W��������
            endPos = Input.mousePosition;

            // �X�V���Ԃ�����܂�deltaTime�Ōv�Z
            startPosTime -= Time.deltaTime;

            // �X�s�[�h�͉�����邽�тɏ���������
            speed = Common.SS_INIT_SPEED;

            // startPosTime��0�ɂȂ邽�т�startPos�����݂�endPos�ŏ�����
            if (startPosTime <= 0)
            {
                // startPosTime�̏�����
                startPosTime = Common.SS_START_POS_TIME;
                startPos = endPos;
            }
        }

        // ��ʂ���w�������ꂽ�Ƃ�
        if (Input.GetMouseButtonUp(0))
        {
            // �w�������ꂽ�̂�false
            isTap = false;
        }

        // ��ʂ�������Ă��Ȃ��Ԃ͌�����������
        if (!isTap)
        {
            speed *= Common.SS_SLOW_DOWN_SPEED;
            //direction *= Common.SS_SLOW_DOWN_SPEED;

            if (speed < 0.05f || Mathf.Abs(direction) < 50)
            {
                // �ʏ�̈ړ������������̂�false
                isMove = false;

                // �ǔ����钆�S�_���m�肳����
                stageIndex = CheckCenter();

                // �X�e�[�W���̒��S�Ɋ�点��
                if (stage.GetComponent<RectTransform>().localPosition.x < -(stageNamePos[stageIndex] + 20))
                {
                    stage.GetComponent<RectTransform>().localPosition += new Vector3(Common.SS_DIRECTION_SPEED, 0, 0) * Time.deltaTime;
                }
                else if (stage.GetComponent<RectTransform>().localPosition.x > -(stageNamePos[stageIndex] - 20))
                {
                    stage.GetComponent<RectTransform>().localPosition += new Vector3(-Common.SS_DIRECTION_SPEED, 0, 0) * Time.deltaTime;
                }
                else
                {
                    if (speed < Common.SS_MIN_SPEED)
                    {
                        // ���S�̍��W����
                        stage.GetComponent<RectTransform>().localPosition = new Vector3(-stageNamePos[stageIndex],
                                                                                        stage.GetComponent<RectTransform>().localPosition.y,
                                                                                        stage.GetComponent<RectTransform>().localPosition.z);
                    }

                    // �~�߂�
                    speed = 0;
                }
            }
        }
    }

    /// <summary>
    /// �X�e�[�W���̈ړ�����
    /// </summary>
    void StageMove()
    {
        // startPos��endPos���v�Z���ċ��������߂�
        direction = (endPos.x - startPos.x) * speed;

        if (isMove)
        {
            // �v�Z���ʂ��v���X�̏ꍇ(0���܂܂Ȃ�)
            if (direction > 0)
            {
                // �X�e�[�W���I�u�W�F�N�g��0����(�l���}�C�i�X)�̎������ړ�������
                if (stage.transform.position.x < CenterUI.transform.position.x)
                {
                    stage.transform.position += new Vector3((direction * speed), 0, 0) * Time.deltaTime;
                }
            }
            else
            {
                // �X�e�[�W���I�u�W�F�N�g��0�ȏ�̎������ړ�����
                if (stageNameObject[stageRelease - 1].transform.position.x >= CenterUI.transform.position.x)
                {
                    stage.transform.position += new Vector3((direction * speed), 0, 0) * Time.deltaTime;
                }
            }
        }

        // ��ԋ߂����S���W�̃C���f�b�N�X���擾
        stageSpaceIndex = CheckCenter();

        if (speed < 0.075f)
        {
            isMove = false;
            speed = 0;
        }

        // ���S���W�Ɉ�ԋ߂��I�u�W�F�N�g�̓����x���Ȃ��ɂ���
        stageNameObject[stageSpaceIndex].GetComponent<Image>().color = maxAlpha;
        stageNameObject[stageSpaceIndex].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0, 1f);

        // ���S���W�Ɉ�ԋ߂��I�u�W�F�N�g�̃T�C�Y��ύX����
        var size = 1.25f;
        stageNameObject[stageSpaceIndex].GetComponent<RectTransform>().localScale = new Vector3(size, size, size);

        // �L�������X�e�[�W���ړ�����
        CharaMove();

        // �J�������X�e�[�W���ړ�����
        CameraMove();

        // �P�X�e�[�W�̂ݕ\������Ă����ꍇ
        if (stageRelease == 1)
        {
            print("���off");
            var tmp1 = arrowImages[0].GetComponent<Image>().color;
            tmp1.a = 0f;
            arrowImages[0].GetComponent<Image>().color = tmp1;

            var tmp2 = arrowImages[0].GetComponent<Image>().color;
            tmp2.a = 0f;
            arrowImages[1].GetComponent<Image>().color = tmp2;

            return;
        }

        if (stageSpaceIndex == 0)
        {
            //arrowImages[0].GetComponent<Image>().enabled = false;
            var tmp1 = arrowImages[0].GetComponent<Image>().color;
            tmp1.a = 0;
            arrowImages[0].GetComponent<Image>().color = tmp1;
            var tmp2 = arrowImages[1].GetComponent<Image>().color;
            tmp2.a = 0.5f;
            arrowImages[1].GetComponent<Image>().color = tmp2;//errorTexts[0].SetActive(false);
        }
        else if (stageSpaceIndex == stageRelease - 1)
        {
            var tmp1 = arrowImages[0].GetComponent<Image>().color;
            tmp1.a = 0.5f;
            arrowImages[0].GetComponent<Image>().color = tmp1;
            var tmp2 = arrowImages[1].GetComponent<Image>().color;
            tmp2.a = 0;
            arrowImages[1].GetComponent<Image>().color = tmp2;
        }
        else
        {
            var tmp1 = arrowImages[0].GetComponent<Image>().color;
            tmp1.a = 0.5f;
            arrowImages[0].GetComponent<Image>().color = tmp1;

            var tmp2 = arrowImages[1].GetComponent<Image>().color;
            tmp2.a = 0.5f;
            arrowImages[1].GetComponent<Image>().color = tmp2;

            //errorTexts[0].SetActive(true);
        }
    }

    /// <summary>
    /// �X�e�[�W���ړ�����L�����̐���
    /// </summary>
    void CharaMove()
    {
        Vector2 direction = stageObject[stageSpaceIndex].transform.position - collegeStudent.transform.position;

        if (direction.x > 0)
        {
            if (collegeStudent.transform.position.x < stageObject[stageSpaceIndex].transform.position.x)
            {
                collegeStudent.transform.position += new Vector3(direction.x, 0, 0) * Common.SS_CHARA_SPEED * Time.deltaTime;
                var lScale = collegeStudent.transform.localScale;
                if (lScale.x <= 0)
                {
                    lScale.x *= -1;
                }
                collegeStudent.transform.localScale = lScale;
            }
        }
        else
        {
            if (collegeStudent.transform.position.x > stageObject[stageSpaceIndex].transform.position.x)
            {
                collegeStudent.transform.position += new Vector3(direction.x, 0, 0) * Common.SS_CHARA_SPEED * Time.deltaTime;

                var lScale = collegeStudent.transform.localScale;
                if (lScale.x >= 0)
                {
                    lScale.x *= -1;
                }
                collegeStudent.transform.localScale = lScale;
            }
        }

        if (direction.y > 0)
        {
            if (collegeStudent.transform.position.y < stageObject[stageSpaceIndex].transform.position.y)
            {
                collegeStudent.transform.position += new Vector3(0, direction.y, 0) * Common.SS_CHARA_SPEED * Time.deltaTime;
            }
        }
        else
        {
            if (collegeStudent.transform.position.y > stageObject[stageSpaceIndex].transform.position.y)
            {
                collegeStudent.transform.position += new Vector3(0, direction.y, 0) * Common.SS_CHARA_SPEED * Time.deltaTime;
            }
        }

        float limit = 0.15f;

        if (MathF.Abs(direction.x) > limit || MathF.Abs(direction.y) > limit)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }

    /// <summary>
    /// �J�����ړ��̐���
    /// </summary>
    void CameraMove()
    {
        Vector2 direction = stageObject[stageSpaceIndex].transform.position - mainCameraObject.transform.position;

        if (direction.x > 0)
        {
            if (mainCameraObject.transform.position.x < stageObject[stageSpaceIndex].transform.position.x)
            {
                mainCameraObject.transform.position += new Vector3(direction.x, 0, 0) * Common.SS_CAMERA_SPEED * Time.deltaTime;
            }
        }
        else
        {
            if (mainCameraObject.transform.position.x > stageObject[stageSpaceIndex].transform.position.x)
            {
                mainCameraObject.transform.position += new Vector3(direction.x, 0, 0) * Common.SS_CAMERA_SPEED * Time.deltaTime;
            }
        }

        if (direction.y > 0)
        {
            if (mainCameraObject.transform.position.y < stageObject[stageSpaceIndex].transform.position.y)
            {
                mainCameraObject.transform.position += new Vector3(0, direction.y, 0) * Common.SS_CAMERA_SPEED * Time.deltaTime;
            }
        }
        else
        {
            if (mainCameraObject.transform.position.y > stageObject[stageSpaceIndex].transform.position.y)
            {
                mainCameraObject.transform.position += new Vector3(0, direction.y, 0) * Common.SS_CAMERA_SPEED * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// �X�e�[�W���̒��S���W���擾(1�t���[�������Čv�Z����)
    /// </summary>
    void GetCenter()
    {
        if (count == 1)
        {
            for (int i = 0; i < stageNamePos.Length; i++)
            {
                // ���S���W���擾
                stageNamePos[i] = stageNameObject[i].GetComponent<RectTransform>().localPosition.x;

                if (i != 0 && i < stageNamePos.Length - 1)
                {
                    // �X�e�[�W���̊Ԃ̍��W���擾
                    stageNameSpace[i - 1] = (stageNamePos[i]) - 250;
                }
            }

            CheckCenter();
        }

        count++;
    }

    /// <summary>
    /// ���t���[����ʒ��S�Ɉ�ԋ߂��X�e�[�W�̒��S���W���v�Z����
    /// </summary>
    /// <returns>���S�Ɉ�ԋ߂��������S���W�̃C���f�b�N�X��Ԃ�</returns>
    int CheckCenter()
    {
        float sum = Common.SS_INIT_SUM;
        int index = 0;

        for (int i = 0; i < stageNamePos.Length; i++)
        {
            //print("sum:" + sum);
            //print("stageNamePos:" + stageNamePos[i]);
            //print("index:" + index);

            // �X�e�[�W���I�u�W�F�N�g�̃��l������������
            stageNameObject[i].GetComponent<Image>().color = halfAlpha;
            stageNameObject[i].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0, 0.5f);

            // �X�e�[�W���I�u�W�F�N�g�̃T�C�Y��������
            var size = 1f;
            stageNameObject[stageSpaceIndex].GetComponent<RectTransform>().localScale = new Vector3(size, size, size);

            // �X�e�[�W�̍��W�Ɗe�X�e�[�W�̒��S�_�̍��W�̍����v�Z
            var stageDirection = Mathf.Abs(Mathf.Abs(stage.GetComponent<RectTransform>().localPosition.x) - stageNamePos[i]);

            // �e�X�e�[�W���I�u�W�F�N�g�̋����𔻒�Asum��菬�������sum������������
            if (Mathf.Abs(sum) > stageDirection)
            {
                sum = stageDirection;
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    /// �o���{�^�����������Ƃ��̏���
    /// </summary>
    public void TapReadyButton()
    {
        // BGM���X�g�b�v����
        SoundManager.smInstance.StopBGM();

        // SE���Đ�
        AudioClip clip = LoadSE("se_battle_start");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);

        // ������X�e�[�W��ID��ۑ�
        PlayerPrefs.SetInt(Common.KEY_STAGE_ID, stageSpaceIndex);
        PlayerPrefs.Save();

        // �I�u�W�F�N�g�𐶐�����
        print("�ċA���Ă��Ȃ��R���[�`���̏����J�n");
        StartCoroutine("ObjectGenerat");
        print("�ċA���Ă��Ȃ��R���[�`���̏����I��");

        //if (stageSpaceIndex == 0)
        //{
        //    // ������X�e�[�W��ID��ۑ�
        //    PlayerPrefs.SetInt(Common.KEY_STAGE_ID, stageSpaceIndex);
        //    PlayerPrefs.Save();

        //    // �I�u�W�F�N�g�𐶐�����
        //    print("�ċA���Ă��Ȃ��R���[�`���̏����J�n");
        //    StartCoroutine("ObjectGenerat");
        //    print("�ċA���Ă��Ȃ��R���[�`���̏����I��");
        //}
    }

    /// <summary>
    /// �e�L�X�g�ƃV�[���`�F���W�̍�������w�肵���b���҂��Ă���\������
    /// </summary>
    /// <returns></returns>
    IEnumerator ObjectGenerat()
    {
        // 0.25�b�҂��Ĉȍ~�̏���������
        yield return new WaitForSeconds(0.25f);

        if (battleTextIndex < battleObjects.Length)
        {
            battleTextIndex++;
            DispBattleText();
            // 5�񎩕����g��ǂݍ���
            print(battleTextIndex + "��ڍċA�J�n");
            StartCoroutine("ObjectGenerat");
            print(battleTextIndex + "��ڍċA�I��");
        }
        else
        {
            Invoke("SceneChangeGenerat", 0.25f);
        }
    }

    /// <summary>
    /// �퓬�J�n�̃e�L�X�g��\������
    /// </summary>
    void DispBattleText()
    {
        var battleText = Resources.Load("Prefabs/BattleTexts/BattleText" + battleTextIndex);
        battleObjects[battleTextIndex - 1] = Instantiate(battleText, battleTextObject.transform);
        battleObjects[battleTextIndex - 1].GetComponent<Animator>().SetTrigger("StartAnimation");
    }

    /// <summary>
    /// �^�C�~���O�����킹�邽�߂�Invoke���Ďg��
    /// </summary>
    void SceneChangeGenerat()
    {
        // �v���n�u��ǂݍ���
        var prefab = Resources.Load("Prefabs/scene_change_prefab");

        // �V�[���`�F���W�̃X���C�h�𐶐�
        var fadeObject = Instantiate(prefab, collegeStudent.transform.position, Quaternion.identity);

        // �A�j���[�V�������N��
        fadeObject.GetComponent<Animator>().SetTrigger("Fade_In");

        // ��2�����Ŏw�肵���b���҂��ăQ�[���V�[����ǂݍ���
        Invoke("LoadGameScene", 1.0f);
    }

    /// <summary>
    /// �V�[�����[�h
    /// </summary>
    void LoadGameScene()
    {
        Common.LoadScene("GameScene");
    }

    /// <summary>
    /// SE�̃��[�h
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}
