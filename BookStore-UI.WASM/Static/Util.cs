using Microsoft.AspNetCore.Components;

namespace BookStore_UI.WASM.Static
{
    public static class Util
    {
        public static void NavigateTo(NavigationManager navigationManager, string target)
        {
            navigationManager.NavigateTo(target);
        }
    }
}
