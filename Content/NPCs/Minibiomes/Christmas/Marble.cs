/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class Marble : ModNPC  
    {
        bool hasCollidedWithWall = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 25;
            NPC.defense = 10;
            NPC.width = 60;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Marble"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.rotation += 0.05f * (float)NPC.direction + (NPC.velocity.X / 40);

            //only run screenshake code if the player is close enough
            if (player.Distance(NPC.Center) < 250f)
            {
                //shake the screen if the skull is rolling at maximum speed
                if ((NPC.velocity.X >= 6 || NPC.velocity.X <= -6) && player.velocity.Y == 0 && Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                {
                    hasCollidedWithWall = false;

                    //kill this npc and spawn small marbles
                }
            }

            if (NPC.localAI[0] > 0)
            {
                NPC.localAI[0]--;

                NPC.velocity.X *= 0.2f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RollingSkullGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
*/