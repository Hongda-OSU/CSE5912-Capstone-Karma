using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] 
        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new InputActions();

        }

        private void OnEnable()
        {
        }
    }
}
