// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Globalization;
using System.IO;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core
{
	static class NotifierExtensions
	{
		public static void NotifyMissingMethod(this ITrackerCallback callback, ICorDebugModule module, string methodName, string className)
		{
			callback.Notify(
				TrackerNotificationLevel.Error,
				module.GetName(),
				string.Format(
					CultureInfo.InvariantCulture,
					"Unable to find any methods called '{0}' on '{1}'",
					methodName,
					className));
		}

		public static void NotifyMissingClass(this ITrackerCallback callback, ICorDebugModule module, string className)
		{
			var modulePath = module.GetName();

			callback.Notify(
				TrackerNotificationLevel.Error,
				modulePath,
				string.Format(
					CultureInfo.InvariantCulture,
					"Unable to find class '{0}' in module '{1}'",
					className,
					Path.GetFileName(modulePath)));
		}

		public static void NotifyMissingNestedClass(this ITrackerCallback callback, ICorDebugModule module, string hostFullClassName, string nestedClassName)
		{
			callback.Notify(
				TrackerNotificationLevel.Error,
				module.GetName(),
				string.Format(
					CultureInfo.InvariantCulture,
					"Unable to find nested class '{0}' in '{1}'",
					nestedClassName,
					hostFullClassName));
		}

		public static void NotifyIsZapModule(this ITrackerCallback callback, ICorDebugModule module)
		{
			var modulePath = module.GetName();

			callback.Notify(
				TrackerNotificationLevel.Warning,
				modulePath,
				string.Format(
					CultureInfo.InvariantCulture,
					"Attempted to dissable optomisations for '{0}' but faild as it's using an NGEN'ed image. This may result in some information being unavailable.",
					Path.GetFileName(modulePath)));
		}

		public static void NotifyModuleIsAttaching(this ITrackerCallback callback, ICorDebugModule module)
		{
			var modulePath = module.GetName();

			callback.Notify(
				TrackerNotificationLevel.Warning,
				modulePath,
				string.Format(
					CultureInfo.InvariantCulture,
					"Attempted to dissable optomisations for '{0}' but failed as it was already loaded at the time of attach. This may result in some information being unavailable.",
					Path.GetFileName(modulePath)));
		}

		public static void NotifyNoThis(this ITrackerCallback callback, ICorDebugModule module, string classFullName, string methodName)
		{
			var modulePath = module.GetName();

			callback.Notify(
				TrackerNotificationLevel.Error,
				modulePath,
				string.Format(
					CultureInfo.InvariantCulture,
					"Cannot access 'this' on the static method '{0}.{1}'.",
					classFullName,
					methodName));
		}

		public static void NotifyNoParam(this ITrackerCallback callback, ICorDebugModule module, string classFullName, string methodName, string paramName)
		{
			var modulePath = module.GetName();

			callback.Notify(
				TrackerNotificationLevel.Error,
				modulePath,
				string.Format(
					CultureInfo.InvariantCulture,
					"Cannot find the param '{2}' on the method '{0}.{1}'.",
					classFullName,
					methodName,
					paramName));
		}

		public static void NotifyBreakpointFail(this ITrackerCallback callback, ICorDebugModule module, MetaDataToken token, int error)
		{
			callback.Notify(
				TrackerNotificationLevel.Error,
				module.GetName(),
				string.Format(
					CultureInfo.InvariantCulture,
					"Error attempting to set a breakpoint in function {0} (error: {1}).",
					token,
					error));
		}
	}
}
