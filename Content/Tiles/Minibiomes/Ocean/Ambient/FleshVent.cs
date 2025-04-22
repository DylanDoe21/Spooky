using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class FleshVent : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(140, 52, 24));
			DustType = DustID.Blood;
			HitSound = SoundID.Dig;
        }

		public override void NearbyEffects(int i, int j, bool closer)
		{
            if (Main.rand.NextBool(2) && Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0 && !Main.gamePaused && Main.instance.IsActive)
            {
                Vector2 Position = new Vector2(i * 16 + 20, j * 16 + 10);
				Dust.NewDustPerfect(Position, ModContent.DustType<SmokeEffect>(), new Vector2(0, Main.rand.NextFloat(-1f, -0.5f)), 125, Color.Brown * 0.5f, 0.2f);
            }
			if (Main.rand.NextBool(2) && Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0 && !Main.gamePaused && Main.instance.IsActive)
			{
				Vector2 Position = new Vector2(i * 16 + 53, j * 16 + 6);
				Dust.NewDustPerfect(Position, ModContent.DustType<SmokeEffect>(), new Vector2(0, Main.rand.NextFloat(-1f, -0.5f)), 125, Color.Brown * 0.5f, 0.2f);
			}
			if (Main.rand.NextBool(2) && Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0 && !Main.gamePaused && Main.instance.IsActive)
			{
				Vector2 Position = new Vector2(i * 16 + 5, j * 16 + 25);
				Dust.NewDustPerfect(Position, ModContent.DustType<SmokeEffect>(), new Vector2(0, Main.rand.NextFloat(-1f, -0.5f)), 125, Color.Brown * 0.5f, 0.2f);
			}
		}
    }
}