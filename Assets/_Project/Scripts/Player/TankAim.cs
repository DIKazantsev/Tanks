using Tanks.Input;
using UnityEngine;

namespace Tanks.Player
{
    /// <summary>Converts the mouse aim point into a world direction for the independent turret.</summary>
    public sealed class TankAim : MonoBehaviour
    {
        [SerializeField] private TankInput input = null;
        [SerializeField] private TankTurret turret = null;
        [SerializeField] private Camera aimingCamera = null;

        private void Update()
        {
            Ray aimRay = aimingCamera.ScreenPointToRay(input.AimPosition);
            Plane groundPlane = new(Vector3.up, transform.position);
            if (!groundPlane.Raycast(aimRay, out float distance)) return;

            Vector3 aimPoint = aimRay.GetPoint(distance);
            turret.SetAimDirection(aimPoint - turret.transform.position);
        }
    }
}
