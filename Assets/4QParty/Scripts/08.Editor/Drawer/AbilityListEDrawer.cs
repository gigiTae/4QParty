using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using FQParty.GamePlay.Abilities;

[CustomEditor(typeof(AbilityList))]
public class AbilityListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AbilityList abilityList = (AbilityList)target;

        GUILayout.Space(10); // 여백 추가

        if (GUILayout.Button("모든 어빌리티 가져오기"))
        {
            CollectAbilities(abilityList);
        }
    }

    private void CollectAbilities(AbilityList abilityList)
    {
        abilityList.Abilities = new List<Ability>();

        string[] guids = AssetDatabase.FindAssets("t:Ability");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Ability asset = AssetDatabase.LoadAssetAtPath<Ability>(path);

            if (asset != null)
            {
                abilityList.Abilities.Add(asset);
            }
        }

        EditorUtility.SetDirty(abilityList);
        AssetDatabase.SaveAssets();

        Debug.Log($"{abilityList.Abilities.Count}개의 어빌리티를 가져왔습니다.");
    }
}