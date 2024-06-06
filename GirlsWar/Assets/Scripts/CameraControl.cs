using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;

// �J�����̔�̌v�Z
// �J������x���W = �J������size * (1920 / 1080)
// �J�����T�C�Y = m / 2
// �c = (1080/ 1920) * ��

public class CameraControl : MonoBehaviour
{
    public Camera cameraMain;
    public Text[] texts;
    public GameObject[] gameObjects;
    public bool isTablet;  // �^�u���b�g�Ńv���C���Ă��邩�ǂ���

    // �J�����̃T�C�Y
    float vMin = 6.0f;

    //���O��2�_�Ԃ̋���.
    private float backDist = 0.0f;

    private float startPosTime;

    //private float cameraW, cameraH;

    private Vector3 startPos;  // �^�b�v���ꂽ���W
    private Vector3 endPos;    // �����Ă���Ԃ̍��W

    private Vector2 cameraPosRightTop, cameraPosLeftBottom;

    private float v;
    //private float limit;

    private bool isTap;     // ������Ă��邩�ǂ���
    private bool isMove;    // �ړ��ł��邩�ǂ���

    private float speed;

    //private string log;

    private float cameraLeftLimit, cameraRightLimit, cameraBottomLimit;  // ��ʂ̃X���C�v�ŃJ�����ړ��ł�����E�l

    private float cameraWidth;

    private float cameraSize;

    private float maxViewSize;

    private float m;  // �c�̔䗦�v�Z�̌��ʂ��Z�b�g

    Vector2 screensize;

    private void Start()
    {

        // �t���[�����[�g�ő�l��60�ɐݒ�
        Application.targetFrameRate = 60;

        v = cameraMain.orthographicSize;

        speed = 0;

        isTap = false;

        startPosTime = Common.CC_START_POS_TIME;

        //cameraH = cameraMain.orthographicSize;

        //log = "";

        GetCameraViewport();

        // �J�����T�C�Y��1�̎��̃J�����̉f���Ă���͈͂��v�Z
        var width = (Mathf.Abs(cameraPosLeftBottom.x - cameraPosRightTop.x)) / cameraMain.orthographicSize;

        print("cameraSize:" + width);

        //var distance = Vector2.Distance(gameObjects[0].transform.position, gameObjects[1].transform.position);
        
        // 0�
        // �ړ����E = (���� + ��̕� + �]��) / 2
        //limit = (distance + Common.CASTLE_WIDTH + Common.CASTLE_WIDTH * 2) / 2;

        cameraLeftLimit  = gameObjects[0].transform.position.x - Common.CASTLE_WIDTH * 2;  // �����̌��E�l
        cameraRightLimit = gameObjects[1].transform.position.x + Common.CASTLE_WIDTH * 2;  // �E���̌��E�l

        cameraWidth = Mathf.Abs(cameraRightLimit - cameraLeftLimit);

        print("cameraWidth" + cameraWidth);

#if false
        if (!isTablet)
        {
            m = (Common.SCREEN_HEIGHT_PHONE / Common.SCREEN_WIDTH_PHONE) * cameraWidth;
        }
        else
        {
            m = (Common.SCREEN_HEIGHT_TAB / Common.SCREEN_WIDTH_TAB) * cameraWidth;
        }
#else
        m = ((float)Screen.currentResolution.height / (float)Screen.currentResolution.width) * cameraWidth;

#endif

        //print("�A�X��" + (Common.SCREEN_HEIGHT / Common.SCREEN_WIDTH));
        print("m" + m);

        maxViewSize = m / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // ���ݕ`�悳��Ă���J�����̃T�C�Y���擾
        GetCameraViewport();

        // �^�b�v����Ă��邩���m�F
        TapCheck();

        // �J�����̈ړ�����
        CameraMove();

        //// �o�O���ؗp�̏���
        //if(Input.GetKey(KeyCode.RightArrow))
        //{
        //    v += (backDist - 1) / Common.CC_SPEED;
        //}

        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    v += (backDist + 1) / Common.CC_SPEED;
        //}
    }

    void GetCameraViewport()
    {
        // �J�����̃T�C�Y���擾
        cameraPosRightTop = Camera.main.ViewportToWorldPoint(Vector2.one);
        cameraPosLeftBottom = Camera.main.ViewportToWorldPoint(Vector2.zero);

        cameraSize = Mathf.Abs(cameraPosRightTop.x - cameraPosLeftBottom.x);

        cameraBottomLimit = (gameObjects[0].transform.position.y - 0.5f) - ((Common.CC_SPACE * cameraMain.orthographicSize));  // �n�ʂ̃X�y�[�X�̊m��

        //print("cameraSize:" + Mathf.Abs(cameraPosRightTop.x - cameraPosLeftBottom.x));
        //print("cameraPosRightTop: " + cameraPosRightTop);
        //print("cameraPosLeftBottom:" + cameraPosLeftBottom);
    }

