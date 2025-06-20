using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
	public class LabMetalPipe : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(119, 124, 149));
			RegisterItemDrop(ModContent.ItemType<LabMetalPipeItem>());
            DustType = DustID.Iron;
            HitSound = SoundID.Item52;
		}

		public override bool Slope(int i, int j)
		{
			return false;
		}
	}

	public class LabMetalPipeSafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/LabMetalPipe";

		public override void SetStaticDefaults()
		{
            Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(119, 124, 149));
            DustType = -1;
            HitSound = SoundID.Item52;
		}

		public override bool Slope(int i, int j)
		{
			return false;
		}
	}
}
