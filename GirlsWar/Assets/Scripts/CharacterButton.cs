using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    GameObject charaPrefab;            // キャラデータ
    Friend charaFriend;
    GameManager gm;
    Generator generator;
    [SerializeField] Image charaObjImage;           // 子のキャラサムネImage
    [SerializeField] Text costText;                 // 子のキャラのコスト表示用Text

    [SerializeField] GameObject panelBlack;
    [SerializeField] Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // キャラデータ取得
        charaFriend = charaPrefab.GetComponent<Friend>();
        // 生産経過時間初期化、初期値はマックス
        charaFriend.SetRegenDeltaTime(charaFriend.GetRegenTime());
        // キャラのボタンサムネ画像
        charaObjImage.sprite = charaFriend.GerCharaThumbnail();
        // コストの表示
        costText.text = charaFriend.GetCost().ToString() + "円";
        // ジェネレーター取得
        generator = GameObject.Find("Generator").GetComponent<Generator>();
        // プレイヤー取得
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
    /// ボタン生成時に渡される
    /// </summary>
    public void SetPrefabData(GameObject _prefab)
    {
        charaPrefab = _prefab;
    }

    /// <summary>
    /// キャラ出撃ボタン押下
    /// </summary>
    public void PushCharaButton()
    {
        //print(charaPrefab.name + "のボタンが押された");
        // 生成命令
        if(generator.FriendGenerate(charaPrefab, charaFriend))
        {
            charaFriend.SetRegenDeltaTime(0);
            gm.PushCharaButtonSE();
        }
    }
}
