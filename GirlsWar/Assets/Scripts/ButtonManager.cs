using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
using System;
using Unity.VisualScripting;

public class ButtonManager : MonoBehaviour
{
    #region シリアル化変数

    [SerializeField, Tooltip("シーンチェンジの時のフェードアウト用のパネルをセット")]
    private GameObject sceneChagePanel;

    #endregion

    #region 外部変数

    string sceneName;    // シーンの名前を格納
    
    float panelAlpha = 0;
    float panelDeltaAlpha = 1.0f;

    bool isButtonDown;  // ボタンが押されたかどうか

    AudioSource audioSource;

    #endregion

    #region Unityデフォルト関数

    private void Start()
    {
        Init();  // 外部変数を初期化
    }

    private void Update()
    {
        if (isButtonDown)
        {
            ChangeScene();
        }
    }

    #endregion

    /// <summary>
    /// 変数などの初期化
    /// </summary>
    void Init()
    {
        isButtonDown = false;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 戻るボタンを押したときの処理
    /// 引数には移動したいシーンの名前を入れる
    /// </summary>
    public void PushButton(string _sceneName)
    {
        isButtonDown = true;
        sceneName = _sceneName;
        AudioClip clip = LoadSE("se_button2");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// PushButtonの引数にしてされた名前を元にシーンを変える
    /// </summary>
    void ChangeScene()
    {
        sceneChagePanel.SetActive(true);
        var c = sceneChagePanel.GetComponent<Image>().color;
        if (c.a >= 1)
        {
            Common.LoadScene(sceneName);
        }
        c.a = panelAlpha;
        sceneChagePanel.GetComponent<Image>().color = c;
        panelAlpha += panelDeltaAlpha * Time.deltaTime;
    }

    /// <summary>
    /// SEのロード
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}