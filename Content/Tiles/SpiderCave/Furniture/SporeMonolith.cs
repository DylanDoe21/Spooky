using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
    public class SporeMonolith : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<SporeMonolithItem>());
            AddMapEntry(new Color(197, 187, 215));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public void HandleActivation(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 2;

            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    Tile CheckTile = Framing.GetTileSafely(x, y);
                    CheckTile.TileType = (ushort)ModContent.TileType<SporeMonolithOn>();
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, left, top, 6);
            }
        }

        public override void HitWire(int i, int j)
        {
            HandleActivation(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            HandleActivation(i, j);

            return true;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SporeMonolithItem>();
		}
	}

    public class SporeMonolithOn : SporeMonolith
    {
        public void HandleActivation(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 2;

            for (int x = left; x < left + 2; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    Tile CheckTile = Framing.GetTileSafely(x, y);
					CheckTile.TileType = (ushort)ModContent.TileType<SporeMonolith>();
				}
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, left, top, 6);
            }
        }

        public override void HitWire(int i, int j)
        {
            HandleActivation(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            HandleActivation(i, j);

            return true;
        }

        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SporeMonolithItem>();
		}
    }
}