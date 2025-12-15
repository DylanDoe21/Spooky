using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class YuletideLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 14;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed(DamageClass.Magic) += 0.1f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ChristmasWoodItem>(), 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}