namespace ContractAuditBlockchain.Core
{
    public class LogicConstants
    {
        public enum RentContractStatus
        {
            CREATED,
            SIGNED,
            EXPIRED
        }

        public class Roles
        {
            public const string Administrator = "Administrator";
            public const string Client = "Client";
        }

        public enum ErrorMessages
        {
            LinkExpired = 1,
            LockedOut = 2

        }
    }
}
