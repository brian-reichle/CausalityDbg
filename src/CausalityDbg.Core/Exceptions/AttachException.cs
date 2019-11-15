// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CausalityDbg.Core
{
	[Serializable]
	public sealed class AttachException : Exception
	{
		const string ErrorTypeName = "ErrorType";

		public AttachException(AttachErrorType type)
			: this(type, DefaultMessage(type), null)
		{
		}

		public AttachException(AttachErrorType type, string message)
			: this(type, message, null)
		{
		}

		public AttachException(AttachErrorType type, Exception innerException)
			: this(type, DefaultMessage(type), innerException)
		{
		}

		public AttachException(AttachErrorType type, string message, Exception innerException)
			: base(message, innerException)
		{
			_errorType = type;
		}

		AttachException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_errorType = (AttachErrorType)info.GetInt32(ErrorTypeName);
		}

		public AttachErrorType ErrorType
		{
			[DebuggerStepThrough]
			get { return _errorType; }
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ErrorTypeName, (int)_errorType);
		}

		static string DefaultMessage(AttachErrorType errorType)
		{
			return errorType switch
			{
				AttachErrorType.AlreadyAttached => "Another debugger is already attached to the specified process.",
				AttachErrorType.AttachToSelf => "Cannot attch to self.",
				AttachErrorType.FrameworkNotLoaded => "The target process has not loaded the CLR.",
				AttachErrorType.ProcessNotFound => "Process not found.",
				AttachErrorType.UnsupportedCLRVersion => "The target process has loaded an unsupported version of the CLR.",
				AttachErrorType.MissingCLRVersion => "The selected version of the CLR isn't installed.",
				AttachErrorType.FileNotFound => "The target process executable could not be found.",
				AttachErrorType.DirectoryNotFound => "The target directory could not be found.",
				AttachErrorType.IncompatiblePlatforms => "Cannot debug a process running on an incompatible platform.",
				_ => string.Empty,
			};
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly AttachErrorType _errorType;
	}
}
