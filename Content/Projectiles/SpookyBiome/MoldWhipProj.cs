using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class MoldWhipProj : ModProjectile
	{
		private float ChargeTime 
		{
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults() 
		{
			// This makes the projectile use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			// This method quickly sets the whip's properties.
			Projectile.DefaultToWhip();

			// use these to change from the vanilla defaults
			Projectile.WhipSettings.Segments = 25;
			Projectile.WhipSettings.RangeMultiplier = 1f;
		}

		// This example uses PreAI to implement a charging mechanic.
		// If you remove this, also remove Item.channel = true from the item's SetDefaults.
		public override bool PreAI() 
		{
			Player owner = Main.player[Projectile.owner];

			// Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
			if (!owner.channel || ChargeTime >= 120) 
			{
				return true; // Let the vanilla whip AI run.
			}

			if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
			{
				Projectile.WhipSettings.Segments++;
			}

			// Increase range up to 2x for full charge.
			Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

			// Reset the animation and item timer while charging.
			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false; // Prevent the vanilla whip AI from running.
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

			if (target.life <= 0)
            {
				if (target.HasBuff(ModContent.BuffType<MoldWhipDebuff>()))
				{
					Vector2 Speed = new Vector2(3f, 0f).RotatedByRandom(2 * Math.PI);

					for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
					{
						Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(target.GetSource_FromThis(), target.Center, speed,
							ModContent.ProjectileType<MoldWhipFly>(), 15, 0f, Main.myPlayer, 0, 0);
						}
					}
				}
			}
		}

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			target.AddBuff(ModContent.BuffType<MoldWhipDebuff>(), 180);
        }

        public override bool PreDraw(ref Color lightColor) 
        {
			List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				//14 is the width of the whole whip, 26 is the height for the tips hotbox
				Rectangle frame = new(0, 0, 14, 26);
				Vector2 origin = new(5, 8);
				float scale = 1;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2) 
                {
					frame.Y = 58;
					frame.Height = 18;
					scale = 1.2f;
				}
				else if (i > 10) 
                {
					frame.Y = 46;
					frame.Height = 12;
				}
				else if (i > 5) 
                {
					frame.Y = 34;
					frame.Height = 12;
				}
				else if (i > 0) 
                {
					frame.Y = 22;
					frame.Height = 22;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}