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
    public class CatacombWeeds : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(31, 85, 37));
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
            HitSound = SoundID.Grass;
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

			if (tileBelow.HasTile && tileBelow.TileType == ModContent.TileType<CemeteryGrass>()) 
            {
				return true;
			}
            else
            {
			    WorldGen.KillTile(i, j);
            }

			return true;
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(35) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<CemeteryGrassSeeds>());
            }
        }
    }
}