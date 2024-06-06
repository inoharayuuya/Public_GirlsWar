using System;
using UnityEngine;
using UnityEngine.UI;
using Const;

public class MessageButton : MonoBehaviour
{
    #region シリアル化変数

    [SerializeField, Tooltip("メッセージを表示するメッセージボックスをセット")]
    private GameObject messageBox;
    [SerializeField, Tooltip("表示するメッセージテキストファイルをセット")]
    private TextAsset txtFile;

    #endregion

    #region 外部変数

    string[,] messages;  // 二次元配列

    int index;  // 前回のindexを格納
    int genreIndex;  // ジャンルの数を格納

    AudioSource audioSource;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 変数などの初期化
    /// </summary>
    void Init()
    {
        index = -1;

        string tmpMessage = txtFile.text;

        // ジャンルごとに区切る
        string[] tmpArMsgs = tmpMessage.Split('\n', System.StringSplitOptions.None);

        // ジャンルと時間帯分の二次元配列を用意
        messages = new string[tmpArMsgs.Length, Common.TIME_SEP];

        // 時間帯ごとに区切る
        for (int i = 0; i < tmpArMsgs.Length; i++)
        {
            string[] tmp = tmpArMsgs[i].Split(',', System.StringSplitOptions.None);
            
            for (int j = 0; j < Common.TIME_SEP; j++)
            {
                messages[i, j] = tmp[j];
                messages[i, j] = messages[i, j].Replace('_', '\n');
            }
        }

        // ジャンルの配列の長さを取得
        genreIndex = tmpArMsgs.Length;

        audioSource = gameObject.AddComponent<AudioSource>();

        // メッセージのジャンルの数をランダムに取得
        var rand = UnityEngine.Random.Range(0, genreIndex * 100);
        int tmpIndex = 0;

        if (rand < 125)
        {
            tmpIndex = 0;
        }
        else if (rand < 250)
        {
            tmpIndex = 1;
        }
        else if (rand < 375)
        {
            tmpIndex = 2;
        }
        else
        {
            tmpIndex = 3;
        }

        // 前回とメッセージのジャンルが被っている間はランダムに取得しなおす
        int count = 0;
        while (index == tmpIndex)
        {
            count++;
            print("while");
            rand = UnityEngine.Random.Range(0, genreIndex * 100);

            if (rand < 132)
            {
                tmpIndex = 0;
            }
            else if (rand < 264)
            {
                tmpIndex = 1;
            }
            else if (rand < 396)
            {
                tmpIndex = 2;
            }
            else
            {
                tmpIndex = 3;
            }

            if (count > 100) { return; }
        }

        // 今回のメッセージの番号を変数で持っておく
        index = tmpIndex;
        print("tmpIndex:" + tmpIndex);

        // 現在の時間を取得して時間帯ごとに番号を変える
        int nowHour = DateTime.Now.Hour;
        print("現在の時刻：" + nowHour);
        //nowHour = 13;  // テスト
        var timeIndex = 0;
        if (4 <= nowHour && nowHour < 12)        // 4時から11時まで朝の時間帯
        {
            timeIndex = 0;
        }
        else if (12 <= nowHour && nowHour < 18)  // 12時から17時まで昼の時間帯
        {
            timeIndex = 1;
        }
        else                                     // それ以外が夜の時間帯
        {
            timeIndex = 2;
        }

        // メッセージボックスにテキストをセット
        messageBox.GetComponentInChildren<Text>().text = messages[tmpIndex, timeIndex];
    }

    /// <summary>
    /// メッセージボックスを押したときの処理
    /// </summary>
    public void PushMessageButton()
    {
        // メッセージボックスのアニメーションを再生
        messageBox.GetComponent<Animator>().SetTrigger("Action");

        AudioClip clip = LoadSE("se_button");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);

        // メッセージのジャンルの数をランダムに取得
        var rand = UnityEngine.Random.Range(0, genreIndex * 100);
        int tmpIndex = 0;

        if (rand < 125)
        {
            tmpIndex = 0;
        }
        else if (rand < 250)
        {
            tmpIndex = 1;
        }
        else if (rand < 375)
        {
            tmpIndex = 2;
        }
        else
        {
            tmpIndex = 3;
        }

        // 前回とメッセージのジャンルが被っている間はランダムに取得しなおす
        int count = 0;
        while (index == tmpIndex)
        {
            count++;
            print("while");
            rand = UnityEngine.Random.Range(0, genreIndex * 100);

            if (rand < 132)
            {
                tmpIndex = 0;
            }
            else if (rand < 264)
            {
                tmpIndex = 1;
            }
            else if (rand < 396)
            {
                tmpIndex = 2;
            }
            else
            {
                tmpIndex = 3;
            }

            if (count > 100) { return; }
        }

        // 今回のメッセージの番号を変数で持っておく
        index = tmpIndex;
        print("tmpIndex:" + tmpIndex);

        // 現在の時間を取得して時間帯ごとに番号を変える
        int nowHour = DateTime.Now.Hour;
        print("現在の時刻：" + nowHour);
        //nowHour = 13;  // テスト
        var timeIndex = 0;
        if (4 <= nowHour && nowHour < 12)        // 4時から11時まで朝の時間帯
        {
            timeIndex = 0;
        }
        else if (12 <= nowHour && nowHour < 18)  // 12時から17時まで昼の時間帯
        {
            timeIndex = 1;
        }
        else                                     // それ以外が夜の時間帯
        {
            timeIndex = 2;
        }

        // メッセージボックスにテキストをセット
        messageBox.GetComponentInChildren<Text>().text = messages[tmpIndex, timeIndex];
    }

    /// <summary>
    /// SEのロード
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}
