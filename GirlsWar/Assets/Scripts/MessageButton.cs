using System;
using UnityEngine;
using UnityEngine.UI;
using Const;

public class MessageButton : MonoBehaviour
{
    #region �V���A�����ϐ�

    [SerializeField, Tooltip("���b�Z�[�W��\�����郁�b�Z�[�W�{�b�N�X���Z�b�g")]
    private GameObject messageBox;
    [SerializeField, Tooltip("�\�����郁�b�Z�[�W�e�L�X�g�t�@�C�����Z�b�g")]
    private TextAsset txtFile;

    #endregion

    #region �O���ϐ�

    string[,] messages;  // �񎟌��z��

    int index;  // �O���index���i�[
    int genreIndex;  // �W�������̐����i�[

    AudioSource audioSource;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// �ϐ��Ȃǂ̏�����
    /// </summary>
    void Init()
    {
        index = -1;

        string tmpMessage = txtFile.text;

        // �W���������Ƃɋ�؂�
        string[] tmpArMsgs = tmpMessage.Split('\n', System.StringSplitOptions.None);

        // �W�������Ǝ��ԑѕ��̓񎟌��z���p��
        messages = new string[tmpArMsgs.Length, Common.TIME_SEP];

        // ���ԑт��Ƃɋ�؂�
        for (int i = 0; i < tmpArMsgs.Length; i++)
        {
            string[] tmp = tmpArMsgs[i].Split(',', System.StringSplitOptions.None);
            
            for (int j = 0; j < Common.TIME_SEP; j++)
            {
                messages[i, j] = tmp[j];
                messages[i, j] = messages[i, j].Replace('_', '\n');
            }
        }

        // �W�������̔z��̒������擾
        genreIndex = tmpArMsgs.Length;

        audioSource = gameObject.AddComponent<AudioSource>();

        // ���b�Z�[�W�̃W�������̐��������_���Ɏ擾
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

        // �O��ƃ��b�Z�[�W�̃W������������Ă���Ԃ̓����_���Ɏ擾���Ȃ���
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

        // ����̃��b�Z�[�W�̔ԍ���ϐ��Ŏ����Ă���
        index = tmpIndex;
        print("tmpIndex:" + tmpIndex);

        // ���݂̎��Ԃ��擾���Ď��ԑт��Ƃɔԍ���ς���
        int nowHour = DateTime.Now.Hour;
        print("���݂̎����F" + nowHour);
        //nowHour = 13;  // �e�X�g
        var timeIndex = 0;
        if (4 <= nowHour && nowHour < 12)        // 4������11���܂Œ��̎��ԑ�
        {
            timeIndex = 0;
        }
        else if (12 <= nowHour && nowHour < 18)  // 12������17���܂Œ��̎��ԑ�
        {
            timeIndex = 1;
        }
        else                                     // ����ȊO����̎��ԑ�
        {
            timeIndex = 2;
        }

        // ���b�Z�[�W�{�b�N�X�Ƀe�L�X�g���Z�b�g
        messageBox.GetComponentInChildren<Text>().text = messages[tmpIndex, timeIndex];
    }

    /// <summary>
    /// ���b�Z�[�W�{�b�N�X���������Ƃ��̏���
    /// </summary>
    public void PushMessageButton()
    {
        // ���b�Z�[�W�{�b�N�X�̃A�j���[�V�������Đ�
        messageBox.GetComponent<Animator>().SetTrigger("Action");

        AudioClip clip = LoadSE("se_button");
        audioSource.volume = 0.25f;
        audioSource.PlayOneShot(clip);

        // ���b�Z�[�W�̃W�������̐��������_���Ɏ擾
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

        // �O��ƃ��b�Z�[�W�̃W������������Ă���Ԃ̓����_���Ɏ擾���Ȃ���
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

        // ����̃��b�Z�[�W�̔ԍ���ϐ��Ŏ����Ă���
        index = tmpIndex;
        print("tmpIndex:" + tmpIndex);

        // ���݂̎��Ԃ��擾���Ď��ԑт��Ƃɔԍ���ς���
        int nowHour = DateTime.Now.Hour;
        print("���݂̎����F" + nowHour);
        //nowHour = 13;  // �e�X�g
        var timeIndex = 0;
        if (4 <= nowHour && nowHour < 12)        // 4������11���܂Œ��̎��ԑ�
        {
            timeIndex = 0;
        }
        else if (12 <= nowHour && nowHour < 18)  // 12������17���܂Œ��̎��ԑ�
        {
            timeIndex = 1;
        }
        else                                     // ����ȊO����̎��ԑ�
        {
            timeIndex = 2;
        }

        // ���b�Z�[�W�{�b�N�X�Ƀe�L�X�g���Z�b�g
        messageBox.GetComponentInChildren<Text>().text = messages[tmpIndex, timeIndex];
    }

    /// <summary>
    /// SE�̃��[�h
    /// </summary>
    AudioClip LoadSE(string _fileName)
    {
        return (AudioClip)Resources.Load("Sounds/SE/" + _fileName);
    }
}
