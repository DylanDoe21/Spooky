using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient
{
	public class JungleCabbageBoulderProj : ModProjectile
	{
		public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Boulder);
            Projectile.Size = new Vector2(48, 46);
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
		}

		public override bool? CanCutTiles()
        {
            return false;
        }
		
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return oldVelocity.X != Projectile.velocity.X;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);

            for (int numDusts = 0; numDusts < 35; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].noGravity = true;
            }
		}
	}
}