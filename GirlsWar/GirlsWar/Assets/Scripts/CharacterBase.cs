using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�������ʃN���X
/// </summary>
public class CharacterBase : MonoBehaviour
{
    [SerializeField] public bool isCastle;

    [SerializeField] GameObject hitEffectObj;

    #region �ϐ�
    /// <summary>
    /// �U���Ώۂ̃L�����̐e�I�u�W�F�N�g
    /// </summary>
    protected GameObject opponent;                  // �G�̃I�u�W�F�N�g�O���[�v

    [SerializeField] CharacterState state;          // �A�j���[�V�����p�̏�ԕϐ�
    Animator animator;                              // �A�j���[�^�[�R���|�[�l���g
    protected Generator generator;
    protected PlayerManager pm;
    protected GameManager gm;

    [SerializeField] protected int maxHp;           // �L�����̍ő�̗́A�m�b�N�o�b�N�v�Z�Ŏg�p
    [SerializeField] protected int hp;                        // �L�����̎��ۂ̗̑�
    [SerializeField] protected int atk;             // �L�����̍U����
    [SerializeField] protected int def;             // �L�����̎����
    [SerializeField] protected float sp;            // �L�����̈ړ����x
    [SerializeField] protected int maxKnockBack;    // �L�����̃m�b�N�o�b�N�ő��
    float knockBackDamage;                          // �P�m�b�N�o�b�N����_���[�W��
    int knockBackCnt;                               // ���ۂɃm�b�N�o�b�N������

    [SerializeField] protected float range;         // �L�����̍U���˒�����
    [SerializeField] protected float interval;      // �L�����̍U���Ԋu
    protected float attackDeltatime;                // �U���Ԋu�̊Ǘ��p�o�ߎ���, interval�ŏ�����

    protected bool isDead;                          // ����ł邩
    [SerializeField] protected bool isMultiAttack;  // �͈͍U�����ǂ���

    #endregion


    #region �֐�

    /// <summary>
    /// ����������
    /// </summary>
    virtual protected void Init()
    {
        if(!isCastle)
        {
            // �R���|�[�l���g�擾
            animator = GetComponent<Animator>();
            generator = GameObject.Find("Generator").GetComponent<Generator>();
            pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            // �ϐ��̏�����
            attackDeltatime = interval;
            isDead = false;
            state = CharacterState.IDLE;

            knockBackCnt = 0;
            knockBackDamage = maxHp / maxKnockBack;
        }

        hp = maxHp;

    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    public void Move()
    {
        // �ړ����x���Q�Ƃ��Ĉړ�
        transform.position += new Vector3(sp, 0, 0) * Time.deltaTime;
        state = CharacterState.RUN;
    }

    /// <summary>
    /// �U������
    /// </summary>
    public void Attack()
    {
        // �U���Ώۂ�����̂ōU���Ԋu���Q�Ƃ��čU�����邩�ǂ�������
        if (attackDeltatime > interval)
        {
            // �U��
            state = CharacterState.ATTACK;
        }
        else
        {
            state = CharacterState.IDLE;
        }
    }

    /// <summary>
    /// �_���[�W���󂯂����̏���
    /// �U��������U�����̂��̊֐����Ăяo��
    /// </summary>
    public void Damage(int a)
    {

        // �m�b�N�o�b�N���Ȃ疳�G
        if (state == CharacterState.HURT)
        {
            return;
        }
        // �h��͂̌v�Z
        int dmg = a - def;

        // �񕜂��Ȃ��悤��
        if (dmg < 0)
        {
            dmg = 0;
        }

        // ���ۂ�HP�ɑ��
        hp -= dmg;

        // ��̏ꍇ�����܂ł�return
        if(isCastle)
        {
            return;
        }

        // �ݐσ_���[�W���v�Z
        var totalDamage = maxHp - hp;
        // �ݐσ_���[�W���琳�����m�b�N�o�b�N�񐔂��Z�o
        var newKnockBackCnt = (int)(totalDamage / knockBackDamage);

        // ���̃m�b�N�o�b�N�񐔂Ƃ̂��ꂪ����Ȃ�m�b�N�o�b�N���s��
        if (knockBackCnt < newKnockBackCnt)
        {
            // �m�b�N�o�b�N�񐔂̏C��
            knockBackCnt = newKnockBackCnt;
            // �m�b�N�o�b�N�A�j���[�V�������Ă�
            state = CharacterState.HURT;
            print("�m�b�N�o�b�N����");
        }

        var pos = new Vector3(0, 2f, 0) + transform.position;
        Instantiate(hitEffectObj, transform.position, Quaternion.identity, transform);
    }

    public int GetMaxHP() {  return maxHp; }




    /// <summary>
    /// �U���˒��������݂Ĕ͈͓��ɓG������Ȃ�ړ����~�߂邽��false��Ԃ�
    /// </summary>
    public bool RangeCheckMove()
    {
        // �����͓�����O��
        bool flg = true;
        // �U���Ώۂ̃I�u�W�F�N�g�����[�v
        foreach (Transform child in opponent.transform)
        {
            // ����ł���z��Active�łȂ��z�͖���
            if (child.gameObject.GetComponent<CharacterBase>().isDead || 
                child.gameObject.activeSelf == false)
            {
                continue;
            }
            
            // ���W�̍����擾
            // ���w�L�����̔��]�ɑΉ����邽��localScale���v�Z�ɓ����
            var ls = transform.localScale.x >= 0 ? 1 : -1;
            var diffPosX = (child.position.x - transform.position.x) * ls;

            // �˒��������ɂ��邩�A�����̑O���ɂ���Ȃ�U���͈͓�
            if (diffPosX < range && diffPosX >= 0)
            {
                flg = false;
                break;
            }
        }

        return flg;
    }


    /// <summary>
    /// �U���˒����������Ĕ͈͓��ɓG������Ȃ�͈͓��̃L������hp�����炷
    /// </summary>
    /// <returns></returns>
    public void RangeCheckAttack()
    {
        // �U���Ώۃ��X�g
        List<GameObject> list = new List<GameObject>();
        // �U���ΏۃO���[�v�̎q��S����
        foreach (Transform child in opponent.transform)
        {
            // ����ł���z��Active�łȂ��z�͖���
            if (child.gameObject.GetComponent<CharacterBase>().isDead || 
                child.gameObject.activeSelf == false)
            {
                continue;
            }

            // ���W�̍����擾
            // �G�������ʃN���X�̏����Ȃ̂�
            // ���w�L�����̔��]�ɑΉ����邽��localScale���v�Z�ɓ����
            var ls = transform.localScale.x >= 0 ? 1 : -1;
            var diffPosX = (child.position.x - transform.position.x) * ls;

            // �˒��������ɂ��邩�A�����̑O���ɂ���Ȃ�U���͈͓��A���X�g�ɒǉ�
            if (diffPosX <= range && diffPosX >= 0)
            {
                list.Add(child.gameObject);
            }

        }

        // �͈͍U�����P�̍U���ŏ�������

        // �U���ΏۑS���Ƀ_���[�W
        if (isMultiAttack)
        {
            foreach (GameObject child in list)
            {
                
                child.GetComponent<CharacterBase>().Damage(atk);
            }
        }
        // �����Ƃ��������߂����̂ɂ̂݃_���[�W
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
            // ��O�͍U�����s
            if (list.Count != 0 && index != -1)
            {
                list[index].GetComponent<CharacterBase>().Damage(atk);
            }
        }

        gm.CharaAttackSE();
    }

