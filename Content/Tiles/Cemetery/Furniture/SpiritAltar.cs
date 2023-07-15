using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class SpiritAltar : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 5;	
			TileObjectData.newTile.Origin = new Point16(1, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(136, 0, 253), name);
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<EMFReader>();
            player.cursorItemIconText = "";
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }

        public override bool RightClick(int i, int j)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()))
            {
                return true;
            }

            for (int k = 0; k < Main.projectile.Length; k++)
            {

                if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<SpookySpiritSpawn>())
                {
                    return true;
                }
            }

            //check if player has the EMF reader
            Player player = Main.LocalPlayer;
            if (player.HasItem(ModContent.ItemType<EMFReader>()) && !Main.dayTime)
            {
                int x = i;
                int y = j;
                while (Main.tile[x, y].TileType == Type) x--;
                x++;
                while (Main.tile[x, y].TileType == Type) y--;
                y++;

                Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, x * 16, y * 16),
                x * 16 + 28, y * 16 + 20, 0, -1, ModContent.ProjectileType<SpookySpiritSpawn>(), 0, 0, Main.myPlayer);
            }

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            //draw actual glowmask
            Texture2D glowtex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Furniture/SpiritAltarGlow").Value;

            spriteBatch.Draw(glowtex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);

            if (!Main.dayTime)
            {
                //draw glowy outline
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Furniture/SpiritAltarOutline").Value;

                float glowspeed = Main.GameUpdateCount * 0.02f;
                float glowbrightness = (float)MathF.Sin(j / 15f - glowspeed);

                spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Fuchsia * glowbrightness);

            }
        }
    }
}