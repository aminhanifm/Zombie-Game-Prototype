using UnityEngine;

namespace ZGP.Game{
    public class ZGPCameraProperties : MonoBehaviour
    {
        public enum CameraTypes{
            TopDown,
            ThirdPerson
        }

        public CameraTypes cameraType;
        public Transform target;
        public float smoothSpeed = 0.125f;
        public Vector3 offset;
        public GameObject[] cameras;

        public ZGPInputManager joystickIM;
        public TouchZone touchZone;
        public JoystickController lookInput;

        private ZGPCharacterMovement characterMovement;

        void Start()
        {
            characterMovement = FindObjectOfType<ZGPCharacterMovement>();
            ChangeCamera(CameraTypes.TopDown);
        }

        void Update()
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        public void ToggleCamera(){
            if(cameraType == CameraTypes.TopDown){
                cameraType = CameraTypes.ThirdPerson;
            } else {
                cameraType = CameraTypes.TopDown;
            }

            SetCamera();
        }

        private void ChangeCamera(CameraTypes type){
            cameraType = type;
            SetCamera();
        }

        private void SetCamera(){
            if(cameraType == CameraTypes.TopDown){
                cameras[0].SetActive(true);
                cameras[1].SetActive(false);
                characterMovement.MainCam = cameras[0];
                touchZone.gameObject.SetActive(false);
                lookInput.joystickOutputEvent.RemoveListener(joystickIM.CamInput);
                lookInput.joystickOutputEvent.AddListener(joystickIM.LookInput);
            } else {
                cameras[0].SetActive(false);
                cameras[1].SetActive(true);
                characterMovement.MainCam = cameras[1];
                touchZone.gameObject.SetActive(true);
                lookInput.joystickOutputEvent.RemoveListener(joystickIM.LookInput);
                lookInput.joystickOutputEvent.AddListener(joystickIM.CamInput);
            }
        }
    }
}