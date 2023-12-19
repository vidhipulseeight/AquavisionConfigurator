using System.Text.RegularExpressions;

namespace AquavisionConfigurator.App_Start {
	public class CommonHelper {
		public static bool IsValidEmail(string email) {
			if (string.IsNullOrWhiteSpace(email)) {
				return false;
			}
			email = email.Trim();
			return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);			
		}
		public static bool IsValidPhone(string phone) {
			if (string.IsNullOrWhiteSpace(phone)) {
				return false;
			}
			phone = phone.Trim();
			return Regex.IsMatch(phone, @"^[0-9]{10,12}$");
		}
		public static string EnsureMaximumLength(string str, int maxLength) {
			if (string.IsNullOrEmpty(str))
				return str;

			return str.Length > maxLength ? str.Substring(0, maxLength) : str;
		}
	}
}