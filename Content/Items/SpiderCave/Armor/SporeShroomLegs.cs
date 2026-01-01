using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SporeShroomLegs : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SporeShroomHead>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 10;
			Item.width = 26;
			Item.height = 12;
			Item.rare = ItemRarityID.LightRed;
		}

		public override void UpdateEquip(Player player) 
		{
        }
	}
}