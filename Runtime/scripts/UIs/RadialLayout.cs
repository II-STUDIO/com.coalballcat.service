using UnityEngine;
using UnityEngine.UI;

namespace Coalballcat.Services.UI
{

    public class RadialLayout : LayoutGroup
    {
        public float fDistance;
        public bool preventOverlaping = true;
        [Range(0f, 360f)]
        public float MinAngle, MaxAngle, StartAngle;

        protected override void Start()
        {
            base.Start();
#if !UNITY_EDITOR
        CalculateRadial();
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CalculateRadial();
        }

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();

            CalculateRadial();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            CalculateRadial();
        }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        public override void CalculateLayoutInputVertical()
        {
            CalculateRadial();
        }
        public override void CalculateLayoutInputHorizontal()
        {
            CalculateRadial();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateRadial();
        }
#endif

        public void CalculateRadial()
        {
            m_Tracker.Clear();

            int childCount = transform.childCount;

            if (childCount == 0)
                return;

            if (preventOverlaping)
            {
                float angleIntarvel = 360f / childCount;
                MaxAngle = Mathf.Clamp(MaxAngle, 0f, 360f - angleIntarvel);
            }

            float fOffsetAngle = ((MaxAngle - MinAngle)) / (childCount - 1);

            float fAngle = StartAngle;
            for (int i = 0; i < childCount; i++)
            {
                RectTransform child = (RectTransform)transform.GetChild(i);

                if (child != null)
                {
                    //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                    m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                    Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                    child.localPosition = vPos * fDistance;
                    //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                    child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                    fAngle += fOffsetAngle;
                }
            }

        }
    }
}