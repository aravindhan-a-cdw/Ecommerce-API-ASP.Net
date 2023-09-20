namespace EcommerceAPI.Utilities
{
    public static class Constants
	{
		public static string JWT_SECRET_CONFIGURATION_KEY { get; } = "ApiSettings:Secret";

        public static string POSTGRES_CONFIGURATION_KEY { get; } = "PostgresqlConnection";

		public static string REDIS_CONFIGURATION_KEY { get; } = "RedisCacheConnection";

		public static class Messages
		{
			public static string CONFIGURATION_KEY_NOT_SET { get; } = "Some Value has not been set in the Configuration file! Jwt Secret, Postgres connection string and redis connection string are required";

			public static string UNHANDLED_EXCEPTION { get; } = "Some unhandled exception occurred!";

            public const string DATA_NOT_VALID = "Data not valid!";
            public const string EMAIL_EXISTS = "Email already exists!";
            public const string AGE_NOT_ELIGIBLE = "You are not 18 years old yet! Come back later!";
            public const string BANNED_PRODUCT = "The product has been banned by government";
            public const string DISCLAIMER = "The requested product has an on going Trial and this platform is not responsible for the health quality of this product.";
            public const string RESTRICT_QUANTITY = "The product has an ongoing trial and hence the maximum quantity must be less than or equal to 10";
        }

		public static class Roles
		{
			public const string CUSTOMER = "customer";

			public const string ADMIN = "admin";
		}

        public static class CacheExpiration
        {
            public const int BANNED_PRODUCTS_EXPIRY = 60 * 60 * 2; // 2 hours
            public const int GET_PRODUCT_EXPIRY = 60 * 60; // 1 hour
            public const int GET_ALL_PRODUCT_EXPIRY = 60 * 5; // 5 mins
        }

        public static class Utility
        {
            public const string BANNED_PRODUCTS_URL = "https://65014f45736d26322f5b7b24.mockapi.io/cosmo/ecart";

        }

        public static class Routes
        {
            public const string CONTROLLER = "[controller]";

            public static class Cart
            {
                public const string GET_CART = "GetCart";
                public const string ADD_PRODUCT_TO_CART = "AddProductToCart";
                public const string UPDATE_PRODUCT_IN_CART = "UpdateProductInCart";
                public const string DELETE_PRODUCT_IN_CART = "DeleteProductInCart";
                public const string PROCEED_TO_CHECKOUT = "ProceedToCheckout";

            }

            public static class Product
            {
                public const string GET_ALL_PRODUCTS = "GetAllProducts";
                public const string GET_PRODUCT = "GetProduct";
                public const string CREATE_PRODUCT = "CreateProduct";
                public const string UPDATE_PRODUCT = "UpdateProduct";
                public const string DELETE_PRODUCT = "DeleteProduct";
            }

            public static class User
            {
                public const string GET_USER = "GetUser";
                public const string CUSTOMER_REGISTRATION = "CustomerRegistration";
                public const string ADMIN_REGISTRATION = "AdminRegistration";
                public const string USER_LOGIN = "UserLogin";
                public const string USER_LOGOUT = "UserLogout";
            }

        }

		public static class Swagger
		{
			public static string JWT_SECURITY_DESCRIPTION { get; } = "JWT Authorization using Bearer Scheme in Header";

            public static class Cart
            {
                public const string GET_CART_SUMMARY = "Get Products in Cart";
                public const string GET_CART_DESCRIPTION = "This endpoint allows Customer to get Products in the cart";

                public const string ADD_PRODUCT_TO_CART_SUMMARY = "Add Product to cart";
                public const string ADD_PRODUCT_TO_CART_DESCRIPTION = "This endpoint allows Customer to add Product to cart or increase the quantity of the Product in the cart";

                public const string UPDATE_PRODUCT_IN_CART_SUMMARY = "Update the Products in the cart";
                public const string UPDATE_PRODUCT_IN_CART_DESCRIPTION = "This endpoint allows Customer to Update the quantity in the Cart";

                public const string DELETE_PRODUCT_IN_CART_SUMMARY = "Delete a Product from cart";
                public const string DELETE_PRODUCT_IN_CART_DESCRIPTION = "This endpoint allows Customer to delete a Product from Cart";

                public const string PROCEED_TO_CHECKOUT_SUMMARY = "Proceed to checkout with the products in the Cart";
                public const string PROCEED_TO_CHECKOUT_DESCRIPTION = "This endpoint allows Customer to create Order with the products in the Cart";

            }

            public static class Category
			{
				public const string GET_ALL_SUMMARY = "Get all Categories and its related Products";
				public const string GET_ALL_DESCRIPTION = "This endpoint provides all the categories and a list of all its related products";

				public const string GET_CATEGORY_SUMMARY = "Get a Category and list of its related Products";
				public const string GET_CATEGORY_DESCRIPTION = "This endpoint gets a category and its list of related products";

				public const string CREATE_CATEGORY_DESCRIPTION = "This endpoint allows the admin to create a new category";
				public const string CREATE_CATEGORY_SUMMARY = "Create a new Category";

                public const string UPDATE_CATEGORY_SUMMARY = "Update a category by providing its id";
                public const string UPDATE_CATEGORY_DESCRIPTION = "This endpoint allows the admin to update category";

                public const string DELETE_CATEGORY_SUMMARY = "Delete a category by providing its id";
                public const string DELETE_CATEGORY_DESCRIPTION = "This endpoint allows the admin to delete a category";

            }

			public static class Inventory
			{
                public const string GET_INVENTORIES_SUMMARY = "Get all inventories of all products";
                public const string GET_INVENTORIES_DESCRIPTION = "This endpoint allows admin to get all inventories of all products";

                public const string GET_ALL_INVENTORY_OF_PRODUCT_SUMMARY = "Get All Inventory of a Product";
                public const string GET_ALL_INVENTORY_OF_PRODUCT_DESCRIPTION = "This endpoint allows admin to get all the inventory of products";

                public const string GET_INVENTORY_SUMMARY = "Get a single Inventory with Id";
                public const string GET_INVENTORY_DESCRIPTION = "This endpoint allows admin to view Inventory of a Product";

                public const string CREATE_INVENTORY_SUMMARY = "Create New Inventory for Product";
                public const string CREATE_INVENTORY_DESCRIPTION = "This endpoint allows admin to create New Inventory";

                public const string UPDATE_INVENTORY_SUMMARY = "Update the inventory of a Product";
                public const string UPDATE_INVENTORY_DESCRIPTION = "This endpoint allows Admin to update the inventory of a product";

                public const string DELETE_INVENTORY_SUMMARY = "Delete a Inventory for a Product";
                public const string DELETE_INVENTORY_DESCRIPTION = "This endpoint allows Admin to delete Inventory of a Product";
            }

            public static class Product
            {
                public const string GET_ALL_PRODUCTS_SUMMARY = "Get all Products";
                public const string GET_ALL_PRODUCTS_DESCRIPTION = "This endpoint gets a list of all products";

                public const string GET_PRODUCT_SUMMARY = "Get details of a Single Product";
                public const string GET_PRODUCT_DESCRIPTION = "This endpoint gets a product with its id";

                public const string CREATE_PRODUCT_SUMMARY = "Create a new product";
                public const string CREATE_PRODUCT_DESCRIPTION = "This endpoint allows admin to create a new product";

                public const string UPDATE_PRODUCT_SUMMARY = "Update Product details";
                public const string UPDATE_PRODUCT_DESCRIPTION = "This endpoint allows admin to update a product";

                public const string DELETE_PRODUCT_SUMMARY = "Delete Product";
                public const string DELETE_PRODUCT_DESCRIPTION = "This endpoint allows admin to delete a product";

            }

            public static class User
            {
                public const string GET_USER_SUMMARY = "Get user details";
                public const string GET_USER_DESCRIPTION = "This endpoint retrieves user details and users orders and cart items";

                public const string CUSTOMER_REGISTRATION_SUMMARY = "Create a new Customer";
                public const string CUSTOMER_REGISTRATION_DESCRIPTION = "This endpoint allows to register a new Customer";

                public const string ADMIN_REGISTRATION_SUMMARY = "Create new Admin";
                public const string ADMIN_REGISTRATION_DESCRIPTION = "This endpoint allows to create a new Admin user";

                public const string USER_LOGIN_SUMMARY = "Login user and get access token";
                public const string USER_LOGIN_DESCRIPTION = "This endpoint allows users to login in and get an access token";

                public const string USER_LOGOUT_SUMMARY = "Logout user and invalidate the token";
                public const string USER_LOGOUT_DESCRIPTION = "This endpoint allows user to logout and invalidate the access token";

            }
        }
	}
}

