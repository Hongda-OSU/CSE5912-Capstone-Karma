using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class StatsPanelControl : UI
    {
        private VisualElement statsPanel;

        private Dictionary<string, StatRow> nameToRow;

        private static StatsPanelControl instance;
        public static StatsPanelControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            Initialize();

            statsPanel = root.Q<VisualElement>("StatsPanel");

            nameToRow = new Dictionary<string, StatRow>();
            foreach (VisualElement child in statsPanel.Q<VisualElement>("Stats").Children())
            {
                if (child.name != "Margin")
                {
                    nameToRow.Add(child.name, new StatRow(child));
                }
            }
        }
        private void Start()
        {
            foreach (var kvp in nameToRow)
            {
                kvp.Value.levelupButton.clicked += delegate { PlayerStats.Instance.LevelUpStat(kvp.Key); };
            }
        }

        private void LateUpdate()
        {
            UpdateData();
        }

        private void UpdateData()
        {
            var stats = PlayerStats.Instance;
            foreach (var row in nameToRow.Values)
            {
                var button = row.levelupButton;
                if (stats.StatPoint == 0)
                {
                    button.pickingMode = PickingMode.Ignore;
                    button.style.opacity = 0.5f;
                }
                else
                {
                    button.pickingMode = PickingMode.Position;
                    button.style.opacity = 1f;
                }
            }
            

            SetData("Level", stats.Level.ToString());
            SetData("Experience", Mathf.Ceil(stats.Experience) + " / " + Mathf.Ceil(stats.ExperienceToUpgrade));
            SetData("StatPoint", stats.StatPoint.ToString());

            SetData("Health", Mathf.Ceil(stats.Health) + " / " + Mathf.Ceil(stats.MaxHealth));
            SetData("Shield_armor", Mathf.Ceil(stats.Shield_armor) + " / " + Mathf.Ceil(stats.MaxShield_armor));
            SetData("Shield_energy", Mathf.Ceil(stats.Shield_energy) + " / " + Mathf.Ceil(stats.MaxShield_energy));

            SetData("CritRate", Mathf.Ceil(stats.CritRate * 100) + "%");
            SetData("CritDamage", Mathf.Ceil((1f + stats.CritDamageFactor) * 100) + "%");

            SetData("PhysicalDamage", Mathf.Ceil(stats.DamageFactor_physic * 100) + "%");
            SetData("FireDamage", Mathf.Ceil(stats.DamageFactor_fire * 100) + "%");
            SetData("CryoDamage", Mathf.Ceil(stats.DamageFactor_cryo * 100) + "%");
            SetData("ElectroDamage", Mathf.Ceil(stats.DamageFactor_electro * 100) + "%");
            SetData("VenomDamage", Mathf.Ceil(stats.DamageFactor_venom * 100) + "%");

            var resist = stats.GetResist();
            SetResistData(resist, Element.Type.Physical);
            SetResistData(resist, Element.Type.Fire);
            SetResistData(resist, Element.Type.Cryo);
            SetResistData(resist, Element.Type.Electro);
            SetResistData(resist, Element.Type.Venom);
        }
        private void SetData(string name, string data)
        {
            nameToRow[name].data.text = data;
        }

        private void SetResistData(Resist resist, Element.Type element)
        {
            string name = element.ToString() + "Resist";

            float resistValue = resist.FindResisByElement(element).Value;

            SetData(name, resistValue.ToString());
            nameToRow[name].data.text += "(" + Mathf.Ceil(Damage.PercentageReduced(resistValue) * 100) + "% reduced)";
        }

    }
}
