using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class GoldrushLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 38;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.pickSpeed -= 0.1f;
		}
	}
}