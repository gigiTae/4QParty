using UnityEngine;
using UnityEditor;
using System.IO;


namespace FQParty.Default
{
    /// <summary>
    /// Readme 
    /// </summary>
    public class ReadmeCreator
    {
        [MenuItem("Assets/Create/Readme", false, 80)]
        public static void CreateTextFile()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }

            string fileName = "Readme.txt";
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName);

            // --- 가독성을 높인 텍스트 템플릿 적용 ---
            string projectName = PlayerSettings.productName; // 유니티 프로젝트 이름 가져오기
            string createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string content = $@"# PROJECT: {projectName}
============================================================
생성 일시: {createdAt}
============================================================


============================================================
";
            // ---------------------------------------

            File.WriteAllText(fullPath, content);

            AssetDatabase.Refresh();

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}