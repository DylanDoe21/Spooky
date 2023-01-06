using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class EyeBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Eye Chestpiece");
			Tooltip.SetDefault("5% increased summon damage"
			+ "\nIncreases your max minions and sentries by 1");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 36;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.05f;
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 75)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
