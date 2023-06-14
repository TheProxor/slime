using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace TheProxor.Utils
{
	public static partial class Extensions
	{
		public static string SafeStringFormat(this string text, params object[] args)
		{
			if (text == null)
			{
				Debug.LogError("Text cannot be null!");
				return string.Empty;
			}

			List<int> argIndexes = FindArgIndexes();


			if (argIndexes.Count != args.Length)
			{
				Debug.LogError($"Args count not equal string params count! String: <b>'{text}'</b>");
				return text;
			}

			foreach (var argIndex in argIndexes)
			{
				if (argIndex < args.Length && argIndex >= 0)
				{
					text = text.Replace("{" + argIndex.ToString() + "}", args[argIndex].ToString());
				}
				else
				{
					Debug.LogError($"There is no index {argIndex} among the string params args!");
				}
			}

			return text;



			List<int> FindArgIndexes()
			{
				List<int> result = new List<int>(args.Length);

				foreach (var match in new Regex(@"{\d{1,}}").Matches(text))
				{
					string matchString = match.ToString();

					matchString = matchString.Remove(0, 1);
					matchString = matchString.Remove(matchString.Length - 1, 1);

					int index = int.Parse(matchString);

					result.Add(index);
				}

				return result;
			}
		}
	}
}
