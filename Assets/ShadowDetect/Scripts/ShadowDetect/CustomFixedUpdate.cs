using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// C#
public class CustomFixedUpdate
{
    private float _FixedDeltaTime;
    private float _ReferenceTime = 0;
    private float _FixedTime = 0;
    private float _MaxAllowedTimestep = 0.3f;
    private System.Action _FixedUpdate;
    private System.Diagnostics.Stopwatch _Timeout = new System.Diagnostics.Stopwatch();

    public CustomFixedUpdate(float aFixedDeltaTime, System.Action aFixecUpdateCallback)
    {
        _FixedDeltaTime = aFixedDeltaTime;
        _FixedUpdate = aFixecUpdateCallback;
    }

    public bool Update(float aDeltaTime)
    {
        _Timeout.Reset();
        _Timeout.Start();

        _ReferenceTime += aDeltaTime;
        while (_FixedTime < _ReferenceTime)
        {
            _FixedTime += _FixedDeltaTime;
            if (_FixedUpdate != null)
                _FixedUpdate();
            if ((_Timeout.ElapsedMilliseconds / 1000.0f) > _MaxAllowedTimestep)
                return false;
        }
        return true;
    }

    public float FixedDeltaTime
    {
        get { return _FixedDeltaTime; }
        set { _FixedDeltaTime = value; }
    }
    public float MaxAllowedTimestep
    {
        get { return _MaxAllowedTimestep; }
        set { _MaxAllowedTimestep = value; }
    }
    public float ReferenceTime
    {
        get { return _ReferenceTime; }
    }
    public float FixedTime
    {
        get { return _FixedTime; }
    }
}
