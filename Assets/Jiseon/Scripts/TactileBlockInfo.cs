using UnityEngine;

public enum TactileType
{
    Guide,    // 유도블록
    Warning  // 경고블록
}

public class TactileBlockInfo : MonoBehaviour
{
    public TactileType type;
}
