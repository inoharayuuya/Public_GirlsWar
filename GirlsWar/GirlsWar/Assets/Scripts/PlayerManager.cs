using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
using System;

public class PlayerManager : MonoBehaviour
{
    // �Ґ��͖{���Z���N�g����ǂݍ��ޗ\�肾���������Z�̂��ߒ��ɐݒ�


    // ������
    float money;
    // �������x��
    MoneyLevel moneyLevel;


    /// <summary>
    /// �����̃��x�����A�Œ�l�Ȃ̂�const�ɒu������
    /// </summary>
    readonly MoneyInfo[] moneyInfo = new MoneyInfo[]{
        new MoneyInfo(15f, 300f,  40),
        new MoneyInfo(20f, 600f,  200),
        new MoneyInfo(30f, 1000f, 1000)
    };

    /// <summary>
    /// �Ґ����A�{���͓ǂݍ���
    /// </summary>
    [SerializeField] List<GameObject> formation;

    private void Awake()
    {
        // todo: �Ґ�����ǂݍ���
    }


    // Start is called before the first frame update
    void Start()
    {
        moneyLevel = MoneyLevel.LEVEL1;
        money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        AddMoney();
    }

    /// <summary>
    /// ���t���[�������𑝂₵�Ă���
    /// </summary>
    void AddMoney()
    {
        // �t���[������
        money += moneyInfo[(int)moneyLevel].GetMoneyRatio() * Time.deltaTime;
        // ���z�̍ő�l�ŏ��
        if (money > moneyInfo[(int)moneyLevel].GetMaxMoney())
        {
            money = moneyInfo[(int)moneyLevel].GetMaxMoney();
        }
    }

    /// <summary>
    /// ��V�l�����̊֐��A���̊֐��̃I�[�o�[���[�h
    /// </summary>
    /// <param name="_add"></param>
    public void AddMoney(float _add)
    {
        // �t���[������
        money += _add;
        // ���z�̍ő�l�ŏ��
        if (money > moneyInfo[(int)moneyLevel].GetMaxMoney())
        {
            money = moneyInfo[(int)moneyLevel].GetMaxMoney();
        }
    }

    /// <summary>
    /// �������x���A�b�v
    /// </summary>
    public void MoneyLevelUp()
    {
        money -= moneyInfo[(int)moneyLevel].GetLvUpMoney();
        moneyLevel++;

    }

    /// <summary>���̂������x����MoneyInfo�^��Ԃ�</summary>
    public MoneyInfo GetMoneyInfo() { return moneyInfo[(int)moneyLevel]; }

    /// <summary>���݂̏�������Ԃ�</summary>
    public float GetMoney() { return money; }

    /// <summary>���݂̏��������獷������</summary>
    public void UseMoney(float value) {  money -= value; }

    /// <summary>���݂̂������x����Ԃ�</summary>
    public MoneyLevel GetMoneyLevel() { return moneyLevel; }


    /// <summary>�Ґ�����List��Ԃ�</summary>
    public List<GameObject> GetFormation() { return formation; }

    /// <summary>��������0�ɂ���</summary>
    public void ResetMoney() { money = 0; }

}
