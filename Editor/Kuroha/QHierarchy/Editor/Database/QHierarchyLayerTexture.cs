using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyLayerTexture
    {
        internal readonly string layer;
        internal Texture2D texture;

        internal QHierarchyLayerTexture(string layer, Texture2D texture)
        {
            this.layer = layer;
            this.texture = texture;
        }

        internal static List<QHierarchyLayerTexture> LoadLayerTextureList()
        {
            var layerTextureList = new List<QHierarchyLayerTexture>();

            var customTagIcon = QHierarchySettings.Instance().Get<string>(EM_QHierarchySettings.LayerIconList);

            var customLayerIconArray = customTagIcon.Split(';');

            var layers = new List<string>(UnityEditorInternal.InternalEditorUtility.layers);

            for (var i = 0; i < customLayerIconArray.Length - 1; i += 2)
            {
                var layer = customLayerIconArray[i];

                if (!layers.Contains(layer))
                {
                    continue;
                }

                var texturePath = customLayerIconArray[i + 1];

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                if (texture != null)
                {
                    var tagTexture = new QHierarchyLayerTexture(layer, texture);
                    layerTextureList.Add(tagTexture);
                }
            }

            return layerTextureList;
        }

        internal static void SaveLayerTextureList(EM_QHierarchySettings hierarchySettings, List<QHierarchyLayerTexture> layerTextureList)
        {
            var result = new StringBuilder();

            foreach (var layerTexture in layerTextureList)
            {
                var id = AssetDatabase.GetAssetPath(layerTexture.texture.GetInstanceID());
                result.Append($"{layerTexture.layer};{id};");
            }

            QHierarchySettings.Instance().Set(hierarchySettings, result.ToString());
        }
    }
}
