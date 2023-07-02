using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientKatana : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientKatanaSwingSlash>();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(5))
			{
                int[] Types = new int[] { DustID.RedTorch, DustID.BlueTorch, DustID.PurpleTorch };

				int dustEffect = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.Next(Types));
				Main.dust[dustEffect].noGravity = true;
				Main.dust[dustEffect].fadeIn = 1.12f;
				Main.dust[dustEffect].velocity *= 0.25f;
			}
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SentientKatanaSwingSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			
            return false;
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            float divide = 1.5f;

            Projectile.NewProjectile(player.GetSource_OnHit(target), Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, 
            ModContent.ProjectileType<SentientKatanaSlashSpawner>(), Item.damage / (int)divide, Item.knockBack, player.whoAmI, 0f, 0f);
        }
    }
}