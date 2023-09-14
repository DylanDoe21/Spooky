using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.NPCs.Hallucinations;
using Spooky.Content.Projectiles.Cemetery;
using Spooky.Content.Biomes;
using Spooky.Content.Tiles.Pylon;

namespace Spooky.Content.Items.BossSummon
{
    public class Horseshoe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
			Item.noMelee = true;
            Item.noUseGraphic = true;              
            Item.width = 26;
            Item.height = 30;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<HorseshoeProj>();
            Item.shootSpeed = 25f;
        }

        public override bool CanUseItem(Player player)
        {
            if (!Flags.encounteredHorse && !NPC.AnyNPCs(ModContent.NPCType<TheHorse>()))
            {
                return true;
            }

            if (player.ownedProjectileCounts[Item.shoot] > 0)
			{
				return false;
			}

            return true;
        }
		
        public override bool? UseItem(Player player)
        {
            if (!Flags.encounteredHorse)
            {
                int type = ModContent.NPCType<TheHorse>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else 
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (Flags.encounteredHorse)
            {
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            }

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddTile(TileID.Anvils)
            .AddCondition(CemeteryCondition.InCemeteryBiome)
            .Register();
        }
    }
}