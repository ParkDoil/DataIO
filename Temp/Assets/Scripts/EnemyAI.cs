using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None,       // 
    Idle,       // ���
    Walk,       // ���� patrol
    Run,        // ���� trace
    Attack,     // ����
    KnockBack,  // �ǰ� damaged
    Hide        // ����
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState State;
    public EnemyState PrevState = EnemyState.None;

    Animator _animator;

    // �̵�����
    Vector3 _targetPosition;
    float _moveSpeed = 1f;
    float _rorationSpeed = 1f;

    // �� Ž�� ����
    public GameObject Target;
    bool _isFindEnemy = false;
    Camera _eye;
    Plane[] _eyePlanes;

    // ���� �浹 ����
    GameObject _weaponCollider;

    // ����
    DissolveControl _dissolveControl;

    void Awake()
    {
        DataManager.Instance.SaveGameData();

        MonsterData data =  DataManager.Instance.GetMonsterData(2);
        _moveSpeed = data.MoveSpeed;
        _rorationSpeed = data.RoataionSpeed;
    }

    void Start()
    {
        _dissolveControl = GetComponentInChildren<DissolveControl>();
        _dissolveControl.ChangeState(DissolveControl.State.Hide_Off);

        _animator = GetComponent<Animator>();
        _eye = transform.GetComponentInChildren<Camera>();
        SphereCollider[] _sphereColliders = GetComponentsInChildren<SphereCollider>();

        foreach(var sphereCollider in _sphereColliders)
        {
            if(sphereCollider.name == "WeaponCollider")
            {
                _weaponCollider = sphereCollider.gameObject;
                break;
            }
        }
        _weaponCollider.SetActive(false);

        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        switch (State)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Walk:
                UpdateWalk();
                break;

            case EnemyState.Run:
                UpdateRun();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.KnockBack:
                UpdateKnockBack();
                break;
            case EnemyState.Hide:
                UpdateHide();
                break;
        }
    }

    #region UpdateDetail
    // �� �����Ӹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
    void UpdateIdle()
    {

    }
    void UpdateWalk()
    {
        if (IsFindEnemy())
        {
            ChangeState(EnemyState.Run);
            return;
        }

        // ���������� �̵��ϴ� �ڵ�
        Vector3 dir = _targetPosition - transform.position;

        if (dir.sqrMagnitude <= 0.2f)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        var targetRotation = Quaternion.LookRotation(_targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rorationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }
    void UpdateRun()
    {
        // ���������� �̵��ϴ� �ڵ�
        Vector3 dir = _targetPosition - transform.position;

        if (dir.magnitude <= 2.0f)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        var targetRotation = Quaternion.LookRotation(_targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rorationSpeed * 2f * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * 2f * Time.deltaTime;
    }
    void UpdateAttack()
    {

    }
    void UpdateKnockBack()
    {

    }
    void UpdateHide()
    {

    }
    #endregion


    #region CoroutineDetail
    IEnumerator CoroutineIdle()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("��� ���� ����");
        _animator.SetBool("IsIdle", true);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
            ChangeState(EnemyState.Hide);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("���� ���� ����");
        _animator.SetBool("IsWalk", true);

        // ������ ����
        _targetPosition = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));

        _dissolveControl.ChangeState(DissolveControl.State.Hide_Off);

        while (true)
        {
            yield return new WaitForSeconds(10f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
            ChangeState(EnemyState.Idle);
        }
    }
    IEnumerator CoroutineRun()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        Debug.Log("���� ���� ����");
        _animator.SetBool("IsRun", true);
        _targetPosition = Target.transform.position;


        while (true)
        {
            yield return new WaitForSeconds(5f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

        }
    }
    IEnumerator CoroutineAttack()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineKnockBack()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // �ð����� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)

        }
    }
    IEnumerator CoroutineHide()
    {
        // �ѹ��� �����ؾ� �ϴ� ���� (���°� �ٲ� �� ����)
        _dissolveControl.ChangeState(DissolveControl.State.Hide_On);

        yield return new WaitForSeconds(3f);
        ChangeState(EnemyState.Walk);
        yield break;
    }
    #endregion

    void ChangeState(EnemyState nextState)
    {
        //if (PrevState == nextState) return;

        StopAllCoroutines();

        PrevState = State;
        State = nextState;
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsWalk", false);
        _animator.SetBool("IsRun", false);
        _animator.SetBool("IsAttack", false);
        _animator.SetBool("IsKnockBack", false);

        switch (State)
        {
            case EnemyState.Idle:
                StartCoroutine(CoroutineIdle());
                break;

            case EnemyState.Walk:
                StartCoroutine(CoroutineWalk());
                break;

            case EnemyState.Run:
                StartCoroutine(CoroutineRun());
                break;

            case EnemyState.Attack:
                StartCoroutine(CoroutineAttack());
                break;

            case EnemyState.KnockBack:
                StartCoroutine(CoroutineKnockBack());
                break;
            case EnemyState.Hide:
                StartCoroutine(CoroutineHide());
                break;
        }
    }

    bool IsFindEnemy()
    {
        if (!Target.activeSelf) return false;

        Bounds _targetBounds = Target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        _eyePlanes = GeometryUtility.CalculateFrustumPlanes(_eye);
        _isFindEnemy = GeometryUtility.TestPlanesAABB(_eyePlanes, _targetBounds);

        return _isFindEnemy;
    }

    void OnAttack(AnimationEvent _animationEvent)
    {
        Debug.Log($"OnAttack() : " + _animationEvent.intParameter);
        if (_animationEvent.intParameter == 1)
        {
            _weaponCollider.SetActive(true);
        }
        else
        {
            _weaponCollider.SetActive(false);
        }
    }
}
