using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵味方共通クラス
/// </summary>
public class CharacterBase : MonoBehaviour
{
    [SerializeField] public bool isCastle;

    [SerializeField] GameObject hitEffectObj;

    #region 変数
    /// <summary>
    /// 攻撃対象のキャラの親オブジェクト
    /// </summary>
    protected GameObject opponent;                  // 敵のオブジェクトグループ

    [SerializeField] CharacterState state;          // アニメーション用の状態変数
    Animator animator;                              // アニメーターコンポーネント
    protected Generator generator;
    protected PlayerManager pm;
    protected GameManager gm;

    [SerializeField] protected int maxHp;           // キャラの最大体力、ノックバック計算で使用
    [SerializeField] protected int hp;                        // キャラの実際の体力
    [SerializeField] protected int atk;             // キャラの攻撃力
    [SerializeField] protected int def;             // キャラの守備力
    [SerializeField] protected float sp;            // キャラの移動速度
    [SerializeField] protected int maxKnockBack;    // キャラのノックバック最大回数
    float knockBackDamage;                          // １ノックバックするダメージ量
    int knockBackCnt;                               // 実際にノックバックした回数

    [SerializeField] protected float range;         // キャラの攻撃射程距離
    [SerializeField] protected float interval;      // キャラの攻撃間隔
    protected float attackDeltatime;                // 攻撃間隔の管理用経過時間, intervalで初期化

    protected bool isDead;                          // 死んでるか
    [SerializeField] protected bool isMultiAttack;  // 範囲攻撃かどうか

    #endregion


    #region 関数

    /// <summary>
    /// 初期化処理
    /// </summary>
    virtual protected void Init()
    {
        if(!isCastle)
        {
            // コンポーネント取得
            animator = GetComponent<Animator>();
            generator = GameObject.Find("Generator").GetComponent<Generator>();
            pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            // 変数の初期化
            attackDeltatime = interval;
            isDead = false;
            state = CharacterState.IDLE;

            knockBackCnt = 0;
            knockBackDamage = maxHp / maxKnockBack;
        }

        hp = maxHp;

    }

    /// <summary>
    /// 移動処理
    /// </summary>
    public void Move()
    {
        // 移動速度を参照して移動
        transform.position += new Vector3(sp, 0, 0) * Time.deltaTime;
        state = CharacterState.RUN;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    public void Attack()
    {
        // 攻撃対象がいるので攻撃間隔を参照して攻撃するかどうか判定
        if (attackDeltatime > interval)
        {
            // 攻撃
            state = CharacterState.ATTACK;
        }
        else
        {
            state = CharacterState.IDLE;
        }
    }

    /// <summary>
    /// ダメージを受けた時の処理
    /// 攻撃側が被攻撃側のこの関数を呼び出す
    /// </summary>
    public void Damage(int a)
    {

        // ノックバック中なら無敵
        if (state == CharacterState.HURT)
        {
            return;
        }
        // 防御力の計算
        int dmg = a - def;

        // 回復しないように
        if (dmg < 0)
        {
            dmg = 0;
        }

        // 実際のHPに代入
        hp -= dmg;

        // 城の場合ここまででreturn
        if(isCastle)
        {
            return;
        }

        // 累積ダメージを計算
        var totalDamage = maxHp - hp;
        // 累積ダメージから正しいノックバック回数を算出
        var newKnockBackCnt = (int)(totalDamage / knockBackDamage);

        // 元のノックバック回数とのずれがあるならノックバックを行う
        if (knockBackCnt < newKnockBackCnt)
        {
            // ノックバック回数の修正
            knockBackCnt = newKnockBackCnt;
            // ノックバックアニメーションを呼ぶ
            state = CharacterState.HURT;
            print("ノックバックした");
        }

        var pos = new Vector3(0, 2f, 0) + transform.position;
        Instantiate(hitEffectObj, transform.position, Quaternion.identity, transform);
    }

    public int GetMaxHP() {  return maxHp; }




    /// <summary>
    /// 攻撃射程距離をみて範囲内に敵がいるなら移動を止めるためfalseを返す
    /// </summary>
    public bool RangeCheckMove()
    {
        // 初期は動ける前提
        bool flg = true;
        // 攻撃対象のオブジェクト分ループ
        foreach (Transform child in opponent.transform)
        {
            // 死んでいる奴とActiveでない奴は無視
            if (child.gameObject.GetComponent<CharacterBase>().isDead || 
                child.gameObject.activeSelf == false)
            {
                continue;
            }
            
            // 座標の差を取得
            // 自陣キャラの反転に対応するためlocalScaleを計算に入れる
            var ls = transform.localScale.x >= 0 ? 1 : -1;
            var diffPosX = (child.position.x - transform.position.x) * ls;

            // 射程距離内にいるかつ、自分の前方にいるなら攻撃範囲内
            if (diffPosX < range && diffPosX >= 0)
            {
                flg = false;
                break;
            }
        }

        return flg;
    }


    /// <summary>
    /// 攻撃射程距離を見て範囲内に敵がいるなら範囲内のキャラのhpを減らす
    /// </summary>
    /// <returns></returns>
    public void RangeCheckAttack()
    {
        // 攻撃対象リスト
        List<GameObject> list = new List<GameObject>();
        // 攻撃対象グループの子を全検索
        foreach (Transform child in opponent.transform)
        {
            // 死んでいる奴とActiveでない奴は無視
            if (child.gameObject.GetComponent<CharacterBase>().isDead || 
                child.gameObject.activeSelf == false)
            {
                continue;
            }

            // 座標の差を取得
            // 敵味方共通クラスの処理なので
            // 自陣キャラの反転に対応するためlocalScaleを計算に入れる
            var ls = transform.localScale.x >= 0 ? 1 : -1;
            var diffPosX = (child.position.x - transform.position.x) * ls;

            // 射程距離内にいるかつ、自分の前方にいるなら攻撃範囲内、リストに追加
            if (diffPosX <= range && diffPosX >= 0)
            {
                list.Add(child.gameObject);
            }

        }

        // 範囲攻撃か単体攻撃で処理分け

        // 攻撃対象全員にダメージ
        if (isMultiAttack)
        {
            foreach (GameObject child in list)
            {
                
                child.GetComponent<CharacterBase>().Damage(atk);
            }
        }
        // もっとも距離が近いものにのみダメージ
        else
        {
            float target = float.MaxValue;
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var ls = transform.localScale.x >= 0 ? 1 : -1;
                var diffPosX = (list[i].transform.position.x - transform.position.x) * ls;
                if (target > diffPosX)
                {
                    target = diffPosX;
                    index = i;
                }
            }
            // 例外は攻撃失敗
            if (list.Count != 0 && index != -1)
            {
                list[index].GetComponent<CharacterBase>().Damage(atk);
            }
        }

        gm.CharaAttackSE();
    }

