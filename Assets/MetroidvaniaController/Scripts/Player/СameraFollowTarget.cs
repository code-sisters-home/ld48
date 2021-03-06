using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class –°ameraFollowTarget : MonoBehaviour
{
    private static –°ameraFollowTarget _target;
    public static –°ameraFollowTarget Instance => _target;

    private void Awake()
    {
        if (_target != null)
        {
            Destroy(_target);
        }
        _target = this;
    }
}
