using UnityEngine;

namespace Tanks.Combat
{
    public sealed class EnemyVisualVariant : MonoBehaviour
    {
        [SerializeField] private Color tint = Color.white;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock propertyBlock;

        private void Awake() => ApplyTint();

        private void OnValidate() => ApplyTint();

        private void ApplyTint()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            propertyBlock ??= new MaterialPropertyBlock();
            foreach (Renderer renderer in renderers)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(BaseColor, tint);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}
