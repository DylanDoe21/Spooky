using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class GoreBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Gore Monger Chestmail");
			Tooltip.SetDefault("8% increased damage and critical strike chance"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 15;
			Item.width = 30;
			Item.height = 20;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.08f;
			player.GetCritChance(DamageClass.Generic) += 8;
			player.aggro += 100;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<OrroboroChunk>(), 18)
			.AddIngredient(ModContent.ItemType<EyeBlockItem>(), 65)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
