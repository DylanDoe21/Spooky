using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class NineTailsProj : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 28;
			Projectile.WhipSettings.RangeMultiplier = 0.93f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.75f);
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
				Rectangle frame = new Rectangle(0, 0, 14, 18);
				Vector2 origin = new Vector2(7, 7);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
                {
					frame.Y = 58;
					frame.Height = 18;
				}
				//two body segments
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
				//bottom segment
				else if (i > 0) 
                {
					frame.Y = 22;
					frame.Height = 22;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}

			return false;
		}
	}
}