using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class OrangeOrbiter : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Blooms/SummerOrange";

        float distance = 70f;

        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, 1.12f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			Player player = Main.player[Projectile.owner];

			float MovementScale = SpookyPlayer.PlayerSpeedToMPH(player);

			modifiers.FinalDamage *= MovementScale / 12;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire3, 600);
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.05f * Projectile.direction;

            if (player.dead || !player.active || !player.GetModPlayer<BloomBuffsPlayer>().SummerOrange)
            {
                Projectile.Kill();
            }

            Projectile.timeLeft = 2;

            float MovementScale = SpookyPlayer.PlayerSpeedToMPH(player);

			Projectile.rotation += 0.05f * MovementScale / 15;

			Projectile.ai[0] += MovementScale == 0 ? 0.5f : (MovementScale / 2.5f) * 0.5f;
            double rad = Projectile.ai[0] * (Math.PI / 180);

            float distanceWithScaling = 70f + MovementScale * 5;
            if (distance > distanceWithScaling)
            {
                distance -= 3;
            }
            if (distance < distanceWithScaling)
            {
                distance += 3;
            }

            Projectile.position.X = player.Center.X - ((float)Math.Cos(rad) * distance) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - ((float)Math.Sin(rad) * distance) - Projectile.height / 2;

            Projectile.ai[1]++;

            if (Projectile.ai[1] % 2 == 0 && MovementScale != 0)
            {
                int newProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OrangeOrbiterTrail>(), Projectile.damage, 0, player.whoAmI);
				Main.projectile[newProj].rotation = Projectile.rotation;
			}
        }
    }
}