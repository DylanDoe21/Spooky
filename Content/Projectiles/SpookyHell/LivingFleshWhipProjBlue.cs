using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Buffs.WhipDebuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class LivingFleshWhipProjBlue : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 35;
			Projectile.WhipSettings.RangeMultiplier = 1.75f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			if (Main.rand.NextBool(15))
			{
				if (!target.HasBuff(ModContent.BuffType<FleshWhipCooldown>()))
				{
					SoundEngine.PlaySound(SoundID.Item131, target.Center);

					for (int numDusts = 0; numDusts < 25; numDusts++)
					{
						int dust = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 100, default, 2f * Main.rand.NextFloat(1f, 2f));
						Main.dust[dust].velocity *= 5f;
						Main.dust[dust].noGravity = true;
							
						if (Main.rand.NextBool(2))
						{
							Main.dust[dust].scale = 0.5f;
							Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
						}
					}

					target.AddBuff(ModContent.BuffType<FleshWhipDefense2>(), 240);
					target.AddBuff(ModContent.BuffType<FleshWhipCooldown>(), 1200);
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 20, 22);
                Vector2 origin = new Vector2(10, 10);
                float scale = 1;

                //tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 82;
					frame.Height = 24;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 64;
					frame.Height = 18;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 44;
					frame.Height = 18;
				}
				//the held part of the whip
				else if (i > 0) 
				{
					frame.Y = 0;
					frame.Height = 24;
				}

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }

            return false;
        }
	}
}