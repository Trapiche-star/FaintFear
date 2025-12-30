using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    // 기본 상태
    public float mental;
    public float battery;

    // 위치
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // 체크포인트
    public string checkpointId;

    // 시간 (선택)
    public string saveTime;
}
