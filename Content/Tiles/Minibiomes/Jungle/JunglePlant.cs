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

namespace Spooky.Content.Tiles.Minibiomes.Jungle
{
    public class JunglePlant1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(41, 129, 52));
            DustType = DustID.Grass;
            HitSound = SoundID.Dig;
        }
    }

    public class JunglePlant2 : JunglePlant1
    {
    }

    public class JunglePlant3 : JunglePlant1
    {
    }

    public class JunglePlant4 : JunglePlant1
    {
    }

    public class JunglePlant5 : JunglePlant1
    {
    }

    public class JunglePlant6 : JunglePlant1
    {
    }
}