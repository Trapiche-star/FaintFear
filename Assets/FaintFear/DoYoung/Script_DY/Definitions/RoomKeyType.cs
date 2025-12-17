namespace FaintFear
{
    /// <summary>
    /// 각 룸에 대응되는 열쇠 타입
    /// DoorLock 인스펙터에서 선택용으로 사용
    /// </summary>
    public enum RoomKeyType
    {
        None,               // 열쇠 필요 없음
        Key,                // 기본 열쇠 (공용 / 테스트용)
        OfficeA_Key,        // 접수처 A 열쇠
        Gatehouse_Key,      // 정문 / 경비소 열쇠
        GuardRoom_Key,      // 경비실 열쇠        
        Basement_Key,       // 지하 열쇠
        DiningRoom_Key      // 식당 열쇠
    }
}
