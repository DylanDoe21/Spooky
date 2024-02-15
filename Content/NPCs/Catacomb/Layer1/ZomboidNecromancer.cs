using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class ZomboidNecromancer : ModNPC
    {
        Vector2 SoulSpawnPosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 75);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
            AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidNecromancer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
            //use regular walking anim when walking
            NPC.frameCounter++;
            if (NPC.localAI[0] <= 360)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
            }
            //use casting animation during casting ai
            if (NPC.localAI[0] > 360)
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            //list of valid enemies that the necromancer can absorb souls from
            int[] NecromancerValidEnemies = new int[] { ModContent.NPCType<BoneStackerMoving1>(), ModContent.NPCType<BoneStackerMoving2>(), ModContent.NPCType<BoneStackerMoving3>(), 
            ModContent.NPCType<RollingSkull1>(), ModContent.NPCType<RollingSkull2>(), ModContent.NPCType<RollingSkull3>(), ModContent.NPCType<RollingSkull4>(),
            ModContent.NPCType<Skeletoid1>(), ModContent.NPCType<Skeletoid2>(), ModContent.NPCType<Skeletoid3>(), ModContent.NPCType<Skeletoid4>(), ModContent.NPCType<SkeletoidBig>() };

            //check every npc and if it is valid, spawn a soul if its close enough when it dies
            for (int i = 0; i <= Main.maxNPCs; i++)
            {
                NPC UndeadNPC = Main.npc[i];
                if (NecromancerValidEnemies.Contains(UndeadNPC.type) && UndeadNPC.Distance(NPC.Center) < 505f && UndeadNPC.life <= 0)
                {
                    int Soul = NPC.NewNPC(NPC.GetSource_FromAI(), (int)UndeadNPC.Center.X, (int)UndeadNPC.Center.Y, ModContent.NPCType<ZomboidNecromancerSoul>(), ai0: NPC.whoAmI);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Soul);
                    }
                }
            }

            //if the necromancer has absorbed a soul, start the summoning countdown
            if (NPC.localAI[1] > 0)
            {
                NPC.localAI[0]++;

                if (NPC.localAI[0] > 360)
                {
                    NPC.aiStyle = 0;

                    NPC.velocity.X *= 0.5f;
                }
                else
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.Crab;
                }

                //spawn a revived "ghost" of a skeletoid or a rolling skull
                if (NPC.localAI[0] >= 450)
                {
                    int[] PhantomEnemy = new int[] { ModContent.NPCType<PhantomRollingSkull>(), ModContent.NPCType<PhantomSkeletoid>() };

                    int SummonedGhost = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, Main.rand.Next(PhantomEnemy));

                    //set the spawned enemies stats based on how many souls have been absorbed, only if multiple souls have been absorbed
                    if (NPC.localAI[1] > 1)
                    {
                        int StatScalingValue = (int)NPC.localAI[1] * 3;

                        Main.npc[SummonedGhost].lifeMax += StatScalingValue;
                        Main.npc[SummonedGhost].life = Main.npc[SummonedGhost].lifeMax;
                        Main.npc[SummonedGhost].damage += StatScalingValue;
                    }
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: SummonedGhost);
                    }

                    for (int numDusts = 0; numDusts < 20; numDusts++)
                    {
                        int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                        Main.dust[dustGore].color = Main.rand.NextBool() ? Color.Cyan : Color.Purple;
                        Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 3f);
                        Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 3f);
                        Main.dust[dustGore].noGravity = true;
                    }

                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                }
            }
            else
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullAmulet>(), 30));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZomboidNecromancerHood>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidNecromancerGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidNecromancerCloth" + numGores).Type);
                    }
                }
            }
        }
    }
}