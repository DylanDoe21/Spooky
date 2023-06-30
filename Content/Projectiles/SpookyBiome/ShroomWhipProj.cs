using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class ShroomWhipProj : ModProjectile
	{
		private float Timer 
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 15;
			Projectile.WhipSettings.RangeMultiplier = 0.9f;
		}

		/*
		public override void AI() 
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;

			if (Timer >= swingTime || owner.itemAnimation <= 0) 
			{
				Projectile.Kill();
				return;
			}

			float swingProgress = Timer / swingTime;

			if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
			{
				List<Vector2> points = Projectile.WhipPointsForCollision;
				points.Clear();
				Projectile.FillWhipControlPoints(Projectile, points);
				int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
				Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));

				Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, 41, 0f, 0f, 100, Color.White);
				dust.position = points[pointIndex];
				dust.fadeIn = 0.3f;
				Vector2 spinningpoint = points[pointIndex] - points[pointIndex - 1];
				dust.noGravity = true;
				dust.velocity *= 0.5f;

				dust.velocity += spinningpoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
				dust.velocity *= 0.5f;
			}
		}
		*/

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Player owner = Main.player[Projectile.owner];

			owner.MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

            if (owner.ownedProjectileCounts[ModContent.ProjectileType<ShroomWhipSpore>()] < 5 && Main.rand.NextBool(5))
            {
                Projectile.NewProjectile(target.GetSource_Death(), target.Center, Vector2.Zero, 
				ModContent.ProjectileType<ShroomWhipSpore>(), Projectile.damage, 0f, Main.myPlayer, 0, 0);
            }
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
				//14 is the width of the whole whip, 16 is the height for the tips hotbox
				Rectangle frame = new(0, 0, 14, 18);
				Vector2 origin = new(5, 8);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 74;
					frame.Height = 18;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 58;
					frame.Height = 16;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 42;
					frame.Height = 16;
				}
				//the held part of the whip
				else if (i > 0) 
				{
					frame.Y = 26;
					frame.Height = 16;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;

				//draw the whip glow outline
				Color glowColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Blue);
				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, glowColor, rotation, origin, scale * 1.2f, SpriteEffects.None, 0);

				//draw the whip itself
				Color color = Lighting.GetColor(element.ToTileCoordinates());
				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}