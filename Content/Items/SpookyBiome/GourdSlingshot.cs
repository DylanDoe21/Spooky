using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class GourdSlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

        public override void SetDefaults()
        {
            Item.damage = 15;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 34;
            Item.height = 40;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<GourdSlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<GourdSlingshotProj>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<GourdSlingshotProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 8)
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 15)
            .AddIngredient(ItemID.Cobweb, 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}