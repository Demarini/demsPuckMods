using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

// Token: 0x020001DC RID: 476
public static class ConfigUtils
{
	// Token: 0x06000DFC RID: 3580 RVA: 0x00042188 File Offset: 0x00040388
	public static T LoadConfigFromFile<T>(string filePath, bool createIfNotExists = true) where T : class, new()
	{
		T result;
		if (string.IsNullOrEmpty(filePath))
		{
			result = default(!!0);
			return result;
		}
		if (!File.Exists(filePath) && createIfNotExists)
		{
			ConfigUtils.SaveConfigToFile<T>(filePath, Activator.CreateInstance<T>());
		}
		try
		{
			result = ConfigUtils.LoadConfigFromSerializedString<T>(File.ReadAllText(filePath));
		}
		catch (Exception ex)
		{
			Debug.LogError("[ConfigUtils] Error loading config from " + filePath + ": " + ex.Message);
			result = default(!!0);
		}
		return result;
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x00042208 File Offset: 0x00040408
	public static T LoadConfigFromSerializedString<T>(string serializedString) where T : class, new()
	{
		T result;
		if (string.IsNullOrEmpty(serializedString))
		{
			result = default(!!0);
			return result;
		}
		try
		{
			result = JsonSerializer.Deserialize<T>(serializedString, null);
		}
		catch (Exception ex)
		{
			Debug.LogError("[ConfigUtils] Error loading config from serialized string: " + ex.Message);
			result = default(!!0);
		}
		return result;
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00042268 File Offset: 0x00040468
	public static void SaveConfigToFile<T>(string filePath, T config)
	{
		try
		{
			JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			string text = JsonSerializer.Serialize<T>(config, jsonSerializerOptions);
			Debug.Log("[ConfigUtils] Serialized config " + typeof(!!0).Name + ": " + text);
			File.WriteAllText(filePath, text);
		}
		catch (Exception ex)
		{
			Debug.LogError("[ConfigUtils] Error saving config to " + filePath + ": " + ex.Message);
		}
	}
}
