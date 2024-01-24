using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
using System;

public class PlayerManager : MonoBehaviour
{
    // 編成は本来セレクトから読み込む予定だったが時短のため直に設定


    // 所持金
    float money;
    // お金レベル
    MoneyLevel moneyLevel;


    /// <summary>
    /// お金のレベル情報、固定値なのでconstに置きたい
    /// </summary>
    readonly MoneyInfo[] moneyInfo = new MoneyInfo[]{
        new MoneyInfo(15f, 300f,  40),
        new MoneyInfo(20f, 600f,  200),
        new MoneyInfo(30f, 1000f, 1000)
    };

    /// <summary>
    /// 編成情報、本来は読み込む
    /// </summary>
    [SerializeField] List<GameObject> formation;

    private void Awake()
    {
        // todo: 編成情報を読み込む
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
    /// 毎フレームお金を増やしていく
    /// </summary>
    void AddMoney()
    {
        // フレーム処理
        money += moneyInfo[(int)moneyLevel].GetMoneyRatio() * Time.deltaTime;
        // 財布の最大値で上限
        if (money > moneyInfo[(int)moneyLevel].GetMaxMoney())
        {
            money = moneyInfo[(int)moneyLevel].GetMaxMoney();
        }
    }

    /// <summary>
    /// 報酬獲得時の関数、↑の関数のオーバーロード
    /// </summary>
    /// <param name="_add"></param>
    public void AddMoney(float _add)
    {
        // フレーム処理
        money += _add;
        // 財布の最大値で上限
        if (money > moneyInfo[(int)moneyLevel].GetMaxMoney())
        {
            money = moneyInfo[(int)moneyLevel].GetMaxMoney();
        }
    }

    /// <summary>
    /// お金レベルアップ
    /// </summary>
    public void MoneyLevelUp()
    {
        money -= moneyInfo[(int)moneyLevel].GetLvUpMoney();
        moneyLevel++;

    }

    /// <summary>今のお金レベルのMoneyInfo型を返す</summary>
    public MoneyInfo GetMoneyInfo() { return moneyInfo[(int)moneyLevel]; }

    /// <summary>現在の所持金を返す</summary>
    public float GetMoney() { return money; }

    /// <summary>現在の所持金から差し引く</summary>
    public void UseMoney(float value) {  money -= value; }

    /// <summary>現在のお金レベルを返す</summary>
    public MoneyLevel GetMoneyLevel() { return moneyLevel; }


    /// <summary>編成情報のListを返す</summary>
    public List<GameObject> GetFormation() { return formation; }

    /// <summary>所持金を0にする</summary>
    public void ResetMoney() { money = 0; }

}
