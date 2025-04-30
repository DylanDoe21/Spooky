using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
    public class DesertSandstoneWall : ModWall 
    {
        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(105, 48, 10));
            DustType = DustID.YellowStarfish;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Desert/DesertSandstoneWallMerge");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//down wall merge
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //left wall merge
            if (Main.tile[i - 1, j].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //up wall merge
            if (Main.tile[i, j - 1].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //right wall merge
            if (Main.tile[i + 1, j].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
            }
        }
    }

    public class DesertSandstoneWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Desert/DesertSandstoneWall";

        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(105, 48, 10));
            DustType = DustID.YellowStarfish;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Desert/DesertSandstoneWallMerge");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//down wall merge
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //left wall merge
            if (Main.tile[i - 1, j].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //up wall merge
            if (Main.tile[i, j - 1].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
            }
            //right wall merge
            if (Main.tile[i + 1, j].WallType == ModContent.WallType<DesertSandWall>())
            {
                spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
            }
        }
    }
}