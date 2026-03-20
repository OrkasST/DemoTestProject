using UnityEditor;

[CustomEditor(typeof(BlockData))]
public class BlockDataEditor : Editor
{
    #region SerializedProperties
    SerializedProperty InitialScale;
    SerializedProperty InitialPosition;
    SerializedProperty InitialRotation;

    SerializedProperty ParryngEndPosition;
    SerializedProperty ParryngEndScale;
    SerializedProperty ParryngEndRotation;

    SerializedProperty BlockPosition;
    SerializedProperty BlockScale;
    SerializedProperty BlockRotation;

    SerializedProperty PreparingTime;
    SerializedProperty ParryingMoveTime;
    SerializedProperty ParryingTime;
    SerializedProperty BlockMoveTime;
    SerializedProperty RecoveryTime;

    SerializedProperty DamageDecrease;

    bool InitialGroup = false;
    bool ParryingGroup = false;
    bool BlockGroup = false;
    bool TimingsGroup = false;
    #endregion

    private void OnEnable()
    {
        InitialScale = serializedObject.FindProperty("InitialScale");
        InitialPosition = serializedObject.FindProperty("InitialPosition");
        InitialRotation = serializedObject.FindProperty("InitialRotation");

        ParryngEndPosition = serializedObject.FindProperty("ParryngEndPosition");
        ParryngEndScale = serializedObject.FindProperty("ParryngEndScale");
        ParryngEndRotation = serializedObject.FindProperty("ParryngEndRotation");

        BlockPosition = serializedObject.FindProperty("BlockPosition");
        BlockScale = serializedObject.FindProperty("BlockScale");
        BlockRotation = serializedObject.FindProperty("BlockRotation");

        PreparingTime = serializedObject.FindProperty("PreparingTime");
        ParryingMoveTime = serializedObject.FindProperty("ParryingMoveTime");
        ParryingTime = serializedObject.FindProperty("ParryingTime");
        BlockMoveTime = serializedObject.FindProperty("BlockMoveTime");
        RecoveryTime = serializedObject.FindProperty("RecoveryTime");

        DamageDecrease = serializedObject.FindProperty("DamageDecrease");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InitialGroup = EditorGUILayout.BeginFoldoutHeaderGroup(InitialGroup, "Initial");
        if (InitialGroup)
        {
            EditorGUILayout.PropertyField(InitialPosition);
            EditorGUILayout.PropertyField(InitialRotation);
            EditorGUILayout.PropertyField(InitialScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        ParryingGroup = EditorGUILayout.BeginFoldoutHeaderGroup(ParryingGroup, "Parrying");
        if (ParryingGroup)
        {
            EditorGUILayout.PropertyField(ParryngEndPosition);
            EditorGUILayout.PropertyField(ParryngEndRotation);
            EditorGUILayout.PropertyField(ParryngEndScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        BlockGroup = EditorGUILayout.BeginFoldoutHeaderGroup(BlockGroup, "Block");
        if (BlockGroup)
        {
            EditorGUILayout.PropertyField(BlockPosition);
            EditorGUILayout.PropertyField(BlockRotation);
            EditorGUILayout.PropertyField(BlockScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        TimingsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(TimingsGroup, "Timings");
        if (TimingsGroup)
        {
            EditorGUILayout.PropertyField(PreparingTime);
            EditorGUILayout.PropertyField(ParryingMoveTime);
            EditorGUILayout.PropertyField(ParryingTime);
            EditorGUILayout.PropertyField(BlockMoveTime);
            EditorGUILayout.PropertyField(RecoveryTime);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        EditorGUILayout.Slider(DamageDecrease, 0, 1f);

        serializedObject.ApplyModifiedProperties();
    }
}
