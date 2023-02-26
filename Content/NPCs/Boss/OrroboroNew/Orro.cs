using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.OrroboroNew
{
    //[AutoloadBossHead]
    public class Orro : ModNPC
    {
        public bool idleRotation = true;
        public bool spawnedOtherWorm = false;

        Vector2 SavePlayerPosition;
        Vector2 SaveNPCPosition;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/OrroboroNew/OrroBC",
                Position = new Vector2(100f, 5f),
                PortraitPositionXOverride = 100f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(idleRotation);
            writer.Write(spawnedOtherWorm);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            idleRotation = reader.ReadBoolean();
            spawnedOtherWorm = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 28000;
            NPC.damage = 65;
            NPC.defense = 65;
            NPC.width = 128;
            NPC.height = 276;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 30, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A powerful and calculated creature that will work together with Boro to defend its territory. Its regal appearance reflects its abilities to use magic and long range combat."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //normal animation
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override void AI()
        {   
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 90 / 3 : Main.expertMode ? 70 / 2 : 50;

            NPC.spriteDirection = NPC.direction;

            //use rotation depending on if its idle or not
            if (idleRotation)
            {
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                float RotateX = player.Center.X - vector.X;
                float RotateY = player.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            }
            else
            {
                NPC.rotation = NPC.velocity.ToRotation() + (int)4.71239;
			    NPC.rotation += 0f * NPC.direction;
            }

            switch ((int)NPC.ai[0])
			{
                //turn invisible and go above player when boro is spawned
                //if orro is spawned after boro, then stop attacking, summon boro back to the fight, and reset ai
                case -1:
				{
                    break;
                }

                //shoot spread of spikes at the player (mfw ror2 imp overlord attack)
                case 0:
				{
                    break;
                }

                //do a big, predictive dash toward the player
                case 1:
				{
                    break;
                }

                //shoot bouncy biomass that creates a big debuff aura
                case 2:
				{
                    break;
                }

                //go under the player, then charge up and shoot a spread of magic projectiles or something
                case 3:
				{
                    break;
                }

                //shoot a bunch of small, homing biomass that explodes into debuff auras
                case 4:
				{
                    break;
                }
            }
		}

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            /*
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagBigBone>()));
            
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SkullSeed>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BigBoneRelicItem>()));

            //normal drops
            int[] MainItem = new int[] { ModContent.ItemType<BigBoneHammer>(), ModContent.ItemType<BigBoneBow>(), 
            ModContent.ItemType<BigBoneStaff>(), ModContent.ItemType<BigBoneScepter>() };

            notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

            npcLoot.Add(notExpertRule);
            */
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}