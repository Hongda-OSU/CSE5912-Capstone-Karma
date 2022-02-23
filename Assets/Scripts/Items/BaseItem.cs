using UnityEngine;

namespace CSE5912.PolyGamers
{
    public abstract class BaseItem : MonoBehaviour
    {
        public enum ItemType { Firearms, Attachment, Others }
        public ItemType CurrentItemType;

        private float rotateSpeed = 90f;

        private float floatSpeed = 1f;
        private float floatHeight = 0.1f;


        protected virtual void Update()
        {
            transform.RotateAround(transform.position, transform.up, rotateSpeed * Time.deltaTime);

            //Vector3 pos = transform.position;
            //float newY = Mathf.Sin(Time.time * floatSpeed);
            //transform.position = new Vector3(pos.x, newY, pos.z) * floatHeight;
            //Debug.Log(transform.position);
        }
    }
}