using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class TomatoPuttyFlyingSmall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.width = 38;
            NPC.height = 28;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.15f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 14;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPuttyFlyingSmall"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //flying animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 12; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.Red, 1.25f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
        }
    }
}