using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
    public class GnomeHouse1 : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(228, 188, 68));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Furniture/GnomeHouse1Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.2f);
        }

        public override void NearbyEffects(int i, int j, bool closer)
		{
            if (Main.rand.NextBool(500) && GnomeCount(i, j) <= 0 && Main.tile[i, j].TileFrameX == 18 && Main.tile[i, j].TileFrameY == 36)
            {
                Spooky.MushGnomeSpawnX = (i * 16) + 5;
                Spooky.MushGnomeSpawnY = (j * 16) + 6;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.SpawnMushGnome);
                    packet.Send();
                }
                else
                {
                    int[] Gnomes = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };
                    NPC.NewNPC(null, Spooky.MushGnomeSpawnX, Spooky.MushGnomeSpawnY, Main.rand.Next(Gnomes));
                }
            }
        }

        public static int GnomeCount(int X, int Y)
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC GnomeNPC = Main.npc[i];

				int[] EventNPCs = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), 
				ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };

                Vector2 TilePos = new Vector2(X * 16, Y * 16);

				if (GnomeNPC.active && EventNPCs.Contains(GnomeNPC.type) && GnomeNPC.Distance(TilePos) <= 200f)
				{
					NpcCount++;
				}
				else
				{
					continue;
				}
			}

			return NpcCount;
		}
    }

    public class GnomeHouse2 : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(1, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(98, 80, 183));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Furniture/GnomeHouse2Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.2f);
        }

        public override void NearbyEffects(int i, int j, bool closer)
		{
            if (Main.rand.NextBool(500) && GnomeHouse1.GnomeCount(i, j) <= 0 && Main.tile[i, j].TileFrameX == 18 && Main.tile[i, j].TileFrameY == 72)
            {
                Spooky.MushGnomeSpawnX = (i * 16) + 5;
                Spooky.MushGnomeSpawnY = (j * 16) + 6;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.SpawnMushGnome);
                    packet.Send();
                }
                else
                {
                    int[] Gnomes = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };
                    NPC.NewNPC(null, Spooky.MushGnomeSpawnX, Spooky.MushGnomeSpawnY, Main.rand.Next(Gnomes));
                }
            }
        }
    }

    public class GnomeHouse3 : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(1, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(186, 23, 14));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Furniture/GnomeHouse3Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.2f);
        }

        public override void NearbyEffects(int i, int j, bool closer)
		{
            if (Main.rand.NextBool(500) && GnomeHouse1.GnomeCount(i, j) <= 0 && Main.tile[i, j].TileFrameX == 18 && Main.tile[i, j].TileFrameY == 72)
            {
                Spooky.MushGnomeSpawnX = (i * 16) + 5;
                Spooky.MushGnomeSpawnY = (j * 16) + 6;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.SpawnMushGnome);
                    packet.Send();
                }
                else
                {
                    int[] Gnomes = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };
                    NPC.NewNPC(null, Spooky.MushGnomeSpawnX, Spooky.MushGnomeSpawnY, Main.rand.Next(Gnomes));
                }
            }
        }
    }

    public class GnomeHouse4 : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(82, 165, 76));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Furniture/GnomeHouse4Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.2f);
        }

        public override void NearbyEffects(int i, int j, bool closer)
		{
            if (Main.rand.NextBool(500) && GnomeHouse1.GnomeCount(i, j) <= 0 && Main.tile[i, j].TileFrameX == 18 && Main.tile[i, j].TileFrameY == 54)
            {
                Spooky.MushGnomeSpawnX = (i * 16) + 5;
                Spooky.MushGnomeSpawnY = (j * 16) + 6;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.SpawnMushGnome);
                    packet.Send();
                }
                else
                {
                    int[] Gnomes = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };
                    int Gnome = NPC.NewNPC(null, Spooky.MushGnomeSpawnX, Spooky.MushGnomeSpawnY, Main.rand.Next(Gnomes));
                    Main.npc[Gnome].velocity.X = Main.rand.NextBool() ? -1 : 1;
                }
            }
        }
    }
}