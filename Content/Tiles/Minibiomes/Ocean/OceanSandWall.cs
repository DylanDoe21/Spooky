using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
    public class OceanSandWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(111, 96, 51));
            //DustType = DustID.Grass;
        }

        /*
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            //spawn water infront of the wall
            if ((tile.LiquidAmount == 0 || tile.LiquidType == LiquidID.Water) && !tile.HasTile)
            {
                tile.Get<LiquidData>().LiquidType = LiquidID.Water;
                tile.LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.sendWater(i, j);
                }
            }
        }
        */
    }
}