using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
using System;
using Unity.VisualScripting;

public class ButtonManager : MonoBehaviour
{
    #region �V���A�����ϐ�

    [SerializeField, Tooltip("�V�[���`�F���W�̎��̃t�F�[�h�A�E�g�p�̃p�l�����Z�b�g")]
    private GameObject sceneChagePanel;

    #endregion

    #region �O���ϐ�

    string sceneName;    // �V�[���̖��O���i�[
    
    float panelAlpha = 0;
    float panelDeltaAlpha = 1.0f;

    bool isButtonDown;  // �{�^���������ꂽ���ǂ���

    AudioSource audioSource;

    #endregion

    #region Unity�f�t�H���g�֐�

    private void Start()
    {
        Init();  // �O���ϐ���������
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
    /// �ϐ��Ȃǂ̏�����
    /// </summary>
    void Init()
    {
        isButtonDown = false;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// �߂�{�^�����������Ƃ��̏���
    /// �����ɂ͈ړ��������V�[���̖��O������
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
    /// PushButton�̈����ɂ��Ă��ꂽ���O�����ɃV�[����ς���
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
    /// SE�̃��[�h
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}