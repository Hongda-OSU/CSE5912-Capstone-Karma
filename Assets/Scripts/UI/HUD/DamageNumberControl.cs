using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace CSE5912.PolyGamers
{
    public class DamageNumberControl : MonoBehaviour
    {
        [SerializeField] private GameObject numberPrefab;

        [SerializeField] private int fontSize = 2;
        [SerializeField] private int critFontSize = 4;

        [SerializeField] private float floatingDistance = 1f;
        [SerializeField] private float popUpScale = 2f;
        [SerializeField] private float fadingTime = 0.15f;
        [SerializeField] private float displayTime = 1f;

        private static DamageNumberControl instance;
        public static DamageNumberControl Instance { get { return instance; } }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            var go = Instantiate(numberPrefab);
            Destroy(go);
        }

        public IEnumerator DisplayDamageNumber(Damage damage, Vector3 position)
        {
            int number = (int)damage.ResolvedValue;
            if (number <= 0)
                yield break;

            Quaternion rotation = Quaternion.identity;
            rotation.y = 180f;

            GameObject numberObj = Instantiate(numberPrefab, position, rotation);
            numberObj.transform.SetParent(gameObject.transform);

            TextMeshPro tmp = numberObj.GetComponent<TextMeshPro>();
            tmp.text = number.ToString();
            tmp.fontSize = fontSize;
            tmp.color = Element.Instance.TypeToColor[damage.Element];

            if (damage.IsCrit)
            {
                tmp.fontSize = critFontSize;
                tmp.text = "*" + tmp.text;
            }

            tmp.alpha = 0f;
            numberObj.GetComponent<RectTransform>().localScale = Vector3.one * popUpScale;

            float timeSince = 0f;
            var pos = numberObj.transform.position;
            while (timeSince < fadingTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                tmp.alpha = timeSince / fadingTime;
                numberObj.GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one * popUpScale, Vector3.one, timeSince / fadingTime);
                var newPos = pos + Vector3.up * Mathf.Lerp(0f, floatingDistance * 0.75f, timeSince / fadingTime);
                numberObj.transform.position = newPos;
            }

            timeSince = 0f;
            pos = numberObj.transform.position;
            while (timeSince < displayTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                var newPos = pos + Vector3.up * Mathf.Lerp(0f, floatingDistance, timeSince / displayTime);
                numberObj.transform.position = newPos;
            }


            timeSince = 0f;
            while (timeSince < fadingTime)
            {
                timeSince += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                tmp.alpha = 1 - timeSince / fadingTime;
            }
            Destroy(numberObj);
        }
    }
}
