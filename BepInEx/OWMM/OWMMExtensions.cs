using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.OWMM
{
	internal static class Extensions
	{
		private static string ToLiteral(this string input)
		{
			if (string.IsNullOrEmpty(input)) return "\"\"";
			StringBuilder literal = new StringBuilder(input.Length + 2);
			literal.Append("\"");
			foreach (var c in input)
			{
				switch (c)
				{
					case '\"': literal.Append("\\\""); break;
					case '\\': literal.Append(@"\\"); break;
					case '\0': literal.Append(@"\0"); break;
					case '\a': literal.Append(@"\a"); break;
					case '\b': literal.Append(@"\b"); break;
					case '\f': literal.Append(@"\f"); break;
					case '\n': literal.Append(@"\n"); break;
					case '\r': literal.Append(@"\r"); break;
					case '\t': literal.Append(@"\t"); break;
					case '\v': literal.Append(@"\v"); break;
					default:
						// ASCII printable character
						if (c >= 0x20 && c <= 0x7e)
						{
							literal.Append(c);
							// As UTF16 escaped character
						}
						else
						{
							literal.Append(@"\u");
							literal.Append(((int)c).ToString("x4"));
						}
						break;
				}
			}
			literal.Append("\"");
			return literal.ToString();
		}

		public static string ToJSON(this ModSocketMessage socketMessage)
		{
			string senderName = socketMessage.SenderName.ToLiteral();
			string senderType = socketMessage.SenderType.ToLiteral();
			int type = (int)socketMessage.Type;
			string message = socketMessage.Message.ToLiteral();
			return $"{{\"senderName\":{senderName},\"senderType\":{senderType},\"type\":{type},\"message\":{message}}}";
		}

		public static MessageType BepInExToOWMM(this Logging.LogLevel level)
		{
			switch (level)
			{
				case Logging.LogLevel.Error:
					return MessageType.Error;
				case Logging.LogLevel.Warning:
					return MessageType.Warning;
				case Logging.LogLevel.Info:
					return MessageType.Info;
				case Logging.LogLevel.Debug:
					return MessageType.Debug;
				case Logging.LogLevel.Fatal:
					return MessageType.Fatal;
				case Logging.LogLevel.Message:
				default:
					return MessageType.Message;
			}
		}
	}
}
