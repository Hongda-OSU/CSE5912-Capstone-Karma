using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Element : MonoBehaviour
    {
        [SerializeField] private Color physicalColor = Color.white;
        [SerializeField] private Color fireColor = Color.red;
        [SerializeField] private Color cryoColor = Color.cyan;
        [SerializeField] private Color electroColor = Color.blue;
        [SerializeField] private Color venomColor = Color.green;
        private Dictionary<Type, Color> typeToColor;

        public enum Type
        {
            Physical,
            Fire,
            Cryo,
            Electro,
            Venom,
        }

        private static Element instance;
        public static Element Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            typeToColor = new Dictionary<Type,Color>();
            typeToColor.Add(Type.Physical, physicalColor);
            typeToColor.Add(Type.Fire, fireColor);
            typeToColor.Add(Type.Cryo, cryoColor);
            typeToColor.Add(Type.Electro, electroColor);
            typeToColor.Add(Type.Venom, venomColor);
        }

        public Dictionary<Type, Color> TypeToColor { get { return typeToColor; } }
    }
}
