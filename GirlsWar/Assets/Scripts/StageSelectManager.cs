using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Const;

// todo stageReleaseにステージクラスから参照して取得する
// todo 矢印のアニメーションがずれるのを修正する

public class StageSelectManager : MonoBehaviour
{
    #region  変数宣言

    /* シリアル化した変数 */
    [SerializeField, Tooltip("ステージ名が格納されているオブジェクトを格納")]
    private GameObject stage;
    [SerializeField, Tooltip("ステージ名のプレハブを格納")]
    private GameObject stageNamePrefab;
    [SerializeField, Tooltip("表示するステージ名を格納")]
    private string[] stageName;
    [SerializeField, Tooltip("ステージ上のキャラを格納")]
    private GameObject collegeStudent;
    [SerializeField, Tooltip("ステージ(赤丸)を格納")]
    private GameObject[] stageObject;
    [SerializeField, Tooltip("UIの中心座標を格納")]
    private GameObject CenterUI;
    [SerializeField, Tooltip("警告テキストを格納")]
    private GameObject[] errorTexts;
    [SerializeField, Tooltip("ステージの横に表示する矢印を格納")]
    private GameObject[] arrowImages;
    [SerializeField, Tooltip("ステージの間に置く丸を格納")]
    private GameObject circle;
    [SerializeField, Tooltip("戦闘開始テキストの親オブジェクトを格納")]
    private GameObject battleTextObject;

    /* パブリック変数 */
    public int stageRelease;  // 現在解放されているステージ数

    /* プライベート変数 */
    private GameObject[] stageNameObject;  // ステージ名オブジェクトを格納

    private Camera mainCameraObject;   // カメラのオブジェクト

    private Vector3 startPos;  // タップされた座標
    private Vector3 endPos;    // 押している間の座標

    private bool isTap;   // タップしているかどうか
    private bool isMove;  // 動けるかどうか

    private float speed;             // ステージ名オブジェクトを動かすスピード
    private float startPosTime;      // startPosTimeのコピーを保存
    private float direction;         // タップされた座標から押された座標を引いた値を格納
    private float[] stageNamePos;    // ステージの座標を格納
    private float[] stageNameSpace;  // ステージ名オブジェクトどうしの間

    private int count = 0;
    private int stageIndex = 0;
    private int stageSpaceIndex = 0;
    private int clearCnt;  // クリアしているステージ数を格納

    private Color halfAlpha = new Color(1.0f, 1.0f, 1.0f, 0.5f);  // α値が半分の変数
    private Color maxAlpha = new Color(1.0f, 1.0f, 1.0f, 1.0f);  // α値が最大の変数(透明じゃなくなる)

    private Animator animator;
    private AudioSource audioSource;

    private Data loadData;

    private int battleTextIndex;

    private UnityEngine.Object[] battleObjects = new UnityEngine.Object[5];

    #endregion

    private void Awake()
    {
        // ステージ情報を保存する
        SaveStage();
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
        // デバック用
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            print("データを削除しました");
            PlayerPrefs.DeleteKey(Common.KEY_STAGE_DATA);
        }

        // 画面が押されているかを判定する
        TapCheck();

        // ステージの移動処理
        StageMove();

