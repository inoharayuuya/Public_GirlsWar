using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 味方クラス、共通から派生
/// </summary>
public class Friend : CharacterBase
{
    [SerializeField] Sprite charaThumbnail; // キャラのサムネ

    [SerializeField] int ChraracterID;      // キャラクターのID
    [SerializeField] int cost;              // 生産必要金額
    [SerializeField] float regenTime;       // 再生産時間
    float regenDeltaTime;  // 生産経過時間


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
    /// 味方キャラ固有の処理をオーバーライドで追加
    /// </summary>
    override protected void Init()
    {
        base.Init();

        // 相手のグループの取得
        opponent = GameObject.Find("EnemyGroup");

        // 移動方向を反転
        sp *= -1;
        var lScale = transform.localScale;
        lScale.x *= -1;
        transform.localScale = lScale;
    }
    /// <summary>キャラのIDを返す</summary>
    public int GetID(){ return ChraracterID; }
    /// <summary>キャラのコストを返す</summary>
    public int GetCost(){ return cost; }
    /// <summary>キャラの再生成時間を返す</summary>
    public float GetRegenTime(){ return regenTime; }
    /// <summary>キャラ生成の経過時間を返す</summary>
    public float GetRegenDeltaTime(){ return regenDeltaTime; }
    /// <summary>キャラボタン用のサムネ</summary>
    public Sprite GerCharaThumbnail() { return charaThumbnail; }
    /// <summary>経過時間設定、ボタン生成時にregenTimeで初期化する用</summary>
    public void SetRegenDeltaTime(float t) {  regenDeltaTime = t; }
    /// <summary>キャラの経過時間を足す</summary>
    public void AddDeltaTime() { regenDeltaTime += Time.deltaTime; }

    protected override void Dead()
    {
        generator.friendCnt--;
        base.Dead();
    }
}