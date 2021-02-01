namespace BookStore_UI.Static
{
    public static class Endpoints
    {
#if DEBUG
        public static string BaseUrl = "https://localhost:44361/api/";
#else
        public static string BaseUrl = "https://bookstore-api20210131111843.azurewebsites.net/api/";
#endif        
        public static string AuthorsEndpoint = $"{BaseUrl}authors/";
        public static string BooksEndpoint = $"{BaseUrl}books/";

        public static string UsersUrl = $"{BaseUrl}users/";
        public static string LoginEndpoint = $"{UsersUrl}login/";
        public static string RegisterEndpoint = $"{UsersUrl}register/";
    }
}
