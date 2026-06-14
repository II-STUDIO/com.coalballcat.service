using UnityEngine;
using UnityEngine.UI;

namespace Coalballcat.Services.UI
{
    /// <summary>
    /// A uGUI graphic that combines 9-slice rendering with a linear (horizontal/vertical)
    /// fill — something the built-in <see cref="Image"/> cannot do simultaneously.
    ///
    /// The fill is modelled as a visible window along the chosen axis; every 9-slice patch
    /// is clipped to that window (with matching UV interpolation), so borders stay crisp at
    /// any fill amount.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Sliced Filled Image")]
    public class SlicedFilledImage : MaskableGraphic, ILayoutElement
    {
        public enum FillDirection { Right = 0, Left = 1, Up = 2, Down = 3 }

        [SerializeField] private Sprite m_Sprite;
        [SerializeField] private FillDirection m_FillDirection = FillDirection.Right;
        [Range(0f, 1f)]
        [SerializeField] private float m_FillAmount = 1f;
        [SerializeField] private bool m_FillCenter = true;
        [SerializeField] private float m_PixelsPerUnitMultiplier = 1f;

        public Sprite sprite
        {
            get => m_Sprite;
            set { if (m_Sprite != value) { m_Sprite = value; SetAllDirty(); } }
        }

        public FillDirection fillDirection
        {
            get => m_FillDirection;
            set { if (m_FillDirection != value) { m_FillDirection = value; SetVerticesDirty(); } }
        }

        public float fillAmount
        {
            get => m_FillAmount;
            set
            {
                float clamped = Mathf.Clamp01(value);
                if (!Mathf.Approximately(m_FillAmount, clamped)) { m_FillAmount = clamped; SetVerticesDirty(); }
            }
        }

        public bool fillCenter
        {
            get => m_FillCenter;
            set { if (m_FillCenter != value) { m_FillCenter = value; SetVerticesDirty(); } }
        }

        public float pixelsPerUnitMultiplier
        {
            get => m_PixelsPerUnitMultiplier;
            set { m_PixelsPerUnitMultiplier = Mathf.Max(0.01f, value); SetVerticesDirty(); }
        }

        public override Texture mainTexture => m_Sprite != null ? m_Sprite.texture : s_WhiteTexture;

        private float PixelsPerUnit
        {
            get
            {
                float spritePpu = m_Sprite != null ? m_Sprite.pixelsPerUnit : 100f;
                float referencePpu = canvas != null ? canvas.referencePixelsPerUnit : 100f;
                return m_PixelsPerUnitMultiplier * spritePpu / referencePpu;
            }
        }

        private bool HasBorder => m_Sprite != null && m_Sprite.border.sqrMagnitude > 0f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (m_Sprite == null)
            {
                base.OnPopulateMesh(vh);
                return;
            }

            vh.Clear();

            if (m_FillAmount <= 0f)
                return;

            Rect rect = GetPixelAdjustedRect();
            Vector4 outer = UnityEngine.Sprites.DataUtility.GetOuterUV(m_Sprite);

            if (!HasBorder)
            {
                PopulateSimple(vh, rect, outer);
                return;
            }

            PopulateSliced(vh, rect, outer);
        }

        // ── Simple (no border) ────────────────────────────────────────────────────

        private void PopulateSimple(VertexHelper vh, Rect rect, Vector4 outer)
        {
            // Single patch spanning the whole rect.
            GetVisibleWindow(rect, out float cutMin, out float cutMax, out bool horizontal);
            AddPatch(vh,
                rect.xMin, rect.yMin, rect.xMax, rect.yMax,
                outer.x, outer.y, outer.z, outer.w,
                cutMin, cutMax, horizontal);
        }

        // ── 9-slice ───────────────────────────────────────────────────────────────

        private void PopulateSliced(VertexHelper vh, Rect rect, Vector4 outer)
        {
            Vector4 inner = UnityEngine.Sprites.DataUtility.GetInnerUV(m_Sprite);
            Vector4 border = GetAdjustedBorder(m_Sprite.border / PixelsPerUnit, rect);

            float[] xs =
            {
                rect.xMin,
                rect.xMin + border.x,
                rect.xMax - border.z,
                rect.xMax
            };
            float[] ys =
            {
                rect.yMin,
                rect.yMin + border.y,
                rect.yMax - border.w,
                rect.yMax
            };
            float[] us = { outer.x, inner.x, inner.z, outer.z };
            float[] vs = { outer.y, inner.y, inner.w, outer.w };

            GetVisibleWindow(rect, out float cutMin, out float cutMax, out bool horizontal);

            for (int cx = 0; cx < 3; cx++)
            {
                for (int ry = 0; ry < 3; ry++)
                {
                    if (!m_FillCenter && cx == 1 && ry == 1)
                        continue;

                    AddPatch(vh, xs[cx], ys[ry], xs[cx + 1], ys[ry + 1],
                        us[cx], vs[ry], us[cx + 1], vs[ry + 1],
                        cutMin, cutMax, horizontal);
                }
            }
        }

