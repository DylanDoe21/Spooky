using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using System.Collections.Generic;
using Terraria.ID;

namespace Spooky.Content.Projectiles.Blooms
{
    public class OrangeOrbiterTrail : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Blooms/SummerOrange";

        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.hide = true;
			Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
        }

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
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

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			Player player = Main.player[Projectile.owner];

			float MovementScale = SpookyPlayer.PlayerSpeed(player);

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

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SummerOrange)
            {
                Projectile.Kill();
            }

            Projectile.alpha += 12;
        }
    }
}