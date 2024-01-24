using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Const;


public class SoundManager : MonoBehaviour
{
    [Tooltip("�T�E���h�}�l�[�W���[�̃C���X�^���X")]
    public static SoundManager smInstance;

    [Tooltip("BGM�p�̃X�s�[�J�[")]
    public AudioSource audioSource;

    [Tooltip("���݂̃V�[��")]
    string thisSceneName;

    private void Awake()
    {

        if (smInstance == null)
        {
            // �V���O���g����
            smInstance = this;
            DontDestroyOnLoad(gameObject);


            // �f���Q�[�g�ŃC�x���g�Ɋ֐��ǉ�
            // ���̃C�x���g�̓V�[���؂�ւ����O�ɌĂ΂��
            SceneManager.sceneLoaded += SelectBGM;
            // �R���|�[�l���g�擾
            audioSource = GetComponent<AudioSource>();

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    /// <summary>
    /// BGM��p
    /// ���݂̃V�[���Ɠǂݍ��܂�鎟�̃V�[������A
    /// BGM�̐؂�ւ��𐧌�
    /// </summary>
    void SelectBGM(Scene _nextScene, LoadSceneMode _mode)
    {
        print("�V�[���̓ǂݍ��݂�����");
        print(_nextScene.name);
        // ������
        AudioClip _clip = null;
        float _vol = 0;

        // ���ɓǂݍ��܂��V�[���̖��O�ŕ���
        switch (_nextScene.name)
        {
            case Common.SCENE_NAME_TITLE:
                _clip = LoadBGM("bgm_title");
                _vol = 1f;
                break;
            case Common.SCENE_NAME_HOME:
            case Common.SCENE_NAME_SELECT:
                // ���݂̃V�[�����z�[�����Z���N�g�Ȃ�
                // BGM�̐؂�ւ����s�v�Ȃ̂ŏ������I����
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
        print("��l������");
        audioSource.clip = _clip;
        audioSource.volume = _vol;
        audioSource.loop = true;
        audioSource.Play();

        thisSceneName = _nextScene.name;
    }

    /// <summary>
    /// �t�@�C���̖��O���w�肵��Resources����ǂݍ���
    /// </summary>
    AudioClip LoadBGM(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/BGM/" + _fileName);
    }

    /// <summary>
    /// BGM��~�p
    /// </summary>
    public void StopBGM()
    {
        audioSource.Stop();
    }
}
