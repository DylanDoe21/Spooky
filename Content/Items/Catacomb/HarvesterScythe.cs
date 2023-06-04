using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class HarvesterScythe : SwingWeaponBase
	{
		public override int Length => 45;
		public override int TopSize => 20;
		public override float SwingDownSpeed => 13.5f;
		public override bool CollideWithTiles => false;
		static bool hasHitSomething = false;

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.crit = 10;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
			Item.width = 64;          
			Item.height = 60;         
			Item.useTime = 48;
			Item.useAnimation = 48;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 7;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.scale = 1.2f;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.MagnetSphere, player.velocity.X / 2, player.velocity.Y / 2, 0, default, 1.5f);
			Main.dust[dust].noGravity = true;
		}

		public override void UseAnimation(Player player)
		{
			hasHitSomething = false;

			base.UseAnimation(player);
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (!hasHitSomething)
			{
				hasHitSomething = true;

				Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<ScytheHitProj>(), Item.damage, Item.knockBack, Main.myPlayer, 0, 0);
			}

            if (target.life <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<SoulBolt>()] < 5)
            {
				Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
				ModContent.ProjectileType<SoulBolt>(), Item.damage, 0f, Main.myPlayer, 0, 0);
			}
		}
	}
}
