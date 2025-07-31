using System.Diagnostics;
using Verse;

namespace ResourceAlert
{
	public static class DebugLog
	{
		// this method will only be called when the DEBUG symbol is defined
		[Conditional("DEBUG")]
		public static void Message(string text)
		{
			Log.Message(text);
		}

		// overloads if you like formatting:
		[Conditional("DEBUG")]
		public static void Message(string format, params object[] args)
		{
			Log.Message(string.Format(format, args));
		}
	}
}
