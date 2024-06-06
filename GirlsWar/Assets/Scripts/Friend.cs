using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �����N���X�A���ʂ���h��
/// </summary>
public class Friend : CharacterBase
{
    [SerializeField] Sprite charaThumbnail; // �L�����̃T���l

    [SerializeField] int ChraracterID;      // �L�����N�^�[��ID
    [SerializeField] int cost;              // ���Y�K�v���z
    [SerializeField] float regenTime;       // �Đ��Y����
    float regenDeltaTime;  // ���Y�o�ߎ���


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
    /// �����L�����ŗL�̏������I�[�o�[���C�h�Œǉ�
    /// </summary>
    override protected void Init()
    {
        base.Init();

        // ����̃O���[�v�̎擾
        opponent = GameObject.Find("EnemyGroup");

        // �ړ������𔽓]
        sp *= -1;
        var lScale = transform.localScale;
        lScale.x *= -1;
        transform.localScale = lScale;
    }
    /// <summary>�L������ID��Ԃ�</summary>
    public int GetID(){ return ChraracterID; }
    /// <summary>�L�����̃R�X�g��Ԃ�</summary>
    public int GetCost(){ return cost; }
    /// <summary>�L�����̍Đ������Ԃ�Ԃ�</summary>
    public float GetRegenTime(){ return regenTime; }
    /// <summary>�L���������̌o�ߎ��Ԃ�Ԃ�</summary>
    public float GetRegenDeltaTime(){ return regenDeltaTime; }
    /// <summary>�L�����{�^���p�̃T���l</summary>
    public Sprite GerCharaThumbnail() { return charaThumbnail; }
    /// <summary>�o�ߎ��Ԑݒ�A�{�^����������regenTime�ŏ���������p</summary>
    public void SetRegenDeltaTime(float t) {  regenDeltaTime = t; }
    /// <summary>�L�����̌o�ߎ��Ԃ𑫂�</summary>
    public void AddDeltaTime() { regenDeltaTime += Time.deltaTime; }

    protected override void Dead()
    {
        generator.friendCnt--;
        base.Dead();
    }
}