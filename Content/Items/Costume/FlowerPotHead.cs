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
	public class FlowerPotHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flower Pot Head");
			// Tooltip.SetDefault("'That's not how you're supposed to use it'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 24;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}