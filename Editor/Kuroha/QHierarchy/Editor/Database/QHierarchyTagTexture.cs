using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyTagTexture
    {
        internal readonly string tag;
        internal Texture2D texture;

        internal QHierarchyTagTexture(string tag, Texture2D texture)
        {
            this.tag = tag;
            this.texture = texture;
        }

        internal static List<QHierarchyTagTexture> LoadTagTextureList()
        {
            var tagTextureList = new List<QHierarchyTagTexture>();

            var customTagIcon = QHierarchySettings.Instance().Get<string>(EM_QHierarchySettings.TagIconList);

            var customTagIconArray = customTagIcon.Split(';');

            var tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);

            for (var i = 0; i < customTagIconArray.Length - 1; i += 2)
            {
                var tag = customTagIconArray[i];

                if (!tags.Contains(tag))
                {
                    continue;
                }

                var texturePath = customTagIconArray[i + 1];

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                if (texture != null)
                {
                    var tagTexture = new QHierarchyTagTexture(tag, texture);

                    tagTextureList.Add(tagTexture);
                }
            }

            return tagTextureList;
        }

        internal static void SaveTagTextureList(EM_QHierarchySettings hierarchySettings, List<QHierarchyTagTexture> tagTextureList)
        {
            var result = new StringBuilder();

            foreach (var tagTexture in tagTextureList)
            {
                var id = AssetDatabase.GetAssetPath(tagTexture.texture.GetInstanceID());
                result.Append($"{tagTexture.tag};{id};");
            }

            QHierarchySettings.Instance().Set(hierarchySettings, result.ToString());
        }
    }
}
