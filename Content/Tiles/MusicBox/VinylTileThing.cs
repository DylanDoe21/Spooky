using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.MusicBox
{
    public class VinylTileThing : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            DustType = -1;
            HitSound = SoundID.Dig;
        }
    }

    public class VinylTileThing2 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing3 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing4 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing5 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing6 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing7 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing8 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}

    public class VinylTileThing9 : VinylTileThing
	{
		public override string Texture => "Spooky/Content/Tiles/MusicBox/VinylTileThing";
	}
}