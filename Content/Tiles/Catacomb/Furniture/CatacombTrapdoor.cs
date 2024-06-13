using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class CatacombTrapdoor1 : ModTile
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
            TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.Origin = new Point16(3, 0);
            TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Color.Gray, name);
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
            player.cursorItemIconID = ModContent.ItemType<CatacombKey1>();
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<CatacombKey1>()))
            {
                SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 7;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 1;

                for (int x = left; x < left + 7; x++)
                {
                    for (int y = top; y < top + 1; y++)
                    {
                        WorldGen.KillTile(x, y);
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.CatacombKey1);
                    packet.Send();
                }
                else
                {
                    Flags.CatacombKey1 = true;
                }
            }

            return base.RightClick(i, j);
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

    public class CatacombTrapdoor2 : ModTile
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
            TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.Origin = new Point16(3, 0);
            TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Color.Gray, name);
            DustType = DustID.Stone;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CatacombKey2>();
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<CatacombKey2>()))
            {
                SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 7;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 1;

                for (int x = left; x < left + 7; x++)
                {
                    for (int y = top; y < top + 1; y++)
                    {
                        WorldGen.KillTile(x, y);
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.CatacombKey2);
                    packet.Send();
                }
                else
                {
                    Flags.CatacombKey2 = true;
                }
            }

            return base.RightClick(i, j);
        }
    }

    public class CatacombTrapdoor3 : ModTile
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
            TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.Origin = new Point16(3, 0);
            TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Color.Gray, name);
            DustType = DustID.Stone;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CatacombKey3>();
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<CatacombKey3>()))
            {
                SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i * 16, j * 16));

                int left = i - Main.tile[i, j].TileFrameX / 18 % 7;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 1;

                for (int x = left; x < left + 7; x++)
                {
                    for (int y = top; y < top + 1; y++)
                    {
                        WorldGen.KillTile(x, y);
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.CatacombKey3);
                    packet.Send();
                }
                else
                {
                    Flags.CatacombKey3 = true;
                }
            }

            return base.RightClick(i, j);
        }
    }
}