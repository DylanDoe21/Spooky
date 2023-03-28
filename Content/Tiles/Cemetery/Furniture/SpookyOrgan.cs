using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class SpookyOrgan : ModTile
    {
        public static readonly SoundStyle OrganSound = new("Spooky/Content/Sounds/Organ", SoundType.Sound);

		public override void SetStaticDefaults()
		{
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Spirit Organ");
			AddMapEntry(new Color(136, 0, 253), name);
            DustType = DustID.WoodFurniture;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        /*
        public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled  = true;
			player.cursorItemIconID = ModContent.ItemType<OldWoodOrganItem>();
			player.cursorItemIconText = "";
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
        */

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}

        public override bool RightClick(int i, int j)
		{
            int x = i;
            int y = j;
            while (Main.tile[x, y].TileType == Type) x--;
            x++;
            while (Main.tile[x, y].TileType == Type) y--;
            y++;

            if (!NPC.downedBoss2)
            {
                Main.NewText("The spirit has not deemed you worthy of playing it's organ", 171, 64, 255);
            }
            else
            {
                SoundEngine.PlaySound(OrganSound, new Vector2(x * 16f, y * 16f));
            }

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Furniture/SpookyOrganOutline").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			intensity *= (float)MathF.Sin(-i / 8f + time + j);
			intensity += 0.7f;

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Purple * intensity);
		}
    }
}