using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;
using System.Linq;
using System.IO;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigFlowerPot : ModNPC  
    {
		int[] AttackPattern = new int[] { 0, 1, 2 };

		bool SpawnedBigBone = false;
		bool RandomizedPattern = false;

		public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			//attack pattern
			writer.Write(AttackPattern[0]);
			writer.Write(AttackPattern[1]);
			writer.Write(AttackPattern[2]);

			//bools
			writer.Write(SpawnedBigBone);
			writer.Write(RandomizedPattern);

			//floats
			writer.Write(NPC.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//attack pattern
			AttackPattern[0] = reader.ReadInt32();
			AttackPattern[1] = reader.ReadInt32();
			AttackPattern[2] = reader.ReadInt32();

			//bools
			SpawnedBigBone = reader.ReadBoolean();
			RandomizedPattern = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
		}

		public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.width = 130;
            NPC.height = 114;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }

		public override bool CheckActive()
        {
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public void SwitchToNextAttack(Player player)
		{
			NPC.ai[1]++;

			if (NPC.ai[1] >= AttackPattern.Length)
			{
				NPC.ai[1] = 0;
				RandomizedPattern = false;
			}
		}

		public override void AI()
        {
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			//if the pot has been watered spawn big bone as a bulb
			if (NPC.ai[2] == 1)
            {
                if (!SpawnedBigBone)
                {
                    //spawn message
                    string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.BigBulbSpawn");

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText(text, 171, 64, 255);
                    }
                    else
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                    }
                    
                    int BigBone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 100, ModContent.NPCType<BigBone>(), ai3: NPC.whoAmI);
                    
                    NetMessage.SendData(MessageID.SyncNPC, number: BigBone);

                    SpawnedBigBone = true;
                    NPC.netUpdate = true;
                }

				if (!RandomizedPattern)
				{
					//shuffle the attack pattern list
					AttackPattern = AttackPattern.OrderBy(x => Main.rand.Next()).ToArray();
					RandomizedPattern = true;
					NPC.netUpdate = true;
				}

				NPC.ai[0] = AttackPattern[(int)NPC.ai[1]];

				switch ((int)NPC.ai[0])
                {
                    //solar flowers (transition for phase 2)
                    case -1:
                    {
                        break;
                    }

					//grow tons of pitcher plants to spray poison/venom upward
					case 0:
                    {
						NPC.localAI[0]++;

						if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 180 && NPC.localAI[0] % 15 == 0)
						{
							int VelocityX = Main.rand.Next(-12, 13);
							int VelocityY = Main.rand.Next(-15, -10);

							Vector2 RandomPosition = new Vector2(NPC.Top.X + Main.rand.Next(-(NPC.width / 2) + 15, (NPC.width / 2) - 15), NPC.Top.Y + 25);
							NPCGlobalHelper.ShootHostileProjectile(NPC, RandomPosition, new Vector2(VelocityX, VelocityY), 
							ModContent.ProjectileType<VineBase>(), NPC.damage, 4.5f, ai2: 0);
						}

						if (NPC.localAI[0] >= 360)
						{
							NPC.localAI[0] = 0;
							SwitchToNextAttack(player);

							NPC.netUpdate = true;
						}

                        break;
                    }

					//orchid seed attack
					case 1:
                    {
						NPC.localAI[0]++;

						if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 180 && NPC.localAI[0] % 20 == 0)
						{
							Vector2 RandomPosition = new Vector2(NPC.Top.X + Main.rand.Next(-(NPC.width / 2) + 15, (NPC.width / 2) - 15), NPC.Top.Y + 25);

							Vector2 ShootSpeed = player.Center - RandomPosition;
							ShootSpeed.Normalize();
							ShootSpeed *= 28;

							NPCGlobalHelper.ShootHostileProjectile(NPC, RandomPosition, ShootSpeed,
							ModContent.ProjectileType<VineBase>(), NPC.damage, 4.5f, ai2: 2);
						}

						if (NPC.localAI[0] >= 360)
						{
							NPC.localAI[0] = 0;
							SwitchToNextAttack(player);

							NPC.netUpdate = true;
						}

						break;
                    }

					//grow and shoot bouncing sunflowers
					case 2:
					{
						NPC.localAI[0]++;

						if (NPC.localAI[0] == 100)
						{
							for (int numProjectiles = -6; numProjectiles <= 6; numProjectiles++)
							{
								Vector2 NPCTopPos = new Vector2(NPC.Top.X + Main.rand.Next(-(NPC.width / 2) + 15, (NPC.width / 2) - 15), NPC.Top.Y + 25);

								NPCGlobalHelper.ShootHostileProjectile(NPC, NPCTopPos, 10f * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(12) * numProjectiles),
								ModContent.ProjectileType<VineBase>(), NPC.damage, 4.5f, ai2: 1);
							}
						}

						if (NPC.localAI[0] >= 490)
						{
							NPC.localAI[0] = 0;
							SwitchToNextAttack(player);

							NPC.netUpdate = true;
						}

						break;
					}

					//spawn earthworms
					case 3:
                    {
						//debug text until i come up with something
						Main.NewText("Fallback Attack 2");

						SwitchToNextAttack(player);

                        break;
                    }
                }
            }
        }
    }
}