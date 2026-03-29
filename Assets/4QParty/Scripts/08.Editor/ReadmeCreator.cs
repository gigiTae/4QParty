using UnityEngine;
using UnityEditor;
using System.IO;

namespace FQParty.Editor
{
    /// <summary>
    /// 마크다운 형식의 Readme 파일을 생성합니다.
    /// </summary>
    public class ReadmeCreator
    {
        [MenuItem("Assets/Create/Readme (Markdown)", false, 80)]
        public static void CreateMarkdownFile()
        {
            // 1. 경로 설정
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }

            // 2. 파일명 설정 (.md 확장자)
            string fileName = "Readme.md";
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName);

            // 3. 마크다운 스타일 템플릿 적용
            string projectName = PlayerSettings.productName;
            string createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // --- 마크다운 문법 적용 ---
            // # 은 제목, > 는 인용구(박스), ** 는 굵게 표시됩니다.
            string content = $@"
📝 개요


⚠️ 매우 중요
* 여기에 중요한 주의사항을 작성하세요.

";

            // 4. 파일 쓰기 및 리프레시
            File.WriteAllText(fullPath, content);
            AssetDatabase.Refresh();

            // 5. 생성된 파일 포커싱
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}