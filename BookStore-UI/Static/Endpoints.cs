﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Static
{
    public static class Endpoints
    {
        public static string BaseUrl = "https://localhost:44361/api/";
        public static string AuthorsEndpoint = $"{BaseUrl}authors/";
        public static string BooksEndpoint = $"{BaseUrl}books/";

        public static string UsersUrl = $"{BaseUrl}users/";
        public static string LoginEndpoint = $"{UsersUrl}login/";
        public static string RegisterEndpoint = $"{UsersUrl}register/";
    }
}
