using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // 기본 상태
    public float mental;
    public float battery;
    public int batteryCount;

    // 위치
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // 체크포인트
    public string checkpointId;

    // 시간 (선택)
    public string saveTime;

    // 튜토리얼 완료
    public bool tutorialCompleted;

    // ⭐ 조명 영구 꺼짐 상태
    public bool lightsPermaOff;

    // ⭐ 월드 오브젝트 상태
    public List<string> destroyedObjects = new List<string>();

    public List<MovedObjectData> movedObjects = new List<MovedObjectData>();
}

[Serializable]
public class MovedObjectData
{
    public string id;
    public Vector3 position;
}