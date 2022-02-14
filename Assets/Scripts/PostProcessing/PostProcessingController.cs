using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CSE5912.PolyGamers
{
    public class PostProcessingController : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume blurryCameraView;


        private static PostProcessingController instance;
        public static PostProcessingController Instance { get { return instance; } }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;

            if (blurryCameraView == null)
                blurryCameraView = GetComponent<PostProcessVolume>();

            blurryCameraView.enabled = false;
        }

        public void SetBlurryCameraView(bool isActive) { blurryCameraView.enabled = isActive; }
    }
}
