using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;
using Spooky.Core;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ScarecrowShotgunner : ModNPC  
    {
        bool IsShooting = false;
        
        public static readonly SoundStyle ReloadSound = new("Spooky/Content/Sounds/ScarecrowReload", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) 
            {
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(IsShooting);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            IsShooting = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 350;
            NPC.damage = 50;
            NPC.defense = 5;
            NPC.width = 22;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinWarrior;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ScarecrowShotgunner"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (!IsShooting || (IsShooting && NPC.localAI[0] > 210))
            {
                //walking animation
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0 || NPC.velocity == Vector2.Zero)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                //still frame
                if (NPC.velocity == Vector2.Zero)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                Player player = Main.player[NPC.target];

                if (player.position.X < NPC.Center.X + 65 && player.position.X > NPC.Center.X - 65 && player.position.Y < NPC.Center.Y - 25)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (player.position.X > NPC.Center.X + 65 || player.position.X < NPC.Center.X - 65)
                {
                    if (player.position.Y < NPC.Center.Y - 40)
                    {
                        NPC.frame.Y = 7 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            if (player.Distance(NPC.Center) <= 350f || NPC.localAI[0] >= 30)
            {
                IsShooting = true;

                NPC.localAI[0]++;

                if (NPC.localAI[0] <= 210)
                {
                    NPC.velocity.X *= 0.5f;

                    NPC.aiStyle = 0;
                }
                else
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.GoblinWarrior;
                }

                //cock the shotgun 4 times
                if (NPC.localAI[0] == 30 || NPC.localAI[0] == 60 || NPC.localAI[0] == 90 || NPC.localAI[0] == 120)
                {
                    SoundEngine.PlaySound(ReloadSound, NPC.Center);
                }

                //shoot bullet spreads 4 times
                if (NPC.localAI[0] == 180 || NPC.localAI[0] == 190 || NPC.localAI[0] == 200 || NPC.localAI[0] == 210)
                {
                    SoundEngine.PlaySound(SoundID.Item38, NPC.Center);

                    Vector2 positonToShootFrom = new Vector2(NPC.Center.X, NPC.Center.Y);

                    if (player.position.X < NPC.Center.X + 70 && player.position.X > NPC.Center.X - 70 && player.position.Y < NPC.Center.Y - 25)
                    {
                        positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? 6 : -6), NPC.Center.Y - 22);
                    }
                    else if (player.position.X > NPC.Center.X + 70 || player.position.X < NPC.Center.X - 70)
                    {
                        if (player.position.Y < NPC.Center.Y - 40)
                        {
                            positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -20 : 20), NPC.Center.Y - 14);
                        }
                        else
                        {
                            positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -20 : 20), NPC.Center.Y + 2);
                        }
                    }

                    for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
                    {
                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 25f;

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(10));

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), positonToShootFrom, newVelocity, ProjectileID.BulletSnowman, NPC.damage / 4, 0, player.whoAmI);
                    }
                }
                
                //delay before it can shoot again
                if (NPC.localAI[0] >= 360)
                {
                    NPC.localAI[0] = 0;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                IsShooting = false;

                NPC.aiStyle = 3;
                AIType = NPCID.GoblinWarrior;
            }
        }
    }
}