    /// <summary>
    /// �U���A�j���[�V�������I�������ɌĂ΂��֐�
    /// �U���A�j���[�V�����C�x���g�ŌĂяo��
    /// </summary>
    void EndAttackMotion()
    {
        attackDeltatime = 0;
        state = CharacterState.IDLE;
    }


    /// <summary>
    /// ��_���[�W�i�m�b�N�o�b�N�j�A�j���[�V�����I�����ɌĂ΂��֐�
    /// �m�b�N�o�b�N�A�j���[�V�����C�x���g�ŌĂяo��
    /// </summary>
    void EndHurtMotion()
    {
        // ���S����
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
    /// ���񂾂Ƃ��̏���
    /// ���S�A�j���[�V�����I�����ɁA���S�A�j���[�V�����C�x���g����Ăяo��
    /// </summary>
    void EndDeadMotion()
    {
        //Invoke("Dead", 0.5f);
        Dead();
    }
    /// <summary>
    /// �I�u�W�F�N�g�j��
    /// </summary>
    virtual protected void Dead()
    {
        Destroy(gameObject);
    }


    /// <summary>
    /// state���Q�Ƃ��ăA�j���[�V�����̏���
    /// </summary>
    void Animation()
    {
        switch (state)
        {
            // �U���ҋ@
            case CharacterState.IDLE:
                animator.SetTrigger("Idle");
                break;
            // �ړ�
            case CharacterState.RUN:
                animator.SetTrigger("Run");
                break;
            // �U��
            case CharacterState.ATTACK:
                animator.SetTrigger("Attack");
                break;
            // ��_���[�W�i�m�b�N�o�b�N�j
            case CharacterState.HURT:
                animator.SetTrigger("Hurt");
                break;
            // ���S
            case CharacterState.DEAD:
                animator.SetTrigger("Die");
                break;
        }
    }


    #endregion

    // �G�������ʂ�Update
    virtual protected void Update()
    {
        if (!isCastle)
        {
            // �O��̍U������̌o�ߎ��Ԃ��L�^�A�U������Ɏg�p
            attackDeltatime += Time.deltaTime;

            // ����ł��炸�A�m�b�N�o�b�N�����ĂȂ����̂ݍs���\
            if (state != CharacterState.DEAD && state != CharacterState.HURT)
            {
                // �ړ��ł��邩������s��
                if (RangeCheckMove())
                {
                    if(state != CharacterState.ATTACK)
                    {
                        // �ړ�
                        Move();
                    }

                }
                else
                {
                    // �U���̏����ɓ���
                    Attack();
                }
            }

            // �A�j���[�V��������
            Animation();
        }
        
    }

    public int GetHP()
    {
        return hp;
    }

}
