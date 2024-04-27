/*
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
    public class ShadowTrapdoor : ModTile
    {
        public override void SetStaticDefaults()
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.NotReallySolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			//TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(114, 13, 39), name);
            DustType = DustID.Blood;
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.HasItem(ItemID.ShadowKey) && Main.tile[i, j].TileFrameY <= 0)
            {
                SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 1;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 1; y++)
                    {
                        Main.tile[x, y].TileFrameY += 18;
                    }
                }
            }

            return base.RightClick(i, j);
        }

        public override bool Slope(int i, int j) 
        {
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = 1;
        }
    }
}
*/