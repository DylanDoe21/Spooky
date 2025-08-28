using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class TubeCoralLime1 : ModTile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private Asset<Texture2D> PlantTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(95, 178, 51));
            HitSound = SoundID.Dig;
            DustType = -1;
        }

        public static void DrawPlant(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
		{
			Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));

			Main.spriteBatch.Draw(tex, drawPos, source, Lighting.GetColor(i, j), 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			//do not draw the tile texture itself
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralLime");
			
			Tile tile = Framing.GetTileSafely(i, j);

			//draw the tile only on the bottom center of each tiles y-frame
			if (tile.TileFrameX == 18 && tile.TileFrameY == 36)
			{
                //reminder: offset negative numbers are right and down, while positive is left and up
                Vector2 offset = new Vector2((PlantTexture.Width() / 2) - 4, (PlantTexture.Height() / 3) - 12);

                DrawPlant(i, j, PlantTexture.Value, new Rectangle(0, 66 * 0, 62, 64), TileGlobal.TileOffset, offset);
			}
		}
    }

    public class TubeCoralLime2 : TubeCoralLime1
    {
        private Asset<Texture2D> PlantTexture;
        
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralLime");
			
			Tile tile = Framing.GetTileSafely(i, j);

			//draw the tile only on the bottom center of each tiles y-frame
			if (tile.TileFrameX == 18 && tile.TileFrameY == 36)
			{
                //reminder: offset negative numbers are right and down, while positive is left and up
                Vector2 offset = new Vector2((PlantTexture.Width() / 2) - 4, (PlantTexture.Height() / 3) - 12);

                DrawPlant(i, j, PlantTexture.Value, new Rectangle(0, 66 * 1, 62, 64), TileGlobal.TileOffset, offset);
			}
		}
    }

    public class TubeCoralLime3 : TubeCoralLime1
    {
        private Asset<Texture2D> PlantTexture;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralLime");
			
			Tile tile = Framing.GetTileSafely(i, j);

			//draw the tile only on the bottom center of each tiles y-frame
			if (tile.TileFrameX == 18 && tile.TileFrameY == 36)
			{
                //reminder: offset negative numbers are right and down, while positive is left and up
                Vector2 offset = new Vector2((PlantTexture.Width() / 2) - 4, (PlantTexture.Height() / 3) - 12);

                DrawPlant(i, j, PlantTexture.Value, new Rectangle(0, 66 * 2, 62, 64), TileGlobal.TileOffset, offset);
			}
		}
    }
}