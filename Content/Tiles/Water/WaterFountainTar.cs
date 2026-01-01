using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.Water
{
    public class WaterFountainTar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AnimationFrameHeight = 54;
            AddMapEntry(new Color(125, 125, 125)); //, Language.GetText("MapObject.FloorLamp"));
            RegisterItemDrop(ModContent.ItemType<WaterFountainTarItem>());
            DustType = DustID.Stone;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.dedServ && Main.tile[i, j].TileFrameX >= 36)
            {
                Main.SceneMetrics.ActiveFountainColor = ModContent.Find<ModWaterStyle>("Spooky/TarWaterStyle").Slot;
            }
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 3;
                frameCounter = 0;
            }
        }

        public void HandleActivation(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 3;

			if (tile.TileFrameX < 36)
			{
				for (int x = left; x < left + 2; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						Tile CheckTile = Framing.GetTileSafely(x, y);
						CheckTile.TileFrameX += 36;
					}
				}
			}
            else
            {
                for (int x = left; x < left + 2; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						Tile CheckTile = Framing.GetTileSafely(x, y);
						CheckTile.TileFrameX -= 36;
					}
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
            player.cursorItemIconID = ModContent.ItemType<WaterFountainTarItem>();
        }
    }
}