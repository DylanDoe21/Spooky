using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Ocean;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabEntranceSensor : ModTile
    {
        public override void SetStaticDefaults()
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
			TileID.Sets.NotReallySolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(Color.MediumSpringGreen);
            DustType = -1;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            float divide = 700f;

			r = 124f / divide;
			g = 177f / divide;
			b = 99f / divide;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = 1;
        }
    }

    public class LabSensorPlayer : ModPlayer
    {
        int Delay = 0;

        public static readonly SoundStyle SensorSound = new("Spooky/Content/Sounds/EMFGhostPowerful", SoundType.Sound) { Volume = 0.7f };

        public override void PostUpdate()
		{
            Tile tile = Main.tile[(int)Player.Center.X / 16, (int)Player.Center.Y / 16];
            if (Delay <= 0 && tile.TileType == ModContent.TileType<LabEntranceSensor>())
            {
                SoundEngine.PlaySound(SensorSound, Player.Center);

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.type == ModContent.NPCType<Dunkleosteus>())
                    {
                        Dunkleosteus.AlertedPosition = Player.Center;
                        npc.ai[0] = 0;
                    }
                }

                Delay = 35;
            }
            
            if (Delay > 0 && tile.TileType != ModContent.TileType<LabEntranceSensor>())
            {
                Delay--;
            }
        }
    }
}