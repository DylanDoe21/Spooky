using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class KrakenHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			SpookyPlayer.AddGlowMask(Item.type, "Spooky/Content/Items/Costume/KrakenHead_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
			glowMaskColor = Color.White;
		}
	}

	public class KrakenVanityHeadLayer : HelmetGlowmaskVanityLayer
	{
		protected override int ID => 10;
		protected override EquipType Type => EquipType.Head;
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
	}
}