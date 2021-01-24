using Microsoft.AspNetCore.Components;

namespace BookStore_UI.Static
{
    public static class Util
    {
        public static void NavigateTo(NavigationManager navigationManager, string target)
        {
            navigationManager.NavigateTo(target);
        }
    }
}
