using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Core
{
    public enum LogType { Info, Error }

    public class Settings
    {
        public static string DateTimeFormat => "yyyy/MM/dd HH:mm:ss";
        public static string AccessToken => ConfigurationManager.AppSettings[nameof(AccessToken)];
        public static int BatchSize => int.Parse(ConfigurationManager.AppSettings[nameof(BatchSize)]);
        public static int MaxRetries => int.Parse(ConfigurationManager.AppSettings[nameof(MaxRetries)]);
        public static int ProductSyncFrequency => int.Parse(ConfigurationManager.AppSettings[nameof(ProductSyncFrequency)]);
        //public static string StockLevelSyncFrequency => ConfigurationManager.AppSettings[nameof(StockLevelSyncFrequency)];
        public static int OrderSyncFrequency => int.Parse(ConfigurationManager.AppSettings[nameof(OrderSyncFrequency)]);
        public static bool RunProductSync => Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(RunProductSync)]);
        public static bool RunOrderSync => Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(RunOrderSync)]);
        public static string UserID => ConfigurationManager.AppSettings[nameof(UserID)];
        public static int InvoiceAddressTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(InvoiceAddressTypeID)]);
        public static int DeliveryAddressTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(DeliveryAddressTypeID)]);
        public static int RegionID => int.Parse(ConfigurationManager.AppSettings[nameof(RegionID)]);
        public static int OrderTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(OrderTypeID)]);
        public static int OrderStatusID => int.Parse(ConfigurationManager.AppSettings[nameof(OrderStatusID)]);
        public static string OrderItemInstanceCollectedStatusID => ConfigurationManager.AppSettings[nameof(OrderItemInstanceCollectedStatusID)];
        public static int CurrencyID => int.Parse(ConfigurationManager.AppSettings[nameof(CurrencyID)]);
        public static int DeliveryTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(DeliveryTypeID)]);
        public static int CollectionDeliveryTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(CollectionDeliveryTypeID)]);
        public static TimeSpan NextDayDeliveryCutOff
        {
            get
            {
                string[] hourMinute = ConfigurationManager.AppSettings[nameof(NextDayDeliveryCutOff)].Split(':');
                int hour = Convert.ToInt32(hourMinute[0]);
                int minute = Convert.ToInt32(hourMinute[1]);
                TimeSpan cutOffDate = new TimeSpan(hour, minute, 0);
                return cutOffDate;
            }
        }
        public static int GladstoneLocationID => int.Parse(ConfigurationManager.AppSettings[nameof(GladstoneLocationID)]);
        public static string PaymentStatusID => ConfigurationManager.AppSettings[nameof(PaymentStatusID)];
        public static int AccountTypeID => int.Parse(ConfigurationManager.AppSettings[nameof(AccountTypeID)]);
        public static int MembershipLevelID => int.Parse(ConfigurationManager.AppSettings[nameof(MembershipLevelID)]);
        public static string DefaultCustomerEmail => ConfigurationManager.AppSettings[nameof(DefaultCustomerEmail)];
        public static string DefaultAddressStreet_address => ConfigurationManager.AppSettings[nameof(DefaultAddressStreet_address)];
        public static string DefaultAddressExtended_address => ConfigurationManager.AppSettings[nameof(DefaultAddressExtended_address)];
        public static string DefaultAddressLocality => ConfigurationManager.AppSettings[nameof(DefaultAddressLocality)];
        public static string DefaultAddressRegion => ConfigurationManager.AppSettings[nameof(DefaultAddressRegion)];
        public static string DefaultAddressPostalCode => ConfigurationManager.AppSettings[nameof(DefaultAddressPostalCode)];
        public static string DefaultAddressPhone => ConfigurationManager.AppSettings[nameof(DefaultAddressPhone)];
        public static bool IsProcessOrdersWithPaymentPending => bool.Parse(ConfigurationManager.AppSettings[nameof(IsProcessOrdersWithPaymentPending)]);

        private static string _ProductSyncLastRun = null;
        public static string ProductSyncLastRun
        {
            get
            {
                if (_ProductSyncLastRun == null)
                {
                    _ProductSyncLastRun = ConfigurationManager.AppSettings[nameof(ProductSyncLastRun)];
                }
                return _ProductSyncLastRun;
            }
            set
            {
                _ProductSyncLastRun = value;
            }
        }

        private static string _OrderSyncLastRun = null;
        public static string OrderSyncLastRun
        {
            get
            {
                if (_OrderSyncLastRun == null)
                {
                    _OrderSyncLastRun = ConfigurationManager.AppSettings[nameof(OrderSyncLastRun)];
                }
                return _OrderSyncLastRun;
            }
            set
            {
                _OrderSyncLastRun = value;
            }
        }


        public static string ApiBaseURL => ConfigurationManager.AppSettings[nameof(ApiBaseURL)];
        public static string ApiURL_Listings => ConfigurationManager.AppSettings[nameof(ApiURL_Listings)];
        public static string ApiURL_Listings_RetrieveBySKU => ConfigurationManager.AppSettings[nameof(ApiURL_Listings_RetrieveBySKU)];
        public static string ApiURL_Orders_RetrieveByDate => ConfigurationManager.AppSettings[nameof(ApiURL_Orders_RetrieveByDate)];
        public static string ApiURL_Orders_SetAsCollected => ConfigurationManager.AppSettings[nameof(ApiURL_Orders_SetAsCollected)];
        public static string ApiURL_Orders_SetAsDispatched => ConfigurationManager.AppSettings[nameof(ApiURL_Orders_SetAsDispatched)];

        public const string ApiURL_SaleOrders = "salesOrders";
        public const string ApiURL_UpdateProductStockLevelsAndPrice = "products/partial";
        public const string Header_MediaType = "application/hal+json";
        public const string Header_AcceptLanguage = "en";
        public const string Header_AcceptVersion = "3.0";
        public const string Header_DisplayCurrency = "GBP";

        public static void Save()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            configuration.AppSettings.Settings[nameof(ProductSyncLastRun)].Value = ProductSyncLastRun;
            configuration.AppSettings.Settings[nameof(OrderSyncLastRun)].Value = OrderSyncLastRun;

            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
