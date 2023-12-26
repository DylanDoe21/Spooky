using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class PheromoneWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 42;
            Item.height = 42;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 12);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<PheromoneWhipProj>();
			Item.shootSpeed = 3f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}
    }
}