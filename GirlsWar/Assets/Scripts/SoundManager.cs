using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Const;


public class SoundManager : MonoBehaviour
{
    [Tooltip("サウンドマネージャーのインスタンス")]
    public static SoundManager smInstance;

    [Tooltip("BGM用のスピーカー")]
    public AudioSource audioSource;

    [Tooltip("現在のシーン")]
    string thisSceneName;

    private void Awake()
    {

        if (smInstance == null)
        {
            // シングルトン化
            smInstance = this;
            DontDestroyOnLoad(gameObject);


            // デリゲートでイベントに関数追加
            // このイベントはシーン切り替え直前に呼ばれる
            SceneManager.sceneLoaded += SelectBGM;
            // コンポーネント取得
            audioSource = GetComponent<AudioSource>();

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    /// <summary>
    /// BGM専用
    /// 現在のシーンと読み込まれる次のシーンから、
    /// BGMの切り替えを制御
    /// </summary>
    void SelectBGM(Scene _nextScene, LoadSceneMode _mode)
    {
        print("シーンの読み込みしたよ");
        print(_nextScene.name);
        // 初期化
        AudioClip _clip = null;
        float _vol = 0;

        // 次に読み込まれるシーンの名前で分岐
        switch (_nextScene.name)
        {
            case Common.SCENE_NAME_TITLE:
                _clip = LoadBGM("bgm_title");
                _vol = 1f;
                break;
            case Common.SCENE_NAME_HOME:
            case Common.SCENE_NAME_SELECT:
                // 現在のシーンがホームかセレクトなら
                // BGMの切り替えが不要なので処理を終える
                if (thisSceneName == Common.SCENE_NAME_HOME || thisSceneName == Common.SCENE_NAME_SELECT)
                {
                    thisSceneName = _nextScene.name;
                    return;
                }
                _clip = LoadBGM("bgm_home_select");
                _vol = 0.3f;
                break;
            case Common.SCENE_NAME_GAME:
                _clip = LoadBGM("bgm_game");
                _vol = 0.3f;
                break;
        }
        print("大人がすよ");
        audioSource.clip = _clip;
        audioSource.volume = _vol;
        audioSource.loop = true;
        audioSource.Play();

        thisSceneName = _nextScene.name;
    }

    /// <summary>
    /// ファイルの名前を指定してResourcesから読み込む
    /// </summary>
    AudioClip LoadBGM(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/BGM/" + _fileName);
    }

    /// <summary>
    /// BGM停止用
    /// </summary>
    public void StopBGM()
    {
        audioSource.Stop();
    }
}
