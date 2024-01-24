using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] GameObject castleFriendObj;
    [SerializeField] GameObject castleEnemyObj;
    [SerializeField] GameObject friendGroup;
    [SerializeField] GameObject enemyGroup;
    PlayerManager pm;
    GameManager gm;


    int maxFriendNum = 10;
    int maxEnemyNum = 30;

    public int friendCnt;
    public int enemyCnt;

    private void Start()
    {
        pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        friendCnt = 0;
        enemyCnt = 0;
    }

    private void Update()
    {
        if (maxFriendNum > friendCnt)
        {
            gm.DeleteErrorMsg();
        }
        else
        {
            gm.DispErrorMsg("これ以上出撃できません！");
        }
    }

    /// <summary>
    /// キャラ生成関数、キャラのPrefabとキャラのデータを渡して生成
    /// 生成可能時にtrueを返してボタン側で処理
    /// </summary>
    public bool FriendGenerate(GameObject _prefab, Friend _charaData)
    {
        bool ret;
        // 生成可能な金額がたまっている&生成のクールタイムを過ぎている
        if ((pm.GetMoney() >= _charaData.GetCost()) &&
            (_charaData.GetRegenDeltaTime() >= _charaData.GetRegenTime()) &&
            maxFriendNum > friendCnt
            )
        {
            ret = true;
            pm.UseMoney(_charaData.GetCost());
            friendCnt++;
            Instantiate(_prefab, ShiftPosition(castleFriendObj.transform.position), Quaternion.identity, friendGroup.transform);
        }
        else
        {
            ret = false;
        }

        return ret;

    }

    public bool CanGen(GameObject _prefab, Friend _charaData)
    {
        bool ret;
        // 生成可能な金額がたまっている&生成のクールタイムを過ぎている
        if ((pm.GetMoney() >= _charaData.GetCost()) &&
            (_charaData.GetRegenDeltaTime() >= _charaData.GetRegenTime())
            )
        {
            ret = true;
        }
        else
        {
            ret = false;
        }

        return ret;

    }

    public bool EnemyGenrate(GameObject _prefab)
    {
        if(gm.GetGameState() != GameState.GAMING)
        {
            return false;
        }
        bool ret;
        if (maxEnemyNum > enemyCnt)
        {
            ret = true;
            // 敵の生成
            enemyCnt++;
            Instantiate(_prefab, ShiftPosition(castleEnemyObj.transform.position), Quaternion.identity, enemyGroup.transform);
        }
        else
        {
            ret = false;
        }

        return ret;
    }

    Vector3 ShiftPosition(Vector3 _pos)
    {
        float range = 0.2f;
        float rnd =  Random.Range(-range, range);



        return new Vector3(_pos.x, _pos.y + rnd, _pos.z);
    }
}
