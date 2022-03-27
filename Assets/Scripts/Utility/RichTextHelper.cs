using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class RichTextHelper : MonoBehaviour
    {
        public static string WrapInColor(string content, Color color)
        {
            string colorInHtml = ColorUtility.ToHtmlStringRGBA(color);

            string result = "<color = " + colorInHtml + ">" + content + "</color>";

            return result;
        }
        public static string WrapInColor(string content, Element.Type type)
        {
            string colorInHtml = ColorUtility.ToHtmlStringRGBA(Element.Instance.TypeToColor[type]);

            string result = "<color = " + colorInHtml + ">" + content + "</color>";

            return result;
        }
    }
}
