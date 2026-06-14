using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Coalballcat.Services.UI
{
    /// <summary>
    /// Base type for a "previous / next" option selector. Wires up the prev/next buttons,
    /// keeps a current index, and updates optional labels. Subclasses decide what happens
    /// at the ends of the list (see <see cref="LoopableSelectionSlider"/>).
    /// </summary>
    public abstract class SelectionSlider : MonoBehaviour
    {
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text optionLabel;
        [SerializeField] private TMP_Text optionNumberLabel;

        [SerializeField] private List<string> options = new();

        [Tooltip("Raised with the new index whenever the selection changes.")]
        public UnityEvent<int> OnIndexChanged = new();

        public IReadOnlyList<string> Options => options;
        public int Index => currentIndex;
        public int Count => options.Count;
        public string CurrentOption => (currentIndex >= 0 && currentIndex < options.Count) ? options[currentIndex] : string.Empty;

        protected int currentIndex;

        protected virtual void Awake()
        {
            if (prevButton != null) prevButton.onClick.AddListener(OnPreviousButtonPressed);
            if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonPressed);
        }

        protected virtual void OnDestroy()
        {
            if (prevButton != null) prevButton.onClick.RemoveListener(OnPreviousButtonPressed);
            if (nextButton != null) nextButton.onClick.RemoveListener(OnNextButtonPressed);
        }

        protected virtual void Start()
        {
            RefreshLabel();
        }

        public abstract void OnPreviousButtonPressed();
        public abstract void OnNextButtonPressed();

        /// <summary>Replace the option list and reset to the first entry.</summary>
        public void SetOptions(IEnumerable<string> newOptions)
        {
            options.Clear();
            if (newOptions != null)
                options.AddRange(newOptions);
            SetIndex(0, notify: false);
        }

        /// <summary>Set the current index directly. Clamped to the valid range.</summary>
        public void SetIndex(int index, bool notify = true)
        {
            if (options.Count == 0)
            {
                currentIndex = 0;
                RefreshLabel();
                return;
            }

            currentIndex = Mathf.Clamp(index, 0, options.Count - 1);
            RefreshLabel();

            if (notify)
                OnIndexChanged.Invoke(currentIndex);
        }

        protected void ApplyIndex(int index)
        {
            currentIndex = index;
            RefreshLabel();
            OnIndexChanged.Invoke(currentIndex);
        }

        protected void RefreshLabel()
        {
            if (optionLabel != null)
                optionLabel.text = options.Count > 0 ? options[currentIndex] : string.Empty;

            if (optionNumberLabel != null)
                optionNumberLabel.text = options.Count > 0 ? $"{currentIndex + 1}/{options.Count}" : "0/0";
        }
    }
}
