using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesApp
{
    internal static class CurrentUser
    {
        public static int Id { get; set; }
        public static string LastName { get; set; } = "";
        public static string FirstName { get; set; } = "";
        public static string MiddleName { get; set; } = "";
        public static string Role { get; set; } = "Гость";
        public static string FullName =>
            string.IsNullOrEmpty(MiddleName)
                ? $"{LastName} {FirstName}"
                : $"{LastName} {FirstName} {MiddleName}";

        public static bool IsAdministrator => Role == "Администратор";
        public static bool IsManager => Role == "Менеджер";
        public static bool IsClient => Role == "Авторизированный клиент";
        public static bool IsGuest => Role == "Гость";
    }
}
