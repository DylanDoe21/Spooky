using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Ocean;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class ZombieEelTail : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.DamageType = DamageClass.Summon;
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

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

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

			if (player.channel && player.active && !player.dead && !player.noItems && !player.CCed && ItemGlobal.ActiveItem(player).type == ModContent.ItemType<EelSushi>())
            {
                Projectile.timeLeft = 2;
            }

            //head parent
            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.ProjectileType<ZombieEelHead>())
            {
                Projectile.Kill();
            }

            //segment parent
			Projectile SegmentParent = Main.projectile[(int)Projectile.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center - Projectile.Center;

			if (SegmentParent.rotation != Projectile.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - Projectile.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.35f);
			}

			Projectile.rotation = SegmentCenter.ToRotation() + 1.57f;

            Projectile.spriteDirection = Parent.spriteDirection;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				Projectile.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 35f;
			}
        }
    }
}
