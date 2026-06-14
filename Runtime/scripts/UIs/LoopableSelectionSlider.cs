namespace Coalballcat.Services.UI
{
    /// <summary>
    /// Selection slider that wraps around: going past the last option returns to the
    /// first, and vice versa.
    /// </summary>
    public class LoopableSelectionSlider : SelectionSlider
    {
        public override void OnPreviousButtonPressed()
        {
            if (Count == 0) return;
            int index = Index - 1;
            if (index < 0) index = Count - 1;
            ApplyIndex(index);
        }

        public override void OnNextButtonPressed()
        {
            if (Count == 0) return;
            int index = Index + 1;
            if (index > Count - 1) index = 0;
            ApplyIndex(index);
        }
    }
}
