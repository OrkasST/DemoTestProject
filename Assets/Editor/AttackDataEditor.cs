using UnityEditor;

[CustomEditor(typeof(AttackData))]
public class AttackDataEditor : Editor
{
    #region SerializedProperties
    SerializedProperty InitialScale;
    SerializedProperty InitialPosition;
    SerializedProperty InitialRotation;

    SerializedProperty DealingDamagePosition;
    SerializedProperty DealingDamageScale;
    SerializedProperty DealingDamageRotation;

    SerializedProperty EndPosition;
    SerializedProperty EndScale;
    SerializedProperty EndRotation;

    SerializedProperty ChargingTime;
    SerializedProperty AccelerateTime;
    SerializedProperty DealDamageTime;
    SerializedProperty RecoveryTime;

    SerializedProperty Damage;

    bool InitialGroup = false;
    bool DealingDamageDroup = false;
    bool EndDroup = false;
    bool TimingsGroup = false;
    #endregion

    private void OnEnable()
    {
        InitialScale = serializedObject.FindProperty("InitialScale");
        InitialPosition = serializedObject.FindProperty("InitialPosition");
        InitialRotation = serializedObject.FindProperty("InitialRotation");

        DealingDamagePosition = serializedObject.FindProperty("DealingDamagePosition");
        DealingDamageScale = serializedObject.FindProperty("DealingDamageScale");
        DealingDamageRotation = serializedObject.FindProperty("DealingDamageRotation");

        EndPosition = serializedObject.FindProperty("EndPosition");
        EndScale = serializedObject.FindProperty("EndScale");
        EndRotation = serializedObject.FindProperty("EndRotation");

        ChargingTime = serializedObject.FindProperty("ChargingTime");
        AccelerateTime = serializedObject.FindProperty("AccelerateTime");
        DealDamageTime = serializedObject.FindProperty("DealDamageTime");
        RecoveryTime = serializedObject.FindProperty("RecoveryTime");

        Damage = serializedObject.FindProperty("Damage");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InitialGroup = EditorGUILayout.BeginFoldoutHeaderGroup(InitialGroup, "Initial");
        if (InitialGroup )
        {
            EditorGUILayout.PropertyField(InitialPosition);
            EditorGUILayout.PropertyField(InitialRotation);
            EditorGUILayout.PropertyField(InitialScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        DealingDamageDroup = EditorGUILayout.BeginFoldoutHeaderGroup(DealingDamageDroup, "Dealing Damage");
        if (DealingDamageDroup)
        {
            EditorGUILayout.PropertyField(DealingDamagePosition);
            EditorGUILayout.PropertyField(DealingDamageRotation);
            EditorGUILayout.PropertyField(DealingDamageScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EndDroup = EditorGUILayout.BeginFoldoutHeaderGroup(EndDroup, "End");
        if (EndDroup)
        {
            EditorGUILayout.PropertyField(EndPosition);
            EditorGUILayout.PropertyField(EndRotation);
            EditorGUILayout.PropertyField(EndScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        TimingsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(TimingsGroup, "Timings");
        if (TimingsGroup)
        {
            EditorGUILayout.PropertyField(ChargingTime);
            EditorGUILayout.PropertyField(AccelerateTime);
            EditorGUILayout.PropertyField(DealDamageTime);
            EditorGUILayout.PropertyField(RecoveryTime);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(Damage);

        serializedObject.ApplyModifiedProperties();
    }
}
