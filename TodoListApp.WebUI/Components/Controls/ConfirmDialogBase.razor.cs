using Microsoft.AspNetCore.Components;

public class ConfirmDialogBase : ComponentBase
{
    [Parameter] public bool Show { get; set; }
    [Parameter] public string Title { get; set; } = "Confirm";
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Event returns true for OK, false for Cancel
    [Parameter] public EventCallback<bool> OnClose { get; set; }

    protected async Task OnOkClicked()
    {
        await OnClose.InvokeAsync(true);
    }

    protected async Task OnCancelClicked()
    {
        await OnClose.InvokeAsync(false);
    }
}
