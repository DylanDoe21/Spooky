using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
    public class SporeMushroom : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick2Grass>() };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(31, 85, 37));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX == 18 * 0 || tile.TileFrameX == 18 * 1)
            {
                r = 0f;
                g = 0.2f;
                b = 0.5f;
            }
            if (tile.TileFrameX == 18 * 2 || tile.TileFrameX == 18 * 3)
            {
                r = 0.5f;
                g = 0f;
                b = 0f;
            }
            if (tile.TileFrameX == 18 * 4 || tile.TileFrameX == 18 * 5)
            {
                r = 0.5f;
                g = 0.25f;
                b = 0f;
            }
            if (tile.TileFrameX == 18 * 6 || tile.TileFrameX == 18 * 7)
            {
                r = 0.15f;
                g = 0.5f;
                b = 0f;
            }
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
    }
}