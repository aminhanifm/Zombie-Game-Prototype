using UnityEngine;

namespace ZGP.Game
{
    public class LookAtCam : MonoBehaviour
    {
        private ZGPGameManager GM => ZGPGameManager.Instance;
        private ZGPCameraProperties CP => GM.CameraProperties;

        private bool IsTopDownCam => CP.cameraType == ZGPCameraProperties.CameraTypes.TopDown;
        private bool IsThirdPersonCam => CP.cameraType == ZGPCameraProperties.CameraTypes.ThirdPerson;

        void Update()
        {
            if(IsTopDownCam){
                transform.rotation = Quaternion.Euler(62, -140, 0);
            } else {
                transform.rotation = Quaternion.LookRotation(transform.position - CP.cameras[1].transform.position);
            }
        }
    }
}