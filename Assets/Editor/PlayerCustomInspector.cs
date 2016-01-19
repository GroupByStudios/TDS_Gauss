using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerCustomInspector : Editor 
{

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		DrawPropertiesExcluding(serializedObject, "Attributes");

		SerializedProperty _array = serializedObject.FindProperty("Attributes");
		EditorGUILayout.PropertyField(_array);

		if (_array.isExpanded){
			EditorGUI.indentLevel += 1;

			for(int i = 0; i < _array.arraySize; i++)
			{
				SerializedProperty _element = _array.GetArrayElementAtIndex(i);
				if (_element != null)
					EditorGUILayout.PropertyField(_element, true);
			}

			EditorGUI.indentLevel -= 1;
		}

		serializedObject.ApplyModifiedProperties();
	}

}