        // ステージ名の中心座標を取得
        GetCenter();
    }

    /// <summary>
    /// ステージ情報の保存
    /// </summary>
    void SaveStage()
    {
        if (PlayerPrefs.HasKey(Common.KEY_STAGE_DATA))
        {
            loadData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
            print("データあったよ");

            // テストでJsonデータを表示
            print("Json型" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        }
        else
        {
            loadData = new Data();
            loadData.stages = new Stage[4];
            print("データなかったよ、だから新しく作ったよ");

            // 各ステージの
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

            // データ表示
            print("Json型" + PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
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
    /// 初期化
    /// </summary>
    void Init()
    {
        // フレームレート最大値を60に設定
        Application.targetFrameRate = 60;

        // CollegeStudentのアニメーターを変数にセット
        animator = collegeStudent.GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();

        startPos = Vector3.zero;  // タップされた始めの位置
        endPos = Vector3.zero;  // タップが終わったときの位置

        speed = Common.SS_INIT_SPEED;             // 設定した初期スピードで初期化
        startPosTime = Common.SS_START_POS_TIME;  // startPosの値を更新する更新頻度
        direction = 0;

        // ステージとキャラ、カメラの座標に使う変数
        var pos = stageObject[0].transform.position;  // ここのstageObjectのインデックスを変えればステージの初期座標が変わる

        // ステージの初期値をセット
        var stagePos = stage.transform.position;
        stagePos.x = -pos.x;
        stage.transform.position = stagePos;

        stageNameObject = new GameObject[Common.SS_MAX_STAGE];
        stageNamePos = new float[Common.SS_MAX_STAGE];
        stageNameSpace = new float[Common.SS_MAX_STAGE - 1];

        isTap = false;
        isMove = false;

        // ステージデータ読み込み
        loadData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Common.KEY_STAGE_DATA));
        clearCnt = 0;

        // isClearの数をカウント
        foreach (var stage in loadData.stages)
        {
            if (stage.isClear)
            {
                clearCnt++;
            }
            else
            {
                print("isClearがfalseのところがありました。");
                break;
            }
        }

        print("デバッグ：" + clearCnt);

        // 全ステージを生成し、ヒエラルキーの名前も変更
        for (int i = 0; i < Common.SS_MAX_STAGE; i++)
        {
            GameObject instance = Instantiate(stageNamePrefab, stage.transform);
            stageNameObject[i] = instance;
        }

        if (clearCnt == Common.SS_MAX_STAGE)
        {
            clearCnt--;
        }

        // 表示するステージ数分表示
        for (int i = 0; i < clearCnt + 1; i++)
        {
            stageNameObject[i].GetComponentInChildren<Text>().text = stageName[i];
        }

        // 最初に選択されているステージのα値を変える
        stageNameObject[0].GetComponent<Image>().color = maxAlpha;

        // 解放されてないステージ名オブジェクトを非表示にする
        for (int i = clearCnt + 1; i < Common.SS_MAX_STAGE; i++)
        {
            stageNameObject[i].SetActive(false);
            stageObject[i].SetActive(false);
        }

        // キャラの初期座標をセット
        var collegeStudentPos = collegeStudent.transform.position;
        collegeStudentPos.x = pos.x;
        collegeStudent.transform.position = collegeStudentPos;

        // カメラオブジェクト取得
        mainCameraObject = Camera.main;

        // カメラの初期座標をセット
        var cameraPos = mainCameraObject.transform.position;
        cameraPos.x = pos.x;
        mainCameraObject.transform.position = cameraPos;

        battleTextIndex = 0;
        stageRelease = clearCnt + 1;

        // ステージの間の丸を生成
        CreateCircle();
        //Invoke("CreateCircle", 0.5f);
    }

    /// <summary>
    /// circleを生成
    /// </summary>
    void CreateCircle()
    {
        // ステージオブジェクトの間に間隔を持ってcircleを生成
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
    /// タップチェック処理
    /// </summary>
    void TapCheck()
    {
        // 画面がタップされたとき
        if (Input.GetMouseButtonDown(0))
        {
            // startPosに画面が押された座標を代入する
            startPos = Input.mousePosition;

            // 画面が押されているのでtrueにする
            isTap = true;

            // 動けるようになるのでtrue
            isMove = true;
        }

        // 画面がタップされている間
        if (Input.GetMouseButton(0))
        {
            // endPosには毎フレーム画面が押されている座標を代入する
            endPos = Input.mousePosition;

            // 更新時間がくるまでdeltaTimeで計算
            startPosTime -= Time.deltaTime;

            // スピードは押されるたびに初期化する
            speed = Common.SS_INIT_SPEED;

            // startPosTimeが0になるたびにstartPosを現在のendPosで初期化
            if (startPosTime <= 0)
            {
                // startPosTimeの初期化
                startPosTime = Common.SS_START_POS_TIME;
                startPos = endPos;
            }
        }

        // 画面から指が離されたとき
        if (Input.GetMouseButtonUp(0))
        {
            // 指が離されたのでfalse
            isTap = false;
        }

        // 画面が押されていない間は減速し続ける
        if (!isTap)
        {
            speed *= Common.SS_SLOW_DOWN_SPEED;
            //direction *= Common.SS_SLOW_DOWN_SPEED;

            if (speed < 0.05f || Mathf.Abs(direction) < 50)
            {
                // 通常の移動を消したいのでfalse
                isMove = false;

                // 追尾する中心点を確定させる
                stageIndex = CheckCenter();

                // ステージ名の中心に寄らせる
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
                        // 中心の座標を代入
                        stage.GetComponent<RectTransform>().localPosition = new Vector3(-stageNamePos[stageIndex],
                                                                                        stage.GetComponent<RectTransform>().localPosition.y,
                                                                                        stage.GetComponent<RectTransform>().localPosition.z);
                    }

                    // 止める
                    speed = 0;
                }
            }
        }
    }

    /// <summary>
    /// ステージ名の移動処理
    /// </summary>
    void StageMove()
    {
        // startPosとendPosを計算して距離を求める
        direction = (endPos.x - startPos.x) * speed;

        if (isMove)
        {
            // 計算結果がプラスの場合(0を含まない)
            if (direction > 0)
            {
                // ステージ名オブジェクトが0未満(値がマイナス)の時だけ移動させる
                if (stage.transform.position.x < CenterUI.transform.position.x)
                {
                    stage.transform.position += new Vector3((direction * speed), 0, 0) * Time.deltaTime;
                }
            }
            else
            {
                // ステージ名オブジェクトが0以上の時だけ移動する
                if (stageNameObject[stageRelease - 1].transform.position.x >= CenterUI.transform.position.x)
                {
                    stage.transform.position += new Vector3((direction * speed), 0, 0) * Time.deltaTime;
                }
            }
        }

        // 一番近い中心座標のインデックスを取得
        stageSpaceIndex = CheckCenter();

        if (speed < 0.075f)
        {
            isMove = false;
            speed = 0;
        }

        // 中心座標に一番近いオブジェクトの透明度をなしにする
        stageNameObject[stageSpaceIndex].GetComponent<Image>().color = maxAlpha;
        stageNameObject[stageSpaceIndex].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0, 1f);

        // 中心座標に一番近いオブジェクトのサイズを変更する
        var size = 1.25f;
        stageNameObject[stageSpaceIndex].GetComponent<RectTransform>().localScale = new Vector3(size, size, size);

        // キャラがステージを移動する
        CharaMove();

        // カメラがステージを移動する
        CameraMove();

        // １ステージのみ表示されていた場合
        if (stageRelease == 1)
        {
            print("矢印off");
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
    /// ステージを移動するキャラの制御
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
    /// カメラ移動の制御
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
    /// ステージ名の中心座標を取得(1フレームおいて計算する)
    /// </summary>
    void GetCenter()
    {
        if (count == 1)
        {
            for (int i = 0; i < stageNamePos.Length; i++)
            {
                // 中心座標を取得
                stageNamePos[i] = stageNameObject[i].GetComponent<RectTransform>().localPosition.x;

                if (i != 0 && i < stageNamePos.Length - 1)
                {
                    // ステージ名の間の座標を取得
                    stageNameSpace[i - 1] = (stageNamePos[i]) - 250;
                }
            }

            CheckCenter();
        }

        count++;
    }

    /// <summary>
    /// 毎フレーム画面中心に一番近いステージの中心座標を計算する
    /// </summary>
    /// <returns>中心に一番近かった中心座標のインデックスを返す</returns>
    int CheckCenter()
    {
        float sum = Common.SS_INIT_SUM;
        int index = 0;

        for (int i = 0; i < stageNamePos.Length; i++)
        {
            //print("sum:" + sum);
            //print("stageNamePos:" + stageNamePos[i]);
            //print("index:" + index);

            // ステージ名オブジェクトのα値を初期化する
            stageNameObject[i].GetComponent<Image>().color = halfAlpha;
            stageNameObject[i].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0, 0.5f);

            // ステージ名オブジェクトのサイズを初期化
            var size = 1f;
            stageNameObject[stageSpaceIndex].GetComponent<RectTransform>().localScale = new Vector3(size, size, size);

            // ステージの座標と各ステージの中心点の座標の差を計算
            var stageDirection = Mathf.Abs(Mathf.Abs(stage.GetComponent<RectTransform>().localPosition.x) - stageNamePos[i]);

            // 各ステージ名オブジェクトの距離を判定、sumより小さければsumを書き換える
            if (Mathf.Abs(sum) > stageDirection)
            {
                sum = stageDirection;
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    /// 出撃ボタンを押したときの処理
    /// </summary>
    public void TapReadyButton()
    {
        // BGMをストップする
        SoundManager.smInstance.StopBGM();

        // SEを再生
        AudioClip clip = LoadSE("se_battle_start");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);

        // 押されステージのIDを保存
        PlayerPrefs.SetInt(Common.KEY_STAGE_ID, stageSpaceIndex);
        PlayerPrefs.Save();

        // オブジェクトを生成する
        print("再帰していないコルーチンの処理開始");
        StartCoroutine("ObjectGenerat");
        print("再帰していないコルーチンの処理終了");

        //if (stageSpaceIndex == 0)
        //{
        //    // 押されステージのIDを保存
        //    PlayerPrefs.SetInt(Common.KEY_STAGE_ID, stageSpaceIndex);
        //    PlayerPrefs.Save();

        //    // オブジェクトを生成する
        //    print("再帰していないコルーチンの処理開始");
        //    StartCoroutine("ObjectGenerat");
        //    print("再帰していないコルーチンの処理終了");
        //}
    }

    /// <summary>
    /// テキストとシーンチェンジの黒いやつを指定した秒数待ってから表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator ObjectGenerat()
    {
        // 0.25秒待って以降の処理をする
        yield return new WaitForSeconds(0.25f);

        if (battleTextIndex < battleObjects.Length)
        {
            battleTextIndex++;
            DispBattleText();
            // 5回自分自身を読み込む
            print(battleTextIndex + "回目再帰開始");
            StartCoroutine("ObjectGenerat");
            print(battleTextIndex + "回目再帰終了");
        }
        else
        {
            Invoke("SceneChangeGenerat", 0.25f);
        }
    }

    /// <summary>
    /// 戦闘開始のテキストを表示する
    /// </summary>
    void DispBattleText()
    {
        var battleText = Resources.Load("Prefabs/BattleTexts/BattleText" + battleTextIndex);
        battleObjects[battleTextIndex - 1] = Instantiate(battleText, battleTextObject.transform);
        battleObjects[battleTextIndex - 1].GetComponent<Animator>().SetTrigger("StartAnimation");
    }

    /// <summary>
    /// タイミングを合わせるためにInvokeして使う
    /// </summary>
    void SceneChangeGenerat()
    {
        // プレハブを読み込む
        var prefab = Resources.Load("Prefabs/scene_change_prefab");

        // シーンチェンジのスライドを生成
        var fadeObject = Instantiate(prefab, collegeStudent.transform.position, Quaternion.identity);

        // アニメーションを起動
        fadeObject.GetComponent<Animator>().SetTrigger("Fade_In");

        // 第2引数で指定した秒数待ってゲームシーンを読み込む
        Invoke("LoadGameScene", 1.0f);
    }

    /// <summary>
    /// シーンロード
    /// </summary>
    void LoadGameScene()
    {
        Common.LoadScene("GameScene");
    }

    /// <summary>
    /// SEのロード
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}
