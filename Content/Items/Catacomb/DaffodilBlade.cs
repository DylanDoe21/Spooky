using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class DaffodilBlade : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 32;
            Item.height = 36;
            Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.LightRed; 
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<DaffodilBladeSlash>();
            Item.scale = 1.3f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Grass);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<DaffodilBladeSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			Main.projectile[Slash].scale *= (Item.scale - 0.22f) * (player.meleeScaleGlove ? 1.1f : 1f);

            return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 18)
			.AddIngredient(ItemID.Stinger, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}