using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Spooky.Content.Gores
{
	public class LeafGreen : ModGore
	{
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
			ChildSafety.SafeGore[gore.type] = true;
			gore.behindTiles = true;
			gore.numFrames = 8;
			gore.timeLeft = Gore.goreTime * 3;
		}

        public override bool Update(Gore gore)
		{
			gore.rotation = gore.velocity.ToRotation() + MathHelper.PiOver2;
			gore.position += gore.velocity;
			if (Collision.SolidCollision(gore.position, 2, 2) || gore.timeLeft <= 60) 
            {
				gore.alpha += 25;
				if (gore.alpha >= 255)
				{
					gore.active = false;
					return false;
				}
			}

			gore.frameCounter++;
			if (gore.frameCounter > 7) 
            {
				gore.frameCounter = 0;
				gore.frame++;
				if (gore.frame > 7) 
                {
                    gore.frame = 0;
                }
			}

			return false;
		}
	}

	public class LeafOrange : LeafGreen
	{
	}

	public class LeafRed : LeafGreen
	{
	}

	public class LeafGreenTreeFX : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			gore.numFrames = 8;
			gore.frame = (byte)Main.rand.Next(8);
			gore.frameCounter = (byte)Main.rand.Next(8);
			UpdateType = 910;
		}
	}

	public class LeafOrangeTreeFX : LeafGreenTreeFX
	{
	}

	public class LeafRedTreeFX : LeafGreenTreeFX
	{
	}
}