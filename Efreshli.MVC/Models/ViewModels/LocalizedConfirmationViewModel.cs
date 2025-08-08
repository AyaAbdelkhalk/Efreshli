using Efreshli.Application.Resources;
using Microsoft.Extensions.Localization;

namespace Efreshli.MVC.Models.ViewModels
{
    public class LocalizedConfirmationViewModel
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public int ItemId { get; set; }

        // Use your existing resource keys
        public string TitleKey { get; set; } = SharedResourcesKeys.Confirm.Delete;
        public string MessageKey { get; set; } = SharedResourcesKeys.Confirm.Delete;
        public string ConfirmButtonKey { get; set; } = SharedResourcesKeys.Actions.Delete;
        public string CancelButtonKey { get; set; } = SharedResourcesKeys.Actions.Cancel;

        // For dynamic entity names in messages
        public string EntityNameKey { get; set; }
        public string EntityValue { get; set; }
    }
}
