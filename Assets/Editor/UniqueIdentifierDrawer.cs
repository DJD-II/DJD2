using System;
using UnityEditor;
using UnityEngine;

// Placeholder for UniqueIdDrawer script

// Place this file inside Assets/Editor
[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        // Generate a unique ID, defaults to an empty string if nothing has been serialized yet
        if (prop.stringValue == "")
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = false;
                Guid guid = Guid.NewGuid();

                UniqueID[] ids = GameObject.FindObjectsOfType<UniqueID>();
                foreach (UniqueID id in ids)
                {
                    if (id.uniqueId == null)
                        continue;

                    if (id.uniqueId.Equals(guid.ToString()))
                    {
                        repeat = true;
                        break;
                    }
                }
                if (!repeat)
                    prop.stringValue = guid.ToString();
            }
        }

        // Place a label so it can't be edited by accident
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
    }

    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
    }
}

