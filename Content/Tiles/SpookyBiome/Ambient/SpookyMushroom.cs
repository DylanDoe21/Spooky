using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyMushroom : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(166, 158, 187));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.15f;
            g = 0.05f;
            b = 0.15f;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -14;
            height = 32;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			int type = -1;

			if (tileBelow.HasTile)
            {
				type = tileBelow.TileType;
			}

			if (type == ModContent.TileType<MushroomMoss>()) 
            {
				return true;
			}

			WorldGen.KillTile(i, j);

			return true;
		}
    }
}