﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityTK.Serialization
{
	public enum SerializerErrorSeverity
	{
		INFO,
		WARNING,
		ERROR
	}

	public struct SerializerError
	{
		public SerializerErrorSeverity severity;

		/// <summary>
		/// The file in which this error has been detected.
		/// </summary>
		public string file;

		/// <summary>
		/// The line number of the error in <see cref="file"/>
		/// </summary>
		public int lineNumber;

		public string message;

		public readonly string stackTrace;

		public SerializerError(SerializerErrorSeverity severity, string file, int lineNumber, string message)
		{
			this.severity = severity;
			this.file = file;
			this.lineNumber = lineNumber;
			this.message = message;
			this.stackTrace = new System.Diagnostics.StackTrace().ToString();
		}

		/// <summary>
		/// Creates an error message from this error to be used when logging it to some console or logfile.
		/// </summary>
		public string GetFullMessage()
		{
			return string.Format("Parsing error in file '{0}' on line {1}:\nSeverity:{2}\n{3}\n\nStack Trace:\n{4}", this.file, this.lineNumber.ToString(), this.severity.ToString(), this.message, this.stackTrace);
		}

		/// <summary>
		/// Logs <see cref="GetFullMessage"/> to the unity console using the Debug.Log methods.
		/// </summary>
		public void DoUnityDebugLog()
		{
			string msg = GetFullMessage();
			switch (this.severity)
			{
				case SerializerErrorSeverity.INFO: Debug.Log(msg); break;
				case SerializerErrorSeverity.WARNING: Debug.LogWarning(msg); break;
				case SerializerErrorSeverity.ERROR: Debug.LogError(msg); break;
			}
		}
	}
}
