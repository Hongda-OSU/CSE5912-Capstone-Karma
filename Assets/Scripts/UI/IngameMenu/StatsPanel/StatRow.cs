using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class StatRow
    {
        public VisualElement row;
        public Label label;
        public Label data;
        public Button levelupButton;

        public StatRow(VisualElement row)
        {
            this.row = row;
            label = row.Q<Label>("Label");
            data = row.Q<Label>("Data");
            levelupButton = row.Q<Button>("LevelUp");
        }
    }
}
