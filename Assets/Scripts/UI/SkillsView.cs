using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class SkillsView : UI
    {
        private void Start()
        {
            Initialize();

            root.style.display = DisplayStyle.None;
        }
    }
}
