using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;

// カメラの比の計算
// カメラのx座標 = カメラのsize * (1920 / 1080)
// カメラサイズ = m / 2
// 縦 = (1080/ 1920) * 横

public class CameraControl : MonoBehaviour
{
    public Camera cameraMain;
    public Text[] texts;
    public GameObject[] gameObjects;
    public bool isTablet;  // タブレットでプレイしているかどうか

    // カメラのサイズ
    float vMin = 6.0f;

    //直前の2点間の距離.
    private float backDist = 0.0f;

    private float startPosTime;

    //private float cameraW, cameraH;

    private Vector3 startPos;  // タップされた座標
    private Vector3 endPos;    // 押している間の座標

    private Vector2 cameraPosRightTop, cameraPosLeftBottom;

    private float v;
    //private float limit;

    private bool isTap;     // 押されているかどうか
    private bool isMove;    // 移動できるかどうか

    private float speed;

    //private string log;

    private float cameraLeftLimit, cameraRightLimit, cameraBottomLimit;  // 画面のスワイプでカメラ移動できる限界値

    private float cameraWidth;

    private float cameraSize;

    private float maxViewSize;

    private float m;  // 縦の比率計算の結果をセット

    Vector2 screensize;

    private void Start()
    {

        // フレームレート最大値を60に設定
        Application.targetFrameRate = 60;

        v = cameraMain.orthographicSize;

        speed = 0;

        isTap = false;

        startPosTime = Common.CC_START_POS_TIME;

        //cameraH = cameraMain.orthographicSize;

        //log = "";

        GetCameraViewport();

        // カメラサイズが1の時のカメラの映している範囲を計算
        var width = (Mathf.Abs(cameraPosLeftBottom.x - cameraPosRightTop.x)) / cameraMain.orthographicSize;

        print("cameraSize:" + width);

        //var distance = Vector2.Distance(gameObjects[0].transform.position, gameObjects[1].transform.position);
        
        // 0基準
        // 移動限界 = (距離 + 城の幅 + 余白) / 2
        //limit = (distance + Common.CASTLE_WIDTH + Common.CASTLE_WIDTH * 2) / 2;

        cameraLeftLimit  = gameObjects[0].transform.position.x - Common.CASTLE_WIDTH * 2;  // 左側の限界値
        cameraRightLimit = gameObjects[1].transform.position.x + Common.CASTLE_WIDTH * 2;  // 右側の限界値

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

        //print("アス比" + (Common.SCREEN_HEIGHT / Common.SCREEN_WIDTH));
        print("m" + m);

        maxViewSize = m / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // 現在描画されているカメラのサイズを取得
        GetCameraViewport();

        // タップされているかを確認
        TapCheck();

        // カメラの移動処理
        CameraMove();

        //// バグ検証用の処理
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
        // カメラのサイズを取得
        cameraPosRightTop = Camera.main.ViewportToWorldPoint(Vector2.one);
        cameraPosLeftBottom = Camera.main.ViewportToWorldPoint(Vector2.zero);

        cameraSize = Mathf.Abs(cameraPosRightTop.x - cameraPosLeftBottom.x);

        cameraBottomLimit = (gameObjects[0].transform.position.y - 0.5f) - ((Common.CC_SPACE * cameraMain.orthographicSize));  // 地面のスペースの確保

        //print("cameraSize:" + Mathf.Abs(cameraPosRightTop.x - cameraPosLeftBottom.x));
        //print("cameraPosRightTop: " + cameraPosRightTop);
        //print("cameraPosLeftBottom:" + cameraPosLeftBottom);
    }

    void TapCheck()
    {
        // マルチタッチかどうか確認
        if (Input.touchCount >= 2)
        {
            isTap = true;

            //log = ("2本以上で押されています");

            // タッチしている２点を取得
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            //2点タッチ開始時の距離を記憶
            if (t2.phase == TouchPhase.Began)
            {
                backDist = Vector2.Distance(t1.position, t2.position);
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                // 更新時間がくるまでdeltaTimeで計算
                startPosTime -= Time.deltaTime;

                // タッチ位置の移動後、長さを再測し、前回の距離からの相対値を取る。
                float newDist = Vector2.Distance(t1.position, t2.position);
                //view = view + (backDist - newDist) / 100.0f;
                //v = v + (newDist - backDist) / 1000.0f;
                //view = view + (backDist - newDist);
                
                if (startPosTime < 0)
                {
                    startPosTime = Common.CC_START_POS_TIME;
                    backDist = newDist;
                }

                // 限界値をオーバーした際の処理
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
        // 2本以下の時
        else
        {
            //log = ("離された");

            // 画面がタップされたとき
            if (Input.GetMouseButtonDown(0))
            {
                isTap = true;

                isMove = true;

                //log = ("1本で押された");

                // 押されている座標を取得
                startPos = Input.GetTouch(0).position;

                speed = Common.CC_INIT_SPEED;
            }

            // 画面がタップされている間
            if (Input.GetMouseButton(0))
            {
                //log = ("1本で押され続けている");

                // endPosには毎フレーム画面が押されている座標を代入する
                endPos = Input.GetTouch(0).position;

                // 更新時間がくるまでdeltaTimeで計算
                startPosTime -= Time.deltaTime;

                // startPosTimeが0になるたびにstartPosを現在のendPosで初期化
                if (startPosTime <= 0)
                {
                    // startPosTimeの初期化
                    startPosTime = Common.CC_START_POS_TIME;
                    startPos = endPos;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                //log = ("離された");

                isTap = false;
            }
        }

        // 相対値が変更した場合、カメラに相対値を反映させる
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
        // startPosとendPosを計算して距離を求める
        var direction = (endPos.x - startPos.x) * speed;

        // カメラのwidthを計算
        var cameraRight = cameraPosRightTop.x - cameraMain.transform.position.x;
        var cameraLeft  = cameraPosLeftBottom.x + cameraMain.transform.position.x;

        if (cameraSize < cameraWidth && cameraMain.orthographicSize >= vMin)
        {
            var tmpLeft = cameraPosLeftBottom.x - cameraLeftLimit;
            // カメラの左側の移動限界値を超えた場合カメラを限界値以上に移動させない
            if (tmpLeft < 0)
            {
                cameraMain.transform.position += new Vector3(-tmpLeft, 0, 0);
                isMove = false;
                return;
            }

            var tmpRight = cameraPosRightTop.x - cameraRightLimit;
            // カメラの右側の移動限界値を超えた場合カメラを限界値以上に移動させない
            if (tmpRight > 0)
            {
                cameraMain.transform.position += new Vector3(-tmpRight, 0, 0);
                isMove = false;
                return;
            }

            var tmpBottom = cameraPosLeftBottom.y - cameraBottomLimit;
            // カメラを下側に追従させる
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
