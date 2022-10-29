using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class FlyScroll : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Scroll of the Flies");
			Tooltip.SetDefault("Summons flies that linger above you or any nearby target"
			+ "\nAfter a few seconds, they will fling themselves towards your cursor"
			+ "\nOnly up to 10 flies can exist at once");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.damage = 12;
			Item.mana = 12;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 32;
            Item.height = 42;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item77;
            Item.shoot = ModContent.ProjectileType<FlyMinion>();
            Item.shootSpeed = 10f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 10;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 5)
			.AddIngredient(ItemID.Silk, 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}