using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.SpookyHell.Projectiles
{
	public class ValleySharkMouth : ModNPC
	{
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 22;
            NPC.height = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<ValleyShark>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/Projectiles/ValleySharkMouthChain");
                
                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y + 3);
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            Player player = Main.player[Parent.target];

            if (NPC.ai[0] <= 25)
            {
                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<ValleyShark>())
            {
                NPC.active = false;
            }

            //set mouth to retract immediately when it hits the targetted player
            if (NPC.Hitbox.Intersects(player.Hitbox))
            {
                if (NPC.ai[1] == 0)
                {
                    NPC.ai[0] = 24;
                    NPC.ai[1] = 1;
                }
            }

            //if the mouth is grappled onto the player
            if (NPC.ai[1] == 1)
            {
                player.Center = NPC.Center;
            }

            NPC.ai[0]++;

            if (NPC.ai[0] == 1)
            {
                Vector2 ChargeSpeed = player.Center - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 22;
                NPC.velocity = ChargeSpeed;
            }

            if (NPC.ai[0] == 25)
            {
                NPC.localAI[0] = NPC.rotation;
                NPC.localAI[1] = NPC.direction;
            }

            if (NPC.ai[0] > 25)
            {
                NPC.rotation = NPC.localAI[0];
                NPC.direction = (int)NPC.localAI[1];

                Vector2 ChargeSpeed = Parent.Center - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 22;
                NPC.velocity = ChargeSpeed;

                if (NPC.Distance(Parent.Center) <= 30f)
                {
                    //if the mouth is grappled onto the player, then set the valley shark ai to immediately do its slam attack
                    if (NPC.ai[1] == 1)
                    {
                        Parent.localAI[0] = 1;
                        Parent.localAI[1] = 340;
                        Parent.localAI[2] = 0;
                    }

                    NPC.active = false;
                }
            }
		}
	}
}