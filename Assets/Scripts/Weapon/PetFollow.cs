using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class PetFollow : MonoBehaviour
    {
        [SerializeField] private Transform player;


        void Update()
        {
            Vector3 rot = Quaternion.LookRotation(player.position - transform.position).eulerAngles;
            rot.x = rot.z = 0;
            transform.rotation = Quaternion.Euler(rot);
            if (Vector3.Distance(transform.position, player.position) > 2.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
    }
}