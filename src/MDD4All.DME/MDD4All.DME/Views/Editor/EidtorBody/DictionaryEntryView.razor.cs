using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;
using MDD4All.DME.ViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class DictionaryEntryView : ComponentBase
    {
        [Parameter]
        public DictionaryEntryViewModel DataContext { get; set; } = null!;

        [Parameter]
        public bool DeleteMode { get; set; } = false;

        [Parameter] public int MaxDepth { get; set; }
        [Parameter] public int CurrentDepth { get; set; }

        /// <summary>
        /// Führt den Löschbefehl für diesen spezifischen Dictionary-Eintrag aus.
        /// </summary>
        private void OnDeleteEntry()
        {
            if (DataContext.DeleteCommand != null && DataContext.DeleteCommand.CanExecute(null))
            {
                DataContext.DeleteCommand.Execute(null);
            }
        }

        private Dictionary<string, object> GetParameters(ObjectEditorViewModel? vm)
        {
            var parameters = new Dictionary<string, object>();

            if (vm == null) return parameters;

            if (vm is PrimitivePropertyViewModel pvm)
            {
                // Primitives: Nur Daten und Kompakt-Flag
                parameters.Add("ViewModel", pvm);
                parameters.Add("IsCompact", true);
            }
            else
            {
                // Komplexe Typen: Brauchen alles für die Rekursion
                parameters.Add("DataContext", vm);
                parameters.Add("MaxDepth", MaxDepth);
                parameters.Add("CurrentDepth", CurrentDepth);
                parameters.Add("IsCompact", true);
            }
            return parameters;
        }

        private Type GetViewType(ObjectEditorViewModel? vm)
        {
            if (vm == null) return typeof(PrimitivePropertyEditorView); // Fallback

            if (vm is PrimitivePropertyViewModel) return typeof(PrimitivePropertyEditorView);
            if (vm is ComplexObjectEditorViewModel) return typeof(ComplexObjectEditorView);
            if (vm is IndexedCollectionEditorViewModel) return typeof(IndexedCollectionEditorView);
            if (vm is DictionaryEditorViewModel) return typeof(DictionaryEditorView);

            return typeof(PrimitivePropertyEditorView);
        }
    }
}