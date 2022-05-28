using System;

namespace BepInEx.OWMM
{
	using Logging;
	public class LogListener : ILogListener, IDisposable
	{
		public ModSocket socket = null;

		public LogListener() => Initialize();

		public void Initialize()
		{
			Logger.LogInfo("Initializing OWMM Log Listener");
			int consolePort = 0;
			string[] args = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-consolePort") int.TryParse(args[i + 1], out consolePort);
			}
			Logger.LogInfo("Console Port: " + consolePort);
			if (consolePort != 0) socket = new ModSocket(consolePort);
			Logger.LogInfo("Initialized OWMM Log Listener");
		}

		public bool lastError = false;

		public void LogEvent(object sender, LogEventArgs args)
		{
			if (lastError)
            {
				lastError = false;
				return;

			}
            try
			{
				socket?.WriteToSocket(new ModSocketMessage
				{
					SenderName = args.Source.SourceName,
					SenderType = "OWMM.LogListener",
					Type = args.Level.BepInExToOWMM(),
					Message = args.Data.ToString(),
				});
			}
            catch (Exception ex)
            {
				lastError = true;
				Logger.LogError(ex);
            }
		}

		public void Dispose()
		{
			socket?.Close();
		}
	}
}