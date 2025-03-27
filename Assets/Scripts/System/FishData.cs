using UnityEditor.Build.Content;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Fish Data", fileName = "FishData")]
public class FishData : ScriptableObject
{
    [Header("물고기 타입")]
    public FishType fishType;

    [Header("물고기 이름")]
    public string fishName;

    [Header("프리팹")]
    public GameObject prefab;

    [Header("크기 (스케일)")]
    public Vector2 size = Vector2.one;

    [Header("이동 속도 (좌우 프레임 단위)")]
    public float moveSpeed = 3f;

    [Header("수직 속도 (프레임 단위)")]
    public float fallSpeed = 1f;
}

public enum FishType
{
    BigFish,
    MidFish,
    SmallFish,
    // TODO: 필요 시 계속 추가
}
