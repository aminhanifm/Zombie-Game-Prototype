using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ZGP.Game
{
	public class ZGPCharacterInputs : MonoBehaviour
	{
		public Vector2 Move {get; private set;}
		public Vector2 PlayerLook {get; private set;}
		public Vector2 CamLook {get; private set;}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			Move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			PlayerLook = newLookDirection;
		}

		public void CamLookInput(Vector2 newCamLookDirection){
			CamLook = newCamLookDirection;
		}
	}
	
}