using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    None,       // 
    Idle,       // 대기
    Walk,       // 순찰 patrol
    Run,        // 추적 trace
    Attack,     // 공격
    KnockBack,  // 피격 damaged
    Hide        // 은신
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState State;
    public EnemyState PrevState = EnemyState.None;

    Animator _animator;

    // 이동관련
    Vector3 _targetPosition;
    float _moveSpeed = 1f;
    float _rorationSpeed = 1f;

    // 적 탐지 관련
    public GameObject Target;
    bool _isFindEnemy = false;
    Camera _eye;
    Plane[] _eyePlanes;

    // 공격 충돌 관련
    GameObject _weaponCollider;

    // 은신
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
    // 매 프레임마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
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

        // 목적지까지 이동하는 코드
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
        // 목적지까지 이동하는 코드
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
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("대기 상태 시작");
        _animator.SetBool("IsIdle", true);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
            ChangeState(EnemyState.Hide);
            yield break;
        }
    }
    IEnumerator CoroutineWalk()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("순찰 상태 시작");
        _animator.SetBool("IsWalk", true);

        // 목적지 설정
        _targetPosition = transform.position + new Vector3(Random.Range(-7f, 7f), 0f, Random.Range(-7f, 7f));

        _dissolveControl.ChangeState(DissolveControl.State.Hide_Off);

        while (true)
        {
            yield return new WaitForSeconds(10f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)
            ChangeState(EnemyState.Idle);
        }
    }
    IEnumerator CoroutineRun()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        Debug.Log("추적 상태 시작");
        _animator.SetBool("IsRun", true);
        _targetPosition = Target.transform.position;


        while (true)
        {
            yield return new WaitForSeconds(5f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)

        }
    }
    IEnumerator CoroutineAttack()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(1f);
        ChangeState(EnemyState.Idle);
        yield break;
    }
    IEnumerator CoroutineKnockBack()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // 시간마다 수행해야 하는 동작 (상태가 바뀔 때 마다)

        }
    }
    IEnumerator CoroutineHide()
    {
        // 한번만 수행해야 하는 동작 (상태가 바뀔 때 마다)
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
