using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[LegacyName("SpookyBody")]
	[AutoloadEquip(EquipType.Body)]
	public class SpiritHorsemanBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spirit Horseman's Chestmail");
			Tooltip.SetDefault("3% increased damage");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 34;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 15)
			.AddIngredient(ItemID.Silk, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
		*/
	}
}