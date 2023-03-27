namespace OrdersManager
{
    public static class Routes
    {
        /// <summary>
        /// Default Uri separator.
        /// </summary>
        private const string Slash = "/";

        /// <summary>
        /// Default API endpoint prefix.
        /// </summary>
        private const string Prefix = $"api{Slash}";
      
        /// <summary>
        /// Base API route.
        /// </summary>
        private const string Base = $"{Prefix}";

        /// <summary>
        /// Static class that contains Orders API endpoint routes.
        /// </summary>
        public static class Orders
        {
            /// <summary>
            /// ip address + port for API
            /// </summary>
            public const string _apiAddress = "https://localhost:7063/";
            /// <summary>
            /// Base controller route.
            /// </summary>
            private const string Controller = $"{_apiAddress}{Base}{nameof(Orders)}{Slash}";

            public const string AllOrders = Controller;
            /// <summary>
            /// Providers list endpoint route.
            /// </summary>
            public const string Providers = $"{Controller}{nameof(Providers)}";

            /// <summary>
            /// Order CreateEdit endpoint route.
            /// </summary>
            public const string CreateEditOrder = $"{Controller}{nameof(CreateEditOrder)}";

            /// <summary>
            /// Order by id
            /// </summary>
            public const string Order = $"{Controller}{nameof(Order)}{Slash}";

            /// <summary>
            /// Provider by id
            /// </summary>
            public const string Provider = $"{Providers}{nameof(Provider)}{Slash}";

        }
    }
}
