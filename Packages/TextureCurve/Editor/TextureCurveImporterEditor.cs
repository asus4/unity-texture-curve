/*
MIT License

Copyright (c) 2018 Andy Duboc

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace TextureCurve
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.AssetImporters;

    [CustomEditor(typeof(TextureCurveImporter)), CanEditMultipleObjects]
    public class TextureCurveImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty _redCurve;
        SerializedProperty _greenCurve;
        SerializedProperty _blueCurve;
        SerializedProperty _alphaCurve;

        SerializedProperty _resolution;
        SerializedProperty _wrapMode;
        SerializedProperty _filterMode;

        int _textureMinRes = 64;
        int _textureMaxRes = 1024;

        List<int> _textureResolutions = new List<int>();
        List<GUIContent> _textureResolutionsNames = new List<GUIContent>();

        public override void OnEnable()
        {
            base.OnEnable();

            _redCurve = serializedObject.FindProperty("_red");
            _greenCurve = serializedObject.FindProperty("_green");
            _blueCurve = serializedObject.FindProperty("_blue");
            _alphaCurve = serializedObject.FindProperty("_alpha");

            _wrapMode = serializedObject.FindProperty("_wrapMode");
            _filterMode = serializedObject.FindProperty("_filterMode");

            _resolution = serializedObject.FindProperty("_resolution");

            _textureResolutions.Clear();
            _textureResolutionsNames.Clear();

            for (int i = _textureMinRes; i <= _textureMaxRes; i *= 2)
            {
                _textureResolutions.Add(i);
                _textureResolutionsNames.Add(new GUIContent(i.ToString()));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_redCurve);
            EditorGUILayout.PropertyField(_greenCurve);
            EditorGUILayout.PropertyField(_blueCurve);
            EditorGUILayout.PropertyField(_alphaCurve);

            EditorGUILayout.IntPopup(_resolution, _textureResolutionsNames.ToArray(), _textureResolutions.ToArray());

            EditorGUILayout.PropertyField(_wrapMode);
            EditorGUILayout.PropertyField(_filterMode);


            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();

            if (GUILayout.Button("Export Texture"))
            {
                ExportAsTexture((TextureCurveImporter)target);
            }
        }

        [MenuItem("Assets/Create/Texture Curve")]
        public static void CreateNewAsset()
        {
            ProjectWindowUtil.CreateAssetWithContent("Texture Curve.texcurve", "");
        }

        private static void ExportAsTexture(TextureCurveImporter curve)
        {
            string path = EditorUtility.SaveFilePanel("Export Texture", "", "TextureCurve.png", "png");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            curve.Bake();
            var bytes = ImageConversion.EncodeToPNG(curve.Texture);

            File.WriteAllBytes(path, bytes);
        }



    }
}
