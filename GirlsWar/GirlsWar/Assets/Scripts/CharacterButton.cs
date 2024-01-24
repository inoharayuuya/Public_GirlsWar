using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    GameObject charaPrefab;            // �L�����f�[�^
    Friend charaFriend;
    GameManager gm;
    Generator generator;
    [SerializeField] Image charaObjImage;           // �q�̃L�����T���lImage
    [SerializeField] Text costText;                 // �q�̃L�����̃R�X�g�\���pText

    [SerializeField] GameObject panelBlack;
    [SerializeField] Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // �L�����f�[�^�擾
        charaFriend = charaPrefab.GetComponent<Friend>();
        // ���Y�o�ߎ��ԏ������A�����l�̓}�b�N�X
        charaFriend.SetRegenDeltaTime(charaFriend.GetRegenTime());
        // �L�����̃{�^���T���l�摜
        charaObjImage.sprite = charaFriend.GerCharaThumbnail();
        // �R�X�g�̕\��
        costText.text = charaFriend.GetCost().ToString() + "�~";
        // �W�F�l���[�^�[�擾
        generator = GameObject.Find("Generator").GetComponent<Generator>();
        // �v���C���[�擾
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        charaFriend.AddDeltaTime();

        if(generator.CanGen(charaPrefab, charaFriend))
        {
            panelBlack.SetActive(false);
        }
        else
        {
            panelBlack.SetActive(true);
        }

        slider.value = charaFriend.GetRegenDeltaTime() / charaFriend.GetRegenTime();
        if(slider.value >= 1)
        {
            slider.gameObject .SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// �{�^���������ɓn�����
    /// </summary>
    public void SetPrefabData(GameObject _prefab)
    {
        charaPrefab = _prefab;
    }

    /// <summary>
    /// �L�����o���{�^������
    /// </summary>
    public void PushCharaButton()
    {
        //print(charaPrefab.name + "�̃{�^���������ꂽ");
        // ��������
        if(generator.FriendGenerate(charaPrefab, charaFriend))
        {
            charaFriend.SetRegenDeltaTime(0);
            gm.PushCharaButtonSE();
        }
    }
}
