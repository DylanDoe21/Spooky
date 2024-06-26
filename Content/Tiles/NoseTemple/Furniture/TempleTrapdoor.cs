using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
    public class TempleTrapdoor : ModTile
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
			TileID.Sets.HasOutlines[Type] = true;
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
            AddMapEntry(new Color(125, 125, 125), name);
            DustType = DustID.Stone;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.ShadowKey;
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.HasItem(ItemID.ShadowKey))
            {
                SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i * 16, j * 16));

                int left = i - Framing.GetTileSafely(i, j).TileFrameX / 18 % 3;
                int top = j - Framing.GetTileSafely(i, j).TileFrameY / 18 % 1;

                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 1; y++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
						tile.HasTile = false;
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
				    NetMessage.SendTileSquare(-1, i, j + 1, 3);
                }
            }

            return true;
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