
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector3 StructurePosition;
    public Vector3 LightsPosition;
    public Vector3 CameraBoundariesPosition;
}
