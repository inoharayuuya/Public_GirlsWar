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
    /// �G�L�����ŗL�̏������I�[�o�[���C�h�Œǉ�
    /// </summary>
    override protected void Init()
    {
        base.Init();
        // ����̃O���[�v�̎擾
        opponent = GameObject.Find("FriendGroup");
    }

    override protected void Dead()
    {
        generator.enemyCnt--;
        pm.AddMoney(reward);
        print("�G��|���ĕ�V�l��");
        base.Dead();
    }

    /// <summary>
    /// �G�̏��HP������
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
