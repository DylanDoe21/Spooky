using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class EggplantSpawner : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
			NPC.width = 20;
			NPC.height = 20;
			NPC.knockBackResist = 0f;
			NPC.npcSlots = 0;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
			NPC.dontCountMe = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override bool CheckActive()
        {
            return false;
        }

		public static bool SolidTile(int i, int j)
		{
			return Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
		}

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            switch ((int)NPC.ai[0])
            {
                //fly upward until it hits a valid ceiling 
                case 0:
				{
					NPC.ai[1]++;

					if (NPC.ai[1] == 1)
					{
						NPC.velocity.Y -= 10;
					}

					if (NPC.ai[1] > 5 && IsColliding())
					{
						NPC.ai[0]++;
					}

                    break;
                }

                //spawn the eggplant once the ceiling is found
                case 1: 
                {
                    if (NPC.ai[1] < 15)
                    {
                        NPC.active = false;
                    }

                    if (NPC.ai[1] >= 15)
                    {
                        int EggPlant = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 3, ModContent.NPCType<EggplantBase>());

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {   
                            NetMessage.SendData(MessageID.SyncNPC, number: EggPlant);
                        }

                        NPC.netUpdate = true;
                        NPC.active = false;
                    }

                    break;
                }
            }
        }

		public bool IsColliding()
		{
			int minTilePosX = (int)(NPC.position.X / 16) - 1;
			int maxTilePosX = (int)((NPC.position.X + NPC.width) / 16) + 2;
			int minTilePosY = (int)(NPC.position.Y / 16) - 1;
			int maxTilePosY = (int)((NPC.position.Y + NPC.height) / 16) + 2;
			if (minTilePosX < 0)
			{
				minTilePosX = 0;
			}
			if (maxTilePosX > Main.maxTilesX)
			{
				maxTilePosX = Main.maxTilesX;
			}
			if (minTilePosY < 0)
			{
				minTilePosY = 0;
			}
			if (maxTilePosY > Main.maxTilesY)
			{
				maxTilePosY = Main.maxTilesY;
			}

			for (int i = minTilePosX; i < maxTilePosX; ++i)
			{
				for (int j = minTilePosY; j < maxTilePosY; ++j)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && !Main.tile[i, j].IsActuated &&
					Main.tileSolid[(int)Main.tile[i, j].TileType] && !TileID.Sets.Platforms[(int)Main.tile[i, j].TileType])
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);

						if (NPC.position.X + NPC.width > vector2.X && NPC.position.X < vector2.X + 16.0 &&
						(NPC.position.Y + NPC.height > (double)vector2.Y && NPC.position.Y < vector2.Y + 16.0))
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
