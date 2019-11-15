// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using Microsoft.Win32;

namespace CausalityDbg.Main
{
	static class SettingsStorage
	{
		public static Settings Instance
		{
			get => _settings ?? (_settings = Load());
			set
			{
				if (value != _settings)
				{
					Save(value);
					_settings = value;
				}
			}
		}

		public static SettingsLaunch Launch
		{
			get => _launch ?? (_launch = LoadLaunch());
			set
			{
				if (value != _launch)
				{
					SaveLaunch(value);
					_launch = value;
				}
			}
		}

		static Settings _settings;
		static SettingsLaunch _launch;

		static void Save(Settings settings)
		{
			using var settingRoot = Registry.CurrentUser.CreateSubKey(KeyBase, RegistryKeyPermissionCheck.ReadWriteSubTree);
			DeleteAllExistingTools(settingRoot);

			settingRoot.SetValue("ToolCount", settings.Tools.Length, RegistryValueKind.DWord);

			for (var i = 0; i < settings.Tools.Length; i++)
			{
				using var toolRoot = settingRoot.CreateSubKey("Tool" + i, RegistryKeyPermissionCheck.ReadWriteSubTree);
				WriteTool(toolRoot, settings.Tools[i]);
			}
		}

		static Settings Load()
		{
			using (var settingRoot = Registry.CurrentUser.OpenSubKey(KeyBase, RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (settingRoot != null)
				{
					return new Settings(LoadTools(settingRoot));
				}
			}

			return new Settings(ImmutableArray<SettingsExternalTool>.Empty);
		}

		static void SaveLaunch(SettingsLaunch model)
		{
			using var settingRoot = Registry.CurrentUser.CreateSubKey(KeyBase, RegistryKeyPermissionCheck.ReadWriteSubTree);
			using var launchRoot = settingRoot.CreateSubKey("Launch", RegistryKeyPermissionCheck.ReadWriteSubTree);
			launchRoot.SetValue("Process", model.Process ?? string.Empty, RegistryValueKind.String);
			launchRoot.SetValue("Directory", model.Directory ?? string.Empty, RegistryValueKind.String);
			launchRoot.SetValue("Arguments", model.Arguments ?? string.Empty, RegistryValueKind.String);
			launchRoot.SetValue("RTVersion", model.RuntimeVersion ?? string.Empty, RegistryValueKind.String);
			launchRoot.SetValue("ZapDisable", (int)model.Mode, RegistryValueKind.DWord);
		}

		static SettingsLaunch LoadLaunch()
		{
			using (var settingRoot = Registry.CurrentUser.OpenSubKey(KeyBase, RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (settingRoot != null)
				{
					using var launchRoot = settingRoot.OpenSubKey("Launch", RegistryKeyPermissionCheck.ReadSubTree);

					if (launchRoot != null)
					{
						var process = (string)launchRoot.GetValue("Process", string.Empty);
						var directory = (string)launchRoot.GetValue("Directory", string.Empty);
						var arguments = (string)launchRoot.GetValue("Arguments", string.Empty);
						var version = (string)launchRoot.GetValue("RTVersion", string.Empty);
						var mode = (NGenMode)launchRoot.GetValue("ZapDisable", (int)NGenMode.Standard);
						return new SettingsLaunch(process, directory, arguments, version, mode);
					}
				}
			}

			return new SettingsLaunch(string.Empty, string.Empty, string.Empty, null, NGenMode.Standard);
		}

		static ImmutableArray<SettingsExternalTool> LoadTools(RegistryKey settingRoot)
		{
			var count = (int)settingRoot.GetValue("ToolCount", 0);

			if (count <= 0)
			{
				return ImmutableArray<SettingsExternalTool>.Empty;
			}

			var builder = ImmutableArray.CreateBuilder<SettingsExternalTool>(count);
			builder.Count = count;

			for (var i = 0; i < count; i++)
			{
				using var toolRoot = settingRoot.OpenSubKey("Tool" + i, RegistryKeyPermissionCheck.ReadSubTree);

				if (toolRoot != null)
				{
					builder[i] = ReadTool(toolRoot);
				}
			}

			return builder.ToImmutable();
		}

		static void DeleteAllExistingTools(RegistryKey settingsRoot)
		{
			var names = settingsRoot.GetSubKeyNames();

			for (var i = 0; i < names.Length; i++)
			{
				var name = names[i];
				if (!name.StartsWith("Tool", StringComparison.Ordinal)) continue;
				if (name == "ToolCount") continue;

				settingsRoot.DeleteSubKeyTree(name);
			}
		}

		static void WriteTool(RegistryKey toolRoot, SettingsExternalTool tool)
		{
			toolRoot.SetValue("Name", tool.Name ?? string.Empty, RegistryValueKind.String);
			toolRoot.SetValue("Process", tool.Process ?? string.Empty, RegistryValueKind.String);
			toolRoot.SetValue("Arguments", tool.Arguments ?? string.Empty, RegistryValueKind.String);
		}

		static SettingsExternalTool ReadTool(RegistryKey toolRoot)
		{
			return new SettingsExternalTool(
				(string)toolRoot.GetValue("Name", string.Empty),
				(string)toolRoot.GetValue("Process", string.Empty),
				(string)toolRoot.GetValue("Arguments", string.Empty));
		}

		const string KeyBase = @"Software\CausalityDbg";
	}
}
