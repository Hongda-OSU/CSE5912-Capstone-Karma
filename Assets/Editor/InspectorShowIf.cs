using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    //[CustomEditor(typeof(AttachmentItem))]
    //public class InspectorShowIf : Editor
    //{
    //    private SerializedObject obj;

    //    private SerializedProperty set;


    //    public override void OnInspectorGUI()
    //    {
    //        // If we call base the default inspector will get drawn too.
    //        // Remove this line if you don't want that to happen.
    //        base.OnInspectorGUI();

    //        obj = new SerializedObject(target);

    //        set = obj.FindProperty("set");

    //        AttachmentItem attachmentItem = target as AttachmentItem;

    //        if (attachmentItem.Rarity == Attachment.AttachmentRarity.Divine)
    //        {
    //            EditorGUILayout.PropertyField(set);
    //            //attachmentItem.Set = (Attachment.AttachmentSet)EditorGUILayout.EnumFlagsField(attachmentItem.Set, "Set");
    //        }
    //    }
    //}
}
