using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DylanDoeHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			DylanHeadGlowmask.AddGlowMask(Item.type, "Spooky/Content/Items/Costume/DylanDoeHead_Head_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
			glowMaskColor = Color.White;
		}
	}

	public class DylanHeadGlowmask : ModPlayer
	{
		internal static readonly Dictionary<int, Texture2D> ItemGlowMask = new Dictionary<int, Texture2D>();

		internal new static void Unload() => ItemGlowMask.Clear();
		public static void AddGlowMask(int itemType, string texturePath) => ItemGlowMask[itemType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	}

	public abstract class DylanGlowmaskVanityLayer : PlayerDrawLayer
	{
		protected abstract int ID { get; }
		protected abstract EquipType Type { get; }

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (!drawInfo.drawPlayer.armor[ID].IsAir)
			{
				if (drawInfo.drawPlayer.armor[ID].type >= ItemID.Count && !Main.dayTime &&
				DylanHeadGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[ID].type, out Texture2D textureLegs))//Vanity Legs
				{
					for (int i = 0; i < 2; i++)
					{
						DrawHeadGlowMask(Type, textureLegs, drawInfo);
					}
				}
			}
		}

		public static void DrawHeadGlowMask(EquipType type, Texture2D texture, PlayerDrawSet info)
		{
			float shakeX = Main.rand.Next(-1, 1);
			float shakeY = Main.rand.Next(-1, 1);

			Vector2 adjustedPosition = new Vector2((int)(info.Position.X - Main.screenPosition.X + shakeX) + 
			((info.drawPlayer.width - info.drawPlayer.bodyFrame.Width) / 2), (int)(info.Position.Y - Main.screenPosition.Y + shakeY) + 
			info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4);

			DrawData drawData = new DrawData(texture, adjustedPosition + info.drawPlayer.headPosition + info.rotationOrigin, info.drawPlayer.bodyFrame, info.headGlowColor, info.drawPlayer.headRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
			{
				//shader = info.headArmorShader //NEEDSUPDATE ALL of these
			};
			info.DrawDataCache.Add(drawData);
		}
	}

	public class DylanVanityHeadLayer : DylanGlowmaskVanityLayer
	{
		protected override int ID => 10;
		protected override EquipType Type => EquipType.Head;
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
	}
}