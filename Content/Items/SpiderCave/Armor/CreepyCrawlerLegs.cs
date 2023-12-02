using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class CreepyCrawlerLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
            player.moveSpeed += 0.15f;
            player.runAcceleration += 0.05f;
            player.jumpBoost = true;
        }
	}
}