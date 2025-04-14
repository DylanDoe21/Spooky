using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class RadishYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
        }

        public override void SetDefaults()
        {
			Item.damage = 22;
			Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.width = 62;          
			Item.height = 44;
            Item.useTime = 22;
            Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<RadishYoyoProj>();
            Item.shootSpeed = 8f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}