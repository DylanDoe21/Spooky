using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyBody")]
	[AutoloadEquip(EquipType.Body)]
	public class GourdBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Rotten Gourd Breastplate");
			// Tooltip.SetDefault("3% increased melee critical strike chance");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 28;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Melee) += 0.03f;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 30)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}