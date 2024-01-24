using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    [SerializeField] float reward;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 敵キャラ固有の処理をオーバーライドで追加
    /// </summary>
    override protected void Init()
    {
        base.Init();
        // 相手のグループの取得
        opponent = GameObject.Find("FriendGroup");
    }

    override protected void Dead()
    {
        generator.enemyCnt--;
        pm.AddMoney(reward);
        print("敵を倒して報酬獲得");
        base.Dead();
    }

    /// <summary>
    /// 敵の城のHP初期化
    /// </summary>
    public void CastleInit(int _hp)
    {
        if(isCastle)
        {

            hp = _hp;
            maxHp = _hp;
        }
    }

}
