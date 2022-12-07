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
	public class LittleEyeHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Little Eye's Wizard Hat");
			Tooltip.SetDefault("'It is way too big for your head'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}