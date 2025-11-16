using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class GlowshroomCandelabra : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(122, 72, 203), Lang.GetItemName(ItemID.Candelabra));
            RegisterItemDrop(ModContent.ItemType<GlowshroomCandelabraItem>());
            DustType = DustID.Slush;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Candelabras };
        }

        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            
            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {

                    if (Main.tile[x, y].TileFrameX >= 36)
                    {
                        Main.tile[x, y].TileFrameX -= 36;
                    }
                    else
                    {
                        Main.tile[x, y].TileFrameX += 36;
                    }
                }
            }

            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left + 1, top);
                Wiring.SkipWire(left + 1, top + 1);
            }

            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 300f;

                r = 122f / divide;
                g = 72f / divide;
                b = 203f / divide;
            }
        }
    }

    public class GlowshroomYellowCandelabra : GlowshroomCandelabra
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(179, 128, 50), Lang.GetItemName(ItemID.Candelabra));
            RegisterItemDrop(ModContent.ItemType<GlowshroomYellowCandelabraItem>());
            DustType = DustID.Slush;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Candelabras };
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 300f;

                r = 204f / divide;
                g = 164f / divide;
                b = 63f / divide;
            }
        }
    }
}