using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.ComponentModel;

namespace MDD4All.DME.Views.EditorView
{
    public partial class PrimitivePropertyEditorView : IDisposable
    {
        #region Parameters
        [Parameter]
        public PrimitivePropertyViewModel ViewModel { get; set; } = null!;

        [Parameter]
        public bool IsCompact { get; set; } = false;

        [Parameter]
        public bool ShowTitle { get; set; } = true;
        #endregion

        #region Private Fields
        private string? _localValue;
        #endregion

        #region Lifecycle and Event Subscription
        protected override void OnParametersSet()
        {
            if (this.ViewModel != null)
            {
                // Initialize the local UI state from the ViewModel
                this._localValue = this.ViewModel.Item?.ToString();

                // Subscribe to property changes for snap-back support
                this.ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        public void Dispose()
        {
            if (this.ViewModel != null)
            {
                // Unsubscribe to avoid memory leaks
                this.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PrimitivePropertyViewModel.Item))
            {
                // If the ViewModel item changes (e.g. reverted by the Dictionary logic),
                // we must update our local shadow variable and refresh the UI.
                this._localValue = this.ViewModel.Item?.ToString();
                this.InvokeAsync(this.StateHasChanged);
            }
        }
        #endregion

        #region Event Handlers
        private void OnInput(ChangeEventArgs e)
        {
            // Only update the local value while the user is typing
            this._localValue = e.Value?.ToString();
        }

        private void OnCommit(ChangeEventArgs e)
        {
            string? newValue = e.Value?.ToString();

            // Push the change to the ViewModel
            this.ViewModel.Item = newValue;

            // Re-sync local value immediately. If the ViewModel rejected the change,
            // this restores the previous valid state.
            this._localValue = this.ViewModel.Item?.ToString();
        }

        private void OnBlur()
        {
            // Final sync when the field loses focus
            this._localValue = this.ViewModel.Item?.ToString();
            this.StateHasChanged();
        }

        private void OnIntegerChanged(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            // Remove non-digit characters except for a leading minus sign
            var cleanedValue = new string(value.Where((c, index) =>
                char.IsDigit(c) || (index == 0 && c == '-')
            ).ToArray());

            if (cleanedValue == "-" || string.IsNullOrEmpty(cleanedValue)) return;

            if (long.TryParse(cleanedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
            {
                try
                {
                    // Convert to the specific target type (int, long, sbyte, etc.)
                    this.ViewModel.Item = Convert.ChangeType(result, this.ViewModel.Type!);
                }
                catch (OverflowException)
                {
                    // Value is too large for the target type
                }
            }

            this.StateHasChanged();
        }

        private void OnDateTimeChanged(string? value)
        {
            if (DateTime.TryParse(value, out DateTime dt))
            {
                this.ViewModel.Item = dt;
                this.StateHasChanged();
            }
        }
        #endregion
    }
}