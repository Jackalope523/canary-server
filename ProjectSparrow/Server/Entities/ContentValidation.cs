using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Entities
{
	internal static class ContentValidation
	{
		private static ProfanityFilter filter;
		private static List<string> DisallowedPhrases = new()
		{ "crack", "cocaine" };

		static ContentValidation()
		{
			filter = new ProfanityFilter(DisallowedPhrases);
		}

		public static bool IsEmailValid(string email)
		{
			if (!MailAddress.TryCreate(email, out _)) { return false; }

			return true;
		}

		public static string NormaliseText(string content)
		{
			// Check if text contains inappropriate phrases
			string finalText = filter.CensorText(content);

			return finalText;
		}

		public static bool TryNormalisePhoneNumber(string phoneNumber, out string normalisedPhoneNumber)
		{
			normalisedPhoneNumber = PhoneNumberUtil.ExtractPossibleNumber(phoneNumber);

			// Check if phone number is valid
			if (string.IsNullOrEmpty(normalisedPhoneNumber)) { return false; }
			if (!PhoneNumberUtil.IsViablePhoneNumber(normalisedPhoneNumber)) { return false; }

			// Normalise number
			normalisedPhoneNumber = PhoneNumberUtil.Normalize(normalisedPhoneNumber);
			return true;
		}
	}

	internal class ProfanityFilter
	{
		public IList<string> CensoredWords { get; private set; }


		public ProfanityFilter(IEnumerable<string> censoredWords)
		{
			CensoredWords = new List<string>(censoredWords);
		}

		public string CensorText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{ throw new ArgumentNullException(nameof(text)); }
				

			string censoredText = text;

			foreach (string censoredWord in CensoredWords)
			{
				string regularExpression = ToRegexPattern(censoredWord);

				censoredText = Regex.Replace(censoredText, regularExpression, StarCensoredMatch,
					RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			}

			return censoredText;
		}

		private static string StarCensoredMatch(Match m)
		{
			string word = m.Captures[0].Value;

			return new string('*', word.Length);
		}

		private string ToRegexPattern(string wildcardSearch)
		{
			string regexPattern = Regex.Escape(wildcardSearch);

			regexPattern = regexPattern.Replace(@"\*", ".*?");
			regexPattern = regexPattern.Replace(@"\?", ".");

			if (regexPattern.StartsWith(".*?"))
			{
				regexPattern = regexPattern.Substring(3);

				regexPattern = @"(^\b)*?" + regexPattern;
			}

			regexPattern = @"\b" + regexPattern + @"\b";

			return regexPattern;
		}
	}
}
