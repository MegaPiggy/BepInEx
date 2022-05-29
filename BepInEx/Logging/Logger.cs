using System;
using System.Collections.Generic;
using System.Reflection;

namespace BepInEx.Logging
{
	/// <summary>
	/// A static Logger instance.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// Collection of all log listeners that receive log events.
		/// </summary>
		public static ICollection<ILogListener> Listeners { get; } = new List<ILogListener>();

		/// <summary>
		/// Collection of all log source that output log events.
		/// </summary>
		public static ICollection<ILogSource> Sources { get; } = new LogSourceCollection();


		private static readonly StaticLogSource InternalLogSource = new StaticLogSource("BepInEx");

		private static bool internalLogsInitialized;

		internal static void InitializeInternalLoggers()
		{
			if (internalLogsInitialized)
				return;
			
			Sources.Add(new HarmonyLogSource());

			internalLogsInitialized = true;
		}
		
		internal static void InternalLogEvent(object sender, LogEventArgs eventArgs)
		{
			foreach (var listener in Listeners)
			{
				listener?.LogEvent(sender, eventArgs);
			}
		}

		internal static void Log(LogLevel level, object data, Assembly calling)
		{
			InternalLogSource.Log(level, data, calling.GetName().Name);
		}

		/// <summary>
		/// Logs an entry to the current logger instance.
		/// </summary>
		/// <param name="level">The level of the entry.</param>
		/// <param name="data">The textual value of the entry.</param>
		public static void Log(LogLevel level, object data) => Log(level, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Fatal"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogFatal(object data) => Log(LogLevel.Fatal, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Error"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogError(object data) => Log(LogLevel.Error, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Warning"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogWarning(object data) => Log(LogLevel.Warning, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Message"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogMessage(object data) => Log(LogLevel.Message, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Info"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogInfo(object data) => Log(LogLevel.Info, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Logs a message with <see cref="LogLevel.Debug"/> level.
		/// </summary>
		/// <param name="data">Data to log.</param>
		public static void LogDebug(object data) => Log(LogLevel.Debug, data, Assembly.GetCallingAssembly());

		/// <summary>
		/// Creates a new log source with a name and attaches it to log sources.
		/// </summary>
		/// <param name="sourceName">Name of the log source to create.</param>
		/// <returns>An instance of <see cref="ManualLogSource"/> that allows to write logs.</returns>
		public static ManualLogSource CreateLogSource(string sourceName)
		{
			var source = new ManualLogSource(sourceName);

			Sources.Add(source);

			return source;
		}


		private class LogSourceCollection : List<ILogSource>, ICollection<ILogSource>
		{
			void ICollection<ILogSource>.Add(ILogSource item)
			{
				if (item == null)
					throw new ArgumentNullException(nameof(item), "Log sources cannot be null when added to the source list.");

				item.LogEvent += InternalLogEvent;

				base.Add(item);
			}

			void ICollection<ILogSource>.Clear()
			{
				foreach (var item in base.ToArray())
				{
					((ICollection<ILogSource>)this).Remove(item);
				}
			}

			bool ICollection<ILogSource>.Remove(ILogSource item)
			{
				if (item == null)
					return false;

				if (!base.Contains(item))
					return false;

				item.LogEvent -= InternalLogEvent;

				base.Remove(item);

				return true;
			}
		}

		private class StaticLogSource : ILogSource
		{
			/// <inheritdoc />
			public string SourceName => !string.IsNullOrEmpty(currentSourceName) ? currentSourceName : originalSourceName;

			private string originalSourceName = string.Empty;
			private string currentSourceName = string.Empty;

			/// <inheritdoc />
			public event EventHandler<LogEventArgs> LogEvent;

			/// <summary>
			/// Creates a manual log source.
			/// </summary>
			/// <param name="sourceName">Name of the log source.</param>
			public StaticLogSource(string sourceName)
			{
				originalSourceName = sourceName;
				Sources.Add(this);
			}

			internal void Log(LogLevel level, object data, string source)
			{
				currentSourceName = source;
				LogEvent?.Invoke(this, new LogEventArgs(data, level, this));
			}

			/// <inheritdoc />
			public void Dispose() { }
		}
	}
}