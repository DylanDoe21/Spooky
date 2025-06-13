using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
    public class LabMetalPlateWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(48, 60, 70));
        }
    }
}