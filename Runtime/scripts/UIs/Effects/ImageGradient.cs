using UnityEngine;
using UnityEngine.UI;

namespace Coalballcat.Services.UI
{
    /// <summary>
    /// Two-color linear gradient mesh effect for uGUI graphics (Image / Text).
    /// The gradient is evaluated per-vertex along an adjustable angle.
    /// </summary>
    [AddComponentMenu("UI/Effects/Coalballcat Gradient")]
    public class ImageGradient : BaseMeshEffect
    {
        [SerializeField] private Color color1 = Color.white;
        [SerializeField] private Color color2 = Color.black;
        [Range(-180f, 180f)]
        [SerializeField] private float angle = -90f;

        public Color Color1
        {
            get => color1;
            set { color1 = value; SetGraphicDirty(); }
        }

        public Color Color2
        {
            get => color2;
            set { color2 = value; SetGraphicDirty(); }
        }

        public float Angle
        {
            get => angle;
            set { angle = Mathf.Clamp(value, -180f, 180f); SetGraphicDirty(); }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled || graphic == null || vh.currentVertCount == 0)
                return;

            Rect rect = graphic.rectTransform.rect;

            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // Maximum projection magnitude from the rect center to a corner along 'dir'
            // for a normalized (-0.5..0.5) square. Used to remap projection into 0..1.
            float maxProjection = 0.5f * (Mathf.Abs(dir.x) + Mathf.Abs(dir.y));

            UIVertex vertex = default;
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);

                float nx = (rect.width != 0f) ? (vertex.position.x - rect.xMin) / rect.width - 0.5f : 0f;
                float ny = (rect.height != 0f) ? (vertex.position.y - rect.yMin) / rect.height - 0.5f : 0f;

                float projection = nx * dir.x + ny * dir.y;
                float t = maxProjection > 0f ? (projection + maxProjection) / (2f * maxProjection) : 0.5f;

                vertex.color *= Color.Lerp(color1, color2, Mathf.Clamp01(t));
                vh.SetUIVertex(vertex, i);
            }
        }

        private void SetGraphicDirty()
        {
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }
}
