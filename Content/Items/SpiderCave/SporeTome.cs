using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SporeTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item77;
            Item.shoot = ModContent.ProjectileType<SporeTomeMushroom>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));

			Projectile.NewProjectile(source, player.Center, newVelocity, ModContent.ProjectileType<SporeTomeMushroom>(), damage, knockback, player.whoAmI, ai0: Main.rand.Next(0, 4));

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}