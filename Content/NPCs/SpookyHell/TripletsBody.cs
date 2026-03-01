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
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TripletsBody : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Pitch = 1.2f, Volume = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TripletsBestiary",
            };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4500;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 44;
            NPC.height = 40;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.1f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Triplets"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            if (NPC.ai[1] >= 3)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;

                player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false, null, true);
            }

            if (NPC.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //spawn eyes
                    for (int numEnemies = 0; numEnemies <= 2; numEnemies++)
                    {
                        int EyeToSpawn = ModContent.NPCType<TripletsEyeGreen>();

                        if (numEnemies == 1)
                        {
                            EyeToSpawn = ModContent.NPCType<TripletsEyePurple>();
                        }
                        if (numEnemies == 2)
                        {
                            EyeToSpawn = ModContent.NPCType<TripletsEyeRed>();
                        }

                        int NewEnemy = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, EyeToSpawn, ai1: numEnemies, ai2: Main.rand.Next(0, 2), ai3: NPC.whoAmI);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
                        }
                    }
                }

                NPC.ai[0]++;
                NPC.netUpdate = true;
            }
            
            NPC.localAI[0]++;
            if (NPC.localAI[0] >= 180)
            {   
                NPC.velocity *= 0.75f;
            }
            if (NPC.localAI[0] >= 300)
            {
                NPC.localAI[0] = 0;
            }

            //this is when the eyes shoot their laser
            if (NPC.localAI[0] == 239)
            {
                SoundEngine.PlaySound(SoundID.Item12 with { Pitch = -1.2f }, NPC.Center);
            }
            
            if (NPC.localAI[0] < 180)
            {
                int MaxSpeed = 2;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 1) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 1)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * 0.02f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed - 1, MaxSpeed + 1);
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * 0.01f;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
                //spawn blood explosion clouds
                for (int numExplosion = 0; numExplosion < 8; numExplosion++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(0.5f, 0.8f));
                    Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsBodyGore" + numGores).Type);
                    }
                }
            }
        }
    }
}