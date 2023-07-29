using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushLakeWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(19, 14, 37));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.Dig;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if ((Main.tile[i, j].LiquidAmount == 0 || Main.tile[i, j].LiquidType == LiquidID.Water) && !Main.tile[i, j].HasTile)
            {
                Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                Main.tile[i, j].LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.sendWater(i, j);
                }
            }

            if ((Main.tile[i, j - 1].LiquidAmount == 0 || Main.tile[i, j - 1].LiquidType == LiquidID.Water) && !Main.tile[i, j - 1].HasTile)
            {
                Main.tile[i, j - 1].Get<LiquidData>().LiquidType = LiquidID.Water;
                Main.tile[i, j - 1].LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j - 1);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.sendWater(i, j - 1);
                }
            }
        }
    }
}