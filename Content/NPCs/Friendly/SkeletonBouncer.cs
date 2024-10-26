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

namespace Spooky.Content.NPCs.Friendly
{
    public class SkeletonBouncer : ModNPC  
    {
        int NumAmmo = 200;
        int AmmoRegenTimer = 0;

        bool IsShooting = false;

        public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/PartyNailgun", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(NumAmmo);
            writer.Write(AmmoRegenTimer);

            //bools
            writer.Write(IsShooting);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            NumAmmo = reader.ReadInt32();
            AmmoRegenTimer = reader.ReadInt32();

            //bools
            IsShooting = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 9999;
            NPC.width = 42;
			NPC.height = 56;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            TownNPCStayingHomeless = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.RaveyardBiome>().Type };
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SkeletonBouncer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (!IsShooting)
            {
                //walking animation
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0 || NPC.velocity == Vector2.Zero)
                {
                    NPC.frame.Y = 2 * frameHeight;
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
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (player.position.X > NPC.Center.X + 65 || player.position.X < NPC.Center.X - 65)
                {
                    if (player.position.Y < NPC.Center.Y - 40)
                    {
                        NPC.frame.Y = 10 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 9 * frameHeight;
                    }
                }
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.SkeletonBouncer.Dialogue" + Main.rand.Next(1, 6));
		}

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			player.GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (projectile.type == ProjectileID.RottenEgg)
			{
				Main.player[projectile.owner].GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
			}
		}

		public override void AI()
        {
            NPC.spriteDirection = NPC.direction;

            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile && Main.LocalPlayer.active && !Main.LocalPlayer.dead)
            {
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];

                NPC.friendly = false;

                if (!player.dead)
                {
                    player.AddBuff(ModContent.BuffType<BouncerDeathmark>(), 2);
                }

                if (player.Distance(NPC.Center) < 400f && NumAmmo > 0)
                {
                    IsShooting = true;

                    NPC.aiStyle = 0;

                    NPC.velocity.X *= 0.5f;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] % 2 == 0 && NPC.velocity.Y == 0)
                    {
                        NumAmmo--;

                        SoundEngine.PlaySound(ShootSound, NPC.Center);

                        Vector2 positonToShootFrom = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (player.position.X < NPC.Center.X + 65 && player.position.X > NPC.Center.X - 65 && player.position.Y < NPC.Center.Y - 25)
                        {
                            positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? 6 : -6), NPC.Center.Y - 23);
                        }
                        else if (player.position.X > NPC.Center.X + 65 || player.position.X < NPC.Center.X - 65)
                        {
                            if (player.position.Y < NPC.Center.Y - 40)
                            {
                                positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -20 : 20), NPC.Center.Y - 15);
                            }
                            else
                            {
                                positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -20 : 20), NPC.Center.Y + 1);
                            }
                        }

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 5f;

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(10));

                        int bolt = Projectile.NewProjectile(NPC.GetSource_FromAI(), positonToShootFrom, newVelocity,
                        ModContent.ProjectileType<PartyNailBolt>(), player.statLifeMax / 2, 0, player.whoAmI, 0f, 0f);

                        Main.projectile[bolt].friendly = false;
                        Main.projectile[bolt].hostile = true;
                        Main.projectile[bolt].tileCollide = false;
                    }
                }
                else
                {
                    IsShooting = false;

                    NPC.aiStyle = 26;

                    if (NumAmmo < 200)
                    {
                        AmmoRegenTimer++;

                        if (AmmoRegenTimer > 300)
                        {
                            NumAmmo = 200;
                            AmmoRegenTimer = 0;
                        }    
                    }
                }
            }
            else
            {
                NPC.friendly = true;

                IsShooting = false;

                NPC.aiStyle = 7;
            }

            if (!Flags.RaveyardHappening)
            {
                NPC.alpha += 5;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PartyNailgun>()));
        }
    }
}