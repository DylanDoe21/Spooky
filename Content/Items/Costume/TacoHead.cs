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
	public class TacoHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Taco's Mask");
			Tooltip.SetDefault("'Great for impersonating spooky mod devs!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
		}
	}
}