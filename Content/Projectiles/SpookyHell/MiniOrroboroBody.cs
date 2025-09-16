using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class MiniBoroBody : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody";

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorMouth)
			{
				Projectile.timeLeft = 2;
			}

            //head parent
            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.ProjectileType<MiniBoroHead>())
            {
                Projectile.Kill();
            }

            //segment parent
			Projectile SegmentParent = Main.projectile[(int)Projectile.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - Projectile.Center;

			if (SegmentParent.rotation != Projectile.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - Projectile.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			Projectile.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				Projectile.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 18f;
			}
        }
    }

    public class MiniOrroBody : MiniBoroBody
    {
        public override string Texture => "Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody";

        private static Asset<Texture2D> ProjTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorEye)
			{
				Projectile.timeLeft = 2;
			}

            //head parent
            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.ProjectileType<MiniOrroHead>())
            {
                Projectile.Kill();
            }

            //segment parent
			Projectile SegmentParent = Main.projectile[(int)Projectile.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - Projectile.Center;

			if (SegmentParent.rotation != Projectile.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - Projectile.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			Projectile.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				Projectile.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 18f;
			}
        }
    }
}
