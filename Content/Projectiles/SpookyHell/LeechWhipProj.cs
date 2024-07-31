using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class LeechWhipProj : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 50;
			Projectile.WhipSettings.RangeMultiplier = 2.1f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			if (Main.rand.NextBool(15))
            {
				Item.NewItem(Projectile.GetSource_DropAsItem(), target.position, target.Size, ModContent.ItemType<LeechWhipGore>());
            }
		}

		public override bool PreDraw(ref Color lightColor) 
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				//14 is the width of the whole whip, 26 is the height for the tips hotbox
				Rectangle frame = new Rectangle(0, 0, 20, 26);
				Vector2 origin = new Vector2(10, 10);
				float scale = 1;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2) 
                {
					frame.Y = 72;
					frame.Height = 18;
				}
				//loop between the two middle segments
				else if (i % 2 == 0) 
				{
					frame.Y = 56;
					frame.Height = 16;
				}
				else if (i % 1 == 0) 
				{
					frame.Y = 40;
					frame.Height = 16;
				}
				else if (i > 0) 
                {
					frame.Y = 24;
					frame.Height = 24;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}