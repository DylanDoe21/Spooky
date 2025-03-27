using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class CarrotWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
			Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true; 
            Item.width = 28;
            Item.height = 36;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item8;
			//Item.shoot = ModContent.ProjectileType<GhostPepperMinion>();
            //Item.shootSpeed = 0f;
        }
    }
}