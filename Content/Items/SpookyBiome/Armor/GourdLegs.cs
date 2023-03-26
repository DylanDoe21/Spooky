using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyLegs")]
	[AutoloadEquip(EquipType.Legs)]
	public class GourdLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Rotten Gourd Leggings");
			Tooltip.SetDefault("5% increased melee speed");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 25)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}