        // ── Fill window ─────────────────────────────────────────────────────────────

        private void GetVisibleWindow(Rect rect, out float cutMin, out float cutMax, out bool horizontal)
        {
            horizontal = m_FillDirection == FillDirection.Left || m_FillDirection == FillDirection.Right;

            float spanStart = horizontal ? rect.xMin : rect.yMin;
            float spanEnd = horizontal ? rect.xMax : rect.yMax;
            float visible = (spanEnd - spanStart) * m_FillAmount;

            bool fromStart = m_FillDirection == FillDirection.Right || m_FillDirection == FillDirection.Up;
            if (fromStart)
            {
                cutMin = spanStart;
                cutMax = spanStart + visible;
            }
            else
            {
                cutMin = spanEnd - visible;
                cutMax = spanEnd;
            }
        }

        /// <summary>
        /// Emits a quad clipped to the visible fill window along the fill axis,
        /// interpolating UVs to match the clip.
        /// </summary>
        private void AddPatch(VertexHelper vh,
            float x0, float y0, float x1, float y1,
            float u0, float v0, float u1, float v1,
            float cutMin, float cutMax, bool horizontal)
        {
            if (horizontal)
            {
                float clampedMin = Mathf.Max(x0, cutMin);
                float clampedMax = Mathf.Min(x1, cutMax);
                if (clampedMax <= clampedMin || x1 <= x0)
                    return;

                float tMin = (clampedMin - x0) / (x1 - x0);
                float tMax = (clampedMax - x0) / (x1 - x0);
                float newU0 = Mathf.Lerp(u0, u1, tMin);
                float newU1 = Mathf.Lerp(u0, u1, tMax);
                u0 = newU0;
                u1 = newU1;
                x0 = clampedMin;
                x1 = clampedMax;
            }
            else
            {
                float clampedMin = Mathf.Max(y0, cutMin);
                float clampedMax = Mathf.Min(y1, cutMax);
                if (clampedMax <= clampedMin || y1 <= y0)
                    return;

                float tMin = (clampedMin - y0) / (y1 - y0);
                float tMax = (clampedMax - y0) / (y1 - y0);
                float newV0 = Mathf.Lerp(v0, v1, tMin);
                float newV1 = Mathf.Lerp(v0, v1, tMax);
                v0 = newV0;
                v1 = newV1;
                y0 = clampedMin;
                y1 = clampedMax;
            }

            int idx = vh.currentVertCount;
            Color32 c = color;
            vh.AddVert(new Vector3(x0, y0), c, new Vector2(u0, v0));
            vh.AddVert(new Vector3(x0, y1), c, new Vector2(u0, v1));
            vh.AddVert(new Vector3(x1, y1), c, new Vector2(u1, v1));
            vh.AddVert(new Vector3(x1, y0), c, new Vector2(u1, v0));

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx + 2, idx + 3, idx);
        }

        private Vector4 GetAdjustedBorder(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                float combined = border[axis] + border[axis + 2];
                float size = rect.size[axis];

                // Shrink borders proportionally if they don't fit the rect.
                if (size < combined && combined != 0f)
                {
                    float ratio = size / combined;
                    border[axis] *= ratio;
                    border[axis + 2] *= ratio;
                }
            }
            return border;
        }

        // ── ILayoutElement ──────────────────────────────────────────────────────────

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }
        public virtual float minWidth => 0f;
        public virtual float minHeight => 0f;
        public virtual float preferredWidth => m_Sprite != null ? UnityEngine.Sprites.DataUtility.GetMinSize(m_Sprite).x / PixelsPerUnit : 0f;
        public virtual float preferredHeight => m_Sprite != null ? UnityEngine.Sprites.DataUtility.GetMinSize(m_Sprite).y / PixelsPerUnit : 0f;
        public virtual float flexibleWidth => -1f;
        public virtual float flexibleHeight => -1f;
        public virtual int layoutPriority => 0;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_PixelsPerUnitMultiplier = Mathf.Max(0.01f, m_PixelsPerUnitMultiplier);
            m_FillAmount = Mathf.Clamp01(m_FillAmount);
        }
#endif
    }
}
