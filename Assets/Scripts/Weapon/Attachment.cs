using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Attachment
    {
        public string name;
        public Sprite iconImage;
        public string description;
        public Firearms attachedTo;

        public string BuildDescription()
        {
            description =
                "Name: " + name +
                "\nAttached to: " + attachedTo;

            return description;
        }
    }
}
