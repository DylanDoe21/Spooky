using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class Casket1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(93, 62, 39));
            DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(750) && !Main.gamePaused && Main.instance.IsActive)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.PooFly, new ParticleOrchestraSettings
                {
                    PositionInWorld = new Vector2(i * 16 + 8, j * 16 - 8)
                });
            }
        }
    }

	public class Casket2 : Casket1
    {
	}

	public class Casket3 : Casket1
    {
	}

	public class Casket4 : Casket1
    {
	}
}