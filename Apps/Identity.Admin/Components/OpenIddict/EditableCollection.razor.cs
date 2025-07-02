using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class EditableCollection<TItem, TValue> : ComponentBase
    {
        private int _itemSize = 3;
        [Parameter]
        [SuppressMessage("Usage", "BL0007:Component parameters should be auto properties", Justification = "The validation in Set would be lost.")]
        public int ItemSize
        {
            get => _itemSize;
            set
            {
                if(value >0 && value <= 12)
                {
                    _itemSize = value;
                }
            }
        }

        private int _spacing = 2;
        [Parameter]
        [SuppressMessage("Usage", "BL0007:Component parameters should be auto properties", Justification = "The validation in Set would be lost.")]
        public int Spacing
        {
            get => _spacing;
            set
            {
                if(value is > 0 and <= 16)
                {
                    _spacing = value;
                }
            }
        }

        [Parameter]
        public required IEnumerable<TItem> Items { get; set; }
      
        [Parameter]
        public required RenderFragment<TItem> ItemTemplate { get; set; }     

        [Parameter]
        public EventCallback<TItem> OnDeleteItem { get; set; }

        [Parameter]
        public required TValue NewItem { get; set; }

        [Parameter]
        public required RenderFragment<TValue> AddItemTemplate { get; set; }

        [Parameter]
        public EventCallback<TValue> OnAddNewItem { get; set; }

        private bool CanAddItem => AddItemTemplate != null; // no fix

    }
}