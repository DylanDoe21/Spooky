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

namespace Spooky.Content.Tiles.Minibiomes.Desert.Ambient
{
    public class DesertPileSmall1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(154, 80, 36));
            DustType = DustID.Sand;
            HitSound = SoundID.Dig;
        }
    }

    public class DesertPileSmall2 : DesertPileSmall1
    {
    }

    public class DesertPileSmall3 : DesertPileSmall1
    {
    }
}