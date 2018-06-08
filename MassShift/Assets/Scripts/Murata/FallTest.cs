using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTest : MonoBehaviour
{
    [SerializeField]
    Vector3 fallPos = Vector3.zero;
    [SerializeField]
    Transform fallTransform = null;
    [SerializeField]
    float fallSpanTime = 3.0f;
    float lastFallTime = 0.0f;
    [SerializeField]

    float nowTime = 0.0f;
    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        nowTime = Time.time - lastFallTime;
        if (nowTime > fallSpanTime)
        {
            if (fallTransform)
            {
                fallTransform.position = fallPos;
                lastFallTime = Time.time;
            }
        }
    }
}
