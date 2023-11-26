using UnityEngine;

namespace ZGP.Game
{
    public class ZGPInputManager : MonoBehaviour
    {
        public ZGPCharacterInputs characterInputs;

        public JoystickController[] Joysticks {get; set;}
        public TouchZone[] TouchZones {get; set;}

        private void Awake()
        {
            Joysticks = GetComponentsInChildren<JoystickController>();
            TouchZones = GetComponentsInChildren<TouchZone>();
        }

        public void MoveInput(Vector2 MoveDirection)
        {
            characterInputs.MoveInput(MoveDirection);
        }

        public void LookInput(Vector2 LookDirection)
        {
            characterInputs.LookInput(LookDirection);
        }

        public void CamInput(Vector2 CamDirection){
            characterInputs.CamLookInput(CamDirection);
        }
    

        public void OnDisable(){
            if(Joysticks.Length > 0){
                foreach(JoystickController js in Joysticks){
                    js.OnPointerUp(null);
                }
            }

            if(TouchZones.Length > 0){
                foreach(TouchZone tz in TouchZones){
                    tz.OnPointerUp(null);
                }   
            }
        }
        
    }

}
