using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.UserInterfaces;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
	public class LabComputerDummy : ModNPC
	{
		public override void SetStaticDefaults()
		{
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 54;
			NPC.height = 36;
			NPC.friendly = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.alpha = 255;
            NPC.aiStyle = -1;
		}

		public override bool CanChat() 
        {
			return true;
		}

		public override string GetChat()
		{
			RottenDepthsEmailUI.ComputerPos = NPC.Center;
			RottenDepthsEmailUI.UIOpen = true;

			return "";
		}

		public override void AI()
		{
			if (Main.tile[(int)NPC.Center.X / 16, (int)NPC.Center.Y / 16].TileType != ModContent.TileType<LabComputer>())
			{
				NPC.active = false;
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}