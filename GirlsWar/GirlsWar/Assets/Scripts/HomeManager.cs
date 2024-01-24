using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField, Tooltip("�L�����N�^�[�Ґ��p�l�����Z�b�g")]
    private GameObject panel;
    [SerializeField, Tooltip("�V�[���`�F���W�̎��̃t�F�[�h�A�E�g�p�̃p�l�����Z�b�g")]
    private GameObject[] sceneChagePanels;

    float panelAlpha = 0;
    float panelDeltaAlpha = 1.0f;
    
    bool isButtonDown;  // �{�^���������ꂽ���ǂ���

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isButtonDown)
        {
            ChangeScene();
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void Init()
    {
        panel.SetActive(false);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// �L�����N�^�[�Ґ��{�^�����������Ƃ��̏���
    /// </summary>
    public void PushSetPartyButton()
    {
        isButtonDown = true;
        panelAlpha = 0;
        AudioClip clip = LoadSE("se_button2");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// PushButton�̈����ɂ��Ă��ꂽ���O�����ɃV�[����ς���
    /// </summary>
    void ChangeScene()
    {
        if (panel.activeSelf)
        {
            sceneChagePanels[1].SetActive(true);
            var c = sceneChagePanels[1].GetComponent<Image>().color;
            if (c.a >= 1)
            {
                panel.SetActive(false);
                isButtonDown = false;
                sceneChagePanels[1].SetActive(false);
                c.a = 0;
                sceneChagePanels[1].GetComponent<Image>().color = c;
                return;
            }
            c.a = panelAlpha;
            sceneChagePanels[1].GetComponent<Image>().color = c;
            panelAlpha += panelDeltaAlpha * (Time.deltaTime * 2);
        }
        else
        {
            sceneChagePanels[0].SetActive(true);
            var c = sceneChagePanels[0].GetComponent<Image>().color;
            if (c.a >= 1)
            {
                panel.SetActive(true);
                isButtonDown = false;
                sceneChagePanels[0].SetActive(false);
                c.a = 0;
                sceneChagePanels[0].GetComponent<Image>().color = c;
                return;
            }
            c.a = panelAlpha;
            sceneChagePanels[0].GetComponent<Image>().color = c;
            panelAlpha += panelDeltaAlpha * (Time.deltaTime * 2);
        }

    }

    /// <summary>
    /// SE�̃��[�h
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}