    /// <summary>
    /// 攻撃アニメーションが終わった後に呼ばれる関数
    /// 攻撃アニメーションイベントで呼び出し
    /// </summary>
    void EndAttackMotion()
    {
        attackDeltatime = 0;
        state = CharacterState.IDLE;
    }


    /// <summary>
    /// 被ダメージ（ノックバック）アニメーション終了時に呼ばれる関数
    /// ノックバックアニメーションイベントで呼び出し
    /// </summary>
    void EndHurtMotion()
    {
        // 死亡判定
        if(hp <= 0)
        {
            isDead = true;
            state = CharacterState.DEAD;
        }
        else
        {
            state = CharacterState.IDLE;
        }
        
    }

    /// <summary>
    /// 死んだときの処理
    /// 死亡アニメーション終了時に、死亡アニメーションイベントから呼び出し
    /// </summary>
    void EndDeadMotion()
    {
        //Invoke("Dead", 0.5f);
        Dead();
    }
    /// <summary>
    /// オブジェクト破棄
    /// </summary>
    virtual protected void Dead()
    {
        Destroy(gameObject);
    }


    /// <summary>
    /// stateを参照してアニメーションの処理
    /// </summary>
    void Animation()
    {
        switch (state)
        {
            // 攻撃待機
            case CharacterState.IDLE:
                animator.SetTrigger("Idle");
                break;
            // 移動
            case CharacterState.RUN:
                animator.SetTrigger("Run");
                break;
            // 攻撃
            case CharacterState.ATTACK:
                animator.SetTrigger("Attack");
                break;
            // 被ダメージ（ノックバック）
            case CharacterState.HURT:
                animator.SetTrigger("Hurt");
                break;
            // 死亡
            case CharacterState.DEAD:
                animator.SetTrigger("Die");
                break;
        }
    }


    #endregion

    // 敵味方共通のUpdate
    virtual protected void Update()
    {
        if (!isCastle)
        {
            // 前回の攻撃からの経過時間を記録、攻撃判定に使用
            attackDeltatime += Time.deltaTime;

            // 死んでおらず、ノックバックもしてない時のみ行動可能
            if (state != CharacterState.DEAD && state != CharacterState.HURT)
            {
                // 移動できるか判定を行う
                if (RangeCheckMove())
                {
                    if(state != CharacterState.ATTACK)
                    {
                        // 移動
                        Move();
                    }

                }
                else
                {
                    // 攻撃の処理に入る
                    Attack();
                }
            }

            // アニメーション制御
            Animation();
        }
        
    }

    public int GetHP()
    {
        return hp;
    }

}
