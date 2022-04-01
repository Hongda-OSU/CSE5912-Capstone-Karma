using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class BaseItem : MonoBehaviour
    {
        public enum ItemType { Firearms, Attachment, Others }
        public ItemType CurrentItemType;

        [SerializeField] private Outline outline;
        [SerializeField] private Renderer indicator;
        [SerializeField] private float opacity = 0.3f;

        private float distanceToDisplay = 40f;

        private float rotateSpeed = 90f;

        private float floatSpeed = 1f;
        private float floatHeight = 0.1f;


        protected virtual void Update()
        {
            transform.RotateAround(transform.position, transform.up, rotateSpeed * Time.deltaTime);

            var distanceToPlayer = Vector3.Distance(PlayerManager.Instance.Player.transform.position, transform.position);

            outline.enabled = distanceToPlayer < distanceToDisplay;
            indicator.enabled = distanceToPlayer < distanceToDisplay;


            //Vector3 pos = transform.position;
            //float newY = Mathf.Sin(Time.time * floatSpeed);
            //transform.position = new Vector3(pos.x, newY, pos.z) * floatHeight;
            //Debug.Log(transform.position);
        }

        protected void SetupVfx(Color color)
        {
            outline.OutlineColor = color;
            indicator.material.color = color;
        }

    }
}