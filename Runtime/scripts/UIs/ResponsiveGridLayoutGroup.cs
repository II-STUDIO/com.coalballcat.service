using UnityEngine;
using UnityEngine.UI;

namespace Coalballcat.Services.UI
{

    public class ResponsiveGridLayoutGroup : GridLayoutGroup
    {
        [SerializeField] private float widthMultiple = 1f;
        [SerializeField] private float heightMultiple = 1f;
        [SerializeField] private bool applyWidth = true;
        [SerializeField] private bool applyHeight = true;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            CalculateCellConstraint();
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputVertical();

            CalculateCellConstraint();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            CalculateCellConstraint();
        }
#endif

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();

            CalculateCellConstraint();

        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();

            CalculateCellConstraint();

        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            CalculateCellConstraint();

        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();

            CalculateCellConstraint();
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();

            CalculateCellConstraint();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CalculateCellConstraint();
        }

        private void CalculateCellConstraint()
        {
            CalculateFixedColumn();
            CalculateFixedRow();
        }

        private void CalculateFixedColumn()
        {
            if (constraint != Constraint.FixedColumnCount)
                return;

            int columCount = constraintCount;

            if (columCount < 1)
                columCount = 1;

            float width = rectTransform.rect.width;
            float spacingValue = spacing.x * columCount;

            ApplySize(constraintCount, width, spacingValue);
        }

        private void CalculateFixedRow()
        {
            if (constraint != Constraint.FixedRowCount)
                return;

            int rawCount = constraintCount;

            if (rawCount < 1)
                rawCount = 1;

            float height = rectTransform.rect.height;
            float spacingValue = spacing.y * rawCount;

            ApplySize(rawCount, height, spacingValue);
        }

        private void ApplySize(int constraintCount, float constraintSize, float spacingValue)
        {
            float resultArea = constraintSize - spacingValue;

            float resultSize = resultArea / constraintCount;

            Vector2 size = cellSize;

            if (applyWidth)
                size.x = resultSize * widthMultiple;

            if (applyHeight)
                size.y = resultSize * heightMultiple;

            cellSize = size;
        }
    }
}