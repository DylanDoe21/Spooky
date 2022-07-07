using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Spooky.Content.Backgrounds
{
	public static class BackgroundExtensions
	{
		public static bool IsInWorld(this Point point)
		{
			return point.X >= 0 && point.Y >= 0 && point.X < Main.maxTilesX && point.Y < Main.maxTilesY;
		}
		
		public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			if (flags == null)
			{
				flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			}
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return field.GetValue(obj);
		}

		public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			if (flags == null)
			{
				flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			}
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return (T)field.GetValue(obj);
		}
	}
}