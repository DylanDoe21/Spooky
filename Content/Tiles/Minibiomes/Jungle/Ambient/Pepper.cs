using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Minibiomes.Jungle.Ambient
{
    public class Pepper : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(201, 34, 32));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			if (Main.rand.NextBool(400) && !Main.tile[i, j - 1].HasTile && !Main.gamePaused && Main.instance.IsActive)
			{
				Dust.NewDust(new Vector2((i - 1) * 16, (j - 1) * 16), 1, 1, Main.rand.NextFromList(ModContent.DustType<StinkCloud1>(), ModContent.DustType<StinkCloud2>()));
			}
		}
	}
}