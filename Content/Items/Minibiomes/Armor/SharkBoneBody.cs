using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SharkBoneBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 38;
			Item.height = 24;
			Item.rare = ItemRarityID.Blue;
		}

        public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 8;
			player.fishingSkill += 5;
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