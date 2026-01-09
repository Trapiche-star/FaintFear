using FaintFear;
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

    // 저장된 씬 이름
    public string savedSceneName = "Level01";

    // 시간 (선택)
    public string saveTime;

    // 튜토리얼 완료
    public bool tutorialCompleted;

    // ⭐ 조명 영구 꺼짐 상태
    public bool lightsPermaOff;

    // ⭐ 월드 오브젝트 상태 (기본값 명시적 초기화)
    public List<string> destroyedObjects = new List<string>();
    public List<MovedObjectData> movedObjects = new List<MovedObjectData>();

    // ⭐ 문 상태 저장
    public List<DoorStateData> doorStates = new List<DoorStateData>();

    // ⭐ 문서 읽음 상태 저장
    public List<string> readDocuments = new List<string>();

    // ⭐ 열쇠 보유 상태 저장
    public List<string> ownedKeys = new List<string>();

    // ⭐ 퍼즐 인벤토리 상태
    public bool[] ownedLevers = new bool[4];
    public bool hasBoltCutter = false;
    public bool hasHook = false;

    // 분전반
    public PowerBoxData powerBoxData = new PowerBoxData();

    // 엘리베이터
    public ElevatorData elevatorData = new ElevatorData();

    public EndingData endingData = new EndingData();

    // ⭐ 생성자 추가 (null 방지)
    public SaveData()
    {
        // 리스트가 null이 아닌지 보장
        if (destroyedObjects == null) destroyedObjects = new List<string>();
        if (movedObjects == null) movedObjects = new List<MovedObjectData>();
        if (doorStates == null) doorStates = new List<DoorStateData>();
        if (readDocuments == null) readDocuments = new List<string>();
        if (ownedKeys == null) ownedKeys = new List<string>();
        if (ownedLevers == null) ownedLevers = new bool[4];
        if (powerBoxData == null) powerBoxData = new PowerBoxData();
        if (elevatorData == null) elevatorData = new ElevatorData();
        if (endingData == null) endingData = new EndingData();
    }
}

[Serializable]
public class PowerBoxData
{
    public bool[] filledSlots = new bool[4];
    public bool[] leverObjectsActive = new bool[4];
    public bool isPowerSupplied = false;
    public bool isCompleted = false;
}

[Serializable]
public class ElevatorData
{
    public bool isPowerSupplied = false;
}

[Serializable]
public class MovedObjectData
{
    public string id;
    public Vector3 position;
}

[Serializable]
public class EndingData
{
    public bool[] activatedLevers = new bool[4];
}

[Serializable]
public class EnemyRuntimeState
{
    public EnemyState state;
    public Vector3 position;
    public Vector3 lastKnownPlayerPos;
}

[Serializable]
public class DoorStateData
{
    public string id;
    public bool isOpen;
    public bool isLocked;
    public bool wasSaved;
}