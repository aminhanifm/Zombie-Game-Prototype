using UnityEngine;
using System.Collections;

namespace ZGP.Game
{
    public interface IPushable
    {
        void ApplyPush(Vector3 pushDirection, float pushDistance, float pushDuration);
        IEnumerator PushTransform(Vector3 pushDirection, float pushDistance, float pushDuration);
    }
}