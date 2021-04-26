using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class СameraFollowTarget : MonoBehaviour
{
    private static СameraFollowTarget _target;
    public static СameraFollowTarget Instance => _target;

    private void Awake()
    {
        if (_target != null)
        {
            Destroy(_target);
        }
        _target = this;
    }
}
