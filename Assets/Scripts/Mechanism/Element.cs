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
            }
            instance = this;
        }

        public Color GetColor(Element.Type type)
        {
            switch (type)
            {
                case Type.Physical:
                    return physicalColor;
                case Type.Fire:
                    return fireColor;
                case Type.Cryo:
                    return cryoColor;
                case Type.Electro:
                    return electroColor;
                case Type.Venom:
                    return venomColor;
            }
            return Color.black;
        }
    }
}
