using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class HazmatBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 15;
			Item.width = 30;
			Item.height = 20;
			Item.rare = ItemRarityID.Pink;
		}

		public override void EquipFrameEffects(Player player, EquipType type)
        {
			player.GetModPlayer<SpookyPlayer>().DrawHazmatBack = true;
		}

        public override void UpdateEquip(Player player) 
		{
		}

        /*
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 30)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
        */
	}
}