using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveControl : MonoBehaviour
{
    public enum State
    {
        Hide_On, Hide_Off
    }

    public State state = State.Hide_Off;
    Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }


    void Update()
    {
        switch (state)
        {
            case State.Hide_On:
                UpdateHideOn();
                break;
            case State.Hide_Off:
                UpdateHideOff();
                break;
        }
    }

    void UpdateHideOn()
    {
        float _dissolveAmount = _material.GetFloat("_DissolveAmount");
        if (_dissolveAmount < 1f)
        {
            _material.SetFloat("_DissolveAmount", _dissolveAmount + (0.5f * Time.deltaTime));
        }
        else
        {
            _material.SetFloat("_DissolveAmount", 1f);
        }
    }
    void UpdateHideOff()
    {
        float _dissolveAmount = _material.GetFloat("_DissolveAmount");
        if (_dissolveAmount > 0f)
        {
            _material.SetFloat("_DissolveAmount", _dissolveAmount - (0.5f * Time.deltaTime));
        }
        else
        {
            _material.SetFloat("_DissolveAmount", 0f);
        }
    }
    public void ChangeState(State _nextState)
    {
        state = _nextState;
    }
}
