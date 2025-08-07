using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Resources
{
   
    public static class SharedResourcesKeys
    {
        /// <summary>
        /// General-purpose messages and labels used throughout the application.
        /// </summary>
        public static class General
        {
            public const string Welcome = "General.Welcome";
            public const string Loading = "General.Loading";
            public const string Processing = "General.Processing";
            public const string PleaseWait = "General.PleaseWait";
            public const string Search = "General.Search";
            public const string Filter = "General.Filter";
            public const string Sort = "General.Sort";
            public const string Select = "General.Select";
            public const string All = "General.All";
            public const string None = "General.None";
            public const string Yes = "General.Yes";
            public const string No = "General.No";
            public const string OK = "General.OK";
            public const string Apply = "General.Apply";
            public const string Reset = "General.Reset";
            public const string Clear = "General.Clear";
        }
        public static class Entities
        {
            public const string Department = "Entities.Department";
        }
        /// <summary>
        /// Main application navigation links.
        /// </summary>
        public static class Navigation
        {
            public const string Home = "Navigation.Home";
            public const string About = "Navigation.About";
            public const string Services = "Navigation.Services";
            public const string Contact = "Navigation.Contact";
            public const string FAQ = "Navigation.FAQ";
            public const string Blog = "Navigation.Blog";
            public const string Pricing = "Navigation.Pricing";
            public const string Portfolio = "Navigation.Portfolio";
            public const string Testimonials = "Navigation.Testimonials";
            public const string News = "Navigation.News";
            public const string Dashboard = "Navigation.Dashboard";
            public const string Reports = "Navigation.Reports";
        }



        /// <summary>
        /// Validation keys for various input, format, uniqueness, file, date, UI, security, and other checks.
        /// </summary>
        public static class Validation
        {
            // Basic validation
            public const string Required = "Validation.Required";
            public const string Invalid = "Validation.Invalid";

            // Length validations
            public const string MinLength = "Validation.MinLength";
            public const string MaxLength = "Validation.MaxLength";
            public const string ExactLength = "Validation.ExactLength";

            // Number validations
            public const string NumericOnly = "Validation.NumericOnly";
            public const string IntegerOnly = "Validation.IntegerOnly";
            public const string DecimalOnly = "Validation.DecimalOnly";
            public const string MustBeGreaterThanZero = "Validation.MustBeGreaterThanZero";
            public const string MustBeGreaterThanOrEqualToZero = "Validation.MustBeGreaterThanOrEqualToZero";
            public const string MustBePositiveNumber = "Validation.MustBePositiveNumber";
            public const string MustBeNegativeNumber = "Validation.MustBeNegativeNumber";
            public const string ValueMustBeGreaterThan = "Validation.ValueMustBeGreaterThan";
            public const string ValueMustBeLessThan = "Validation.ValueMustBeLessThan";
            public const string ValueMustBeBetween = "Validation.ValueMustBeBetween";

            // Format validations
            public const string EmailInvalid = "Validation.EmailInvalid";
            public const string UrlInvalid = "Validation.UrlInvalid";
            public const string DateInvalid = "Validation.DateInvalid";
            public const string TimeInvalid = "Validation.TimeInvalid";
            public const string DateTimeInvalid = "Validation.DateTimeInvalid";
            public const string PhoneNumberInvalid = "Validation.PhoneNumberInvalid";
            public const string CreditCardInvalid = "Validation.CreditCardInvalid";
            public const string IBANInvalid = "Validation.IBANInvalid";

            // Password validations
            public const string PasswordTooShort = "Validation.PasswordTooShort";
            public const string PasswordComplexity = "Validation.PasswordComplexity";
            public const string PasswordMissingUppercase = "Validation.PasswordMissingUppercase";
            public const string PasswordMissingLowercase = "Validation.PasswordMissingLowercase";
            public const string PasswordMissingDigit = "Validation.PasswordMissingDigit";
            public const string PasswordMissingSpecialChar = "Validation.PasswordMissingSpecialChar";
            public const string PasswordsDoNotMatch = "Validation.PasswordsDoNotMatch";

            // Uniqueness validations
            public const string UsernameAlreadyExists = "Validation.UsernameAlreadyExists";
            public const string EmailAlreadyRegistered = "Validation.EmailAlreadyRegistered";
            public const string UniqueValueRequired = "Validation.UniqueValueRequired";

            // File validations
            public const string FileTooLarge = "Validation.FileTooLarge";
            public const string InvalidFileType = "Validation.InvalidFileType";
            public const string FileDimensionsTooLarge = "Validation.FileDimensionsTooLarge";
            public const string NoFileSelected = "Validation.NoFileSelected";

            // Date validations
            public const string FutureDateRequired = "Validation.FutureDateRequired";
            public const string PastDateRequired = "Validation.PastDateRequired";
            public const string DateMustBeAfter = "Validation.DateMustBeAfter";
            public const string DateMustBeBefore = "Validation.DateMustBeBefore";
            public const string EndDateAfterStartDate = "Validation.EndDateAfterStartDate";

            // UI element validations
            public const string CheckedRequired = "Validation.CheckedRequired";
            public const string UncheckedRequired = "Validation.UncheckedRequired";
            public const string SelectAtLeastOne = "Validation.SelectAtLeastOne";
            public const string SelectMaxItems = "Validation.SelectMaxItems";
            public const string MustAcceptTerms = "Validation.MustAcceptTerms";
            public const string MustSelectOption = "Validation.MustSelectOption";

            // Security validations
            public const string ConfirmationRequired = "Validation.ConfirmationRequired";
            public const string TooManyAttempts = "Validation.TooManyAttempts";
            public const string AccountLocked = "Validation.AccountLocked";
            public const string CaptchaInvalid = "Validation.CaptchaInvalid";

            // Other validations
            public const string FieldMismatch = "Validation.FieldMismatch";
            public const string AllowedCharacters = "Validation.AllowedCharacters";
        }


        /// <summary>
        /// Common action verbs used for buttons, links, and commands.
        /// </summary>
        public static class Actions
        {
            public const string Save = "Actions.Save";
            public const string Submit = "Actions.Submit";
            public const string Cancel = "Actions.Cancel";
            public const string Edit = "Actions.Edit";
            public const string Delete = "Actions.Delete";
            public const string Create = "Actions.Create";
            public const string Add = "Actions.Add";
            public const string Update = "Actions.Update";
            public const string View = "Actions.View";
            public const string Close = "Actions.Close";
            public const string Back = "Actions.Back";
            public const string Next = "Actions.Next";
            public const string Previous = "Actions.Previous";
            public const string Refresh = "Actions.Refresh";
            public const string Continue = "Actions.Continue";
            public const string Finish = "Actions.Finish";
            public const string Send = "Actions.Send";
            public const string Print = "Actions.Print";
            public const string Share = "Actions.Share";
            public const string Copy = "Actions.Copy";
            public const string Paste = "Actions.Paste";
        }

        /// <summary>
        /// Keys related to user account management (login, registration, profile).
        /// </summary>
        public static class User
        {
            public const string Login = "User.Login";
            public const string Logout = "User.Logout";
            public const string Register = "User.Register";
            public const string Username = "User.Username";
            public const string Email = "User.Email";
            public const string Password = "User.Password";
            public const string ConfirmPassword = "User.ConfirmPassword";
            public const string ForgotPassword = "User.ForgotPassword";
            public const string ResetPassword = "User.ResetPassword";
            public const string ChangePassword = "User.ChangePassword";
            public const string CreateAccount = "User.CreateAccount";
            public const string UpdateProfile = "User.UpdateProfile";
            public const string DeleteAccount = "User.DeleteAccount";
            public const string MyProfile = "User.MyProfile";
            public const string MyOrders = "User.MyOrders";
            public const string FirstName = "User.FirstName";
            public const string LastName = "User.LastName";
            public const string FullName = "User.FullName";
            public const string PhoneNumber = "User.PhoneNumber";
            public const string DateOfBirth = "User.DateOfBirth";
            public const string Gender = "User.Gender";
            public const string Address = "User.Address";
            public const string City = "User.City";
            public const string Country = "User.Country";
            public const string RememberMe = "User.RememberMe";
        }

        /// <summary>
        /// Keys for e-commerce features like shopping cart, checkout, and products.
        /// </summary>
        public static class ECommerce
        {
            public const string AddToCart = "ECommerce.AddToCart";
            public const string RemoveFromCart = "ECommerce.RemoveFromCart";
            public const string ViewCart = "ECommerce.ViewCart";
            public const string EmptyCart = "ECommerce.EmptyCart";
            public const string Checkout = "ECommerce.Checkout";
            public const string PlaceOrder = "ECommerce.PlaceOrder";
            public const string AddToWishlist = "ECommerce.AddToWishlist";
            public const string RemoveFromWishlist = "ECommerce.RemoveFromWishlist";
            public const string ViewWishlist = "ECommerce.ViewWishlist";
            public const string Compare = "ECommerce.Compare";
            public const string QuickView = "ECommerce.QuickView";
            public const string Subtotal = "ECommerce.Subtotal";
            public const string Tax = "ECommerce.Tax";
            public const string Shipping = "ECommerce.Shipping";
            public const string Discount = "ECommerce.Discount";
            public const string Total = "ECommerce.Total";
            public const string Price = "ECommerce.Price";
            public const string Quantity = "ECommerce.Quantity";
            public const string ProductCode = "ECommerce.ProductCode";
            public const string Category = "ECommerce.Category";
            public const string Brand = "ECommerce.Brand";
            public const string InStock = "ECommerce.InStock";
            public const string OutOfStock = "ECommerce.OutOfStock";
            public const string ShippingAddress = "ECommerce.ShippingAddress";
            public const string BillingAddress = "ECommerce.BillingAddress";
            public const string PaymentMethod = "ECommerce.PaymentMethod";
            public const string PaymentInfo = "ECommerce.PaymentInfo";
            public const string OrderNumber = "ECommerce.OrderNumber";
            public const string OrderDate = "ECommerce.OrderDate";
            public const string OrderStatus = "ECommerce.OrderStatus";
            public const string TrackOrder = "ECommerce.TrackOrder";
            public const string DeliveryDate = "ECommerce.DeliveryDate";
            public const string CouponCode = "ECommerce.CouponCode";
            public const string ApplyCoupon = "ECommerce.ApplyCoupon";
        }

        /// <summary>
        /// Keys related to file operations like upload, download, and export.
        /// </summary>
        public static class File
        {
            public const string Upload = "File.Upload";
            public const string Download = "File.Download";
            public const string Export = "File.Export";
            public const string Import = "File.Import";
            public const string Sync = "File.Sync";
            public const string Syncing = "File.Syncing";
            public const string Browse = "File.Browse";
            public const string SelectFile = "File.SelectFile";
            public const string DropFiles = "File.DropFiles";
            public const string FileName = "File.FileName";
            public const string FileSize = "File.FileSize";
            public const string FileType = "File.FileType";
            public const string LastModified = "File.LastModified";
        }

        /// <summary>
        /// Keys for settings, preferences, and configuration panels.
        /// </summary>
        public static class Settings
        {
            public const string Title = "Settings.Title";
            public const string Preferences = "Settings.Preferences";
            public const string GeneralSettings = "Settings.GeneralSettings";
            public const string AccountSettings = "Settings.AccountSettings";
            public const string PrivacySettings = "Settings.PrivacySettings";
            public const string SecuritySettings = "Settings.SecuritySettings";
            public const string Language = "Settings.Language";
            public const string Theme = "Settings.Theme";
            public const string LightTheme = "Settings.LightTheme";
            public const string DarkTheme = "Settings.DarkTheme";
            public const string AutoTheme = "Settings.AutoTheme";
            public const string Notifications = "Settings.Notifications";
            public const string EmailNotifications = "Settings.EmailNotifications";
            public const string PushNotifications = "Settings.PushNotifications";
            public const string Enable = "Settings.Enable";
            public const string Disable = "Settings.Disable";
            public const string ResetToDefault = "Settings.ResetToDefault";
            public const string SaveChanges = "Settings.SaveChanges";
            public const string DiscardChanges = "Settings.DiscardChanges";
        }

        /// <summary>
        /// Feedback messages indicating successful operations.
        /// </summary>
        public static class Success
        {
            public const string Created = "Success.Created";
            public const string Updated = "Success.Updated";
            public const string Deleted = "Success.Deleted";
            public const string Saved = "Success.Saved";
            public const string Uploaded = "Success.Uploaded";
            public const string Downloaded = "Success.Downloaded";
            public const string Imported = "Success.Imported";
            public const string Exported = "Success.Exported";
            public const string OperationSuccessful = "Success.OperationSuccessful";
            public const string Sent = "Success.Sent";
            public const string Copied = "Success.Copied";
            public const string Activated = "Success.Activated";
            public const string Deactivated = "Success.Deactivated";
        }

        /// <summary>
        /// Feedback messages for errors and failed operations.
        /// </summary>
        public static class Error
        {
            public const string GeneralError = "Error.GeneralError";
            public const string NetworkError = "Error.NetworkError";
            public const string ServerError = "Error.ServerError";
            public const string ValidationError = "Error.ValidationError";
            public const string RequiredField = "Error.RequiredField";
            public const string InvalidEmail = "Error.InvalidEmail";
            public const string InvalidPassword = "Error.InvalidPassword";
            public const string PasswordMismatch = "Error.PasswordMismatch";
            public const string LoginFailed = "Error.LoginFailed";
            public const string AccessDenied = "Error.AccessDenied";
            public const string FileNotFound = "Error.FileNotFound";
            public const string FileTooLarge = "Error.FileTooLarge";
            public const string InvalidFileType = "Error.InvalidFileType";
            public const string UploadFailed = "Error.UploadFailed";
            public const string DownloadFailed = "Error.DownloadFailed";
            public const string ConnectionTimeout = "Error.ConnectionTimeout";
            public const string SessionExpired = "Error.SessionExpired";
            public const string UnauthorizedAccess = "Error.UnauthorizedAccess";
            public const string InvalidOperation = "Error.InvalidOperation";
            public const string DataNotFound = "Error.DataNotFound";
            public const string DataNotFoundWithParameter = "Error.DataNotFoundWithParameter";
            public const string UnprocessableEntity = "Error.UnprocessableEntity";
            public const string BadRequest = "Error.BadRequest";
        }

        /// <summary>
        /// Prompts asking the user to confirm a critical action.
        /// </summary>
        public static class Confirm
        {
            public const string Delete = "Confirm.Delete";
            public const string Save = "Confirm.Save";
            public const string Cancel = "Confirm.Cancel";
            public const string Exit = "Confirm.Exit";
            public const string Logout = "Confirm.Logout";
            public const string Reset = "Confirm.Reset";
            public const string Overwrite = "Confirm.Overwrite";
            public const string UnsavedChanges = "Confirm.UnsavedChanges";
        }

        /// <summary>
        /// Keys related to time, date, and relative time expressions.
        /// </summary>
        public static class Time
        {
            public const string Today = "Time.Today";
            public const string Yesterday = "Time.Yesterday";
            public const string Tomorrow = "Time.Tomorrow";
            public const string Now = "Time.Now";
            public const string Recently = "Time.Recently";
            public const string JustNow = "Time.JustNow";
            public const string MinutesAgo = "Time.MinutesAgo";
            public const string HoursAgo = "Time.HoursAgo";
            public const string DaysAgo = "Time.DaysAgo";
            public const string WeeksAgo = "Time.WeeksAgo";
            public const string MonthsAgo = "Time.MonthsAgo";
            public const string YearsAgo = "Time.YearsAgo";
            public const string StartDate = "Time.StartDate";
            public const string EndDate = "Time.EndDate";
            public const string CreatedDate = "Time.CreatedDate";
            public const string ModifiedDate = "Time.ModifiedDate";
            public const string ExpiryDate = "Time.ExpiryDate";
        }

        /// <summary>
        /// Common status indicators for objects and processes.
        /// </summary>
        public static class Status
        {
            public const string Active = "Status.Active";
            public const string Inactive = "Status.Inactive";
            public const string Pending = "Status.Pending";
            public const string Approved = "Status.Approved";
            public const string Rejected = "Status.Rejected";
            public const string Completed = "Status.Completed";
            public const string InProgress = "Status.InProgress";
            public const string Cancelled = "Status.Cancelled";
            public const string Draft = "Status.Draft";
            public const string Published = "Status.Published";
            public const string Archived = "Status.Archived";
            public const string Expired = "Status.Expired";
            public const string Locked = "Status.Locked";
            public const string Unlocked = "Status.Unlocked";
            public const string Online = "Status.Online";
            public const string Offline = "Status.Offline";
            public const string Available = "Status.Available";
            public const string Unavailable = "Status.Unavailable";
            public const string Busy = "Status.Busy";
        }

        /// <summary>
        /// Keys for pagination controls.
        /// </summary>
        public static class Pagination
        {
            public const string FirstPage = "Pagination.FirstPage";
            public const string LastPage = "Pagination.LastPage";
            public const string NextPage = "Pagination.NextPage";
            public const string PreviousPage = "Pagination.PreviousPage";
            public const string Page = "Pagination.Page";
            public const string Of = "Pagination.Of";
            public const string ItemsPerPage = "Pagination.ItemsPerPage";
            public const string ShowingItems = "Pagination.ShowingItems";
            public const string NoItems = "Pagination.NoItems";
            public const string Results = "Pagination.Results";
        }

        /// <summary>
        /// Keys for search and filter UI elements.
        /// </summary>
        public static class Search
        {
            public const string SearchResults = "Search.SearchResults";
            public const string NoResults = "Search.NoResults";
            public const string SearchFor = "Search.SearchFor";
            public const string SearchQuery = "Search.SearchQuery";
            public const string AdvancedSearch = "Search.AdvancedSearch";
            public const string ClearSearch = "Search.ClearSearch";
            public const string FilterBy = "Search.FilterBy";
            public const string SortBy = "Search.SortBy";
            public const string Ascending = "Search.Ascending";
            public const string Descending = "Search.Descending";
            public const string Name = "Search.Name";
            public const string Date = "Search.Date";
            public const string Size = "Search.Size";
            public const string Type = "Search.Type";
        }

        /// <summary>
        /// Labels for common form fields.
        /// </summary>
        public static class Form
        {
            public const string Title = "Form.Title";
            public const string Description = "Form.Description";
            public const string Content = "Form.Content";
            public const string Message = "Form.Message";
            public const string Comments = "Form.Comments";
            public const string Tags = "Form.Tags";
            public const string Keywords = "Form.Keywords";
            public const string URL = "Form.URL";
            public const string Link = "Form.Link";
            public const string Image = "Form.Image";
            public const string Video = "Form.Video";
            public const string Audio = "Form.Audio";
            public const string Document = "Form.Document";
            public const string Attachment = "Form.Attachment";
            public const string Priority = "Form.Priority";
            public const string Status = "Form.Status";
            public const string Notes = "Form.Notes";
            public const string Remarks = "Form.Remarks";
        }

        /// <summary>
        /// Keys for help and support sections.
        /// </summary>
        public static class Help
        {
            public const string HelP = "Help.Help";
            public const string Support = "Help.Support";
            public const string Documentation = "Help.Documentation";
            public const string Tutorial = "Help.Tutorial";
            public const string FAQ = "Help.FAQ";
            public const string ContactSupport = "Help.ContactSupport";
            public const string UserGuide = "Help.UserGuide";
            public const string TechnicalSupport = "Help.TechnicalSupport";
            public const string CustomerService = "Help.CustomerService";
            public const string LiveChat = "Help.LiveChat";
            public const string TicketSystem = "Help.TicketSystem";
            public const string CreateTicket = "Help.CreateTicket";
            public const string ViewTickets = "Help.ViewTickets";
        }

        /// <summary>
        /// Miscellaneous keys for various UI elements and labels.
        /// </summary>
        public static class Misc
        {
            public const string Required = "Misc.Required";
            public const string Optional = "Misc.Optional";
            public const string More = "Misc.More";
            public const string Less = "Misc.Less";
            public const string ShowMore = "Misc.ShowMore";
            public const string ShowLess = "Misc.ShowLess";
            public const string Expand = "Misc.Expand";
            public const string Collapse = "Misc.Collapse";
            public const string Details = "Misc.Details";
            public const string Summary = "Misc.Summary";
            public const string Information = "Misc.Information";
            public const string Warning = "Misc.Warning";
            public const string Error = "Misc.Error";
            public const string Success = "Misc.Success";
            public const string Notice = "Misc.Notice";
            public const string Alert = "Misc.Alert";
            public const string Tip = "Misc.Tip";
            public const string Version = "Misc.Version";
            public const string Copyright = "Misc.Copyright";
            public const string AllRightsReserved = "Misc.AllRightsReserved";
            public const string PrivacyPolicy = "Misc.PrivacyPolicy";
            public const string TermsOfService = "Misc.TermsOfService";
            public const string TermsAndConditions = "Misc.TermsAndConditions";
        }
    }
}
