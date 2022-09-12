using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Projectiles;
 
namespace Spooky.Content.Items.Catacomb
{
	public class HarvesterScythe : SwingWeaponBase
	{
		public override int Length => 30;
		public override int TopSize => 35;
		public override float SwingDownSpeed => 16f;
		public override bool CollideWithTiles => false;
		static bool hasHitSomething = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harvester's Scythe");
			Tooltip.SetDefault("Killing enemies with the scythe will release souls around you"
			+ "\nThe more souls you have, the faster and stronger this weapon gets" 
			+ "\nAfter you have 10 souls, they will launch themselves everywhere and home in on enemies");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 35; 
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
			Item.width = 64;           
			Item.height = 60;         
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 160, 
			player.velocity.X / 2, player.velocity.Y / 2, 0, Color.Transparent, 1.5f)].noGravity = true;

			Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 160, 
			player.velocity.X / 2, player.velocity.Y / 2, 0, Color.Transparent, 1.5f)].noGravity = true;
		}

		public override void UseAnimation(Player player)
		{
			hasHitSomething = false;

			base.UseAnimation(player);
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
			if (!hasHitSomething)
			{
				hasHitSomething = true;

				int HitProj = Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<SwingWeaponHit>(), Item.damage, 0f, Main.myPlayer, 0, 0);
				Main.projectile[HitProj].width = 60;
				Main.projectile[HitProj].height = 60;
			}
		}
	}
}
