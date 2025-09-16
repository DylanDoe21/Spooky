using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Tameable;

namespace Spooky.Content.Items
{
    public class PetCommander : ModItem
    {
        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/PetCommandWhistle", SoundType.Sound) { Volume = 0.5f };

        public override void SetDefaults()
        {                
            Item.width = 32;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = UseSound;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.rare = ItemRarityID.Blue;
        }

		public override bool? UseItem(Player player)
		{
			foreach (NPC npc in Main.ActiveNPCs)
			{
				Rectangle Hitbox = new Rectangle((int)(npc.Center.X - 30), (int)(npc.Center.Y - 30), 60, 60);
                if (Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) && npc.Distance(player.Center) <= 150f && npc.GetGlobalNPC<NPCGlobal>().NPCTamed)
                {
					npc.localAI[2]++;
                    if (npc.localAI[2] > 2)
                    {
                        npc.localAI[2] = 0;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0);
                        }
                    }

					int StateValue = (int)npc.localAI[2] + 1;

                    if (npc.type == ModContent.NPCType<Turkey>())
                    {
					    CombatText.NewText(npc.getRect(), Color.Lime, Language.GetTextValue("Mods.Spooky.NPCs.Turkey.State" + StateValue.ToString()), false);
                    }
                    if (npc.type == ModContent.NPCType<LittleDunk>())
                    {
					    CombatText.NewText(npc.getRect(), Color.Lime, Language.GetTextValue("Mods.Spooky.NPCs.LittleDunk.State" + StateValue.ToString()), false);
                    }
				}
            }

			return true;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}