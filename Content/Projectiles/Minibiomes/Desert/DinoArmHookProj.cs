using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class DinoArmHookProj : ModProjectile
	{
        private static Asset<Texture2D> chainTexture;

		public override void Load() 
        {
			chainTexture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Minibiomes/Desert/DinoArmHookChain");
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.SquirrelHook);
		}

		public override float GrappleRange() 
        {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) 
        {
			numHooks = 2;
		}

		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 18f;
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 12;
		}

		public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
			Tile tile = Main.tile[x, y];
			if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree) 
            {
				return true;
			}

			return null;
		}

		public override bool PreDrawExtras() 
        {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) 
            {
				directionToPlayer /= distanceToPlayer; 
				directionToPlayer *= chainTexture.Height(); 

				center += directionToPlayer;
				directionToPlayer = playerCenter - center; 
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition, chainTexture.Value.Bounds, drawColor, chainRotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}