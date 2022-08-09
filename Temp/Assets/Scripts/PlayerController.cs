using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asset;

public class PlayerController : MonoBehaviour, ITargetable
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void OnDamaged()
    {
        Debug.Log("공격을 받았다");
    }
}
