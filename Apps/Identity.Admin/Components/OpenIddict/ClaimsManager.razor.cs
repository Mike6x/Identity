using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class ClaimsManager : ComponentBase
    {

        [Parameter]
        public required IEnumerable<ClaimViewModel> Claims { get; set; }

        [Parameter]
        public EventCallback OnAddItem { get; set; }

        [Parameter]
        public EventCallback<ClaimViewModel> OnDeleteItem { get; set; }
        
        [Parameter]
        public required Func<ClaimViewModel, ClaimViewModel, Task<bool>> OnUpdateItem { get; set; }

        private MudTable<ClaimViewModel>? _table;
        
        private ClaimViewModel _selectedClaim = new();
        private ClaimViewModel _elementBeforeEdit = new();
        
        private string _searchString = string.Empty;

        private void EditItem(ClaimViewModel model)
        {
            _selectedClaim = model;
            _table?.SetEditingItem(model);
            BackupItem(model);
        }

        private async Task UpdateItemAsync()
        {
            var success = await this.OnUpdateItem(_elementBeforeEdit, _selectedClaim);
            if(!success)
            {
                ResetItemToOriginalValues(_selectedClaim);
            }    
        }

        private void BackupItem(object element)
        {
            if(element is ClaimViewModel beforeEdit)
            {
                _elementBeforeEdit = new ClaimViewModel
                {
                    Type = beforeEdit.Type,
                    Value = beforeEdit.Value,
                    IncludeInAccessToken = beforeEdit.IncludeInAccessToken,
                    IncludeInIdentityToken = beforeEdit.IncludeInIdentityToken
                };
            }
           
        }

        private void ResetItemToOriginalValues(object element)
        {
            ((ClaimViewModel)element).Type = _elementBeforeEdit.Type;
            ((ClaimViewModel)element).Value = _elementBeforeEdit.Value;
            ((ClaimViewModel)element).IncludeInAccessToken = _elementBeforeEdit.IncludeInAccessToken;
            ((ClaimViewModel)element).IncludeInIdentityToken = _elementBeforeEdit.IncludeInIdentityToken;
        }

        private bool FilterFunc(ClaimViewModel element)
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (element.Type.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Value.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
