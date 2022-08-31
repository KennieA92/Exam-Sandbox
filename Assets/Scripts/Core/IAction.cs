using UnityEngine;

namespace GAME.Core
{
    public interface IAction
    {
        void StartAction(Vector3 destination, float speed);
        void CancelAction();
    }
}