

using UnityEditor;
using UnityEngine;

public class ShowInInspectorAttribute : PropertyAttribute
{

}
# if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowInInspectorAttribute))]
public class ShowInInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetObject = property.serializedObject.targetObject;
        var targetType = targetObject.GetType();
        var field = targetType.GetField(property.name, System.Reflection.BindingFlags.NonPublic);

        if (field != null)
        {
            // Get the current value of the field
            var fieldValue = field.GetValue(targetObject);

            // Display the appropriate field based on the type
            EditorGUI.BeginChangeCheck();

            if (fieldValue is int)
            {
                fieldValue = EditorGUI.IntField(position, label, (int)fieldValue);
            }
            else if (fieldValue is float)
            {
                fieldValue = EditorGUI.FloatField(position, label, (float)fieldValue);
            }
            else if (fieldValue is string)
            {
                fieldValue = EditorGUI.TextField(position, label, (string)fieldValue);
            }
            else if (fieldValue is bool)
            {
                fieldValue = EditorGUI.Toggle(position, label, (bool)fieldValue);
            }
            // Add other types as needed

            if (EditorGUI.EndChangeCheck())
            {
                field.SetValue(targetObject, fieldValue);
                EditorUtility.SetDirty(targetObject);
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Field not found");
        }
    }
}
# endif
