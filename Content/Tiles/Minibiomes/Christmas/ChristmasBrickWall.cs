using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasBrickRedWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(70, 5, 5));
            RegisterItemDrop(ModContent.ItemType<ChristmasBrickRedWallItem>());
            DustType = -1;
        }
	}

    public class ChristmasBrickBlueWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(27, 33, 81));
            RegisterItemDrop(ModContent.ItemType<ChristmasBrickBlueWallItem>());
            DustType = -1;
        }
	}

    public class ChristmasBrickGreenWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(23, 63, 32));
            RegisterItemDrop(ModContent.ItemType<ChristmasBrickGreenWallItem>());
            DustType = -1;
        }
	}

    public class ChristmasBrickRedWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickRedWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(70, 5, 5));
            DustType = -1;
        }
    }

    public class ChristmasBrickBlueWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickBlueWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(27, 33, 81));
            DustType = -1;
        }
    }

    public class ChristmasBrickGreenWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickGreenWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(23, 63, 32));
            DustType = -1;
        }
    }
}