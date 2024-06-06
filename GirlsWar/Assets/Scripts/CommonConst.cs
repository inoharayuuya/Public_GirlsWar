using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Const
{
    /// <summary>
    /// ゲームの状態
    /// 勝ち、負け、勝負中
    /// </summary>
    public enum GameState
    {
        WIN,
        LOSE,
        GAMING
    }


    /// <summary>
    /// ステージの番号
    /// </summary>
    public enum StageNumber
    {
        STAGE0,
        STAGE1,
        STAGE2
    }


    public enum CharacterState
    {
        IDLE,
        RUN,
        ATTACK,
        HURT,
        DEAD
    }

    /// <summary>
    /// お金レベル
    /// PlayerManagerのmoneyInfo配列も同時に増やす
    /// </summary>
    public enum MoneyLevel
    {
        LEVEL1,
        LEVEL2,
        LEVEL3
    }

    /// <summary>
    /// 敵キャラのID.
    /// キャラ配列に追加次第列挙増やしていく
    /// </summary>
    public enum EnemyID
    {
        NORMAL,
        LIGHTNING,
        FIRE
    }

    /// <summary>
    /// ステージのID
    /// ステージを追加するごとに増やしていく
    /// </summary>
    public enum STAGE_ID
    {
        TUTORIAL,
        STAGE1,
        STAGE2,
        STAGE3
    }

    public static class Common
    {
        /// <summary>
        /// シーン読み込み
        /// </summary>
        /// <param name="_sceneName"></param>
        public static void LoadScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
        }

        public const float SCREEN_WIDTH_TAB    = 1920.0f;  // スクリーンのwidth
        public const float SCREEN_HEIGHT_PHONE = 720.0f;   // スクリーンのheight
        public const float SCREEN_WIDTH_PHONE  = 1520.0f;  // スクリーンのheight
        public const float SCREEN_HEIGHT_TAB   = 1200.0f;  // スクリーンのheight
        public const float CASTLE_WIDTH        = 2.0f;     // 城のwidth


        public const string SCENE_NAME_TITLE = "Title";
        public const string SCENE_NAME_HOME = "HomeScene";
        public const string SCENE_NAME_SELECT = "StageSelectScene";
        public const string SCENE_NAME_GAME = "GameScene";



        public const float SS_START_POS_TIME  = 0.1f;    // startPosの更新頻度
        public const float SS_SLOW_DOWN_SPEED = 0.97f;   // 減速率
        public const float SS_INIT_SPEED      = 0.5f;    // スピードの初期値
        public const float SS_INIT_SUM        = 10000f;  // sumの初期値
        public const float SS_MIN_SPEED       = 0.05f;   // スピードの最小値
        public const float SS_DIRECTION_SPEED = 1000f;   // ステージ名オブジェクトをステージ名の中心まで動かす時のスピード
        public const float SS_CHARA_SPEED     = 2.5f;    // キャラクターの移動スピード
        public const float SS_CAMERA_SPEED    = 10f;     // カメラの移動スピード
        public const float SS_CIRCLE_SPACE    = 0.5f;    // 生成するcircleの間隔
        public const int   SS_MAX_STAGE       = 4;       // マックスのステージ数
        public const int   SS_RELEASE_STAGE   = 4;       // 実装できているステージ数

        public const float CC_SPEED           = 150.0f;  // カメラのピンチイン、ピンチアウトをスピード
        public const float CC_INIT_SPEED      = 0.5f;    // カメラ移動にかけるスピードの初期値
        public const float CC_START_POS_TIME  = 0.1f;    // startPosの更新頻度
        public const float CC_SLOW_DOWN_SPEED = 0.98f;   // 減速率
        public const float CC_SPACE           = 0.3f;    // 画面の下の地面部分のスペース

        public const int TIME_SEP = 3;  // 時間帯分割やループ用

        public static readonly string[] ENEMY_NAMES = {
            "NormalSlime",
            "LightningSlime",
            "FireSlime"
        };

        public const string KEY_STAGE_ID = "stage_id";
        public const string KEY_STAGE_DATA = "stage_data";

        public const string PASSWORD = "sagae";
    }

    /// <summary>
    /// お金レベルの情報
    /// </summary>
    public class MoneyInfo
    {
        float moneyRatio;
        float maxMoney;
        int lvUpMoney;

        public MoneyInfo(float _moneyRatio, float _maxMoney, int _LvUpMoney)
        {
            moneyRatio = _moneyRatio;
            maxMoney = _maxMoney;
            lvUpMoney = _LvUpMoney;
        }
        /// <summary>１秒に増加するお金の量を返す</summary>
        public float GetMoneyRatio() { return moneyRatio; }
        /// <summary>財布の最大容量</summary>
        public float GetMaxMoney() { return maxMoney; }
        /// <summary>レベルアップに必要な金額を返す</summary>
        public int GetLvUpMoney() { return lvUpMoney; }
    }

    [Serializable]
    public class EnemyInfo
    {
        public GameObject prefab;
        public EnemyID eId;
        public float startTime;
        public float interval;
        public int num;
        public float deltaTime;
        public int cnt;

        /// <summary>
        /// 誰が、何秒後から、何秒ごとに、何体
        /// </summary>
        public EnemyInfo(EnemyID _id, float _st, float _interval, int _n)
        {
            eId = _id;
            prefab = (GameObject)Resources.Load("Prefabs/" + Common.ENEMY_NAMES[(int)_id]);
            startTime = _st;
            interval = _interval;
            num = _n;
            cnt = 0;
            deltaTime = 0;
        }
    }

    [Serializable]
    public class Stage
    {
        public int castleHp;
        //public float castleDistance;
        public bool isInstall;
        public bool isClear = false;
        public EnemyInfo[] enemyInfos;

        /// <summary>
        /// 敵の城のHP、それぞれの城の間の距離、そのステージに出てくる敵の情報(配列)
        /// </summary>
        public Stage()
        {
            castleHp = 0;
            isInstall = false;
            enemyInfos = new EnemyInfo[] { };
        }
    }

    [Serializable]
    public class Data
    {
        public Stage[] stages;
    }
}



