using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Const
{
    /// <summary>
    /// �Q�[���̏��
    /// �����A�����A������
    /// </summary>
    public enum GameState
    {
        WIN,
        LOSE,
        GAMING
    }


    /// <summary>
    /// �X�e�[�W�̔ԍ�
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
    /// �������x��
    /// PlayerManager��moneyInfo�z��������ɑ��₷
    /// </summary>
    public enum MoneyLevel
    {
        LEVEL1,
        LEVEL2,
        LEVEL3
    }

    /// <summary>
    /// �G�L������ID.
    /// �L�����z��ɒǉ�����񋓑��₵�Ă���
    /// </summary>
    public enum EnemyID
    {
        NORMAL,
        LIGHTNING,
        FIRE
    }

    /// <summary>
    /// �X�e�[�W��ID
    /// �X�e�[�W��ǉ����邲�Ƃɑ��₵�Ă���
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
        /// �V�[���ǂݍ���
        /// </summary>
        /// <param name="_sceneName"></param>
        public static void LoadScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
        }

        public const float SCREEN_WIDTH_TAB    = 1920.0f;  // �X�N���[����width
        public const float SCREEN_HEIGHT_PHONE = 720.0f;   // �X�N���[����height
        public const float SCREEN_WIDTH_PHONE  = 1520.0f;  // �X�N���[����height
        public const float SCREEN_HEIGHT_TAB   = 1200.0f;  // �X�N���[����height
        public const float CASTLE_WIDTH        = 2.0f;     // ���width


        public const string SCENE_NAME_TITLE = "Title";
        public const string SCENE_NAME_HOME = "HomeScene";
        public const string SCENE_NAME_SELECT = "StageSelectScene";
        public const string SCENE_NAME_GAME = "GameScene";



        public const float SS_START_POS_TIME  = 0.1f;    // startPos�̍X�V�p�x
        public const float SS_SLOW_DOWN_SPEED = 0.97f;   // ������
        public const float SS_INIT_SPEED      = 0.5f;    // �X�s�[�h�̏����l
        public const float SS_INIT_SUM        = 10000f;  // sum�̏����l
        public const float SS_MIN_SPEED       = 0.05f;   // �X�s�[�h�̍ŏ��l
        public const float SS_DIRECTION_SPEED = 1000f;   // �X�e�[�W���I�u�W�F�N�g���X�e�[�W���̒��S�܂œ��������̃X�s�[�h
        public const float SS_CHARA_SPEED     = 2.5f;    // �L�����N�^�[�̈ړ��X�s�[�h
        public const float SS_CAMERA_SPEED    = 10f;     // �J�����̈ړ��X�s�[�h
        public const float SS_CIRCLE_SPACE    = 0.5f;    // ��������circle�̊Ԋu
        public const int   SS_MAX_STAGE       = 4;       // �}�b�N�X�̃X�e�[�W��
        public const int   SS_RELEASE_STAGE   = 4;       // �����ł��Ă���X�e�[�W��

        public const float CC_SPEED           = 150.0f;  // �J�����̃s���`�C���A�s���`�A�E�g���X�s�[�h
        public const float CC_INIT_SPEED      = 0.5f;    // �J�����ړ��ɂ�����X�s�[�h�̏����l
        public const float CC_START_POS_TIME  = 0.1f;    // startPos�̍X�V�p�x
        public const float CC_SLOW_DOWN_SPEED = 0.98f;   // ������
        public const float CC_SPACE           = 0.3f;    // ��ʂ̉��̒n�ʕ����̃X�y�[�X

        public const int TIME_SEP = 3;  // ���ԑѕ����⃋�[�v�p

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
    /// �������x���̏��
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
        /// <summary>�P�b�ɑ������邨���̗ʂ�Ԃ�</summary>
        public float GetMoneyRatio() { return moneyRatio; }
        /// <summary>���z�̍ő�e��</summary>
        public float GetMaxMoney() { return maxMoney; }
        /// <summary>���x���A�b�v�ɕK�v�ȋ��z��Ԃ�</summary>
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
        /// �N���A���b�ォ��A���b���ƂɁA����
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
        /// �G�̏��HP�A���ꂼ��̏�̊Ԃ̋����A���̃X�e�[�W�ɏo�Ă���G�̏��(�z��)
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



