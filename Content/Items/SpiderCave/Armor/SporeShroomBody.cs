using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SporeShroomBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 16;
			Item.width = 34;
			Item.height = 20;
			Item.rare = ItemRarityID.LightRed;
		}

		public override void UpdateEquip(Player player) 
		{
        }
	}
}