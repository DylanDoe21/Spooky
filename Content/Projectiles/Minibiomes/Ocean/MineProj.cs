using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Minibiomes.Ocean;
using Spooky.Content.NPCs.Minibiomes.Ocean;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class MineProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Minibiomes/Ocean/Mine";

		private Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
			Projectile.hide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

		public override bool? CanHitNPC(NPC target)
		{
			return target.type != ModContent.NPCType<Dunkleosteus>();
		}

		public override void AI()
		{
			Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

			Projectile.velocity *= 0.95f;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ModContent.ItemType<Mine>());

				if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
				{
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
				}
			}
		}
    }
}