    void TapCheck()
    {
        // �}���`�^�b�`���ǂ����m�F
        if (Input.touchCount >= 2)
        {
            isTap = true;

            //log = ("2�{�ȏ�ŉ�����Ă��܂�");

            // �^�b�`���Ă���Q�_���擾
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            //2�_�^�b�`�J�n���̋������L��
            if (t2.phase == TouchPhase.Began)
            {
                backDist = Vector2.Distance(t1.position, t2.position);
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                // �X�V���Ԃ�����܂�deltaTime�Ōv�Z
                startPosTime -= Time.deltaTime;

                // �^�b�`�ʒu�̈ړ���A�������đ����A�O��̋�������̑��Βl�����B
                float newDist = Vector2.Distance(t1.position, t2.position);
                //view = view + (backDist - newDist) / 100.0f;
                //v = v + (newDist - backDist) / 1000.0f;
                //view = view + (backDist - newDist);
                
                if (startPosTime < 0)
                {
                    startPosTime = Common.CC_START_POS_TIME;
                    backDist = newDist;
                }

                // ���E�l���I�[�o�[�����ۂ̏���
                if (v > maxViewSize)
                {
                    cameraMain.orthographicSize = maxViewSize;
                    v = maxViewSize;
                    //camera.transform.position = new Vector3(0,0,camera.transform.position.z);
                }
                else if (v < vMin)
                {
                    cameraMain.orthographicSize = vMin;
                    v = vMin;
                }
                else
                {
                    v += (backDist - newDist) / Common.CC_SPEED;
                }
            }
        }
        // 2�{�ȉ��̎�
        else
        {
            //log = ("�����ꂽ");

            // ��ʂ��^�b�v���ꂽ�Ƃ�
            if (Input.GetMouseButtonDown(0))
            {
                isTap = true;

                isMove = true;

                //log = ("1�{�ŉ����ꂽ");

                // ������Ă�����W���擾
                startPos = Input.GetTouch(0).position;

                speed = Common.CC_INIT_SPEED;
            }

            // ��ʂ��^�b�v����Ă����
            if (Input.GetMouseButton(0))
            {
                //log = ("1�{�ŉ����ꑱ���Ă���");

                // endPos�ɂ͖��t���[����ʂ�������Ă�����W��������
                endPos = Input.GetTouch(0).position;

                // �X�V���Ԃ�����܂�deltaTime�Ōv�Z
                startPosTime -= Time.deltaTime;

                // startPosTime��0�ɂȂ邽�т�startPos�����݂�endPos�ŏ�����
                if (startPosTime <= 0)
                {
                    // startPosTime�̏�����
                    startPosTime = Common.CC_START_POS_TIME;
                    startPos = endPos;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                //log = ("�����ꂽ");

                isTap = false;
            }
        }

        // ���Βl���ύX�����ꍇ�A�J�����ɑ��Βl�𔽉f������
        if (v > vMin && v < maxViewSize)
        {
            cameraMain.orthographicSize = v;
        }

        if (!isTap)
        {
            speed *= Common.CC_SLOW_DOWN_SPEED;
        }
    }

    void CameraMove()
    {
        // startPos��endPos���v�Z���ċ��������߂�
        var direction = (endPos.x - startPos.x) * speed;

        // �J������width���v�Z
        var cameraRight = cameraPosRightTop.x - cameraMain.transform.position.x;
        var cameraLeft  = cameraPosLeftBottom.x + cameraMain.transform.position.x;

        if (cameraSize < cameraWidth && cameraMain.orthographicSize >= vMin)
        {
            var tmpLeft = cameraPosLeftBottom.x - cameraLeftLimit;
            // �J�����̍����̈ړ����E�l�𒴂����ꍇ�J���������E�l�ȏ�Ɉړ������Ȃ�
            if (tmpLeft < 0)
            {
                cameraMain.transform.position += new Vector3(-tmpLeft, 0, 0);
                isMove = false;
                return;
            }

            var tmpRight = cameraPosRightTop.x - cameraRightLimit;
            // �J�����̉E���̈ړ����E�l�𒴂����ꍇ�J���������E�l�ȏ�Ɉړ������Ȃ�
            if (tmpRight > 0)
            {
                cameraMain.transform.position += new Vector3(-tmpRight, 0, 0);
                isMove = false;
                return;
            }

            var tmpBottom = cameraPosLeftBottom.y - cameraBottomLimit;
            // �J�����������ɒǏ]������
            if (tmpBottom < 0)
            {
                cameraMain.transform.position += new Vector3(0, -tmpBottom, 0);
            }
            else
            {
                cameraMain.transform.position += new Vector3(0, -tmpBottom, 0);
            }
        }
        else
        {
            cameraMain.transform.position = new Vector3(0, cameraMain.transform.position.y, cameraMain.transform.position.z);
            isMove = false;
        }

        if (isMove)
        {
            cameraMain.transform.position += new Vector3(-direction, 0, 0) * speed * Time.deltaTime;
        }

        if (speed < 0.075f)
        {
            isMove = false;
            speed = 0;
        }
    }
}
