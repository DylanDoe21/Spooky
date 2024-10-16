using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class OrroBodyWings : ModNPC
    {
		public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroboroBodyWings";

		public float WingRotationDegrees;

	    public bool wingFlap;
		public bool IsPhase1Segment;
		public bool IsHeadSegmentCharging;

        private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> Wing1Texture;
		private static Asset<Texture2D> Wing2Texture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Wing1Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing1");
			Wing2Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing2");

			if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
			{
				for (int i = 0; i < 360; i += 30)
				{
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

					Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
					spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.2f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, SpriteEffects.None, 0);

					Vector2 vector1 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);
					Vector2 vector2 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);

					float num = MathHelper.ToRadians(WingRotationDegrees);

					spriteBatch.Draw(Wing1Texture.Value, NPC.Center + circular - Main.screenPosition + vector1, null, color * 0.2f,
					NPC.rotation + num, new Vector2(0f, Wing1Texture.Height() / 2 + 15f), NPC.scale * 1.05f, SpriteEffects.None, 0f);

					spriteBatch.Draw(Wing2Texture.Value, NPC.Center + circular - Main.screenPosition + vector2, null, color * 0.2f,
					NPC.rotation - num, new Vector2(Wing2Texture.Width(), Wing1Texture.Height() / 2 + 15f), NPC.scale * 1.05f, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Wing1Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing1");
			Wing2Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing2");

			Vector2 vector1 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);
			Vector2 vector2 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);

			float num = MathHelper.ToRadians(WingRotationDegrees);

			spriteBatch.Draw(Wing1Texture.Value, NPC.Center - Main.screenPosition + vector1, null, NPC.GetAlpha(drawColor),
			NPC.rotation + num, new Vector2(0f, Wing1Texture.Height() / 2 + 15f), 1f, SpriteEffects.None, 0f);

			spriteBatch.Draw(Wing2Texture.Value, NPC.Center - Main.screenPosition + vector2, null, NPC.GetAlpha(drawColor),
			NPC.rotation - num, new Vector2(Wing2Texture.Width(), Wing1Texture.Height() / 2 + 15f), 1f, SpriteEffects.None, 0f);
		}

		public override bool PreAI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[3]];

            //kill segment if the head doesnt exist
			if (!Parent.active || (Parent.type != ModContent.NPCType<OrroHeadP1>() && Parent.type != ModContent.NPCType<OrroHead>() && Parent.type != ModContent.NPCType<BoroHead>()))
            {
				SpawnGores();

				NPC.active = false;
			}

			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
				NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				float dist = (length - (float)NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;

				NPC.velocity = Vector2.Zero;
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
			}

			return true;
		}

		//handle all of this npcs wing-flap variables in AI
		public override void AI()
		{
			IsPhase1Segment = NPC.type == ModContent.NPCType<OrroBodyWingsP1>() || NPC.type == ModContent.NPCType<BoroBodyWingsP1>();

			NPC Parent = Main.npc[(int)NPC.ai[3]];

			//based on the head segment that the wing segment belongs to, set the wings to fold back whenever the head segment this belongs to is using a charging attack
			//probably doesnt looks nice but whatever, it works
			if (Parent.active)
			{
				if (Parent.type == ModContent.NPCType<BoroHead>())
				{
					if (Parent.ai[0] == 0 && Parent.localAI[0] >= 60 && Parent.localAI[0] <= 120)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 2 && Parent.localAI[0] >= 90 && Parent.localAI[0] <= 170)
					{
						IsHeadSegmentCharging = true;
					}
					else
					{
						IsHeadSegmentCharging = false;
					}
				}

				if (Parent.type == ModContent.NPCType<OrroHead>())
				{
					if (Parent.ai[0] == 1 && Parent.localAI[0] >= 70 && Parent.localAI[0] <= 125)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 2 && Parent.localAI[0] >= 90 && Parent.localAI[0] <= 170)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 3 && Parent.localAI[0] >= 210 && Parent.localAI[0] <= 260)
					{
						IsHeadSegmentCharging = true;
					}
					else 
					{ 
						IsHeadSegmentCharging = false;
					}
				}

				if (Parent.type == ModContent.NPCType<OrroHeadP1>())
				{
					if (Parent.ai[0] == -1)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 0 && Parent.localAI[0] >= 190 && Parent.localAI[0] <= 230)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 1 && Parent.localAI[0] >= 90 && Parent.localAI[0] <= 150)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 2 && Parent.localAI[0] >= 100 && Parent.localAI[0] <= 130)
					{
						IsHeadSegmentCharging = true;
					}
					else if (Parent.ai[0] == 3 && Parent.localAI[0] >= 110 && Parent.localAI[0] <= 150)
					{
						IsHeadSegmentCharging = true;
					}
					else
					{
						IsHeadSegmentCharging = false;
					}
				}
			}

			//if the head segment is "charging" then set the wing rotation to go downward and stay downward
			if (IsHeadSegmentCharging)
			{
				wingFlap = true;

				WingRotationDegrees += 12f;
				if (WingRotationDegrees >= 70f)
				{
					WingRotationDegrees = 70f;
				}
			}
			else
			{
				if (!wingFlap)
				{
					WingRotationDegrees += 12f;
					if (WingRotationDegrees >= 70f)
					{
						wingFlap = true;
					}
				}
				if (wingFlap)
				{
					WingRotationDegrees -= WingRotationDegrees > 15f ? 2f : 3.5f;
					if (WingRotationDegrees <= -20f)
					{
						wingFlap = false;
					}
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                SpawnGores();
            }
        }

		public void SpawnGores()
		{
			//body gores
			for (int numGores = 1; numGores <= 4; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BodyGore" + Main.rand.Next(1, 3)).Type);
				}
			}

			//wings gores
			for (int numGores = 1; numGores <= 2; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WingGore" + numGores).Type);
				}
			}
			for (int numGores = 1; numGores <= 2; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WingGore" + numGores).Type);
				}
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }

    public class BoroBodyWings : OrroBodyWings
    {
		public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroboroBodyWings";
        
		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> Wing1Texture;
		private static Asset<Texture2D> Wing2Texture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Wing1Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing1");
			Wing2Texture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroWing2");

			if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
			{
				for (int i = 0; i < 360; i += 30)
				{
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

					Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
					spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.2f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, SpriteEffects.None, 0);

					Vector2 vector1 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);
					Vector2 vector2 = Utils.RotatedBy(new Vector2(0f, 0f), NPC.rotation, default);

					float num = MathHelper.ToRadians(WingRotationDegrees);

					spriteBatch.Draw(Wing1Texture.Value, NPC.Center + circular - Main.screenPosition + vector1, null, color * 0.2f,
					NPC.rotation + num, new Vector2(0f, Wing1Texture.Height() / 2 + 15f), NPC.scale * 1.05f, SpriteEffects.None, 0f);

					spriteBatch.Draw(Wing2Texture.Value, NPC.Center + circular - Main.screenPosition + vector2, null, color * 0.2f,
					NPC.rotation - num, new Vector2(Wing2Texture.Width(), Wing1Texture.Height() / 2 + 15f), NPC.scale * 1.05f, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }

    public class OrroBodyWingsP1 : OrroBodyWings
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroboroBodyWings";

		private static Asset<Texture2D> NPCTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

        public override bool PreAI()
        {
			NPC Parent = Main.npc[(int)NPC.ai[3]];

            //kill segment if the head doesnt exist
			if (!Parent.active || (Parent.type != ModContent.NPCType<OrroHeadP1>() && Parent.type != ModContent.NPCType<OrroHead>() && Parent.type != ModContent.NPCType<BoroHead>()))
            {
				NPC.active = false;
			}

            //go invulnerable and shake during phase 2 transition
            if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()))
            {
                if (Main.npc[(int)NPC.ai[1]].ai[2] > 0)
                {
                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    NPC.velocity *= 0f;

                    NPC.ai[2]++;

                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }
            }
			
			if (NPC.ai[1] < (double)Main.npc.Length)
            {
                Vector2 npcCenter = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float dirX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
                float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - (float)NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;
 
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }

			return true;
        }
    }

    public class BoroBodyWingsP1 : OrroBodyWingsP1
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroboroBodyWings";

		private static Asset<Texture2D> NPCTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }
    }
}
